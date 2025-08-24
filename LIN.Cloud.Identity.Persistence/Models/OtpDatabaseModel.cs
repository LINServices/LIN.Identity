namespace LIN.Cloud.Identity.Persistence.Models;

public class OtpDatabaseModel
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ExpireTime { get; set; }
    public bool IsUsed { get; set; }
    public LIN.Types.Cloud.Identity.Models.Identities.AccountModel Account { get; set; } = null!;
    public int AccountId { get; set; }
}