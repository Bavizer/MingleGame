using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MingleGame.Interfaces;
using UnityEngine;

namespace MingleGame.Core;

internal class EventsHandler : CustomEventsHandler
{
    private static readonly int _defaultLayerMask = LayerMask.GetMask("Default");

    private static MingleGame Event => MingleGame.Instance;

    public override void OnServerRoundRestarted()
    {
        if (Event.IsActive)
            Event.EndEvent();
    }

    public override void OnPlayerTogglingNoclip(PlayerTogglingNoclipEventArgs ev)
        => Interact(ev.Player);

    public override void OnPlayerLeft(PlayerLeftEventArgs ev)
        => MingleGame.Instance.OnPlayerLeft(ev.Player);

    private void Interact(Player player)
    {
        if (Physics.Raycast(player.Camera.position, player.Camera.forward, out var hit, 2f, _defaultLayerMask))
        {
            var component = hit.collider.GetComponentInParent<IInteractable>();
            if (component == null)
                return;

            component.Interact(player);
        }
    }
}
