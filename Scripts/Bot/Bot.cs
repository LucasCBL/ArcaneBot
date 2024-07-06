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
using Stream = TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream;
using TwitchBot.Scripts.Commands;
using TwitchBot.Scripts.Games;
using TwitchBot.Scripts.Users;
using User = TwitchBot.Scripts.Users.User;
using TwitchBot.Scripts.Utils;
using TwitchBot.Scripts.Games.GameUtils;
using TwitchLib.Communication.Interfaces;

namespace TwitchBot.Scripts.Bot
{
    /// <summary>
    /// Bot class, main class of the system
    /// </summary>
    class Bot
    {
        // ---------- Constants -------------
        private const string helpIntro = "The commands enabled in this channel are: ";
        private const string bannedWordReply = "Something in you message was identified as a possible slur, please contact a mod or the bot administrator for an explanation if this is a mistake";
        private const string commandCalledMessage = "command called: ";
        private const string poof = "!poof";
        private const string commandNotFound = "command was not found or not compatible with current channel state";
        private const string modCommandsIntro = "Mod commands are: ";
        private List<string> helpCommandAliases = new() { "help", "h" };
        private List<string> bannedUsers = new() { "fossabot" };

        ///<summary> Embezzle command constant </summary>
        private readonly string[] embezzleResponses = { "im not blammo PogO", "no elbyStare", "no elbyGun", "get a job elbySMH" };
        /// <summary> Bot username </summary>
        private string username;
        /// <summary> Bot user ID </summary>
        private string botID;
        /// <summary> Bot access token </summary>
        private string botAccessToken;

        // TODO: Create channel config files to allow for individual channel commands / games
        /// <summary> Channels that the bot will listen to </summary>
        private List<string> channelNames;

        // TODO: Create channel config files to allow for individual channel commands / games
        /// <summary> Channels that the bot will listen to </summary>
        private List<string> channelIDs;

        /// <summary> Channel storage </summary>
        private Dictionary<string, Channel> channels = new();

        /// <summary> Database for the bot </summary>
        private UserDatabase database;

        /// <summary> Database for the scramble game </summary>
        private GameDatabase<string> scrambleDatabase;

        /// <summary> Database for the trivia game </summary>
        private GameDatabase<Trivia> triviaDatabase;

        /// <summary> Database for the riddle game </summary>
        private GameDatabase<Riddle> riddleDatabase;

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
        public Bot(string username, string botID, string botAccessToken, List<string> channelIDs, string databasePath, string scrambleDatabasePath, string triviaDatabasePath, string riddleDatabasePath)
        {
            this.username = username;
            this.botID = botID;
            this.botAccessToken = botAccessToken;
            this.channelIDs = channelIDs;
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            tokenSource = new CancellationTokenSource();

            // We setup api, task is discarded because we dont want to wait for result;
            SetupApi(factory);
            InitializeDatabases(databasePath, scrambleDatabasePath, triviaDatabasePath, riddleDatabasePath);
            // We set up the client to listen and write to the channel chat
            TwitchClient client = SetUpClient(factory);

            InitializeChannels(client);
            InitializeCommands();
            AutoSaveLoop(200000, tokenSource.Token);
        }

        /// <summary>
        /// Initializes channels
        /// </summary>
        /// <param name="client"></param>
        private async void InitializeChannels(TwitchClient client)
        {
            // Wait until client is connected
            while (!isConnected)
                await Task.Delay(1000);

            // we add client to each channel
            foreach (string channelID in channelIDs)
            {
                string channelName = database.FindUserByID(channelID).name.ToLower();
                channels[channelName] = new(client, channelName, channelID);
                AddGames(channels[channelName]);
            }

            OnlineStreamsCheck();
        }

        /// <summary>
        /// Initializes channels
        /// </summary>
        /// <param name="client"></param>
        private void InitializeChannel(TwitchClient client, string channelID)
        {
            string channelName = database.FindUserByID(channelID).name.ToLower();
            channels[channelName] = new(client, channelName, channelID);
            AddGames(channels[channelName]);
        }

