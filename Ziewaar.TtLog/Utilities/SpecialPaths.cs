using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Reflection;

namespace Ziewaar.TtLog.Utilities
{
    public static class SpecialPaths
    {
        private static string _customAppName = "";
        public static string AssemblyPath
        {
            get { return Assembly.GetExecutingAssembly().Location; }
        }
        public static string AssemblyDirectory
        {
            get { return Path.GetDirectoryName(AssemblyPath); }
        }
        public static string[] NeighbouringDlls
        {
            get { return Directory.GetFiles(AssemblyDirectory, "*.dll"); }
        }
        public static string AppName
        {
            get
            {
                if (string.IsNullOrEmpty(_customAppName))
                {
                    var fileName = Path.GetFileNameWithoutExtension(AssemblyPath);
                    _customAppName = fileName;
                }
                return _customAppName;
            }
            set
            {
                _customAppName = value;
            }
        }
        public static string AppData
        {
            get
            {
                return Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData);
            }
        }
        public static string EjijeData
        {
            get
            {
                var dir = Path.Combine(AppData, "ejije");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }
        public static string GetDataFile(
            string applicationName, string fileName, params string[] subDirectory)
        {
            StringBuilder pathBuilder = new StringBuilder().
                Append(applicationName).
                Append(Path.DirectorySeparatorChar);
            foreach (var item in subDirectory)
                pathBuilder.Append(item).Append(Path.DirectorySeparatorChar);
            pathBuilder.Append(fileName);
            var completePath = Path.Combine(AppData, pathBuilder.ToString());
            var pathDirectory = Path.GetDirectoryName(completePath);
            if (!Directory.Exists(pathDirectory))
                Directory.CreateDirectory(pathDirectory);
            return completePath;
        }
        public static string GetDataDirectory(
            string applicationName, params string[] subDirectory)
        {
            StringBuilder pathBuilder = new StringBuilder().
                Append(applicationName).
                Append(Path.DirectorySeparatorChar);
            foreach (var item in subDirectory)
                pathBuilder.Append(item).Append(Path.DirectorySeparatorChar);
            var pathDirectory = Path.Combine(AppData, pathBuilder.ToString());
            if (!Directory.Exists(pathDirectory))
                Directory.CreateDirectory(pathDirectory);
            return pathDirectory;

        }
        public static string LogDirectory
        {
            get
            {
                var dir = Path.Combine(EjijeData, "log");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }
        public static DateTime ApplicationStartTime
        {
            get
            {
                return Process.GetCurrentProcess().StartTime;
            }
        }
        public static string CurrentLogFile
        {
            get
            {
                var dateString = ApplicationStartTime.ToString(
                    "yyMMdd HHmmss");
                var userName = Regex.Replace(
                    Environment.UserName,
                    @"[^\w]", "-");
                var pid = Process.GetCurrentProcess().Id;
                var logName = string.Format(
                    "{2}-{0}-PID{1}.log.html", userName, pid, dateString);
                var logFile = Path.Combine(LogDirectory, logName);
                if (!File.Exists(logFile))
                    File.WriteAllLines(logFile, new string[] {
                        "<!-- start of log -->"
                    });
                return logFile;
            }
        }
    }
}
