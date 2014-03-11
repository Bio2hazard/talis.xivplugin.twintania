// Talis.XIVPlugin.Twintania
// EventSubscriber.cs
// 
// 	

using System;
using System.Linq;
using FFXIVAPP.IPluginInterface.Events;
using NLog;
using Talis.XIVPlugin.Twintania.Helpers;
using Talis.XIVPlugin.Twintania.Properties;
using Talis.XIVPlugin.Twintania.Utilities;
using Talis.XIVPlugin.Twintania.Windows;

namespace Talis.XIVPlugin.Twintania
{
    public static class EventSubscriber
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

        public static void Subscribe()
        {
            Plugin.PHost.NewConstantsEntity += OnNewConstantsEntity;
            Plugin.PHost.NewChatLogEntry += OnNewChatLogEntry;
            Plugin.PHost.NewMonsterEntries += OnNewMonsterEntries;
        }

        public static void UnSubscribe()
        {
            Plugin.PHost.NewConstantsEntity -= OnNewConstantsEntity;
            Plugin.PHost.NewChatLogEntry -= OnNewChatLogEntry;
            Plugin.PHost.NewMonsterEntries -= OnNewMonsterEntries;
        }

        #region Subscriptions

        private static void OnNewConstantsEntity(object sender, ConstantsEntityEvent constantsEntityEvent)
        {
            // delegate event from constants, not required to subsribe, but recommended as it gives you app settings
            if (sender == null)
            {
                return;
            }
            var constantsEntity = constantsEntityEvent.ConstantsEntity;
            Constants.AutoTranslate = constantsEntity.AutoTranslate;
            Constants.ChatCodes = constantsEntity.ChatCodes;
            Constants.Colors = constantsEntity.Colors;
            Constants.CultureInfo = constantsEntity.CultureInfo;
            Constants.CharacterName = constantsEntity.CharacterName;
            Constants.ServerName = constantsEntity.ServerName;
            Constants.GameLanguage = constantsEntity.GameLanguage;
            Constants.EnableHelpLabels = constantsEntity.EnableHelpLabels;
            PluginViewModel.Instance.EnableHelpLabels = Constants.EnableHelpLabels;
        }

