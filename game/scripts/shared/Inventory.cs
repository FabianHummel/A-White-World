using Raylib_CsLo;
using WhiteWorld.engine;
using WhiteWorld.engine.ecs;
using WhiteWorld.engine.interfaces;

namespace WhiteWorld.game.scripts.shared;

public class Inventory : GameScript, ISerializable {
    public List<Item> Content { get; private set; } = new() {
        Engine.RegisteredItems[0],
        Engine.RegisteredItems[1],
        Engine.RegisteredItems[2],
    };

    public bool IsOpen { get; private set; }

    private int _selected;
    public Item SelectedItem => Content[_selected];

    private float _opacity;
    private Shader _blurShader;

    private int _blurShaderRadius;
    private int _blurShaderWidth;
    private int _blurShaderHeight;

    public override void OnInit() {
        Engine.LoadTexture("Inventory", @"assets/images/inventory.png");
        _blurShader = Engine.GetShader("Blur");
        _blurShaderRadius = Raylib.GetShaderLocation(_blurShader, "radius");
        _blurShaderWidth = Raylib.GetShaderLocation(_blurShader, "width");
        _blurShaderHeight = Raylib.GetShaderLocation(_blurShader, "height");

        Engine.LoadAnimation("Inventory Seaman Idle", @"assets/images/seaman/seaman-idle.gif", 10);
        Engine.StartAnimation("Inventory Seaman Idle");

        // TODO: Load keybind images separately (persistently)
        Engine.LoadTexture("Inventory Keybind Use", @"assets/images/keybinds/enter.png");
        Engine.LoadTexture("Inventory Keybind Drop", @"assets/images/keybinds/g.png");
        Engine.LoadTexture("Inventory Keybind Navigate Up", @"assets/images/keybinds/arrow-up.png");
        Engine.LoadTexture("Inventory Keybind Navigate Down", @"assets/images/keybinds/arrow-down.png");
    }

    public override void OnUpdate() {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_I))
            IsOpen = !IsOpen;

        if (IsOpen) {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
                UseSelectedItem();

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_G))
                DropSelectedItem();
        }
    }

    private void NextItem() {
        _selected = (_selected + 1) % Content.Count;
        Engine.PlayRandomSound("Cycle 1", "Cycle 2");
    }

    private void PreviousItem() {
        _selected = (_selected - 1) % Content.Count;
        if (_selected < 0) _selected = Content.Count - 1;
        Engine.PlayRandomSound("Cycle 1", "Cycle 2");
    }

    private void UseSelectedItem() {
        Engine.Success($"Used item {SelectedItem.Name}!");
    }

    private void DropSelectedItem() {
        Engine.Success($"Dropped item {SelectedItem.Name}!");
    }

    /* Serialization */
    public string Identifier => "Inventory";

    public void Save(DataBuffer buffer) {
        buffer.Add(Content);
    }

    public void Load(DataBuffer? buffer) {
        if (buffer != null) {
            Content = buffer.ReadList<Item>();
        }
    }

    /* Graphical interface */
    public override void OnGui() {
        DrawBackground();

        if (IsOpen) {
            DrawInventory();

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
                NextItem();

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
                PreviousItem();
        }
    }

    /* private float EaseOutQuart(float t) {
        return 1 - MathF.Pow(1 - t, 4);
    }*/

    private void DrawBackground() {
        _opacity = IsOpen ?
            MathF.Min(_opacity + Engine.DeltaTime * 5, 1.0f):
            MathF.Max(_opacity - Engine.DeltaTime * 5, 0.0f);

        var backgroundColor = Raylib.Fade(Raylib.BLACK, _opacity * 0.8f);
        Engine.DrawUiRectangle(0, 0, Engine.CanvasWidth +1, Engine.CanvasHeight +1, backgroundColor);

        // Background Blur
        Raylib.SetShaderValue(_blurShader, _blurShaderWidth, Engine.WindowWidth, ShaderUniformDataType.SHADER_UNIFORM_INT);
        Raylib.SetShaderValue(_blurShader, _blurShaderHeight, Engine.WindowHeight, ShaderUniformDataType.SHADER_UNIFORM_INT);
        Raylib.SetShaderValue(_blurShader, _blurShaderRadius, _opacity * 2, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
    }

    private void DrawInventory() {
        for (int i = 0; i < Content.Count; i++) {
            var item = Content[i];
            DrawItem(item, i);
        }

        /* Player preview */
        Engine.DrawUiRectangleCentered(30, 0, 20, 25, Raylib.LIGHTGRAY, Engine.Align.Center, Engine.Align.Center);
        Engine.DrawUiRectangleCentered(30, 0, 18, 23, Raylib.RAYWHITE,  Engine.Align.Center, Engine.Align.Center);
        Engine.DrawUiAnimation("Inventory Seaman Idle", 30, 0,          Engine.Align.Center, Engine.Align.Center);

        /* Item description */
        var description = $"{SelectedItem.Name}:\n{SelectedItem.Description}";
        Engine.DrawUiTextCentered(description, 0, -2, 5, 1f, Raylib.RAYWHITE, Engine.Align.Center, Engine.Align.End);

        /* Keybinds */
        Engine.DrawUiImage("Inventory Keybind Use", 2, -2, .5f,              Engine.Align.Start, Engine.Align.End);
        Engine.DrawUiTextCentered("Use", 8, -2, 5, 1f, Raylib.RAYWHITE,      Engine.Align.Start, Engine.Align.End);

        Engine.DrawUiImage("Inventory Keybind Drop", 2, -8, .5f,            Engine.Align.Start, Engine.Align.End);
        Engine.DrawUiTextCentered("Drop", 8, -8, 5, 1f, Raylib.RAYWHITE,    Engine.Align.Start, Engine.Align.End);

        Engine.DrawUiImage("Inventory Keybind Navigate Down", -2, -2, .5f,      Engine.Align.End, Engine.Align.End);
        Engine.DrawUiTextCentered("Navigate", -8, -2, 5, 1f, Raylib.RAYWHITE, Engine.Align.End, Engine.Align.End);

        Engine.DrawUiImage("Inventory Keybind Navigate Up", -2, -8, .5f,   Engine.Align.End, Engine.Align.End);
//        Engine.DrawUiTextCentered("", 8, -12, 5, 1f, Raylib.RAYWHITE,   Engine.Align.End, Engine.Align.End);
    }

    private void DrawItem(Item item, int index) {
        const int offset = 13;

        var y = index * offset - (Content.Count / 2) * offset;
        if (_selected == index) {
            Engine.DrawUiImage(item.Texture, -36, y -1, 1.1f, Engine.Align.Center, Engine.Align.Center);
            Engine.DrawUiText(item.Name, -25, y -4, 7, 1f, Raylib.RAYWHITE, Engine.Align.Center, Engine.Align.Center);
        } else {
            Engine.DrawUiImage(item.Texture, -35, y, 0.8f, Engine.Align.Center, Engine.Align.Center);
            Engine.DrawUiText(item.Name, -25, y -3, 5, 1f, Raylib.LIGHTGRAY, Engine.Align.Center, Engine.Align.Center);
        }
    }
}