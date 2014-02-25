// Talis.XIVPlugin.Twintania
// SoundHelper.cs
// 
// 	

using FFXIVAPP.Common.Helpers;
using NLog;
using Talis.XIVPlugin.Twintania.Properties;

namespace Talis.XIVPlugin.Twintania.Helpers
{
    public static class SoundHelper
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
        public static bool PlayCached(string soundFile, int volume = 100)
        {
            var retval = SoundPlayerHelper.PlayCached(soundFile, volume);

            //LogHelper.Log(Logger, "Sound Playback with " + FFXIVAPP.Common.Constants.AudioMode + ":" + soundFile + " at volume:" + volume, LogLevel.Debug);
            LogHelper.Log(Logger, "Sound Playback:" + soundFile + " at volume:" + volume, LogLevel.Debug);

            return retval;
        }
    }
}
