namespace WhiteWorld.engine; 

public class LevelData {
    public List<string[]> Palette { get; }
    public int[,] Tiles { get; }

    public LevelData(List<string[]> palette, int[,] tiles) {
        this.Palette = palette;
        this.Tiles = tiles;
    }
}