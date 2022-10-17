using System.Numerics;
using Raylib_CsLo;
using WhiteWorld.utility;

namespace WhiteWorld.engine;

public static partial class Engine {
    
    public enum Align {
        Start, Center, End
    }
    
    public static int PixelSize { get; private set; } = 4;
    
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
    
    public static void DrawUiTexture(Texture texture, int x, int y, Align ax = Align.Start, Align ay = Align.Start) {
        var posX = ax switch {
            Align.Start => x,
            Align.Center => x + CanvasWidth / 2 - texture.width / 2,
            Align.End => x + CanvasWidth - texture.width,
            _ => throw new Exception("Invalid align")
        };
        var posY = ay switch {
            Align.Start => y,
            Align.Center => y + CanvasHeight / 2 - texture.height / 2,
            Align.End => y + CanvasHeight - texture.height,
            _ => throw new Exception("Invalid align")
        };
        Raylib.DrawTextureEx(texture, new Vector2(posX, posY) * PixelSize, 0, PixelSize, Raylib.WHITE);
    }

    public static void DrawSceneTexture(Texture texture, int x, int y) {
        if (InViewport(new Vector2(x, y), new Vector2(texture.width, texture.height))) {
            Raylib.DrawTextureEx(texture, new Vector2(x, y) * PixelSize - new Vector2(_scene.CameraX, _scene.CameraY), 0, PixelSize, Raylib.WHITE);
        }
    }

    public static void DrawUiImage(string image, int x, int y, Align ax = Align.Start, Align ay = Align.Start) {
        var texture = GetTexture(image);
        DrawUiTexture(texture, x, y, ax, ay);
    }

    public static void DrawSceneImage(string image, int x, int y) {
        var texture = GetTexture(image);
        DrawSceneTexture(texture, x, y);
    }

    public static void StartAnimation(string image) {
        var animation = GetAnimation(image);
        animation.ResetAnimation();
    }

    public static void DrawUiAnimation(string image, int x, int y, Align ax = Align.Start, Align ay = Align.Start) {
        var texture = GetAnimation(image).Texture;
        var posX = ax switch {
            Align.Start => x,
            Align.Center => x + CanvasWidth / 2 - texture.width / 2,
            Align.End => x + CanvasWidth - (texture.width),
            _ => throw new Exception("Invalid align")
        };
        var posY = ay switch {
            Align.Start => y,
            Align.Center => y + CanvasHeight / 2 - texture.height / 2,
            Align.End => y + CanvasHeight - (texture.height),
            _ => throw new Exception("Invalid align")
        };
        DrawUiTexture(texture, posX, posY);
    }
    
    public static void DrawSceneAnimation(string image, int x, int y) {
        var texture = GetAnimation(image).Texture;
        DrawSceneTexture(texture, x, y);
    }
    
    public static Vector2 GetTextLength(Font font, string text, int fontSize) {
        return Raylib.MeasureTextEx(font, text, fontSize, 0);
    }

    public static void DrawUiText(string text, int x, int y, int fontSize, float lineHeight, Color color, Align ax = Align.Start, Align ay = Align.Start) {
        var posX = ax switch {
            Align.Start => x,
            Align.Center => x + CanvasWidth / 2,
            Align.End => x + CanvasWidth,
            _ => throw new Exception("Invalid align")
        };
        var posY = ay switch {
            Align.Start => y,
            Align.Center => y + CanvasHeight / 2,
            Align.End => y + CanvasHeight,
            _ => throw new Exception("Invalid align")
        };

        RaylibExtensions.DrawTextEx(MainFont, text.ToLower(), new Vector2(posX, posY) * PixelSize, fontSize * PixelSize, 0, lineHeight, color);
    }
    
    public static void DrawUiTextCentered(string text, int x, int y, int fontSize, float lineHeight, Color color, Align ax = Align.Start, Align ay = Align.Start) {
        var length = GetTextLength(MainFont, text, fontSize * PixelSize);
        var posX = ax switch {
            Align.Start => x,
            Align.Center => x + CanvasWidth / 2 - (int) (length.X / 2 / PixelSize),
            Align.End => x + CanvasWidth - (int) (length.X / PixelSize),
            _ => throw new Exception("Invalid align")
        };
        var posY = ay switch {
            Align.Start => y,
            Align.Center => y + CanvasHeight / 2 - (int) (length.Y / 2 / PixelSize),
            Align.End => y + CanvasHeight - (int) (length.Y / PixelSize),
            _ => throw new Exception("Invalid align")
        };
        RaylibExtensions.DrawTextEx(MainFont, text.ToLower(), new Vector2(posX, posY) * PixelSize, fontSize * PixelSize, 0, lineHeight, color);
    }
    
    public static void DrawSceneText(string text, int x, int y, int fontSize, float lineHeight, Color color) {
        RaylibExtensions.DrawTextEx(MainFont, text.ToLower(), new Vector2(x * PixelSize - _scene.CameraX, y * PixelSize - _scene.CameraY), fontSize * PixelSize, 0, lineHeight, color);
    }

    public static void DrawUiRectangle(int x, int y, int width, int height, Color color, Align ax = Align.Start, Align ay = Align.Start) {
        var posX = ax switch {
            Align.Start => x,
            Align.Center => x + CanvasWidth / 2,
            Align.End => x + CanvasWidth,
            _ => throw new Exception("Invalid align")
        };
        var posY = ay switch {
            Align.Start => y,
            Align.Center => y + CanvasHeight / 2,
            Align.End => y + CanvasHeight,
            _ => throw new Exception("Invalid align")
        };
        Raylib.DrawRectangle(posX * PixelSize, posY * PixelSize, width * PixelSize, height * PixelSize, color);
    }

    public static void DrawUiRectangleCentered(int x, int y, int width, int height, Color color, Align ax = Align.Start, Align ay = Align.Start) {
        var posX = ax switch {
            Align.Start => x,
            Align.Center => x + CanvasWidth / 2 - width / 2,
            Align.End => x + CanvasWidth - width,
            _ => throw new Exception("Invalid align")
        };
        var posY = ay switch {
            Align.Start => y,
            Align.Center => y + CanvasHeight / 2 - height / 2,
            Align.End => y + CanvasHeight - height,
            _ => throw new Exception("Invalid align")
        };
        Raylib.DrawRectangle(posX, posY, width * PixelSize, height * PixelSize, color);
    }
    
    public static void DrawSceneRectangle(int x, int y, int width, int height, Color color) {
        if (InViewport(new Vector2(x, y), new Vector2(width, height))) {
            Raylib.DrawRectangle(x * PixelSize - (int) _scene.CameraX, y * PixelSize - (int) _scene.CameraY, width * PixelSize, height * PixelSize, color);
        }
    }

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