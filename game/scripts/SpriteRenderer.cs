using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using Transform = WhiteWorld.engine.ecs.scripts.Transform;

namespace WhiteWorld.game.scripts; 

public class SpriteRenderer : GameScript {
    
    public string SpriteName { get; set; }

    private Transform _transform = null!;

    public SpriteRenderer(string spriteName) {
        SpriteName = spriteName;
    }

    public override void OnInit() {
        _transform = GameObject.Transform;
    }

    public override void OnUpdate() {
        Engine.DrawSceneImage(SpriteName, _transform.X, _transform.Y + _transform.Z);
    }
}