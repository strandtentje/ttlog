using System;
using System.Text;

namespace Ziewaar.TtLog
{
    public partial class WrappingConsole
    {
        public WrappingConsole ColorOf(object obj)
        {
            Console.ForegroundColor = (ConsoleColor)(Convert.ToInt32(obj) % 16);
            return this;
        }

        public WrappingConsole UnInv()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            return this;
        }

        public WrappingConsole InvDarkCyan()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            return this;
        }

        public WrappingConsole InvCyan()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Cyan;
            return this;
        }

        public WrappingConsole DarkCyan() => ColorOf(ConsoleColor.DarkCyan);

        public WrappingConsole Cyan() => ColorOf(ConsoleColor.Cyan);

        public WrappingConsole Gray() => ColorOf(ConsoleColor.Gray);

        public WrappingConsole DarkGray() => ColorOf(ConsoleColor.DarkGray);

        public WrappingConsole White() => ColorOf(ConsoleColor.White);

    }
}
