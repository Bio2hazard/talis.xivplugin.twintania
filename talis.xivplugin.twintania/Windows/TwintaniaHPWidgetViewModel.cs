// talis.xivplugin.twintania
// TwintaniaHPWidgetViewModel.cs

using System.ComponentModel;
using System.Runtime.CompilerServices;
using FFXIVAPP.Common.Core.Memory;
using System.Timers;
using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Utilities;
using NLog;
using System;
using talis.xivplugin.twintania.Properties;
using System.Collections.Generic;
using talis.xivplugin.twintania.Helpers;
using MicroTimerLib;
using System.Threading;
using System.Windows.Threading;
using talis.xivplugin.twintania.Events;

namespace talis.xivplugin.twintania.Windows
{
    internal sealed class TwintaniaHPWidgetViewModel : INotifyPropertyChanged
    {
        #region Property Bindings

        private static TwintaniaHPWidgetViewModel _instance;

        private bool _testMode;
        private bool _forceTop;

        private ActorEntity _twintaniaEntity;
        private double _twintaniaHPPercent;
        private bool _twintaniaIsValid;
        private bool _twintaniaEngaged;

        private ActorEntity _dreadknightEntity;
        private double _dreadknightHPPercent;
        private bool _dreadknightIsValid;

        private TimerHelper _twintaniaDivebombTimer;
        private TimerHelper _twintaniaEnrageTimer;

        private int _twintaniaDivebombCount;
        private double _twintaniaDivebombTimeToNextCur;
        private double _twintaniaDivebombTimeToNextMax;
        private int _twintaniaDivebombTimeFull;

        private double _twintaniaEnrageTime;

        private ActorEntity _twintaniaTestActor;
        private ActorEntity _dreadknightTestActor;

        private System.Timers.Timer _twintaniaTestTimer;
        private double _twintaniaTestTimeToNextCur;
        private double _twintaniaTestTimeToNextMax;
        private Queue<Tuple<string, double>> _twintaniaTestList;

        public static TwintaniaHPWidgetViewModel Instance
        {
            get { return _instance ?? (_instance = new TwintaniaHPWidgetViewModel()); }
        }

        public bool TestMode
        {
            get { return _testMode; }
            set
            {
                _testMode = value;
                RaisePropertyChanged();
            }
        }

        public bool ForceTop
        {
            get { return _forceTop; }
            set
            {
                _forceTop = value;
                RaisePropertyChanged();
            }
        }

        public ActorEntity TwintaniaEntity
        {
            get { return _twintaniaEntity ?? (_twintaniaEntity = new ActorEntity()); }
            set
            {
                _twintaniaEntity = value;
                RaisePropertyChanged();
            }
        }

        public ActorEntity TwintaniaTestActor
        {
            get { return _twintaniaTestActor ?? (_twintaniaTestActor = new ActorEntity()); }
            set
            {
                _twintaniaTestActor = value;
                RaisePropertyChanged();
            }
        }

        public ActorEntity DreadknightTestActor
        {
            get { return _dreadknightTestActor ?? (_dreadknightTestActor = new ActorEntity()); }
            set
            {
                _dreadknightTestActor = value;
                RaisePropertyChanged();
            }
        }

        public bool TwintaniaIsValid
        {
            get { return _twintaniaIsValid; }
            set
            {
                _twintaniaIsValid = value;
                RaisePropertyChanged();
            }
        }

        public bool TwintaniaEngaged
        {
            get { return _twintaniaEngaged; }
            set
            {
                _twintaniaEngaged = value;
                RaisePropertyChanged();
            }
        }

        public double TwintaniaHPPercent
        {
            get { return _twintaniaHPPercent; }
            set
            {
                _twintaniaHPPercent = value;
                RaisePropertyChanged();
            }
        }

        public ActorEntity DreadknightEntity
        {
            get { return _dreadknightEntity ?? (_dreadknightEntity = new ActorEntity()); }
            set
            {
                _dreadknightEntity = value;
                RaisePropertyChanged();
            }
        }

        public bool DreadknightIsValid
        {
            get { return _dreadknightIsValid; }
            set
            {
                _dreadknightIsValid = value;
                RaisePropertyChanged();
            }
        }

        public double DreadknightHPPercent
        {
            get { return _dreadknightHPPercent; }
            set
            {
                _dreadknightHPPercent = value;
                RaisePropertyChanged();
            }
        }

        public TimerHelper TwintaniaDivebombTimer
        {
            get { return _twintaniaDivebombTimer ?? (_twintaniaDivebombTimer = new TimerHelper(delegate(object sender, TimerUpdateEventArgs e) { TwintaniaDivebombTimeToNextCur = e.TimeToEvent; })); }
            set
            {
                _twintaniaDivebombTimer = value;
                RaisePropertyChanged();
            }
        }

