namespace LIN.Identity.Models;


public class Account
{

    public int ContextAccount { get; set; }
    public bool IsAdmin { get; set; }
    public bool SensibleInfo { get; set; }
    public IncludeOrg IncludeOrg { get; set; } = IncludeOrg.None;
    public FindOn FindOn { get; set; } = FindOn.AllAccount;
    public IncludeOrgLevel OrgLevel { get; set; } = IncludeOrgLevel.Basic;

}



public enum IncludeOrg
{

    /// <summary>
    /// No incluir la organización
    /// </summary>
    None,

    /// <summary>
    /// Incluir la organización
    /// </summary>
    Include,

    /// <summary>
    /// Incluir la organización según el contexto
    /// </summary>
    IncludeIf

}

public enum IncludeOrgLevel
{

    /// <summary>
    /// Incluir el rol
    /// </summary>
    Basic,

    /// <summary>
    /// Incluir la organización y su nombre
    /// </summary>
    Advance

}

public enum FindOn
{

    /// <summary>
    /// En todas las cuentas
    /// </summary>
    AllAccount,

    /// <summary>
    /// En las cuentas estables
    /// </summary>
    StableAccounts

}