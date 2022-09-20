using System.Numerics;
using WhiteWorld.engine;
using WhiteWorld.engine.scripts;

namespace WhiteWorld.game.scripts;

public class FlowCamera : GameScript {
    
    private readonly string _name;
    private readonly Vector2 _offset;
    private Transform _transform = null!;
    private Vector2 _velocity;
    
    private Engine.Level _level = null!;

    public FlowCamera(string name, Vector2 offset = default) {
        _name = name;
        _offset = offset;
    }

    public override void OnInit() {
        _transform = Engine.GetGameObject(_name).Transform;
        _level = (Engine.Level) Engine.CurrentScene;
    }

    public override void OnUpdate() {
        var currentPosition = new Vector2(_level.CameraX, _level.CameraY);
        var targetPosition = new Vector2(_transform.X * Engine.PixelSize, _transform.Y * Engine.PixelSize) + _offset;
        var pos = SmoothDamp.Calc(
            currentPosition,
            targetPosition,
            ref _velocity,
            0.8f,
            float.PositiveInfinity,
            Engine.DeltaTime);
        
        _level.CameraX = pos.X ;
        _level.CameraY = pos.Y ;
    }

    public override void OnTick() {
        
    }
}