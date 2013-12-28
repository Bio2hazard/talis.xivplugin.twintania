// talis.xivplugin.twintania
// Settings.cs

using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Models;
using FFXIVAPP.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using FontFamily = System.Drawing.FontFamily;

namespace talis.xivplugin.twintania.Properties
{
    internal class Settings : ApplicationSettingsBase, INotifyPropertyChanged
    {
        private static Settings _default;

        public static Settings Default
        {
            get { return _default ?? (_default = ((Settings) (Synchronized(new Settings())))); }
        }

        public override void Save()
        {
            XmlHelper.DeleteXmlNode(Constants.XSettings, "Setting");
            if (Constants.Settings.Count == 0)
            {
            }
            DefaultSettings();
            foreach (var item in Constants.Settings)
            {
                try
                {
                    var xKey = item;
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
                catch (Exception ex)
                {
                    Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
                }
            }
            Constants.XSettings.Save(Constants.BaseDirectory + "Settings.xml");
        }

        private void DefaultSettings()
        {
            Constants.Settings.Clear();
            Constants.Settings.Add("ShowTwintaniaHPWidgetOnLoad");

            Constants.Settings.Add("TwintaniaHPWidgetUseNAudio");
            Constants.Settings.Add("TwintaniaHPWidgetUseSoundCaching");

            Constants.Settings.Add("TwintaniaHPWidgetTop");
            Constants.Settings.Add("TwintaniaHPWidgetLeft");

            Constants.Settings.Add("TwintaniaHPWidgetWidth");
            Constants.Settings.Add("TwintaniaHPWidgetHeight");

            Constants.Settings.Add("TwintaniaHPWidgetUIScale");

            Constants.Settings.Add("TwintaniaHPWidgetEnrageTime");
            Constants.Settings.Add("TwintaniaHPWidgetEnrageVolume");
            Constants.Settings.Add("TwintaniaHPWidgetEnrageCounting");
            Constants.Settings.Add("TwintaniaHPWidgetEnrageAlertFile");

            Constants.Settings.Add("TwintaniaHPWidgetDivebombTimeFast");
            Constants.Settings.Add("TwintaniaHPWidgetDivebombTimeSlow");

            Constants.Settings.Add("TwintaniaHPWidgetDivebombCounting");
            Constants.Settings.Add("TwintaniaHPWidgetDivebombVolume");
            Constants.Settings.Add("TwintaniaHPWidgetDivebombAlertFile");

            Constants.Settings.Add("TwintaniaHPWidgetTwisterVolume");

            Constants.Settings.Add("TwintaniaHPWidgetClickThroughEnabled");
            Constants.Settings.Add("TwintaniaHPWidgetOpacity");
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
                SetValue(key, value);
            }
        }

