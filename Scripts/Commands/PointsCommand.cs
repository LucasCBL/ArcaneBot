using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Commands
{
    /// <summary>
    /// !points command, used to check how many points a user has
    /// </summary>
    public class PointsCommand : BaseCommand
    {
        /// <inheritdoc/>
        public override string CommandKey => "points";
        /// <inheritdoc/>
        public override int MinArgs => 0;
        /// <inheritdoc/>
        public override bool IsOnlineCommand => false;
        /// <inheritdoc/>
        public override bool IsModeratorCommand => false;
        /// <inheritdoc/>
        protected override string[] Aliases { get; set; } = { "balance", "bal" };

        /// <summary>
        /// User database used to check info
        /// </summary>
        private UserDatabase database;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database"></param>
        public PointsCommand(UserDatabase database)
        {
            this.database = database;
        }

        /// <inheritdoc/>
        protected override string GetDescription(Channel channel) => $"use {channel.commandCharacter}points to check your own points and {channel.commandCharacter}points <user> to check another user's points";

        /// </inheritdoc>
        public override async void Execute(User user, Channel channel, ChatMessage message)
        {
            string[] args = StringUtils.SplitCommand(message.Message);
            if(args.Length < 2) {
                channel.SendReply("You currently have " + user.points + " points", message);
                return;
            }
            User targetUser = await database.GetUserByUsername(args[1]);
            // if user is found
            if (targetUser != null)
                channel.SendReply(targetUser.name + " currently has " + targetUser.points + " points", message);
            // user not found
            else
                channel.SendReply("could not find a user with the username " + args[1], message);
        }
    }
}
