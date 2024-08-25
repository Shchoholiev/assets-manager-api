using AssetsManagerApi.Domain.Entities.Identity;

namespace AssetsManagerApi.Application.IRepositories;

public interface IUsersRepository : IBaseRepository<User>
{
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken);
}
