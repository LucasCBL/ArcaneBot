using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace TwitchBot.Scripts.Bot
{
    public class Channel
    {
        /// <summary> Twitch Client, handles all channel interactions </summary>
        public TwitchClient client;

        /// <summary> Twitch channel name </summary>
        public string chanNelName;

        /// <summary> offline status </summary>
        public bool isOffline = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"></param>
        /// <param name="channelName"></param>
        public Channel( TwitchClient client, string channelName)
        {
            this.chanNelName = channelName;
            this.client = client;
            SetupClient();
        }

        private void SetupClient()
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            Console.WriteLine("Trying to join channel " + chanNelName);
            client.JoinChannel(chanNelName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <exception cref="NotImplementedException"></exception>
        public async void SendMessage(string text)
        {
            // TODO: ADD MESASGE QUEUE SO THAT MESSAGES ARE NEVER SKIPPED DUE TO USER ACTIVITY BEING TOO HIGH
            client.SendMessage(chanNelName, text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientOnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientOnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Channel joined: " + e.Channel);
        }
    }
}
