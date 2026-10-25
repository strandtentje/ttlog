using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Ziewaar.TtLog.LogFormats;
using Ziewaar.TtLog.ModifiedConsole;
using Ziewaar.TtLog.Utilities;
using static Ziewaar.TtLog.ModifiedConsole.WrappingConsoleSingleton;
using CC = System.ConsoleColor;

namespace Ziewaar.TtLog.Writers
{
    public class ConsoleLogWriter : ILogWriter
    {
        private DateTime LastTimestamp = DateTime.MinValue;
        private LogEvent LastLogEvent = null;
        public static bool IsTechIgnored = false;

        private LogLevel TemporaryTechLevel = LogLevel.Warn;
        private bool IsTechTemporarilyExpanded = false;
        private bool TryDividerDifference(DateTime currentTimestamp, LogEvent currentEvent, out CC prevCol, out PreviousEntryCompare pcc)
        {
            if (LastLogEvent == null || LastTimestamp == DateTime.MinValue)
            {
                pcc = PreviousEntryCompare.FirstGroup;
                this.LastTimestamp = currentTimestamp;
                this.LastLogEvent = currentEvent;
                prevCol = CC.Gray;
                return true;
            }
            else
            {
                pcc = PreviousEntryCompare.AllSame;
            }

            if ((LastLogEvent.Template.Level == LogLevel.Tech) && (currentEvent.Template.Level != LogLevel.Tech))
                pcc |= PreviousEntryCompare.NewGroup | PreviousEntryCompare.CloseGroup;
            if (LastTimestamp.Date != currentTimestamp.Date)
                pcc |= PreviousEntryCompare.NewDate | PreviousEntryCompare.NewGroup | PreviousEntryCompare.CloseGroup;
            if (LastTimestamp.TimeOfDay.Seconds != currentTimestamp.TimeOfDay.Seconds ||
                LastTimestamp.TimeOfDay.Minutes != currentTimestamp.TimeOfDay.Minutes ||
                LastTimestamp.TimeOfDay.Hours != currentTimestamp.TimeOfDay.Hours)
                pcc |= PreviousEntryCompare.NewTime | PreviousEntryCompare.NewGroup | PreviousEntryCompare.CloseGroup;

            prevCol = (CC)this.LastLogEvent.Template.Level;
            this.LastTimestamp = currentTimestamp;
            this.LastLogEvent = currentEvent;
            return pcc != PreviousEntryCompare.AllSame;
        }

        private static readonly string
            GroupOnlyCloser = string.Concat('┌', new string('─', 6), '┘'),
            OpenerTail = string.Concat('└', new string('─', 6), '┐');

        private void OptionallyReprintDivider(DateTime currentTimestamp, LogEvent currentEvent, out bool sameDate, out bool sameTime)
        {
            PreviousEntryCompare comparison;
            sameDate = true; sameTime = true;
            if (!TryDividerDifference(currentTimestamp, currentEvent, out var previousColour, out comparison))
                return;
            sameDate = !comparison.HasFlag(PreviousEntryCompare.NewDate);
            sameTime = !comparison.HasFlag(PreviousEntryCompare.NewTime);
            if (comparison.HasFlag(PreviousEntryCompare.CloseGroup))
            {
                WrapCons.StartQuietPad().WriteQuietPad(CC.Black, previousColour, GroupOnlyCloser);
            }
            if (comparison.HasFlag(PreviousEntryCompare.NewGroup))
            {
                if (comparison.HasFlag(PreviousEntryCompare.NewDate))
                    WrapCons.StartQuietPad().
                        WriteQuietPad(CC.Black, (CC)currentEvent.Template.Level, "│ ").
                        WriteQuietPad(CC.Black, CC.Cyan, "Date:  ").
                        WriteQuietPad(CC.Black, CC.Cyan, currentTimestamp.ToString("yy-MM-dd"));

                if (comparison.HasFlag(PreviousEntryCompare.NewTime))
                    WrapCons.StartQuietPad().
                        WriteQuietPad(CC.Black, (CC)currentEvent.Template.Level, "│ ").
                        WriteQuietPad(CC.Black, CC.DarkCyan, "Time:  ").
                        WriteQuietPad(CC.Black, CC.DarkCyan, currentTimestamp.ToString("HH:mm:ss"));

                WrapCons.StartQuietPad().WriteQuietPad(CC.Black, (CC)currentEvent.Template.Level, OpenerTail);
            }
        }

        private static readonly string
            DatePad = new string(' ', 6), TimePad = new string(' ', 6); // ░▒▓█
        private static readonly string[] LevelStrings = Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>().Aggregate(
            new string[16], (acc, i) =>
            {
                acc[(byte)i] = "  " + Enum.GetName(typeof(LogLevel), i) + " ";
                return acc;
            }, x => x).ToArray();


