using System;
using System.Collections.Generic;
using System.Text;
using Ziewaar.TtLog.ModifiedConsole;

namespace Ziewaar.TtLog
{
    public partial class WrappingConsole
    {
        private const int COLUMN_COUNT = 120;

        private static readonly string
            DatePad = new string('░', 6), TimePad = new string('░', 6);

        private int TotalPadLength;
        private int Cursor;
        private PadStructure[] Pads = new PadStructure[20];
        private int PadCount = 0;

        public WrappingConsole StartPad()
        {
            TotalPadLength = 0;
            Cursor = 0;
            PadCount = 0;
            Console.WriteLine();
            return this;
        }

        public WrappingConsole StartQuietPad()
        {
            Console.WriteLine();
            return this;
        }

        public WrappingConsole WritePad(ConsoleColor bg, ConsoleColor fg, string toAppend, char blanking, bool blankInvert)
        {
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
            Console.Write(toAppend);
            Pads[PadCount].Character = blanking;
            Pads[PadCount].Length = toAppend.Length;
            Pads[PadCount].Background = blankInvert ? fg : bg;
            Pads[PadCount].Foreground = blankInvert ? bg : fg;
            TotalPadLength += toAppend.Length;
            Cursor += toAppend.Length;
            PadCount++;
            return this;
        }

        public WrappingConsole WriteQuietPad(ConsoleColor bg, ConsoleColor fg, string toAppend)
        {
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
            Console.Write(toAppend);
            return this;
        }

        public WrappingConsole WriteWrapping(ConsoleColor bg, ConsoleColor fg, string toAppend, bool alwaysBreak, int chainCount)
        {
            var crlfIndex = toAppend.IndexOf("\r\n");
            if (crlfIndex >= 0 && crlfIndex < toAppend.Length)
            {
                WriteWrapping(bg, fg, toAppend.Substring(0, crlfIndex), false, 0);
                WriteWrapping(bg, fg, toAppend.Substring(crlfIndex + 2), true, 0);
                return this;
            }
            var lfIndex = toAppend.IndexOf("\n");
            if (lfIndex >= 0 && lfIndex < toAppend.Length)
            {
                WriteWrapping(bg, fg, toAppend.Substring(0, lfIndex), false, 0);
                WriteWrapping(bg, fg, toAppend.Substring(lfIndex + 1), true, 0);
                return this;
            }

            if (alwaysBreak || Cursor + toAppend.Length + 1 > COLUMN_COUNT)
            {
                Console.WriteLine();
                Cursor = TotalPadLength;
                for (int i = 0; i < PadCount; i++)
                {
                    Console.ForegroundColor = Pads[i].Foreground;
                    Console.BackgroundColor = Pads[i].Background;
                    for (int j = 0; j < Pads[i].Length; j++)
                        Console.Write(Pads[i].Character);
                }
            }
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
            if (Cursor + toAppend.Length + 1 > COLUMN_COUNT)
            {
                if (toAppend.Length < 30)
                {
                    if (chainCount > 0)
                        Console.Write('-');
                    WriteWrapping(bg, fg, toAppend, true, chainCount);
                } else
                {
                    var remainingCapacity = COLUMN_COUNT - Cursor - 2;
                    Cursor += remainingCapacity;
                    Console.Write(toAppend.Substring(0, remainingCapacity));
                    Console.Write('-');
                    WriteWrapping(bg, fg, toAppend.Substring(remainingCapacity), false, chainCount);
                }
            }
            else
            {
                Cursor += toAppend.Length;
                Console.Write(toAppend);
            }
            return this;
        }
    }
}
