using AutoMapper;
using KeystoneFX.Application.Common.Commands.Handlers;
using KeystoneFX.Application.Features.Identity.Role.Dtos;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using RoleEntity = KeystoneFX.Domain.Identity.Role;

namespace KeystoneFX.Application.Features.Identity.Role.Commands.Create;

public class CreateRoleCommandHandler
    : CreateCommandHandlerBase<CreateRoleCommand, RoleCreateDto, RoleEntity, Guid>
{
    private readonly RoleManager<RoleEntity> _roleManager;
    private readonly ILookupNormalizer _normalizer;

    public CreateRoleCommandHandler(
        IWriteRepository<RoleEntity, Guid> repo,
        IUnitOfWork uow,
        IMapper mapper,
        RoleManager<RoleEntity> roleManager,
        ILookupNormalizer normalizer)
        : base(repo, uow, mapper)
    {
        _roleManager = roleManager;
        _normalizer = normalizer;
    }

    protected override async Task OnBeforeAddAsync(RoleEntity entity, CreateRoleCommand request, CancellationToken ct)
    {
        var exists = await _roleManager.FindByNameAsync(request.Model.Name);
        if (exists is not null)
            throw new InvalidOperationException($"Role '{request.Model.Name}' already exists.");

        entity.NormalizedName = _normalizer.NormalizeName(entity.Name);
        entity.ConcurrencyStamp = Guid.NewGuid().ToString("N");
    }

    protected override async Task OnAfterAddAsync(RoleEntity entity, CreateRoleCommand request, CancellationToken ct)
    {
        if (request.Model.Claims is { Count: > 0 })
        {
            foreach (var c in request.Model.Claims)
            {
                var res = await _roleManager.AddClaimAsync(entity, new Claim(c.Type, c.Value));
                if (!res.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
            }
        }

        await Uow.SaveChangesAsync(true, ct);
    }
}