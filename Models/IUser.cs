namespace App
{
    // =================================
    // ENUM for roles and registration and permissions
    // =================================
    enum Role
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
        AddLocation,
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
    interface IUser
    {
        string Username { get; }
        string Password { get; }
        Role GetRole();
        Registration GetRegistration();
        List<Permissions> GrantedPermissions { get; }
        bool TryLogin(string username, string password);
        void AcceptPending();
        void DenyPending();
        void AcceptAddLocationPermission();
        void DenyAddLocationPermission();
        void AcceptAddRegistrationsPermission();
        void DenyAddRegistrationsPermission();
        bool HasPermission(string permissionName);
    }

}
