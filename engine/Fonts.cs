using Raylib_CsLo;

namespace WhiteWorld.engine; 

public static unsafe partial class Engine {

    public static Font TitleFont { get; private set; }
    public static Font ParagraphFont { get; private set; }

    private static readonly Logger FontLogger = GetLogger("Engine/Font");

    private static void LoadFonts() {
        FontLogger.Info("Loading fonts...");
        TitleFont = Raylib.LoadFontEx("assets/fonts/BitMirror.ttf", 96, (int*)0, 0);
        ParagraphFont = Raylib.LoadFontEx("assets/fonts/BitMirror.ttf", 96, (int*)0, 0);
    }
    
    private static void UnloadFonts() {
        Raylib.UnloadFont(TitleFont);
        Raylib.UnloadFont(ParagraphFont);
    }
}