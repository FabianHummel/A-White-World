using Raylib_CsLo;
using WhiteWorld.utility;

namespace WhiteWorld.engine;

public static partial class Engine {

    public class DialogueOption {
        public string Title { get; }
        public string Text { get; }
        public Action OnSelect { get; }

        public DialogueOption(string title, string text, Action onSelect) {
            Title = title;
            Text = text;
            OnSelect = onSelect;
        }
    }

    private class Dialogue {
        public string Title { get; }
        public string Text { get; }
        public DialogueOption[] Options { get; }
        public Action? OnContinue { get; }

        public Dialogue(string title, string text, DialogueOption[] options, Action? onContinue) {
            Title = title;
            Text = text;
            Options = options;
            OnContinue = onContinue;
        }
    }

    private static readonly List<Dialogue> DialogueQueue = new();
    private static Dialogue? _currentDialogue;
    private static bool _dialogueOpen;
    private static int _selectedDialogueOption;
    
    private static string _dialogueText = "";
    private static int _dialogueTextFrame = 0;

    public static void QueueDialogue(string title, string text, DialogueOption[] options) {
        DialogueQueue.Add(new Dialogue(title, text, options, null));
    }
    
    public static void QueueDialogue(string title, string text, Action? onContinue = null) {
        DialogueQueue.Add(new Dialogue(title, text, Array.Empty<DialogueOption>(), onContinue));
    }
    
    private static bool IsDialogueComplete() {
        return _dialogueTextFrame >= _currentDialogue!.Text.Length;
    }
    
    private static bool IsDialogueOpen() {
        return _dialogueOpen;
    }
    
    private static bool IsDialogueQueueEmpty() {
        return DialogueQueue.Count == 0;
    }
    
    private static bool HasDialogueOptions() {
        return _currentDialogue!.Options.Length > 0;
    }

    private static void TryNextDialogue() {
        if (!IsDialogueQueueEmpty()) {
            _currentDialogue = DialogueQueue[0];
            DialogueQueue.RemoveAt(0);
            _dialogueOpen = true;
            _dialogueText = "";
            _dialogueTextFrame = 0;
        }
    }

    private static void NextDialogueOption() {
        _selectedDialogueOption = (_selectedDialogueOption + 1) % _currentDialogue!.Options.Length;
    }

    private static void PreviousDialogueOption() {
        _selectedDialogueOption = (_selectedDialogueOption - 1) % _currentDialogue!.Options.Length;
    }

    private static void ContinueDialogue() {
        if (IsDialogueComplete()) {
            if (HasDialogueOptions()) 
                _currentDialogue!.Options[_selectedDialogueOption].OnSelect();
            else
                _currentDialogue!.OnContinue?.Invoke();

            if (IsDialogueQueueEmpty()) {
                _dialogueOpen = false;
                PlaySound("Dialogue Pop 1", "Dialogue Pop 2");
            }
            else {
                TryNextDialogue();
            }
        }
        else {
            _dialogueTextFrame = _currentDialogue!.Text.Length; // If the dialogue isn't complete, finish it
            PlaySound("Dialogue Pop 1", "Dialogue Pop 2");
        }
    }

    private static void UpdateDialogue() {
        if (_dialogueOpen) {
            DrawDialogue();
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) {
                NextDialogueOption();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) {
                PreviousDialogueOption();
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
        OnTick += TickDialogue;
        var box = LoadTexture("Dialogue Box", @"assets/images/dialogue-box.png", persistent: true);
        _dialogueBoxWidth = box.width;
        _dialogueBoxHeight = box.height;
        LoadSound("Dialogue Text 1", @"assets/sounds/text/text-1.wav", persistent: true);
        LoadSound("Dialogue Text 2", @"assets/sounds/text/text-2.wav", persistent: true);
        LoadSound("Dialogue Text 3", @"assets/sounds/text/text-3.wav", persistent: true);
        LoadSound("Dialogue Pop 1", @"assets/sounds/dialogue/dialogue-pop-1.wav", persistent: true);
        LoadSound("Dialogue Pop 2", @"assets/sounds/dialogue/dialogue-pop-2.wav", persistent: true);
    }

    private static void TickDialogue() {
        if (IsDialogueOpen() && Frame % 2 == 0) {
            if (!IsDialogueComplete()) {
                _dialogueTextFrame++;
                PlaySound("Dialogue Text 1", "Dialogue Text 2", "Dialogue Text 3");
            }
            _dialogueText = _currentDialogue!.Text[.._dialogueTextFrame];
        }
    }

    private static void DrawDialogue() {
        DrawImage("Dialogue Box", 0, -5, Align.Center, Align.End);
        
        DrawText(
            _dialogueText,
            -_dialogueBoxWidth / 2 + 7,
            -_dialogueBoxHeight + 2,
            28,
            Raylib.DARKGRAY,
            Align.Center,
            Align.End
        );
        
        var len = GetTextLength(TitleFont, _currentDialogue!.Title, 28);
        DrawRectangle(
            -_dialogueBoxWidth / 2 + 10 - 2,
            -_dialogueBoxHeight - 5,
            (int) len.X / PixelSize + 4,
            (int) len.Y / PixelSize,
            Raylib.RAYWHITE,
            Align.Center,
            Align.End
        );
        
        DrawText(
            _currentDialogue!.Title,
            -_dialogueBoxWidth / 2 + 10,
            -_dialogueBoxHeight - 5,
            28,
            Raylib.DARKGRAY,
            Align.Center,
            Align.End
        );
    }
}