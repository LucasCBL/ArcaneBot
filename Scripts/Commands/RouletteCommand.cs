using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;

namespace TwitchBot.Scripts.Commands
{
    /// <summary>
    /// !Roulette <points> command, made so gamba addicts can lose their points
    /// </summary>
    public class RouletteCommand: IBotCommand
    {
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
        public async void Execute(User user, Channel channel, string[] args)
        {
            int points;
            string bet = args[1].ToLower();
            bool win = MathUtils.RandomNumber(0, 2) == 1;
            if (bet == "all")
                points = user.points;
            else if (bet == "half")
                points = user.points / 2;
            else if (bet[^1] == '%' && float.TryParse(bet[..^1], out float multiplier))
                points = (int)Math.Floor(user.points * (multiplier / 100f));
            // We check that the argument is in the correct format
            else if (int.TryParse(bet, out int parsedPoints))
                points = parsedPoints;
            else
            {
                channel.SendMessage("invalid arguments. " + HelpInfo);
                return;
            }

            // If points is less than 0 its an invalid argument
            if (points < 0)
                channel.SendMessage("invalid arguments. " + HelpInfo);
            else if (points > user.points)
                channel.SendMessage("You do not have enough points PogO");
            else if (win)
            {
                channel.SendMessage("You have won " + points + " points Pog");
                user.AddPoints(points);
            }
            else
            {
                channel.SendMessage("You have lost " + points + " points PogO");
                user.RemovePoints(points);
            }
        }
    }
}
