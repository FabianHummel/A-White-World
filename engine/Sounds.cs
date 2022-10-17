using Raylib_CsLo;
using WhiteWorld.utility;

namespace WhiteWorld.engine;

public static partial class Engine {

    private static readonly Registry<Sound> RegisteredSounds = new();
    private static readonly Logger SoundLogger = GetLogger("Engine/Sound");
    
    private static readonly bool LoopEnabled = true;
    private static CancellationTokenSource _loopToken = new();
    
    public static Sound LoadSound(string name, string resource, bool persistent = false) {
        SoundLogger.Info($"Loading sound {name} from {resource}");
        var instance = Raylib.LoadSound(resource);
        RegisteredSounds.Add(name,
           ( instance, persistent ) 
        );
        return instance;
    }

    private static void UnloadSounds() {
        var query = from sound in RegisteredSounds
            where !sound.Value.persistent
            select sound;

        foreach (var (id, (sound, _)) in query) {
            RegisteredSounds.Remove(id);
            Raylib.UnloadSound(sound);
        }
    }

    public static void DumpSounds() {
        var query = RegisteredSounds.Select(kvp =>
            $"[Sound] {kvp.Key}: " +
            $"length: {kvp.Value.item.Length():00,0##}s; " +
            $"persistent: {kvp.Value.persistent}; "
        );
        
        SoundLogger.Debug(query.Prepend("Dumping Sounds:").ToArray());
    }
    
    private static Sound GetSound(string name) {
        if (RegisteredSounds.TryGetValue(name, out var sound)) {
            return sound.item;
        }
        SoundLogger.Error($"Sound {name} not found");
        return default;
    }

    public static void PlaySound(string id, bool loop = false) {
        var sound = GetSound(id);
        Raylib.PlaySoundMulti(sound);
        if (loop && LoopEnabled) {
            SoundLogger.Info($"Looping sound {id}");
            Task.Delay((int) sound.Length(), _loopToken.Token).ContinueWith(_ => {
                PlaySound(id, loop);
            }, _loopToken.Token);
        }
    }

    public static void PlayRandomSound(params string[] sounds) {
        PlaySound(sounds[Raylib.GetRandomValue(0, sounds.Length - 1)]);
    }
    
    public static void PlayUnregisteredSound(Sound sound) {
        Raylib.PlaySoundMulti(sound);
        Task.Delay((int) sound.Length()).ContinueWith(_ => {
            Raylib.UnloadSound(sound);
        });
    }

    private static void StopAllSounds() {
        Raylib.StopSoundMulti();
        _loopToken.Cancel();
    }
}