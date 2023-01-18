using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Raylib_CsLo;
using WhiteWorld.engine;

namespace WhiteWorld.utility;

public static class Extensions {
    public static void AddIf<T>(this ICollection<T> source, bool predicate, T item) {
        if (predicate)
            source.Add(item);
    }

    public static void RemoveIf<T>(this ICollection<T> source, bool predicate, T item) {
        if (predicate)
            source.Remove(item);
    }

    public static T Pop<T>(this IList<T> source) {
        var item = source.ElementAt(0);
        source.RemoveAt(0);
        return item;
    }

    public static double Length(this Sound sound) {
        return (double) sound.frameCount / sound.stream.sampleRate * 1000d;
    }

    public static string LineBreaks(this string text, Font font, int fontSize, float maxLength) {
        StringBuilder final = new();
        var lineLength = 0.0f;
        foreach (var part in text.Split(" ")) {
            var len = Raylib.MeasureTextEx(font, $"{part} ", fontSize, 0).X;
            lineLength += len;
            
            if (lineLength > maxLength) {
                final.Append($"\n{part} ");
                lineLength = len; // The word that was just added is the new line length
            } else {
                final.Append($"{part} ");
            }
        }
        return final.ToString();
    }

    public static string Repeats(this string str, long times) {
        var result = new StringBuilder();
        for (var i = 0; i < times; i++) {
            result.Append(str);
        }

        return result.ToString();
    }

    public delegate void ApplyObj<in T>(T obj);

    public static string ToMemoryAddress(this object obj) {
        return $"{obj.GetType()}@{obj.GetHashCode().ToString("X")}";
    }
}

public static class MathUtil {
    public static float Lerp(float firstFloat, float secondFloat, float by) {
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

public static class ImageUtil {
    public static int GifDelay(string path) {
        if (File.Exists(path)) {
            var data = File.ReadAllBytes(path);
            var hex = Convert.ToHexString(data);
            var littleEndian = Regex.Match(hex, @"(?<=21F904..)....").Value; // This took way too long to figure out...
            var bigEndian = littleEndian.Substring(2, 2) + littleEndian.Substring(0, 2); // Swap Nibble -> Big endian
            return Convert.ToInt32(bigEndian, 16) * 10;
        }
        throw new FileNotFoundException("File not found", path);
    }

    public static bool IsGif(string path) {
        if (File.Exists(path)) {
            var data = File.ReadAllBytes(path);
            var hex = Convert.ToHexString(data);
            return hex.StartsWith("474946");
        }
        throw new FileNotFoundException("File not found", path);
    }
}