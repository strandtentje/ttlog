using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Ziewaar.TtLog.LogFormats;
using Ziewaar.TtLog.Utilities;

namespace Ziewaar.TtLog.Writers
{
    public class HtmlLogWriter : ILogWriter
    {
        private readonly StreamWriter HtmlWriter;

        private static readonly string
            LogFileHead = File.ReadAllText(Path.Combine(SpecialPaths.AssemblyDirectory, "LogFileHead.html")),
            LogFileFoot = File.ReadAllText(Path.Combine(SpecialPaths.AssemblyDirectory, "LogFileFoot.html")),

            LogLineTemplateStart = File.ReadAllText(Path.Combine(SpecialPaths.AssemblyDirectory, "LogLineTemplateStart.html")),
            LogLineTemplateEnd = File.ReadAllText(Path.Combine(SpecialPaths.AssemblyDirectory, "LogLineTemplateEnd.html")),
            LogLineLiteral = File.ReadAllText(Path.Combine(SpecialPaths.AssemblyDirectory, "LogLineLiteral.html")),
            LogLineVariable = File.ReadAllText(Path.Combine(SpecialPaths.AssemblyDirectory, "LogLineVariable.html"));

        private readonly HashSet<int> KnownTemplateHandles = new HashSet<int>();

        public HtmlLogWriter(string targetFile)
        {
            var logStyleSource = Path.Combine(SpecialPaths.AssemblyDirectory, "style.css");
            var logStyle = Path.Combine(SpecialPaths.LogDirectory, "style.css");
            if (!File.Exists(logStyle))
                File.Copy(logStyleSource, logStyle);
            this.HtmlWriter = new StreamWriter(targetFile);
            this.HtmlWriter.AutoFlush = false;
            var sanitizedFilename = HttpUtility.HtmlEncode(Path.GetFileName(targetFile));
            var withReplacedTitle = LogFileHead.Replace("FILENAME", sanitizedFilename);
            this.HtmlWriter.Write(withReplacedTitle);
        }

        private readonly StringBuilder LiteralBuildup = new StringBuilder();
        public void Write(DateTime adjustedTimestamp, LogEvent logEvent)
        {
            lock (this.HtmlWriter)
            {
                var template = logEvent.Template;
                if (KnownTemplateHandles.Add(logEvent.Template.Handle))
                {
                    var levelName = Enum.GetName(typeof(LogLevel), logEvent.Template.Level);
                    this.HtmlWriter.Write(
                        LogLineTemplateStart,
                        HttpUtility.HtmlAttributeEncode(levelName),
                        logEvent.Template.Handle,
                        HttpUtility.HtmlEncode(levelName));

                    int argCounter = 4;
                    foreach (var component in template.Components)
                    {
                        if (component.IsPlaceholder)
                        {
                            if (LiteralBuildup.Length > 0)
                            {
                                this.HtmlWriter.WriteLine(LogLineLiteral, LiteralBuildup.ToString());
                                LiteralBuildup.Remove(0, LiteralBuildup.Length);
                            }

                            this.HtmlWriter.WriteLine(LogLineVariable, HttpUtility.HtmlEncode(component.Literal), argCounter);
                            argCounter++;
                        }
                        else
                        {
                            LiteralBuildup.Append(HttpUtility.HtmlEncode(component.Literal));
                        }
                    }
                    if (LiteralBuildup.Length > 0)
                    {
                        this.HtmlWriter.WriteLine(LogLineLiteral, LiteralBuildup.ToString());
                        LiteralBuildup.Remove(0, LiteralBuildup.Length);
                    }
                    this.HtmlWriter.WriteLine(LogLineTemplateEnd);
                }



                this.HtmlWriter.Write(
                    "<pre id=\"line{0}\">[\"", logEvent.Handle.ToString("0000000000000000"));
                this.HtmlWriter.Write(logEvent.Template.Handle.ToString());
                this.HtmlWriter.Write("\", \"");
                this.HtmlWriter.Write(adjustedTimestamp.ToString("yyMMdd"));
                this.HtmlWriter.Write("\", \"");
                this.HtmlWriter.Write(adjustedTimestamp.ToString("HHmmss"));
                this.HtmlWriter.Write("\", \"");
                this.HtmlWriter.Write(Enum.GetName(typeof(LogLevel), logEvent.Template.Level));

                foreach (var item in logEvent.Params)
                {
                    var ser = "`null`";
                    if (item != null)
                    {
                        try
                        {
                            ser = item.ToString();
                        }
                        catch (Exception ex)
                        {
                            ser = ex.ToString();
                        }
                    }
                    this.HtmlWriter.Write("\", \"");
                    this.HtmlWriter.Write(HttpUtility.JavaScriptStringEncode(HttpUtility.HtmlEncode(ser)));
                }
                this.HtmlWriter.WriteLine("\"]</pre>");

                if (logEvent.Template.Level == LogLevel.Stop)
                    this.HtmlWriter.Flush();
            }
        }

        public void Flush()
        {
            this.HtmlWriter.Flush();
        }

        public void Dispose()
        {
            this.HtmlWriter.Write(LogFileFoot);
            this.HtmlWriter.Flush();
            this.HtmlWriter.Close();
        }
    }
}