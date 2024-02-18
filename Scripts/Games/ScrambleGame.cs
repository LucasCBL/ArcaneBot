using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Scripts.Games.GameUtils;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;

namespace TwitchBot.Scripts.Games
{
    /// <summary>
    /// Scramble game, scrambles a word 
    /// </summary>
    public class ScrambleGame : BaseQuizGame<string>
    {
        public ScrambleGame(Action<string> messageSender, UserDatabase database, GameDatabase<string> gameData, int prize = 10) : base(messageSender, database, gameData, prize)
        {
            gameId = "scramble";
        }

        /// <inheritdoc/>
        protected override string GetQuestion(string data)
        {
            return "A new game of scramble has started, unscramble the word: " + ScrambleWord(data);
        }

        /// <inheritdoc/>
        protected override string GetSolution(string data)
        {
            return data;
        }


        /// <summary>
        /// Scrambles a word
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        static string ScrambleWord(string word)
        {
            // word needs at least 2 different characters to be scrambled
            if(word.Distinct().Count() < 1)
                return word;

            // Convert the word to a character array
            char[] characters = word.ToCharArray();

            // Shuffle the characters using Fisher-Yates shuffle algorithm
            for (int i = 0; i < characters.Length; i++)
            {
                int randomIndex = MathUtils.RandomNumber(i, characters.Length);
                // Swap characters
                char temp = characters[randomIndex];
                characters[randomIndex] = characters[i];
                characters[i] = temp;
            }
            string result = new (characters);

            // Convert the character array back to a string
            return result != word ? result : ScrambleWord(word);
        }
    }
}
