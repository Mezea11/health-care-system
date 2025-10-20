namespace App;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// FileHandler is responsible for saving and loading users to/from a JSON file.
/// </summary>
class FileHandler
{
    // The filename where users are stored
    public const string UserJsonFileName = "users.json";

    /// <summary>
    /// Loads users from the JSON file.
    /// If the file does not exist, returns a list of test users.
    /// </summary>
    public static List<IUser> LoadUsersFromJson()
    {
        // If the file does not exist → create test data
        if (!File.Exists(UserJsonFileName))
        {
            return new List<IUser>
            {
                new User(0, "patient", "123", Role.Patient),
                new User(1, "personell", "123", Role.Personnel),
                new User(2, "admin", "123", Role.Admin),
                new User(3, "admin1", "123", Role.Admin),
                new User(4, "admin2", "123", Role.Admin),
                new User(5, "superadmin", "123", Role.SuperAdmin)
            };
        }

        try
        {
            // Read the entire JSON file as text
            string json = File.ReadAllText(UserJsonFileName);

            // Configure deserialization options
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // allows "username" or "Username"
                Converters = { new JsonStringEnumConverter() } // enums are stored as strings
            };

            // Deserialize JSON into a list of User objects
            List<User>? users = JsonSerializer.Deserialize<List<User>>(json, options);

            // Convert to List<IUser> (since we work with the interface)
            return users?.ConvertAll<IUser>(u => u) ?? new List<IUser>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR loading users from JSON: {ex.Message}");
            return new List<IUser>();
        }
    }

    /// <summary>
    /// Saves a list of users to the JSON file.
    /// </summary>
    public static void SaveUsersToJson(List<IUser> users)
    {
        try
        {
            // Configure serialization options
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // makes the JSON file nicely formatted
                Converters = { new JsonStringEnumConverter() } // enums are stored as strings
            };

            // Because IUser is an interface, we need to serialize the concrete User objects
            List<User> concreteUsers = new List<User>();
            foreach (User user in users)
            {
                if (user is User thatUser)
                    concreteUsers.Add(user);
            }

            // Serialize the list to JSON
            string json = JsonSerializer.Serialize(concreteUsers, options);

            // Write JSON to file
            File.WriteAllText(UserJsonFileName, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR saving users to JSON: {ex.Message}");
        }
    }
}
