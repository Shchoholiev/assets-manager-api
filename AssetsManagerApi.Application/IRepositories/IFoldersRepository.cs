using AssetsManagerApi.Domain.Entities;

namespace AssetsManagerApi.Application.IRepositories;
public interface IFoldersRepository : IBaseRepository<Folder>
{
    Task<Folder> GetFolderAsync(string id, CancellationToken cancellationToken);
}
