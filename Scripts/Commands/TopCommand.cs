using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Users;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Commands
{
    /// <summary>
    /// Returns the top 5 users in the points leaderboard
    /// </summary>
    public class TopCommand : BaseCommand
    {
        /// <inheritdoc/>
        public override string CommandKey => "top";
        /// <inheritdoc/>
        public override int MinArgs => 0;
        /// <inheritdoc/>
        public override bool IsOnlineCommand => false;
        /// <inheritdoc/>
        public override bool IsModeratorCommand => false;
        /// <inheritdoc/>
        protected override string[] Aliases { get; set; } = { "leaderboard", "winners" };

        /// <summary>
        /// User database reference
        /// </summary>
        private UserDatabase database;

        public TopCommand(UserDatabase database)
        { 
            this.database = database;
        }

        /// <inheritdoc/>
        public override void Execute(User user, Channel channel, ChatMessage message)
        {
            List<User> top5 = database.GetTopPoints();
            string topText = "Current point leaderboard top 5 is: ";

            for (int i = 0; i < top5.Count; i++)
            {
                topText += (i + 1) + " - " + top5[i].name + " - " + top5[i].points + ". ";
            }
            channel.SendMessage(topText);
        }

        /// <inheritdoc/>
        protected override string GetDescription(Channel channel) => $"use {channel.commandCharacter}top to check the current top 5 users in the point ranking.";

    }
}
