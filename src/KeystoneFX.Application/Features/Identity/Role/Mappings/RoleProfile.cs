using AutoMapper;
using KeystoneFX.Application.Features.Identity.Role.Dtos;
using RoleEntity = KeystoneFX.Domain.Identity.Role;

namespace KeystoneFX.Application.Features.Identity.Role.Mappings;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        // Create
        CreateMap<RoleCreateDto, RoleEntity>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore())
            .ForMember(d => d.ConcurrencyStamp, o => o.Ignore())
            .ForMember(d => d.NormalizedName, o => o.Ignore())
            // audit
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.CreatedOnUtc, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOnUtc, o => o.Ignore())
            // navigations
            .ForMember(d => d.UserRoles, o => o.Ignore())
            .ForMember(d => d.RoleClaims, o => o.Ignore());

        // Update
        CreateMap<RoleUpdateDto, RoleEntity>()
            .ForMember(d => d.RowVersion, o => o.MapFrom(s => s.RowVersion))
            .ForMember(d => d.Name, o => o.Ignore())
            .ForMember(d => d.NormalizedName, o => o.Ignore())
            .ForMember(d => d.ConcurrencyStamp, o => o.Ignore())
            // navigations
            .ForMember(d => d.UserRoles, o => o.Ignore())
            .ForMember(d => d.RoleClaims, o => o.Ignore())
            .ForAllMembers(opt => opt.Condition((src, _, srcMember) => srcMember != null));
    }
}