using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Enums;

namespace AssetsManagerApi.Application.Utils;

public static class FolderMerger
{
    public static FolderDto MergeFolders(IEnumerable<FolderDto> roots)
    {
        var result = new FolderDto { Name = "MergedRoot", Type = FileType.Folder };

        foreach (var folder in roots)
        {
            MergeInto(result, folder);
        }

        return result;
    }

    private static void MergeInto(FolderDto target, FolderDto source)
    {
        target.Items ??= [];
        foreach (var item in source.Items ?? [])
        {
            Console.WriteLine(item.Type);
            if (item is FolderDto sourceSubfolder)
            {
                sourceSubfolder.Items ??= [];

                var existing = target.Items
                    .OfType<FolderDto>()
                    .FirstOrDefault(f => f.Name == sourceSubfolder.Name);

                if (existing != null)
                {
                    MergeInto(existing, sourceSubfolder);
                }
                else
                {
                    target.Items.Add(sourceSubfolder);
                }
            }
            else if (item is CodeFileDto file)
            {
                if (string.Equals(file.Name, "Program.cs", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(file.Name, "Startup.cs", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                
                target.Items.Add(new CodeFileDto
                {
                    Name = file.Name,
                    Type = FileType.CodeFile,
                    Text = file.Text,
                    Language = file.Language
                });
            }
        }
    }
}
