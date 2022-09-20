using Raylib_CsLo;
using WhiteWorld.engine;

namespace WhiteWorld.game.scripts; 

public class LevelRenderer : GameScript {
    
    private Engine.Level _level = null!;
    private List<Texture> _textures = null!;

    public override void OnInit() {
        _level = (Engine.Level) Engine.CurrentScene;
        _textures = new List<Texture>();
        foreach (var item in _level.LevelData.Palette) {
            _textures.Add(Engine.GetTexture(item));
        }
    }

    public override void OnUpdate() {
        for (var i = 0; i < _level.LevelWidth - 1; i++) {
            for (var j = 0; j < _level.LevelHeight - 1; j++) {
                Engine.DrawSceneTexture(
                    _textures[_level.LevelData.Tiles[i, j]], i * 16, j * 16
                );
            }
        }
    }

    public override void OnTick() {
        
    }
}