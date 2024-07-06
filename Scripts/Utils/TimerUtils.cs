using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Scripts.Utils
{
    public static class TimerUtils
    {
        /// <summary>
        /// Starts a timed action
        /// </summary>
        /// <param name="timeInSeconds"></param>
        /// <param name="token"></param>
        /// <param name="action"></param>
        public static async void StartTimedAction(int timeInSeconds, Action action, CancellationToken token)
        {
            // We wait for max duration before cancelling game
            await Task.Delay(1000 * timeInSeconds);

            // If cancelled we return
            if (token.IsCancellationRequested)
                return;

            action.Invoke();
        }
    }
}
