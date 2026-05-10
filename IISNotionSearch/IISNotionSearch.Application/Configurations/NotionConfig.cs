namespace IISNotionSearch.Application.Configurations;

public class NotionConfig
{
    public string BaseUrl { get; set; } = null!;
    public string InternalIntegrationSecret { get; set; } = null!;
    public string Version { get; set; } = null!;
}