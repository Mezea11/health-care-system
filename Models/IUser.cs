namespace App
{
    // =================================
    // ENUM for roles and registration and permissions
    // =================================
    public enum Role
    {
        Patient,
        Personnel,
        Admin,
        SuperAdmin,

    }
    public enum Permissions
    {
        None,
        AddRegistrations,
        AddPersonell,
        AddAdmin,
        AddLocation,
        ViewPermissions
    }

    public enum Registration // Enum för början av registrerings processen
    {
        Accepted,
        Pending,
        Denied,
    }

    public enum Region
    {
        None,
        Skåne,
        Norrland,
        Götaland,
    }


    // ============================
    // Interface for all users
    // ============================
    public interface IUser
    {
        int Id { get; }
        string Username { get; }
        string Password { get; }
        Role GetRole();
        Registration GetRegistration();
        List<Permissions> PermissionList { get; }
        bool TryLogin(string username, string password);
        void AcceptPending();
        void DenyPending();
        void AcceptAddLocationPermission();
        void DenyAddLocationPermission();
        void AcceptAddRegistrationsPermission();
        void DenyAddRegistrationsPermission();
        void AcceptAddPersonellPermission();
        void DenyAddPersonellPermission();

        void AcceptViewPermissions();
        void DenyViewPermissions();
        bool HasPermission(string permissionName);
        void AssignRegion(Region region);
        Region? GetAssignedRegion();
    }

}
