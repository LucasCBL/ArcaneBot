using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Scripts.Users;
using TwitchLib.Api.Helix;
using TwitchLib.Api;
using System.Text.Json;
using TwitchBot.Scripts.Utils;

namespace TwitchBot.Scripts.Games.GameUtils
{
    /// <summary>
    /// Generic class for a game database, just loads a database and allows you to add things to it 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GameDatabase<T>
    {
        protected List<T> data;
        protected string dataPath;
        private GameDatabase(string path)
        {
            dataPath = path;
        }

        /// <summary>
        /// Reads the database to load all users
        /// </summary>
        public static GameDatabase<T> LoadDatabase(string path)
        {
            string text = File.ReadAllText(path);


            GameDatabase<T> database = new(path);
            database.data = new(JsonSerializer.Deserialize<List<T>>(text));
            if (database.data is null)
                Console.WriteLine("INVALID DATABASE");

            return database;
        }

        /// <summary>
        /// Stores the database to the given path (default: userfilepath)
        /// </summary>
        /// <param name="path"></param>
        public void SaveDatabase(string path = null)
        {
            path ??= dataPath;
            Console.WriteLine($"saving to: {path}");
            string text = JsonSerializer.Serialize(data);
            File.WriteAllText(path, text);
        }
        /// <summary>
        /// reloads the database from dataPath
        /// </summary>
        /// <param name="path"></param>
        public void ReloadDatabase()
        {
            string text = File.ReadAllText(dataPath);

            var data = JsonSerializer.Deserialize<List<T>>(text);
            if (data is null)
            {
                Console.WriteLine("INVALID DATABASE");
                throw new NullReferenceException("INVALID DATABASE");
            }
            Console.WriteLine($"loading : {dataPath}");
            this.data = data;
        }

        /// <summary>
        /// Returns a random value from the database
        /// </summary>
        /// <returns></returns>
        public T GetRandomData()
        {
            return data[MathUtils.RandomNumber(0, data.Count)];
        }        
    }
}
