using AutoMapper;
using KeystoneFX.Application.Common.Commands.Handlers;
using KeystoneFX.Application.Common.Security;
using KeystoneFX.Application.Features.Identity.Role.Dtos;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using RoleEntity = KeystoneFX.Domain.Identity.Role;

namespace KeystoneFX.Application.Features.Identity.Role.Commands.Update;

public class UpdateRoleCommandHandler
    : UpdateCommandHandlerBase<UpdateRoleCommand, RoleUpdateDto, RoleEntity, Guid>
{
    private readonly RoleManager<RoleEntity> _roleManager;

    public UpdateRoleCommandHandler(
        IWriteRepository<RoleEntity, Guid> repo,
        IUnitOfWork uow,
        IMapper mapper,
        RoleManager<RoleEntity> roleManager)
        : base(repo, uow, mapper)
    {
        _roleManager = roleManager;
    }

    protected override Task OnNotFoundAsync(Guid id, CancellationToken ct)
        => throw new KeyNotFoundException($"Role '{id}' not found.");

    protected override Task OnBeforeMapAsync(RoleEntity entity, RoleUpdateDto dto, UpdateRoleCommand request, CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    protected override async Task OnAfterUpdateAsync(RoleEntity entity, int affectedRows, UpdateRoleCommand request, CancellationToken ct)
    {
        if (request.Model.Claims is not null)
        {
            var current = await _roleManager.GetClaimsAsync(entity);
            var target = request.Model.Claims
                .Select(c => new Claim(c.Type, c.Value))
                .Distinct(ClaimEqualityComparer.TypeAndValue)
                .ToList();

            var toRemove = current.Except(target, ClaimEqualityComparer.TypeAndValue).ToList();
            var toAdd = target.Except(current, ClaimEqualityComparer.TypeAndValue).ToList();

            foreach (var claim in toRemove)
            {
                var r = await _roleManager.RemoveClaimAsync(entity, claim);
                if (!r.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", r.Errors.Select(e => e.Description)));
            }

            foreach (var claim in toAdd)
            {
                var r = await _roleManager.AddClaimAsync(entity, claim);
                if (!r.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", r.Errors.Select(e => e.Description)));
            }
        }

        await Uow.SaveChangesAsync(true, ct);
    }
}