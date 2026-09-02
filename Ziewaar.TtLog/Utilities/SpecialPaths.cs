using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        public static string AppDataRoot
        {
            get
            {
                return Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData);
            }
        }
        private static string backingAssyAppdata = null;
        public static string AppDataForAssy
        {
            get
            {
                if (backingAssyAppdata == null)
                {
                    backingAssyAppdata = Path.Combine(AppDataRoot, AppName);
                    if (!Directory.Exists(backingAssyAppdata))
                        Directory.CreateDirectory(backingAssyAppdata);
                    return backingAssyAppdata;
                }
                else
                {
                    return backingAssyAppdata;
                }
            }
            set
            {
                backingAssyAppdata = value;
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
            var completePath = Path.Combine(AppDataRoot, pathBuilder.ToString());
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
            var pathDirectory = Path.Combine(AppDataRoot, pathBuilder.ToString());
            if (!Directory.Exists(pathDirectory))
                Directory.CreateDirectory(pathDirectory);
            return pathDirectory;

        }
        public static string LogDirectory
        {
            get
            {
                var dir = Path.Combine(AppDataForAssy, "log");
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
