using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.FileStorage;
using BuildingBlocks.Core.Response;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Features.Merchants.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Merchants.Commads.UpdateMerchant;

public record UpdateMerchantCommand: IRequest<ApiResponse<MerchantDto>>
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public string? ShortName { get; init; }

    public string? TaxCode { get; init; }

    public string? ContractNumber { get; init; }

    public DateTime? ContractDate { get; init; }

    public TimeSpan StartTime { get; init; }

    public TimeSpan EndTime { get; init; }

    public MerchantType Type { get; init; }

    public string? ZaloOA { get; init; }

    public string? Fanpage { get; init; }

    public string? Website { get; init; }

    public string? Address { get; init; }

    public string? Represent { get; init; }

    public string? Email { get; init; }

    public string? PhoneNumber { get; init; }

    public IFormFile? Logo { get; init; }

    public bool IsLogo { get; init; }

    public int? ServicePackageId { get; init; } = null;

    public bool IsActive { get; init; }

    public List<IFormFile> Images { get; init; } = new List<IFormFile>();

    public List<UpdateBrandModel>? Brands { get; init; } = new List<UpdateBrandModel>();

    public List<string> LinkUrls { get; init; } = new List<string>();

    public List<int> WeekdayOffs { get; set; } = new List<int>();

    public DateTime? DeploymentDate { get; init; }

    public string? ContactPhoneNumber { get; init; }
}

public record UpdateBrandModel
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public IFormFile? Logo { get; init; }

    public string? LogoLink { get; set; }
}

public class UpdateMerchantCommandHandler : IRequestHandler<UpdateMerchantCommand, ApiResponse<MerchantDto>>
{
    private readonly ICatalogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public UpdateMerchantCommandHandler(ICatalogDbContext context, IMapper mapper, IStorageService storageService)
    {
        _context = context;
        _mapper = mapper;
        _storageService = storageService;
    }

    public  async Task<ApiResponse<MerchantDto>> Handle(UpdateMerchantCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Merchant
           .Include(x => x.MerchantContractImages)
           .Include(x => x.Brands)
           .Include(x => x.MerchantWeekdayOffs)
           .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(MerchantDto), request.Id);
        }
        entity.Name = request.Name;
        entity.ShortName = request.ShortName;
        entity.TaxCode = request.TaxCode;
        entity.ContractNumber = request.ContractNumber;
        entity.ContractDate = request.ContractDate;
        entity.StartTime = request.StartTime;
        entity.EndTime = request.EndTime;
        entity.Type = request.Type;
        entity.ZaloOA = request.ZaloOA;
        entity.Fanpage = request.Fanpage;
        entity.Website = request.Website;
        entity.Address = request.Address;
        entity.Represent = request.Represent;
        entity.Email = request.Email;
        entity.PhoneNumber = request.PhoneNumber;
        entity.IsActive = request.IsActive;
        entity.ServicePackageId = request.ServicePackageId;
        entity.DeploymentDate = request.DeploymentDate;
        entity.ContactPhoneNumber = request.ContactPhoneNumber;

        if (request.Logo != null && request.Logo.Length > 0)
        {
            var imageUrl = await _storageService.SaveFileAsync(request.Logo, cancellationToken);
            if (!string.IsNullOrEmpty(entity.Logo))
                await _storageService.DeleteFileAsync(entity.Logo, cancellationToken);
            entity.Logo = imageUrl;
        }
        else if (request.IsLogo)
        {
            if (!string.IsNullOrEmpty(entity.Logo))
                await _storageService.DeleteFileAsync(entity.Logo, cancellationToken);
            entity.Logo = null;
        }
        var oldImageUrls = entity.MerchantContractImages.Select(g => g.Url).Where(url => !request.LinkUrls.Contains(url)).ToList();
        if (oldImageUrls.Any())
        {
            await _storageService.DeleteFileAsync(oldImageUrls, cancellationToken);
        }
        var updatedImageUrls = new List<string>();
        {
            var imageUrls = await _storageService.SaveFilesAsync(request.Images, cancellationToken);
            updatedImageUrls.AddRange(imageUrls);
        }
        if (request.LinkUrls != null && request.LinkUrls.Any())
        {
            updatedImageUrls.AddRange(request.LinkUrls);
        }
        entity.SetContractImages(
        updatedImageUrls.Select(url => new MerchantContractImage
        {
            Url = url
        }).ToList()
        );
        if (request.WeekdayOffs.Any())
        {
            var weekdayOffs = request.WeekdayOffs.Select(p => new MerchantWeekdayOff
            {
                WeekdayOff = p
            }).ToList();
            entity.SetWeekdayOffs(weekdayOffs);
        }
        var brands = new List<Brand>();
        if (request.Brands != null && request.Brands.Any())
        {
            
            foreach (var brand in request.Brands)
            {
                string logoUrl = string.Empty;
                if (brand.Logo != null && brand.Logo.Length > 0)
                {
                    if (!string.IsNullOrEmpty(brand.LogoLink))
                        await _storageService.DeleteFileAsync(brand.LogoLink, cancellationToken);
                    var imageUrl = await _storageService.SaveFileAsync(brand.Logo, cancellationToken);
                    logoUrl = imageUrl;
                }
                else if (!string.IsNullOrWhiteSpace(brand.LogoLink))
                {
                    logoUrl = brand.LogoLink;
                }
                var newBrand = new Brand
                {
                    Name = brand.Name,
                    Logo = logoUrl,
                };
                brands.Add(newBrand);
            }
        }
        entity.SetBrands(brands);
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<MerchantDto>.Success(_mapper.Map<MerchantDto>(entity));
    }
}
