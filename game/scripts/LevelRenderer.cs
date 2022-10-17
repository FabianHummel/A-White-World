using Raylib_CsLo;
using WhiteWorld.engine;
using WhiteWorld.engine.ecs;

namespace WhiteWorld.game.scripts; 

public class LevelRenderer : GameScript {
    
    private Engine.Level _level = null!;
    private readonly List<Texture[]> _textures = new();
    private (int paletteId, int variant)[,] _tiles = null!;

    public override void OnInit() {
        _level = (Engine.Level) Engine.CurrentScene;

        foreach (var item in _level.LevelData.Palette) {
            var variants = new Texture[item.Length];
            for (var i = 0; i < item.Length; i++) {
                variants[i] = Engine.GetTexture(item[i]);
            }
            _textures.Add(variants);
        }

        var random = new Random();
        _tiles = new (int, int)[_level.LevelWidth, _level.LevelHeight];
        for (var x = 0; x < _level.LevelWidth; x++) {
            for (var y = 0; y < _level.LevelHeight; y++) {
                var tile = _level.LevelData.Tiles[x, y];
                _tiles[x, y] = (tile, random.Next(0, _textures[tile].Length));
            }
        }
    }

    public override void OnUpdate() {
        for (var i = 0; i < _level.LevelWidth; i++) {
            for (var j = 0; j < _level.LevelHeight; j++) {
                var tile = _tiles[i, j];
                switch (tile) {
                    default:
                        var variants = _textures[tile.paletteId];
                        Engine.DrawSceneTexture(variants[tile.variant], i * 16, j * 16);
                        break;

                    case (-1, _):
                        continue;
                }
            }
        }
    }
}