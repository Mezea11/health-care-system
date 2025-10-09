namespace App;

// ============================
// Main class for users extends IUser interface
// ============================
class User : IUser
{
    public string Username { get; private set; }

    // SET PASSWORD TO PRIVATE
    public string Password { get; private set; }
    private Role role;

    public User(string username, string password, Role role)
    {
        Username = username;
        Password = password;
        this.rol e = role;
    }

    public Role GetRole() => role;

    // Try login method
    public bool TryLogin(string username, string password)
    {
        return Username == username && Password == password;
    }
}
