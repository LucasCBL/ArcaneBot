using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Commands
{
    /// <summary>
    /// Was made as a test command, its an inside joke
    /// </summary>
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

        public RatDetectionCommand()
        {
            // empty by design.
        }

        /// <inheritdoc/>
        public string HelpInfo(Channel channel) => $"use {channel.commandCharacter}ratDetection <username> to check if someone is a rat ";

        /// </inheritdoc>
        public void Execute(User user, Channel channel, ChatMessage message)
        {
            string[] args = StringUtils.SplitCommand(message.Message);
            if (MathUtils.RandomNumber(0, 100) < 20)
                channel.SendMessage(args[1] + " IS A SECRET RAT SCATTER");
            else
                channel.SendMessage(args[1] + " is not a secret rat OKAY");
        }
    }
}
