using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Ziewaar.TtLog.LogFormats;
using Ejije.Logging;

namespace Ziewaar.TtLog.Utilities
{
    public static class LoggingExtensions
    {
        public static DeferredLogger PostMessage(
            this DeferredLogger logger,
            FormatTemplate template,
            params object[] args)
        {
            if (logger == null)
                throw new InvalidOperationException("Wasn't started yet a message was posted.");
            else
                logger.PostMessage(new LogEvent()
                {
                    AtTick = logger.Stopwatch.Elapsed,
                    Template = template,
                    Params = args,
                });
            return logger;
        }
    }
}
