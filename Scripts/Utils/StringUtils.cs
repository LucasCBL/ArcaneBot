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
            return input.Split();
        }
    }
}
