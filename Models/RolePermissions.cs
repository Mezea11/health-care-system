namespace App
{
    public static class RolePermissions
    {
        public static readonly Dictionary<Role, List<Permissions>> Map = new()
    {
        { Role.Patient, new List<Permissions> { Permissions.None } },
        { Role.Personnel, new List<Permissions> { Permissions.None } },
        { Role.Admin, new List<Permissions> { Permissions.None } },
        { Role.SuperAdmin, new List<Permissions> { Permissions.None } }
    };
    }
}
