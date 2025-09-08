using KeystoneFX.Application.Common.Commands;
using KeystoneFX.Application.Features.Identity.User.Dtos;

namespace KeystoneFX.Application.Features.Identity.User.Commands.Update;

public sealed record UpdateUserCommand(Guid Id, UserUpdateDto Model)
    : UpdateCommandBase<Guid, UserUpdateDto>(Id, Model);