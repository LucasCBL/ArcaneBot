﻿using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Commands
{
    /// <summary>
    /// Returns the top 5 users in the points leaderboard
    /// </summary>
    public class RankCommand : BaseCommand
    {
        /// <inheritdoc/>
        public override string CommandKey => "rank";
        /// <inheritdoc/>
        public override int MinArgs => 0;
        /// <inheritdoc/>
        public override bool IsOnlineCommand => false;
        /// <inheritdoc/>
        public override bool IsModeratorCommand => false;
        /// <inheritdoc/>
        protected override string[] Aliases { get; set; }

        /// <summary>
        /// User database reference
        /// </summary>
        private UserDatabase database;

        public RankCommand(UserDatabase database)
        {
            this.database = database;
        }

        /// <inheritdoc/>
        public override async void Execute(User user, Channel channel, ChatMessage message)
        {
            string[] args = StringUtils.SplitCommand(message.Message);
            User targetUser = user;
            if (args.Length > 1)
            {
                User argUser = await database.GetUserByUsername(args[1]);
                if(argUser is not null)
                    targetUser = argUser;
            }

            string rankingMessage = "Local Point rank: " + (database.GetUserPointRank(targetUser, channel, false) + 1) + " (" + targetUser.points+ "). ";
            rankingMessage += "Local Point loss rank: " + (database.GetUserPointLossRank(targetUser, channel, false) + 1) + " (" + targetUser.pointLoss + "). ";
            rankingMessage += "Local !poof rank: " + (database.GetUserPoofRank(targetUser, channel, false) + 1) + " (" + targetUser.poofCount + "). ";
            channel.SendMessage(rankingMessage);
        }

        /// <inheritdoc/>
        protected override string GetDescription(Channel channel) => $"use {channel.commandCharacter}rank to check your current local ranking (compared against anyone who has chatted in this chat), and {channel.commandCharacter}rank <user> to check another users ranking.";

    }
}
