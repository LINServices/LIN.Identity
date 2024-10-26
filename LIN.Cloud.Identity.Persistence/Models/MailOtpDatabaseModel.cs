using LIN.Types.Cloud.Identity.Models;

namespace LIN.Cloud.Identity.Persistence.Models;

public class MailOtpDatabaseModel
{
    public MailModel MailModel { get; set; }
    public OtpDatabaseModel OtpDatabaseModel { get; set; }
    public int MailId { get; set; }
    public int OtpId { get; set; }
}