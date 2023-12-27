using System.Numerics;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Raylib_CsLo;
using Raylib_CsLo.InternalHelpers;

namespace WhiteWorld.utility;

public class RaylibExtensions {

    // Reimplementing DrawTextEx cause original author is too lazy to implement custom lineheight
    public static unsafe void DrawTextEx(Font font, string text, Vector2 position, float fontSize, float spacing, float lineHeight, Color tint) {
        var sotext = text.MarshalUtf8(); // Convert to Raylib internal C-type

        if (font.texture.id == 0) font = Raylib.GetFontDefault();  // Security check in case of not valid font
        var size = Raylib.TextLength(text);    // Total size in bytes of the text, scanned by codepoints in loop

        var textOffsetY = 0;            // Offset between lines (on line break '\n')
        var textOffsetX = 0.0f;       // Offset X to next character to draw

        var scaleFactor = fontSize/font.baseSize;         // Character quad scaling factor

        for (var i = 0; i < size;)
        {
            // Get next codepoint from byte string and glyph index in font
            var codepointByteCount = 0;
            var codepoint = RaylibExtensions.GetCodepointNext(&sotext.AsPtr()[i], &codepointByteCount);
            var index = Raylib.GetGlyphIndex(font, codepoint);

            // NOTE: Normally we exit the decoding sequence as soon as a bad byte is found (and return 0x3f)
            // but we need to draw all of the bad bytes using the '?' symbol moving one byte
            if (codepoint == 0x3f) codepointByteCount = 1;

            if (codepoint == '\n')
            {
                // NOTE: Fixed line spacing of 1.5 line-height
                textOffsetY += (int)(lineHeight * font.baseSize * scaleFactor);
                textOffsetX = 0.0f;
            }
            else
            {
                if ((codepoint != ' ') && (codepoint != '\t'))
                {
                    Raylib.DrawTextCodepoint(font, codepoint, new Vector2(position.X + textOffsetX, position.Y + textOffsetY), fontSize, tint);
                }

                if (font.glyphs[index].advanceX == 0) textOffsetX += (font.recs[index].width*scaleFactor + spacing);
                else textOffsetX += (font.glyphs[index].advanceX*scaleFactor + spacing);
            }

            i += codepointByteCount;   // Move text bytes counter to next codepoint
        }
    }

    private static unsafe int GetCodepointNext(sbyte *text, int *codepointSize)
    {
        int codepoint;       // Codepoint (defaults to '?')
        *codepointSize = 0;

        // Get current codepoint and bytes processed
        if (0xf0 == (0xf8 & text[0]))
        {
            // 4 byte UTF-8 codepoint
            codepoint = ((0x07 & text[0]) << 18) | ((0x3f & text[1]) << 12) | ((0x3f & text[2]) << 6) | (0x3f & text[3]);
            *codepointSize = 4;
        }
        else if (0xe0 == (0xf0 & text[0]))
        {
            // 3 byte UTF-8 codepoint */
            codepoint = ((0x0f & text[0]) << 12) | ((0x3f & text[1]) << 6) | (0x3f & text[2]);
            *codepointSize = 3;
        }
        else if (0xc0 == (0xe0 & text[0]))
        {
            // 2 byte UTF-8 codepoint
            codepoint = ((0x1f & text[0]) << 6) | (0x3f & text[1]);
            *codepointSize = 2;
        }
        else
        {
            // 1 byte UTF-8 codepoint
            codepoint = text[0];
            *codepointSize = 1;
        }

        return codepoint;
    }

    private enum StateEnum
    {
        MeasureState = 0,
        DrawState = 1
    };

    private static StateEnum StateEnumOpposite(StateEnum e)
    {
        return e == StateEnum.DrawState ? StateEnum.MeasureState : StateEnum.DrawState;
    }

