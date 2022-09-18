using System.Numerics;
using Raylib_CsLo;
using WhiteWorld.game.scenes;

namespace WhiteWorld.engine;

public static partial class Engine {
    
    public const int TargetTps = 30;
    public static bool Initialized { get; private set; }

    private static readonly Logger EngineLogger = GetLogger("Engine/Main");
    
    private static event Action OnTick;

    public static void Main() {
        Config();
        Init();
        while (!Raylib.WindowShouldClose()) {
            Update();
        }
        Close();
    }

    private static void Config() {
        EngineLogger.Info("Configuring engine...");
        Raylib.SetConfigFlags(
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            ConfigFlags.FLAG_WINDOW_RESIZABLE | ConfigFlags.FLAG_WINDOW_HIGHDPI
        );
    }

    private static void Init() {
        Raylib.InitWindow(1100, 620, "A White World");
        Raylib.SetTargetFPS(60);
        Raylib.InitAudioDevice();
        LoadFonts();
        LoadShaders();
        InitDialogue();

        SetSceneImmediate<Intro>();
        
        EngineLogger.Info("Engine initialized.");
        Initialized = true;
    }
    
    private static void Update() {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_F1)) {
            DumpGameObjects();
        }
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_F2)) {
            DumpSounds();
        }
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_F3)) {
            DumpTextures();
        }
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_F4)) {
            DumpAnimations();
        }
        
        WindowWidth = Raylib.GetScreenWidth();
        WindowHeight = Raylib.GetScreenHeight();
        
        CanvasWidth = WindowWidth / PixelSize;
        CanvasHeight = WindowHeight / PixelSize;

        // Sort entities by their position in 3D space
        //  Y ↑   ↗ Z
        //    | ╱      
        //    +⎯⎯⎯> X
        var renderSortedQuery =
            from gameObject in GameObjects.Values.Select(pair => pair.item)
            orderby gameObject.Transform.Z, gameObject.Transform.Y
            select gameObject;
        
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Raylib.RAYWHITE);

        Draw(renderSortedQuery.ToList());
        
        Raylib.DrawFPS(WindowWidth - 150, 10);
        Raylib.EndDrawing();
    }

    private static void Draw(List<GameObject> targets) {
        DeltaTime = Raylib.GetFrameTime();
        GameTime += DeltaTime;
        if (Initialized) {
            _frameTimeCounter += DeltaTime;
            
            if (_frameTimeCounter >= 1.0f / TargetTps) {
                _frameTimeCounter = 0;
                Frame++;
                
                OnTick.Invoke();
                
                foreach (var animation in RegisteredAnimations.Values.Select(anim => anim.item)) {
                    animation.UpdateAnimation();
                }

                foreach (var gameObject in targets) {
                    gameObject.TickScripts();
                }
            }
            
            foreach (var gameObject in targets) {
                gameObject.UpdateScripts();
            }
            
            _scene?.OnUpdate();
        }
        
        UpdateDialogue(); // Call this after the scene update so that the
                          // dialogue box can be drawn on top of all scene objects.
        
        UpdateCurtain(); // Call this after the scene update so that the
                         // curtain can be drawn on top of everything else
    }
    
    private static void Close() {
        EngineLogger.Info("Disposing engine...");
        UnloadSounds();
        UnloadFonts();
        UnloadShaders();
        UnloadTextures();
        Raylib.CloseWindow();
        Raylib.CloseAudioDevice();
    }
    
    public static void Debug(params string[] messages) {
        EngineLogger.Debug(messages);
    }
    
    public static void Info(params string[] messages) {
        EngineLogger.Info(messages);
    }
    
    public static void Warn(params string[] messages) {
        EngineLogger.Warn(messages);
    }
    
    public static void Error(params string[] messages) {
        EngineLogger.Error(messages);
    }
}