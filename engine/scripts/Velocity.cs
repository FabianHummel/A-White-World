using System.Numerics;
using WhiteWorld.engine.ecs;
using WhiteWorld.utility;

namespace WhiteWorld.engine.scripts;

public class Velocity : GameScript {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public float Friction { get; set; } = 1f;

    public Vector3 AsVec3 => new Vector3(X, Y, Z);

    public Velocity() {
        this.X = 0f;
        this.Y = 0f;
        this.Z = 0f;
    }

    public Velocity(Vector3 initialVelocity, float friction = 1f) {
        this.X = initialVelocity.X;
        this.Y = initialVelocity.Y;
        this.Z = initialVelocity.Z;
        this.Friction = friction;
    }

    public static Vector3 operator +(Velocity a) => a.AsVec3;
    public static Vector3 operator -(Velocity a) => -a.AsVec3;

    private Transform _transform = null!;
    private Vector3 _fposition;

    public override void OnInit() {
        _transform = GameObject.Transform;
    }

    public override void OnUpdate() {
        _fposition += AsVec3 * Engine.DeltaTime;

        {
            var floored = (int) Math.Floor(_fposition.X);
            _transform.X += floored;
            _fposition.X -= floored;
        }
        {
            var floored = (int) Math.Floor(_fposition.Y);
            _transform.Y += floored;
            _fposition.Y -= floored;
        }
        {
            var floored = (int) Math.Floor(_fposition.Z);
            _transform.Z += floored;
            _fposition.Z -= floored;
        }

        this.X = MathUtil.Lerp(this.X, 0.0f, this.Friction * Engine.DeltaTime);
        this.Y = MathUtil.Lerp(this.Y, 0.0f, this.Friction * Engine.DeltaTime);
        this.Z = MathUtil.Lerp(this.Z, 0.0f, this.Friction * Engine.DeltaTime);

        Engine.Debug(_fposition);
    }
}