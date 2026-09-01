using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Ziewaar.TtLog.LogFormats;
using Ziewaar.TtLog.Writers;
using Ziewaar.TtLog.Utilities;

namespace Ejije.Logging
{
    public class DeferredLogger : IDisposable
    {
        private readonly object StartLock = new object();
        public bool IsStarting { get; private set; }

        public void Start()
        {
            lock (StartLock)
            {
                if (IsStarting)
                {
                    Debug.WriteLine("Logger already starting");
                    return;
                }
                IsStarting = true;
            }
            Debug.WriteLine("Starting Logger...");

            var thread = new Thread(new ParameterizedThreadStart(Worker));
            //thread.SetApartmentState(ApartmentState.STA);
            thread.Priority = ThreadPriority.AboveNormal;
            // thread.IsBackground = true;
            // We need to do this to give the logger thread some 
            // scheduler time to start.
            using (var loggerReady = 
                new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                thread.Start(loggerReady);
                Thread.Sleep(100);
                if (!loggerReady.WaitOne(4096))
                {
                    Debug.WriteLine(
                        "Failed to start logger; thread did not come up.");
                    Thread.Sleep(3000);
                    Environment.Exit(29);
                }
            }
        }

        private readonly ILogWriter[] Writers;

        public DeferredLogger(ILogWriter[] writers)
        {
            this.Writers = writers;
        }

        private bool IsDisposing = false;
        private readonly EventWaitHandle NewMessages =
            new EventWaitHandle(false, EventResetMode.AutoReset);
        public readonly Stopwatch Stopwatch = Stopwatch.StartNew();        
        private readonly object SwapLock = new object();
        private int Unflushed = 0;
        private long LastFlush = 0;
        private Queue<LogEvent> EventSink = new Queue<LogEvent>();
        private Queue<LogEvent> EventsToPrint = new Queue<LogEvent>();

        private const int
            MESSAGE_TIMEOUT_MS = 2048,
            FLUSH_COUNT_TRESHOLD_N = 128,
            FLUSH_TIMEOUT_COOLDOWN_MS = 2048,
            BUFFER_PURGE_LIMIT_N = 256;

        private void Worker(object loggerReadyObject)
        {
            Debug.WriteLine("Enter Log Worker...");            
            UsingWorker((EventWaitHandle)loggerReadyObject, 0);
        }

        private void UsingWorker(EventWaitHandle loggerReady, int writerIndex)
        {
            if (writerIndex < Writers.Length)
            {
                using (Writers[writerIndex])
                    UsingWorker(loggerReady, writerIndex + 1);
            } else
            {
                loggerReady.Set();
                Debug.WriteLine("Log available signalled");
                while (!this.IsDisposing)
                {
                    if (EventSink.Count > 0 || EventsToPrint.Count > 0
                        || NewMessages.WaitOne(MESSAGE_TIMEOUT_MS))
                    {
                        SwapLogQueues();
                    }
                    else if (Unflushed > FLUSH_COUNT_TRESHOLD_N ||
                        (Stopwatch.ElapsedMilliseconds - LastFlush)
                        > FLUSH_TIMEOUT_COOLDOWN_MS)
                    {
                        LastFlush = Stopwatch.ElapsedMilliseconds;
                        Unflushed = 0;
                        foreach (var item in Writers)
                            item.Flush();
                    }
                }
            }
        }

        public void PostMessage(LogEvent le)
        {
            if (!IsStarting)
            {
                throw new InvalidOperationException(
                    "Cannot log because the logger has not been started.\n" + 
                    "Capture Log.Start() in a using block in " + 
                    "Program.Main");
            }
            lock (SwapLock)
            {
                EventSink.Enqueue(le);
                NewMessages.Set();
            }
        }

        private void SwapLogQueues()
        {
            Queue<LogEvent> swap;
            lock (SwapLock)
            {
                if (EventsToPrint.Count == 0)
                {
                    if (EventSink.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        swap = EventSink;
                        EventSink = EventsToPrint;
                        EventsToPrint = swap;
                    }
                }
            }
            for (int i = 0;
                i < BUFFER_PURGE_LIMIT_N &&
                EventsToPrint.Count > 0; i++)
            {
                Unflushed++;
                var logEvent = EventsToPrint.Dequeue();
                var timeStamp = SpecialPaths.ApplicationStartTime.Add(logEvent.AtTick);
                foreach (var item in Writers)
                    item.Write(timeStamp, logEvent);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.IsDisposing = true;
        }

        #endregion
    }
}
