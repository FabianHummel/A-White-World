using WhiteWorld.engine.gui;

namespace WhiteWorld.engine.interfaces;

public interface IUpdatable {
    void OnInit();
    void OnTick();
    void OnUpdate();
    void OnGui(GuiContext ctx);
}