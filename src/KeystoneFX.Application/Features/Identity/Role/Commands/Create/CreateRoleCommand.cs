using KeystoneFX.Application.Common.Commands;
using KeystoneFX.Application.Features.Identity.Role.Dtos;

namespace KeystoneFX.Application.Features.Identity.Role.Commands.Create;

public record CreateRoleCommand(RoleCreateDto Model)
    : CreateCommandBase<RoleCreateDto, Guid>(Model);