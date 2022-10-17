using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.ecs.scripts;
using WhiteWorld.game.scripts.intro;

namespace WhiteWorld.game.scenes; 

public class Intro : Engine.Scene {

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
            { "Sea", ( @"assets/images/seashore-intro-sea.gif", 5 ) },
            { "Guy", ( @"assets/images/seashore-intro-guy.gif", 5 ) }
        },
        
        new Dictionary<string, string> {
            
        },
        
        new Dictionary<string, GameObject> {
            { "Intro Title", new GameObject()
                .WithTransform(new Transform(0, -30))
                .AddScript(new Title("~ A White World ~"))
                .AddScript(new TitleAnim())
            },

            { "Continue Text", new GameObject()
                .WithTransform(new Transform(0, -10))
                .AddScript(new Continue("Press <Space> to continue..."))
            }
        }
    ) { }

    public override void OnLoad() {
        Engine.PlaySound("Seashore Waves", true);
        Engine.StartAnimation("Guy");
    }

    public override void OnUpdate() {
        Engine.DrawUiAnimation("Sea", 0, 0, Engine.Align.Center, Engine.Align.Center);
        Engine.DrawUiAnimation("Guy", 0, 0, Engine.Align.Center, Engine.Align.Center);
    }

    public override void OnTick() {
        
    }
}