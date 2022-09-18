namespace WhiteWorld.engine.intro; 

public class SineWaveAnimation : IGameScript {
    private int _startY;

    public override void OnInit() {
        _startY = GameObject.Transform.Y;
    }
        
    public override void OnUpdate() {
        GameObject.Transform.Y = (int) (Math.Sin(Engine.GameTime) * 4) + _startY;
    }

    public override void OnTick() { }
}