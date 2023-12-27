namespace WhiteWorld.engine; 

public partial class Engine {
    public static Logger GetLogger(string name) {
        return new Logger(name);
    }

    public static void Debug(params object[] messages) {
        EngineLogger.Debug(messages);
    }

    public static void Info(params object[] messages) {
        EngineLogger.Info(messages);
    }

    public static void Warn(params object[] messages) {
        EngineLogger.Warn(messages);
    }

    public static void Error(params object[] messages) {
        EngineLogger.Error(messages);
    }

    public static void Success(params object[] messages) {
        EngineLogger.Success(messages);
    }
}

public enum LogLevel {
    Debug, Info, Warn, Error, Success
}

public interface ILogger {
    void Log(LogLevel level, params object[] messages);

    void Debug(params object[] messages);
    void Info(params object[] messages);
    void Warn(params object[] messages);
    void Error(params object[] messages);
    void Success(params object[] messages);
}

public class Logger : ILogger {
    private readonly string _name;

    public Logger(string name) {
        _name = name;
    }

    public void Log(LogLevel level, params object[] messages) {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("╭─");
        
        Console.ForegroundColor = level.ToColor();
        Console.Write($"[{level.ToString().ToUpper()}] ");
        
        Console.ResetColor();
        Console.Write($"[{Engine.GameTime:###,00}s | {Engine.Frame}] [{_name}]\n");

        for (var i = 0; i < messages.Length; i++) {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(i == messages.Length - 1 ? "╰─ " : "├─ " );

            Console.ForegroundColor = level.ToColor();
            Console.Write($"{level.ToIcon()} ");
        
            Console.ResetColor();
            Console.WriteLine(messages[i].ToString());
        }
    }
    
    public void Debug(params object[] messages) {
        Log(LogLevel.Debug, messages);
    }
    
    public void Info(params object[] messages) {
        Log(LogLevel.Info, messages);
    }
    
    public void Warn(params object[] messages) {
        Log(LogLevel.Warn, messages);
    }
    
    public void Error(params object[] messages) {
        Log(LogLevel.Error, messages);
    }

    public void Success(params object[] messages) {
        Log(LogLevel.Success, messages);
    }
}

public static class LogLevelExtension {
    public static ConsoleColor ToColor(this LogLevel level) {
        return level switch {
            LogLevel.Debug => ConsoleColor.DarkGray,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Warn => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Success => ConsoleColor.Green,
            _ => ConsoleColor.White
        };
    }
    
    public static string ToIcon(this LogLevel level) {
        return level switch {
            LogLevel.Debug => "\uf188",
            LogLevel.Info => "\uf05a",
            LogLevel.Warn => "\uf071",
            LogLevel.Error => "\uf06a",
            LogLevel.Success => "\uf00c",
            _ => "\uf05a"
        };
    }
}