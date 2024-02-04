using System.Diagnostics;
using System.Threading.Channels;
using TwitchLib.Client.Models;

namespace TwitchBot.Scripts.Games
{
    public abstract class BaseGame
    {
        /// <summary> Maximum duration of the game, once  </summary>
        public int maxDuration = 60;

        /// <summary> Message called when game is not completed after maxDuration ends </summary>
        public string timeoutMessage;
        
        /// <summary> Cancellation token source, serves as the internal identifier for the instance of the game </summary>
        protected CancellationTokenSource cancellationTokenSource;
        
        /// <summary> action invoked whenever a message needs to be sent </summary>
        private Action<string> sendMessage;

        /// <summary>
        /// Whether or not the game is currently running
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseGame(Action<string> messageSender) {
            sendMessage = messageSender;
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
        private void EndGame()
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
        public async void StartTimedWarning(string warning, int timer, CancellationToken token)
        {
            // We wait for max duration before cancelling game
            await Task.Delay(1000 * timer, token);

            // If cancelled we return
            if (token.IsCancellationRequested)
                return;

            // Sends warning message after timer expires
            SendMessage(warning);
        }

        /// <summary>
        /// Sends a message using the action stored int constructor
        /// </summary>
        /// <param name="message"></param>
        protected void SendMessage(string message) 
        {
            sendMessage.Invoke(message);
        }

        /// <summary>
        /// Starts the cancel procedure
        /// </summary>
        /// <param name="token"></param>
        public async void StartTimedCancel(CancellationToken token)
        {
            // We wait for max duration before cancelling game
            await Task.Delay(1000 * maxDuration, token);
            
            // Return if game instance is cancelled
            if (token.IsCancellationRequested)
                return;

            CancelGame();
        }
    }
}