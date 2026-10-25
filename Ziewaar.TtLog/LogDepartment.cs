using System.Collections.Generic;
using Ziewaar.TtLog.LogFormats;

namespace Ziewaar.TtLog
{
    public class LogDepartment
    {
        public static readonly LogDepartment Default = new LogDepartment();
        public SortedSet<LogLevel> Mutes;
        public LogDepartment(params LogLevel[] mutes)
        {
            this.Mutes = new SortedSet<LogLevel>(mutes);
        }
    }
}