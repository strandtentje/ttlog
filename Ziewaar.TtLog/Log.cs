using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ziewaar.TtLog.LogFormats;
using Ziewaar.TtLog.Writers;
using Ziewaar.TtLog.Utilities;

namespace Ejije.Logging
{
    public class Log : FormatTemplate
    {
        private Log(LogLevel level, IEnumerable<FormatComponent> components) : base(level, components)
        {
            
        }

        public static Log From(LogLevel level, string format)
        {
            List<FormatComponent> components = new List<FormatComponent>();
            int placeholderCount = 0;

            for (
                int cursor = 0;
                cursor < format.Length;
                )
            {
                var component = FormatComponent.From(
                    format, ref placeholderCount, ref cursor);
                components.Add(component);
            }
            return new Log(level, components);
        }
        public static Log Cool(string format)
        {
            return From(LogLevel.Cool, format);
        }
        public static Log Cool(params string[] format)
        {
            return From(LogLevel.Cool, string.Join(" ", format));
        }
        public static Log Oops(params string[] format)
        {
            return From(LogLevel.Oops, string.Join(" ", format));
        }
        public static Log Tech(string format)
        {
            return From(LogLevel.Tech, format);
        }
        public static Log Tech(params string[] format)
        {
            return From(LogLevel.Tech, string.Join(" ", format));
        }
        public static Log Info(string format)
        {
            return From(LogLevel.Info, format);
        }
        public static Log Info(params string[] format)
        {
            return From(LogLevel.Info, string.Join(" ", format));
        }
        public static Log Warn(string format)
        {
            return From(LogLevel.Warn, format);
        }
        public static Log Warn(params string[] format)
        {
            return From(LogLevel.Warn, string.Join(" ", format));
        }
        public static Log Fail(string format)
        {
            return From(LogLevel.Fail, format);
        }
        public static Log Fail(params string[] format)
        {
            return From(LogLevel.Fail, string.Join(" ", format));
        }
        public static Log Stop(string format)
        {
            return From(LogLevel.Stop, format);
        }
        public static Log Stop(params string[] format)
        {
            return From(LogLevel.Stop, string.Join(" ", format));
        }
        private static DeferredLogger Instance;
        private class FakeDisposable : IDisposable
        {
            public void Dispose()
            {
                
            }
        }
        private static readonly Log 
            LoggerStarted = Log.Info("Logger into {file} has started"),
            LoggerAlreadyStarted = Log.Warn("Logger was already started!");
        public static IDisposable Start()
        {       
            if (Instance == null)
            {
                var InstanceLogFile = SpecialPaths.CurrentLogFile;
                Instance = new DeferredLogger(new ILogWriter[] { new ConsoleLogWriter(), new HtmlLogWriter(InstanceLogFile) });
                Instance.Start();
        
                Post(LoggerStarted, InstanceLogFile);            
                return Instance;
            } else
            {
                Post(LoggerAlreadyStarted);                
                return new FakeDisposable();
            }
        }
        public static void Post(
            FormatTemplate template,
            params object[] args)
        {            
            Instance.PostMessage(template, args);
        }
    }
}
