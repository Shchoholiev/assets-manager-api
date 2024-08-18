namespace AssetsManagerApi.Domain.Entities;

public abstract class EntityBase
{
    public string Id { get; set; }

    public string CreatedById { get; set; }

    public DateTime CreatedDateUtc { get; set; }

    public bool IsDeleted { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTime? LastModifiedDateUtc { get; set; }
}
