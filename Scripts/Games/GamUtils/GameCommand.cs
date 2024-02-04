using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Commands;

namespace TwitchBot.Scripts.Games
{
    internal class GameCommand<Game> : IBotCommand where Game : BaseGame
    {
        /// <summary>  </summary>
        public string CommandKey => commandKey;
        /// <summary>  </summary>
        public int MinArgs => 0;
        /// <summary>  </summary>
        public bool IsOnlineCommand => false;
        /// <summary>  </summary>
        public bool IsModeratorCommand => false;

        private string commandKey;

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
        public void Execute(Channel channel, string[] args)
        {
            BaseGame game = channel.GetGame<Game>();
            game?.StartGame();
        }
    }
}
