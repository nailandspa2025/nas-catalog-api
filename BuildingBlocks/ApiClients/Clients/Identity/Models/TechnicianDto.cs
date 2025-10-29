namespace BuildingBlocks.ApiClients.Clients.Identity.Models;

public class TechnicianDto
{
    public long Id { get; set; }

    public string TechnicianName { get; set; } = null!;

    public string? TechnicianAddress { get; set; }

    public string Phone { get; set; } = null!;

    public string? Avatar { get; set; }
}
