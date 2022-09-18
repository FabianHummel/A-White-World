using Raylib_CsLo;
using WhiteWorld.game.scenes;
using WhiteWorld.utility;
using Transform = WhiteWorld.engine.scripts.Transform;

namespace WhiteWorld.engine.intro; 

public class ContinueText : IGameScript {

    private readonly string _text;
    private Transform _transform = null!;
    private Color _textColor = Raylib.RAYWHITE;

    public ContinueText(string text) {
        _text = text;
    }
    
    public override void OnInit() {
        _transform = GameObject.Transform;
        
        Engine.Wait(() => {
            Engine.Transition(t => {
                _textColor = Util.LerpColor(Raylib.RAYWHITE, Raylib.LIGHTGRAY, t);
            }, 2.0f);
        }, 2.0f);
    }

    public override void OnUpdate() {
        Engine.DrawTextCentered(_text, _transform.X, _transform.Y, 28, _textColor, Engine.Align.Center, Engine.Align.Center);
        
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) {
            Engine.SetScene<Seashore>();
        }
    }

    public override void OnTick() { }
}