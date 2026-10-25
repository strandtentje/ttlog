using System.Diagnostics;
using TtLog.ModifiedConsole;

namespace TtLog.CoreDemo;

public static class Program
{
    private static readonly Log 
        CoolDemo = Log.Cool("Welcome to TtLog {args}"),
        TechDemo = Log.Tech("This is a demo of tech {randomguid}"),
        Tech2Demo = Log.Tech("We group boring log entries (Tech) and non-boring log entries (anything else)"),
        InfoDemo = Log.Info("And we log Fast; let's benchmark the Cool Log Level"),
        Cool2Demo = Log.Cool("This level is useful for happy messages"),
        WarnDemo = Log.Warn("That blocked the caller code for {µs}"),
        Info2Demo = Log.Info("Notice we don't repeat the same time and date, only posting what changes"),
        ShitDemo = Log.Oops("This level is useful for unlikely things {like}"),
        FailDemo = Log.Fail("Or this one for catastrophes {like}"),
        StopDemo = Log.Stop("Finally, we may send this once to explain why the application stopped {bye}"),
        WrapDemo = Log.Tech("Wrapping is a little prettier in console than just breaking at the end of the console window, let us {demonstrate}"),
        StackTraceDemo = Log.Warn("Or notice how we deal with readability of {exceptions}");
    
    public static void Main(string[] args)
    {
        WrappingConsole.ColumnCount = 100;
        using var _ = Log.Start();
        
        Log.Post(CoolDemo, args.OfType<string>());
        Log.Post(TechDemo, Guid.NewGuid());
        Log.Post(Tech2Demo, Guid.NewGuid());
        Log.Post(InfoDemo, 0);
        Log.Post(Cool2Demo);
        var tx = Stopwatch.GetTimestamp();
        Log.Post(Cool2Demo);
        var ts = Stopwatch.GetTimestamp();
        Log.Post(WarnDemo, (((float)(ts - tx) * 1000000.0) / Stopwatch.Frequency).ToString("0.000"));
        Thread.Sleep(1200);
        Log.Post(Info2Demo);
        Log.Post(ShitDemo, new DivideByZeroException("Hey that's some illegal maths"));
        Log.Post(FailDemo,
            new InvalidOperationException("Casing of pre-famulated amulite surmounted by a malleable logarithmic."));
        try
        {
            try
            {
                try
                {
                    throw new Exception("First Exception");
                }
                catch (Exception ex)
                {
                    throw new Exception("Second Exception", ex);
                }
            }
            catch (Exception ex2)
            {
                throw new Exception("Third Exception", ex2);
            }
        }
        catch (Exception ex3)
        {
            Log.Post(StackTraceDemo, ex3);
        }

        Thread.Sleep(800);
        Log.Post(WrapDemo, $"""
                            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer eget orci rutrum, eleifend turpis quis, lacinia nibh. Donec at ligula vel urna consequat suscipit. Praesent lorem velit, dictum a mauris nec, mollis eleifend dui. Praesent cursus augue sapien, nec condimentum massa pulvinar ac. Morbi at neque ipsum. In vitae dolor ullamcorper, varius tortor nec, faucibus justo. Aliquam non auctor metus, ut dapibus elit. Vestibulum posuere tristique sollicitudin. Quisque semper enim id massa porttitor, ac fringilla lacus commodo. Duis nunc enim, consectetur eu semper et, mattis sit amet erat. Vestibulum sit amet lorem leo. Aliquam quis bibendum lorem. Sed eget vestibulum justo, id varius odio. Vestibulum posuere tempor venenatis. Donec pellentesque nulla velit, eu porta leo lacinia nec. Nulla facilisi. 
                            """);
        Log.Post(StopDemo, "bye");
        Console.ReadLine();
    }
}