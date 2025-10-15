namespace App;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class FileHandler
{
    public const string UserCsvFileName = "users_export.csv";

    public static void SaveUsersToCsv(List<IUser> users)
    {
        List<string> PrintToUsers = new() { "id,username,password,Role,permissionList" };
        // foreach (List<IUser> user in users)
        // {
        //     Console.WriteLine(user.ToString());
        // }

        Console.WriteLine(users.Count);
        try
        {
            File.WriteAllLines(UserCsvFileName, PrintToUsers, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: something happened when exporting users to CSV: {ex.Message}");
        }
    }
}