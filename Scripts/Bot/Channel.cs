using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchBot.Scripts.Games;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Bot
{
    public class Channel
    {
        /// <summary> Twitch channel name </summary>
        public string channelName;
        /// <summary> offline status </summary>
        public bool isOffline = true;
        /// <summary> Twitch Client, handles all channel interactions </summary>
        public TwitchClient client;
        /// <summary> game list </summary>
        private List<BaseGame> games = new();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"></param>
        /// <param name="channelName"></param>
        public Channel( TwitchClient client, string channelName)
        {
            this.channelName = channelName;
            this.client = client;
            SetupClient();
        }

        /// <summary>
        /// Sets up the client for this channel, makes it join the channel
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void SetupClient()
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            Console.WriteLine("Trying to join channel " + channelName);
            client.JoinChannel(channelName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <exception cref="NotImplementedException"></exception>
        public async void SendMessage(string text)
        {
            // TODO: ADD MESASGE QUEUE SO THAT MESSAGES ARE NEVER SKIPPED DUE TO USER ACTIVITY BEING TOO HIGH
            client.SendMessage(channelName, text);
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
        /// Returns game ot <typeparamref name="Game"/> type if it exists in the game list
        /// </summary>
        /// <typeparam name="Game"></typeparam>
        /// <returns></returns>
        public BaseGame GetGame<Game>() where Game : BaseGame
        {
            // We look in tha game list to see if the game is already there
            foreach (BaseGame game in games)
                if (game is Game)
                    return game;

            return null;
        }

        /// <summary>
        /// Checks all currently active games
        /// </summary>
        /// <param name="chatMessage"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void CheckActiveGames(ChatMessage chatMessage)
        {
            foreach(BaseGame game in games)
                if (game.IsRunning)
                    game.RunCheckInput(chatMessage);
        }

        /// <summary>
        /// Adds game to game list
        /// </summary>
        /// <param name="game"></param>
        public void AddGame(BaseGame game) => games.Add(game);
    }
}
