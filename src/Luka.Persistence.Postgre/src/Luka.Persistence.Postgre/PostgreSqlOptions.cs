namespace Luka.Persistence.Postgre;

public class PostgreSqlOptions
{
    public string Host { get; set; }
    public ushort Port { get; set; }
    public string Database { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public bool IsNeedToSyncPermissionEnumToDb { get; set; }
}