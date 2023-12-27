using Raylib_CsLo;

namespace WhiteWorld.engine.gui;

using RayColor = Raylib_CsLo.Color;

public class Color {
    public static RayColor White = Raylib.RAYWHITE;
    public static RayColor Black = Raylib.DARKGRAY;
    public static RayColor Gray = Raylib.LIGHTGRAY;
    public static RayColor Debug = new RayColor {r = 200, g = 100, b = 100, a = 50};
}
