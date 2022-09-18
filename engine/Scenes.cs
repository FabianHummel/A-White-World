using System.Reflection;
using Raylib_CsLo;

namespace WhiteWorld.engine;

public static partial class Engine {

    private static Scene? _scene;
    private static readonly Logger SceneLogger = GetLogger("Engine/Scene");

    private static float _curtainOuterRadius;
    private static float _curtainInnerRadius;
    private static CurtainTarget _curtainTarget = CurtainTarget.Open;

    private static bool _sceneSwitchQueued;
    private static Type? _queuedScene;

    public static async void SetScene<T>() where T : Scene, new() {
        await CloseCurtain();
        await Task.Delay(1000); // Extra delay for better room temperature
        _sceneSwitchQueued = true;
        _queuedScene = typeof(T);
        await OpenCurtain();
    }
    
    private static void SetSceneImmediate<T>() where T : Scene, new() {
        SceneLogger.Info($"Switching to scene {typeof(T).Name}");
        Initialized = false;
        StopAllSounds();
        _loopToken = new CancellationTokenSource();
        UnloadSounds();
        UnloadTextures();
        UnloadAnimations();
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
        DrawCircle(
            CanvasWidth / 2, CanvasHeight / 2,
            (float) (EaseInOutQuad(_curtainOuterRadius) * maxRadius), 
            (float) (EaseInOutQuad(_curtainInnerRadius) * maxRadius),
            Raylib.RAYWHITE,
            4.0f, Raylib.DARKGRAY
        );
    }
}

public abstract class Scene {
    
    private readonly Dictionary<string, string> _soundsToRegister;
    private readonly Dictionary<string, string> _texturesToRegister;
    private readonly Dictionary<string, (string, int)> _animationsToRegister;
    private readonly Dictionary<string, GameObject> _gameObjectsToSpawn;

    protected Scene(Dictionary<string, string> soundsToRegister, Dictionary<string, string> texturesToRegister, Dictionary<string, (string, int)> animationsToRegister, Dictionary<string, GameObject> gameObjectsToSpawn) {
        _soundsToRegister = soundsToRegister;
        _texturesToRegister = texturesToRegister;
        _animationsToRegister = animationsToRegister;
        _gameObjectsToSpawn = gameObjectsToSpawn;
    }

    public void Load() {
        foreach (var (id, sound) in _soundsToRegister) {
            Engine.LoadSound(id, sound);
        }
        
        foreach (var (id, texture) in _texturesToRegister) {
            Engine.LoadTexture(id, texture);
        }
        
        foreach (var (id, (texture, frames)) in _animationsToRegister) {
            Engine.LoadAnimation(id, texture, frames);
        }
        
        foreach (var (id, gameObject) in _gameObjectsToSpawn) {
            Engine.SpawnGameObject(id, gameObject);
        }
        
        OnLoad();
    }

    // ReSharper disable once MemberCanBeProtected.Global
    public abstract void OnLoad();
    
    public abstract void OnUpdate();
    
    public abstract void OnTick();
}