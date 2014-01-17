// talis.xivplugin.twintania
// Settings.cs

using System.IO;
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Xml.Linq;
using talis.xivplugin.twintania.Helpers;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using FontFamily = System.Drawing.FontFamily;

namespace talis.xivplugin.twintania.Properties
{
    internal class Settings : ApplicationSettingsBase, INotifyPropertyChanged
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
            Constants.XSettings.Save(Path.Combine(FFXIVAPP.Common.Constants.PluginsSettingsPath, "talis.xivplugin.twintania.xml"));
        }

        private void DefaultSettings()
        {
            Constants.Settings.Clear();

            Constants.Settings.Add("ShowTwintaniaWidgetOnLoad");

            Constants.Settings.Add("TwintaniaWidgetUseNAudio");
            Constants.Settings.Add("TwintaniaWidgetUseSoundCaching");

            Constants.Settings.Add("TwintaniaWidgetTop");
            Constants.Settings.Add("TwintaniaWidgetLeft");
            Constants.Settings.Add("TwintaniaWidgetWidth");
            Constants.Settings.Add("TwintaniaWidgetHeight");
            Constants.Settings.Add("TwintaniaWidgetUIScale");

            Constants.Settings.Add("TwintaniaWidgetEnrageTime");
            Constants.Settings.Add("TwintaniaWidgetEnrageVolume");
            Constants.Settings.Add("TwintaniaWidgetEnrageCounting");
            //Constants.Settings.Add("TwintaniaWidgetEnrageAlertFile");

            Constants.Settings.Add("TwintaniaWidgetDivebombTimeFast");
            Constants.Settings.Add("TwintaniaWidgetDivebombTimeSlow");
            Constants.Settings.Add("TwintaniaWidgetDivebombCounting");
            Constants.Settings.Add("TwintaniaWidgetDivebombVolume");
            //Constants.Settings.Add("TwintaniaWidgetDivebombAlertFile");

            Constants.Settings.Add("TwintaniaWidgetTwisterVolume");

            Constants.Settings.Add("TwintaniaWidgetClickThroughEnabled");
            Constants.Settings.Add("TwintaniaWidgetOpacity");
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
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
            catch (SettingsPropertyWrongTypeException ex)
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
        [DefaultSettingValue("780")]
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
        [DefaultSettingValue("100")]
        public int TwintaniaWidgetEnrageVolume
        {
            get { return ((int)(this["TwintaniaWidgetEnrageVolume"])); }
            set
            {
                this["TwintaniaWidgetEnrageVolume"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaWidgetEnrageCounting
        {
            get { return ((bool)(this["TwintaniaWidgetEnrageCounting"])); }
            set
            {
                this["TwintaniaWidgetEnrageCounting"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("AlertSounds/LowHealth.wav")]
        public string TwintaniaWidgetEnrageAlertFile
        {
            get { return ((string)(this["TwintaniaWidgetEnrageAlertFile"])); }
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
        public bool TwintaniaWidgetUseNAudio
        {
            get { return ((bool) (this["TwintaniaWidgetUseNAudio"])); }
            set
            {
                this["TwintaniaWidgetUseNAudio"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool TwintaniaWidgetUseSoundCaching
        {
            get { return ((bool) (this["TwintaniaWidgetUseSoundCaching"])); }
            set
            {
                this["TwintaniaWidgetUseSoundCaching"] = value;
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
        [DefaultSettingValue("AlertSounds/LowHealth.wav")]
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
        [DefaultSettingValue("100")]
        public int TwintaniaWidgetTwisterVolume
        {
            get { return ((int) (this["TwintaniaWidgetTwisterVolume"])); }
            set
            {
                this["TwintaniaWidgetTwisterVolume"] = value;
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
        public bool TwintaniaWidgetClickThroughEnabled
        {
            get { return ((bool) (this["TwintaniaWidgetClickThroughEnabled"])); }
            set
            {
                this["TwintaniaWidgetClickThroughEnabled"] = value;
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
