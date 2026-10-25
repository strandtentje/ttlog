using System.Diagnostics;
using Ziewaar.TtLog;
using Ziewaar.TtLog.ModifiedConsole;

Log.Start();
WrappingConsole.ColumnCount = 80;

var coolDemo = Log.Cool("Welcome to TtLog");
var techDemo = Log.Tech("This is a demo of tech {randomguid}");
var tech2Demo = Log.Tech("We group boring log entries (Tech) and non-boring log entries (anything else)");
var infoDemo = Log.Info("And we log Fast; let's benchmark the Cool Log Level");
var cool2Demo = Log.Cool("This level is useful for happy messages");
var warnDemo = Log.Warn("That blocked the caller code for {µs}");
var info2Demo = Log.Info("Notice we don't repeat the same time and date, only posting what changes");
var shitDemo = Log.Oops("This level is useful for unlikely things {like}");
var failDemo = Log.Fail("Or this one for catastrophes {like}");
var stopDemo = Log.Stop("Finally, we may send this once to explain why the application stopped {bye}");
var wrapDemo =
    Log.Tech(
        "Wrapping is a little prettier in console than just breaking at the end of the console window, let us {demonstrate}");
var stackTraceDemo = Log.Warn("Or notice how we deal with readability of {exceptions}");

Log.Post(coolDemo);
Log.Post(techDemo, Guid.NewGuid());
Log.Post(tech2Demo, Guid.NewGuid());
Log.Post(infoDemo, 0);
var tx = Stopwatch.GetTimestamp();
Log.Post(cool2Demo);
var ts = Stopwatch.GetTimestamp();
Log.Post(warnDemo, (((float)(ts - tx) * 1000000.0) / Stopwatch.Frequency).ToString("0.000"));
Thread.Sleep(1200);
Log.Post(info2Demo);
Log.Post(shitDemo, new DivideByZeroException("Hey that's some illegal maths"));
Log.Post(failDemo,
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
    Log.Post(stackTraceDemo, ex3);
}

Thread.Sleep(800);
Log.Post(wrapDemo, $"""
                    Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer eget orci rutrum, eleifend turpis quis, lacinia nibh. Donec at ligula vel urna consequat suscipit. Praesent lorem velit, dictum a mauris nec, mollis eleifend dui. Praesent cursus augue sapien, nec condimentum massa pulvinar ac. Morbi at neque ipsum. In vitae dolor ullamcorper, varius tortor nec, faucibus justo. Aliquam non auctor metus, ut dapibus elit. Vestibulum posuere tristique sollicitudin. Quisque semper enim id massa porttitor, ac fringilla lacus commodo. Duis nunc enim, consectetur eu semper et, mattis sit amet erat. Vestibulum sit amet lorem leo. Aliquam quis bibendum lorem. Sed eget vestibulum justo, id varius odio. Vestibulum posuere tempor venenatis. Donec pellentesque nulla velit, eu porta leo lacinia nec. Nulla facilisi. 
                    """);
Log.Post(stopDemo, "bye");
Console.ReadLine();