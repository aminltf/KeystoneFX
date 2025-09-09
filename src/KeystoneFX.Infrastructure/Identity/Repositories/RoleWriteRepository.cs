using KeystoneFX.Application.Common.Abstractions.Repositories.Identity;
using KeystoneFX.Domain.Identity;
using KeystoneFX.Infrastructure.Identity.Contexts;
using KeystoneFX.Infrastructure.Persistence.Repositories;

namespace KeystoneFX.Infrastructure.Identity.Repositories;

public class RoleWriteRepository : EfWriteRepository<Role, Guid>, IRoleWriteRepository
{
    public RoleWriteRepository(IdentityContext context) : base(context) { }
}