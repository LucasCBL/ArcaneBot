using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;
using User = TwitchBot.Scripts.Users.User;

namespace TwitchBot.Scripts.Games
{
    // TODO IMPROVE THIS, THIS IS JUST A TEST GAME FOR NOW
    // TODO IF NO ONE BETS ON ONE SIDE THE GAME ENTERS GAMBA ULTRA GAMBA MODE, POINTS ARE EITHER DOUBLED OR DESTROYED
    public class CoinGame : BaseGame
    {
        /// <summary> Whether the active throw landed on heads or tails, its determined at the start of the game </summary>
        bool heads;
        /// <summary> List of heads players </summary>
        List<User> headsPlayers = new();
        /// <summary> List of tails players </summary>
        List<User> tailsPlayers = new();


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageSender"></param>
        public CoinGame(Action<string> messageSender, UserDatabase database) : base(messageSender, database) {
            maxDuration = 20;
            gameId = "coingame";
        }

        ///<inheritdoc></inheritdoc>
        public override void StartGame()
        {
            base.StartGame();
            heads = MathUtils.RandomNumber(0, 2) == 0;
            SendMessage("A coin will land in 20 seconds, say heads or tails in chat, winners get 10 points, losers lose 10 points if they have any");
            headsPlayers.Clear();
            tailsPlayers.Clear();
            StartTimedWarning("10 seconds left for coin game", 10, cancellationTokenSource.Token);
        }

        ///<inheritdoc></inheritdoc>
        protected override void CheckInput(ChatMessage message)
        {
            string[] args = message.Message.Split(' ');
            if (args.Length < 1)
                return;
            User user = database.FindOrAddUserByID(message.UserId, message.Username);
            string calledFace = args[0].ToLower();
            Console.WriteLine(args[0]);
            if (calledFace is "heads" or "head" && !tailsPlayers.Contains(user))
            {
                if (!headsPlayers.Contains(user))
                    headsPlayers.Add(user);
            }
            else if (calledFace is "tails" or "tail" && !headsPlayers.Contains(user))
            {
                if (!tailsPlayers.Contains(user))
                    tailsPlayers.Add(user);
                // TODO: point handling
            }
        }

        ///<inheritdoc></inheritdoc>
        public override void CancelGame()
        {
            base.CancelGame();
            string message = " The coin landed on " + (heads ? "heads" : "tails") + ". Winners are: ";
            List<User> winners = heads ? headsPlayers : tailsPlayers;
            List<User> losers = heads ? tailsPlayers : headsPlayers;
            foreach (User winner in winners)
            {
                message += winner.name + ", ";
                winner.AddPoints(10);
            }

            foreach(User loser in losers)
                loser.RemovePoints(loser.points > 10 ? 10: loser.points);

            SendMessage(message);
        }
    }
}
