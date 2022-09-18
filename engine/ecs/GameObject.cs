using WhiteWorld.engine.scripts;
using WhiteWorld.utility;

namespace WhiteWorld.engine;

public class GameObject {
    private readonly List<IGameScript> _scripts = new();
    
    public Transform Transform { get; }

    public GameObject(int x = 0, int y = 0, int z = 0, params IGameScript[] scripts) {
        Transform = new Transform(x, y, z);
        AddScript(Transform);
        foreach (var gameScript in scripts) {
            AddScript(gameScript);
        }
    }

    public void InitScripts() {
        foreach (var gameScript in _scripts) {
            gameScript.OnInit();
        }
    }
    
    public void UpdateScripts() {
        foreach (var gameScript in _scripts) {
            gameScript.OnUpdate();
        }
    }
    
    public void TickScripts() {
        foreach (var gameScript in _scripts) {
            gameScript.OnTick();
        }
    }

    public void Unload() {
        
    }

    public GameObject AddScript<T>(T script) where T : IGameScript {
        var type = script.GetType();
        _scripts.AddIf(!(
            Attribute.IsDefined(type, typeof(DisallowMultipleInstancesAttribute)) &&
            _scripts.Any(s => s.GetType() == type)
        ), script);
        script.GameObject = this;
        if (Engine.Initialized) script.OnInit(); // Initialize script if engine is already initialized
        return this;
    }
    
    public GameObject AddScripts<T>(IEnumerable<T> scripts) where T : IGameScript {
        foreach (var script in scripts) {
            AddScript(script);
        }
        return this;
    }
    
    public T? GetScript<T>() where T : IGameScript {
        return (T?) _scripts.FirstOrDefault(s => s is T);
    }
    
    public IEnumerable<T> GetScripts<T>() where T : IGameScript {
        return from gameScript in _scripts where gameScript is T select (T) gameScript;
    }

    public GameObject RemoveScript<T>(T script) where T : IGameScript {
        _scripts.Remove(script);
        return this;
    }
    
    public GameObject RemoveScripts<T>() where T : IGameScript {
        _scripts.RemoveAll(s => s is T);
        return this;
    }
    
    public int GetScriptCount<T>() where T : IGameScript {
        return _scripts.Count(s => s is T);
    }
    
    public int GetScriptCount() {
        return _scripts.Count;
    }
    
    public bool HasScript<T>() where T : IGameScript {
        return _scripts.Any(s => s is T);
    }
}