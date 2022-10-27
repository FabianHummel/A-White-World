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
        switch (direction) {
            default: return null;
            case PlayerController.Direction.Left: {
                return Raycast(new Vector2(-1f, 0f));
            }
            case PlayerController.Direction.Right: {
                return Raycast(new Vector2(1f, 0f));
            }
            case PlayerController.Direction.Up: {
                return Raycast(new Vector2(0f, -1f));
            }
            case PlayerController.Direction.Down: {
                return Raycast(new Vector2(0f, 1f));
            }
        }
    }

    private Interactable? Raycast(Vector2 direction) {
        var origin = _transform.Position2D + new Vector2(_transform.W / 2.0f, _transform.H / 4.0f * 3.0f);
        return Engine.Raycast(Engine.ActiveInteractables, origin, direction, _distance);
    }
}