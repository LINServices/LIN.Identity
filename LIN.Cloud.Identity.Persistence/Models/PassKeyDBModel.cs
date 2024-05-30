using LIN.Types.Cloud.Identity.Models;

namespace LIN.Cloud.Identity.Persistence.Models;


public class PassKeyDBModel
{

    public int Id { get; set; }
    public DateTime Time { get; set; }
    public int AccountId { get; set; }
    public AccountModel Account { get; set; } = null!;

}