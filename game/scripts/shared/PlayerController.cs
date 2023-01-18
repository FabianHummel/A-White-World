using System.Numerics;
using Raylib_CsLo;
using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.scripts;
using Transform = WhiteWorld.engine.scripts.Transform;

namespace WhiteWorld.game.scripts.shared;

public class PlayerController : GameScript {
    public enum Direction {
        Left, Right, Up, Down
    }

    private readonly float _speed;
    private Transform _transform = null!;
    private SpriteRenderer _renderer = null!;
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
        _renderer = GameObject.GetScript<SpriteRenderer>();
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

        _camera.TargetSmoothTime = _isMoving ? 0.5f : 5.0f;
        _renderer.SpriteName = $"Seaman {(_isMoving ? "Walking" : "Idle")}{(_flipped ? " Flipped" : "")}";
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

    public void DropItem(InventoryItem item) {
        var initialVelocity = Facing switch {
            Direction.Up => new Vector3(0f, -15f, 0),
            Direction.Right => new Vector3(15f, 0f, 0),
            Direction.Down => new Vector3(0f, 15f, 0),
            Direction.Left => new Vector3(-15f, 0f, 0),
            _ => new Vector3(0f, 0f, 8f)
        };

        Engine.SpawnGameObject(new Item(item)
            .WithTransform(new Transform(_transform.X + _transform.W / 2, _transform.Y + _transform.H / 2, _transform.Z + 5, 8, 8))
            .AddScript(new Velocity(initialVelocity, friction: 2f, gravity: 20f))
        );
    }
}