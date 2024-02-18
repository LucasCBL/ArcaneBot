using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Commands;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Games
{
    internal class GameCommand<Game> : IBotCommand where Game : BaseGame
    {
        /// <inheritdoc/>
        public string CommandKey => commandKey;
        /// <inheritdoc/>
        public int MinArgs => 0;
        /// <inheritdoc/>
        public bool IsOnlineCommand => false;
        /// <inheritdoc/>
        public bool IsModeratorCommand => false;
        /// <inheritdoc/>
        private string commandKey;
        /// <inheritdoc/>
        private string[] aliases = {};

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public GameCommand(string gameCommand, string[] aliases = null)
        {
            this.commandKey = gameCommand;
            this.aliases = aliases;
        }

        /// <inheritdoc/>
        public string HelpInfo(Channel channel)
        {
            return "Chat game, use " + channel.commandCharacter + commandKey + " to start a new game. Aliases: " + StringUtils.GetString(GetAliases(channel));
        }

        /// <inheritdoc/>
        public List<string> GetAliases(Channel channel)
        {
            //TODO: ADD CHANNEL ALIASES ONCE CONFIG IS IMPLEMENTED
            List<string> aliases = new List<string>();
            if(this.aliases is not null)
                aliases.AddRange(this.aliases);
            return aliases;
        }

        /// <summary>
        /// Starts the game
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="args"></param>
        public void Execute(User user, Channel channel, ChatMessage message)
        {
            BaseGame game = channel.GetGame<Game>();
            if (game.IsRunning)
            {
                channel.SendMessage($"[{CommandKey}] is already running");
                return;
            }
            game?.StartGame();
        }
    }
}
