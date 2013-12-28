// talis.xivplugin.twintania
// French.cs

using System.Windows;

namespace talis.xivplugin.twintania.Localization
{
    public abstract class French
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
