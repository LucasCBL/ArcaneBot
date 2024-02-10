using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Commands
{
    /// <summary>
    /// !Roulette <points> command, made so gamba addicts can lose their points
    /// </summary>
    public class RouletteCommand: IBotCommand
    {
        /// <summary> Message to show the game so as to not confuse chatter </summary>
        private const string gameIntroMessage = "[Roulette] ";

        /// <inheritdoc/>
        public string CommandKey => "roulette";
        /// <inheritdoc/>
        public int MinArgs => 1;
        /// <inheritdoc/>
        public bool IsOnlineCommand => false;
        /// <inheritdoc/>
        public bool IsModeratorCommand => false;
        /// <inheritdoc/>
        public string HelpInfo => "use !roulette <points> bet your points, there is a 50/50 chance to either double the points or lose them, you can use !roulette <points>% to bet a percentage of your points or use !roulette all or !roulette half to bet all or half of your points";


        /// <summary>
        /// Constructor
        /// </summary>
        public RouletteCommand()
        {
        }

        /// </inheritdoc>
        public async void Execute(User user, Channel channel, ChatMessage message)
        {
            int points = -1;
            string[] args = message.Message.Split();
            string bet = args[1].ToLower();
            bool win = MathUtils.RandomNumber(0, 2) == 1;

            // case: all points
            if (bet == "all")
                points = user.points;
            // case: half points
            else if (bet == "half")
                points = user.points / 2;
            // case: % of points
            else if (bet[^1] == '%' && float.TryParse(bet[..^1], out float multiplier))
                points = (int)Math.Floor(user.points * (multiplier / 100f));
            // case: int amount of points
            else if (int.TryParse(bet, out int parsedPoints))
                points = parsedPoints;

            // message handling
            string result = gameIntroMessage + GenerateResultMessage(user, points, win);
            channel.SendReply(result, message.Id);

            // if invalid no need to assign points
            if (points < 0 || points > user.points)
                return;
            
            // points assignment
            if(win)
                user.AddPoints(points);
            else
                user.RemovePoints(points);
        }

        /// <summary>
        /// Generates result message
        /// </summary>
        /// <param name="user"></param>
        /// <param name="points"></param>
        /// <param name="win"></param>
        /// <returns></returns>
        private string GenerateResultMessage(User user, int points, bool win)
        {
            if (points < 0)
                return "invalid arguments. " + HelpInfo;
            else if (points > user.points)
                return "You do not have enough points PogO";
            else
                return "You have " + (win ? $"won {points} points Pog " : $"lost {points} points PogO ") + "new balance: " + user.points;
        }
    }
}
