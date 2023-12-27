using System.Reflection;
using Newtonsoft.Json;
using Raylib_CsLo;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.gui;
using WhiteWorld.engine.interfaces;
using WhiteWorld.utility;

namespace WhiteWorld.engine;

public static partial class Engine {

    private static Scene _scene = null!;
    private static readonly Logger SceneLogger = GetLogger("Engine/Scene");

    private static float _curtainOuterRadius;
    private static float _curtainInnerRadius;
    private static CurtainTarget _curtainTarget = CurtainTarget.Open;

    private static bool _sceneSwitchQueued;
    private static Type? _queuedScene;
    
    public static Scene CurrentScene => _scene;

    public static async void SetScene<T>() where T : Scene, new() {
        await CloseCurtain();
        _sceneSwitchQueued = true;
        _queuedScene = typeof(T);
        await Task.Delay(1000); // Extra delay for better room temperature
        await OpenCurtain();
    }
    
    private static void SetSceneImmediate<T>() where T : Scene, new() {
        SceneLogger.Info($"Switching to scene {typeof(T).Name}");
        Initialized = false;
        StopAllSounds();
        CreateLoopToken();
        UnloadSounds();
        UnloadTextures();
        UnloadAnimations();
        UnloadResources();
        RemoveGameObjects();
        _scene = new T();
        _scene.Load();
        Initialized = true;
    }

    private enum CurtainTarget {
        Open, Close
    }
    
    private static double EaseInOutQuad(double x) {
        return x < 0.5 ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2;
    }

    private static async Task CloseCurtain() {
        SceneLogger.Info("Closing curtain");
        _curtainInnerRadius = 0;
        _curtainTarget = CurtainTarget.Close;
        while (_curtainTarget == CurtainTarget.Close && _curtainOuterRadius <= 1.0) {
            await Task.Yield();
        }
    }
    
    private static async Task OpenCurtain() {
        SceneLogger.Info("Opening curtain");
        _curtainTarget = CurtainTarget.Open;
        while (_curtainTarget == CurtainTarget.Open && _curtainInnerRadius <= 1.0) {
            await Task.Yield();
        }
        _curtainOuterRadius = 0;
        _curtainInnerRadius = 0;
    }

    private static void UpdateCurtain() {
        switch (_curtainTarget) {
            case CurtainTarget.Open:
                _curtainInnerRadius = _curtainInnerRadius >= 1 ? 1 : _curtainInnerRadius + DeltaTime;
                break;
            case CurtainTarget.Close:
                _curtainOuterRadius = _curtainOuterRadius >= 1 ? 1 : _curtainOuterRadius + DeltaTime;
                break;
            default:
                throw new Exception("Unknown curtain target");
        }

        // Warning: Pfusch incoming - here goes nothing
        if (_sceneSwitchQueued) {
            typeof(Engine)
                .GetMethod("SetSceneImmediate", BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(_queuedScene!)
                .Invoke(null, null);
            _sceneSwitchQueued = false;
        }
        
        var maxRadius = Math.Sqrt(CanvasWidth / 2 * CanvasWidth / 2 + CanvasHeight / 2 * CanvasHeight / 2) + 5.0f;
        DrawCurtain(
            CanvasWidth / 2, CanvasHeight / 2,
            (float) (EaseInOutQuad(_curtainOuterRadius) * maxRadius), 
            (float) (EaseInOutQuad(_curtainInnerRadius) * maxRadius),
            Raylib.RAYWHITE,
            4.0f, Raylib.DARKGRAY
        );
    }
    
    public abstract class Level : Scene {
        
        public LevelData LevelData { get; }
        public int LevelWidth { get; }
        public int LevelHeight { get; }

        protected Level(
            string levelPath,
            Dictionary<string, string>? soundsToRegister = null,
            Dictionary<string, string>? texturesToRegister = null,
            Dictionary<string, string>? resourcesToRegister = null,
            Dictionary<string, GameObject>? gameObjectsToSpawn = null
        ) : base(
            soundsToRegister,
            texturesToRegister,
            resourcesToRegister,
            gameObjectsToSpawn
        ) {
            var json = Raylib.LoadFileText(levelPath);
            var data = JsonConvert.DeserializeObject<LevelData>(json);
            if (data == null) {
                SceneLogger.Error($"Failed to load level {levelPath}");
                LevelData = default!;
            } else {
                LevelData = data;
            }
            
            LevelWidth = LevelData.Tiles.GetLength(0);
            LevelHeight = LevelData.Tiles.GetLength(1);
        }
    }

    public abstract class Scene : IUpdatable {
        
        public float CameraX { get; set; }
        public float CameraY { get; set; }
        
        private readonly Dictionary<string, string>? _soundsToRegister;
        private readonly Dictionary<string, string>? _texturesToRegister;
        private readonly Dictionary<string, string>? _resourcesToRegister;
        private readonly Dictionary<string, GameObject>? _gameObjectsToSpawn;

        protected Scene(
            Dictionary<string, string>? soundsToRegister = null,
            Dictionary<string, string>? texturesToRegister = null,
            Dictionary<string, string>? resourcesToRegister = null,
            Dictionary<string, GameObject>? gameObjectsToSpawn = null
        ) {
            _soundsToRegister = soundsToRegister;
            _texturesToRegister = texturesToRegister;
            _resourcesToRegister = resourcesToRegister;
            _gameObjectsToSpawn = gameObjectsToSpawn;
        }

        public void Load(bool persistent = false) {
            if (_soundsToRegister != null)
                foreach (var (id, sound) in _soundsToRegister)
                {
                    LoadSound(id, sound, persistent);
                }

            if (_texturesToRegister != null)
                foreach (var (id, texture) in _texturesToRegister)
                {
                    if (ImageUtil.IsGif(texture)) {
                        LoadAnimation(id, texture, persistent);
                    } else {
                        LoadTexture(id, texture, persistent);
                    }
                }

            if (_resourcesToRegister != null)
                foreach (var (id, resource) in _resourcesToRegister)
                {
                    LoadResource(id, resource, persistent);
                }

            if (_gameObjectsToSpawn != null)
                foreach (var (id, gameObject) in _gameObjectsToSpawn)
                {
                    SpawnGameObject(id, gameObject, persistent);
                }

            foreach (var (_, (gameObject, _)) in GameObjects) {
                gameObject.InitScripts();
            }
            
            OnInit();
        }

        public virtual void OnInit() {}
        public virtual void OnTick() {}
        public virtual void OnUpdate() {}
        public virtual void OnGui(GuiContext ctx) {}
    }
}