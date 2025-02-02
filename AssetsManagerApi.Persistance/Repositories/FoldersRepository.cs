using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Persistance.Db;
using System.Drawing.Printing;
using System;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos;
using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Persistance.Repositories;
public class FoldersRepository : BaseRepository<Folder>, IFoldersRepository
{
    private readonly Container _foldersContainer;
    private readonly Container _codeFilesContainer;

    public FoldersRepository(CosmosDbContext db)
        : base(db, "Folders")
    {
        _foldersContainer = db.GetContainerAsync("Folders").Result;
        _codeFilesContainer = db.GetContainerAsync("CodeFiles").Result;
    }

    public async Task<Folder> GetFolderAsync(string id, CancellationToken cancellationToken)
    {
        var response = await _foldersContainer.ReadItemAsync<Folder>(
            id,
            new PartitionKey(id),
            cancellationToken: cancellationToken);
        var folder = response.Resource;

        if (folder?.Items != null)
        {
            folder.Items = await LoadFolderItemsRecursive(folder);
        }

        return folder;
    }

    private async Task<List<FileSystemNode>> LoadFolderItemsRecursive(Folder folder)
    {
        var updatedItems = new List<FileSystemNode>();

        foreach (var item in folder.Items)
        {
            if (item.Type == FileType.Folder)
            {
                var subFolderResponse = await _foldersContainer.ReadItemAsync<Folder>(
                    item.Id,
                    new PartitionKey(item.Id));

                var subFolder = subFolderResponse.Resource;

                if (subFolder?.Items != null)
                {
                    subFolder.Items = await LoadFolderItemsRecursive(subFolder);
                }

                updatedItems.Add(subFolder);
            }
            else if (item.Type == FileType.CodeFile)
            {
                var codeFileResponse = await _codeFilesContainer.ReadItemAsync<CodeFile>(
                    item.Id,
                    new PartitionKey(item.Id));

                var codeFile = codeFileResponse.Resource;
                updatedItems.Add(codeFile);
            }
        }

        return updatedItems;
    }

}