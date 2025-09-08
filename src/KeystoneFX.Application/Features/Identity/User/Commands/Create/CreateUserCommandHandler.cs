using AutoMapper;
using KeystoneFX.Application.Common.Commands.Handlers;
using KeystoneFX.Application.Features.Identity.User.Dtos;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
#region using Entities
using UserEntity = KeystoneFX.Domain.Identity.User;
using RoleEntity = KeystoneFX.Domain.Identity.Role;
#endregion

namespace KeystoneFX.Application.Features.Identity.User.Commands.Create;

public sealed class CreateUserCommandHandler
    : CreateCommandHandlerBase<CreateUserCommand, UserCreateDto, UserEntity, Guid>
{
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly ILookupNormalizer _normalizer;
    private readonly UserManager<UserEntity> _userManager;
    private readonly RoleManager<RoleEntity> _roleManager;

    public CreateUserCommandHandler(
        IWriteRepository<UserEntity, Guid> repo,
        IUnitOfWork uow,
        IMapper mapper,
        IPasswordHasher<UserEntity> passwordHasher,
        ILookupNormalizer normalizer,
        UserManager<UserEntity> userManager,
        RoleManager<RoleEntity> roleManager)
        : base(repo, uow, mapper)
    {
        _passwordHasher = passwordHasher;
        _normalizer = normalizer;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    protected override Task OnBeforeAddAsync(UserEntity entity, CreateUserCommand request, CancellationToken ct)
    {
        // Normalize + security fields
        entity.SecurityStamp = Guid.NewGuid().ToString("N");
        entity.ConcurrencyStamp = Guid.NewGuid().ToString("N");

        if (!string.IsNullOrWhiteSpace(entity.UserName))
            entity.NormalizedUserName = _normalizer.NormalizeName(entity.UserName);

        if (!string.IsNullOrWhiteSpace(entity.Email))
            entity.NormalizedEmail = _normalizer.NormalizeEmail(entity.Email);

        // Hash password
        entity.PasswordHash = _passwordHasher.HashPassword(entity, request.Model.Password);

        // Defaults
        entity.EmailConfirmed = false;
        entity.PhoneNumberConfirmed = false;
        entity.TwoFactorEnabled = false;
        entity.LockoutEnabled = true;

        return Task.CompletedTask;
    }

    protected override async Task OnAfterAddAsync(UserEntity entity, CreateUserCommand request, CancellationToken ct)
    {
        // Roles
        if (request.Model.Roles is { Count: > 0 })
        {
            foreach (var roleName in request.Model.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                    throw new InvalidOperationException($"Role '{roleName}' does not exist.");
            }

            var r = await _userManager.AddToRolesAsync(entity, request.Model.Roles);
            if (!r.Succeeded)
                throw new InvalidOperationException(string.Join("; ", r.Errors.Select(e => e.Description)));
        }

        // Claims
        if (request.Model.Claims is { Count: > 0 })
        {
            var claims = request.Model.Claims.Select(c => new Claim(c.Type, c.Value));
            var r = await _userManager.AddClaimsAsync(entity, claims);
            if (!r.Succeeded)
                throw new InvalidOperationException(string.Join("; ", r.Errors.Select(e => e.Description)));
        }

        await Uow.SaveChangesAsync(true, ct);
    }
}