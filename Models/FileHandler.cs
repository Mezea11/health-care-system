namespace App;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

class FileHandler
{
    public const string UserJsonFileName = "users.json";

    public static List<IUser> LoadUsersFromJson()
    {
        if (!File.Exists(UserJsonFileName))
        {
            // Skapa testdata om filen inte finns
            List<IUser> testUsers = new List<IUser>
            {
                new User(0, "patient", "123", Role.Patient),
                new User(1, "personell", "123", Role.Personnel),
                new User(2, "admin", "123", Role.Admin),
                new User(3, "admin1", "123", Role.Admin),
                new User(4, "admin2", "123", Role.Admin),
                new User(5, "superadmin", "123", Role.SuperAdmin)
            };

            // Tilldela permissions direkt ska göras av administratörer
            // foreach (User user in testUsers.OfType<User>())
            // {
            //     user.ApplyRolePermissions();
            // }

            // return testUsers;
        }

        try
        {
            string json = File.ReadAllText(UserJsonFileName);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            List<User>? users = JsonSerializer.Deserialize<List<User>>(json, options);
            var result = users?.ConvertAll<IUser>(u => u) ?? new List<IUser>();

            // Säkerställ att alla användare får rätt permissions baserat på roll
            // foreach (var u in result.OfType<User>())
            //     u.ApplyRolePermissions();

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR loading users from JSON: {ex.Message}");
            return new List<IUser>();
        }
    }

    public static void SaveUsersToJson(List<IUser> users)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var concreteUsers = new List<User>();
            foreach (var u in users)
            {
                if (u is User user)
                    concreteUsers.Add(user);
            }

            string json = JsonSerializer.Serialize(concreteUsers, options);
            File.WriteAllText(UserJsonFileName, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR saving users to JSON: {ex.Message}");
        }
    }
}
