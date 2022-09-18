using WhiteWorld.engine;

namespace WhiteWorld.engine; 

public static partial class Engine {
    
    private static readonly Registry<GameObject> GameObjects = new();
    private static readonly Logger GameObjectLogger = GetLogger("Engine/GameObject");
    
    public static GameObject SpawnGameObject(string name, GameObject gameObject, bool persistent = false) {
        GameObjectLogger.Info($"Spawning {name} with {gameObject.GetScriptCount()} scripts attached");
        GameObjects.Add(name, 
            ( gameObject, persistent )
        );
        gameObject.InitScripts();
        return gameObject;
    }
    
    public static GameObject SpawnGameObject(GameObject gameObject, bool persistent = false) {
        return SpawnGameObject($"{GameTime}: {gameObject}", gameObject, persistent);
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
        return GameObjects[name].item;
    }
    
    public static void DumpGameObjects() {
        var query = GameObjects.Select(kvp =>
            $"[GameObject] {kvp.Key}: " +
            $"{kvp.Value.item.GetScriptCount()} scripts attached; " +
            $"persistent: {kvp.Value.persistent}; "
        );
        
        GameObjectLogger.Debug(query.Prepend("Dumping GameObjects:").ToArray());
    }
}