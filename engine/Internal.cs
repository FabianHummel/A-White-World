namespace WhiteWorld.engine; 

public static partial class Engine {
    public class Registry<T> : Dictionary<string, (T item, bool persistent)> { }
}