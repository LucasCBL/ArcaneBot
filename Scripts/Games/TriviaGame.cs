using TwitchBot.Scripts.Games.GameUtils;
using TwitchBot.Scripts.Users;

namespace TwitchBot.Scripts.Games
{
    /// <summary>
    /// Trivia game
    /// </summary>
    public class TriviaGame : BaseQuizGame<Trivia>
    {
        public TriviaGame(Action<string> messageSender, UserDatabase database, GameDatabase<Trivia> gameData, int prize = 10) : base(messageSender, database, gameData, prize)
        {
            gameId = "trivia";
        }

        /// <inheritdoc/>
        protected override string GetQuestion(Trivia data)
        {
            return data.Question;
        }

        /// <inheritdoc/>
        protected override string GetSolution(Trivia data)
        {
            return data.Answer;
        }
    }

    /// <summary>
    /// A trivia object, contains question and answer
    /// </summary>
    public class Trivia
    {
        /// <summary>  Trivia question </summary>
        public string Question { get; set; }
        /// <summary> Trivia answer </summary>
        public string Answer { get; set; }
    }
}
