using AutoMapper;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.UserStores.Models;

public class UserStoreDto
{
    public long Id { get; set; }

    public string? UserId { get; set; }

    public long StoreId { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UserStore, UserStoreDto>();
        }
    }
}