        public TimerHelper TwintaniaEnrageTimer
        {
            get { return _twintaniaEnrageTimer ?? (_twintaniaEnrageTimer = new TimerHelper(delegate(object sender, TimerUpdateEventArgs e) { TwintaniaEnrageTime = e.TimeToEvent; })); }
            set
            {
                _twintaniaEnrageTimer = value;
                RaisePropertyChanged();
            }
        }

        public int TwintaniaDivebombCount
        {
            get { return _twintaniaDivebombCount; }
            set
            {
                _twintaniaDivebombCount = value;
                RaisePropertyChanged();
            }
        }

        public double TwintaniaDivebombTimeToNextCur
        {
            get { return _twintaniaDivebombTimeToNextCur; }
            set
            {
                _twintaniaDivebombTimeToNextCur = value;
                RaisePropertyChanged();
            }
        }

        public double TwintaniaDivebombTimeToNextMax
        {
            get { return _twintaniaDivebombTimeToNextMax; }
            set
            {
                _twintaniaDivebombTimeToNextMax = value;
                RaisePropertyChanged();
            }
        }

        public int TwintaniaDivebombTimeFull
        {
            get { return _twintaniaDivebombTimeFull; }
            set
            {
                _twintaniaDivebombTimeFull = value;
                RaisePropertyChanged();
            }
        }

        public double TwintaniaEnrageTime
        {
            get { return _twintaniaEnrageTime; }
            set
            {
                _twintaniaEnrageTime = value;
                RaisePropertyChanged();
            }
        }

        public System.Timers.Timer TwintaniaTestTimer
        {
            get
            {
                if (_twintaniaTestTimer == null)
                {
                    _twintaniaTestTimer = new System.Timers.Timer(100);
                    _twintaniaTestTimer.Elapsed += delegate
                    {
                        TwintaniaTestTimeToNextCur -= 0.1;

                        TwintaniaEntity = TwintaniaTestActor;

                        if (TwintaniaTestTimeToNextCur <= 0.00)
                        {
                            Tuple<string, double> next = TwintaniaTestList.Dequeue();
                            if (next.Item2 == 0)
                            {
                                _twintaniaTestTimer.Stop();
                            }
                            else
                            {
                                switch (next.Item1)
                                {
                                    case "Divebomb":
                                        TriggerDiveBomb();
                                    break;

                                    case "Twister":
                                        SoundHelper.Play(@"\AlertSounds\aruba.wav", Settings.Default.TwintaniaHPWidgetTwisterVolume);
                                    break;

                                    case "End":
                                        TestModeStop();
                                    break;
                                }
                                TwintaniaTestTimeToNextCur = next.Item2;
                            }
                        }
                    };
                }
                return _twintaniaTestTimer;
            }
        }

        public double TwintaniaTestTimeToNextCur
        {
            get { return _twintaniaTestTimeToNextCur; }
            set
            {
                _twintaniaTestTimeToNextCur = value;
                RaisePropertyChanged();
            }
        }

        public double TwintaniaTestTimeToNextMax
        {
            get { return _twintaniaTestTimeToNextMax; }
            set
            {
                _twintaniaTestTimeToNextMax = value;
                RaisePropertyChanged();
            }
        }