        private static void OnNewChatLogEntry(object sender, ChatLogEntryEvent chatLogEntryEvent)
        {
            // delegate event from chat log, not required to subsribe
            // this updates 100 times a second and only sends a line when it gets a new one
            if (sender == null)
            {
                return;
            }
            var chatLogEntry = chatLogEntryEvent.ChatLogEntry;
            try
            {
                /*if (chatLogEntry.Line.ToLower()
                                .StartsWith("com:"))
                {
                    LogPublisher.HandleCommands(chatLogEntry);
                }*/
                LogPublisher.Process(chatLogEntry);
            }
            catch (Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }

        private static void OnNewMonsterEntries(object sender, ActorEntitiesEvent actorEntitiesEvent)
        {
            // delegate event from monster entities from ram, not required to subsribe
            // this updates 10x a second and only sends data if the items are found in ram
            // currently there no change/new/removed event handling (looking into it)
            if (sender == null || TwintaniaWidgetViewModel.Instance.TestMode)
            {
                return;
            }
            var monsterEntities = actorEntitiesEvent.ActorEntities;

            var twintania = monsterEntities.SingleOrDefault(monster => (monster.NPCID1 == 4295027 && monster.NPCID2 == 2021));
            if (twintania != null && twintania.IsValid && twintania.HPCurrent > 0)
            {

                TwintaniaWidgetViewModel.Instance.TwintaniaEntity = twintania;
                TwintaniaWidgetViewModel.Instance.TwintaniaIsValid = true;
                TwintaniaWidgetViewModel.Instance.TwintaniaHPPercent = (double) twintania.HPPercent;
                if (twintania.IsClaimed && !TwintaniaWidgetViewModel.Instance.TwintaniaEngaged)
                {
                    LogHelper.Log(Logger, "Twintania engaged in combat.", LogLevel.Debug);
                    TwintaniaWidgetViewModel.Instance.EnrageTimerStart();
                    TwintaniaWidgetViewModel.Instance.TwintaniaEngaged = true;
                } 
                else if (TwintaniaWidgetViewModel.Instance.TwintaniaEngaged && !twintania.IsClaimed)
                {
                    LogHelper.Log(Logger, "Twintania found, but not engaged in combat.", LogLevel.Debug);
                    TwintaniaWidgetViewModel.Instance.EnrageTimerStop();
                    TwintaniaWidgetViewModel.Instance.TwintaniaEngaged = false;
                }
                TwintaniaWidgetViewModel.Instance.CheckCurrentPhase();
            }
            else if (TwintaniaWidgetViewModel.Instance.TwintaniaIsValid)
            {
                if (twintania != null)
                    LogHelper.Log(Logger, "Twintania no longer tracked. ( IsValid:" + twintania.IsValid + " HPCurrent:" + twintania.HPCurrent + " )", LogLevel.Debug);
                else
                    LogHelper.Log(Logger, "Twintania no longer tracked. ( Not found in memory )", LogLevel.Debug);

                TwintaniaWidgetViewModel.Instance.DivebombTimerStop();
                TwintaniaWidgetViewModel.Instance.EnrageTimerStop();
                TwintaniaWidgetViewModel.Instance.TwintaniaEntity = null;
                TwintaniaWidgetViewModel.Instance.TwintaniaIsValid = false;
                TwintaniaWidgetViewModel.Instance.TwintaniaEngaged = false;
                TwintaniaWidgetViewModel.Instance.TwintaniaHPPercent = 0;
                TwintaniaWidgetViewModel.Instance.TwintaniaDivebombCount = 1;
                TwintaniaWidgetViewModel.Instance.TwintaniaDivebombTimeToNextCur = 0;
                TwintaniaWidgetViewModel.Instance.TwintaniaDivebombTimeToNextMax = 0;
                TwintaniaWidgetViewModel.Instance.WidgetTitle = "P1";
                TwintaniaWidgetViewModel.Instance.CurrentPhase = 1;
                TwintaniaWidgetViewModel.Instance.hasPhaseAlertPlayed = false;
            }

            var dreadknight = monsterEntities.SingleOrDefault(monster => (monster.NPCID1 == 4295031 && monster.NPCID2 == 2026));
            if (dreadknight != null && dreadknight.IsValid && dreadknight.HPCurrent > 0)
            {
                TwintaniaWidgetViewModel.Instance.DreadknightEntity = dreadknight;
                TwintaniaWidgetViewModel.Instance.DreadknightIsValid = true;
                TwintaniaWidgetViewModel.Instance.DreadknightHPPercent = (double) dreadknight.HPPercent;
            }
            else if (TwintaniaWidgetViewModel.Instance.DreadknightIsValid)
            {
                if (dreadknight != null)
                    LogHelper.Log(Logger, "Dread Knight no longer tracked. ( IsValid:" + dreadknight.IsValid + " HPCurrent:" + dreadknight.HPCurrent + " )", LogLevel.Debug);
                else
                    LogHelper.Log(Logger, "Dread Knight no longer tracked. ( Not found in memory )", LogLevel.Debug);

                TwintaniaWidgetViewModel.Instance.DreadknightEntity = null;
                TwintaniaWidgetViewModel.Instance.DreadknightIsValid = false;
                TwintaniaWidgetViewModel.Instance.DreadknightHPPercent = 0;
            }

            // Get NPCID next run
            var asclepius = monsterEntities.SingleOrDefault(monster => (monster.Name == "Asclepius"));
            if (asclepius != null && asclepius.IsValid && dreadknight.HPCurrent >= 0)
            {
                TwintaniaWidgetViewModel.Instance.AsclepiusEntity = asclepius;
                TwintaniaWidgetViewModel.Instance.AsclepiusHPPercent = (double)asclepius.HPPercent;
                TwintaniaWidgetViewModel.Instance.AsclepiusIsValid = true;
            }
            else if (TwintaniaWidgetViewModel.Instance.AsclepiusIsValid)
            {
                if (asclepius != null)
                    LogHelper.Log(Logger, "Asclepius no longer tracked. ( IsValid:" + asclepius.IsValid + " HPCurrent:" + asclepius.HPCurrent + " )", LogLevel.Debug);
                else
                    LogHelper.Log(Logger, "Asclepius no longer tracked. ( Not found in memory )", LogLevel.Debug);

                TwintaniaWidgetViewModel.Instance.AsclepiusIsValid = false;
                TwintaniaWidgetViewModel.Instance.AsclepiusHPPercent = 0;
                TwintaniaWidgetViewModel.Instance.AsclepiusEntity = null;
            }
        }

        #endregion
    }
}