        /// <summary>
        /// Gives all users poitns
        /// </summary>
        /// <param name="points"></param>
        public void GiveAll(int points)
        {
            database.GiveAll(points);
        }

        /// <summary>
        /// Sets up the client to allow us to chat
        /// </summary>
        private TwitchClient SetUpClient(ILoggerFactory factory)
        {
            ConnectionCredentials credentials = new ConnectionCredentials(username, botAccessToken);
            ClientOptions clientOptions = new();
            WebSocketClient customClient = new WebSocketClient(clientOptions);
#if DEBUG_MODE
            TwitchClient client = new TwitchClient(customClient, ClientProtocol.WebSocket, logger: factory.CreateLogger<TwitchClient>());
#else
            TwitchClient client = new TwitchClient(customClient, ClientProtocol.WebSocket);
#endif
            // we initialize the clien
            client.Initialize(credentials);
            client.Connect();
            client.OnMessageReceived += OnMessageReceived;
            client.OnJoinedChannel += OnJoinedChannel;
            client.OnConnected += OnConnected;
            return client;
        }

        /// <summary>
        /// Sets up the twitch api that allows us to check channel status
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        private void SetupApi(ILoggerFactory factory)
        {
#if DEBUG_MODE
            api = new(loggerFactory: factory);
#else
            api = new();
#endif
            api.Settings.ClientId = botID;
            api.Settings.AccessToken = botAccessToken;

            LiveStreamMonitorService monitorService = new(api);
            monitorService.SetChannelsById(channelIDs);
            monitorService.Start();
            // We add listeners for changes to stream state
            monitorService.OnStreamUpdate += UpdateStreamsOnlineStatus;
            monitorService.OnStreamOffline += ApiOnStreamOffline;
            monitorService.OnStreamOnline -= ApiOnStreamOnline;
        }

        /// <summary>
        /// Initializes all databases
        /// </summary>
        /// <param name="databasePath"></param>
        /// <param name="scrambleDatabasePath"></param>
        /// <param name="triviaDatabasePath"></param>
        /// <param name="riddleDatabasePath"></param>
        private void InitializeDatabases(string databasePath, string scrambleDatabasePath, string triviaDatabasePath, string riddleDatabasePath)
        {
            database = UserDatabase.LoadDatabase(api, databasePath);
            scrambleDatabase = GameDatabase<string>.LoadDatabase(scrambleDatabasePath);
            triviaDatabase = GameDatabase<Trivia>.LoadDatabase(triviaDatabasePath);
            riddleDatabase = GameDatabase<Riddle>.LoadDatabase(riddleDatabasePath);
        }

        /// <summary>
        /// Initializesa all commands
        /// </summary>
        private void InitializeCommands()
        {
            commands.Add(new RatDetectionCommand());
            commands.Add(new RouletteCommand());
            commands.Add(new GameCommand<CoinGame>("coingame"));
            commands.Add(new GameCommand<ScrambleGame>("scramble", new[]{ "scramba", "scrabble" }));
            commands.Add(new GameCommand<TriviaGame>("trivia"));
            commands.Add(new GameCommand<RiddleGame>("riddle"));
            commands.Add(new PointsCommand(database));
            commands.Add(new GivePointsCommand(database));
            commands.Add(new PoofCountCommand(database));
            commands.Add(new TopCommand(database));
            commands.Add(new TopLosersCommand(database));
            commands.Add(new RankCommand(database));
            commands.Add(new GlobalRankCommand(database));
            commands.Add(new TopPoofCommand(database));
            commands.Add(new TextCommand("embezzle", embezzleResponses));
        }

