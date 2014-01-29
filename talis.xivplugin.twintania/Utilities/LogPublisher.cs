// talis.xivplugin.twintania
// LogPublisher.cs

using FFXIVAPP.Common.Core.Memory;
using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using talis.xivplugin.twintania.Helpers;
using talis.xivplugin.twintania.Properties;
using talis.xivplugin.twintania.Windows;

namespace talis.xivplugin.twintania.Utilities
{
    public static class LogPublisher
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

        private static List<string> divebomb = new List<string>
        {
            "Divebomb",
            "Bombe plongeante",
            "Sturzbombe",
            "ダイブボム"
        };

        private static List<string> twister = new List<string>
        {
            "Twister",
            "Tornade",
            "Wirbelsturm",
            "ツイスター"
        };

        private static List<string> deathsentence = new List<string>
        {
            "Death Sentence",
            "Sentence de mort",
            "Todesurteil",
            "デスセンテンス"
        };

        public static void Process(ChatLogEntry chatLogEntry)
        {
            try
            {
                if(TwintaniaWidgetViewModel.Instance.TwintaniaIsValid)
                {
                    var line = chatLogEntry.Line.Replace("  ", " ");
                    var name = TwintaniaWidgetViewModel.Instance.TwintaniaEntity.Name;

                    if (chatLogEntry.Code == "2AAB" && Regex.IsMatch(line, @"(?i)^\s*.*\b" + name + @"\b.*\b(" + string.Join("|", divebomb.Select(Regex.Escape).ToArray()) + @"\b)"))
                    {
                        TwintaniaWidgetViewModel.Instance.TriggerDiveBomb();
                    }
                    else if (chatLogEntry.Code == "2AAB" && Regex.IsMatch(line, @"(?i)^\s*.*\b" + name + @"\b.*\b(" + string.Join("|", twister.Select(Regex.Escape).ToArray()) + @"\b)"))
                    {
                        TwintaniaWidgetViewModel.Instance.TriggerTwister();                        
                    }
                    else if ((chatLogEntry.Code == "292B" || chatLogEntry.Code == "312B" || chatLogEntry.Code == "28AB") && Regex.IsMatch(line, @"(?i)^\s*.*\b" + name + @"\b.*\b(" + string.Join("|", deathsentence.Select(Regex.Escape).ToArray()) + @"\b)"))
                    {
                        TwintaniaWidgetViewModel.Instance.TriggerDeathSentence();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }

        /*
        public static void HandleCommands(ChatLogEntry chatLogEntry)
        {
            // process commands
            if (chatLogEntry.Code == "0038")
            {
                var commandsRegEx = CommandBuilder.CommandsRegEx.Match(chatLogEntry.Line.Trim());
                if (commandsRegEx.Success)
                {
                    var widget = commandsRegEx.Groups["widget"].Success ? commandsRegEx.Groups["widget"].Value : "";
                    switch (widget)
                    {
                        case "dps":
                            Widgets.Instance.ShowDPSWidget();
                            break;
                    }
                }
            }
        }
         */
    }
}
