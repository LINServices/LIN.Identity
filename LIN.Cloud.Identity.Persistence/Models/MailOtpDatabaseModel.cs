namespace LIN.Cloud.Identity.Persistence.Models;

public class MailOtpDatabaseModel
{
    public MailModel MailModel { get; set; } = null!;
    public OtpDatabaseModel OtpDatabaseModel { get; set; } = null!;
    public int MailId { get; set; }
    public int OtpId { get; set; }
}