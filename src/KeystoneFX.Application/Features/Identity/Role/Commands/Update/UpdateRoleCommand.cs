using KeystoneFX.Application.Common.Commands;
using KeystoneFX.Application.Features.Identity.Role.Dtos;

namespace KeystoneFX.Application.Features.Identity.Role.Commands.Update;

public record UpdateRoleCommand(Guid Id, RoleUpdateDto Model)
    : UpdateCommandBase<Guid, RoleUpdateDto>(Id, Model);