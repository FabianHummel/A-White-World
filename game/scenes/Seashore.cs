using System.Numerics;
using Raylib_CsLo;
using WhiteWorld.engine;
using WhiteWorld.game.scripts;

namespace WhiteWorld.game.scenes; 

public class Seashore : Engine.Level {

    public Seashore() : base(
        @"assets/levels/seashore.json",
        
        new Dictionary<string, string> {
            
        },
        
        new Dictionary<string, string> {
            { "Red Box", @"assets/images/seashore/red-box.png" },
            { "Green Box", @"assets/images/seashore/green-box.png" },
            { "Blue Box", @"assets/images/seashore/blue-box.png" },
            { "Seaman", @"assets/images/seaman.png" },
        },
        
        new Dictionary<string, (string, int)> {
            
        },
        
        new Dictionary<string, string> {
            
        },
        
        new Dictionary<string, GameObject> {
            { "Level Renderer",  new GameObject(scripts: new GameScript[] {
                new LevelRenderer()
            }) },
            
            { "Player", new GameObject(0, 0, 1, scripts: new GameScript[] {
                new SpriteRenderer("Seaman"),
                new PlayerController(50.0f)
            }) },
            
            { "Flow Camera", new GameObject(scripts: new GameScript[] {
                new FlowCamera("Player", -new Vector2(
                    Engine.WindowWidth / 2.0f, Engine.WindowHeight / 2.0f
                ))
            }) },
        }
    ) {}

    public override void OnLoad() {
        Engine.Wait(() => {
            Engine.QueueDialogue("Wind's voices", "Hey, wake up!", () => {
                Console.WriteLine("Continuing Dialogue");
            });
        }, 1.0f);
    }

    public override void OnUpdate() {

    }

    public override void OnTick() {
        
    }
}