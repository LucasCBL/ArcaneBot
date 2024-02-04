using System.IO;
using System.Text.Json;
namespace TwitchBot.Scripts.User
{
    /// <summary>
    /// Class containing the list of users
    /// </summary>
    public class UserDatabase
    {
        public const string userFilePath = "./users.json";
        /// <summary> List containing all current users  </summary>
        List<User> users = new();

        public UserDatabase()
        {
        }

        /// <summary>
        /// Reads the database to load all users
        /// </summary>
        public static UserDatabase LoadDatabase(string path = userFilePath)
        {
            string text = File.ReadAllText(path);

            return JsonSerializer.Deserialize<UserDatabase>(text);
        }

        /// <summary>
        /// Stores the database to the given path (default: userfilepath)
        /// </summary>
        /// <param name="path"></param>
        public void SaveDatabase(string path = userFilePath)
        {
            string text = JsonSerializer.Serialize(this);
            File.WriteAllText(path, text);
        }

        /// <summary>
        /// Returns user in database, if found
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User? FindUserByID(string userId)
        {
            return users.Find((User user) => user.id == userId);
        }
    }
}