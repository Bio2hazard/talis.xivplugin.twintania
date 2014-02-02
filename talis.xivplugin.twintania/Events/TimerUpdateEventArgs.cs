
namespace Talis.XIVPlugin.Twintania.Events
{
    public class TimerUpdateEventArgs
    {
        public double TimeToEvent { get; private set; }

        public TimerUpdateEventArgs(double timeToEvent)
        {
            TimeToEvent = timeToEvent;
        }
    }
}
