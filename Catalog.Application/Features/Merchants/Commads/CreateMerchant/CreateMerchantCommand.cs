using AutoMapper;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Merchants.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Catalog.Application.Features.Merchants.Commads.CreateMerchant;

public record CreateMerchantCommand: IRequest<ApiResponse<MerchantDto>>
{
    public string Name { get; init; } = null!;

    public string? ShortName { get; init; }

    public string? TaxCode { get; init; }

    public string? ContractNumber { get; init; }

    public DateTime? ContractDate { get; init; }

    public TimeSpan StartTime { get; init; }

    public TimeSpan EndTime { get; init; }

    public MerchantType Type { get; init; } = MerchantType.None;

    public string? ZaloOA { get; init; }

    public string? Fanpage { get; init; }

    public string? Website { get; init; }

    public string? Address { get; init; }

    public string? Represent { get; init; }

    public string? Email { get; init; }

    public string? PhoneNumber { get; init; }

    public IFormFile? Logo { get; init; }

    public int? ServicePackageId { get; init; } = null;

    public bool IsActive { get; init; }

    public string? ContactPhoneNumber { get; init; }

    public List<int> WeekdayOffs { get; init; } = new List<int>();

    public List<IFormFile> Images { get; init; } = new List<IFormFile>();

    public List<CreateBrandModel>? Brands { get; init; } = new List<CreateBrandModel>();

    public DateTime? DeploymentDate { get; init; }
}

public record CreateBrandModel
{
    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public IFormFile? Logo { get; init; }
}

public class CreateMerchantCommandHandler : IRequestHandler<CreateMerchantCommand, ApiResponse<MerchantDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public CreateMerchantCommandHandler(ICatalogDbContext context, IMapper mapper, IStorageService storageService)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }

    public async Task<ApiResponse<MerchantDto>> Handle(CreateMerchantCommand request, CancellationToken cancellationToken)
    {
        var entity = new Merchant
        {
            Name = request.Name,
            ShortName = request.ShortName,
            TaxCode = request.TaxCode,
            ContractNumber = request.ContractNumber,
            ContractDate = request.ContractDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Type = request.Type,
            ZaloOA = request.ZaloOA,
            Fanpage = request.Fanpage,
            Website = request.Website,
            Address = request.Address,
            Represent = request.Represent,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            ServicePackageId = request.ServicePackageId,
            IsActive = request.IsActive,
            ContactPhoneNumber = request.ContactPhoneNumber,
            DeploymentDate = request.DeploymentDate
        };
        if (request.Logo != null && request.Logo.Length > 0)
        {
            var imageUrl = await _storageService.SaveFileAsync(request.Logo, cancellationToken);
            entity.Logo = imageUrl;
        }
        if (request.WeekdayOffs.Any())
        {
            var weekdayOffs = request.WeekdayOffs.Select(p => new MerchantWeekdayOff
            {
                WeekdayOff = p
            }).ToList();
            entity.SetWeekdayOffs(weekdayOffs);
        }
        if (request.Images.Any())
        {
            var imageUrls = await _storageService.SaveFilesAsync(request.Images, cancellationToken);
            var contractImages = imageUrls.Select(p => new MerchantContractImage
            {
                Url = p
            }).ToList();
            entity.SetContractImages(contractImages);
        }
        if (request.Brands != null && request.Brands.Any())
        {
            var brands = new List<Brand>();
            foreach (var brandRequest in request.Brands)
            {
                var brand = new Brand
                {
                    Name = brandRequest.Name,
                    Description = brandRequest.Description,
                    Merchant = entity
                };
                if (brandRequest.Logo != null && brandRequest.Logo.Length > 0)
                {
                    var imageUrl = await _storageService.SaveFileAsync(brandRequest.Logo, cancellationToken);
                    brand.Logo = imageUrl;
                }
                brands.Add(brand);
            }

            entity.SetBrands(brands);
        }
        _context.Merchant.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<MerchantDto>.Success(_mapper.Map<MerchantDto>(entity));
    }
}
