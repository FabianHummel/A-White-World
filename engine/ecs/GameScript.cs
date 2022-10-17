using WhiteWorld.engine.ecs.interfaces;

namespace WhiteWorld.engine.ecs;

public abstract class GameScript : IUpdatable {
    public GameObject GameObject { get; set; } = null!;

    public virtual void OnInit() {}
    public virtual void OnUpdate() {}
    public virtual void OnTick() {}
}

public class DisallowMultipleInstancesAttribute : Attribute {}

public class DisallowRemovalAttribute : Attribute {}