namespace LIN.Identity.Services;


public static class OrgRoleExt
{



    public static bool IsGretter(this OrgRoles me, OrgRoles other)
    {

        switch (me)
        {

            case OrgRoles.SuperManager:
                {
                    return false;
                }

            case OrgRoles.Manager:
                {
                    return other == OrgRoles.SuperManager;
                }

            case OrgRoles.Regular:
                {
                    return other == OrgRoles.SuperManager || other == OrgRoles.Manager;
                }

            case OrgRoles.Guest:
                {
                    return other == OrgRoles.SuperManager || other == OrgRoles.Manager;
                }

            case OrgRoles.Undefine:
                {
                    return true;
                }

        }

        return false;



    }



    public static bool IsAdmin(this OrgRoles rol) => rol == OrgRoles.SuperManager || rol == OrgRoles.Manager;


}