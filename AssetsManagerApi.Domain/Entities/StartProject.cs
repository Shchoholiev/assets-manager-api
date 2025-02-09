namespace AssetsManagerApi.Domain.Entities;

public class StartProject : EntityBase
{
    public List<string> CodeAssetsIds { get; set; }

    public string CompanyId { get; set; }

    /// <summary>
    /// The id of code asset that was created as a result of the start project combination.
    /// This approach simplifies management of start project code. 
    /// </summary>
    public string? CodeAssetId { get; set; }
}
