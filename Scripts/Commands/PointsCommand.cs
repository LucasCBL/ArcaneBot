using TwitchBot.Scripts.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;

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
        public async void Execute(User user, Channel channel, string[] args)
        {
            User targetUser = user;
            if(args.Length >= 2) {
                targetUser = await database.GetUserByUsername(args[1]);
            }
            // TODO REPLY INSTEAD OF MESSAGE
            channel.SendMessage(targetUser.name + " currently has " + targetUser.points + " points");
        }
    }
}
