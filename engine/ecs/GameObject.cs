using WhiteWorld.engine.ecs.interfaces;
using WhiteWorld.engine.ecs.scripts;
using WhiteWorld.utility;

namespace WhiteWorld.engine.ecs;

public class GameObject {
    private readonly List<GameScript> _scripts = new();
    private readonly List<GameObject> _children = new();
    
    public IReadOnlyList<GameScript> Scripts => _scripts;
    public GameObject? Parent { get; private set; }
    public Transform Transform { get; private set; }

    public GameObject() {
        Transform = new Transform();
        AddScript(Transform);
    }

    public void InitScripts() {
        foreach (var gameScript in _scripts) {
            gameScript.OnInit();
        }

        InitEngineInterfaces();

        // Recursively initialize children's scripts
        foreach (var child in _children) {
            child.InitScripts();
        }
    }

    private void InitEngineInterfaces() {

    }
    
    public void UpdateScripts() {
        foreach (var gameScript in _scripts) {
            gameScript.OnUpdate();
        }

        UpdateEngineInterfaces();

        // Recursively update children
        foreach (var child in _children) {
            child.UpdateScripts();
        }
    }

    private void UpdateEngineInterfaces() {
        foreach (var gameScript in _scripts.OfType<IViewport>()) {
            if (!gameScript.InViewport && Engine.InViewport(gameScript.Position, gameScript.Size)) {
                gameScript.InViewport = true;
                gameScript.OnViewportEnter();
            } else if (gameScript.InViewport && !Engine.InViewport(gameScript.Position, gameScript.Size)) {
                gameScript.InViewport = false;
                gameScript.OnViewportExit();
            }
        }
    }
    
    public void TickScripts() {
        foreach (var gameScript in _scripts) {
            gameScript.OnTick();
        }

        TickEngineInterfaces();

        // Recursively tick children
        foreach (var child in _children) {
            child.TickScripts();
        }
    }

    private void TickEngineInterfaces() {

    }

    public void Unload() {
        
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
        _children.Add(child.Apply(o => {
            o.Parent = this;
        }));
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