        public void Write(DateTime adjustedTimestamp, LogEvent logEvent)
        {
            if (!IsTechTemporarilyExpanded && logEvent.Template.Level == LogLevel.Tech && IsTechIgnored)
                return;

            if (logEvent.Template.Level >= TemporaryTechLevel)
            {
                IsTechTemporarilyExpanded = true;
            }
            else if (logEvent.Template.Level == LogLevel.Tech)
            {

            }
            else
            {
                IsTechTemporarilyExpanded = false;
            }

            var template = logEvent.Template;

            bool sameTime, sameDate;
            OptionallyReprintDivider(adjustedTimestamp, logEvent, out sameDate, out sameTime);

            WrapCons.StartPad().
                WritePad(CC.Black, (CC)template.Level, LevelStrings[(int)template.Level], ' ', false).
                WritePad(CC.Black, (CC)template.Level, "├", '│', false).
                WritePad(CC.Black, (CC)template.Level, " ", ' ', false);

            var plainTextColor = template.Level == LogLevel.Tech ? CC.DarkGray : CC.Gray;
            var varBlockColor = template.Level == LogLevel.Tech ? CC.Gray : CC.White;
            var varValueColor = template.Level == LogLevel.Tech ? CC.DarkGreen : CC.Green;

            foreach (var item in template.Components)
            {
                if (item.IsPlaceholder)
                {
                    WrapCons.WriteWrapping(CC.Black, varBlockColor, string.Format("[{0}=", item.Literal), false, 0);
                    WriteFormattedArg(item, template.Level == LogLevel.Tech, logEvent.Params);
                    WrapCons.WriteWrapping(CC.Black, varBlockColor, "]", false, 0);
                }
                else
                {
                    var parts = item.Literal.MakeWrappable().ToArray();
                    for (var i = 0; i < parts.Length; i++)
                        WrapCons.WriteWrapping(CC.Black, plainTextColor, parts[i], false, i);
                }
            }

            if (template.Level == LogLevel.Stop)
            {
                WrapCons.StartQuietPad().WriteQuietPad(CC.Black, (CC)template.Level, GroupOnlyCloser);
                this.LastLogEvent = null;
                this.LastTimestamp = DateTime.MinValue;
            }
        }


        private WrappingConsole WriteFormattedArg(FormatComponent fmt, bool isMuted, object[] args)
        {
            var missingColor = isMuted ? CC.DarkYellow : CC.Yellow;
            var valueColor = isMuted ? CC.DarkGreen : CC.Green;
            var nothingColor = isMuted ? CC.DarkMagenta : CC.Magenta;
            var badColor = isMuted ? CC.DarkRed : CC.Red;

            if (args.Length == 0 ||
                fmt.PlaceholderIndex < 0 ||
                fmt.PlaceholderIndex >= args.Length)
                return WrapCons.WriteWrapping(missingColor, CC.Black, "missing", false, 0);

            try
            {
                var arg = args[fmt.PlaceholderIndex];
                var argEx = arg as Exception;
                if (argEx != null)
                    return WriteException(badColor, 0, argEx);
                if (arg != null)
                {
                    var splitUp = arg.ToString().MakeWrappable().ToArray();
                    var iOffset = 0;
                    for (int i = 0; i < splitUp.Length; i++)
                    {
                        string item = splitUp[i];
                        if (string.IsNullOrWhiteSpace(item) && item.Contains("\r"))
                        {
                            var nextItem = splitUp.ElementAtOrDefault(i + 1);
                            if (string.IsNullOrWhiteSpace(nextItem) && nextItem?.Contains("\n") == true)
                                i++;
                            else if (nextItem == null)
                                return WrapCons;
                            WrapCons.WriteWrapping(CC.Black, valueColor, "", true, 0);
                            iOffset = i;
                        }
                        else if (string.IsNullOrWhiteSpace(item) && item.Contains("\n"))
                        {
                            var nextItem = splitUp.ElementAtOrDefault(i + 1);
                            if (nextItem == null)
                                return WrapCons;
                        }
                        else
                        {
                            WrapCons.WriteWrapping(CC.Black, valueColor, item, false, i - iOffset);
                        }
                    }

                    return WrapCons;
                }
                return WrapCons.WriteWrapping(nothingColor, CC.Black, "`null`", false, 0);
            }
            catch (Exception ex)
            {
                return WrapCons.WriteWrapping(badColor, CC.Black, string.Format("Unserializable; {0}", ex.ToString()), false, 0);
            }
        }

        private WrappingConsole WriteException(CC badColor, int depth, Exception argEx)
        {
            var targetSiteName = "`unknown`";
            var targetSiteType = "`unknown`";
            if (argEx.TargetSite != null)
            {
                targetSiteName = argEx.TargetSite.Name;
                if (argEx.TargetSite.DeclaringType != null)
                    targetSiteType = argEx.TargetSite.DeclaringType.Name;
            }

            WrapCons.
                WriteWrapping(CC.Black, badColor, string.Format(" {0}:{1}", depth.ToString("000"), argEx.GetType().Name), true, 0).
                WriteWrapping(CC.Black, badColor, string.Format("  Message: \"{0}\"", argEx.Message), true, 0).
                WriteWrapping(CC.Black, badColor, string.Format("  Thrown by: {0}.{1}", targetSiteType, targetSiteName), true, 0);
            if (argEx.InnerException != null)
                WriteException(badColor, depth + 1, argEx.InnerException);

            return WrapCons;
        }

        public void Dispose()
        {

        }

        public void Flush()
        {

        }
    }
}