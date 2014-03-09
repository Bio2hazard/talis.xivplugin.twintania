// Talis.XIVPlugin.Twintania
// TwintaniaWidgetViewModel.cs
// 
// 	

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using FFXIVAPP.Common.Core.Memory;
using NLog;
using Talis.XIVPlugin.Twintania.Events;
using Talis.XIVPlugin.Twintania.Helpers;
using Talis.XIVPlugin.Twintania.Properties;

namespace Talis.XIVPlugin.Twintania.Windows
{
    internal sealed class TwintaniaWidgetViewModel : INotifyPropertyChanged
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

        #region Property Bindings

        private static TwintaniaWidgetViewModel _instance;

        private static string _widgetTitle = "[ T5 Tracker ]";

        private ActorEntity _dreadknightEntity;
        private double _dreadknightHPPercent;
        private bool _dreadknightIsValid;
        private bool _forceTop;
        private bool _testMode;
        private double _twintaniaDeathSentenceTime;
        private TimerHelper _twintaniaDeathSentenceTimer;

        private int _twintaniaDivebombCount = 1;
        private double _twintaniaDivebombTimeToNextCur;
        private double _twintaniaDivebombTimeToNextMax;
        private TimerHelper _twintaniaDivebombTimer;
        private bool _twintaniaEngaged;

        private double _twintaniaEnrageTime;
        private TimerHelper _twintaniaEnrageTimer;

        private ActorEntity _twintaniaEntity;
        private double _twintaniaHPPercent;
        private bool _twintaniaIsValid;

        private Queue<Tuple<string, double>> _twintaniaTestList;
        private double _twintaniaTestTimeToNextCur;
        private double _twintaniaTestTimeToNextMax;
        private Timer _twintaniaTestTimer;
        private double _twintaniaTwisterTime;
        private TimerHelper _twintaniaTwisterTimer;

