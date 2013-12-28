// talis.xivplugin.twintania
// SoundHelper.cs

using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Utilities;
using NAudio.Wave;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using talis.xivplugin.twintania.Properties;

namespace talis.xivplugin.twintania.Helpers
{
    internal static class SoundHelper
    {
        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        private static Dictionary<string, Tuple<IWavePlayer, WaveChannel32>> soundFiles = new Dictionary<string, Tuple<IWavePlayer, WaveChannel32>>();

        private static WaveChannel32 loadStream(string path)
        {
            if (path.EndsWith("mp3"))
                return new WaveChannel32(new Mp3FileReader(path));
            else if (path.EndsWith("wav"))
                return new WaveChannel32(new WaveFileReader(path));
            else
                throw new Exception("Invalid sound file " + path);
        }

        public static void Play(string file, int volume)
        {
            try
            {
                if(Settings.Default.TwintaniaHPWidgetUseNAudio)
                {
                    IWavePlayer player;
                    WaveChannel32 stream;

                    if(Settings.Default.TwintaniaHPWidgetUseSoundCaching)
                    {
                        Tuple<IWavePlayer, WaveChannel32> value;

                        if (soundFiles.TryGetValue(file, out value))
                        {
                            Logging.Log(LogManager.GetCurrentClassLogger(), "Loaded sound file " + file + " from dictionary", new Exception());
                            player = value.Item1;
                            stream = value.Item2;
                            stream.Position = 0;
                        }
                        else
                        {
                            Logging.Log(LogManager.GetCurrentClassLogger(), "Stored sound file " + Constants.BaseDirectory + file + " into dictionary", new Exception());
                            player = new WaveOut();
                            stream = loadStream(Constants.BaseDirectory + file);
                            player.Init(stream);
                            soundFiles.Add(file, Tuple.Create(player, stream));
                        }
                    }
                    else
                    {
                        player = new WaveOut();
                        stream = loadStream(Constants.BaseDirectory + file);
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
                    int NewVolume = ((ushort.MaxValue / 100) * volume);
                    uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
                    waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
                    SoundPlayerHelper.Play(Constants.BaseDirectory, file);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
        }
    }
}
