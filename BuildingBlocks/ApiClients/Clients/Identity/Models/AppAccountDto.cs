namespace BuildingBlocks.ApiClients.Clients.Identity.Models;

public class AppAccountDto
{
	public int Id { get; set; }

    public string FullName { get; set; } 

    public string Email { get; set; }

    public string Phone { get; set; }

    public string? Avatar { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public int Gender { get; set; }
}

