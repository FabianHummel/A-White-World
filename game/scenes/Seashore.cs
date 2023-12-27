using System.Numerics;
using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.scripts;
using WhiteWorld.game.scenes.parents;
using WhiteWorld.game.scripts.seashore;
using WhiteWorld.game.scripts.shared;

namespace WhiteWorld.game.scenes; 

public class Seashore : PlayerScene {
    public Seashore() : base(
        @"assets/levels/seashore.json",

        soundsToRegister: new() {
            { "Seashore Waves", @"assets/sounds/music/seashore-waves.wav" },
            { "Ocean", @"assets/sounds/ambient/ocean.wav" }
        },

        texturesToRegister: new() {
            { "Sand TRBL 1", @"assets/images/seashore/sand/sand-trbl-1.png" },
            { "Sand TRBL 2", @"assets/images/seashore/sand/sand-trbl-2.png" },
            { "Sand TRBL 3", @"assets/images/seashore/sand/sand-trbl-3.png" },
            { "Sand TRBL 4", @"assets/images/seashore/sand/sand-trbl-4.png" },

//            { "Seashore T 1", @"assets/images/seashore/seashore/seashore-t-1.png" },
//            { "Seashore R 1", @"assets/images/seashore/seashore/seashore-r-1.png" },
            { "Seashore B 1", @"assets/images/seashore/seashore/seashore-b-1.png" },
//            { "Seashore L 1", @"assets/images/seashore/seashore/seashore-l-1.png" },

//            { "Ocean T 1", @"assets/images/seashore/ocean/ocean-t-1.gif" },
//            { "Ocean R 1", @"assets/images/seashore/ocean/ocean-r-1.gif" },
            { "Ocean B 1", @"assets/images/seashore/ocean/ocean-b-1.gif" },
//            { "Ocean L 1", @"assets/images/seashore/ocean/ocean-l-1.gif" },
        },

        gameObjectsToSpawn: new() {
            { "Level Renderer", new GameObject()
                .WithTransform(new Transform(0, 0, -1, 0, 0))
                .AddScript(new LevelRenderer())
            },

            { "Test Collider", new GameObject()
                .WithTransform(new Transform(20, 20, 0, 10, 10))
                .AddScript(new Collider(Vector2.Zero, Vector2.One * 10))
                .AddScript(new TestInteraction())
            },

            { "Test Collider 2", new GameObject()
                .WithTransform(new Transform(45, 25, 0, 20, 10))
                .AddScript(new Collider(Vector2.Zero, new Vector2(20, 10)))
                .AddScript(new SimpleInteraction("Box 2", "Hello, I'm a box!", "Don't mind me..."))
            }
        }
    ) {}

    public override void OnInit() {
        foreach (var s in new[] { "Seashore Waves", "Ocean" }) {
            Engine.PlaySound(s, true);
        }
        Engine.Wait(() => {
            Engine.QueueDialogue("Wind's voices", "Hey, wake up!", () => {
                Console.WriteLine("Continuing Dialogue");
            });
            Engine.QueueDialogue("Wind's voices", "You have been sleeping all day long...", () => {
                Console.WriteLine("Continuing Dialogue");
            });
        }, 1.0f);
    }
}