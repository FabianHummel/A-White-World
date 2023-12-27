using Raylib_CsLo;

namespace WhiteWorld.engine.gui;

using RayColor = Raylib_CsLo.Color;

public class GuiContext {
    #region Inherited properties

    public decimal X { get; init; }
    public decimal Y { get; init; }
    
    public decimal ComputeX(decimal w) => AlignX switch {
        Align.Start => X,
        Align.Center => X + W / 2 - w / 2,
        Align.End => X + W - w
    };
    
    public decimal ComputeY(decimal h) => AlignY switch {
        Align.Start => Y,
        Align.Center => Y + H / 2 - h / 2,
        Align.End => Y + H - h
    };

    #endregion

    #region Default properties

    public decimal W { get; init; }
    public decimal H { get; init; }
    public decimal Opacity { get; set; }
    public Align AlignX { get; set; } = Align.Start;
    public Align AlignY { get; set; } = Align.Start;

    #endregion

    public void CreatePane(decimal x, decimal y, decimal w, decimal h, Action<GuiContext> ctx) {
        ctx.Invoke(new GuiContext {
            X = ComputeX(w) + x,
            Y = ComputeY(h) + y,
            W = w,
            H = h
        });
    }

    public void DrawText(string text, decimal x, decimal y, int fontSize, RayColor color) {
        var textSize = Engine.GetTextLength(Engine.MainFont, text, fontSize, 1.0f);
        var computedX = ComputeX((int) textSize.X) + x;
        var computedY = ComputeY((int) textSize.Y) + y;
        Engine.DrawUiText(text, (int) computedX, (int) computedY, fontSize, 1.0f, color);
    }

    public void DrawRectangle(decimal x, decimal y, decimal w, decimal h, RayColor color) {
        var computedX = ComputeX(w) + x;
        var computedY = ComputeY(h) + y;
        Engine.DrawUiRectangle((int) computedX, (int) computedY, (int) w, (int) h, color);
    }

    public void DrawTexture(string id, decimal x, decimal y) {
        var image = Engine.GetTexture(id);
        DrawTexture(image, x, y);
    }
    
    public void DrawTexture(Texture texture, decimal x, decimal y) {
        var computedX = ComputeX(texture.width) + x;
        var computedY = ComputeY(texture.height) + y;
        Engine.DrawUiTexture(texture, (int) computedX, (int) computedY);
    }
    
    public void DrawPixel(decimal x, decimal y, RayColor color) {
        var computedX = ComputeX(1) + x;
        var computedY = ComputeY(1) + y;
        Engine.DrawUiPixel((int) computedX, (int) computedY, color);
    }
}