        public static TwintaniaWidgetViewModel Instance
        {
            get { return _instance ?? (_instance = new TwintaniaWidgetViewModel()); }
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

        public string WidgetTitle
        {
            get { return _widgetTitle; }
            set
            {
                _widgetTitle = "[ T5 Tracker | " + value + " ]";
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

        public double TwintaniaEnrageTime
        {
            get { return _twintaniaEnrageTime; }
            set
            {
                _twintaniaEnrageTime = value;
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

        public double TwintaniaTwisterTime
        {
            get { return _twintaniaTwisterTime; }
            set
            {
                _twintaniaTwisterTime = value;
                RaisePropertyChanged();
            }
        }

        public TimerHelper TwintaniaTwisterTimer
        {
            get { return _twintaniaTwisterTimer ?? (_twintaniaTwisterTimer = new TimerHelper(delegate(object sender, TimerUpdateEventArgs e) { TwintaniaTwisterTime = e.TimeToEvent; })); }
            set
            {
                _twintaniaTwisterTimer = value;
                RaisePropertyChanged();
            }
        }

        public double TwintaniaDeathSentenceTime
        {
            get { return _twintaniaDeathSentenceTime; }
            set
            {
                _twintaniaDeathSentenceTime = value;
                RaisePropertyChanged();
            }
        }

        public TimerHelper TwintaniaDeathSentenceTimer
        {
            get { return _twintaniaDeathSentenceTimer ?? (_twintaniaDeathSentenceTimer = new TimerHelper(delegate(object sender, TimerUpdateEventArgs e) { TwintaniaDeathSentenceTime = e.TimeToEvent; })); }
            set
            {
                _twintaniaDeathSentenceTimer = value;
                RaisePropertyChanged();
            }
        }

        public Timer TwintaniaTestTimer
        {
            get
            {
                if (_twintaniaTestTimer == null)
                {
                    _twintaniaTestTimer = new Timer(100);
                    _twintaniaTestTimer.Elapsed += delegate
                    {
                        TwintaniaTestTimeToNextCur -= 0.1;

                        if (TwintaniaTestTimeToNextCur <= 0.00)
                        {
                            var next = TwintaniaTestList.Dequeue();
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            if (next.Item2 == 0)
                            {
                                _twintaniaTestTimer.Stop();
                                TestModeStop();
                            }
                            else
                            {
                                switch (next.Item1)
                                {
                                    case "Divebomb":
                                        TriggerDiveBomb();
                                        break;

                                    case "Twister":
                                        TriggerTwister();
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

        #region Loading Functions

        #endregion

        #region Utility Functions

        public void CheckCurrentPhase()
        {
            LogHelper.Log(Logger, TwintaniaHPPercent.ToString(), LogLevel.Debug);
            //Since there is no real way of telling if twin has change phase ill check health
            if (TwintaniaHPPercent <= 1.0 && TwintaniaHPPercent >= 0.85)
            {
                WidgetTitle = "P1";
            }
            else if (TwintaniaHPPercent <= 0.84 && TwintaniaHPPercent >= 0.55)
            {
                WidgetTitle = "P2";
                if (Settings.Default.TwintaniaWidgetPhaseEnabled)
                {
                    SoundHelper.PlayCached(Settings.Default.TwintaniaWidgetPhaseAlertFile, Settings.Default.TwintaniaWidgetPhaseVolume);
                }
            }
            // Need to test her y position to check if in air or not.
            // Ill fix this next time i do twin
            // TwintaniaEntity.Y > 25???
            else if (TwintaniaHPPercent <= 0.54 && !TwintaniaIsValid)
            {
                WidgetTitle = "P3";
                if (Settings.Default.TwintaniaWidgetPhaseEnabled)
                {
                    SoundHelper.PlayCached(Settings.Default.TwintaniaWidgetPhaseAlertFile, Settings.Default.TwintaniaWidgetPhaseVolume);
                }
            }
            else if (TwintaniaHPPercent <= 0.54 && TwintaniaHPPercent >= 0.30)
            {
                WidgetTitle = "P4";
                if (Settings.Default.TwintaniaWidgetPhaseEnabled)
                {
                    SoundHelper.PlayCached(Settings.Default.TwintaniaWidgetPhaseAlertFile, Settings.Default.TwintaniaWidgetPhaseVolume);
                }
            }
            else if (TwintaniaHPPercent <= 0.29)
            {
                WidgetTitle = "P5";
                if (Settings.Default.TwintaniaWidgetPhaseEnabled)
                {
                    SoundHelper.PlayCached(Settings.Default.TwintaniaWidgetPhaseAlertFile, Settings.Default.TwintaniaWidgetPhaseVolume);
                }
            }
        }

        public void TriggerDiveBomb()
        {
            LogHelper.Log(Logger, "Divebomb Triggered (" + TwintaniaDivebombCount + ")", LogLevel.Debug);
            TwintaniaDivebombCount++;
            if (TwintaniaDivebombCount <= 6)
            {
                if (TwintaniaDivebombCount == 4)
                {
                    TwintaniaDivebombTimeToNextCur = Settings.Default.TwintaniaWidgetDivebombTimeSlow;
                    TwintaniaDivebombTimeToNextMax = Settings.Default.TwintaniaWidgetDivebombTimeSlow;
                }
                else
                {
                    TwintaniaDivebombTimeToNextCur = Settings.Default.TwintaniaWidgetDivebombTimeFast;
                    TwintaniaDivebombTimeToNextMax = Settings.Default.TwintaniaWidgetDivebombTimeFast;
                }
                DivebombTimerStart();
            }
        }

        public void TriggerTwister()
        {
            LogHelper.Log(Logger, "Twister Triggered", LogLevel.Debug);
            if (Settings.Default.TwintaniaWidgetTwisterAlertPlaySound)
            {
                SoundHelper.PlayCached(Settings.Default.TwintaniaWidgetTwisterAlertFile, Settings.Default.TwintaniaWidgetTwisterAlertVolume);
            }

            if (Settings.Default.TwintaniaWidgetTwisterWarningEnabled)
            {
                TwisterTimerStart();
            }
        }

        public void TriggerDeathSentence()
        {
            LogHelper.Log(Logger, "Death Sentence Triggered", LogLevel.Debug);
            if (Settings.Default.TwintaniaWidgetDeathSentenceAlertPlaySound)
            {
                SoundHelper.PlayCached(Settings.Default.TwintaniaWidgetDeathSentenceAlertFile, Settings.Default.TwintaniaWidgetDeathSentenceAlertVolume);
            }

            if (Settings.Default.TwintaniaWidgetDeathSentenceWarningEnabled)
            {
                DeathSentenceTimerStart();
            }
        }

        public void TestModeStart()
        {
            if (TestMode)
            {
                TestModeStop();
            }
            LogHelper.Log(Logger, "Test Mode Started", LogLevel.Trace);

            Widgets.Instance.ShowTwintaniaWidget();
            ForceTop = true;

            TestMode = true;

            TwintaniaEntity = new ActorEntity
            {
                Name = "Twintania",
                HPMax = 514596,
                HPCurrent = 514596
            };
            TwintaniaEngaged = true;
            TwintaniaIsValid = true;
            TwintaniaHPPercent = 1;

            CheckCurrentPhase();

            EnrageTimerStart();

            TwintaniaDivebombCount = 1;
            TwintaniaDivebombTimeToNextCur = 0;
            TwintaniaDivebombTimeToNextMax = 0;

            DreadknightEntity = new ActorEntity
            {
                Name = "Dreadknight",
                HPMax = 11250,
                HPCurrent = 11250
            };
            DreadknightIsValid = true;
            DreadknightHPPercent = 1;

            TwintaniaTestTimeToNextCur = 0.3;

            TwintaniaTestList.Enqueue(Tuple.Create("Divebomb", Settings.Default.TwintaniaWidgetDivebombTimeFast + 0.5));

            TwintaniaTestList.Enqueue(Tuple.Create("Divebomb", Settings.Default.TwintaniaWidgetDivebombTimeFast + 0.5));

            TwintaniaTestList.Enqueue(Tuple.Create("Divebomb", Settings.Default.TwintaniaWidgetDivebombTimeSlow + 0.5));

            TwintaniaTestList.Enqueue(Tuple.Create("Divebomb", Settings.Default.TwintaniaWidgetDivebombTimeFast + 0.5));

            TwintaniaTestList.Enqueue(Tuple.Create("Divebomb", Settings.Default.TwintaniaWidgetDivebombTimeFast + 0.5));

            TwintaniaTestList.Enqueue(Tuple.Create("Twister", 1.0));

            TwintaniaTestList.Enqueue(Tuple.Create("End", (double)0));

            TwintaniaTestTimer.Start();
        }

        public void TestModeStop()
        {
            if (!TestMode)
            {
                return;
            }

            TwintaniaTestTimer.Stop();

            LogHelper.Log(Logger, "Test Mode Stopped", LogLevel.Trace);
            ForceTop = false;

            DivebombTimerStop();
            EnrageTimerStop();

            TwintaniaTestList.Clear();

            TwintaniaEntity = null;
            TwintaniaEngaged = false;
            TwintaniaIsValid = false;
            TwintaniaHPPercent = 0;

            TwintaniaDivebombCount = 1;
            TwintaniaDivebombTimeToNextCur = 0;
            TwintaniaDivebombTimeToNextMax = 0;

            DreadknightEntity = null;
            DreadknightIsValid = false;
            DreadknightHPPercent = 0;

            TestMode = false;
        }

        public void DivebombTimerStart()
        {
            TwintaniaDivebombTimer.SoundWhenFinished = Settings.Default.TwintaniaWidgetDivebombAlertFile;
            TwintaniaDivebombTimer.Volume = Settings.Default.TwintaniaWidgetDivebombVolume;
            TwintaniaDivebombTimer.Counting = Settings.Default.TwintaniaWidgetDivebombCounting;

            LogHelper.Log(Logger, "Starting Timer for Divebomb " + TwintaniaDivebombCount, LogLevel.Debug);

            TwintaniaDivebombTimer.Start(TwintaniaDivebombTimeToNextMax, 25);
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged("TwintaniaDivebombTimer");
        }

        public void DivebombTimerStop()
        {
            LogHelper.Log(Logger, "Stopping Timer for Divebomb " + TwintaniaDivebombCount, LogLevel.Debug);

            TwintaniaDivebombTimer.Stop();
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged("TwintaniaDivebombTimer");
        }

        public void EnrageTimerStart()
        {
            TwintaniaEnrageTimer.SoundWhenFinished = Settings.Default.TwintaniaWidgetEnrageAlertFile;
            TwintaniaEnrageTimer.Volume = Settings.Default.TwintaniaWidgetEnrageVolume;
            TwintaniaEnrageTimer.Counting = Settings.Default.TwintaniaWidgetEnrageCounting;

            LogHelper.Log(Logger, "Starting Enrage Timer", LogLevel.Debug);

            TwintaniaEnrageTimer.Start(Settings.Default.TwintaniaWidgetEnrageTime, 25);
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged("TwintaniaEnrageTimer");
        }

        public void EnrageTimerStop()
        {
            LogHelper.Log(Logger, "Stopping Enrage Timer", LogLevel.Debug);

            TwintaniaEnrageTimer.Stop();
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged("TwintaniaEnrageTimer");
        }

        public void TwisterTimerStart()
        {
            TwintaniaTwisterTimer.SoundWhenFinished = Settings.Default.TwintaniaWidgetTwisterWarningPlaySound ? Settings.Default.TwintaniaWidgetTwisterWarningFile : "";

            TwintaniaTwisterTimer.Volume = Settings.Default.TwintaniaWidgetTwisterWarningVolume;
            TwintaniaTwisterTimer.Counting = Settings.Default.TwintaniaWidgetTwisterWarningCounting;

            LogHelper.Log(Logger, "Starting Twister Timer", LogLevel.Debug);

            TwintaniaTwisterTimer.Start(Settings.Default.TwintaniaWidgetTwisterWarningTime, 25);
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged("TwintaniaTwisterTimer");
        }

        public void TwisterTimerStop()
        {
            LogHelper.Log(Logger, "Stopping Twister Timer", LogLevel.Debug);

            TwintaniaTwisterTimer.Stop();
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged("TwintaniaTwisterTimer");
        }

        public void DeathSentenceTimerStart()
        {
            TwintaniaDeathSentenceTimer.SoundWhenFinished = Settings.Default.TwintaniaWidgetDeathSentenceWarningPlaySound ? Settings.Default.TwintaniaWidgetDeathSentenceWarningFile : "";

            TwintaniaDeathSentenceTimer.Volume = Settings.Default.TwintaniaWidgetDeathSentenceWarningVolume;
            TwintaniaDeathSentenceTimer.Counting = Settings.Default.TwintaniaWidgetDeathSentenceWarningCounting;

            LogHelper.Log(Logger, "Starting Death Sentence Timer", LogLevel.Debug);

            TwintaniaDeathSentenceTimer.Start(Settings.Default.TwintaniaWidgetDeathSentenceWarningTime, 25);
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged("TwintaniaDeathSentenceTimer");
        }

        public void DeathSentenceTimerStop()
        {
            LogHelper.Log(Logger, "Stopping Death Sentence Timer", LogLevel.Debug);

            TwintaniaDeathSentenceTimer.Stop();
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged("TwintaniaDeathSentenceTimer");
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
