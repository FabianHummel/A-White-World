using WhiteWorld.engine;

namespace WhiteWorld.engine.scripts; 

[DisallowMultipleInstances, DisallowRemoval]
public class Transform : IGameScript {
    public int X { get; set; }  //   Y ↑   ↗ Z  
    public int Y { get; set; }  //     | ╱      
    public int Z { get; set; }  //     +⎯⎯⎯> X

    public Transform(int x = 0, int y = 0, int z = 0) {
        X = x; Y = y; Z = z;
    }

    public override void OnInit() {
        Console.WriteLine("Transform initialized"); 
    }

    public override void OnUpdate() {
        
    }

    public override void OnTick() {
        
    }
}