        /// <summary>
        /// Adds all games to each channel
        /// </summary>
        /// <param name="channel"></param>
        private void AddGames(Channel channel)
        {
            channel.AddGame(new CoinGame(channel.SendMessage, database));
            channel.AddGame(new ScrambleGame(channel.SendMessage, database, scrambleDatabase));
            channel.AddGame(new TriviaGame(channel.SendMessage, database, triviaDatabase));
            channel.AddGame(new RiddleGame(channel.SendMessage, database, riddleDatabase));
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void UpdateStreamsOnlineStatus(object? sender, OnStreamUpdateArgs args)
        {
            Console.WriteLine(" ---------------------- updating channel info: " + args.Stream.UserName + " -----------------------");
            channels[args.Stream.UserName.ToLower()].isOffline = false;
        }

        /// <summary>
        /// Function called when stream changes to offline status
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void ApiOnStreamOffline(object source, OnStreamOfflineArgs args)
        {
            Console.WriteLine("--------- " + args.Stream.UserName + "is now offline -------------------------");
            channels[args.Stream.UserName.ToLower()].isOffline = true;
        }

        /// <summary>
        /// Function called when stream changes to online status
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void ApiOnStreamOnline(object source, OnStreamOnlineArgs args)
        {
            Console.WriteLine("--------- " + args.Stream.UserName + " is now online --------------");
            channels[args.Channel.ToLower()].isOffline = false;
        }

        /// <summary>
        /// Checks whether the monitored stream is online or not
        /// </summary>
        /// <returns></returns>
        public async void OnlineStreamsCheck()
        {
            GetStreamsResponse streams = await api.Helix.Streams.GetStreamsAsync(userIds: channelIDs);
           
            foreach (string channel in channels.Keys)
                channels[channel].isOffline = true;
            
            // We add each online stream to the list of online streams.
            foreach (Stream onlineStream in streams.Streams)
                channels[onlineStream.UserName.ToLower()].isOffline = false;
        }

        /// <summary>
        /// Event called when a message is received on any of teh active channels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Channel channel = channels[e.ChatMessage.Channel.ToLower()];
            User user = database.FindOrAddUserByID(e.ChatMessage.UserId, e.ChatMessage.Username);
            if (!user.channelIds.Contains(channel.channelId))
                user.channelIds.Add(channel.channelId);

            string message = e.ChatMessage.Message;
            string[] args = StringUtils.SplitCommand(message);

            if (TryGetRating(args, out float rating))
            {
                channel.RatingHandler(rating, user);
            }
            // Anti embezzling measure 
            if (bannedUsers.Contains(user.name.ToLower()))
                return;

            // if message is a poof call we increment the pof counter
            if (message == poof)
                user.poofCount++;

            // Command handling
            if (message[0] == channel.commandCharacter)
            {
                if ((DateTime.Now - user.lastCommand).TotalSeconds < channel.commandCooldown)
                    return;


                args = StringUtils.SplitCommand(message[1..]);

                // if there is no arg we return
                if (args.Length == 0)
                    return;
                
                // check the input for banned words such as slurs
                if(StringUtils.ContainsBannedWord(args))
                {
                    channel.SendReply(bannedWordReply, e.ChatMessage);
                    return;
                }

                string commandKey = args[0].ToLower();
                Console.WriteLine(commandCalledMessage + DateTime.Now.ToString() + " " + user.name + ": " +  message + " --- Channel: " + channel.channelName );

                // Help command
                if (helpCommandAliases.Contains(commandKey))
                {
                    Help(channel, user, e.ChatMessage);
                    return;
                }

                // we find the command in our command list
                IBotCommand calledCommand = FindCommand(channel, commandKey);
                // If no command is found we return
                if (calledCommand is null || (!channel.isOffline && !calledCommand.IsOnlineCommand))
                {
                    Console.WriteLine(commandNotFound);
                    return;
                }

                // call command if args are correct, send help info message otherwise
                if (args.Length > calledCommand.MinArgs)
                {
                    user.lastCommand = DateTime.Now;
                    calledCommand.Execute(user, channel, e.ChatMessage);
                }
                else
                    channel.SendReply(calledCommand.HelpInfo(channel), e.ChatMessage);
            }
            // If input is not a command we check if there are ay active games going on
            else
            {
                channel.CheckActiveGames(e.ChatMessage);
            }
        }

