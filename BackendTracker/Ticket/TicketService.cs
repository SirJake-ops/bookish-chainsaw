using System.Reflection;
using BackendTracker.Context;
using BackendTracker.Entities.ApplicationUser;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendTracker.Ticket;

public class TicketService(IDbContextFactory<ApplicationContext> context)
{
    public async Task<IEnumerable<Ticket>> GetTickets(Guid submitterId)
    {
        var methodContext = await context.CreateDbContextAsync();
        try
        {
            var tickets = await methodContext.Tickets
                .Where(m => m.SubmitterId == submitterId || m.AssigneeId == submitterId)
                .ToListAsync();
            return tickets;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving tickets.", ex);
        }
    }

    public async Task<TicketResponse> CreateTicket(TicketRequestBody ticketBody)
    {
        var methodContext = await context.CreateDbContextAsync();
        try
        {
            ApplicationUser submitter = await methodContext.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == ticketBody.SubmitterId) ?? throw new Exception("Submitter not found");

            var ticket = new Ticket
            {
                Environment = ticketBody.Environment,
                Title = ticketBody.Title,
                Description = ticketBody.Description,
                StepsToReproduce = ticketBody.StepsToReproduce,
                ExpectedResult = ticketBody.ExpectedResult,
                SubmitterId = ticketBody.SubmitterId,
                AssigneeId = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TicketId = Guid.NewGuid(),
                Submitter = submitter,
                Assignee = null,
                Files = ticketBody.Files,
                IsResolved = ticketBody.IsResolved
            };

            methodContext.Tickets.Add(ticket);
            await methodContext.SaveChangesAsync();

            var ticketResponse = new TicketResponse
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                StepsToReproduce = ticket.StepsToReproduce,
                ExpectedResult = ticket.ExpectedResult,
                Environment = ticket.Environment.ToString(),
                SubmitterId = ticket.SubmitterId,
                CreatedAt = ticket.CreatedAt,
                IsResolved = ticket.IsResolved
            };

            return ticketResponse;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while creating the ticket.", ex);
        }
    }

    public async Task<TicketResponse> UpdateTicket(TicketRequestBody ticketBody)
    {
        var methodContext = await context.CreateDbContextAsync();
        try
        {
            Ticket dbTicket =
                await methodContext.Tickets.FirstOrDefaultAsync(m => m.SubmitterId == ticketBody.SubmitterId) ??
                throw new Exception("Ticket not found");
            ApplicationUser user =
                await methodContext.ApplicationUsers.FirstOrDefaultAsync(m => m.Id == ticketBody.SubmitterId) ??
                throw new Exception("User for ticket not found.");

            Ticket ticket = new Ticket
            {
                SubmitterId = user.Id,
                TicketId = dbTicket.TicketId,
                AssigneeId = null,
                Environment = ticketBody.Environment,
                StepsToReproduce = ticketBody.StepsToReproduce,
                ExpectedResult = ticketBody.ExpectedResult,
                Assignee = user,
                Description = ticketBody.Description,
                Title = ticketBody.Title,
                Files = ticketBody.Files,
                Submitter = user,
                IsResolved = ticketBody.IsResolved
            };

            methodContext.Tickets.Update(ticket);
            await methodContext.SaveChangesAsync();

            var ticketResponse = new TicketResponse
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                StepsToReproduce = ticket.StepsToReproduce,
                ExpectedResult = ticket.ExpectedResult,
                Environment = ticket.Environment.ToString(),
                SubmitterId = ticket.SubmitterId,
                CreatedAt = ticket.CreatedAt
            };
            return ticketResponse;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the ticket.", ex);
        }
    }

    public async Task<TicketResponse> UpdateTicketProp(Dictionary<string, object> propertyToUpdate, Guid ticketId)
    {
        var methodContext = await context.CreateDbContextAsync();
        try
        {
            var ticketToPatch = await methodContext.Tickets.FirstOrDefaultAsync(m => m.TicketId == ticketId) ??
                                throw new Exception("Ticket could not be found.");


            foreach (var (key, val) in propertyToUpdate)
            {
                PropertyInfo? propertyInfo = ticketToPatch.GetType().GetProperty(key);
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    object? convertedValue = ConvertValue(val, propertyInfo.PropertyType);
                    propertyInfo.SetValue(ticketToPatch, convertedValue);
                }
            }

            ticketToPatch.UpdatedAt = DateTime.UtcNow;

            await methodContext.SaveChangesAsync();

            return new TicketResponse
            {
                TicketId = ticketToPatch.TicketId,
                Title = ticketToPatch.Title,
                Description = ticketToPatch.Description,
                StepsToReproduce = ticketToPatch.StepsToReproduce,
                ExpectedResult = ticketToPatch.ExpectedResult,
                Environment = ticketToPatch.Environment.ToString(),
                SubmitterId = ticketToPatch.SubmitterId,
                CreatedAt = ticketToPatch.CreatedAt
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Property could not be updated.");
        }
    }

    public async Task<Result<Object>> DeleteTicket(Guid ticketId)
    {
        var methodContext = await context.CreateDbContextAsync();

        try
        {
            var ticket = await methodContext.Tickets.FirstOrDefaultAsync(m => m.TicketId == ticketId) ??
                         throw new Exception("Ticket was not found.");

            var user = await methodContext.ApplicationUsers.Include(u => u.SubmittedTickets)
                .FirstOrDefaultAsync(u => u.Id == ticket.TicketId);

            user?.SubmittedTickets.Remove(ticket);
            methodContext.Tickets.Remove(ticket);
            await methodContext.SaveChangesAsync();

            return new OkObjectResult(new { message = "Delete went through, ticket removed." });
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new
            {
                message = "An error occurred while deleting the ticket.",
                details = ex.Message
            });
        }
    }

    private static object? ConvertValue(object value, Type targetType)
    {
        if (value == null)
            return null;


        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        try
        {
            if (underlyingType.IsEnum)
            {
                if (value is string str)
                    return Enum.Parse(underlyingType, str, ignoreCase: true);
                else
                    return Enum.ToObject(underlyingType, value);
            }

            if (underlyingType == typeof(Guid))
            {
                if (value is string str)
                    return Guid.Parse(str);
                if (value is Guid g)
                    return g;
            }

            if (underlyingType == typeof(DateTime))
            {
                if (value is string str)
                    return DateTime.Parse(str);
                if (value is DateTime dt)
                    return dt;
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to convert value '{value}' to type '{targetType.Name}'", ex);
        }

        return Convert.ChangeType(value, underlyingType);
    }
}