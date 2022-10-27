using WhiteWorld.engine.ecs;
using WhiteWorld.utility;

namespace WhiteWorld.engine; 

public static partial class Engine {
    
    private static readonly Registry<GameObject> GameObjects = new();
    private static readonly Logger GameObjectLogger = GetLogger("Engine/GameObject");
    
    public static GameObject SpawnGameObject(string name, GameObject gameObject, bool persistent = false) {
        gameObject.Name = name;
        gameObject.Load();
        GameObjectLogger.Info($"Spawning {name} with {gameObject.GetScriptCount()} scripts attached");
        GameObjects.Add(name, ( gameObject, persistent ));
        return gameObject;
    }
    
    public static GameObject SpawnGameObject(GameObject gameObject, bool persistent = false) {
        return SpawnGameObject(gameObject.ToMemoryAddress(), gameObject, persistent);
    }

    private static void TickGameObjects(IReadOnlyList<GameObject> targets) {
        foreach (var gameObject in targets) {
            gameObject.TickScripts();
        }
    }

    private static void UpdateGameObjects(IReadOnlyList<GameObject> targets) {
        foreach (var gameObject in targets) {
            gameObject.UpdateScripts();
        }
    }

    private static void UpdateGui(IReadOnlyList<GameObject> targets) {
        foreach (var gameObject in targets) {
            gameObject.UpdateGui();
        }
    }

    private static void RemoveGameObjects() {
        var query = from gameObject in GameObjects
            where !gameObject.Value.persistent
            select gameObject;

        foreach (var (id, (gameObject, _)) in query) {
            GameObjects.Remove(id);
            gameObject.Unload();
        }
    }

    public static GameObject GetGameObject(string name) {
        if (GameObjects.TryGetValue(name, out var gameObject)) {
            return gameObject.item;
        }
        GameObjectLogger.Error($"Could not find game object {name}");
        return default!;
    }
    
    public static void DumpGameObjects() {
        var query = GameObjects.Select(kvp =>
            $"[GameObject] {kvp.Key}: " +
            $"{kvp.Value.item.GetScriptCount()} scripts attached; " +
            $"persistent: {kvp.Value.persistent}; "
        );
        
        GameObjectLogger.Debug(query.Prepend("Dumping GameObjects:"));
    }
}