using KeystoneFX.Application.Common.Abstractions.Repositories.Identity;
using KeystoneFX.Domain.Identity;
using KeystoneFX.Infrastructure.Identity.Contexts;
using KeystoneFX.Infrastructure.Persistence.Repositories;
using KeystoneFX.Infrastructure.Persistence.Specifications;

namespace KeystoneFX.Infrastructure.Identity.Repositories;

public class UserReadRepository : EfReadRepository<User, Guid>, IUserReadRepository
{
    public UserReadRepository(IdentityContext db, SpecificationEvaluatorOptions? options = null) : base(db, options) { }
}