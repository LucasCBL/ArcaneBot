
namespace TwitchBot.Scripts.Utils
{
    internal class MathUtils
    {
        static readonly Random rand = new();

        /// <summary>
        /// Returns random number in range
        /// </summary>
        /// <param name="min">included, min value</param>
        /// <param name="max">not included, max value</param>
        /// <returns></returns>
        public static int RandomNumber(int min, int max) => rand.Next(min, max);
    }
}
