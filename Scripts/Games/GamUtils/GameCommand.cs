using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Commands;
using TwitchBot.Scripts.Users;
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
        public string HelpInfo => "Chat game, use " + commandKey + " to start a new game";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public GameCommand(string gameCommand)
        {
            this.commandKey = gameCommand;
        }

        /// <summary>
        /// Starts the game
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="args"></param>
        public void Execute(User user, Channel channel, ChatMessage message)
        {
            BaseGame game = channel.GetGame<Game>();
            game?.StartGame();
        }
    }
}
