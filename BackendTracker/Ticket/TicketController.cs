using EntityGraphQL.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendTracker.Ticket;

[ApiController]
[Route("api/tickets")]
public class TicketController(TicketService ticketService) : Controller
{
    [HttpGet]
    [Authorize]
    public async Task<IEnumerable<Ticket>> GetTickets([FromQuery] Guid submitterId)
    {
        return await ticketService.GetTickets(submitterId);
    }

    [HttpPost]
    [Authorize]
    public async Task<TicketResponse> CreateTicket([FromBody] TicketRequestBody ticketBody)
    {
        return await ticketService.CreateTicket(ticketBody);
    }

    [HttpPut]
    [Authorize]
    public async Task<TicketResponse> UpdateTicket([FromBody] TicketRequestBody ticketBody)
    {
        return await ticketService.UpdateTicket(ticketBody);
    }

    [HttpPatch]
    [Authorize]
    public async Task<TicketResponse> UpdateTicketProp([FromBody] Dictionary<string, AnyType> propertyToUpdate)
    {
        throw new NotImplementedException("This is gonna be interesting to accomplish");
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteTicket([FromQuery] Guid ticketId)
    {
        await ticketService.DeleteTicket(ticketId);
        return NoContent();
    }
}