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

    /***TODO: Things for tomorrow - Look into how we are updating the ticket, should we just pass in a single value, or pass the entire ticket.
    */
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

    public async Task DeleteTicket(Guid ticketId)
    {
        throw new NotImplementedException();
    }
}