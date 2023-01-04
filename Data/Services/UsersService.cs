using System.Text.Json;
using Coursework.Data.Model;

namespace Coursework.Data.Services;

public static class UsersService
{
    public const string SeedUsername = "admin";
    public const string SeedPassword = "admin";
    public static User CurrentUser = null;


    public static User GetCurrentUser()
    {
        return CurrentUser;
    }

    private static void SaveAllUsers(List<User> users)
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();
        string appUsersFilePath = Utils.GetAppUsersFilePath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(users);
        File.WriteAllText(appUsersFilePath, json);
    }

    public static List<User> GetAllUsers()
    {
        string appUsersFilePath = Utils.GetAppUsersFilePath();
        if (!File.Exists(appUsersFilePath))
        {
            return new List<User>();
        }

        var json = File.ReadAllText(appUsersFilePath);

        return JsonSerializer.Deserialize<List<User>>(json);
    }

    public static List<User> Create(Guid userId, string username, string password, Role role)
    {
        List<User> users = GetAllUsers();
        bool usernameExists = users.Any(x => x.Username == username);

        if (usernameExists)
        {
            throw new Exception("Username already exists.");
        }

        users.Add(
            new User
            {
                Username = username,
                PasswordHash = Utils.HashSecret(password),
                Role = role,
                CreatedBy = userId
            }
        );
        SaveAllUsers(users);
        return users;
    }

    public static void SeedUsers()
    {
        var users = GetAllUsers().FirstOrDefault(x => x.Role == Role.Admin);

        if (users == null)
        {
            Create(Guid.Empty, SeedUsername, SeedPassword, Role.Admin);
        }
    }

    public static User GetById(Guid id)
    {
        List<User> users = GetAllUsers();
        return users.FirstOrDefault(x => x.Id == id);
    }

    public static List<User> Delete(Guid id)
    {
        List<User> users = GetAllUsers();
        User user = users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        TodosService.DeleteByUserId(id);
        users.Remove(user);
        SaveAllUsers(users);

        return users;
    }

    public static User Login(string username, string password)
    {
        var loginErrorMessage = "Invalid username or password.";
        List<User> users = GetAllUsers();
        User user = users.FirstOrDefault(x => x.Username == username);

        if (user == null)
        {
            throw new Exception(loginErrorMessage);
        }

        bool passwordIsValid = Utils.VerifyHash(password, user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new Exception(loginErrorMessage);
        }

        CurrentUser = user;
        return user;
    }

    public static User ChangePassword(Guid id, string currentPassword, string newPassword)
    {
        if (currentPassword == newPassword)
        {
            throw new Exception("New password must be different from current password.");
        }

        List<User> users = GetAllUsers();
        User user = users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        bool passwordIsValid = Utils.VerifyHash(currentPassword, user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new Exception("Incorrect current password.");
        }

        user.PasswordHash = Utils.HashSecret(newPassword);
        user.HasInitialPassword = false;
        SaveAllUsers(users);

        return user;
    }
}
