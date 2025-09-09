using KeystoneFX.Application.Common.Commands;
using KeystoneFX.Application.Features.Identity.User.Dtos;

namespace KeystoneFX.Application.Features.Identity.User.Commands.Create;

public record CreateUserCommand(UserCreateDto Model)
    : CreateCommandBase<UserCreateDto, Guid>(Model);