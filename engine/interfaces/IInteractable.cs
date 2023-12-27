using WhiteWorld.engine.ecs;

namespace WhiteWorld.engine.interfaces;

public interface IInteractable {
    void OnInteract(GameObject interactor);
}