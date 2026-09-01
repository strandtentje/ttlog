using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ziewaar.TtLog.LogFormats
{
    public enum LogLevel : byte
    {
        Tech = ConsoleColor.Blue,
        Info = ConsoleColor.Magenta,
        Cool = ConsoleColor.Green,
        Warn = ConsoleColor.Yellow,
        Oops = ConsoleColor.DarkYellow,
        Fail = ConsoleColor.Red,
        Stop = ConsoleColor.DarkRed,
    }
}
