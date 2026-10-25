using System;
using System.Collections.Generic;
using System.Text;
using TtLog.LogFormats;

namespace TtLog.Writers
{
    public interface ILogWriter : IDisposable
    {
        void Flush();
        void Write(DateTime adjustedTimestamp, LogEvent logEvent);
    }
}
