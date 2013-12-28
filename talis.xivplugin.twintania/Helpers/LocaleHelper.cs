﻿// talis.xivplugin.twintania
// LocaleHelper.cs

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using talis.xivplugin.twintania.Localization;

namespace talis.xivplugin.twintania.Helpers
{
    internal static class LocaleHelper
    {
        /// <summary>
        /// </summary>
        /// <param name="cultureInfo"> </param>
        public static Dictionary<string, string> Update(CultureInfo cultureInfo)
        {
            var culture = cultureInfo.TwoLetterISOLanguageName;
            ResourceDictionary dictionary;
            if (Constants.Supported.Contains(culture))
            {
                switch (culture)
                {
                    case "ja":
                        dictionary = Japanese.Context();
                        break;
                    case "de":
                        dictionary = German.Context();
                        break;
                    case "fr":
                        dictionary = French.Context();
                        break;
                    default:
                        dictionary = English.Context();
                        break;
                }
            }
            else
            {
                dictionary = English.Context();
            }
            return dictionary.Cast<DictionaryEntry>()
                             .ToDictionary(item => (string) item.Key, item => (string) item.Value);
        }
    }
}
