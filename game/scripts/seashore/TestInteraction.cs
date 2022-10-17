using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.ecs.scripts;

namespace WhiteWorld.game.scripts.seashore;

[DisallowMultipleInstances]
public class TestInteraction : Interactable {
    public override void OnInteract(GameObject interactor) {
        Engine.QueueDialogue("Box", "Hello! Let me introduce myself...");
        Engine.QueueDialogue("Box", "Guess what, I am a box!");
        Engine.QueueDialogue("Box", "But not like any other box...", () => {
            Console.WriteLine("Closing Dialogue");
            // start a quest...
        });
    }
}