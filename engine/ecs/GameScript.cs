using WhiteWorld.engine;

namespace WhiteWorld.engine; 

public abstract class GameScript {
    public GameObject GameObject { get; set; } = null!;
    // public Engine.Scene Scene { get; set; } = null!;

    public abstract void OnInit();
    public abstract void OnUpdate();
    public abstract void OnTick();
}

public class DisallowMultipleInstancesAttribute : Attribute {}

public class DisallowRemovalAttribute : Attribute {}