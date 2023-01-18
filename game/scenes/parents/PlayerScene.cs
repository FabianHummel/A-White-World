using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.scripts;
using WhiteWorld.game.scripts.shared;

namespace WhiteWorld.game.scenes.parents;

public abstract class PlayerScene : Engine.Level {
    protected PlayerScene(
        string level,
        Dictionary<string, string>? soundsToRegister = null,
        Dictionary<string, string>? texturesToRegister = null,
        Dictionary<string, string>? resourcesToRegister = null,
        Dictionary<string, GameObject>? gameObjectsToSpawn = null
    ) : base(level,

        soundsToRegister: new(soundsToRegister ?? new()) {

        },

        texturesToRegister: new(texturesToRegister ?? new()) {
            { "Seaman Walking",         @"assets/images/seaman/seaman-walking.gif" },
            { "Seaman Walking Flipped", @"assets/images/seaman/seaman-walking-flipped.gif" },
            { "Seaman Idle",            @"assets/images/seaman/seaman-idle.gif" },
            { "Seaman Idle Flipped",    @"assets/images/seaman/seaman-idle-flipped.gif" },
        },

        resourcesToRegister: new(resourcesToRegister ?? new()) {

        },

        gameObjectsToSpawn: new(gameObjectsToSpawn ?? new()) {
            { "Seaman", new GameObject()
                .WithTransform(new Transform(0, 0, 0, 16, 16))
                .AddScript(new SpriteRenderer("Seaman Idle"))
                .AddScript(new PlayerController(speed: 40.0f))
                .AddScript(new InteractionController(distance: 10.0f))
                .AddScript(new Inventory())
            },

            { "Flow Camera", new GameObject()
                .AddScript(new FlowCamera("Seaman"))
            },
        }
    ) { }
}