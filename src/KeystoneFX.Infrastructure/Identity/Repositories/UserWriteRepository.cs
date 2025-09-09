using KeystoneFX.Application.Common.Abstractions.Repositories.Identity;
using KeystoneFX.Domain.Identity;
using KeystoneFX.Infrastructure.Identity.Contexts;
using KeystoneFX.Infrastructure.Persistence.Repositories;

namespace KeystoneFX.Infrastructure.Identity.Repositories;

public class UserWriteRepository : EfWriteRepository<User, Guid>, IUserWriteRepository
{
    public UserWriteRepository(IdentityContext context) : base(context) { }
}