namespace WhiteWorld.engine.ecs.interfaces;

public interface IUpdatable {
    virtual void OnInit() {}
    virtual void OnUpdate() {}
    virtual void OnTick() {}
}