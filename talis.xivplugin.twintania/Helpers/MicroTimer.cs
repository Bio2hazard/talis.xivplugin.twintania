// Talis.XIVPlugin.Twintania
// MicroTimer.cs
// 
// 	

using System;
using System.Diagnostics;
using System.Threading;

namespace Talis.XIVPlugin.Twintania.Helpers
{
    /// <summary>
    ///     MicroStopwatch class
    /// </summary>
    public class MicroStopwatch : Stopwatch
    {
        private readonly double _microSecPerTick = 1000000D / Frequency;

        public MicroStopwatch()
        {
            if (!IsHighResolution)
            {
                throw new Exception("On this system the high-resolution " + "performance counter is not available");
            }
        }

        public long ElapsedMicroseconds
        {
            get { return (long) (ElapsedTicks * _microSecPerTick); }
        }
    }

    /// <summary>
    ///     MicroTimer class
    /// </summary>
    public class MicroTimer
    {
        public delegate void MicroTimerElapsedEventHandler(object sender, MicroTimerEventArgs timerEventArgs);

        private long _ignoreEventIfLateBy = long.MaxValue;
        private bool _stopTimer = true;
        private Thread _threadTimer;
        private long _timerIntervalInMicroSec;

        public MicroTimer()
        {
        }

        public MicroTimer(long timerIntervalInMicroseconds)
        {
            Interval = timerIntervalInMicroseconds;
        }

        public long Interval
        {
            get { return Interlocked.Read(ref _timerIntervalInMicroSec); }
            set { Interlocked.Exchange(ref _timerIntervalInMicroSec, value); }
        }

        public long IgnoreEventIfLateBy
        {
            get { return Interlocked.Read(ref _ignoreEventIfLateBy); }
            set { Interlocked.Exchange(ref _ignoreEventIfLateBy, value <= 0 ? long.MaxValue : value); }
        }

        public bool Enabled
        {
            set
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
            get { return (_threadTimer != null && _threadTimer.IsAlive); }
        }

        public event MicroTimerElapsedEventHandler MicroTimerElapsed;

        public void Start()
        {
            if (Enabled || Interval <= 0)
            {
                return;
            }

            _stopTimer = false;

            ThreadStart threadStart = () => NotificationTimer(ref _timerIntervalInMicroSec, ref _ignoreEventIfLateBy, ref _stopTimer);

            _threadTimer = new Thread(threadStart)
            {
                Priority = ThreadPriority.Highest
            };
            _threadTimer.Start();
        }

        public void Stop()
        {
            _stopTimer = true;
        }

        public void StopAndWait()
        {
            StopAndWait(Timeout.Infinite);
        }

        public bool StopAndWait(int timeoutInMilliSec)
        {
            _stopTimer = true;

            if (!Enabled || _threadTimer.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                return true;
            }

            return _threadTimer.Join(timeoutInMilliSec);
        }

        public void Abort()
        {
            _stopTimer = true;

            if (Enabled)
            {
                _threadTimer.Abort();
            }
        }

        private void NotificationTimer(ref long timerIntervalInMicroSec, ref long ignoreEventIfLateBy, ref bool stopTimer)
        {
            var timerCount = 0;
            long nextNotification = 0;

            var microStopwatch = new MicroStopwatch();
            microStopwatch.Start();

            while (!stopTimer)
            {
                var callbackFunctionExecutionTime = microStopwatch.ElapsedMicroseconds - nextNotification;

                var timerIntervalInMicroSecCurrent = Interlocked.Read(ref timerIntervalInMicroSec);
                var ignoreEventIfLateByCurrent = Interlocked.Read(ref ignoreEventIfLateBy);

                nextNotification += timerIntervalInMicroSecCurrent;
                timerCount++;
                long elapsedMicroseconds;

                while ((elapsedMicroseconds = microStopwatch.ElapsedMicroseconds) < nextNotification)
                {
                    Thread.SpinWait(10);
                }

                var timerLateBy = elapsedMicroseconds - nextNotification;

                if (timerLateBy >= ignoreEventIfLateByCurrent)
                {
                    continue;
                }

                var microTimerEventArgs = new MicroTimerEventArgs(timerCount, elapsedMicroseconds, timerLateBy, callbackFunctionExecutionTime);
                MicroTimerElapsed(this, microTimerEventArgs);
            }

            microStopwatch.Stop();
        }
    }

    /// <summary>
    ///     MicroTimer Event Argument class
    /// </summary>
    public class MicroTimerEventArgs : EventArgs
    {
        // Simple counter, number times timed event (callback function) executed
        public MicroTimerEventArgs(int timerCount, long elapsedMicroseconds, long timerLateBy, long callbackFunctionExecutionTime)
        {
            TimerCount = timerCount;
            ElapsedMicroseconds = elapsedMicroseconds;
            TimerLateBy = timerLateBy;
            CallbackFunctionExecutionTime = callbackFunctionExecutionTime;
        }

        public int TimerCount { get; private set; }

        // Time when timed event was called since timer started
        public long ElapsedMicroseconds { get; private set; }

        // How late the timer was compared to when it should have been called
        public long TimerLateBy { get; private set; }

        // Time it took to execute previous call to callback function (OnTimedEvent)
        public long CallbackFunctionExecutionTime { get; private set; }
    }
}
