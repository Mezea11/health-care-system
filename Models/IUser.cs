namespace App
{
    // =================================
    // ENUM for roles and registration
    // =================================
    enum Role
    {
        Patient,
        Personnel,
        Admin,

    }
    enum Registration // Enum för början av registrerings processen
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
        int Id { get; }
        string Username { get; }
        string Password { get; }
        Role GetRole();
        Registration GetRegistration();
        bool TryLogin(string username, string password);

        void AcceptPending();
        void DenyPending();
    }

}
