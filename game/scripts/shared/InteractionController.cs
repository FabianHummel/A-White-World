using System.Numerics;
using Raylib_CsLo;
using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.scripts;
using Transform = WhiteWorld.engine.scripts.Transform;

namespace WhiteWorld.game.scripts.shared;

public class InteractionController : GameScript {
    private Transform _transform = null!;
    private PlayerController _player = null!;
    private readonly float _distance;

    public InteractionController(float distance) {
        this._distance = distance;
    }

    public override void OnInit() {
        _transform = GameObject.Transform;
        _player = GameObject.GetScript<PlayerController>();
    }

    public override void OnUpdate() {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_E) && !Engine.DialogueOpen) {
            Interact(_player.Facing)?.OnInteract(GameObject);
        }
    }

    private Interactable? Interact(PlayerController.Direction direction) {
        return direction switch {
            PlayerController.Direction.Left => Raycast(new Vector2(-1f, 0f)),
            PlayerController.Direction.Right => Raycast(new Vector2(1f, 0f)),
            PlayerController.Direction.Up => Raycast(new Vector2(0f, -1f)),
            PlayerController.Direction.Down => Raycast(new Vector2(0f, 1f)),
            _ => null,
        };
    }

    private Interactable? Raycast(Vector2 direction) {
        var origin = _transform.Position2D + new Vector2(_transform.W / 2.0f, _transform.H / 4.0f * 3.0f);
        return Engine.Raycast(Engine.ActiveInteractables, origin, direction, _distance);
    }
}