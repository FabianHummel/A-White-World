using System.Numerics;
using Raylib_CsLo;

namespace WhiteWorld.engine;

public static partial class Engine {
    
    public enum Align {
        Start, Center, End
    }
    
    public const int PixelSize = 4;
    
    public static int WindowWidth { get; private set; }
    public static int WindowHeight { get; private set; }
    
    public static int CanvasWidth { get; private set; }
    public static int CanvasHeight { get; private set; }

    public static void DrawPixel(int x, int y, Color color) {
        Raylib.DrawRectangle(x * PixelSize, y * PixelSize, PixelSize, PixelSize, color);
    }
    
    private static void DrawTexture(Texture texture, int x, int y) {
        Raylib.DrawTextureEx(texture, new Vector2(x, y) * PixelSize, 0, PixelSize, Raylib.WHITE);
    }

    public static void DrawImage(string image, int x, int y, Align ax = Align.Start, Align ay = Align.Start) {
        var texture = GetTexture(image);
        var posX = ax switch {
            Align.Start => x,
            Align.Center => x + ( CanvasWidth / 2 - texture.width / 2 ),
            Align.End => x + ( CanvasWidth - texture.width ),
            _ => throw new Exception("Invalid align")
        };
        var posY = ay switch {
            Align.Start => y,
            Align.Center => y + ( CanvasHeight / 2 - texture.height / 2 ),
            Align.End => y + ( CanvasHeight - texture.height ),
            _ => throw new Exception("Invalid align")
        };
        DrawTexture(texture, posX, posY);
    }

    public static void StartAnimation(string image) {
        var animation = GetAnimation(image);
        animation.ResetAnimation();
    }

    public static void DrawAnimation(string image, int x, int y, Align ax = Align.Start, Align ay = Align.Start) {
        var texture = GetAnimation(image).Texture;
        var posX = ax switch {
            Align.Start => x,
            Align.Center => x + ( CanvasWidth / 2 - texture.width / 2 ),
            Align.End => x + ( CanvasWidth - texture.width ),
            _ => throw new Exception("Invalid align")
        };
        var posY = ay switch {
            Align.Start => y,
            Align.Center => y + ( CanvasHeight / 2 - texture.height / 2 ),
            Align.End => y + ( CanvasHeight - texture.height ),
            _ => throw new Exception("Invalid align")
        };
        DrawTexture(texture, posX, posY);
    }
    
    public static Vector2 GetTextLength(Font font, string text, int fontSize) {
        return Raylib.MeasureTextEx(font, text, fontSize, 0);
    }

    public static void DrawText(string text, int x, int y, int fontSize, Color color, Align ax = Align.Start, Align ay = Align.Start) {
        var posX = ax switch {
            Align.Start => x * PixelSize,
            Align.Center => x * PixelSize + WindowWidth / 2,
            Align.End => x * PixelSize + WindowWidth,
            _ => throw new Exception("Invalid align")
        };
        var posY = ay switch {
            Align.Start => y * PixelSize,
            Align.Center => y * PixelSize + WindowHeight / 2,
            Align.End => y * PixelSize + WindowHeight,
            _ => throw new Exception("Invalid align")
        };
        Raylib.DrawTextEx(TitleFont, text, new Vector2(posX, posY), fontSize, 0, color);
    }
    
    public static void DrawTextCentered(string text, int x, int y, int fontSize, Color color, Align ax = Align.Start, Align ay = Align.Start) {
        var length = GetTextLength(TitleFont, text, fontSize);
        var posX = ax switch {
            Align.Start => x * PixelSize,
            Align.Center => x * PixelSize + ( WindowWidth / 2 - (int) length.X / 2 ),
            Align.End => x * PixelSize + ( WindowWidth - (int) length.X ),
            _ => throw new Exception("Invalid align")
        };
        var posY = ay switch {
            Align.Start => y * PixelSize,
            Align.Center => y * PixelSize + ( WindowHeight / 2 - (int) length.Y / 2 ),
            Align.End => y * PixelSize + ( WindowHeight - (int) length.Y ),
            _ => throw new Exception("Invalid align")
        };
        Raylib.DrawTextEx(TitleFont, text, new Vector2(posX, posY), fontSize, 0, color);
    }

    public static void DrawRectangle(int x, int y, int width, int height, Color color, Align ax = Align.Start, Align ay = Align.Start) {
        var posX = ax switch {
            Align.Start => x * PixelSize,
            Align.Center => x * PixelSize + WindowWidth / 2,
            Align.End => x * PixelSize + WindowWidth,
            _ => throw new Exception("Invalid align")
        };
        var posY = ay switch {
            Align.Start => y * PixelSize,
            Align.Center => y * PixelSize + WindowHeight / 2,
            Align.End => y * PixelSize + WindowHeight,
            _ => throw new Exception("Invalid align")
        };
        Raylib.DrawRectangle(posX, posY, width * PixelSize, height * PixelSize, color);
    }
    
    public static void DrawRectangleCentered(int x, int y, int width, int height, Color color, Align ax = Align.Start, Align ay = Align.Start) {
        var posX = ax switch {
            Align.Start => x * PixelSize,
            Align.Center => x * PixelSize + (WindowWidth / 2 - width * PixelSize / 2),
            Align.End => x * PixelSize + (WindowWidth - width * PixelSize),
            _ => throw new Exception("Invalid align")
        };
        var posY = ay switch {
            Align.Start => y * PixelSize,
            Align.Center => y * PixelSize + (WindowHeight / 2 - height * PixelSize / 2),
            Align.End => y * PixelSize + (WindowHeight - height * PixelSize),
            _ => throw new Exception("Invalid align")
        };
        Raylib.DrawRectangle(posX, posY, width * PixelSize, height * PixelSize, color);
    }

    public static void DrawCircle(int cx, int cy, float or, float ir, Color c, float br = 0.0f, Color bc = default) {
        for (var y = 0; y < CanvasHeight; y++) {
            for (var x = 0; x < CanvasWidth; x++) {
                var dx = x - cx;
                var dy = y - cy;
                var d = MathF.Sqrt(dx * dx + dy * dy);
                if (d < or && d > ir - br) { // if distance inside outer and inner radius
                    if (d > or - br || d < ir) { // if distance inside outer or inner border
                        DrawPixel(x, y, bc);
                    } else {
                        DrawPixel(x, y, c);
                    }
                }
            }
        }
    }
}