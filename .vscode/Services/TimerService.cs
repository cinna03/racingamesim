using System;
using System.Windows.Threading;

namespace TimeBasedRacingGame.Services
{
    /// <summary>
    /// Service for managing game timer functionality
    /// </summary>
    public class TimerService
    {
        private DispatcherTimer _timer;
        private Action _onTick;
        private TimeSpan _interval;

        /// <summary>
        /// Gets whether the timer is currently running
        /// </summary>
        public bool IsRunning => _timer?.IsEnabled ?? false;

        /// <summary>
        /// Initializes a new instance of the TimerService class
        /// </summary>
        /// <param name="interval">Timer interval in seconds</param>
        public TimerService(double interval = 1.0)
        {
            _interval = TimeSpan.FromSeconds(interval);
            _timer = new DispatcherTimer();
            _timer.Interval = _interval;
            _timer.Tick += (sender, e) => _onTick?.Invoke();
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void Start()
        {
            if (!_timer.IsEnabled)
            {
                _timer.Start();
            }
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// Sets the callback action to execute on each tick
        /// </summary>
        /// <param name="onTick">Action to execute on timer tick</param>
        public void SetTickCallback(Action onTick)
        {
            _onTick = onTick;
        }
    }
}

