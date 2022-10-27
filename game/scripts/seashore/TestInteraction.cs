using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.scripts;

namespace WhiteWorld.game.scripts.seashore;

[DisallowMultipleInstances]
public class TestInteraction : Interactable {
    public override void OnInteract(GameObject interactor) {
        Engine.QueueDialogue("Box", "Hello! Let me introduce myself...");
        Engine.QueueDialogue("Box", "Guess what, I am a box!");
        Engine.QueueDialogue("Box", "But not like any other box...");
        Engine.QueueDialogue("Box", "Who are you?", new[] {
            new Engine.DialogueOption("A lonely\nseamen", () => {
                Engine.QueueDialogue("Box", "We could be friends! Then you would'nt\nbe so lonely anymore!");
            }),
            new Engine.DialogueOption("What is this\nplace?", () => {
                Engine.QueueDialogue("Box", "Oh, this is my home.");
                Engine.QueueDialogue("Box", "I have been here on this spot\nfor several years now.");
                Engine.QueueDialogue("Box", "Sometimes I wish I had legs\nlike everyone else here...");
            })
        });
    }
}