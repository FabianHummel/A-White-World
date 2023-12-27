using WhiteWorld.engine.ecs;
using WhiteWorld.game.scripts.shared;

namespace WhiteWorld.engine;

public static partial class Engine {
    public static List<InventoryItem> RegisteredItems { get; } = new() {
        new InventoryItem("Stick", "Just a normal stick", "Item Stick"),
        new InventoryItem("Lemon", "When life gives you lemons", "Item Stick"),
        new InventoryItem("Rose", "A rose with a spark of life", "Item Stick"),
    };

    private static void LoadItems() {
        Engine.LoadTexture("Item Stick", @"assets/images/items/stick.png", persistent: true);
    }
}

public class InventoryItem {
    public string Name { get; }
    public string Description { get; }
    public string Texture { get; }

    public InventoryItem(string name, string description, string texture) {
        this.Name = name;
        this.Description = description;
        this.Texture = texture;
    }
}

public class Item : GameObject {
    public InventoryItem Equivalent;

    public Item(InventoryItem equivalent) {
        this.Equivalent = equivalent;
        AddScript(new SpriteRenderer(Equivalent.Texture));
    }
}