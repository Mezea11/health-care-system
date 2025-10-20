namespace App
{
    // =================================
    // ENUM for roles and registration and permissions
    // =================================
    public enum Role
    {
        None,
        Patient,
        Personnel,
        Admin,
        SuperAdmin,
    }

    public enum PersonellRoles
    {
        None = 0, // Lägg till detta!
        Doctor,
        Nurse,
        Administrator,
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
        // string Password { get; }
        // Store hashed password and salt instead of plain text and remove password
        string PasswordHash { get; }
        string PasswordSalt { get; }
        void GrantPermission(Permissions perm);
        void RevokePermission(Permissions perm);
        bool HasPermission(string permissionName);
        Role GetRole();
        Registration GetRegistration();
        PersonellRoles PersonelRole { get; }
        void SetRolePersonell(int handleRole, IUser persObj);
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
        void AssignRegion(Region region);
        Region? GetAssignedRegion();
    }

}
