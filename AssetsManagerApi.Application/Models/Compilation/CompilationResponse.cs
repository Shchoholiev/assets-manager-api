namespace AssetsManagerApi.Application.Models.Compilation;

public class CompilationResponse
{
    public bool Succeeded { get; set; }

    public string? Output { get; set; }

    public string? Logs { get; set; }

    public string? Error { get; set; }
}
