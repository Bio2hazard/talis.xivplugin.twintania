using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using talis.xivplugin.twintania.Events;
using talis.xivplugin.twintania.Properties;
using talis.xivplugin.twintania.Windows;

namespace talis.xivplugin.twintania.Helpers
{
    public class TimerHelper : INotifyPropertyChanged
    {
        private Stopwatch _stopWatch;

        private double _timeToEventTotal;
        private double _timeToEventCurrent;
        private int _timeToEventSeconds;

        private bool _enabled = false;

        private long _framecounter;

        private System.Timers.Timer _timer;
        private MicroTimerLib.MicroTimer _microTimer;

        public string timerMode;
        private event EventHandler<TimerUpdateEventArgs> _timerUpdate;
        public string soundWhenFinished;
        public int volume;
        public bool counting;

        public bool enabled
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
            timerMode = "Timer";
            soundWhenFinished = "";
            volume = 100;
            counting = true;
            _timerUpdate = update;
            _stopWatch = new Stopwatch();
        }

        public void Start(double timeToEvent, int interval)
        {
            try
            {
                _timeToEventTotal = timeToEvent;
                _timeToEventCurrent = timeToEvent;
                _timeToEventSeconds = (int)Math.Floor(timeToEvent) + 1;

                switch (timerMode)
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
                            _microTimer = new MicroTimerLib.MicroTimer((interval * 1000));
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
                enabled = true;
            }
            catch(Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
        }

        public void Stop()
        {
            enabled = false;

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
            if (!enabled)
                return;

            //LogManager.GetCurrentClassLogger().Trace(timerMode + " Tick (elapsed:" + ((float)stopWatch.ElapsedMilliseconds / 1000) + ")");

            _timeToEventCurrent -= ((float)_stopWatch.ElapsedMilliseconds / 1000);

            _framecounter += _stopWatch.ElapsedMilliseconds;
            if(_timerUpdate != null && _framecounter >= 25)
            {
                DispatcherHelper.Invoke(() => _timerUpdate(this, new TimerUpdateEventArgs(_timeToEventCurrent)), System.Windows.Threading.DispatcherPriority.DataBind);
                _framecounter = 0;
            }
            
            _stopWatch.Restart();

            if (volume > 0 && counting && (int)Math.Floor(_timeToEventCurrent + 0.7) < _timeToEventSeconds && _timeToEventSeconds > 1)
            {
                _timeToEventSeconds--;
                if (_timeToEventSeconds <= 10)
                {
                    SoundHelper.Play(@"Counting/" + _timeToEventSeconds + @".mp3", volume);
                }
            }
            else if (_timeToEventCurrent <= 0.00)
            {
                enabled = false;

                if(_timerUpdate != null)
                {
                    DispatcherHelper.Invoke(() => _timerUpdate(this, new TimerUpdateEventArgs(0.00)), System.Windows.Threading.DispatcherPriority.DataBind);
                }

                if(soundWhenFinished.Length > 0 && volume > 0)
                {
                    SoundHelper.Play(soundWhenFinished, volume);
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
