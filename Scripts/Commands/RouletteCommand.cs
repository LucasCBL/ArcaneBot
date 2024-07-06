using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Commands
{
    /// <summary>
    /// !Roulette <points> command, made so gamba addicts can lose their points
    /// </summary>
    public class RouletteCommand: BaseCommand
    {
        /// <summary> Message to show the game so as to not confuse chatter </summary>
        private const string gameIntroMessage = "[Roulette] ";

        /// <inheritdoc/>
        public override string CommandKey => "roulette";
        /// <inheritdoc/>
        public override int MinArgs => 1;
        /// <inheritdoc/>
        public override bool IsOnlineCommand => false;
        /// <inheritdoc/>
        public override bool IsModeratorCommand => false;
        /// <inheritdoc/>
        protected override string[] Aliases { get; set; } = { "gamba", "bet" };

        /// <summary>
        /// Constructor
        /// </summary>
        public RouletteCommand()
        {
        }

        /// <inheritdoc/>
        protected override string GetDescription(Channel channel) => $"use {channel.commandCharacter}roulette <points> bet your points, there is a 50/50 chance to either double the points or lose them, you can use {channel.commandCharacter}roulette <points>% to bet a percentage of your points or use {channel.commandCharacter}roulette all or {channel.commandCharacter}roulette half to bet all or half of your points";

        /// </inheritdoc>
        public override async void Execute(User user, Channel channel, ChatMessage message)
        {
            int points = -1;
            string[] args = StringUtils.SplitCommand(message.Message);
            string bet = args[1].ToLower();
            // Changes nothing as opposed to 0,1 random, but feels cooler this way shrug
            bool win = MathUtils.RandomNumber(0, 2) == 1;
            char lastChar = bet[^1]; 
            int userPoints = user.points;

            // case: all points
            if (bet == "all")
                points = userPoints;
            // case: half points
            else if (bet == "half")
                points = userPoints / 2;
            // case: % of points or points expressed as xxx K(as in thousands)
            else if ((lastChar is '%' or 'k') && float.TryParse(bet[..^1], out float number))
                points = (int)Math.Floor(lastChar == '%' ? (userPoints * (number / 100f)) : (number * 1000));
            // case: int amount of points
            else if (int.TryParse(bet, out int parsedPoints))
                points = parsedPoints;

            // if invalid no need to assign points
            if (points > 0 && points <= userPoints)
            {
                // points assignment
                if(win)
                    user.AddPoints(points);
                else
                    user.RemovePoints(points, true);
            }

            // message handling
            string result = gameIntroMessage + GenerateResultMessage(user, channel, userPoints, points, win);
            channel.SendReply(result, message);
        }

        /// <summary>
        /// Generates result message
        /// </summary>
        /// <param name="user"></param>
        /// <param name="points"></param>
        /// <param name="win"></param>
        /// <returns></returns>
        private string GenerateResultMessage(User user, Channel channel, int userPoints, int points, bool win)
        {
            if (points < 0)
                return "invalid arguments. " + HelpInfo(channel);
            else if (points > userPoints)
                return "You do not have enough points PogO";
            else
                return "You have " + (win ? $"won {points} points elbyBlom " : $"lost {points} points PogO ") + "new balance: " + user.points;
        }
    }
}
