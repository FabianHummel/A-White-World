using System.Numerics;
using Raylib_CsLo;
using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using Transform = WhiteWorld.engine.scripts.Transform;

namespace WhiteWorld.game.scripts.shared;

public class PlayerController : GameScript {
    public enum Direction {
        Left, Right, Up, Down
    }

    private readonly float _speed;
    private Transform _transform = null!;
    private AnimationRenderer _animationRenderer = null!;
    private FlowCamera _camera = null!;

    private Vector2 _fposition;

    private bool _isMoving;
    private bool _flipped;

    public Direction Facing { get; private set; }

    public PlayerController(float speed) {
        _speed = speed;
    }

    public override void OnInit() {
        _transform = GameObject.Transform;
        _animationRenderer = GameObject.GetScript<AnimationRenderer>();
        _camera = Engine.GetGameObject("Flow Camera").GetScript<FlowCamera>();
    }

    public override void OnUpdate() {

        var dx = 0;
        var dy = 0;
        if (!Engine.DialogueOpen) {
            if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) {
                Facing = Direction.Left;
                _flipped = true;
                if (CanWalk(Direction.Left)) {
                    _fposition.X -= _speed * Engine.DeltaTime;
                    dx -= 1;
                }
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) {
                Facing = Direction.Right;
                _flipped = false;
                if (CanWalk(Direction.Right)) {
                    _fposition.X += _speed * Engine.DeltaTime;
                    dx += 1;
                }
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) {
                Facing = Direction.Up;
                if (CanWalk(Direction.Up)) {
                    _fposition.Y -= _speed * Engine.DeltaTime;
                    dy -= 1;
                }
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) {
                Facing = Direction.Down;
                if (CanWalk(Direction.Down)) {
                    _fposition.Y += _speed * Engine.DeltaTime;
                    dy += 1;
                }
            }
        }

        _isMoving = dx != 0 || dy != 0;
        if (_isMoving) {
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
        }

        _camera.TargetSmoothTime = _isMoving ? 0.0f : 4.0f;
        _animationRenderer.AnimationName =
            $"Seaman {(_isMoving ? "Walking" : "Idle")}{(_flipped ? " Flipped" : "")}";
    }

    private bool CanWalk(Direction direction) {
        switch (direction) {
            default: return true;
            case Direction.Left: {
                var top = Raycast(new Vector2(0f, -8), new Vector2(-1f, 0f), 4);
                var bottom = Raycast(new Vector2(0f, 0f), new Vector2(-1f, 0f), 4);
                return !top && !bottom;
            }
            case Direction.Right: {
                var top = Raycast(new Vector2(0f, -8), new Vector2(1f, 0f), 4);
                var bottom = Raycast(new Vector2(0f, 0f), new Vector2(1f, 0f), 4);
                return !top && !bottom;
            }
            case Direction.Up: {
                var left = Raycast(new Vector2(-4f, -4f), new Vector2(0f, -1f), 4);
                var right = Raycast(new Vector2(4f, -4f), new Vector2(0f, -1f), 4);
                return !left && !right;
            }
            case Direction.Down: {
                var left = Raycast(new Vector2(-4f, -4f), new Vector2(0f, 1f), 4);
                var right = Raycast(new Vector2(4f, -4f), new Vector2(0f, 1f), 4);
                return !left && !right;
            }
        }
    }

    private bool Raycast(Vector2 offset, Vector2 direction, float distance) {
        var origin = _transform.Position2D + new Vector2(_transform.W / 2.0f, _transform.H) + offset;
        return Engine.Raycast(Engine.ActiveColliders, origin, direction, distance) != null;
    }
}