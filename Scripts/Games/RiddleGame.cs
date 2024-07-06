using TwitchBot.Scripts.Games.GameUtils;
using TwitchBot.Scripts.Users;

namespace TwitchBot.Scripts.Games
{
    /// <summary>
    /// Riddle game, rewards only 5 points since the database is so small
    /// </summary>
    public class RiddleGame : BaseQuizGame<Riddle>
    {
        public RiddleGame(Action<string> messageSender, UserDatabase database, GameDatabase<Riddle> gameData, int prize = 20) : base(messageSender, database, gameData, prize)
        {
            gameId = "riddle";
            this.hintTimer = 30;
            this.maxDuration = 60;
        }

        /// <inheritdoc/>
        protected override string GetQuestion(Riddle data)
        {
            return data.Question;
        }

        /// <inheritdoc/>
        protected override string GetSolution(Riddle data)
        {
            return data.Answer;
        }
    }

    /// <summary>
    /// A trivia object, contains question and answer
    /// </summary>
    public class Riddle
    {
        /// <summary>  Trivia question </summary>
        public string Question { get; set; }
        /// <summary> Trivia answer </summary>
        public string Answer { get; set; }
    }
}

