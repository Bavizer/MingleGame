using LabApi.Features.Wrappers;
using System.Collections.Generic;
using UnityEngine;

namespace MingleGame.Core.Components;

[DisallowMultipleComponent]
public class Room : MonoBehaviour
{
#nullable disable

    private Door _door;

    private BoxCollider _boxCollider;

    internal HashSet<Player> PlayersInRoom { get; } = [];

    public LightSourceToy Light { get; private set; }

    public bool HasRequiredPlayersAmount => PlayersInRoom.Count == MingleGame.Instance.RequiredPlayersInRoom;

    public bool IsDoorLocked
    {
        get => _door.IsLocked;
        set => _door.IsLocked = value;
    }

#nullable restore

    private void Awake()
    {
        var trigger = transform.Find("Trigger");

        _boxCollider = gameObject.AddComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
        _boxCollider.size = trigger.localScale;

        _door = transform.Find("Door").gameObject.AddComponent<Door>();
        _door.Init(this);

        Light = GetComponentInChildren<LightSourceToy>();
    }

    public void OpenDoor()
    {
        if (!_door.IsOpen)
            _door.TryRotateDoor();
    }

    public void CloseDoor()
    {
        if (_door.IsOpen)
            _door.TryRotateDoor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Player.TryGet(other.gameObject, out var player))
            PlayersInRoom.Add(player);
    }

    private void OnTriggerExit(Collider other)
    {
        if (Player.TryGet(other.gameObject, out var player))
            PlayersInRoom.Remove(player);
    }
}
