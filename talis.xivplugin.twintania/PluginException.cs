﻿// Talis.XIVPlugin.Twintania
// PluginException.cs
// 
// 	

using System;
using System.Runtime.Serialization;

namespace Talis.XIVPlugin.Twintania
{
    [Serializable]
    public class PluginException : Exception
    {
        public PluginException()
        {
        }

        public PluginException(string message) : base(message)
        {
        }

        public PluginException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PluginException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
