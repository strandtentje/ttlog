using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using TtLog.LogFormats;
using TtLog.Utilities;
using TtLog.Writers;

namespace TtLog
{
//    public class CliInput : IDisposable
//    {
//        private Dictionary<EventWaitHandle, string> CommandReceivers = new Dictionary<EventWaitHandle, string>();
//        private static EventWaitHandle Disposal = new EventWaitHandle(false, EventResetMode.ManualReset);
//        private static CliInput Instance = null;
//        private bool InputBoxActive;
//        private static readonly object InstanceLock = new object();
//        public CliInput Start()
//        {
//            lock (InstanceLock)
//            {
//                if (Instance != null)
//                    throw new InvalidOperationException("Cli Input already started");
//                Instance = new CliInput();
//            }

//            var inputThread = new Thread(Instance.ReadInputs);
//            inputThread.Start();



//            return Instance;
//        }

//        private StringBuilder CommandBuilder = new StringBuilder();

//        private void ReadInputs(object obj)
//        {
//            while(TryKeypress(out var keyPress))
//            {
//                if (keyPress.Key == ConsoleKey.Oem3)
//                    this.InputBoxActive = !this.InputBoxActive;
//                if (keyPress.Key == ConsoleKey.Escape)
//                    this.InputBoxActive = false;
//                if (this.InputBoxActive)
//                {
//                    if (char.IsLetterOrDigit(keyPress.KeyChar) && CommandBuilder.Length < 65)
//                    {
//                        CommandBuilder.Append(keyPress.KeyChar);
//                        Redraw();
//                    } else if (keyPress.Key == ConsoleKey.Backspace && CommandBuilder.Length > 0)
//                    {
//                        CommandBuilder.Remove(CommandBuilder.Length - 1, 1);
//                        Redraw();
//                    } else if (keyPress.Key == ConsoleKey.Enter)
//                    {
//                        lock(CommandReceivers)
//                        {
//                            var cmd = CommandBuilder.ToString();
//                            foreach (var ewh in CommandReceivers.Keys.ToArray())
//                            {
//                                CommandReceivers[ewh] = cmd;
//                                ewh.Set();
//                            }
//                        }
//                        this.InputBoxActive = false;
//                        CommandBuilder.Clear();
//                        Redraw();
//                    }
//                }
//            }
//        }

//        private bool TryKeypress(out ConsoleKeyInfo cki)
//        {
//            while (!Console.KeyAvailable && Disposal != null && !Disposal.WaitOne(100))
//            {
//                Redraw();
//            }
//            if (Console.KeyAvailable)
//            {
//                cki = Console.ReadKey();
//                return true;
//            }
//            else
//            {
//                cki = default;
//                return false;
//            }
//        }
//        private int LastStringLength, LastWidth, LastHeight;

//        /*
//        private static readonly string
//            GroupOnlyCloser = string.Concat('┌', new string('─', 6), '┘'),
//            OpenerTail = string.Concat('└', new string('─', 6), '┐');
//*/

//        private void Redraw(bool force = false)
//        {
//            if (InputBoxActive == false) return;
//            if (Console.IsOutputRedirected) return;
//            if (!force && LastStringLength == CommandBuilder.Length && LastWidth == Console.WindowWidth && LastHeight == Console.WindowHeight)
//                return;
//            var oldLeft = Console.CursorLeft;
//            var oldTop = Console.CursorTop;
//            Console.CursorLeft = 2;
//            Console.CursorTop = Console.BufferHeight - 4;
//            Console.Write("┌{0}┐", new string('─', Console.BufferWidth - 4));
//            Console.CursorTop = Console.BufferHeight - 3;
//            var ss = CommandBuilder.ToString();
//            if (ss.Length > Console.BufferWidth - 8)
//                ss = ss.Substring(ss.Length - (Console.BufferWidth - 8));
//            else
//                ss = ss.PadRight(Console.BufferWidth - 8);
//            Console.Write("│ {0} │", ss);
//            Console.CursorTop = Console.BufferHeight - 2;
//            Console.Write("└{0}┘", new string('─', Console.BufferWidth - 4));
//            Console.CursorTop = oldTop;
//            Console.CursorLeft = oldLeft;

//            Console.
//        }

//        public string ReadCommand()
//        {
//            using (var ewh = new EventWaitHandle(false, EventResetMode.ManualReset))
//            {
//                string receivedCommand = null;
//                try
//                {
//                    if (CommandReceivers == null) throw new ObjectDisposedException(nameof(CliInput));
//                    lock (CommandReceivers)
//                        CommandReceivers.Add(ewh, null);
//                    while (CommandReceivers.TryGetValue(ewh, out receivedCommand) && !ewh.WaitOne(1000)) ;
//                }
//                catch (NullReferenceException)
//                {
//                    // ok
//                }
//                catch (ObjectDisposedException)
//                {
//                    // ok                    
//                }
//                finally
//                {
//                    if (CommandReceivers != null)
//                        lock (CommandReceivers)
//                            CommandReceivers?.Remove(ewh);
//                }
//                return receivedCommand;
//            }
//        }

//        public void Dispose()
//        {
//            var ewhs = CommandReceivers.Keys.ToArray();
//            if (CommandReceivers == null) throw new ObjectDisposedException(nameof(CliInput));
//            lock (CommandReceivers)
//            {
//                foreach (var ewh in ewhs)
//                    using (ewh)
//                        CommandReceivers.Remove(ewh);
//                CommandReceivers = null;
//            }
//            using (Disposal)
//                Disposal.Set();
//            Disposal = null;
//            lock (InstanceLock)
//                Instance = null;
//        }
//    }

