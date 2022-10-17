using System.Numerics;
using WhiteWorld.engine.ecs.interfaces;

namespace WhiteWorld.engine.ecs.scripts;

public abstract class Interactable : GameScript, IViewport, IInteractable {
    public bool InViewport { get; set; }
    public Vector2 Position => GameObject.Transform.Position2D;
    public Vector2 Size => GameObject.Transform.Size;

    public void OnViewportEnter() {
        Engine.ActiveInteractables.Add(this);
    }

    public void OnViewportExit() {
        Engine.ActiveInteractables.Remove(this);
    }

    public abstract void OnInteract(GameObject interactor);
}