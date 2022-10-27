using System.Numerics;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.interfaces;

namespace WhiteWorld.engine.scripts;

public class Collider : GameScript, IViewport {
    private readonly Vector2 _offset;

    public Collider(Vector2 offset, Vector2 size) {
        this._offset = offset;
        this.Size = size;
    }

    public override void OnInit() {
        if (this.Size.X < 9f || this.Size.Y < 9f) {
            Engine.Warn("Collider size too small, player could clip through!",
                "Consider making both sides at least 9 pixels big."
            );
        }
    }

    public override void OnUpdate() {
        Engine.DebugCollider(this);
    }

    public bool InViewport { get; set; }

    public Vector2 Position => GameObject.Transform.Position2D + this._offset;

    public Vector2 Size { get; }

    public void OnViewportEnter() {
        Engine.ActiveColliders.Add(this);
    }

    public void OnViewportExit() {
        Engine.ActiveColliders.Remove(this);
    }
}