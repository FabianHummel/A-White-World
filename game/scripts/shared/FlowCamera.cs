using System.Numerics;
using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.scripts;
using WhiteWorld.utility;

namespace WhiteWorld.game.scripts.shared;

public class FlowCamera : GameScript {

    private readonly string _name;
    private Transform _target = null!;
    private Vector2 _velocity;
    private float _smoothTime;

    private Engine.Level _level = null!;

    public float TargetSmoothTime { private get; set; } = 0.8f;

    public FlowCamera(string name) {
        _name = name;
    }

    public override void OnInit() {
        _target = Engine.GetGameObject(_name).Transform;
        _level = (Engine.Level) Engine.CurrentScene;
    }

    public override void OnUpdate() {
        _smoothTime = MathUtil.Lerp(_smoothTime, TargetSmoothTime, 0.05f);

        var currentPosition = new Vector2(_level.CameraX, _level.CameraY);

        var camOffset = new Vector2(Engine.CanvasWidth / 2.0f, Engine.CanvasHeight / 2.0f);
        var targetOffset = new Vector2(_target.W / 2.0f, _target.H / 2.0f);
        var targetPosition = new Vector2(_target.X, _target.Y) - camOffset + targetOffset;

        var pos = SmoothDamp.Calc(
            currentPosition,
            targetPosition * Engine.PixelSize,
            ref _velocity,
            _smoothTime,
            float.PositiveInfinity,
            Engine.DeltaTime);

        _level.CameraX = pos.X;
        _level.CameraY = pos.Y;
    }
}