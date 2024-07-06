using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Commands
{
    /// <summary>
    /// Generic command for text responses
    /// </summary>
    public class TextCommand : BaseCommand
    {
        /// <inheritdoc/>
        public override string CommandKey => commandKey;

        /// <inheritdoc/>
        public override int MinArgs => 0;

        /// <inheritdoc/>
        public override bool IsOnlineCommand => isOnlineCommand;

        /// <inheritdoc/>
        public override bool IsModeratorCommand => isModeratorCommand;

        /// <inheritdoc/>
        protected override string[] Aliases { get; set; }

        /// <summary>  whether command is online only or not  </summary>
        protected bool isOnlineCommand;
        /// <summary> whether command is mod only or not </summary>
        protected bool isModeratorCommand;
        /// <summary> command key </summary>
        protected string commandKey;
        /// <summary>  Response list for the command </summary>
        protected string[] responses;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandKey"></param>
        /// <param name="responses"></param>
        /// <param name="aliases"></param>
        /// <param name="minArgs"></param>
        /// <param name="isOnlineCommand"></param>
        /// <param name="isModeratorCommand"></param>
        public TextCommand(string commandKey, string[] responses, string[] aliases = null, bool isOnlineCommand = true, bool isModeratorCommand = false)
        {
            this.commandKey = commandKey;
            this.isOnlineCommand = isOnlineCommand;
            this.isModeratorCommand = isModeratorCommand;
            this.responses = responses;
            Aliases = aliases;
        }

        /// <inheritdoc/>
        public override void Execute(User user, Channel channel, ChatMessage message)
        {
            channel.SendReply(responses[MathUtils.RandomNumber(0, responses.Length)], message);
        }

        /// <inheritdoc/>
        protected override string GetDescription(Channel channel)
        {
            return "Returns a random message from a list";
        }
    }
}
