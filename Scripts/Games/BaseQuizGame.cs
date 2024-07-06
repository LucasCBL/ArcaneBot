using System;
using TwitchBot.Scripts.Games.GameUtils;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Games
{
    /// <summary>
    /// Base class for games that require a database
    /// </summary>
    public abstract class BaseQuizGame<T> : BaseGame
    {
        protected int hintTimer = 15;

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
            GenerateData();

            // amount of displayed letters
            int hintLenth = Math.Min(solution.Length - 1, (int)Math.Ceiling(solution.Length * 0.3));
            // we generate the hint 
            string hint = solution[..hintLenth] + new string('_', solution.Length - hintLenth);
            // message displayed if no one wind by the 15 second mark
            StartTimedWarning($"hint: {hint}", hintTimer, cancellationTokenSource.Token);

        }

        /// <summary>
        /// Generates the question, and the solution
        /// </summary>
        private void GenerateData()
        {
            // Solution and question generation
            activeData = gameData.GetRandomData();
            solution = GetSolution(activeData);
            string question = GetQuestion(activeData);
            string[] questionArgs = StringUtils.SplitCommand(question);
            string[] solutionArgs = StringUtils.SplitCommand(solution);
            // if question or answer contains a banned term we ignore it
            if (StringUtils.ContainsBannedWord(questionArgs) || StringUtils.ContainsBannedWord(solutionArgs))
            {
                GenerateData();
                return;
            }
            SendMessage(question);
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
            string[] solutions = { solution.ToLower() , solution.StartsWith("The ") ? solution.Remove(0,4) : solution };
            string guess = message.Message.ToLower();
            if (guess.StartsWith(solutions[0]) || guess.StartsWith(solutions[1])) {
                SendMessage($"@{message.DisplayName} found the correct answer and won " + prize + $" points PogU The answer was {solution}");
                var user = database.FindUserByID(message.UserId);
                user.AddPoints(prize);
                EndGame();
            }
        }

    }
}
