namespace WhiteWorld.engine; 

public partial class Engine {
    public static Logger GetLogger(string name) {
        return new Logger(name);
    }
}

public class Logger {
    public enum LogLevel {
        Debug, Info, Warn, Error
    }

    private readonly string _name;

    public Logger(string name) {
        _name = name;
    }

    private void Log(LogLevel level, params string[] messages) {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("╭─");
        
        Console.ForegroundColor = level.ToColor();
        Console.Write($"[{level.ToString().ToUpper()}] ");
        
        Console.ResetColor();
        Console.Write($"[{Engine.GameTime:00,0##}s | {Engine.Frame}] [{_name}]\n");

        for (var i = 0; i < messages.Length; i++) {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(i == messages.Length - 1 ? 
                "╰─➔ " : "├─➔ "
            );

            Console.ForegroundColor = level.ToColor();
            Console.Write($"{level.ToIcon()} ");
        
            Console.ResetColor();
            Console.WriteLine(messages[i]);
        }
    }
    
    public void Debug(params string[] messages) {
        Log(LogLevel.Debug, messages);
    }
    
    public void Info(params string[] messages) {
        Log(LogLevel.Info, messages);
    }
    
    public void Warn(params string[] messages) {
        Log(LogLevel.Warn, messages);
    }
    
    public void Error(params string[] messages) {
        Log(LogLevel.Error, messages);
    }
}

public static class LogLevelExtension {
    public static ConsoleColor ToColor(this Logger.LogLevel level) {
        return level switch {
            Logger.LogLevel.Debug => ConsoleColor.DarkGray,
            Logger.LogLevel.Info => ConsoleColor.White,
            Logger.LogLevel.Warn => ConsoleColor.Yellow,
            Logger.LogLevel.Error => ConsoleColor.Red,
            _ => ConsoleColor.White
        };
    }
    
    public static string ToIcon(this Logger.LogLevel level) {
        return level switch {
            Logger.LogLevel.Debug => "\uf188",
            Logger.LogLevel.Info => "\uf05a",
            Logger.LogLevel.Warn => "\uf071",
            Logger.LogLevel.Error => "\uf06a",
            _ => "\uf05a"
        };
    }
}