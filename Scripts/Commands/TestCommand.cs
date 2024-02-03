using TwitchBot.Scripts.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Scripts.Commands
{
    public class TestCommand : IBotCommand
    {
        public string CommandKey => "ratdetection";

        public int MinArgs => 0;

        public bool IsOnlineCommand => true;

        public bool IsModeratorCommand => false;

        Random rand = new Random();

        public TestCommand()
        {
            // empty by design.
        }

        /// </inheritdoc>
        public void Execute(Channel channel, string[] args)
        {
            if ((args[1] is "1amezra" or $"@1amezra") || rand.Next(100) < 20)
                channel.SendMessage(args[1] + " IS A SECRET RAT SCATTER");
            else
                channel.SendMessage(args[1] + " is not a secret rat OKAY");
        }
    }
}
