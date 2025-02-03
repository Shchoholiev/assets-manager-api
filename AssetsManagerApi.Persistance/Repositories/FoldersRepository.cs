using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Persistance.Db;
using System.Drawing.Printing;
using System;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos;
using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Persistance.Repositories;
public class FoldersRepository(CosmosDbContext db)
    : BaseRepository<Folder>(db, "Folders"), IFoldersRepository
{
}