using System.Diagnostics;

namespace TwitchBot.Scripts.Game
{
    public abstract class BaseGame
    {
        /// <summary> Maximum duration of the game, once  </summary>
        public int maxDuration = 60;

        /// <summary> Message called when game is not completed after maxDuration ends </summary>
        public string timeoutMessage;
        
        /// <summary>
        /// Whether or not the game is currently running
        /// </summary>
        public bool IsRunning { get; set; }


        /// <summary> Cancellation token source, serves as the internal identifier for the instance of the game </summary>
        protected CancellationTokenSource cancellationTokenSource;
        
        /// <summary>
        /// Constructor
        /// </summary>
        protected BaseGame() { }

        /// <summary>
        /// Checks the input to update the game state
        /// </summary>
        /// <param name="message"></param>
        public virtual void CheckInput(string message)
        {
            if (cancellationTokenSource.IsCancellationRequested)
                return;
        }

        /// <summary>
        /// Starts the game
        /// </summary>
        public virtual void StartGame()
        {
            cancellationTokenSource = new CancellationTokenSource();
            StartTimedCancel(cancellationTokenSource.Token);

        }

        /// <summary>
        /// Cancels the game
        /// </summary>
        public abstract void CancelGame();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="warning"> Message displayed by the bot after timer ends </param>
        /// <param name="timer"> Time before warning is displayed, in seconds </param>
        /// <param name="token"></param>
        public async void StartTimedWarning(string warning, int timer, CancellationToken token)
        {
            // We wait for max duration before cancelling game
            await Task.Delay(1000 * maxDuration);

            // If cancelled we return
            if (token.IsCancellationRequested)
                return;

            Debug.WriteLine
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        public async void StartTimedCancel(CancellationToken token)
        {
            // We wait for max duration before cancelling game
            await Task.Delay(1000 * maxDuration);
            
            ///
            if (token.IsCancellationRequested)
                return;

            CancelGame();
        }
    }
}