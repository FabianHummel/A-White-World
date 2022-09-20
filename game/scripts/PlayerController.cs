using Raylib_CsLo;
using WhiteWorld.engine;
using Transform = WhiteWorld.engine.scripts.Transform;

namespace WhiteWorld.game.scripts; 

public class PlayerController : GameScript {
    
    private readonly float _speed;
    private Transform _transform = null!;
    
    private float _playerX;
    private float _playerY;
    
    public PlayerController(float speed) {
        _speed = speed;
    }
    
    public override void OnInit() {
        _transform = GameObject.Transform;
    }

    public override void OnUpdate() {
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) {
            _playerX -= _speed * Engine.DeltaTime;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) {
            _playerX += _speed * Engine.DeltaTime;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) {
            _playerY -= _speed * Engine.DeltaTime;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) {
            _playerY += _speed * Engine.DeltaTime;
        }
        
        _transform.X = (int) _playerX;
        _transform.Y = (int) _playerY;
    }

    public override void OnTick() {
        
    }
}