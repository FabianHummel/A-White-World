namespace WhiteWorld.engine;

public static partial class Engine {
    public static List<Item> RegisteredItems { get; } = new() {
        new Item("Stick", "Just a normal stick", "Item Stick"),
        new Item("Lemon", "When life gives you lemons", "Item Stick"),
        new Item("Rose", "A rose with a spark of life", "Item Stick"),
    };

    private static void LoadItems() {
        Engine.LoadTexture("Item Stick", @"assets/images/items/stick.png", persistent: true);
    }
}

public class Item {
    public string Name { get; }
    public string Description { get; }
    public string Texture { get; }

    public Item(string name, string description, string texture) {
        this.Name = name;
        this.Description = description;
        this.Texture = texture;
    }
}