using System.Numerics;

namespace WhiteWorld.engine.ecs.interfaces;

public interface IViewport {
    bool InViewport { get; set; }
    Vector2 Position { get; }
    Vector2 Size { get; }

    void OnViewportEnter();
    void OnViewportExit();
}