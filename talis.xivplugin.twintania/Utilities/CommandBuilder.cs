// FFXIVAPP.Plugin.Widgets
// CommandBuilder.cs
// 
// © 2013 ZAM Network LLC

using System.Text.RegularExpressions;
using FFXIVAPP.Common.RegularExpressions;

namespace FFXIVAPP.Plugin.Widgets.Utilities
{
    public static class CommandBuilder
    {
        public static readonly Regex CommandsRegEx = new Regex(@"^com:widgets (?<widget>\w+)$", SharedRegEx.DefaultOptions);
    }
}
