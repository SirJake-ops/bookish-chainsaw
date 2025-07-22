using System.ComponentModel.DataAnnotations.Schema;
using BackendTracker.Entities;
using BackendTracker.Entities.ApplicationUser;
using BackendTracker.Ticket.FileUpload;

namespace BackendTracker.Ticket.Email;

public class Email : BaseEntity
{
    public required Guid EmailTicketId { get; set; }
    [ForeignKey(nameof(Submitter))] public required Guid SubmitterId { get; set; }
    public required ApplicationUser Submitter { get; set; } = null!;
    public required string Title { get; set; }
    public required string Description { get; set; }

    public required List<TicketFile> Files { get; set; } = new();
}