namespace LIN.Cloud.Identity.Persistence.Models;


public class QueryObjectFilter
{
    public int AccountContext { get; set; }
    public int IdentityContext { get; set; }
    public List<int> OrganizationsDirectories { get; set; } = [];
    public bool IsAdmin { get; set; }
    public bool IncludePhoto { get; set; } = true;
    public FindOn FindOn { get; set; }

}


public enum FindOn
{
    StableAccounts,
    AllAccounts
}