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
        Dictionary<string, (string, int)>? animationsToRegister = null,
        Dictionary<string, string>? resourcesToRegister = null,
        Dictionary<string, GameObject>? gameObjectsToSpawn = null
    ) : base(level,

        soundsToRegister: new Dictionary<string, string>(soundsToRegister ?? new()) {

        },

        texturesToRegister: new Dictionary<string, string>(texturesToRegister ?? new()) {

        },

        animationsToRegister: new Dictionary<string, (string, int)>(animationsToRegister ?? new()) {
            { "Seaman Walking",         ( @"assets/images/seaman/seaman-walking.gif", 4 ) },
            { "Seaman Walking Flipped", ( @"assets/images/seaman/seaman-walking-flipped.gif", 4 ) },
            { "Seaman Idle",            ( @"assets/images/seaman/seaman-idle.gif", 10 ) },
            { "Seaman Idle Flipped",    ( @"assets/images/seaman/seaman-idle-flipped.gif", 10 ) },
        },

        resourcesToRegister: new Dictionary<string, string>(resourcesToRegister ?? new()) {

        },

        gameObjectsToSpawn: new Dictionary<string, GameObject>(gameObjectsToSpawn ?? new()) {
            { "Seaman", new GameObject()
                .WithTransform(new Transform(0, 0, 1, 16, 16))
                .AddScript(new AnimationRenderer("Seaman Idle"))
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