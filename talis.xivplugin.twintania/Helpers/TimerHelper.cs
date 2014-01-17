using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Utilities;
using NLog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using talis.xivplugin.twintania.Events;

namespace talis.xivplugin.twintania.Helpers
{
    public class TimerHelper : INotifyPropertyChanged
    {
        #region Logger
        private static Logger _logger;
        private static Logger Logger
        {
            get
            {
                if (FFXIVAPP.Common.Constants.EnableNLog)
                {
                    return _logger ?? (_logger = LogManager.GetCurrentClassLogger());
                }
                return null;
            }
        }
        #endregion

        private Stopwatch _stopWatch;

        private double _timeToEventCurrent;
        private int _timeToEventSeconds;

        private bool _enabled;

        private long _framecounter;

        private System.Timers.Timer _timer;
        private MicroTimer _microTimer;

        public string TimerMode;
        private event EventHandler<TimerUpdateEventArgs> TimerUpdate;
        public string SoundWhenFinished;
        public int Volume;
        public bool Counting;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                RaisePropertyChanged();
            }
        }

        public TimerHelper(EventHandler<TimerUpdateEventArgs> update = null)
        {
            TimerMode = "Timer";
            SoundWhenFinished = "";
            Volume = 100;
            Counting = true;
            TimerUpdate = update;
            _stopWatch = new Stopwatch();
        }

        public void Start(double timeToEvent, int interval)
        {
            try
            {
                _timeToEventCurrent = timeToEvent;
                _timeToEventSeconds = (int)Math.Floor(timeToEvent) + 1;

                switch (TimerMode)
                {
                    case "Timer":
                        if(_timer == null)
                        {
                            _timer = new System.Timers.Timer(interval);
                            _timer.Elapsed += delegate { TickHandler(); };
                        }
                        else
                        {
                            _timer.Interval = interval;
                        }
                        _timer.Start();
                        break;

                    case "MicroTimer":
                        if(_microTimer == null)
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
            }
            catch(Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }

        public void Stop()
        {
            Enabled = false;

            _stopWatch.Stop();

            if(_timer != null)
            {
                _timer.Stop();
            }
                    
            if(_microTimer != null)
            {
                _microTimer.Stop();
            }
        }

        public void TickHandler()
        {
            if (!Enabled)
                return;

            //LogHelper.Log(Logger, TimerMode + " Tick (elapsed:" + ((float)_stopWatch.ElapsedMilliseconds / 1000) + ")", LogLevel.Trace);

            _timeToEventCurrent -= ((float)_stopWatch.ElapsedMilliseconds / 1000);

            _framecounter += _stopWatch.ElapsedMilliseconds;
            if(TimerUpdate != null && _framecounter >= 25)
            {
                DispatcherHelper.Invoke(() => TimerUpdate(this, new TimerUpdateEventArgs(_timeToEventCurrent)), System.Windows.Threading.DispatcherPriority.DataBind);
                _framecounter = 0;
            }
            
            _stopWatch.Restart();

            if (Volume > 0 && Counting && (int)Math.Floor(_timeToEventCurrent + 0.7) < _timeToEventSeconds && _timeToEventSeconds > 1)
            {
                _timeToEventSeconds--;
                if (_timeToEventSeconds <= 10)
                {
                    SoundPlayerHelper.PlayCached("Counting/" + _timeToEventSeconds + @".mp3", Volume);
                }
            }
            else if (_timeToEventCurrent <= 0.00)
            {
                Enabled = false;

                if(TimerUpdate != null)
                {
                    DispatcherHelper.Invoke(() => TimerUpdate(this, new TimerUpdateEventArgs(0.00)), System.Windows.Threading.DispatcherPriority.DataBind);
                }

                if(SoundWhenFinished.Length > 0 && Volume > 0)
                {
                    SoundPlayerHelper.PlayCached(SoundWhenFinished, Volume);
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
