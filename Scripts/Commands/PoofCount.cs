using TwitchBot.Scripts.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Commands
{
    /// <summary>
    /// !points command, used to check how many points a user has
    /// </summary>
    public class PoofCountCommand : BaseCommand
    {
        /// <inheritdoc/>
        public override string CommandKey => "poofcount";
        /// <inheritdoc/>
        public override int MinArgs => 0;
        /// <inheritdoc/>
        public override bool IsOnlineCommand => true;
        /// <inheritdoc/>
        public override bool IsModeratorCommand => false;
        /// <inheritdoc/>
        protected override string[] Aliases { get; set; }

        /// <summary>
        /// User database used to check info
        /// </summary>
        private readonly UserDatabase database;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database"></param>
        public PoofCountCommand(UserDatabase database)
        {
            this.database = database;
        }

        /// <inheritdoc/>
        protected override string GetDescription(Channel channel) => $"use {channel.commandCharacter}poofCount to check the amount of poofs you have used and {channel.commandCharacter}poofCount <user> to check another user's poofs";

        /// </inheritdoc>
        public override async void Execute(User user, Channel channel, ChatMessage message)
        {
            string[] args = StringUtils.SplitCommand(message.Message);
            if (args.Length < 2)
            {
                channel.SendReply("You currently have used !poof " + user.poofCount + " times", message);
                return;
            }
            User targetUser = await database.GetUserByUsername(args[1]);
            // if user is found
            if (targetUser != null)
                channel.SendReply(targetUser.name + " has used !poof " + targetUser.poofCount + " times", message);
            // user not found
            else
                channel.SendReply("could not find a user with the username " + args[1], message);
        }
    }
}
