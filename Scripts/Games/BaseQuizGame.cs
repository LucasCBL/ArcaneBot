using TwitchBot.Scripts.Games.GameUtils;
using TwitchBot.Scripts.Users;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Games
{
    /// <summary>
    /// Base class for games that require a database
    /// </summary>
    public abstract class BaseQuizGame<T> : BaseGame
    {
        private const int hintTimer = 15;

        /// <summary>  </summary>
        protected T activeData;
        /// <summary>  </summary>
        protected string solution;
        /// <summary>  </summary>
        public int prize;
        /// <summary>  </summary>
        protected GameDatabase<T> gameData;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageSender"></param>
        /// <param name="database"></param>
        /// <param name="gameData"></param>
        protected BaseQuizGame(Action<string> messageSender, UserDatabase database, GameDatabase<T> gameData, int prize = 10) : base(messageSender, database)
        {
            this.gameData = gameData;
            this.prize = prize;
            this.maxDuration = 30;
        }

        /// <inheritdoc/>
        public override void CancelGame()
        {
            base.CancelGame();
            SendMessage("No one found the answer PogO the correct answer was " + solution);
        }

        /// <inheritdoc/>
        public override void StartGame()
        {
            base.StartGame();

            // Solution and question generation
            activeData = gameData.GetRandomData();
            solution = GetSolution(activeData);
            SendMessage(GetQuestion(activeData));

            // amount of displayed letters
            int hintLenth = solution.Length > 5 ? 3 : 2;
            // we generate the hint 
            string hint = solution[..hintLenth] + new string('_', solution.Length - hintLenth);
            // message displayed if no one wind by the 15 second mark
            StartTimedWarning($"hint: {hint}", BaseQuizGame<T>.hintTimer, cancellationTokenSource.Token);

        }

        /// <summary>
        /// Returns the solution corresponding to the data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract string GetSolution(T data);

        /// <summary>
        /// Returns the question corresponding to the data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract string GetQuestion(T data);

        /// <inheritdoc/>
        protected override void CheckInput(ChatMessage message)
        {
            if(message.Message.ToLower().StartsWith(solution.ToLower())) {
                SendMessage($"@{message.DisplayName} found the correct answer and won 10 points PogU The answer was {solution}");
                var user = database.FindUserByID(message.UserId);
                user.AddPoints(prize);
                EndGame();
            }
        }

    }
}
