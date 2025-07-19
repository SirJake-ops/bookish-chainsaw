namespace BackendTracker.Ticket;

public class TicketResponse
{
    public Guid TicketId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StepsToReproduce { get; set; } = string.Empty;
    public string ExpectedResult { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public Guid SubmitterId { get; set; }
    public DateTime CreatedAt { get; set; }
}