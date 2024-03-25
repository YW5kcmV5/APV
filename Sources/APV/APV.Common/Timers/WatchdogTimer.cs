using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace APV.Common.Timers
{
    public class WatchdogTimer : IDisposable
    {
        private readonly object _lock = new object();
        private Timer _timer;
        private ElapsedEventHandler _handler;
        private TimerCallback _callback;
        private Action _elapsed;
        private bool _disposed;

        private void Callback(object state, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (_handler != null)
                {
                    _handler(state, e);
                }
                else if (_callback != null)
                {
                    _callback(state);
                }
                else
                {
                    _elapsed?.Invoke();
                }
            }
        }

        public WatchdogTimer(int intervalInMlsec, TimerCallback callback)
        {
            if (intervalInMlsec <= 0)
                throw new ArgumentOutOfRangeException(nameof(intervalInMlsec));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _callback = callback;
            _timer = new Timer(intervalInMlsec);
            _timer.Elapsed += Callback;
            _timer.Enabled = true;
        }

        public WatchdogTimer(int intervalInMlsec, ElapsedEventHandler handler)
        {
            if (intervalInMlsec <= 0)
                throw new ArgumentOutOfRangeException(nameof(intervalInMlsec));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _handler = handler;
            _timer = new Timer(intervalInMlsec);
            _timer.Elapsed += Callback;
            _timer.Enabled = true;
        }

        public WatchdogTimer(int intervalInMlsec, Action elapsed)
        {
            if (intervalInMlsec <= 0)
                throw new ArgumentOutOfRangeException(nameof(intervalInMlsec));
            if (elapsed == null)
                throw new ArgumentNullException(nameof(elapsed));

            _elapsed = elapsed;
            _timer = new Timer(intervalInMlsec);
            _timer.Elapsed += Callback;
            _timer.Enabled = true;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _handler = null;
                _callback = null;
                _elapsed = null;
                if (_timer != null)
                {
                    _timer.Enabled = false;
                    _timer.Elapsed -= Callback;
                    _timer.Close();
                    _timer = null;
                }
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                _timer.Enabled = false;
                _timer.Enabled = true;
            }
        }

        public bool Enabled
        {
            get
            {
                lock (_lock)
                {
                    return _timer.Enabled;
                }
            }
            set
            {
                lock (_lock)
                {
                    _timer.Enabled = value;
                }
            }
        }
    }
}