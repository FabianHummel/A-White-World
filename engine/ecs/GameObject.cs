using WhiteWorld.engine.gui;
using WhiteWorld.engine.interfaces;
using WhiteWorld.engine.scripts;
using WhiteWorld.utility;

namespace WhiteWorld.engine.ecs;

public class GameObject {
    private readonly List<GameScript> _scripts = new();
    private readonly List<GameObject> _children = new();
    
    public IReadOnlyList<GameScript> Scripts => _scripts;
    public IReadOnlyList<GameObject> Children => _children;

    public GameObject? Parent { get; private set; }
    public Transform Transform { get; private set; }
    public string Name { get; set; } = null!;

    public GameObject() {
        Transform = new Transform();
        AddScript(Transform);
    }

    public void InitScripts() {
        foreach (var gameScript in _scripts) {
            gameScript.OnInit();
        }

        // Recursively initialize children's scripts
        foreach (var child in _children) {
            child.InitScripts();
        }
    }

    public void TickScripts() {
        foreach (var gameScript in _scripts) {
            gameScript.OnTick();
        }

        // Recursively tick children
        foreach (var child in _children) {
            child.TickScripts();
        }
    }

    public void UpdateScripts() {
        foreach (var gameScript in _scripts) {
            gameScript.OnUpdate();
        }

        foreach (var gameScript in _scripts.OfType<IViewport>()) {
            IViewport.Update(gameScript);
        }

        // Recursively update children
        foreach (var child in _children) {
            child.UpdateScripts();
        }
    }

    public void UpdateGui(GuiContext ctx) {
        foreach (var gameScript in _scripts) {
            gameScript.OnGui(ctx);
        }

        // Recursively update children
        foreach (var child in _children) {
            child.UpdateGui(ctx);
        }
    }

    public void Load() {
        foreach (var gameScript in _scripts.OfType<ISerializable>()) {
            DataBuffer? buffer = DataBuffer.ReadFromDisk(gameScript.Identifier);
            gameScript.Load(buffer);
        }
    }

    public void Unload() {
        foreach (var gameScript in _scripts.OfType<ISerializable>()) {
            DataBuffer buffer = new();
            gameScript.Save(buffer);
            buffer.WriteToDisk(gameScript.Identifier);
        }
    }

    public GameObject AddScript<T>(T script) where T : GameScript {
        var type = script.GetType();
        _scripts.AddIf(!(
            Attribute.IsDefined(type, typeof(DisallowMultipleInstancesAttribute)) &&
            _scripts.Any(s => s.GetType() == type)
        ), script);
        script.GameObject = this;
        return this;
    }
    
    public GameObject AddScripts<T>(IEnumerable<T> scripts) where T : GameScript {
        foreach (var script in scripts) {
            AddScript(script);
        }
        return this;
    }
    
    public T GetScript<T>() where T : GameScript {
        return (T) _scripts.FirstOrDefault(s => s is T)!;
    }
    
    public IEnumerable<T> GetScripts<T>() where T : GameScript {
        return from gameScript in _scripts where gameScript is T select (T) gameScript;
    }

    public GameObject RemoveScript<T>(T script) where T : GameScript {
        var type = script.GetType();
        _scripts.RemoveIf(
            !Attribute.IsDefined(type, typeof(DisallowRemovalAttribute)),
            script
        );
        return this;
    }
    
    public GameObject RemoveScripts<T>() where T : GameScript {
        foreach (var script in GetScripts<T>()) {
            RemoveScript(script);
        }
        return this;
    }
    
    public int GetScriptCount<T>() where T : GameScript {
        return _scripts.Count(s => s is T);
    }
    
    public int GetScriptCount() {
        return _scripts.Count;
    }
    
    public bool HasScript<T>() where T : GameScript {
        return _scripts.Any(s => s is T);
    }

    public GameObject WithChild(GameObject child) {
        child.Parent = this;
        _children.Add(child);
        return this;
    }

    public GameObject WithChildren(IEnumerable<GameObject> children) {
        _children.AddRange(children);
        return this;
    }

    public GameObject WithTransform(Transform transform) {
        Transform.UpdateTransform(transform);
        return this;
    }
}
