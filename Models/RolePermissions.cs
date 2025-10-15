namespace App
{
    public static class RolePermissions
    {
        public static readonly Dictionary<Role, List<Permissions>> Map = new()
        {
            { Role.Patient, new List<Permissions> { Permissions.None } },

            { Role.Personnel, new List<Permissions>
                {
                    Permissions.AddRegistrations,
                    Permissions.ViewPermissions
                }
            },

            { Role.Admin, new List<Permissions>
                {
                    Permissions.AddRegistrations,
                    Permissions.AddPersonell,
                    Permissions.AddLocation,
                    Permissions.ViewPermissions
                }
            },

            { Role.SuperAdmin, Enum.GetValues<Permissions>()
                                   .Where(p => p != Permissions.None)
                                   .ToList()
            }
        };
    }
}
