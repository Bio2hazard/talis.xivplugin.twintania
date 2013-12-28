﻿// talis.xivplugin.twintania
// Constants.cs

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using FFXIVAPP.Common.Helpers;

namespace talis.xivplugin.twintania
{
    public static class Constants
    {
        #region Declarations

        public const string BaseDirectory = "./Plugins/talis.xivplugin.twintania/";

        public const string LibraryPack = "pack://application:,,,/talis.xivplugin.twintania;component/";

        public static readonly string[] Supported = new[]
        {
            "en"
        };

        #endregion

        #region Property Bindings

        private static XDocument _xSettings;
        private static List<string> _settings;

        public static XDocument XSettings
        {
            get
            {
                const string file = "./Plugins/talis.xivplugin.twintania/Settings.xml";
                if (_xSettings == null)
                {
                    var found = File.Exists(file);
                    _xSettings = found ? XDocument.Load(file) : ResourceHelper.XDocResource(LibraryPack + "/Defaults/Settings.xml");
                }
                return _xSettings;
            }
            set { _xSettings = value; }
        }

        public static List<string> Settings
        {
            get { return _settings ?? (_settings = new List<string>()); }
            set { _settings = value; }
        }

        #endregion

        #region Property Bindings

        private static Dictionary<string, string> _autoTranslate;
        private static Dictionary<string, string> _chatCodes;
        private static Dictionary<string, string[]> _colors;
        private static CultureInfo _cultureInfo;

        public static Dictionary<string, string> AutoTranslate
        {
            get { return _autoTranslate ?? (_autoTranslate = new Dictionary<string, string>()); }
            set { _autoTranslate = value; }
        }

        public static Dictionary<string, string> ChatCodes
        {
            get { return _chatCodes ?? (_chatCodes = new Dictionary<string, string>()); }
            set { _chatCodes = value; }
        }

        public static string ChatCodesXml { get; set; }

        public static Dictionary<string, string[]> Colors
        {
            get { return _colors ?? (_colors = new Dictionary<string, string[]>()); }
            set { _colors = value; }
        }

        public static CultureInfo CultureInfo
        {
            get { return _cultureInfo ?? (_cultureInfo = new CultureInfo("en")); }
            set { _cultureInfo = value; }
        }

        #endregion

        #region Auto-Properties

        public static string CharacterName { get; set; }

        public static string ServerName { get; set; }

        public static string GameLanguage { get; set; }

        public static bool EnableHelpLabels { get; set; }

        #endregion
    }
}
