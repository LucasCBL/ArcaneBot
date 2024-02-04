using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Games
{
    // TODO IMPROVE THIS, THIS IS JUST A TEST GAME FOR NOW
    // TODO IF NO ONE BETS ON ONE SIDE THE GAME ENTERS GAMBA ULTRA GAMBA MODE, POINTS ARE EITHER DOUBLED OR DESTROYED
    public class CoinGame : BaseGame
    {
        bool heads;
        Random random = new();

        List<string> headsPlayers = new();
        List<string> tailsPlayers = new();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageSender"></param>
        public CoinGame(Action<string> messageSender) : base(messageSender) {
            maxDuration = 30;
        }

        ///<inheritdoc></inheritdoc>
        public override void StartGame()
        {
            base.StartGame();
            heads = random.Next(0, 2) == 0;
            SendMessage("A coin will land in 30 seconds, say heads or tails in chat followed by the amount of points you want to bet (points are currently disabled)");
            headsPlayers.Clear();
            tailsPlayers.Clear();
            StartTimedWarning("10 seconds left for coin game", 20, cancellationTokenSource.Token);
        }

        ///<inheritdoc></inheritdoc>
        protected override void CheckInput(ChatMessage message)
        {
            string[] args = message.Message.Split(' ');
            if (args.Length < 1)
                return;

            string calledFace = args[0].ToLower();
            Console.WriteLine(args[0]);
            if (calledFace is "heads" or "head" && !tailsPlayers.Contains(message.Username))
            {
                if (!headsPlayers.Contains(message.Username))
                    headsPlayers.Add(message.Username);
                // TODO: point handling
            }
            else if (calledFace is "tails" or "tail" && !headsPlayers.Contains(message.Username))
            {
                if (!tailsPlayers.Contains(message.Username))
                    tailsPlayers.Add(message.Username);
                // TODO: point handling
            }
        }

        ///<inheritdoc></inheritdoc>
        public override void CancelGame()
        {
            base.CancelGame();
            string message = " The coin landed on " + (heads ? "heads" : "tails") + ". Winners are: ";
            List<string> winners = heads ? headsPlayers : tailsPlayers;
            foreach (string winner in winners)
            {
                message += winner + ", ";
            }
            SendMessage(message);
        }

    }
}