    public class Log : FormatTemplate
    {
        private Log(LogDepartment department, LogLevel level, IEnumerable<FormatComponent> components) : base(
            department, level, components)
        {
        }

        public static Log From(LogLevel level, string format)
        {
            return From(LogDepartment.Default, level, format);
        }

        public static Log From(LogDepartment department, LogLevel level, string format)
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

            return new Log(department, level, components);
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

        public static Log Tech(LogDepartment department, string format)
        {
            return From(department, LogLevel.Tech, format);
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

        private const long MAX_LOG_DIR_SIZE = 1024 * 1024 * 1024; // a million k's, or a thousand m's, or a single g.

        private static readonly Log
            LoggerStarted = Log.Info("Logger into {file} has started"),
            LoggerAlreadyStarted = Log.Warn("Logger was already started!"),
            ZippedFiles = Log.Tech("Before starting logger, zipped old {log} into {zip}"),
            ZipFail = Log.Oops("Before starting logger, failed to put {log} into {zip} due to {exception}"),
            PurgedOldZip = Log.Tech("Before starting logger, removed old {logarchive}"),
            PurgeQueryFail = Log.Oops("Before starting logger, failed to query old archives due to {exception}"),
            PurgeFail = Log.Oops("Before starting logger, failed to remove old {archive} due to {exception}");

        public static IDisposable Start()
        {
            if (Instance == null)
            {
                var zipFiles = new HashSet<(string zip, string log)>();
                var zipExceptions = new List<(string zip, string log, Exception ex)>();

                var siblingFiles = new DirectoryInfo(SpecialPaths.LogDirectory).GetFiles("*.log.html");

                foreach (var item in siblingFiles)
                {
                    if (item.Name.Length < 4) continue;
                    if (!item.Name.Take(4).All(char.IsDigit)) continue;
                    var datePrefix = item.Name.Substring(0, 4);
                    var zipFile = $"{datePrefix}.log.zip";
                    try
                    {
                        var zipPath = Path.Combine(SpecialPaths.LogDirectory, zipFile);
                        var mode = File.Exists(zipPath) ? ZipArchiveMode.Update : ZipArchiveMode.Create;
                        using (var zip = ZipFile.Open(zipPath, mode))
                            zip.CreateEntryFromFile(item.FullName, item.Name);
                        zipFiles.Add((zipFile, item.Name));
                        item.Delete();
                    }
                    catch (Exception ex)
                    {
                        zipExceptions.Add((zipFile, item.Name, ex));
                    }
                }

                Exception preQueryException = null;
                var deleteExceptions = new HashSet<(string file, Exception ex)>();
                var toPurge = new FileInfo[] { };
                try
                {
                    var allZipsSorted = new DirectoryInfo(SpecialPaths.LogDirectory).GetFiles("*.log.zip")
                        .OrderBy(x => x.Name).ToArray();
                    var combinedZipSize = allZipsSorted.Sum(x => x.Length);

                    toPurge = allZipsSorted.TakeWhile(x =>
                    {
                        var mustPurgeThis = combinedZipSize > MAX_LOG_DIR_SIZE;
                        combinedZipSize -= x.Length;
                        return mustPurgeThis;
                    }).ToArray();

                    foreach (var item in toPurge)
                    {
                        try
                        {
                            item.Delete();
                        }
                        catch (Exception ex)
                        {
                            deleteExceptions.Add((item.Name, ex));
                        }
                    }
                }
                catch (Exception ex)
                {
                    preQueryException = ex;
                }


                var newLogFile = SpecialPaths.CurrentLogFile;
                Instance = new DeferredLogger(
                    new ILogWriter[] { new ConsoleLogWriter(), new HtmlLogWriter(newLogFile) });
                Instance.Start();

                Post(LoggerStarted, newLogFile);

                foreach (var item in zipFiles)
                    Log.Post(ZippedFiles, item.log, item.zip);
                foreach (var item in zipExceptions)
                    Log.Post(ZipFail, item.log, item.zip, item.ex);
                foreach (var item in toPurge)
                    Log.Post(PurgedOldZip, item);
                if (preQueryException != null)
                    Log.Post(PurgeQueryFail, preQueryException);
                foreach (var item in deleteExceptions)
                    Log.Post(PurgeFail, item.file, item.ex);

                return Instance;
            }
            else
            {
                Post(LoggerAlreadyStarted);
                return new FakeDisposable();
            }
        }

        public static bool IsTechHardMuted { get; set; }
        public static void Post(
            FormatTemplate template,
            params object[] args)
        {
            if (template.Level == LogLevel.Tech && IsTechHardMuted)
                return;
            if (!template.Department.Mutes.Contains(template.Level))
                Instance.PostMessage(template, args);
        }
    }
}
