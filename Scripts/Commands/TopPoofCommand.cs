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
    public class TopPoofCommand : BaseCommand
    {
        /// <inheritdoc/>
        public override string CommandKey => "toppoof";
        /// <inheritdoc/>
        public override int MinArgs => 0;
        /// <inheritdoc/>
        public override bool IsOnlineCommand => false;
        /// <inheritdoc/>
        public override bool IsModeratorCommand => false;
        /// <inheritdoc/>
        protected override string[] Aliases { get; set; } = { "poofers", "pooftop", "poofleaderboard" };

        /// <summary>
        /// User database reference
        /// </summary>
        private UserDatabase database;

        public TopPoofCommand(UserDatabase database)
        {
            this.database = database;
        }

        /// <inheritdoc/>
        public override void Execute(User user, Channel channel, ChatMessage message)
        {
            var args = StringUtils.SplitCommand(message.Message);
            bool globalTop = (args.Length > 1 && args[1] == "global");
            List<User> top5 = database.GetTopPoofs(channel, globalTop);
            string topText = "Current" + (globalTop ? " global" : string.Empty) + " !poof leaderboard top 5 is: ";

            for (int i = 0; i < top5.Count; i++)
            {
                topText += (i + 1) + ": " + top5[i].name + " (" + top5[i].poofCount + "). ";
            }
            channel.SendMessage(topText);
        }

        /// <inheritdoc/>
        protected override string GetDescription(Channel channel) => $"use {channel.commandCharacter}toppoof to check the current top 5 users in the point ranking. Use {channel.commandCharacter}toppoof global to see global rankings";

    }
}

