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
    public class PointsCommand : IBotCommand
    {
        /// <inheritdoc/>
        public string CommandKey => "points";
        /// <inheritdoc/>
        public int MinArgs => 0;
        /// <inheritdoc/>
        public bool IsOnlineCommand => false;
        /// <inheritdoc/>
        public bool IsModeratorCommand => false;
        /// <inheritdoc/>
        public string HelpInfo => "use !points to check your own points and !points <user> to check another user's points";

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

        /// </inheritdoc>
        public async void Execute(User user, Channel channel, ChatMessage message)
        {
            string[] args = message.Message.Split();
            if(args.Length < 2) {
                channel.SendReply("You currently have " + user.points + " points", message.Id);
                return;
            }
            User targetUser = await database.GetUserByUsername(args[1]);
            // if user is found
            if (targetUser != null)
                channel.SendReply(targetUser.name + " currently has " + targetUser.points + " points", message.Id);
            // user not found
            else
                channel.SendReply("could not find a user with the username " + args[1], message.Id);
        }
    }
}
