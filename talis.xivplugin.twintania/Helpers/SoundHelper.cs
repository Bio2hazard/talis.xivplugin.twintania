// talis.xivplugin.twintania
// SoundHelper.cs

using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Utilities;
using NAudio.Wave;
using NLog;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using talis.xivplugin.twintania.Properties;

namespace talis.xivplugin.twintania.Helpers
{
    internal static class SoundHelper
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

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        private static Dictionary<string, Tuple<IWavePlayer, WaveChannel32>> _soundFiles = new Dictionary<string, Tuple<IWavePlayer, WaveChannel32>>();

        private static WaveChannel32 LoadStream(string path)
        {
            if (path.EndsWith("mp3"))
                return new WaveChannel32(new Mp3FileReader(path));
            if (path.EndsWith("wav"))
                return new WaveChannel32(new WaveFileReader(path));
            throw new Exception("Invalid sound file " + path);
        }

        public static void Play(string file, int volume)
        {
            try
            {
                if(Settings.Default.TwintaniaWidgetUseNAudio)
                {
                    IWavePlayer player;
                    WaveChannel32 stream;

                    if(Settings.Default.TwintaniaWidgetUseSoundCaching)
                    {
                        Tuple<IWavePlayer, WaveChannel32> value;

                        if (_soundFiles.TryGetValue(file, out value))
                        {
                            LogHelper.Log(Logger, "Loaded sound file " + file + " from dictionary", LogLevel.Trace);
                            player = value.Item1;
                            stream = value.Item2;
                            stream.Position = 0;
                        }
                        else
                        {
                            LogHelper.Log(Logger, "Stored sound file " + Constants.BaseDirectory + file + " into dictionary", LogLevel.Trace);
                            player = new WaveOut();
                            stream = LoadStream(Constants.BaseDirectory + file);
                            player.Init(stream);
                            _soundFiles.Add(file, Tuple.Create(player, stream));
                        }
                    }
                    else
                    {
                        player = new WaveOut();
                        stream = LoadStream(Constants.BaseDirectory + file);
                        player.Init(stream);
                    }

                    stream.Volume = (float)volume / 100;
                    player.Play();

                    player.PlaybackStopped += delegate
                    {
                        player.Dispose();
                    };
                }
                else
                {
                    int newVolume = ((ushort.MaxValue / 100) * volume);
                    uint newVolumeAllChannels = (((uint)newVolume & 0x0000ffff) | ((uint)newVolume << 16));
                    waveOutSetVolume(IntPtr.Zero, newVolumeAllChannels);
                    SoundPlayerHelper.Play(Constants.BaseDirectory, file);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(Logger, ex, LogLevel.Error);
            }
        }
    }
}
