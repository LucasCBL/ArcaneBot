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
    public class RatDetectionCommand : IBotCommand
    {
        /// <inheritdoc/>
        public string CommandKey => "ratdetection";
        /// <inheritdoc/>
        public int MinArgs => 1;
        /// <inheritdoc/>
        public bool IsOnlineCommand => true;
        /// <inheritdoc/>
        public bool IsModeratorCommand => false;
        /// <inheritdoc/>
        public string HelpInfo => "use !ratDetection <username> to check if someone is a rat ";

        public RatDetectionCommand()
        {
            // empty by design.
        }

        /// </inheritdoc>
        public void Execute(User user, Channel channel, ChatMessage message)
        {
            string[] args = message.Message.Split();
            if ((args[1] is "1amezra" or $"@1amezra") || MathUtils.RandomNumber(0, 100) < 20)
                channel.SendMessage(args[1] + " IS A SECRET RAT SCATTER");
            else
                channel.SendMessage(args[1] + " is not a secret rat OKAY");
        }
    }
}
