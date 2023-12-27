using System.Numerics;
using Raylib_CsLo;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.interfaces;

namespace WhiteWorld.engine.scripts;

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

[DisallowMultipleInstances]
public class SimpleInteraction : Interactable {
    private readonly string _title;
    private readonly string[] _textPool;

    public SimpleInteraction(string title, params string[] textPool) {
        this._title = title;
        this._textPool = textPool;
    }

    public override void OnInteract(GameObject interactor) {
        var text = _textPool[Raylib.GetRandomValue(0, _textPool.Length - 1)];
        Engine.QueueDialogue(this._title, text);
    }
}