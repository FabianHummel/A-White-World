namespace WhiteWorld.engine; 

public class LevelData {
    public string[] Palette { get; }
    public int[,] Tiles { get; }

    public LevelData(string[] palette, int[,] tiles) {
        this.Palette = palette;
        this.Tiles = tiles;
    }
}