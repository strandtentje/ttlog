using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TtLog.LogFormats
{
    public class FormatTemplate
    {
        private static int HandleCounter = 0;

        public readonly int Handle;
        public readonly LogDepartment Department;
        public readonly IEnumerable<FormatComponent> Components;
        public readonly LogLevel Level;

        public FormatTemplate(LogDepartment department, LogLevel level, IEnumerable<FormatComponent> components)
        {
            this.Handle = Interlocked.Increment(ref HandleCounter);
            this.Department = department;
            this.Level = level;
            this.Components = components;
        }
    }
}