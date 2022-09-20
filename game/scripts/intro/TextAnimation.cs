using Raylib_CsLo;

namespace WhiteWorld.engine; 

public class TextAnimation : GameScript {

    private string _current = "";
    private readonly string _target;

    public TextAnimation(string text) {
        _target = text;
    }
    
    public override void OnInit() {
        
    }

    public override void OnUpdate() {
        Engine.DrawUiTextCentered(_current, GameObject.Transform.X, GameObject.Transform.Y, 80, Raylib.DARKGRAY, Engine.Align.Center, Engine.Align.Center);
    }

    public override void OnTick() {
        if (Engine.Frame % 3 == 0 && _current.Length < _target.Length) {
            Engine.PlaySound("Text 1", "Text 2", "Text 3");
            _current = _current.Length == 0 ?
                _target[_target.Length / 2].ToString() :
                _target.Substring(_target.Length / 2 - _current.Length / 2 - 1, _current.Length + 2);
        }
    }
}