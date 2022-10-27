using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using Transform = WhiteWorld.engine.scripts.Transform;

namespace WhiteWorld.game.scripts.shared;

public class AnimationRenderer : GameScript {

    public string AnimationName { get; set; }

    private Transform _transform = null!;

    public AnimationRenderer(string animationName) {
        AnimationName = animationName;
    }

    public override void OnInit() {
        _transform = GameObject.Transform;
        Engine.StartAnimation(AnimationName);
    }

    public override void OnUpdate() {
        Engine.DrawSceneAnimation(AnimationName, _transform.X, _transform.Y);
    }
}