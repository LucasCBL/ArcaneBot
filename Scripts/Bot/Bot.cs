using Microsoft.Extensions.Logging;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using TwitchLib.Api.Services;
using TwitchLib.Api;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using Stream = TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream;
using TwitchBot.Scripts.Commands;
using TwitchBot.Scripts.Games;
using TwitchBot.Scripts.Users;
using User = TwitchBot.Scripts.Users.User;

namespace TwitchBot.Scripts.Bot
{
    /// <summary>
    /// Bot class, main class of the system
    /// </summary>
    class Bot
    {
        // ---------- Constants -------------
        /// <summary> Bot username </summary>
        private string username;
        /// <summary> Bot user ID </summary>
        private string botID;
        /// <summary> Bot access token </summary>
        private string botAccessToken;

        // TODO: Create channel config files to allow for individual channel commands / games
        /// <summary> Channels that the bot will listen to </summary>
        private List<string> channelNames;

        /// <summary> Channel storage </summary>
        private Dictionary<string, Channel> channels = new();

        /// <summary> Database for the bot </summary>
        private UserDatabase database;

        /// <summary> Whether the bot is connected to twitch or not </summary>
        private bool isConnected = false;

        // ---------- Handlers -------------
        /// <summary> Twitch API handler </summary>
        private TwitchAPI api;

        /// <summary> All usable commands </summary>
        private List<IBotCommand> commands = new ();

        /// <summary> Cancellation token source </summary>
        private CancellationTokenSource tokenSource;

        /// <summary>
        /// Constructor
        /// </summary>
        public Bot(string username, string botID, string botAccessToken, List<string> channelNames, string databasePath)
        {
            this.username = username;
            this.botID = botID;
            this.botAccessToken = botAccessToken;
            this.channelNames = channelNames;
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            tokenSource = new CancellationTokenSource();
            // We set up the client to listen and write to the channel chat
            SetUpClient(factory);

            // We setup api, task is discarded because we dont want to wait for result;
            _ = SetupApi(factory);

            database = UserDatabase.LoadDatabase(api, databasePath);
            AddCommand(new RatDetectionCommand());
            AddCommand(new GameCommand<CoinGame>("coingame"));
            AddCommand(new PointsCommand(database));
            AddCommand(new GivePointsCommand(database));
            AddCommand(new RouletteCommand());
            AutoSaveLoop(200000, tokenSource.Token);
        }

        /// <summary>
        /// Initiates the autosave loop, which saves the bot data every <paramref name="saveMsDelay"/> miliseconds
        /// </summary>
        /// <param name="saveMsDelay"></param>
        /// <param name="token"></param>
        private async void AutoSaveLoop(int saveMsDelay, CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                await Task.Delay(saveMsDelay, token);
                Save();
            }
        }

        /// <summary>
        /// Adds an active command to the bot.
        /// </summary>
        /// <param name="command"></param>
        public void AddCommand(IBotCommand command)
        {
            commands.Add(command);
        }

        /// <summary>
        /// Cancels the bot processes and saves everything
        /// </summary>
        public void StopBot()
        {
            Save();
            tokenSource.Cancel();
        }

        /// <summary>
        /// Saves all relevant data
        /// </summary>
        public void Save()
        {
            database.SaveDatabase();
        }

        /// <summary>
        /// Sets up the client to allow us to chat
        /// </summary>
        private async void SetUpClient(ILoggerFactory factory)
        {
            ConnectionCredentials credentials = new ConnectionCredentials(username, botAccessToken);
            ClientOptions clientOptions = new();
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            TwitchClient client = new TwitchClient(customClient, ClientProtocol.WebSocket/*, logger: factory.CreateLogger<TwitchClient>()*/);
            client.Initialize(credentials);
            client.Connect();
            client.OnMessageReceived += OnMessageReceived;
            client.OnJoinedChannel += OnJoinedChannel;// We initialize channels
            client.OnConnected += OnConnected;
            while(!isConnected)
                await Task.Delay(1000);

            foreach (string channel in channelNames) {
                channels[channel] = new(client, channel);
                channels[channel].AddGame(new CoinGame(channels[channel].SendMessage, database));
            }

            OnlineStreamsCheck();
        }

        /// <summary>
        /// Action called on channel joined
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("joined channel " + e.Channel + " successfully");
        }

        /// <summary>
        /// Action called once twitch client is connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnected(object sender, OnConnectedArgs e)
        {
            isConnected = true;
        }

        /// <summary>
        /// Sets up the twitch api that allows us to check channel status
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        private async Task SetupApi(ILoggerFactory factory)
        {
            api = new(loggerFactory: factory);
            api.Settings.ClientId = botID;
            api.Settings.AccessToken = botAccessToken;

            LiveStreamMonitorService monitorService = new(api);
            monitorService.SetChannelsByName(channelNames);

            // We add listeners for changes to stream state
            monitorService.OnStreamOffline += ApiOnStreamOffline;
            monitorService.OnStreamOnline -= ApiOnStreamOnline;
        }

        /// <summary>
        /// Function called when stream changes to offline status
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void ApiOnStreamOffline(object source, OnStreamOfflineArgs args)
        {
            channels[args.Channel.ToLower()].isOffline = true;
        }

        /// <summary>
        /// Function called when stream changes to online status
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void ApiOnStreamOnline(object source, OnStreamOnlineArgs args)
        {
            channels[args.Channel.ToLower()].isOffline = false;
        }

        /// <summary>
        /// Checks whether the monitored stream is online or not
        /// </summary>
        /// <returns></returns>
        public async void OnlineStreamsCheck()
        {
            GetStreamsResponse streams = await api.Helix.Streams.GetStreamsAsync(userIds: channelNames, userLogins: channelNames);
            
            // We add each online stream to the list of online streams.
            foreach (Stream onlineStream in streams.Streams)
                channels[onlineStream.UserName.ToLower()].isOffline = false;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Channel channel = channels[e.ChatMessage.Channel.ToLower()];
            User user = database.FindOrAddUserByID(e.ChatMessage.UserId, e.ChatMessage.Username);
            string message = e.ChatMessage.Message;
            if (message[0] == '!')
            {
                Console.WriteLine("command called: " + message);
                IBotCommand calledCommand = null;
                string content = message.Substring(1);
                string[] args = content.Split(' ');
                string commandKey = args[0].ToLower();
                args[0] = e.ChatMessage.Username;
                // we find the command in our command list
                foreach (IBotCommand command in commands)
                    if(command.CommandKey == commandKey)
                        calledCommand = command;

                // If no command is found we return
                if (calledCommand is null || (!channel.isOffline && !calledCommand.IsOnlineCommand))
                {
                    Console.WriteLine("command was not found or not compatible with current channel state");
                    return;
                }

                calledCommand.Execute(user, channel, e.ChatMessage);
            }
            else
            {
                channel.CheckActiveGames(e.ChatMessage);
            }
        }
    }
}