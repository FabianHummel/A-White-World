using Raylib_CsLo;

namespace WhiteWorld.engine; 

public static unsafe partial class Engine {
    
    private static readonly Registry<Texture> RegisteredTextures = new();
    private static readonly Registry<Animation> RegisteredAnimations = new();
    private static readonly Logger TextureLogger = GetLogger("Engine/Textures");
    
    public static Texture LoadTexture(string name, string texture, bool persistent = false) {
        TextureLogger.Info($"Loading texture {name} from {texture}");
        var instance = Raylib.LoadTexture(texture);
        RegisteredTextures.Add(name,
            ( instance, persistent )
        );
        return instance;
    }
    
    private static void UnloadTextures() {
        var query = from texture in RegisteredTextures
            where !texture.Value.persistent
            select texture;

        foreach (var (id, (texture, _)) in query) {
            RegisteredTextures.Remove(id);
            Raylib.UnloadTexture(texture);
        }
    }

    public static void DumpTextures() {
        var query = RegisteredTextures.Select(kvp =>
            $"[Texture] {kvp.Key}: " +
            $"dimensions: {kvp.Value.item.width}x{kvp.Value.item.height}; " +
            $"persistent: {kvp.Value.persistent}; "
        );
        
        SoundLogger.Debug(query.Prepend("Dumping Textures:").ToArray());
    }
    
    public static Texture GetTexture(string name) {
        if (RegisteredTextures.TryGetValue(name, out var texture)) {
            return texture.item;
        }
        TextureLogger.Error($"Texture {name} not found");
        return default;
    }
    
    public class Animation {
        public Texture Texture { get; }
        private readonly Image _image;
        private readonly int _frameCount;
        private readonly int _delay;
        private int _frame;
        private int _startFrame;

        public Animation(string animation, int delay) {
            int frames;
            _image = Raylib.LoadImageAnim(animation, &frames);
            Texture = Raylib.LoadTextureFromImage(_image);
            _frameCount = frames;
            _delay = delay;
        }
        
        public void NextFrame() {
            _frame = (_frame + 1) % _frameCount;
            var offset = (uint) (_frame * _image.width * _image.height);
            Raylib.UpdateTexture(Texture, (uint*)_image.data + offset);
        }

        public void ResetAnimation() {
            _startFrame = Frame;
            _frame = 0;
        }

        public void TickAnimation() {
            var currentTick = Frame - _startFrame;
            if (currentTick % _delay == 0) {
                NextFrame();
            }
        }
        
        public void Unload() {
            Raylib.UnloadImage(_image);
            Raylib.UnloadTexture(Texture);
        }
    }

    public static Animation LoadAnimation(string id, string animation, int delay, bool persistent = false) {
        TextureLogger.Info($"Loading animation {animation}");
        var instance = new Animation(animation, delay);
        RegisteredAnimations.Add(id,
            ( instance, persistent )
        );
        return instance;
    }
    
    private static void TickAnimations() {
        foreach (var animation in RegisteredAnimations.Values.Select(anim => anim.item)) {
            animation.TickAnimation();
        }
    }

    private static void UnloadAnimations() {
        var query = from animation in RegisteredAnimations
            where !animation.Value.persistent
            select animation;
        
        foreach (var (id, (animation, _)) in query) {
            RegisteredAnimations.Remove(id);
            animation.Unload();
        }
    }
    
    public static void DumpAnimations() {
        var query = RegisteredAnimations.Select(kvp =>
            $"[Animation] {kvp.Key}: " +
            $"dimensions: {kvp.Value.item.Texture.width}x{kvp.Value.item.Texture.height}; " +
            $"persistent: {kvp.Value.persistent}; "
        );
        
        SoundLogger.Debug(query.Prepend("Dumping Animations:").ToArray());
    }
    
    public static Animation GetAnimation(string id) {
        if (RegisteredAnimations.TryGetValue(id, out var animation)) {
            return animation.item;
        }
        TextureLogger.Error($"Animation {id} not found");
        return default!;
    }
}
