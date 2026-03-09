using LabApi.Features.Wrappers;
using System.Collections.Generic;
using UnityEngine;

namespace MingleGame.Core.Components;

[DisallowMultipleComponent]
public class Room : MonoBehaviour
{
#nullable disable

    private Door _door;
    private RoomTrigger _roomTrigger;

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
        _roomTrigger = transform.Find("Trigger").gameObject.AddComponent<RoomTrigger>();
        _door = transform.Find("Door").gameObject.AddComponent<Door>();

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
}
