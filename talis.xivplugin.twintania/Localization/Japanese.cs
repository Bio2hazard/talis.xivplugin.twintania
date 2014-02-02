// talis.xivplugin.twintania
// Japanese.cs

using System.Windows;

namespace Talis.XIVPlugin.Twintania.Localization
{
    public abstract class Japanese
    {
        private static readonly ResourceDictionary Dictionary = new ResourceDictionary();

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public static ResourceDictionary Context()
        {
            Dictionary.Clear();
            return Dictionary;
        }
    }
}
