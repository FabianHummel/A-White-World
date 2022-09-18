using Raylib_CsLo;
using WhiteWorld.engine;

namespace WhiteWorld.game.scenes; 

public class Seashore : Scene {
    public Seashore() : base(
        new Dictionary<string, string> {
            
        },
        
        new Dictionary<string, string> {
            
        },
        
        new Dictionary<string, (string, int)> {
            
        },
        
        new Dictionary<string, GameObject> {
            
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