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
        var context1 = await context.CreateDbContextAsync();
        try
        {
            var tickets = await context1.Tickets.Where(m => m.SubmitterId == submitterId || m.AssigneeId == submitterId)
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
        var context1 = await context.CreateDbContextAsync();
        try
        {
            ApplicationUser submitter = await context1.ApplicationUsers
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
                Files = ticketBody.Files
            };

            context1.Tickets.Add(ticket);
            await context1.SaveChangesAsync();

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
            throw new Exception("An error occurred while creating the ticket.", ex);
        }
    }
}