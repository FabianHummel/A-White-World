using WhiteWorld.engine.interfaces;

namespace WhiteWorld.engine.ecs;

public abstract class GameScript : IUpdatable {
    public GameObject GameObject { get; set; } = null!;

    public virtual void OnInit() {}
    public virtual void OnTick() {}
    public virtual void OnUpdate() {}
    public virtual void OnGui() {}
}

public class DisallowMultipleInstancesAttribute : Attribute {}

public class DisallowRemovalAttribute : Attribute {}