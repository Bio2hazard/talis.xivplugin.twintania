// Talis.XIVPlugin.Twintania
// LogPublisher.cs
// 
// 	

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FFXIVAPP.Common.Core.Memory;
using NLog;
using Talis.XIVPlugin.Twintania.Helpers;
using Talis.XIVPlugin.Twintania.Properties;
using Talis.XIVPlugin.Twintania.Windows;

namespace Talis.XIVPlugin.Twintania.Utilities
{
    public static class LogPublisher
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
                if (TwintaniaWidgetViewModel.Instance.TwintaniaIsValid)
                {
                    var line = chatLogEntry.Line.Replace("  ", " ");
                    var name = TwintaniaWidgetViewModel.Instance.TwintaniaEntity.Name;

                    switch (chatLogEntry.Code)
                    {
                        case "2AAB":

                            if (Regex.IsMatch(line, @"(?i)^\s*.*" + name + @".*(" + string.Join("|", divebomb.Select(Regex.Escape)
                                                                                                                   .ToArray()) + @")"))
                            {
                                TwintaniaWidgetViewModel.Instance.TriggerDiveBomb();
                            }
                            else if (Regex.IsMatch(line, @"(?i)^\s*.*" + name + @".*(" + string.Join("|", twister.Select(Regex.Escape)
                                                                                                                       .ToArray()) + @")"))
                            {
                                TwintaniaWidgetViewModel.Instance.TriggerTwister();
                            }
                            else
                            {
                                LogHelper.Log(Logger, "Chat Code " + chatLogEntry.Code + " received - did not match twister or divebomb. Twintania's recorded name:" + name + " Message:" + line, LogLevel.Debug);
                            }


                            break;

                        case "292B":
                        case "312B":
                        case "28AB":
                            if (Regex.IsMatch(line, @"(?i)^\s*.*" + name + @".*(" + string.Join("|", deathsentence.Select(Regex.Escape)
                                                                                                                        .ToArray()) + @")"))
                            {
                                TwintaniaWidgetViewModel.Instance.TriggerDeathSentence();
                            }
                            else
                            {
                                LogHelper.Log(Logger, "Chat Code " + chatLogEntry.Code + " received - did not death sentence. Twintania's recorded name:" + name + " Message:" + line, LogLevel.Debug);
                            }
                            break;

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
