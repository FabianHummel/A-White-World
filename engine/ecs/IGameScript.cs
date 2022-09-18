using WhiteWorld.engine;

namespace WhiteWorld.engine; 

public abstract class IGameScript {
    public GameObject GameObject { get; set; } = null!;

    public abstract void OnInit();
    public abstract void OnUpdate();
    public abstract void OnTick();
}

public class DisallowMultipleInstancesAttribute : Attribute {}

public class DisallowRemovalAttribute : Attribute {}