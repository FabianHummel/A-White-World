using System.Numerics;
using Raylib_CsLo;
using WhiteWorld.utility;

namespace WhiteWorld.engine;

public static partial class     Engine {
    public static int PixelSize { get; private set; } = 4;
    
    private static int _prevWindowWidth;
    private static int _prevWindowHeight;
    public static int WindowWidth { get; private set; }
    public static int WindowHeight { get; private set; }

    public static int CanvasWidth => WindowWidth / PixelSize;
    public static int CanvasHeight => WindowHeight / PixelSize;
    
    private static int CameraX => (int) CurrentScene.CameraX / PixelSize;
    private static int CameraY => (int) CurrentScene.CameraY / PixelSize;

    public static bool InViewport(Vector2 position, Vector2 size) {
        return
            position.X * PixelSize + size.X * PixelSize >= CurrentScene.CameraX &&
            position.X * PixelSize < CurrentScene.CameraX + WindowWidth &&
            position.Y * PixelSize + size.Y * PixelSize >= CurrentScene.CameraY &&
            position.Y * PixelSize < CurrentScene.CameraY + WindowHeight;
    }

    public static void DrawUiPixel(int x, int y, Color color) {
        Raylib.DrawRectangle(x * PixelSize, y * PixelSize, PixelSize, PixelSize, color);
    }
    
    public static void DrawScenePixel(int x, int y, Color color) {
        if (InViewport(new Vector2(x, y), Vector2.One)) {
            Raylib.DrawRectangle(x * PixelSize - (int) _scene.CameraX, y * PixelSize - (int) _scene.CameraY, PixelSize, PixelSize, color);
        }
    }
    
    public static void DrawUiTexture(Texture texture, int x, int y, float scale = 1.0f) {
        Raylib.DrawTextureEx(texture, new Vector2(x, y) * PixelSize, 0, PixelSize * scale, Raylib.WHITE);
    }

    public static void DrawSceneTexture(Texture texture, int x, int y, float rotation = 0f) {
        if (InViewport(new Vector2(x, y), new Vector2(texture.width, texture.height))) {
            Raylib.DrawTextureEx(texture, new Vector2(x, y) * PixelSize - new Vector2(_scene.CameraX, _scene.CameraY), rotation, PixelSize, Raylib.WHITE);
        }
    }

    public static void DrawUiImage(string image, int x, int y, float scale = 1.0f) {
        var texture = GetTexture(image);
        DrawUiTexture(texture, x, y, scale);
    }

    public static void DrawSceneImage(string image, int x, int y) {
        var texture = GetTexture(image);
        DrawSceneTexture(texture, x, y);
    }
    
    public static Vector2 GetTextLength(Font font, string text, int fontSize, float lineHeight) {
        return RaylibExtensions.MeasureTextEx(font, text, fontSize, 0, lineHeight);
    }

    public static void DrawUiText(string text, int x, int y, int fontSize, float lineHeight, Color color) {
        RaylibExtensions.DrawTextEx(MainFont, text.ToLower(), new Vector2(x, y) * PixelSize, fontSize * PixelSize, 0, lineHeight, color);
    }
    
    public static void DrawSceneText(string text, int x, int y, int fontSize, float lineHeight, Color color) {
        RaylibExtensions.DrawTextEx(MainFont, text.ToLower(), new Vector2(x * PixelSize - _scene.CameraX, y * PixelSize - _scene.CameraY), fontSize * PixelSize, 0, lineHeight, color);
    }

    public static void DrawUiRectangle(int x, int y, int width, int height, Color color) {
        Raylib.DrawRectangle(x * PixelSize, y * PixelSize, width * PixelSize, height * PixelSize, color);
    }
    
    public static void DrawSceneRectangle(int x, int y, int width, int height, Color color) {
        if (InViewport(new Vector2(x, y), new Vector2(width, height))) {
            Raylib.DrawRectangle(x * PixelSize - (int) _scene.CameraX, y * PixelSize - (int) _scene.CameraY, width * PixelSize, height * PixelSize, color);
        }
    }


    // TODO: Replace with shader
    public static void DrawCurtain(int cx, int cy, float or, float ir, Color c, float br = 0.0f, Color bc = default) {
        for (var y = 0; y < CanvasHeight; y++) {
            for (var x = 0; x < CanvasWidth; x++) {
                var dx = x - cx;
                var dy = y - cy;
                var d = MathF.Sqrt(dx * dx + dy * dy);
                if (d < or && d > ir - br) { // if distance inside outer and inner radius
                    if (d > or - br || d < ir) { // if distance inside outer or inner border
                        DrawUiPixel(x, y, bc);
                    } else {
                        DrawUiPixel(x, y, c);
                    }
                }
            }
        }
    }
}