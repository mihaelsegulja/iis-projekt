namespace IISNotionSearch.Application.Configurations;

public enum DataSourceType
{
    Local,
    External
}

public class AppConfig
{
    public DataSourceType DataSource { get; set; }
}