using Raylib_CsLo;

namespace WhiteWorld.engine; 

public static partial class Engine {

    private static readonly Registry<Shader> RegisteredShaders = new();
    private static readonly Logger ShaderLogger = GetLogger("Engine/Shader");

    private static void LoadShaders() {
        ShaderLogger.Info("Loading shaders...");
        var blurShader = Raylib.LoadShader(null, Raylib.TextFormat("assets/shaders/blur.fs", 330));
        RegisteredShaders.Add("Blur", (blurShader, true));
    }

    public static Shader GetShader(string name) {
        if (RegisteredShaders.TryGetValue(name, out var shader)) {
            return shader.item;
        }
        ShaderLogger.Error($"Shader {name} not found");
        return default;
    }
    
    private static void UnloadShaders() {
        var query = from shader in RegisteredShaders
            where !shader.Value.persistent
            select shader;

        foreach (var (id, (shader, _)) in query) {
            RegisteredShaders.Remove(id);
            Raylib.UnloadShader(shader);
        }
    }
}