        /// <summary>
        /// Tries to find the command in the command list, returns null otherwise
        /// </summary>
        /// <param name="commandKey"></param>
        /// <returns></returns>
        private IBotCommand FindCommand(Channel channel, string commandKey)
        {
            foreach (IBotCommand command in commands)
            {
                List<string> aliases = command.GetAliases(channel);
                if (command.CommandKey == commandKey || (aliases is not null  && aliases.Contains(commandKey)))
                    return command;
            }

            return null;
        }

        /// <summary>
        /// Shows the user the help menu
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="user"></param>
        /// <param name="chatMessage"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Help(Channel channel, User user, ChatMessage message)
        {
            string[] args = StringUtils.SplitCommand(message.Message);
            bool modHelp = args.Length > 1 ? args[1] == "mods" : false;

            // Mod exclusive command list
            if(modHelp)
            {
                string modCommandsMessage = modCommandsIntro;
                for (int i = 1; i < commands.Count; i++)
                    if (commands[i].IsModeratorCommand)
                        modCommandsMessage += " " + commands[i].CommandKey + ",";
                modCommandsMessage += " The bot is still in development, currently there is no mod commands, since bot customization has not been implemented yet, for any help or questions please whisper socialistWizard on twitch.";
                modCommandsMessage = modCommandsMessage.Remove(modCommandsMessage.Length - 1);
                channel.SendReply(modCommandsMessage, message);
                return;
            }

            // Returns the help menu for the command if any matches the input
            if (args.Length > 1 && !modHelp) {                    
                IBotCommand calledCommand = FindCommand(channel, args[1]);
                // command was not found
                if(calledCommand == null)
                {
                    channel.SendReply($"{args[1]} command not found, to check command list use {channel.commandCharacter}help", message);
                    return;
                }

                channel.SendReply(calledCommand.HelpInfo(channel) + $" [Online compatible: {calledCommand.IsOnlineCommand.ToString().ToLower()}]", message);
                return;
            }

            string helpMessage = helpIntro;
            // TODO: once channel preference system is implemented update this
            for (int i = 0; i < commands.Count; i++)
            {
                if (!commands[i].IsModeratorCommand && (channel.isOffline || commands[i].IsOnlineCommand))
                    helpMessage += " " + commands[i].CommandKey + ",";
            }
            helpMessage = helpMessage.Remove(helpMessage.Length - 1);

            helpMessage += $". For additional info on a command do {channel.commandCharacter}help <commandName> or {channel.commandCharacter}help mods for mod commands";
            channel.SendReply(helpMessage, message);
        }

        /// <summary>
        /// Shows all online channels in console, not in chat
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void CheckOnlineChannels()
        {
            foreach (KeyValuePair<string, Channel> channel in channels)
            {
                Console.WriteLine(channel.Key + " is " + (channel.Value.isOffline ? "offline" : "online"));
            }
        }

        /// <summary>
        /// returns rating
        /// </summary>
        /// <returns></returns>
        public bool TryGetRating(string[] input, out float rating)
        {
            rating = 0;
            foreach(string inputItem in input)
            {
                if (inputItem.EndsWith("/10") && float.TryParse(inputItem.Replace("/10", ""), out rating) && (rating >= 0) && (rating <= 10))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Reloads all databases to reflect manual changes
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void ReloadDatabases()
        {
            database.ReloadDatabase();
            scrambleDatabase.ReloadDatabase();
            triviaDatabase.ReloadDatabase();
        }

        /// <summary>
        /// Returns the user id associated with an user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal async Task<string> GetUserID(string userName)
        {
            return await database.GetUserId(userName);
        }
    }
}