namespace WhiteWorld.engine; 

public static partial class Engine {
    public static float DeltaTime { get; private set; }
    public static float GameTime { get; private set; }
    public static int Frame { get; private set; }

    private static float _tickTimeCounter;
    private static float _secTimeCounter;
    
    private static readonly Logger TimeLogger = GetLogger("Engine/Time");

    public static Task Transition(Action<float> action, float duration) {
        var t = 0.0f;
        var start = GameTime;
        return Task.Run(() => {
            while (t < duration) {
                action(t / duration);
                t = GameTime - start;
            }
        });
    }
    
    public static Task Wait(Action action, float duration) {
        return Task.Delay((int) (duration * 1000.0f)).ContinueWith(_ => {
            action();
        });
    }
}