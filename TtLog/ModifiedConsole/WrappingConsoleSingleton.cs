using System;
using System.Collections.Generic;
using System.Text;

namespace TtLog.ModifiedConsole
{
    public static class WrappingConsoleSingleton
    {
        public static readonly WrappingConsole WrapCons = new WrappingConsole();
    }
}
