using TwitchBot.Scripts.Bot;
using TwitchBot.Scripts.Users;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Commands
{
    interface IBotCommand
    {
        /// <summary>
        /// String needed to activate the command
        /// </summary>
        string CommandKey { get; }

        /// <summary>
        /// Minimum number of arguments needed for the command
        /// </summary>
        int MinArgs { get; }

        /// <summary>
        /// Whether the command can be called in online or not
        /// </summary>
        bool IsOnlineCommand { get; }

        /// <summary>
        /// Whether its a mod only command or not
        /// </summary>
        bool IsModeratorCommand { get; }

        /// <summary>
        /// Gives information about the command when calling !help
        /// </summary>
        public string HelpInfo(Channel channel);

        /// <summary>
        /// Returns all alternative names for this channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public List<string> GetAliases(Channel channel);

        //TODO: ADD A WAY TO MAKE THE INVALID ARGUMENTS MESSAGE GENERAL ACROSS ALL COMMANDS TO REDUCE BOILER PLATE AND REPETITION

        /// <summary>
        /// Calls the command
        /// </summary>
        /// <param name="args"></param>
        /// <param name="channel"></param>
        void Execute(User user, Channel channel, ChatMessage message);
    }
}
