namespace AssetsManagerApi.Domain.Entities;

public class StartProject : EntityBase
{
    public List<string> CodeAssetsIds { get; set; }

    public string CompanyId { get; set; }
}
