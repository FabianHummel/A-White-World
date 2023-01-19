using System.Text;
using Raylib_CsLo;
using WhiteWorld.engine.gui;
using WhiteWorld.utility;
using RayColor = Raylib_CsLo.Color;
using Color = WhiteWorld.engine.gui.Color;

namespace WhiteWorld.engine;

public static partial class Engine {

    public class DialogueOption {
        public string Option { get; }
        public Action OnSelect { get; }

        public DialogueOption(string option, Action onSelect) {
            this.Option = option;
            this.OnSelect = onSelect;
        }
    }

    private class Dialogue {
        public string Title { get; }
        public string Text { get; }
        public DialogueOption[] Options { get; }
        public Action? OnContinue { get; }

        public Dialogue(string title, string text, DialogueOption[] options, Action? onContinue) {
            this.OnContinue = onContinue;
            this.Options = options;
            this.Title = title;
            this.Text = text;
        }

        public bool HasOptions => Options.Length > 0;
    }

    private const int MaxVisibleDialogueOptions = 2;

    private static readonly List<Dialogue> DialogueQueue = new();
    private static Dialogue _currentDialogue = null!;
    private static bool _dialogueOpen;
    private static int _selectedDialogueOption;
    private static int _selectedDialogueOptionLocal;
    
    private static string _dialogueText = "";
    private static int _dialogueTextFrame;

    public static bool DialogueOpen => _dialogueOpen;

    public static void QueueDialogue(string title, string text, DialogueOption[] options) {
        DialogueQueue.Add(new Dialogue(title, text, options, null));
    }
    
    public static void QueueDialogue(string title, string text, Action? onContinue = null) {
        DialogueQueue.Add(new Dialogue(title, text, Array.Empty<DialogueOption>(), onContinue));
    }
    
    private static bool IsDialogueComplete() {
        return _dialogueTextFrame >= _currentDialogue.Text.Length;
    }
    
    private static bool IsDialogueQueueEmpty() {
        return DialogueQueue.Count == 0;
    }
    
    private static bool HasDialogueOptions() {
        return _currentDialogue.HasOptions;
    }

    private static void TryNextDialogue() {
        if (IsDialogueQueueEmpty()) return;
        
        _currentDialogue = DialogueQueue.Pop();
        _selectedDialogueOption = 0;
        _selectedDialogueOptionLocal = 0;
        _dialogueOpen = true;
        _dialogueText = "";
        _dialogueTextFrame = 0;
    }

    private static void NextDialogueOption() {
        if (++_selectedDialogueOptionLocal > MaxVisibleDialogueOptions - 1)
            _selectedDialogueOptionLocal = MaxVisibleDialogueOptions - 1;
        if (++_selectedDialogueOption > _currentDialogue.Options.Length - 1)
            _selectedDialogueOption = _currentDialogue.Options.Length - 1;
        else
            PlayRandomSound("Cycle 1", "Cycle 2");
    }

    private static void PreviousDialogueOption() {
        if (--_selectedDialogueOptionLocal < 0)
            _selectedDialogueOptionLocal = 0;
        if (--_selectedDialogueOption < 0)
            _selectedDialogueOption = 0;
        else
            PlayRandomSound("Cycle 1", "Cycle 2");
    }

    private static void InvokeSelectedDialogueOption() {
        _currentDialogue.Options[_selectedDialogueOption].OnSelect();
        PlayRandomSound("Select 1", "Select 2");
    }

    private static void ContinueDialogue() {
        if (IsDialogueComplete()) {
            if (HasDialogueOptions()) {
                InvokeSelectedDialogueOption();
            } else {
                _currentDialogue.OnContinue?.Invoke();
            }

            if (IsDialogueQueueEmpty()) {
                _dialogueOpen = false;
                PlayRandomSound("Pop 1", "Pop 2");
            }
            else {
                TryNextDialogue();
            }
        }
        else {
            _dialogueTextFrame = _currentDialogue.Text.Length; // If the dialogue isn't complete, finish it
            PlayRandomSound("Pop 1", "Pop 2");
        }
    }

