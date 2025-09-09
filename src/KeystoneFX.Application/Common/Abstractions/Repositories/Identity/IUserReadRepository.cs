using KeystoneFX.Domain.Identity;
using KeystoneFX.Shared.Kernel.Abstractions.Data;

namespace KeystoneFX.Application.Common.Abstractions.Repositories.Identity;

public interface IUserReadRepository : IReadRepository<User, Guid>
{
}