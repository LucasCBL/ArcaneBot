using TwitchBot.Scripts.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// 
        /// </summary>
        bool IsModeratorCommand { get; }

        /// <summary>
        /// Calls the command
        /// </summary>
        /// <param name="args"></param>
        /// <param name="channel"></param>
        void Execute(Channel channel, string[] args);
    }
}
