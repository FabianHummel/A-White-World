using Raylib_CsLo;

namespace WhiteWorld.engine; 

public static unsafe partial class Engine {

    private static readonly Logger ShaderLogger = GetLogger("Engine/Shader");

    private static void LoadShaders() {
        FontLogger.Info("Loading shaders...");
        // _curtainShader = Raylib.LoadShader(
        //     "", Raylib.TextFormat("assets/shaders/curtain.fs", 330)
        // );
    }
    
    private static void UnloadShaders() {
        // Raylib.UnloadShader(_curtainShader);
    }
}