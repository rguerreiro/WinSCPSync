using System;
using System.Timers;
using System.Windows.Threading;

namespace WinSCPSyncGui.Infrastructure.Wpf
{
    public class EventDelayAggregator
    {
        private Timer _timer = null;
        private Dispatcher _dispatcher = null;

        public EventDelayAggregator(Action callback)
            : this(500, callback) // 1/2 second
        {
        }
        
        public EventDelayAggregator(int delay, Action callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            _dispatcher = Dispatcher.CurrentDispatcher;
            _timer = new Timer(delay);
            _timer.Elapsed += (sender, args) =>
            {
                Stop();
                _dispatcher.Invoke(callback, DispatcherPriority.Render);
            };
        }
        
        public void Start()
        {
            if (_timer == null)
                throw new InvalidOperationException("Timer wasn't correctly initiated");

            IsStarted = true;
            _timer.Start();
        }
        
        public void Stop()
        {
            if (_timer == null)
                throw new InvalidOperationException("Timer wasn't correctly initiated");

            IsStarted = false;
            _timer.Stop();
        }
        
        public void Signal()
        {
            if (_timer == null)
                throw new InvalidOperationException("Timer wasn't correctly initiated");

            if (!IsStarted)
            {
                Start();
            }
            else
            {
                _timer.Stop();
                _timer.Start();
            }
        }

        public bool IsStarted { get; private set; }
    }
}
