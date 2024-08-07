﻿using TwitchBot.Scripts.Games;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Bot
{
    public class Channel
    {
        /// <summary> max amount of characters per message </summary>
        public const int CharacterLimit = 500;
        /// <summary> Twitch channel name </summary>
        public string channelName { get; set; }
        /// <summary> Twitch channel name </summary>
        public string channelId { get; set; }
        /// <summary> allows channel to override default ! character to invoke commands </summary>
        public char commandCharacter { get; set; } = '#';
        /// <summary> offline status </summary>
        public bool isOffline = true;
        /// <summary> Twitch Client, handles all channel interactions </summary>
        public TwitchClient client;
        /// <summary> game list </summary>
        private List<BaseGame> games = new();
        /// <summary> Amount of time needed before user is able to use a command again</summary>
        public float commandCooldown { get; set; } = 2f;
        /// <summary> Min time between messages in current channel, expressed in ms </summary>
        public const int chatDelay = 500;
        /// <summary> Whether or not games are currently enabled in this channel, this is a configuration </summary>
        public bool gamesDisabled { get; set; } = false;
        /// <summary> Whether or not games are currently enabled in this channel, this value is temporary, not a config</summary>
        public bool gamesEnabled = true;


        float totalRating = 0;
        List<User> ratedBy = new();
        DateTime lastRating = DateTime.Now;
        CancellationTokenSource ratingCancelToken = new();
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"></param>
        /// <param name="channelName"></param>
        public Channel( TwitchClient client, string channelName, string channelId)
        {
            this.channelName = channelName;
            this.client = client;
            this.channelId = channelId;
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
        /// Sends a message to the channel
        /// </summary>
        /// <param name="text"></param>
        /// <exception cref="NotImplementedException"></exception>
        public async void SendMessage(string text)
        {
            // TODO: ADD MESASGE QUEUE SO THAT MESSAGES ARE NEVER SKIPPED DUE TO USER ACTIVITY BEING TOO HIGH
            // If message is too long we split it across multiple chats
            if(text.Length > CharacterLimit) {
                string[] textArray = StringUtils.SplitByCount(text, CharacterLimit);
                for(int i = 0; i < textArray.Length; i++)
                {
                    client.SendMessage(channelName, textArray[i]);
                    await Task.Delay(chatDelay);
                }
            }
            // If message is less than character limit we send it all at once
            else
                client.SendMessage(channelName, text);
        }

        /// <summary>
        /// Sends a message to the channel
        /// </summary>
        /// <param name="text"></param>
        /// <exception cref="NotImplementedException"></exception>
        public async void SendReply(string text, ChatMessage replyingTo)
        {
            // TODO: ADD MESASGE QUEUE SO THAT MESSAGES ARE NEVER SKIPPED DUE TO USER ACTIVITY BEING TOO HIGH
            client.SendReply(channelName, replyingTo.Id, text);
        }

        /// <summary>
        /// Action called when conencted to channel
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

        internal void RatingHandler(float rating, User user)
        {
            if ((DateTime.Now - lastRating).Seconds > 15)
            {
                totalRating = rating;
                ratedBy.Clear();
                ratedBy.Add(user);
            } 
            else if(ratedBy.Contains(user))
            {
                return;
            }
            else
            {   
                totalRating += rating;
                ratedBy.Add(user);
            }

            ratingCancelToken.Cancel();
            ratingCancelToken = new();
            lastRating = DateTime.Now;
            if (ratedBy.Count >= 3)
                TimerUtils.StartTimedAction(20, UpdateRating, ratingCancelToken.Token);
        }

        public void UpdateRating()
        {
            SendMessage("A total of " + ratedBy.Count + " users have rated this segment. Rating score: " + (totalRating / ratedBy.Count) + "/10");
        }
    }
}
