namespace LIN.Cloud.Identity.Services.Models;

public class QueryIdentityFilter
{
    public FindOn FindOn { get; set; }
    public bool IncludeDates { get; set; }

}

public enum FindOnIdentities
{
    Stable,
    All
}