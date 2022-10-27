using System.Numerics;
using WhiteWorld.engine.ecs;

namespace WhiteWorld.engine.scripts;

[DisallowMultipleInstances, DisallowRemoval]
public class Transform : GameScript {
    public int X { get; set; }  //   Y ↑   ↗ Z (depth)
    public int Y { get; set; }  //     | ╱
    public int Z { get; set; }  //     +⎯⎯⎯> X

    public int W { get; set; }
    public int H { get; set; }

    public Vector3 Position3D => new Vector3(X, Y, Z);
    public Vector2 Position2D => new Vector2(X, Y);
    public Vector2 Position3DTo2D => new Vector2(X, Y + Z);
    public Vector2 Size => new Vector2(W, H);

    public Transform(int x = 0, int y = 0, int z = 0, int w = 1, int h = 1) {
        X = x; Y = y; Z = z;
        W = w; H = h;
    }

    public void UpdateTransform(Transform transform) {
        this.X = transform.X;
        this.Y = transform.Y;
        this.Z = transform.Z;
        this.W = transform.W;
        this.H = transform.H;
    }

    public bool InViewport() {
        return Engine.InViewport(Position3DTo2D, Size);
    }

    public override void OnInit() {
        Console.WriteLine("Transform initialized");
    }

    public override void OnUpdate() {
        Engine.DebugTransform(this);
    }
}