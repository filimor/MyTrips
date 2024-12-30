namespace MyTrips.Infrastructure.Models;

public class AppSetting
{
    public string ConnectionString { get; set; } = null!;
    public int CommandTimeout { get; set; } = 30;
    public int CacheItemExpiration { get; set; } = 60;
}