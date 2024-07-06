using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Scripts.Utils
{
    /// <summary>
    /// Text related utilities
    /// </summary>
    public static class StringUtils
    {
        /// <summary> Database for the banned words, anything including these words should not be sent, this must be set manually</summary>
        public static List<string> bannedWords = new();
        
        /// <summary>
        /// Splits string in groups of maxCount characters
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static string[] SplitByCount(string input, int maxCount)
        {
            int splits = (input.Length / maxCount) + 1;
            string[] result = new string[splits];

            for (int i = 0; i < splits - 1; i++)
                result[i] = input.Substring(i * maxCount, maxCount);

            result[^1] = input[((splits - 1) * maxCount)..];
            return result;
        }

        /// <summary>
        /// Just a call to the split command, but centralized here in case custom behaviors are needed in the future
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string[] SplitCommand(string input)
        {
            return input.Split().Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }

        /// <summary>
        /// Removes the command from
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveCommandFromString(string input)
        {
            string[] result = SplitCommand(input);
            if(result.Length < 2) {
                Console.WriteLine("Error, empty string in RemoveCommandFromString");
                return null;
            }
            return input[result[0].Length..];
        }

        /// <summary>
        /// Turns a list of strings into a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetString(IList<string> input)
        {
            if (input is null || input.Count == 0)
                return string.Empty;

            string message = "";
            foreach(string str in input)
            {
                message += str + ", ";
            }

            message = message.Remove(message.Length - 2);
            return message;
        }

        /// <summary>
        /// This function checks whether the input contains any banned word, such as slurs
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ContainsBannedWord(IEnumerable<string> input)
        {
            // We must ensure that users do not get around the banned words by adding characters to them,
            // there are cases that can be evaded, but these must be handled by the twitch channels themselves
            foreach (string inputStr in input)
                if(ContainsBannedWord(inputStr))
                    return true;

            return false;
        }

        /// <summary>
        /// This function checks whether the input contains any banned word, such as slurs
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ContainsBannedWord(string input)
        {
            // We must ensure that users do not get around the banned words by adding characters to them,
            // there are cases that can be evaded, but these must be handled by the twitch channels themselves
            foreach (string bannedStr in bannedWords)
                if (input.Contains(bannedStr))
                    return true;

            return false;
        }
    }
}
