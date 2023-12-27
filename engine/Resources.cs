using Raylib_CsLo;

namespace WhiteWorld.engine; 

public static partial class Engine {
    private static readonly Registry<string> RegisteredResources = new();
    private static readonly Logger ResourcesLogger = GetLogger("Engine/Resources");

    public static string LoadResource(string name, string resource, bool persistent = false) {
        SoundLogger.Info($"Loading resource {name} from {resource}");
        var instance = Raylib.LoadFileText(resource);
        RegisteredResources.Add(name,
            ( instance, persistent ) 
        );
        return instance;
    }
    
    private static void UnloadResources() {
        var query = from resources in RegisteredResources
            where !resources.Value.persistent
            select resources;

        foreach (var (id, (resource, _)) in query) {
            RegisteredResources.Remove(id);
            Raylib.UnloadFileText(resource);
        }
    }
    
    public static void DumpResources() {
        var query = RegisteredResources.Select(kvp =>
            $"[Resource] {kvp.Key}: " +
            $"persistent: {kvp.Value.persistent}; "
        );
        
        SoundLogger.Debug(query.Prepend("Dumping Resources:").ToArray());
    }
    
    public static string GetResourceContent(string name) {
        if (RegisteredResources.TryGetValue(name, out var resource)) {
            return resource.item;
        }
        ResourcesLogger.Error($"Resource {name} not found");
        return null!;
    }
}