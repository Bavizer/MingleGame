using LabApi.Features.Wrappers;

namespace MingleGame.Interfaces;

public interface IInteractable
{
    public void Interact(Player? sender);
}
