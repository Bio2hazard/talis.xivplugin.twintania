// talis.xivplugin.twintania
// EventSubscriber.cs

using FFXIVAPP.Common.Core.Memory;
using FFXIVAPP.IPluginInterface.Events;
using System;
using System.Linq;
using NLog;
using talis.xivplugin.twintania.Helpers;
using talis.xivplugin.twintania.Utilities;
using talis.xivplugin.twintania.Windows;

namespace talis.xivplugin.twintania
{
    public static class EventSubscriber
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

            ActorEntity twintania = monsterEntities.SingleOrDefault(monster => (monster.NPCID1 == 4295027 && monster.NPCID2 == 2021));
            if (twintania != null && twintania.IsValid && twintania.HPCurrent > 0)
            {
                TwintaniaWidgetViewModel.Instance.TwintaniaEntity = twintania;
                TwintaniaWidgetViewModel.Instance.TwintaniaIsValid = true;
                TwintaniaWidgetViewModel.Instance.TwintaniaHPPercent = (double) twintania.HPPercent;
                if(twintania.IsClaimed && !TwintaniaWidgetViewModel.Instance.TwintaniaEngaged)
                {
                    TwintaniaWidgetViewModel.Instance.EnrageTimerStart();
                    TwintaniaWidgetViewModel.Instance.TwintaniaEngaged = true;
                }
            }
            else if(TwintaniaWidgetViewModel.Instance.TwintaniaIsValid)
            {
                TwintaniaWidgetViewModel.Instance.DivebombTimerStop();
                TwintaniaWidgetViewModel.Instance.EnrageTimerStop();
                TwintaniaWidgetViewModel.Instance.TwintaniaEntity = null;
                TwintaniaWidgetViewModel.Instance.TwintaniaIsValid = false;
                TwintaniaWidgetViewModel.Instance.TwintaniaEngaged = false;
                TwintaniaWidgetViewModel.Instance.TwintaniaHPPercent = 0;
                TwintaniaWidgetViewModel.Instance.TwintaniaDivebombCount = 1;
                TwintaniaWidgetViewModel.Instance.TwintaniaDivebombTimeToNextCur = 0;
                TwintaniaWidgetViewModel.Instance.TwintaniaDivebombTimeToNextMax = 0;
            }

            var dreadknight = monsterEntities.SingleOrDefault(monster => (monster.NPCID1 == 4295031 && monster.NPCID2 == 2026));
            if (dreadknight != null && dreadknight.IsValid && dreadknight.HPCurrent > 0)
            {
                TwintaniaWidgetViewModel.Instance.DreadknightEntity = dreadknight;
                TwintaniaWidgetViewModel.Instance.DreadknightIsValid = true;
                TwintaniaWidgetViewModel.Instance.DreadknightHPPercent = (double) dreadknight.HPPercent;
            }
            else if(TwintaniaWidgetViewModel.Instance.DreadknightIsValid)
            {
                TwintaniaWidgetViewModel.Instance.DreadknightEntity = null;
                TwintaniaWidgetViewModel.Instance.DreadknightIsValid = false;
                TwintaniaWidgetViewModel.Instance.DreadknightHPPercent = 0;
            }
        }

        #endregion
    }
}