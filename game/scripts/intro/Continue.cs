using Raylib_CsLo;
using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.game.scenes;
using WhiteWorld.utility;
using Transform = WhiteWorld.engine.scripts.Transform;

namespace WhiteWorld.game.scripts.intro;

public class Continue : GameScript {

    private readonly string _text;
    private Transform _transform = null!;
    private Color _textColor = Raylib.RAYWHITE;

    public Continue(string text) {
        _text = text;
    }

    public override void OnInit() {
        _transform = GameObject.Transform;

        Engine.Wait(() => {
            Engine.Transition(t => {
                _textColor = MathUtil.LerpColor(Raylib.RAYWHITE, Raylib.LIGHTGRAY, t);
            }, 2.0f);
        }, 2.0f);
    }

    public override void OnUpdate() {
        Engine.DrawUiTextCentered(_text, _transform.X, _transform.Y, 5, 1.5f, _textColor, Engine.Align.Center, Engine.Align.End);

        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) {
            Engine.SetScene<Seashore>();
        }
    }
}