    private static void UpdateDialogue() {
        if (DialogueOpen) {
            if (HasDialogueOptions()) {
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) {
                    PreviousDialogueOption();
                }
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) {
                    NextDialogueOption();
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE) || Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER)) {
                ContinueDialogue();
            }
        }
        else {
            TryNextDialogue();
        }
    }

    private static int _dialogueBoxWidth;
    private static int _dialogueBoxHeight;

    private static void InitDialogue() {
        var box = LoadTexture("Dialogue Box", @"assets/images/dialogue-box.png", persistent: true);
        LoadTexture("Dialogue Box Options", @"assets/images/dialogue-box-options.png", persistent: true); // same size
        _dialogueBoxWidth = box.width;
        _dialogueBoxHeight = box.height;
    }

    private static void TickDialogue() {
        if (DialogueOpen && Frame % 2 == 0) {
            if (!IsDialogueComplete()) {
                _dialogueTextFrame++;
                PlayRandomSound("Text 1", "Text 2", "Text 3");
            }
            _dialogueText = _currentDialogue.Text[.._dialogueTextFrame];
        }
    }

    private static void DrawDialogue(GuiContext ctx) {
        if (!DialogueOpen) return;

        ctx.AlignX = Align.Center;
        ctx.AlignY = Align.End;

        ctx.CreatePane(0, -5, _dialogueBoxWidth, _dialogueBoxHeight, ctx => {
            if (HasDialogueOptions()) {
                ctx.DrawTexture("Dialogue Box Options", 0, 0);
                ctx.CreatePane(6, 6, ctx.W*0.69m - 12, ctx.H - 12, DrawDialogueText);
                ctx.CreatePane(3 + ctx.W*0.66m, 6, ctx.W*0.33m - 4, ctx.H - 12, DrawDialogueOptions);
            } else {
                ctx.DrawTexture("Dialogue Box", 0, 0);
                ctx.CreatePane(6, 6, ctx.W - 12, ctx.H - 12, DrawDialogueText);
            }
            
            DrawTitle(ctx);
        });
    }

    private static void DrawTitle(GuiContext ctx) {
        ctx.AlignX = Align.Start;
        ctx.AlignY = Align.Start;
        var titleLength = (int) GetTextLength(MainFont, _currentDialogue.Title, 5, 1.0f).X;
        ctx.DrawRectangle(7-2, -1, titleLength + 4, 7, Color.Black);
        ctx.DrawRectangle(7-1, 0, titleLength + 2, 5, Color.White);
        ctx.DrawText(_currentDialogue.Title, 7, 0, 5, Color.Black);
    }

    private static void DrawDialogueText(GuiContext ctx) {
        var brokenText = _dialogueText.LineBreaks(MainFont, 5, (float) ctx.W);
        ctx.DrawText(brokenText, 0, 0, 5, Color.Black);
    }
    
    private static void DrawDialogueOptions(GuiContext ctx) {
        var heightSum = 0m;
        var localOffset = ctx.H / 2 * (_selectedDialogueOption - _selectedDialogueOptionLocal);

        for (var i = 0; i < _currentDialogue.Options.Length; i++) {
            if (i >= _selectedDialogueOption - _selectedDialogueOptionLocal && i <= _selectedDialogueOption - _selectedDialogueOptionLocal + MaxVisibleDialogueOptions - 1) {
                var option = _currentDialogue.Options[i];
                var brokenText = option.Option.LineBreaks(MainFont, 5, (float) ctx.W);
                ctx.DrawText(brokenText, 0, heightSum - localOffset, 5, _selectedDialogueOption == i ? Color.Black : Color.Gray);
            }
            heightSum += ctx.H / 2;
        }
    }
}