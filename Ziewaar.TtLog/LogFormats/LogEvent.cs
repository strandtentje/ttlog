using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ziewaar.TtLog.LogFormats
{
    public class LogEvent
    {
        private static long LogEventHandleCounter = 0;
        public long Handle; 
        public TimeSpan AtTick;
        public FormatTemplate Template;
        public object[] Params;
        public LogEvent()
        {
            Handle = Interlocked.Increment(ref LogEventHandleCounter);
        }
    }
}
