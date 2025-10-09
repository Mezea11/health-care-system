namespace App
{
    // ============================
    // ENUM for roles
    // ============================
    enum Role
    {
        Patient,
        Personnel,
        Admin
    }

    // ============================
    // Interface for all users
    // ============================
    interface IUser
    {
        string Username { get; }
        string Password { get; }
        Role GetRole();
        bool TryLogin(string username, string password);
    }

}
