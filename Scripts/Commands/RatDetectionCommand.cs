using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Commands
{
    /// <summary>
    /// Was made as a test command, its an inside joke
    /// </summary>
    public class RatDetectionCommand : BaseCommand
    {
        /// <inheritdoc/>
        public override string CommandKey => "ratdetection";
        /// <inheritdoc/>
        public override int MinArgs => 1;
        /// <inheritdoc/>
        public override bool IsOnlineCommand => false;
        /// <inheritdoc/>
        public override bool IsModeratorCommand => false;
        /// <inheritdoc/>
        protected override string[] Aliases { get; set; } = { "rat" };

        public RatDetectionCommand()
        {
            // empty by design.
        }

        /// <inheritdoc/>
        protected override string GetDescription(Channel channel) => $"use {channel.commandCharacter}ratDetection <username> to check if someone is a rat ";

        /// </inheritdoc>
        public override void Execute(User user, Channel channel, ChatMessage message)
        {
            string content = StringUtils.RemoveCommandFromString(message.Message);

            if (MathUtils.RandomNumber(0, 100) < 20)
                channel.SendMessage(content + " IS A SECRET RAT SCATTER");
            else
                channel.SendMessage(content + " is not a secret rat OKAY");
        }
    }
}
