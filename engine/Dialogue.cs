using Raylib_CsLo;

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
            this.Title = title;
            this.Text = text;
            this.Options = options;
            this.OnContinue = onContinue;
        }
    }

    private static readonly List<Dialogue> DialogueQueue = new();
    private static Dialogue _currentDialogue = null!;
    private static bool _dialogueOpen;
    private static int _selectedDialogueOption;
    
    private static string _dialogueText = "";
    private static int _dialogueTextFrame = 0;

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
        return _currentDialogue.Options.Length > 0;
    }

    private static void TryNextDialogue() {
        if (!IsDialogueQueueEmpty()) {
            _selectedDialogueOption = 0;
            _currentDialogue = DialogueQueue[0];
            DialogueQueue.RemoveAt(0);
            _dialogueOpen = true;
            _dialogueText = "";
            _dialogueTextFrame = 0;
        }
    }

    private static void NextDialogueOption() {
        _selectedDialogueOption = (_selectedDialogueOption + 1) % _currentDialogue.Options.Length;
        PlayRandomSound("Cycle 1", "Cycle 2");
    }

    private static void PreviousDialogueOption() {
        _selectedDialogueOption = (_selectedDialogueOption - 1) % _currentDialogue.Options.Length;
        if (_selectedDialogueOption < 0) _selectedDialogueOption = _currentDialogue.Options.Length - 1;
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
            DrawDialogue();

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

    private static void DrawDialogue() {

        int centerToLeftAlignX = -_dialogueBoxWidth / 2;
        int endToTopAlignY = -_dialogueBoxHeight;

        if (HasDialogueOptions()) {
            DrawUiImage("Dialogue Box Options", 0, -5, ax: Align.Center, ay: Align.End);
            DrawOptions();
        } else {
            DrawUiImage("Dialogue Box", 0, -5, ax: Align.Center, ay: Align.End);
        }
        
        DrawText();
        DrawTitle();

        void DrawText() {
            DrawUiText(
                _dialogueText,
                centerToLeftAlignX + 7,
                endToTopAlignY + 3,
                5, 1.0f,
                Raylib.DARKGRAY,
                Align.Center,
                Align.End );
        }

        void DrawTitle() {
            var len = GetTextLength(MainFont, _currentDialogue.Title, 5, 1.0f);
            DrawUiRectangle(
                centerToLeftAlignX + 10 - 2,
                endToTopAlignY - 5 - 1,
                (int) len.X + 4,
                (int) len.Y + 2,
                Raylib.DARKGRAY,
                Align.Center,
                Align.End );
            DrawUiRectangle(
                centerToLeftAlignX + 10 - 1,
                endToTopAlignY - 5,
                (int) len.X + 2,
                (int) len.Y,
                Raylib.RAYWHITE,
                Align.Center,
                Align.End );
            DrawUiText(
                _currentDialogue.Title,
                centerToLeftAlignX + 10,
                endToTopAlignY - 5,
                5, 1.0f,
                Raylib.DARKGRAY,
                Align.Center,
                Align.End );
        }

        void DrawOptions() {
            var heightSum = 0;
            for (int i = 0; i < _currentDialogue.Options.Length; i++) {
                var option = _currentDialogue.Options[i];
                var selected = _selectedDialogueOption == i;
                if (selected) {
                    DrawUiText(
                        option.Option,
                        centerToLeftAlignX + 80,
                        endToTopAlignY + 1 + heightSum + i - 1,
                        5, 1.0f,
                        Raylib.DARKGRAY,
                        Align.Center,
                        Align.End );
                } else {
                    DrawUiText(
                        option.Option,
                        centerToLeftAlignX + 80,
                        endToTopAlignY + 1 + heightSum + i,
                        5, 1.0f,
                        Raylib.LIGHTGRAY,
                        Align.Center,
                        Align.End );
                }

                heightSum += (int) GetTextLength(MainFont, option.Option, 5, 1.0f).Y;
            }
        }
    }
}
