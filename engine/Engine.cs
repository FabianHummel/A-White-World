using System.Numerics;
using Raylib_CsLo;
using WhiteWorld.engine.ecs;
using WhiteWorld.game.scenes;

namespace WhiteWorld.engine;

public static partial class Engine {
    
    public const int TargetTps = 30;
    public static bool Initialized { get; private set; }

    public static RenderTexture GameTexture { get; private set; }

    private static readonly Logger EngineLogger = GetLogger("Engine/Main");

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
            // ConfigFlags.FLAG_WINDOW_HIGHDPI |
            ConfigFlags.FLAG_WINDOW_RESIZABLE
        );
    }

    private static void Init() {
        Raylib.InitWindow(1100, 620, "A White World");
        Raylib.SetTargetFPS(60);
        Raylib.InitAudioDevice();
        ReloadRenderTexture();
        LoadFonts();
        LoadShaders();
        LoadItems();
        LoadSounds();
        InitDialogue();

        SetSceneImmediate<Intro>();
        

        EngineLogger.Info("Engine initialized.");
        Initialized = true;
    }
    
    private static void Update() {
        WindowWidth = Raylib.GetScreenWidth();
        WindowHeight = Raylib.GetScreenHeight();

        PixelSize = (int) (Math.Min(WindowWidth / 600f, WindowHeight / 320f) * 2f) + 3;

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

        // Sort entities by their position in 3D space
        //  Y ↑   ↗ Z
        //    | ╱      
        //    +⎯⎯⎯> X
        var renderSortedQuery =
            from gameObject in GameObjects.Values.Select(pair => pair.item)
            orderby gameObject.Transform.Z, gameObject.Transform.Y
            select gameObject;
        
        var renderSortedList = renderSortedQuery.ToList();

        DeltaTime = Raylib.GetFrameTime();
        GameTime += DeltaTime;

        _tickTimeCounter += DeltaTime;
        _secTimeCounter += DeltaTime;

        if (_secTimeCounter >= 1.0f) {
            _secTimeCounter = 0;
            ReloadRenderTexture();
        }

        Raylib.BeginTextureMode(GameTexture);
        {
            Raylib.ClearBackground(Raylib.RAYWHITE);
            DrawScene(renderSortedList);
        }
        Raylib.EndTextureMode();

        Raylib.BeginDrawing();
        {
            // Raylib.ClearBackground(Raylib.BLACK);
            Raylib.BeginShaderMode(GetShader("Blur"));
            {
                Raylib.DrawTextureRec(
                    GameTexture.texture,
                    new Rectangle(0, 0, GameTexture.texture.width, -GameTexture.texture.height),
                    Vector2.Zero, Raylib.WHITE);
            }
            Raylib.EndShaderMode();
            DrawGui(renderSortedList);
            DrawFps();
        }
        Raylib.EndDrawing();
    }

    private static void DrawScene(IReadOnlyList<GameObject> targets) {
        if (Initialized) {
            if (_tickTimeCounter >= 1.0f / TargetTps) {
                _tickTimeCounter = 0;
                Frame++;
                
                TickDialogue();
                TickAnimations();
                TickGameObjects(targets);
                _scene.OnTick();
            }
            
            UpdateGameObjects(targets);
            _scene.OnUpdate();
        }
    }

    private static void DrawGui(IReadOnlyList<GameObject> targets) {
        if (Initialized) {
            UpdateGui(targets);
            _scene.OnGui();
        }
        
        UpdateDialogue(); // Call this after the scene update so that the
                          // dialogue box can be drawn on top of all scene objects.

        UpdateCurtain(); // Call this after the scene update so that the
                         // curtain can be drawn on top of everything else
    }
    
    private static void ReloadRenderTexture() {
        if (_prevWindowWidth != WindowWidth || _prevWindowHeight != WindowHeight) {
            _prevWindowWidth = WindowWidth;
            _prevWindowHeight = WindowHeight;

            Raylib.UnloadRenderTexture(GameTexture); // Prevent RAM-Nuke
            GameTexture = Raylib.LoadRenderTexture(WindowWidth, WindowHeight);
        }
    }

    private static void Close() {
        EngineLogger.Info("Disposing engine...");
        UnloadSounds();
        UnloadFonts();
        UnloadShaders();
        UnloadTextures();
        RemoveGameObjects();
        Raylib.CloseWindow();
        Raylib.CloseAudioDevice();
    }
}