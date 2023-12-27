using System.Numerics;

namespace WhiteWorld.engine.interfaces;

public interface IViewport {
    bool InViewport { get; set; }
    Vector2 Position { get; }
    Vector2 Size { get; }

    void OnViewportEnter();
    void OnViewportExit();

    public static void Update(IViewport instance) {
        if (!instance.InViewport && Engine.InViewport(instance.Position, instance.Size)) {
            instance.InViewport = true;
            instance.OnViewportEnter();
        } else if (instance.InViewport && !Engine.InViewport(instance.Position, instance.Size)) {
            instance.InViewport = false;
            instance.OnViewportExit();
        }
    }
}