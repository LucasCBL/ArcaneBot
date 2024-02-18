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
    /// !guve command, allows a user to give points to another user
    /// </summary>
    public class GivePointsCommand : IBotCommand
    {
        /// <inheritdoc/>
        public string CommandKey => "give";
        /// <inheritdoc/>
        public int MinArgs => 2;
        /// <inheritdoc/>
        public bool IsOnlineCommand => false;
        /// <inheritdoc/>
        public bool IsModeratorCommand => false;
        /// <inheritdoc/>
        private UserDatabase database;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database"></param>
        public GivePointsCommand(UserDatabase database)
        {
            this.database = database;
        }

        /// <inheritdoc/>
        public string HelpInfo(Channel channel) =>  $"Use {channel.commandCharacter}give <user> <points> to give points to <user>";

        /// </inheritdoc>
        public async void Execute(User user, Channel channel, ChatMessage message)
        {
            string[] args = StringUtils.SplitCommand(message.Message);
            User gifter = user;
            User receiver = await database.GetUserByUsername(args[1]);

            // If user not found we return
            if(receiver is null)
            {
                channel.SendReply("user not found", message);
                return;
            }

            // We check that the argument is in the correct format
            if(int.TryParse(args[2], out int points))
            {
                // Points check
                if(gifter.points < points)
                {
                    channel.SendReply("Awkward you do not have enough points for this " + gifter.name + " you only have " + gifter.points + " points", message);
                    return;
                }

                gifter.GivePoints(points, receiver);
                channel.SendReply("Giving " + points + " points to " + receiver.name, message);
                return;
            }

            // if points format is incorrect
            channel.SendReply("invalid arguments. " + HelpInfo, message);
        }
    }
}