    public static unsafe void DrawTextBoxed(Font font, string text, Rectangle rec, float fontSize, float spacing, float lineHeight, Color tint) {
        var sotext = text.MarshalUtf8();
        var length = Raylib.TextLength(text);  // Total length in bytes of the text, scanned by codepoints in loop

        float textOffsetY = 0;          // Offset between lines (on line break '\n')
        var textOffsetX = 0.0f;       // Offset X to next character to draw

        var scaleFactor = fontSize/font.baseSize;     // Character rectangle scaling factor

        // Word/character wrapping mechanism variables
        var state = StateEnum.MeasureState;

        var startLine = -1;         // Index where to begin drawing (where a line begins)
        var endLine = -1;           // Index where to stop drawing (where a line ends)
        var lastk = -1;             // Holds last value of the character position

        for (int i = 0, k = 0; i < length; i++, k++)
        {
            // Get next codepoint from byte string and glyph index in font
            var codepointByteCount = 0;
            var codepoint = Raylib.GetCodepoint(&sotext.AsPtr()[i], &codepointByteCount);
            var index = Raylib.GetGlyphIndex(font, codepoint);

            // NOTE: Normally we exit the decoding sequence as soon as a bad byte is found (and return 0x3f)
            // but we need to draw all of the bad bytes using the '?' symbol moving one byte
            if (codepoint == 0x3f) codepointByteCount = 1;
            i += (codepointByteCount - 1);

            float glyphWidth = 0;
            if (codepoint != '\n')
            {
                glyphWidth = (font.glyphs[index].advanceX == 0) ? font.recs[index].width*scaleFactor : font.glyphs[index].advanceX*scaleFactor;

                if (i + 1 < length) glyphWidth = glyphWidth + spacing;
            }

            // NOTE: When wordWrap is ON we first measure how much of the text we can draw before going outside of the rec container
            // We store this info in startLine and endLine, then we change states, draw the text between those two variables
            // and change states again and again recursively until the end of the text (or until we get outside of the container).
            // When wordWrap is OFF we don't need the measure state so we go to the drawing state immediately
            // and begin drawing on the next line before we can get outside the container.
            if (state == StateEnum.MeasureState)
            {
                // TODO: There are multiple types of spaces in UNICODE, maybe it's a good idea to add support for more
                // Ref: http://jkorpela.fi/chars/spaces.html
                if ((codepoint == ' ') || (codepoint == '\t') || (codepoint == '\n')) endLine = i;

                if ((textOffsetX + glyphWidth) > rec.width)
                {
                    endLine = (endLine < 1)? i : endLine;
                    if (i == endLine) endLine -= codepointByteCount;
                    if ((startLine + codepointByteCount) == endLine) endLine = (i - codepointByteCount);

                    state = StateEnumOpposite(state);
                }
                else if ((i + 1) == length)
                {
                    endLine = i;
                    state = StateEnumOpposite(state);
                }
                else if (codepoint == '\n') state = StateEnumOpposite(state);

                if (state == StateEnum.DrawState)
                {
                    textOffsetX = 0;
                    i = startLine;
                    glyphWidth = 0;

                    // Save character position when we switch states
                    var tmp = lastk;
                    lastk = k - 1;
                    k = tmp;
                }
            }
            else
            {
                if (codepoint != '\n')
                {
                    // When text overflows rectangle height limit, just stop drawing
                    if ((textOffsetY + font.baseSize*scaleFactor) > rec.height) break;

                    // Draw current character glyph
                    if ((codepoint != ' ') && (codepoint != '\t'))
                    {
                        Raylib.DrawTextCodepoint(font, codepoint, new Vector2( rec.x + textOffsetX, rec.y + textOffsetY ), fontSize, Raylib.RAYWHITE);
                    }
                }

                if (i == endLine)
                {
                    textOffsetY += lineHeight * font.baseSize * scaleFactor;
                    textOffsetX = 0;
                    startLine = endLine;
                    endLine = -1;
                    glyphWidth = 0;
                    k = lastk;

                    state = StateEnumOpposite(state);
                }
            }

            if ((textOffsetX != 0) || (codepoint != ' ')) textOffsetX += glyphWidth;  // avoid leading spaces
        }
    }

    public unsafe static Vector2 MeasureTextEx(Font font, string text, float fontSize, float spacing, float lineHeight)
    {
        var sotext = text.MarshalUtf8();
        var size = Raylib.TextLength(text);    // Get size in bytes of text
        var tempByteCounter = 0;        // Used to count longer text line num chars
        var byteCounter = 0;

        var textWidth = 0.0f;
        var tempTextWidth = 0.0f;     // Used to count longer text line width

        float textHeight = font.baseSize;
        var scaleFactor = fontSize/font.baseSize;

        for (var i = 0; i < size; i++)
        {
            byteCounter++;

            var next = 0;
            var letter = Raylib.GetCodepoint(&sotext.AsPtr()[i], &next);     // Current character
            var index = Raylib.GetGlyphIndex(font, letter);                  // Index position in sprite font

            // NOTE: normally we exit the decoding sequence as soon as a bad byte is found (and return 0x3f)
            // but we need to draw all of the bad bytes using the '?' symbol so to not skip any we set next = 1
            if (letter == 0x3f) next = 1;
            i += next - 1;

            if (letter != '\n') {
                if (font.glyphs[index].advanceX != 0) textWidth += font.glyphs[index].advanceX;
                else textWidth += (font.recs[index].width + font.glyphs[index].offsetX);
            }
            else {
                if (tempTextWidth < textWidth) tempTextWidth = textWidth;
                byteCounter = 0;
                textWidth = 0;
                textHeight += (font.baseSize*lineHeight); // NOTE: Fixed line spacing of 1.5 lines
            }

            if (tempByteCounter < byteCounter) tempByteCounter = byteCounter;
        }

        if (tempTextWidth < textWidth) tempTextWidth = textWidth;

        return new Vector2(
            tempTextWidth*scaleFactor + ((tempByteCounter - 1)*spacing),
            textHeight*scaleFactor);
    }
}