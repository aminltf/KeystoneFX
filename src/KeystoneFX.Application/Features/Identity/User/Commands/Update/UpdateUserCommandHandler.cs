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

namespace KeystoneFX.Application.Features.Identity.User.Commands.Update;

public class UpdateUserCommandHandler
    : UpdateCommandHandlerBase<UpdateUserCommand, UserUpdateDto, UserEntity, Guid>
{
    private readonly ILookupNormalizer _normalizer;
    private readonly UserManager<UserEntity> _userManager;
    private readonly RoleManager<RoleEntity> _roleManager;

    public UpdateUserCommandHandler(
        IWriteRepository<UserEntity, Guid> repo,
        IUnitOfWork uow,
        IMapper mapper,
        ILookupNormalizer normalizer,
        UserManager<UserEntity> userManager,
        RoleManager<RoleEntity> roleManager)
        : base(repo, uow, mapper)
    {
        _normalizer = normalizer;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    protected override Task OnNotFoundAsync(Guid id, CancellationToken ct)
        => throw new KeyNotFoundException($"User '{id}' not found.");

    protected override Task OnBeforeMapAsync(UserEntity entity, UserUpdateDto dto, UpdateUserCommand request, CancellationToken ct)
    {
        if (dto.UserName is not null)
            entity.NormalizedUserName = _normalizer.NormalizeName(dto.UserName);

        if (dto.Email is not null)
            entity.NormalizedEmail = _normalizer.NormalizeEmail(dto.Email);

        return Task.CompletedTask;
    }

    protected override async Task OnAfterUpdateAsync(UserEntity entity, int affectedRows, UpdateUserCommand request, CancellationToken ct)
    {
        // Roles sync (only if provided)
        if (request.Model.Roles is not null)
        {
            // Validity check
            foreach (var roleName in request.Model.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                    throw new InvalidOperationException($"Role '{roleName}' does not exist.");
            }

            var current = await _userManager.GetRolesAsync(entity);
            var target = request.Model.Roles;

            var toAdd = target.Except(current, StringComparer.OrdinalIgnoreCase).ToArray();
            var toRemove = current.Except(target, StringComparer.OrdinalIgnoreCase).ToArray();

            if (toRemove.Length > 0)
            {
                var r = await _userManager.RemoveFromRolesAsync(entity, toRemove);
                if (!r.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", r.Errors.Select(e => e.Description)));
            }

            if (toAdd.Length > 0)
            {
                var r = await _userManager.AddToRolesAsync(entity, toAdd);
                if (!r.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", r.Errors.Select(e => e.Description)));
            }
        }

        // Claims sync (only if provided)
        if (request.Model.Claims is not null)
        {
            var currentClaims = await _userManager.GetClaimsAsync(entity);

            if (currentClaims.Count > 0)
            {
                var r1 = await _userManager.RemoveClaimsAsync(entity, currentClaims);
                if (!r1.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", r1.Errors.Select(e => e.Description)));
            }

            if (request.Model.Claims.Count > 0)
            {
                var newClaims = request.Model.Claims.Select(c => new Claim(c.Type, c.Value)).ToArray();
                var r2 = await _userManager.AddClaimsAsync(entity, newClaims);
                if (!r2.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", r2.Errors.Select(e => e.Description)));
            }
        }

        await Uow.SaveChangesAsync(true, ct);
    }
}