using System;
using System.Collections.Generic;
using System.Text;

namespace Ziewaar.TtLog.ModifiedConsole
{
    public static class WrappingConsoleSingleton
    {
        public static readonly WrappingConsole WrapCons = new WrappingConsole();
    }
}
