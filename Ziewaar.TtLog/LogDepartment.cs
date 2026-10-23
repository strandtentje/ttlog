namespace Ejije.Logging
{
    public class LogDepartment
    {
        public static readonly LogDepartment Default = new LogDepartment() { IsEnabled = true, };
        public bool IsEnabled;
    }
}