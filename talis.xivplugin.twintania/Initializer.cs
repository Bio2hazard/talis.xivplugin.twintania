// talis.xivplugin.twintania
// Initializer.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Linq;
using FFXIVAPP.Common.Helpers;
using talis.xivplugin.twintania.Helpers;
using talis.xivplugin.twintania.Properties;

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
                Settings.Default.Reset();
                foreach (var xElement in Constants.XSettings.Descendants()
                                                  .Elements("Setting"))
                {
                    var xKey = (string)xElement.Attribute("Key");
                    var xValue = (string)xElement.Element("Value");
                    if (String.IsNullOrWhiteSpace(xKey) || String.IsNullOrWhiteSpace(xValue))
                    {
                        return;
                    }
                    if (Constants.Settings.Contains(xKey))
                    {
                        Settings.SetValue(xKey, xValue);
                    }
                    else
                    {
                        Constants.Settings.Add(xKey);
                    }
                }
            }
        }

        public static void LoadAndCacheSounds()
        {
            PluginViewModel.Instance.SoundFiles.Clear();
            //do your gui stuff here
            var legacyFiles = new List<FileInfo>();
            var filters = new List<string>
            {
                "*.wav",
                "*.mp3"
            };
            foreach (var filter in filters)
            {
                var files = Directory.GetFiles(Constants.BaseDirectory, filter, SearchOption.AllDirectories)
                                     .Select(file => new FileInfo(file));
                legacyFiles.AddRange(files);
            }
            foreach (var legacyFile in legacyFiles)
            {
                if (legacyFile.DirectoryName == null)
                {
                    continue;
                }
                var baseKey = legacyFile.DirectoryName.Replace(Constants.BaseDirectory, "");
                var key = String.IsNullOrWhiteSpace(baseKey) ? legacyFile.Name : String.Format("{0}\\{1}", baseKey.Substring(1), legacyFile.Name);
                if (File.Exists(Path.Combine(FFXIVAPP.Common.Constants.SoundsPath, key)))
                {
                    continue;
                }
                try
                {
                    var directoryKey = String.IsNullOrWhiteSpace(baseKey) ? "" : baseKey.Substring(1);
                    var directory = Path.Combine(FFXIVAPP.Common.Constants.SoundsPath, directoryKey);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    File.Copy(legacyFile.FullName, Path.Combine(FFXIVAPP.Common.Constants.SoundsPath, key));
                    SoundPlayerHelper.TryGetSetSoundFile(key);
                }
                catch (Exception ex)
                {
                }
            }
            foreach (var cachedSoundFile in SoundPlayerHelper.SoundFileKeys())
            {
                PluginViewModel.Instance.SoundFiles.Add(cachedSoundFile);
            }
        }

        public static void SetupWidgetTopMost()
        {
            WidgetTopMostHelper.HookWidgetTopMost();
        }
    }
}
