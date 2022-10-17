using System.Numerics;
using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.ecs.scripts;
using WhiteWorld.game.scripts;
using WhiteWorld.game.scripts.seashore;

namespace WhiteWorld.game.scenes; 

public class Seashore : Engine.Level {

    public Seashore() : base(
        @"assets/levels/seashore.json",

        new Dictionary<string, string> {
            { "Seashore Waves", @"assets/sounds/music/seashore-waves.wav" },
            { "Ocean", @"assets/sounds/ambient/ocean.wav" }
        },

        new Dictionary<string, string> {
            { "Sand TRBL 1", @"assets/images/seashore/sand/sand-trbl-1.png" },
            { "Sand TRBL 2", @"assets/images/seashore/sand/sand-trbl-2.png" },
            { "Sand TRBL 3", @"assets/images/seashore/sand/sand-trbl-3.png" },
            { "Sand TRBL 4", @"assets/images/seashore/sand/sand-trbl-4.png" }
        },

        new Dictionary<string, (string, int)> {
            { "Seaman Walking",         ( @"assets/images/seaman/seaman-walking.gif", 4 ) },
            { "Seaman Walking Flipped", ( @"assets/images/seaman/seaman-walking-flipped.gif", 4 ) },
            { "Seaman Idle",            ( @"assets/images/seaman/seaman-idle.gif", 10 ) },
            { "Seaman Idle Flipped",    ( @"assets/images/seaman/seaman-idle-flipped.gif", 10 ) },
        },

        new Dictionary<string, string> {

        },

        new Dictionary<string, GameObject> {
            { "Level Renderer",  new GameObject()
                .AddScript(new LevelRenderer())
            },

            { "Seaman", new GameObject()
                .WithTransform(new Transform(0, 0, 1, 16, 16))
                .AddScript(new AnimationRenderer("Seaman Idle"))
                .AddScript(new PlayerController(speed: 40.0f))
                .AddScript(new InteractionController(distance: 10.0f))
            },

            { "Flow Camera", new GameObject()
                .AddScript(new FlowCamera("Seaman"))
            },
            
            { "Test Collider", new GameObject()
                .WithTransform(new Transform(20, 20, 1, 10, 10))
                .AddScript(new TestInteraction())
                .AddScript(new Collider(Vector2.Zero, Vector2.One * 10))
            }
        }
    ) {}

    public override void OnLoad() {
        foreach (var s in new[] { "Seashore Waves", "Ocean" }) {
            Engine.PlaySound(s, true);
        }

        Engine.Wait(() => {
            Engine.QueueDialogue("Wind's voices", "Hey, wake up!", () => {
                Console.WriteLine("Continuing Dialogue");
            });
            Engine.QueueDialogue("Wind's voices", "You have been sleeping\nall day long...", () => {
                Console.WriteLine("Continuing Dialogue");
            });
        }, 1.0f);
    }

    public override void OnUpdate() {

    }

    public override void OnTick() {
        
    }
}