namespace WhiteWorld.engine; 

//public class PaletteEntry {
//    public string Id { get; }
//    public string[] Tex { get; }
//    public string[] Ani { get; }
//
//    public PaletteEntry(string id, string[] tex, string[] ani) {
//        this.Id = id;
//        this.Tex = tex;
//        this.Ani = ani;
//    }
//}

public class LevelData {
    public List<string[]> Palette { get; }
    public int[,] Tiles { get; }

    public LevelData(List<string[]> palette, int[,] tiles) {
        this.Palette = palette;
        this.Tiles = tiles;
    }
}