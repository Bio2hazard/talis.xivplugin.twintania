// talis.xivplugin.twintania
// Initializer.cs

using System;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using talis.xivplugin.twintania.Properties;
using System.Text.RegularExpressions;
using NLog;
using FFXIVAPP.Common.Utilities;
using talis.xivplugin.twintania.Helpers;

namespace talis.xivplugin.twintania
{
    internal static class Initializer
    {
        #region Declarations

        #endregion

        /// <summary>
        /// </summary>
        public static void LoadSettings()
        {
            if (Constants.XSettings != null)
            {
                foreach (var xElement in Constants.XSettings.Descendants()
                                                  .Elements("Setting"))
                {
                    var xKey = (string) xElement.Attribute("Key");
                    var xValue = (string) xElement.Element("Value");
                    if (String.IsNullOrWhiteSpace(xKey) || String.IsNullOrWhiteSpace(xValue))
                    {
                        return;
                    }
                    Settings.SetValue(xKey, xValue);
                    if (!Constants.Settings.Contains(xKey))
                    {
                        Constants.Settings.Add(xKey);
                    }
                }
            }
        }

        public static void LoadSounds()
        {
            PluginViewModel.Instance.SoundFiles.Clear();
            //do your gui stuff here

            string regexString;

            if(Settings.Default.TwintaniaHPWidgetUseNAudio)
                regexString = @"^.+\.(wav|mp3)$";
            else
                regexString = @"^.+\.(wav)$";

            var alertsDirectory = Path.Combine(Constants.BaseDirectory, "AlertSounds");
            if (Directory.Exists(alertsDirectory))
            {
                var files = Directory.GetFiles(Constants.BaseDirectory + "AlertSounds")
                                 .Where(file => Regex.IsMatch(file, regexString))
                                 .Select(file => new FileInfo(file));
                foreach (var file in files)
                {
                    PluginViewModel.Instance.SoundFiles.Add(file.Name);
                }
            }
        }

        public static void SetupWidgetTopMost()
        {
            WidgetTopMostHelper.HookWidgetTopMost();
        }
    }
}
