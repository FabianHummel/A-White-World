using Raylib_CsLo;
using WhiteWorld.engine;
using WhiteWorld.engine.intro;
using WhiteWorld.utility;

namespace WhiteWorld.game.scenes; 

public unsafe class Intro : Scene {

    public Intro() : base(
        new Dictionary<string, string> {
            { "Text 1", @"assets/sounds/text/text-1.wav" },
            { "Text 2", @"assets/sounds/text/text-2.wav" },
            { "Text 3", @"assets/sounds/text/text-3.wav" },

            { "Seashore Waves", @"assets/sounds/music/seashore-waves.wav" }
        },
        new Dictionary<string, string> {
            
        },
        
        new Dictionary<string, (string, int)> {
            { "Sea", (@"assets/images/seashore-intro-sea.gif", 5) },
            { "Guy", (@"assets/images/seashore-intro-guy.gif", 5) }
        },
        
        new Dictionary<string, GameObject> {
            { "Intro Title", new GameObject(0, -30, scripts: new IGameScript[] {
                new TextAnimation("~ A White World ~"),
                new SineWaveAnimation()
            }) },
            { "Continue Text", new GameObject(0, 50, scripts: new IGameScript[] {
                new ContinueText("Press <Space> to continue...")
            }) }
        }
    ) { }

    public override void OnLoad() {
        Engine.PlaySound("Seashore Waves", true);
        Engine.StartAnimation("Guy");
    }

    public override void OnUpdate() {
        Engine.DrawAnimation("Sea", 0, 0, Engine.Align.Center, Engine.Align.Center);
        Engine.DrawAnimation("Guy", 0, 0, Engine.Align.Center, Engine.Align.Center);
    }

    public override void OnTick() {
        
    }
}