using System;
using System.IO;
using System.Text.Json;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
namespace TwitchBot.Scripts.Users
{
    /// <summary>
    /// Class containing the list of users
    /// </summary>
    public class UserDatabase
    {
        /// <summary> default file path </summary>
        public string userFilePath;
        /// <summary> List containing all current users  </summary>
        private List<User> users = new();
        /// <summary> Twitch api reference </summary>
        private TwitchAPI api;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserDatabase()
        {
        }

        /// <summary>
        /// Reads the database to load all users
        /// </summary>
        public static UserDatabase LoadDatabase(TwitchAPI api, string path)
        {
            string text = File.ReadAllText(path);

            UserDatabase database = new(); 
            database.users = JsonSerializer.Deserialize<List<User>>(text);
            if (database.users is null)
            {
                Console.WriteLine("INVALID DATABASE");
                throw new NullReferenceException("INVALID DATABASE");
            }
            database.api = api;
            database.userFilePath = path;
            return database;
        }

        /// <summary>
        /// Stores the database to the given path (default: userfilepath)
        /// </summary>
        /// <param name="path"></param>
        public void SaveDatabase(string path = null)
        {
            path ??= userFilePath;
            Console.WriteLine($"saving to: {path}");
            string text = JsonSerializer.Serialize(users);
            File.WriteAllText(path, text);
        }

        /// <summary>
        /// reloads the database from userfilepath)
        /// </summary>
        /// <param name="path"></param>
        public void ReloadDatabase()
        {
            string text = File.ReadAllText(userFilePath);

            var users = JsonSerializer.Deserialize<List<User>>(text);
            if (users is null)
            {
                Console.WriteLine("INVALID DATABASE");
                throw new NullReferenceException("INVALID DATABASE");
            }
            Console.WriteLine($"loading : {userFilePath}");
            this.users = users;
        }

        /// <summary>
        /// Adds a new user to the database
        /// </summary>
        /// <param name="userId"></param>
        public User AddUser(string userId)
        {
            User newUser = new User(userId);
            users.Add(newUser);
            return newUser;
        }

        /// <summary>
        /// Returns user in database, if found, else adds a new user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="username"> if set, then updates username</param>
        /// <returns></returns>
        public User FindOrAddUserByID(string userId, string username = null)
        {
            User user = FindUserByID(userId);
            if (user is null)
                user = AddUser(userId);
            // We always update username if possible
            if(username is not null)
                user.name = username;
            return user;
        }

        /// <summary>
        /// Returns user in database, if found, else returns null
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public User FindUserByID(string userId)
        {
            return users.Find((User user) => user.id == userId);
        }

        /// <summary>
        /// Returns the user matching with the username, creating one if there is none, and null if the username does not exist
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<User> GetUserByUsername(string username)
        {
            string userId = await GetUserId(username);
            if (userId is null)
                return null;

            User user = FindOrAddUserByID(userId);
            // We always update the username, since chatters keep changing it and it may be useful
            user.name = username;
            return user;
        }

        /// <summary>
        /// Query to twitch to find a user using their name, returns user id if found, null otherwise
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private async Task<string> GetUserId(string username)
        {
            string filteredUsername = username;
            if (username[0] == '@')
                filteredUsername = username.Substring(1);
            GetUsersResponse userResponse = await api.Helix.Users.GetUsersAsync(logins: new() { filteredUsername });
            foreach (TwitchLib.Api.Helix.Models.Users.GetUsers.User? user in userResponse.Users)
            {
                if (user != null)
                {
                    return user.Id;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the top 5 users in points
        /// </summary>
        public List<User> GetTopPoints()
        {
            List<User> ordered = users.OrderByDescending(user => user.points).ToList();
            return ordered.GetRange(0,5);
        }

        /// <summary>
        /// Returns the top 5 users in point loss
        /// </summary>
        public List<User> GetTopLosers()
        {
            List<User> ordered = users.OrderByDescending(user => user.pointLoss).ToList();
            return ordered.GetRange(0, 5);
        }

        /// <summary>
        /// Returns point rank of the user
        /// </summary>
        public int GetUserPointRank(User user)
        {
            List<User> ordered = users.OrderByDescending(user => user.points).ToList();
            return ordered.IndexOf(user);
        }

        /// <summary>
        /// Returns point loss rank of the user
        /// </summary>
        public int GetUserPointLossRank(User user)
        {
            List<User> ordered = users.OrderByDescending(user => user.pointLoss).ToList();
            return ordered.IndexOf(user);
        }
    }
}