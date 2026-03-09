using LabApi.Features.Wrappers;
using UnityEngine;

namespace MingleGame.Core.Components;

[DisallowMultipleComponent]
public class RoomTrigger : MonoBehaviour
{
#nullable disable
    private BoxCollider _boxCollider;

    public Room Room { get; private set; }
#nullable restore

    private void Awake()
    {
        _boxCollider = gameObject.AddComponent<BoxCollider>();
        _boxCollider.isTrigger = true;

        Room = GetComponentInParent<Room>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Player.TryGet(other.gameObject, out var player))
            Room.PlayersInRoom.Add(player);
    }

    private void OnTriggerExit(Collider other)
    {
        if (Player.TryGet(other.gameObject, out var player))
            Room.PlayersInRoom.Remove(player);
    }
}
