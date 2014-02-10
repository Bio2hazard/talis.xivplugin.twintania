// Talis.XIVPlugin.Twintania
// TimerHelper.cs
// 
// 	

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Threading;
using FFXIVAPP.Common.Helpers;
using NLog;
using Talis.XIVPlugin.Twintania.Events;
using Talis.XIVPlugin.Twintania.Properties;

namespace Talis.XIVPlugin.Twintania.Helpers
{
    public class TimerHelper : INotifyPropertyChanged
    {
        #region Logger

        private static Logger _logger;

        private static Logger Logger
        {
            get
            {
                if (FFXIVAPP.Common.Constants.EnableNLog || Settings.Default.TwintaniaWidgetAdvancedLogging)
                {
                    return _logger ?? (_logger = LogManager.GetCurrentClassLogger());
                }
                return null;
            }
        }

        #endregion

        public bool Counting;
        public string SoundWhenFinished;
        public string TimerMode;
        public int Volume;
        private bool _enabled;

        private long _framecounter;
        private MicroTimer _microTimer;
        private Stopwatch _stopWatch;

        private double _timeToEventCurrent;
        private int _timeToEventSeconds;

        private Timer _timer;

        public TimerHelper(EventHandler<TimerUpdateEventArgs> update = null)
        {
            TimerMode = "Timer";
            SoundWhenFinished = "";
            Volume = 100;
            Counting = true;
            TimerUpdate = update;
            _stopWatch = new Stopwatch();
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                RaisePropertyChanged();
            }
        }

        private event EventHandler<TimerUpdateEventArgs> TimerUpdate;

        public void Start(double timeToEvent, int interval)
        {
            try
            {
                _timeToEventCurrent = timeToEvent;
                _timeToEventSeconds = (int) Math.Floor(timeToEvent) + 1;

                switch (TimerMode)
                {
                    case "Timer":
                        if (_timer == null)
                        {
                            _timer = new Timer(interval);
                            _timer.Elapsed += delegate { TickHandler(); };
                        }
                        else
                        {
                            _timer.Interval = interval;
                        }
                        _timer.Start();
                        break;

                    case "MicroTimer":
                        if (_microTimer == null)
                        {
                            _microTimer = new MicroTimer((interval * 1000));
                            _microTimer.MicroTimerElapsed += delegate { TickHandler(); };
                        }
                        else
                        {
                            _microTimer.Interval = (interval * 1000);
                        }
                        _microTimer.Start();
                        break;
                }
                _stopWatch.Start();
                Enabled = true;
                LogHelper.Log(Logger, TimerMode + " Started ( Duration:" + timeToEvent +" Interval:" + interval + " Sound When Finished:" + SoundWhenFinished + " Volume:" + Volume + " Counting:" + Counting + " )", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }

        public void Stop()
        {
            Enabled = false;

            _stopWatch.Stop();

            if (_timer != null)
            {
                _timer.Stop();
            }

            if (_microTimer != null)
            {
                _microTimer.Stop();
            }
        }

        public void TickHandler()
        {
            if (!Enabled)
            {
                return;
            }

            //LogHelper.Log(Logger, TimerMode + " Tick (elapsed:" + ((float)_stopWatch.ElapsedMilliseconds / 1000) + ")", LogLevel.Trace);

            _timeToEventCurrent -= ((float) _stopWatch.ElapsedMilliseconds / 1000);

            _framecounter += _stopWatch.ElapsedMilliseconds;
            if (TimerUpdate != null && _framecounter >= 25)
            {
                DispatcherHelper.Invoke(() => TimerUpdate(this, new TimerUpdateEventArgs(_timeToEventCurrent)), DispatcherPriority.DataBind);
                _framecounter = 0;
            }

            _stopWatch.Restart();

            if (Volume > 0 && Counting && (int) Math.Floor(_timeToEventCurrent + 0.7) < _timeToEventSeconds && _timeToEventSeconds > 1)
            {
                _timeToEventSeconds--;
                if (_timeToEventSeconds <= 10)
                {
                    SoundHelper.PlayCached("Plugins\\Talis.XIVPlugin.Twintania\\Counting\\" + _timeToEventSeconds + @".mp3", Volume);
                }
            }
            else if (_timeToEventCurrent <= 0.00)
            {
                Enabled = false;

                if (TimerUpdate != null)
                {
                    DispatcherHelper.Invoke(() => TimerUpdate(this, new TimerUpdateEventArgs(0.00)), DispatcherPriority.DataBind);
                }

                if (SoundWhenFinished.Length > 0 && Volume > 0)
                {
                    SoundHelper.PlayCached(SoundWhenFinished, Volume);
                }

                Stop();
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}
