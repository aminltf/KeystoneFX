using AutoMapper;
using KeystoneFX.Application.Features.Identity.User.Dtos;
using UserEntity = KeystoneFX.Domain.Identity.User;

namespace KeystoneFX.Application.Features.Identity.User.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Create
        CreateMap<UserCreateDto, UserEntity>()
            // keys & concurrency
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore())
            .ForMember(d => d.ConcurrencyStamp, o => o.Ignore())
            .ForMember(d => d.SecurityStamp, o => o.Ignore())
            // normalizeds & hashes (set in handler)
            .ForMember(d => d.NormalizedUserName, o => o.Ignore())
            .ForMember(d => d.NormalizedEmail, o => o.Ignore())
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            // identity internals
            .ForMember(d => d.EmailConfirmed, o => o.Ignore())
            .ForMember(d => d.PhoneNumberConfirmed, o => o.Ignore())
            .ForMember(d => d.AccessFailedCount, o => o.Ignore())
            .ForMember(d => d.LockoutEnabled, o => o.Ignore())
            .ForMember(d => d.LockoutEnd, o => o.Ignore())
            .ForMember(d => d.TwoFactorEnabled, o => o.Ignore())
            // audit & soft delete
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.CreatedOnUtc, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOnUtc, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DeletedBy, o => o.Ignore())
            .ForMember(d => d.DeletedOnUtc, o => o.Ignore())
            .ForMember(d => d.DeletionReason, o => o.Ignore())
            // navigations
            .ForMember(d => d.UserRoles, o => o.Ignore())
            .ForMember(d => d.UserClaims, o => o.Ignore())
            .ForMember(d => d.UserLogins, o => o.Ignore())
            .ForMember(d => d.UserTokens, o => o.Ignore())
            .ForMember(d => d.RefreshTokens, o => o.Ignore());

        // Update (patch semantics)
        CreateMap<UserUpdateDto, UserEntity>()
            .ForMember(d => d.RowVersion, o => o.MapFrom(s => s.RowVersion))
            .ForMember(d => d.NormalizedUserName, o => o.Ignore())
            .ForMember(d => d.NormalizedEmail, o => o.Ignore())
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.ConcurrencyStamp, o => o.Ignore())
            .ForMember(d => d.SecurityStamp, o => o.Ignore())
            // navigations
            .ForMember(d => d.UserRoles, o => o.Ignore())
            .ForMember(d => d.UserClaims, o => o.Ignore())
            .ForMember(d => d.UserLogins, o => o.Ignore())
            .ForMember(d => d.UserTokens, o => o.Ignore())
            .ForMember(d => d.RefreshTokens, o => o.Ignore())
            .ForAllMembers(opt => opt.Condition((src, _, srcMember) => srcMember != null));
    }
}