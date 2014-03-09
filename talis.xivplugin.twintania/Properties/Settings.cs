// Talis.XIVPlugin.Twintania
// Settings.cs
// 
// 	

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Xml.Linq;
using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Models;
using NLog;
using Talis.XIVPlugin.Twintania.Helpers;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using FontFamily = System.Drawing.FontFamily;

namespace Talis.XIVPlugin.Twintania.Properties
{
    internal class Settings : ApplicationSettingsBase, INotifyPropertyChanged
    {
        #region Logger

        private static Logger _logger;

        private static Logger Logger
        {
            get
            {
                if (FFXIVAPP.Common.Constants.EnableNLog || Default.TwintaniaWidgetAdvancedLogging)
                {
                    return _logger ?? (_logger = LogManager.GetCurrentClassLogger());
                }
                return null;
            }
        }

        #endregion

        private static Settings _default;

        public static Settings Default
        {
            get { return _default ?? (_default = ((Settings) (Synchronized(new Settings())))); }
        }

        public override void Save()
        {
            // this call to default settings only ensures we keep the settings we want and delete the ones we don't (old)
            DefaultSettings();
            SaveSettingsNode();
            Constants.XSettings.Save(Path.Combine(FFXIVAPP.Common.Constants.PluginsSettingsPath, "Talis.XIVPlugin.Twintania.xml"));
        }

        private void DefaultSettings()
        {
            Constants.Settings.Clear();

            Constants.Settings.Add("ShowTwintaniaWidgetOnLoad");

            Constants.Settings.Add("TwintaniaWidgetShowTitle");
            Constants.Settings.Add("TwintaniaWidgetClickThroughEnabled");
            Constants.Settings.Add("TwintaniaWidgetOpacity");
            Constants.Settings.Add("TwintaniaWidgetAdvancedLogging");

            Constants.Settings.Add("TwintaniaWidgetTop");
            Constants.Settings.Add("TwintaniaWidgetLeft");
            Constants.Settings.Add("TwintaniaWidgetWidth");
            Constants.Settings.Add("TwintaniaWidgetHeight");
            Constants.Settings.Add("TwintaniaWidgetUIScale");

            Constants.Settings.Add("TwintaniaWidgetEnrageTime");
            Constants.Settings.Add("TwintaniaWidgetEnrageCounting");
            Constants.Settings.Add("TwintaniaWidgetEnrageVolume");
            Constants.Settings.Add("TwintaniaWidgetEnrageAlertFile");

            Constants.Settings.Add("TwintaniaWidgetDivebombTimeFast");
            Constants.Settings.Add("TwintaniaWidgetDivebombTimeSlow");
            Constants.Settings.Add("TwintaniaWidgetDivebombCounting");
            Constants.Settings.Add("TwintaniaWidgetDivebombVolume");
            Constants.Settings.Add("TwintaniaWidgetDivebombAlertFile");

            Constants.Settings.Add("TwintaniaWidgetDeathSentenceAlertPlaySound");
            Constants.Settings.Add("TwintaniaWidgetDeathSentenceAlertVolume");
            Constants.Settings.Add("TwintaniaWidgetDeathSentenceAlertFile");

            Constants.Settings.Add("TwintaniaWidgetDeathSentenceWarningEnabled");
            Constants.Settings.Add("TwintaniaWidgetDeathSentenceWarningShowTimer");
            Constants.Settings.Add("TwintaniaWidgetDeathSentenceWarningTime");
            Constants.Settings.Add("TwintaniaWidgetDeathSentenceWarningPlaySound");
            Constants.Settings.Add("TwintaniaWidgetDeathSentenceWarningCounting");
            Constants.Settings.Add("TwintaniaWidgetDeathSentenceWarningVolume");
            Constants.Settings.Add("TwintaniaWidgetDeathSentenceWarningFile");

            Constants.Settings.Add("TwintaniaWidgetTwisterAlertPlaySound");
            Constants.Settings.Add("TwintaniaWidgetTwisterAlertVolume");
            Constants.Settings.Add("TwintaniaWidgetTwisterAlertFile");

            Constants.Settings.Add("TwintaniaWidgetTwisterWarningEnabled");
            Constants.Settings.Add("TwintaniaWidgetTwisterWarningShowTimer");
            Constants.Settings.Add("TwintaniaWidgetTwisterWarningTime");
            Constants.Settings.Add("TwintaniaWidgetTwisterWarningPlaySound");
            Constants.Settings.Add("TwintaniaWidgetTwisterWarningCounting");
            Constants.Settings.Add("TwintaniaWidgetTwisterWarningVolume");
            Constants.Settings.Add("TwintaniaWidgetTwisterWarningFile");

            Constants.Settings.Add("TwintaniaWidgetPhaseAlertFile");
            Constants.Settings.Add("TwintaniaWidgetPhaseVolume");
            Constants.Settings.Add("TwintaniaWidgetPhaseEnabled");
        }

