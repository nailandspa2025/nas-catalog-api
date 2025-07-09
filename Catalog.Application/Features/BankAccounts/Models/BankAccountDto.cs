using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.BankAccounts.Models;

public class BankAccountDto: BaseAuditableDto
{
    public int Id { get; set; }
    public string AccountName { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;
    public string BankName { get; set; } = null!;
    public string BranchName { get; set; } = null!;
    public string? SwiftCode { get; set; }
    public string? CurrencyCode { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BankAccount, BankAccountDto>();
        }
    }
}
