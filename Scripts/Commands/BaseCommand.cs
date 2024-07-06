using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Commands
{
    public abstract class BaseCommand : IBotCommand
    {
        /// <inheritdoc/>
        public abstract string CommandKey { get; }
        /// <inheritdoc/>
        public abstract int MinArgs { get; }
        /// <inheritdoc/>
        public abstract bool IsOnlineCommand { get; }
        /// <inheritdoc/>
        public abstract bool IsModeratorCommand { get; }

        /// <summary>
        /// Returns general aliases for this command, these are not determined by channels
        /// </summary>
        protected abstract string[] Aliases { get; set; }

        /// <summary>
        /// Returns a description for the command.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        protected abstract string GetDescription(Channel channel);

        /// <inheritdoc/>
        public abstract void Execute(User user, Channel channel, ChatMessage message);

        /// <inheritdoc/>
        public List<string> GetAliases(Channel channel)
        {
            //TOD: ADD CHANNEL ALIASES ONCE CONFIG IS IMPLEMENTED
            List<string> aliases = new List<string>();
            if (Aliases is not null)
                aliases.AddRange(Aliases);
            return aliases;
        }

        /// <inheritdoc/>
        public string HelpInfo(Channel channel)
        {
            string aliasList = StringUtils.GetString(GetAliases(channel));
            aliasList = aliasList.Length > 1 ? aliasList : "no aliases";
            return GetDescription(channel) + ". Aliases: " + aliasList;
        }
    }
}
