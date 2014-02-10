// Talis.XIVPlugin.Twintania
// TimerUpdateEventArgs.cs
// 
// 	

namespace Talis.XIVPlugin.Twintania.Events
{
    public class TimerUpdateEventArgs
    {
        public TimerUpdateEventArgs(double timeToEvent)
        {
            TimeToEvent = timeToEvent;
        }

        public double TimeToEvent { get; private set; }
    }
}