        public Queue<Tuple<string, double>> TwintaniaTestList
        {
            get { return _twintaniaTestList ?? (_twintaniaTestList = new Queue<Tuple<string, double>>()); }
            set
            {
                _twintaniaTestList = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Declarations

        #endregion

        public TwintaniaHPWidgetViewModel()
        {
        }

        #region Loading Functions

        #endregion

        #region Utility Functions

        public void TriggerDiveBomb()
        {
            TwintaniaDivebombCount++;
            if (TwintaniaIsValid && TwintaniaDivebombCount <= 6)
            {
                if (TwintaniaDivebombCount == 4)
                {
                    TwintaniaDivebombTimeToNextCur = Settings.Default.TwintaniaHPWidgetDivebombTimeSlow;
                    TwintaniaDivebombTimeToNextMax = Settings.Default.TwintaniaHPWidgetDivebombTimeSlow;
                    TwintaniaDivebombTimeFull = (int)Math.Floor(Settings.Default.TwintaniaHPWidgetDivebombTimeSlow) + 1;
                }
                else
                {
                    TwintaniaDivebombTimeToNextCur = Settings.Default.TwintaniaHPWidgetDivebombTimeFast;
                    TwintaniaDivebombTimeToNextMax = Settings.Default.TwintaniaHPWidgetDivebombTimeFast;
                    TwintaniaDivebombTimeFull = (int)Math.Floor(Settings.Default.TwintaniaHPWidgetDivebombTimeFast) + 1;
                }
                DivebombTimerStart();
            }
        }

        public void TestModeStart()
        {
            if (TestMode)
                TestModeStop();

            Widgets.Instance.ShowTwintaniaHPWidget();
            ForceTop = true;

            TestMode = true;

            TwintaniaTestActor.Name = "Twintania";
            TwintaniaTestActor.HPMax = 514596;
            TwintaniaTestActor.HPCurrent = 514596;

            TwintaniaEntity = TwintaniaTestActor;

            TwintaniaIsValid = true;
            TwintaniaEngaged = true;

            EnrageTimerStart();

            TwintaniaHPPercent = 1;
            TwintaniaDivebombCount = 1;
            TwintaniaDivebombTimeToNextCur = 0;
            TwintaniaDivebombTimeToNextMax = 0;

            DreadknightTestActor.Name = "Dreadknight";
            DreadknightTestActor.HPMax = 11250;
            DreadknightTestActor.HPCurrent = 11250;

            DreadknightEntity = DreadknightTestActor;

            DreadknightIsValid = true;
            DreadknightHPPercent = 1;

            TwintaniaTestTimeToNextCur = 0.3;

            TwintaniaTestList.Enqueue(Tuple.Create("Divebomb", Settings.Default.TwintaniaHPWidgetDivebombTimeFast + 0.5));

            TwintaniaTestList.Enqueue(Tuple.Create("Divebomb", Settings.Default.TwintaniaHPWidgetDivebombTimeFast + 0.5));

            TwintaniaTestList.Enqueue(Tuple.Create("Divebomb", Settings.Default.TwintaniaHPWidgetDivebombTimeSlow + 0.5));

            TwintaniaTestList.Enqueue(Tuple.Create("Divebomb", Settings.Default.TwintaniaHPWidgetDivebombTimeFast + 0.5));

            TwintaniaTestList.Enqueue(Tuple.Create("Divebomb", Settings.Default.TwintaniaHPWidgetDivebombTimeFast + 0.5));

            TwintaniaTestList.Enqueue(Tuple.Create("Twister", 1.0));
            
            TwintaniaTestList.Enqueue(Tuple.Create("End", (double)0));

            TwintaniaTestTimer.Start();
        }

        public void TestModeStop()
        {
            if (!TestMode)
                return;

            ForceTop = false;

            DivebombTimerStop();
            EnrageTimerStop();

            TwintaniaTestList.Clear();

            TwintaniaEntity = null;

            TwintaniaIsValid = false;
            TwintaniaEngaged = false;

            TwintaniaHPPercent = 0;
            TwintaniaDivebombCount = 1;
            TwintaniaDivebombTimeToNextCur = 0;
            TwintaniaDivebombTimeToNextMax = 0;

            DreadknightIsValid = false;
            DreadknightHPPercent = 0;

            TestMode = false;
        }

        public void DivebombTimerStart()
        {
            TwintaniaDivebombTimer.soundWhenFinished = Settings.Default.TwintaniaHPWidgetDivebombAlertFile;
            TwintaniaDivebombTimer.volume = Settings.Default.TwintaniaHPWidgetDivebombVolume;
            TwintaniaDivebombTimer.counting = Settings.Default.TwintaniaHPWidgetDivebombCounting;

            TwintaniaDivebombTimer.Start(TwintaniaDivebombTimeToNextMax, 25);
            RaisePropertyChanged("TwintaniaDivebombTimer");
        }

        public void DivebombTimerStop()
        {
            TwintaniaDivebombTimer.Stop();
            RaisePropertyChanged("TwintaniaDivebombTimer");
        }

        public void EnrageTimerStart()
        {
            TwintaniaEnrageTimer.soundWhenFinished = Settings.Default.TwintaniaHPWidgetEnrageAlertFile;
            TwintaniaEnrageTimer.volume = Settings.Default.TwintaniaHPWidgetEnrageVolume;
            TwintaniaEnrageTimer.counting = Settings.Default.TwintaniaHPWidgetEnrageCounting;
            
            TwintaniaEnrageTimer.Start(Settings.Default.TwintaniaHPWidgetEnrageTime, 25);
            RaisePropertyChanged("TwintaniaEnrageTimer");
        }

        public void EnrageTimerStop()
        {
            TwintaniaEnrageTimer.Stop();
            RaisePropertyChanged("TwintaniaEnrageTimer");
        }

        #endregion

        #region Command Bindings

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}
