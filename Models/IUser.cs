namespace App
{
    // =================================
    // ENUM for roles and registration
    // =================================
    enum Role
    {
        Patient,
        Personnel,
        Admin
    }

    enum Registration
    {
        Pending,
        Accept,
        Deny
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
        bool TryLogin(string username, string password);

        void AcceptPending();
        void DenyPending();
    }

}
