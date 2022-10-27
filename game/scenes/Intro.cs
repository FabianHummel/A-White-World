using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.scripts;
using WhiteWorld.game.scripts.intro;

namespace WhiteWorld.game.scenes; 

public class Intro : Engine.Scene {

    public Intro() : base(
        soundsToRegister: new Dictionary<string, string> {
            { "Seashore Waves", @"assets/sounds/music/seashore-waves.wav" },
            { "Ocean", @"assets/sounds/ambient/ocean.wav" }
        },

        animationsToRegister: new Dictionary<string, (string, int)> {
            { "Ocean", ( @"assets/images/seashore-intro-sea.gif", 5 ) },
            { "Seaman", ( @"assets/images/seashore-intro-guy.gif", 5 ) }
        },
        
        gameObjectsToSpawn: new Dictionary<string, GameObject> {
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

    public override void OnInit() {
        Engine.StartAnimation("Seaman");
        foreach (var s in new[] { "Seashore Waves", "Ocean" }) {
            Engine.PlaySound(s, true);
        }
    }

    public override void OnUpdate() {
        Engine.DrawUiAnimation("Ocean", 0, 0, Engine.Align.Center, Engine.Align.Center);
        Engine.DrawUiAnimation("Seaman", 0, 0, Engine.Align.Center, Engine.Align.Center);
    }
}