using System.Diagnostics;
using System.Threading.Channels;
using TwitchBot.Scripts.Users;
using TwitchBot.Scripts.Utils;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Games
{
    public abstract class BaseGame
    {
        /// <summary> Name of the game used for messages </summary>
        public string gameId;

        /// <summary> Maximum duration of the game, once  </summary>
        public int maxDuration = 60;
        
        /// <summary> Cancellation token source, serves as the internal identifier for the instance of the game </summary>
        protected CancellationTokenSource cancellationTokenSource;
        
        /// <summary> action invoked whenever a message needs to be sent </summary>
        private Action<string> sendMessage;

        /// <summary> User database reference </summary>
        protected UserDatabase database;

        /// <summary>
        /// Whether or not the game is currently running
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseGame(Action<string> messageSender, UserDatabase database) {
            sendMessage = messageSender;
            this.database = database;
        }

        /// <summary>
        /// Checks the input to update the game state
        /// </summary>
        /// <param name="message"></param>
        public void RunCheckInput(ChatMessage message)
        {
            if (!IsRunning || cancellationTokenSource.IsCancellationRequested)
                return;
            
            // Checks the input in a virtual function to allow override
            CheckInput(message);
        }

        /// <summary>
        /// Checks the input from the game standPoint
        /// </summary>
        /// <param name="message"></param>
        protected abstract void CheckInput(ChatMessage message);

        /// <summary>
        /// Starts the game
        /// </summary>
        public virtual void StartGame()
        {
            if (IsRunning)
                return;
            
            cancellationTokenSource = new CancellationTokenSource();
            StartTimedCancel(cancellationTokenSource.Token);
            IsRunning = true;
        }

        /// <summary>
        /// Cancels the game
        /// </summary>
        public virtual void CancelGame()
        {
            EndGame();
        }

        /// <summary>
        /// Finishes game
        /// </summary>
        protected void EndGame()
        {
            IsRunning = false;
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Sends a message after a specific amount of time, if game instance is cancelled it ignores it
        /// </summary>
        /// <param name="warning"> Message displayed by the bot after timer ends </param>
        /// <param name="timer"> Time before warning is displayed, in seconds </param>
        /// <param name="token"></param>
        public void StartTimedWarning(string warning, int timer, CancellationToken token)
        {
            TimerUtils.StartTimedAction(timer, () => SendMessage(warning), token);
        }

        /// <summary>
        /// Sends a message using the action stored int constructor
        /// </summary>
        /// <param name="message"></param>
        protected void SendMessage(string message) 
        {
            sendMessage.Invoke($"[{gameId}] " + message);
        }

        /// <summary>
        /// Starts the cancel procedure
        /// </summary>
        /// <param name="token"></param>
        public void StartTimedCancel(CancellationToken token)
        {
            TimerUtils.StartTimedAction(maxDuration, CancelGame, token);
        }
    }
}