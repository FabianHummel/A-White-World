using System.Numerics;
using Raylib_CsLo;
using WhiteWorld.engine.scripts;
using Transform = WhiteWorld.engine.scripts.Transform;

namespace WhiteWorld.engine;

public static partial class Engine {
    public const bool Debugging = true;

    private static void DrawFps() {
        if (!Debugging) return;
        Raylib.DrawFPS(10, 10);
    }

    private static void DrawOutline(int x, int y, int w, int h, Color c) {
        DrawSceneRectangle(x, y, w, 1, c);      // top
        DrawSceneRectangle(x, y+h-1, w, 1, c);  // bottom
        DrawSceneRectangle(x, y, 1, h, c);      // left
        DrawSceneRectangle(x+w-1, y, 1, h, c);  // right
    }

    public static void DebugTransform(Transform transform) {
        if (!Debugging) return;
        DrawOutline(
            transform.X, transform.Y,
            transform.W, transform.H,
            Raylib.RED
        );
        DrawSceneText($"{transform.X},{transform.Y} {transform.W}x{transform.H}", transform.X, transform.Y - 4, 4, 1.0f, Raylib.DARKGRAY);
    }

    public static void DebugPoint(Vector2 point) {
        if (!Debugging) return;
        DrawScenePixel(
            (int) Math.Round(point.X),
            (int) Math.Round(point.Y),
            Raylib.BLUE
        );
    }

    public static void DebugCollider(Collider collider) {
        if (!Debugging) return;
        DrawOutline(
            (int) collider.Position.X,
            (int) collider.Position.Y,
            (int) collider.Size.X,
            (int) collider.Size.Y,
            Raylib.GREEN
        );
    }

    public static void DebugRay(Vector2 origin, Vector2 direction, float distance) {
        if (!Debugging) return;
        var realOrigin = origin * PixelSize - new Vector2(
                Engine.CurrentScene.CameraX, Engine.CurrentScene.CameraY
            );
        Raylib.DrawLineV(
            realOrigin,
            realOrigin + direction * distance * PixelSize,
            Raylib.RED
        );
    }
}