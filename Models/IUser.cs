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
    public enum Permissions
    {
        None,
        AddRegistrations,
        AddPersonell,
        AddAdmin,
        AddLocation,
        ViewPatientJournal,
        ViewAllJournals,
        ViewPermissions
    }

    public enum Registration // Enum för början av registrerings processen
    {
        Accepted,
        Pending,
        Denied,
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

        void GrantPermission(Permissions perm);
        void RevokePermission(Permissions perm);
        bool HasPermission(string permissionName);
        bool HasPermission(Permissions permission);
        string PasswordHash { get; }
        string PasswordSalt { get; }
        Role GetRole();
        Registration GetRegistration();
        List<Permissions> PermissionList { get; }
        bool TryLogin(string username, string password);
        void AcceptPending();
        void DenyPending();
        // void AcceptAddLocationPermission();
        // void DenyAddLocationPermission();
        // void AcceptAddRegistrationsPermission();
        // void DenyAddRegistrationsPermission();
        // void AcceptAddPersonellPermission();
        // void DenyAddPersonellPermission();

        // void AcceptViewPermissions();
        // void DenyViewPermissions();
    }

}
