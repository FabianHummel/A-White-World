using System.Numerics;
using System.Text;
using Raylib_CsLo;

namespace WhiteWorld.utility; 

public static class Util {
    public static void AddIf<T>(this ICollection<T> source, bool predicate, T item) {
        if (predicate)
            source.Add(item);
    }
    
    public static double Length(this Sound sound) {
        return (double) sound.frameCount / sound.stream.sampleRate * 1000d;
    }

    public static string Repeats(this string str, long times) {
        var result = new StringBuilder();
        for (var i = 0; i < times; i++) {
            result.Append(str);
        }
        return result.ToString();
    }
    
    private static float Lerp(float firstFloat, float secondFloat, float by) {
        return firstFloat * (1 - by) + secondFloat * by;
    }
    
    public static Vector2 Lerp(Vector2 firstVector, Vector2 secondVector, float by) {
        var retX = Lerp(firstVector.X, secondVector.X, by);
        var retY = Lerp(firstVector.Y, secondVector.Y, by);
        return new Vector2(retX, retY);
    }
    
    public static Color LerpColor(Color firstColor, Color secondColor, float by) {
        var retR = (int) Lerp(firstColor.r, secondColor.r, by);
        var retG = (int) Lerp(firstColor.g, secondColor.g, by);
        var retB = (int) Lerp(firstColor.b, secondColor.b, by);
        var retA = (int) Lerp(firstColor.a, secondColor.a, by);
        return new Color(retR, retG, retB, retA);
    }
}