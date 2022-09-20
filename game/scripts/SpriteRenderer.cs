using Raylib_CsLo;
using WhiteWorld.engine;
using Transform = WhiteWorld.engine.scripts.Transform;

namespace WhiteWorld.game.scripts; 

public class SpriteRenderer : GameScript {
    
    private readonly string _spriteName;
    private Transform _transform = null!;

    public SpriteRenderer(string spriteName) {
        _spriteName = spriteName;
    }

    public override void OnInit() {
        _transform = GameObject.Transform;
    }

    public override void OnUpdate() {
        Engine.DrawSceneImage(_spriteName, _transform.X, _transform.Y);
    }

    public override void OnTick() {
        
    }
}