        public new void Reset()
        {
            DefaultSettings();
            foreach (var key in Constants.Settings)
            {
                var settingsProperty = Default.Properties[key];
                if (settingsProperty == null)
                {
                    continue;
                }
                var value = settingsProperty.DefaultValue.ToString();
                SetValue(key, value, CultureInfo.InvariantCulture);
            }
        }

        public void SetValue(string key, string value, IFormatProvider format = null)
        {
            try
            {
                if (format == null)
                {
                    format = CultureInfo.CurrentCulture;
                }

                var type = Default[key].GetType()
                                       .Name;
                switch (type)
                {
                    case "Boolean":
                        Default[key] = Convert.ToBoolean(value);
                        break;
                    case "Color":
                        var cc = new ColorConverter();
                        var color = cc.ConvertFrom(value);
                        Default[key] = color ?? Colors.Black;
                        break;
                    case "Double":
                        Default[key] = Double.Parse(value, NumberStyles.Any, format);
                        break;
                    case "Font":
                        var fc = new FontConverter();
                        var font = fc.ConvertFromString(value);
                        Default[key] = font ?? new Font(new FontFamily("Microsoft Sans Serif"), 12);
                        break;
                    case "Int32":
                        Default[key] = Int32.Parse(value, NumberStyles.Any, format);
                        break;
                    default:
                        Default[key] = value;
                        break;
                }
                RaisePropertyChanged(key);
            }
            catch (SettingsPropertyNotFoundException ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
            catch (SettingsPropertyWrongTypeException ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
            catch (FormatException ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }

        #region Property Bindings (Settings)

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("#FF000000")]
        public Color ChatBackgroundColor
        {
            get { return ((Color) (this["ChatBackgroundColor"])); }
            set
            {
                this["ChatBackgroundColor"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("#FF800080")]
        public Color TimeStampColor
        {
            get { return ((Color) (this["TimeStampColor"])); }
            set
            {
                this["TimeStampColor"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Microsoft Sans Serif, 12pt")]
        public Font ChatFont
        {
            get { return ((Font) (this["ChatFont"])); }
            set
            {
                this["ChatFont"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public Double Zoom
        {
            get { return ((Double) (this["Zoom"])); }
            set
            {
                this["Zoom"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool ShowTwintaniaWidgetOnLoad
        {
            get { return ((bool) (this["ShowTwintaniaWidgetOnLoad"])); }
            set
            {
                this["ShowTwintaniaWidgetOnLoad"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaWidgetShowTitle
        {
            get { return ((bool) (this["TwintaniaWidgetShowTitle"])); }
            set
            {
                this["TwintaniaWidgetShowTitle"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0.7")]
        public string TwintaniaWidgetOpacity
        {
            get { return ((string) (this["TwintaniaWidgetOpacity"])); }
            set
            {
                this["TwintaniaWidgetOpacity"] = value;
                RaisePropertyChanged();
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
<string>0.5</string>
<string>0.6</string>
<string>0.7</string>
<string>0.8</string>
<string>0.9</string>
<string>1.0</string>
</ArrayOfString>")]
        public StringCollection TwintaniaWidgetOpacityList
        {
            get { return ((StringCollection) (this["TwintaniaWidgetOpacityList"])); }
            set
            {
                this["TwintaniaWidgetOpacityList"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool TwintaniaWidgetAdvancedLogging
        {
            get { return ((bool) (this["TwintaniaWidgetAdvancedLogging"])); }
            set
            {
                this["TwintaniaWidgetAdvancedLogging"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool TwintaniaWidgetClickThroughEnabled
        {
            get { return ((bool) (this["TwintaniaWidgetClickThroughEnabled"])); }
            set
            {
                this["TwintaniaWidgetClickThroughEnabled"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int TwintaniaWidgetTop
        {
            get { return ((int) (this["TwintaniaWidgetTop"])); }
            set
            {
                this["TwintaniaWidgetTop"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("500")]
        public int TwintaniaWidgetLeft
        {
            get { return ((int) (this["TwintaniaWidgetLeft"])); }
            set
            {
                this["TwintaniaWidgetLeft"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("250")]
        public int TwintaniaWidgetWidth
        {
            get { return ((int) (this["TwintaniaWidgetWidth"])); }
            set
            {
                this["TwintaniaWidgetWidth"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("450")]
        public int TwintaniaWidgetHeight
        {
            get { return ((int) (this["TwintaniaWidgetHeight"])); }
            set
            {
                this["TwintaniaWidgetHeight"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("1.0")]
        public string TwintaniaWidgetUIScale
        {
            get { return ((string) (this["TwintaniaWidgetUIScale"])); }
            set
            {
                this["TwintaniaWidgetUIScale"] = value;
                RaisePropertyChanged();
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    <string>0.8</string>
    <string>0.9</string>
    <string>1.0</string>
    <string>1.1</string>
    <string>1.2</string>
    <string>1.3</string>
    <string>1.4</string>
    <string>1.5</string>
</ArrayOfString>")]
        public StringCollection TwintaniaWidgetUIScaleList
        {
            get { return ((StringCollection) (this["TwintaniaWidgetUIScaleList"])); }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("775")]
        public Double TwintaniaWidgetEnrageTime
        {
            get { return ((Double) (this["TwintaniaWidgetEnrageTime"])); }
            set
            {
                this["TwintaniaWidgetEnrageTime"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaWidgetEnrageCounting
        {
            get { return ((bool) (this["TwintaniaWidgetEnrageCounting"])); }
            set
            {
                this["TwintaniaWidgetEnrageCounting"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int TwintaniaWidgetEnrageVolume
        {
            get { return ((int) (this["TwintaniaWidgetEnrageVolume"])); }
            set
            {
                this["TwintaniaWidgetEnrageVolume"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Plugins\\Talis.XIVPlugin.Twintania\\AlertSounds\\LowHealth.wav")]
        public string TwintaniaWidgetEnrageAlertFile
        {
            get { return ((string) (this["TwintaniaWidgetEnrageAlertFile"])); }
            set
            {
                this["TwintaniaWidgetEnrageAlertFile"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("6.8")]
        public Double TwintaniaWidgetDivebombTimeFast
        {
            get { return ((Double) (this["TwintaniaWidgetDivebombTimeFast"])); }
            set
            {
                this["TwintaniaWidgetDivebombTimeFast"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("47.7")]
        public Double TwintaniaWidgetDivebombTimeSlow
        {
            get { return ((Double) (this["TwintaniaWidgetDivebombTimeSlow"])); }
            set
            {
                this["TwintaniaWidgetDivebombTimeSlow"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaWidgetDivebombCounting
        {
            get { return ((bool) (this["TwintaniaWidgetDivebombCounting"])); }
            set
            {
                this["TwintaniaWidgetDivebombCounting"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int TwintaniaWidgetDivebombVolume
        {
            get { return ((int) (this["TwintaniaWidgetDivebombVolume"])); }
            set
            {
                this["TwintaniaWidgetDivebombVolume"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Plugins\\Talis.XIVPlugin.Twintania\\AlertSounds\\LowHealth.wav")]
        public string TwintaniaWidgetDivebombAlertFile
        {
            get { return ((string) (this["TwintaniaWidgetDivebombAlertFile"])); }
            set
            {
                this["TwintaniaWidgetDivebombAlertFile"] = value;
                RaisePropertyChanged();
            }
        }


        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool TwintaniaWidgetDeathSentenceAlertPlaySound
        {
            get { return ((bool) (this["TwintaniaWidgetDeathSentenceAlertPlaySound"])); }
            set
            {
                this["TwintaniaWidgetDeathSentenceAlertPlaySound"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int TwintaniaWidgetDeathSentenceAlertVolume
        {
            get { return ((int) (this["TwintaniaWidgetDeathSentenceAlertVolume"])); }
            set
            {
                this["TwintaniaWidgetDeathSentenceAlertVolume"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Plugins\\Talis.XIVPlugin.Twintania\\AlertSounds\\LowHealth.wav")]
        public string TwintaniaWidgetDeathSentenceAlertFile
        {
            get { return ((string) (this["TwintaniaWidgetDeathSentenceAlertFile"])); }
            set
            {
                this["TwintaniaWidgetDeathSentenceAlertFile"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaWidgetDeathSentenceWarningEnabled
        {
            get { return ((bool) (this["TwintaniaWidgetDeathSentenceWarningEnabled"])); }
            set
            {
                this["TwintaniaWidgetDeathSentenceWarningEnabled"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaWidgetDeathSentenceWarningShowTimer
        {
            get { return ((bool) (this["TwintaniaWidgetDeathSentenceWarningShowTimer"])); }
            set
            {
                this["TwintaniaWidgetDeathSentenceWarningShowTimer"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("32")]
        public Double TwintaniaWidgetDeathSentenceWarningTime
        {
            get { return ((Double) (this["TwintaniaWidgetDeathSentenceWarningTime"])); }
            set
            {
                this["TwintaniaWidgetDeathSentenceWarningTime"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool TwintaniaWidgetDeathSentenceWarningPlaySound
        {
            get { return ((bool) (this["TwintaniaWidgetDeathSentenceWarningPlaySound"])); }
            set
            {
                this["TwintaniaWidgetDeathSentenceWarningPlaySound"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool TwintaniaWidgetDeathSentenceWarningCounting
        {
            get { return ((bool) (this["TwintaniaWidgetDeathSentenceWarningCounting"])); }
            set
            {
                this["TwintaniaWidgetDeathSentenceWarningCounting"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int TwintaniaWidgetDeathSentenceWarningVolume
        {
            get { return ((int) (this["TwintaniaWidgetDeathSentenceWarningVolume"])); }
            set
            {
                this["TwintaniaWidgetDeathSentenceWarningVolume"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Plugins\\Talis.XIVPlugin.Twintania\\AlertSounds\\Gasp.wav")]
        public string TwintaniaWidgetDeathSentenceWarningFile
        {
            get { return ((string) (this["TwintaniaWidgetDeathSentenceWarningFile"])); }
            set
            {
                this["TwintaniaWidgetDeathSentenceWarningFile"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaWidgetTwisterAlertPlaySound
        {
            get { return ((bool) (this["TwintaniaWidgetTwisterAlertPlaySound"])); }
            set
            {
                this["TwintaniaWidgetTwisterAlertPlaySound"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int TwintaniaWidgetTwisterAlertVolume
        {
            get { return ((int) (this["TwintaniaWidgetTwisterAlertVolume"])); }
            set
            {
                this["TwintaniaWidgetTwisterAlertVolume"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Plugins\\Talis.XIVPlugin.Twintania\\AlertSounds\\aruba.wav")]
        public string TwintaniaWidgetTwisterAlertFile
        {
            get { return ((string) (this["TwintaniaWidgetTwisterAlertFile"])); }
            set
            {
                this["TwintaniaWidgetTwisterAlertFile"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaWidgetTwisterWarningEnabled
        {
            get { return ((bool) (this["TwintaniaWidgetTwisterWarningEnabled"])); }
            set
            {
                this["TwintaniaWidgetTwisterWarningEnabled"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaWidgetTwisterWarningShowTimer
        {
            get { return ((bool) (this["TwintaniaWidgetTwisterWarningShowTimer"])); }
            set
            {
                this["TwintaniaWidgetTwisterWarningShowTimer"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("21")]
        public Double TwintaniaWidgetTwisterWarningTime
        {
            get { return ((Double) (this["TwintaniaWidgetTwisterWarningTime"])); }
            set
            {
                this["TwintaniaWidgetTwisterWarningTime"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool TwintaniaWidgetTwisterWarningPlaySound
        {
            get { return ((bool) (this["TwintaniaWidgetTwisterWarningPlaySound"])); }
            set
            {
                this["TwintaniaWidgetTwisterWarningPlaySound"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool TwintaniaWidgetTwisterWarningCounting
        {
            get { return ((bool) (this["TwintaniaWidgetTwisterWarningCounting"])); }
            set
            {
                this["TwintaniaWidgetTwisterWarningCounting"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int TwintaniaWidgetTwisterWarningVolume
        {
            get { return ((int) (this["TwintaniaWidgetTwisterWarningVolume"])); }
            set
            {
                this["TwintaniaWidgetTwisterWarningVolume"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Plugins\\Talis.XIVPlugin.Twintania\\AlertSounds\\sonar.wav")]
        public string TwintaniaWidgetTwisterWarningFile
        {
            get { return ((string) (this["TwintaniaWidgetTwisterWarningFile"])); }
            set
            {
                this["TwintaniaWidgetTwisterWarningFile"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaWidgetPhaseEnabled
        {
            get { return ((bool)(this["TwintaniaWidgetPhaseEnabled"])); }
            set
            {
                this["TwintaniaWidgetPhaseEnabled"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int TwintaniaWidgetPhaseVolume
        {
            get { return ((int)(this["TwintaniaWidgetPhaseVolume"])); }
            set
            {
                this["TwintaniaWidgetPhaseVolume"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Plugins\\Talis.XIVPlugin.Twintania\\AlertSounds\\phase.mp3")]
        public string TwintaniaWidgetPhaseAlertFile
        {
            get { return ((string)(this["TwintaniaWidgetPhaseAlertFile"])); }
            set
            {
                this["TwintaniaWidgetPhaseAlertFile"] = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public new event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion

        #region Iterative Settings Saving

        private void SaveSettingsNode()
        {
            if (Constants.XSettings == null)
            {
                return;
            }
            var xElements = Constants.XSettings.Descendants()
                                     .Elements("Setting");
            var enumerable = xElements as XElement[] ?? xElements.ToArray();
            foreach (var setting in Constants.Settings)
            {
                var element = enumerable.FirstOrDefault(e => e.Attribute("Key")
                                                              .Value == setting);
                if (element == null)
                {
                    var xKey = setting;
                    var xValue = Default[xKey].ToString();
                    var keyPairList = new List<XValuePair>
                    {
                        new XValuePair
                        {
                            Key = "Value",
                            Value = xValue
                        }
                    };
                    XmlHelper.SaveXmlNode(Constants.XSettings, "Settings", "Setting", xKey, keyPairList);
                }
                else
                {
                    var xElement = element.Element("Value");
                    if (xElement != null)
                    {
                        xElement.Value = Default[setting].ToString();
                    }
                }
            }
        }

        #endregion
    }
}
