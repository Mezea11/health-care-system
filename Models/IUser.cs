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
        string RoleDetails { get; }
        string ToPersonnelDisplay();

        void GrantPermission(Permissions perm);
        void RevokePermission(Permissions perm);
        void SetRolePersonell(int handleRole, IUser persObj, string roleDetails);
        void AcceptPending();
        void DenyPending();

        bool TryLogin(string username, string password);
        bool HasPermission(string permissionName);
        bool HasPermission(Permissions permission);
        bool AssignPersonnel(int personnelId);

        Role GetRole();

        List<int> AssignedPersonnelIds { get; }
        List<Permissions> PermissionList { get; }

        Registration GetRegistration();
        PersonellRoles PersonelRole { get; }

        void SetRolePersonell(int handleRole, IUser persObj);
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
