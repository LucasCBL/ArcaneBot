
namespace TwitchBot.Scripts.Utils
{
    internal class MathUtils
    {
        /// <summary> Random number generator, initialized to a random seed </summary>
        static readonly Random rand = new(Environment.TickCount);

        /// <summary>
        /// Returns random number in range
        /// </summary>
        /// <param name="min">included, min value</param>
        /// <param name="max">not included, max value</param>
        /// <returns></returns>
        public static int RandomNumber(int min, int max) => rand.Next(min, max);
    }
}
