ttlog logger for all sorts of .net versions
===========================================

Hi I'm an avid user of Serilog, I think it's really nice and if you're doing anything serious you should probably use that, and not this.

Sometimes I'm working on software that
 - Doesn't have or need DependencyInjection
 - Doesn't need anything other than a console and file sink
 - I write on my Compaq NC6320 with Windows XP and VS2008
 - Is not surrounded by infrastructure to clean up after it
 - Has its performance impacted by passing log strings during runtime
 - Logs so much it makes my eyes hurt to read the output text.

In any subset of those conditions, I'd probably still use Serilog, but I've ran into the occasion twice now where all of those conditions reign true so I wrote this logger on aforementioned Compaq NC6320 in .Net 3.5 and ported it to .Net Standard 2.0 (it still builds to .Net 3.5 with minimal effort, though the language version has crept up to 9.0) 

It does a couple of neat things I could explain but let's look at a screenshot of what it does in terminal.

<img width="1039" height="1281" alt="afbeelding" src="https://github.com/user-attachments/assets/208a6a7b-924c-47c4-9421-adbfa1f06554" />

As for the log files, rather than outputting as `text/plain`, the logger flushes `text/json` to a hidden tag in a `text/html` which contains some `text/javascript` to unpack it into some pretty HTML. Reading those files still isn't as pleasant on the eyes as I would like, but I feel it's an improvement in most situations. The Exceptions could still be nicer.

<img width="1079" height="1107" alt="afbeelding" src="https://github.com/user-attachments/assets/196bab92-658a-4356-ba7a-b1218267014c" />

Lets maybe look at the most important bit, which is "how bad does it impact my program listing"

```csharp
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
```

The main difference is certainly that log formats are defined before they are actually reported - this helps for:
 - The performance aspect; all possible log templates are parsed and pre-rendered as far as possible, once.
 - A subjective reduction in cognitive strain; I feel in C# it's hard to make log strings that are both descriptive and non-obstructive to the program doing the work. Having the opportunity to take those strings out of the calling context, and use brief identifiers instead, I personally like that but you don't have to!

Another difference I like is that for apps that have a complex or non-existing relationship with (Conventional) Dependency Injection, or any app really, this logger only really has `using var _ = Log.Start()` as boilerplate. If that trade-off with more boiler plate for the actual log lines is worth it, is up to you. It automatically figures out where to stick the log files using the `SpecialPaths` class in `Utilities` - it's going to be your OS' equivalent of `%APPDATA%` with a subdirectory. If you're particular about that subdirectory, assign `SpecialPaths.AppName` before starting the logger. 

Misc information you may care about
-----------------------------------

 - No LLM's were harmed in the _making_ of this library. It's source sits on Github now, so LLMs may get trained by this or make reference. That's allright, I guess? I've taken a step back from assisted programming myself. I feel more at ease when working at only the pace the meat-computer upon my shoulders can bear
 - The name is a reference to a television system used in certain Western European countries called Teletext; we used to use this for checking the weather, traffic and news. The stuff this logger outputs to console reminded a coworker of Teletext, so it stuck instead of the keymash I used to describe it, previously.
 - I will not be making a Nuget package. I cannot bear the responsibility of maintenance right now. Feel free to just rawdog chuck this code into your project, or `git submodule` it. That way we can unambiguously agree that if you're using it you'll be responsible for its maintenance in your app. If you at some point find a Nuget bearing this name, that's not mine. It's probably fine, but tread with care. If you're so inclined, feel free to fork and make a Nuget package yourself.
 - This library is storing log lines and in particular their args into `html` and `json`. Which you can then open. In your browser. Which will run Javascript. I think my escaping is pretty skookum? But let's be real this approach is probably a CVE-machine.
