namespace LIN.Cloud.Identity.Persistence.Models;

public class OtpDatabaseModel
{
    public int Id { get; set; }
    public string Code { get; set; }
    public DateTime ExpireTime { get; set; }
    public bool IsUsed { get; set; }
    public LIN.Types.Cloud.Identity.Models.AccountModel Account { get; set; }
    public int AccountId { get; set; }
}