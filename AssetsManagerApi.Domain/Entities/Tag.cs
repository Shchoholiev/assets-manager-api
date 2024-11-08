namespace AssetsManagerApi.Domain.Entities;

public class Tag : EntityBase
{
    public string Name { get; set; }

    public int UseCount { get; set; }
}
