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
    public async Task<ActionResult<TicketResponse>> CreateTicket([FromBody] TicketRequestBody ticketBody)
    {
        return await ticketService.CreateTicket(ticketBody);
    } 
}