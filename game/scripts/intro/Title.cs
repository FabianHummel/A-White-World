using Raylib_CsLo;
using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.gui;

namespace WhiteWorld.game.scripts.intro;

public class Title : GameScript {

    private string _current = "";
    private readonly string _target;
    private int _startY;

    public Title(string text) {
        _target = text;
    }

    public override void OnInit() {
        _startY = GameObject.Transform.Y;
    }

    public override void OnTick() {
        if (Engine.Frame % 3 == 0 && _current.Length < _target.Length) {
            Engine.PlayRandomSound("Text 1", "Text 2", "Text 3");
            _current = _current.Length == 0 ?
                _target[_target.Length / 2].ToString() :
                _target.Substring(_target.Length / 2 - _current.Length / 2 - 1, _current.Length + 2);
        }
    }

    public override void OnGui(GuiContext ctx) {
        ctx.AlignX = Align.Center;
        ctx.AlignY = Align.Start;
        ctx.DrawText(_current, GameObject.Transform.X, GameObject.Transform.Y, 12, Raylib.DARKGRAY);
    }
    
    public override void OnUpdate() {
        GameObject.Transform.Y = (int) (Math.Sin(Engine.GameTime) * 4) + _startY;
    }
}