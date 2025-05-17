using LIN.Types.Cloud.Identity.Enumerations;

namespace LIN.Cloud.Identity.Services.Extensions;

public static class ValidateRoles
{

    public static bool ValidateRead(this IEnumerable<Roles> roles)
    {
        List<Roles> availed =
                    [
                        Roles.Administrator,
                        Roles.Manager,
                        Roles.AccountOperator,
                        Roles.Regular,
                        Roles.Viewer,
                        Roles.SecurityViewer
                    ];


        var sets = availed.Intersect(roles);
        return sets.Any();
    }

    public static bool ValidateReadSecure(this IEnumerable<Roles> roles)
    {
        List<Roles> availed =
                    [
                        Roles.Administrator,
                        Roles.Manager,
                        Roles.AccountOperator,
                        Roles.Regular,
                        Roles.SecurityViewer
                    ];

        var sets = availed.Intersect(roles);
        return sets.Any();
    }

    public static bool ValidateAlterPolicies(this IEnumerable<Roles> roles)
    {
        List<Roles> availed =
                    [
                        Roles.Administrator,
                        Roles.Manager
                    ];

        var sets = availed.Intersect(roles);

        return sets.Any();

    }

    public static bool ValidateAlterMembers(this IEnumerable<Roles> roles)
    {
        List<Roles> availed =
                    [
                        Roles.Administrator,
                        Roles.Manager,
                        Roles.AccountOperator
                    ];

        var sets = availed.Intersect(roles);
        return sets.Any();
    }

    public static bool ValidateInviteMembers(this IEnumerable<Roles> roles)
    {
        List<Roles> availed =
                    [
                        Roles.Administrator,
                        Roles.Manager
                    ];

        var sets = availed.Intersect(roles);
        return sets.Any();
    }

    public static bool ValidateDelete(this IEnumerable<Roles> roles)
    {
        List<Roles> availed =
                    [
                        Roles.Administrator,
                        Roles.Manager
                    ];

        var sets = availed.Intersect(roles);
        return sets.Any();
    }

    public static bool ValidateReadPolicies(this IEnumerable<Roles> roles)
    {
        List<Roles> availed =
                    [
                        Roles.Administrator,
                        Roles.Manager,
                        Roles.AccountOperator,
                        Roles.SecurityViewer
                ];

        var sets = availed.Intersect(roles);
        return sets.Any();
    }

}