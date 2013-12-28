using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace talis.xivplugin.twintania.Events
{
    public class TimerUpdateEventArgs
    {
        public double TimeToEvent { get; private set; }

        public TimerUpdateEventArgs(double timeToEvent)
        {
            this.TimeToEvent = timeToEvent;
        }
    }
}