        public static void SetValue(string key, string value)
        {
            try
            {
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
                        Default[key] = Convert.ToDouble(value);
                        break;
                    case "Font":
                        var fc = new FontConverter();
                        var font = fc.ConvertFromString(value);
                        Default[key] = font ?? new Font(new FontFamily("Microsoft Sans Serif"), 12);
                        break;
                    case "Int32":
                        Default[key] = Convert.ToInt32(value);
                        break;
                    default:
                        Default[key] = value;
                        break;
                }
            }
            catch (SettingsPropertyNotFoundException ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
            catch (SettingsPropertyWrongTypeException ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
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
        public bool ShowTwintaniaHPWidgetOnLoad
        {
            get { return ((bool) (this["ShowTwintaniaHPWidgetOnLoad"])); }
            set
            {
                this["ShowTwintaniaHPWidgetOnLoad"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int TwintaniaHPWidgetTop
        {
            get { return ((int) (this["TwintaniaHPWidgetTop"])); }
            set
            {
                this["TwintaniaHPWidgetTop"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("500")]
        public int TwintaniaHPWidgetLeft
        {
            get { return ((int) (this["TwintaniaHPWidgetLeft"])); }
            set
            {
                this["TwintaniaHPWidgetLeft"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("250")]
        public int TwintaniaHPWidgetWidth
        {
            get { return ((int) (this["TwintaniaHPWidgetWidth"])); }
            set
            {
                this["TwintaniaHPWidgetWidth"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("450")]
        public int TwintaniaHPWidgetHeight
        {
            get { return ((int) (this["TwintaniaHPWidgetHeight"])); }
            set
            {
                this["TwintaniaHPWidgetHeight"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("1.0")]
        public string TwintaniaHPWidgetUIScale
        {
            get { return ((string) (this["TwintaniaHPWidgetUIScale"])); }
            set
            {
                this["TwintaniaHPWidgetUIScale"] = value;
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
        public StringCollection TwintaniaHPWidgetUIScaleList
        {
            get { return ((StringCollection) (this["TwintaniaHPWidgetUIScaleList"])); }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("780")]
        public Double TwintaniaHPWidgetEnrageTime
        {
            get { return ((Double) (this["TwintaniaHPWidgetEnrageTime"])); }
            set
            {
                this["TwintaniaHPWidgetEnrageTime"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int TwintaniaHPWidgetEnrageVolume
        {
            get { return ((int)(this["TwintaniaHPWidgetEnrageVolume"])); }
            set
            {
                this["TwintaniaHPWidgetEnrageVolume"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaHPWidgetEnrageCounting
        {
            get { return ((bool)(this["TwintaniaHPWidgetEnrageCounting"])); }
            set
            {
                this["TwintaniaHPWidgetEnrageCounting"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue(@"\AlertSounds\LowHealth.wav")]
        public string TwintaniaHPWidgetEnrageAlertFile
        {
            get { return ((string)(this["TwintaniaHPWidgetEnrageAlertFile"])); }
            set
            {
                this["TwintaniaHPWidgetEnrageAlertFile"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("6.8")]
        public Double TwintaniaHPWidgetDivebombTimeFast
        {
            get { return ((Double) (this["TwintaniaHPWidgetDivebombTimeFast"])); }
            set
            {
                this["TwintaniaHPWidgetDivebombTimeFast"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("47.7")]
        public Double TwintaniaHPWidgetDivebombTimeSlow
        {
            get { return ((Double) (this["TwintaniaHPWidgetDivebombTimeSlow"])); }
            set
            {
                this["TwintaniaHPWidgetDivebombTimeSlow"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaHPWidgetUseNAudio
        {
            get { return ((bool) (this["TwintaniaHPWidgetUseNAudio"])); }
            set
            {
                this["TwintaniaHPWidgetUseNAudio"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaHPWidgetUseSoundCaching
        {
            get { return ((bool) (this["TwintaniaHPWidgetUseSoundCaching"])); }
            set
            {
                this["TwintaniaHPWidgetUseSoundCaching"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaHPWidgetDivebombCounting
        {
            get { return ((bool) (this["TwintaniaHPWidgetDivebombCounting"])); }
            set
            {
                this["TwintaniaHPWidgetDivebombCounting"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int TwintaniaHPWidgetDivebombVolume
        {
            get { return ((int) (this["TwintaniaHPWidgetDivebombVolume"])); }
            set
            {
                this["TwintaniaHPWidgetDivebombVolume"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue(@"\AlertSounds\LowHealth.wav")]
        public string TwintaniaHPWidgetDivebombAlertFile
        {
            get { return ((string) (this["TwintaniaHPWidgetDivebombAlertFile"])); }
            set
            {
                this["TwintaniaHPWidgetDivebombAlertFile"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int TwintaniaHPWidgetTwisterVolume
        {
            get { return ((int) (this["TwintaniaHPWidgetTwisterVolume"])); }
            set
            {
                this["TwintaniaHPWidgetTwisterVolume"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0.7")]
        public string TwintaniaHPWidgetOpacity
        {
            get { return ((string) (this["TwintaniaHPWidgetOpacity"])); }
            set
            {
                this["TwintaniaHPWidgetOpacity"] = value;
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
        public StringCollection TwintaniaHPWidgetOpacityList
        {
            get { return ((StringCollection) (this["TwintaniaHPWidgetOpacityList"])); }
            set
            {
                this["TwintaniaHPWidgetOpacityList"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool TwintaniaHPWidgetClickThroughEnabled
        {
            get { return ((bool) (this["TwintaniaHPWidgetClickThroughEnabled"])); }
            set
            {
                this["TwintaniaHPWidgetClickThroughEnabled"] = value;
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
    }
}
