using System;

namespace Ziewaar.TtLog.Writers
{
    [Flags]
    public enum PreviousEntryCompare
    {
        AllSame = 0,
        CloseGroup = 1,
        NewGroup = 2,
        NewDate = 4,
        NewTime = 8,
        FirstGroup = NewGroup | NewDate | NewTime,
        AllDifferent = CloseGroup | NewGroup | NewDate | NewTime,
    }
}