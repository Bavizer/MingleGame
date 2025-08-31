using LabApi.Features.Wrappers;
using MEC;
using MingleGame.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace MingleGame.Core.Components;

[DisallowMultipleComponent]
public class Door : MonoBehaviour, IInteractable
{
    private readonly Quaternion _openedRotation = Quaternion.Euler(0f, 90f, 0f);

    private float _rotationDuration = 1f;

    private string _coroutineTag = string.Empty;

#nullable disable
    public Room Room { get; private set; }
#nullable restore

    public bool IsOpen => transform.localRotation == _openedRotation;
    public bool IsLocked { get; internal set; }


    private void Awake()
    {
        _coroutineTag = GetInstanceID().ToString() + "_RotateDoor";
    }

    public void Interact(Player? sender) => TryRotateDoor(sender);

    public void TryRotateDoor(Player? sender = null)
    {
        if (IsLocked)
        {
            sender?.SendHint(MingleGame.Instance.Config.InfoStrings.LockedDoor, 1.5f);
            return;
        }

        if (Room.HasRequiredPlayersAmount && !IsOpen)
        {
            sender?.SendHint(MingleGame.Instance.Config.InfoStrings.FullRoomDoorInteraction, 1.5f);
            return;
        }

        Timing.RunCoroutineSingleton(RotateDoorCoroutine(), _coroutineTag, SingletonBehavior.Abort);
    }

    internal void Init(Room room) => Room = room;

    private IEnumerator<float> RotateDoorCoroutine()
    {
        if (IsLocked)
            yield break;

        var localTarget = IsOpen ? Quaternion.identity : _openedRotation;
        var initialRotation = transform.localRotation;

        var elapsedTime = 0f;
        while (elapsedTime < _rotationDuration)
        {
            yield return Timing.WaitForOneFrame;
            elapsedTime += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(initialRotation, localTarget, elapsedTime / _rotationDuration);
        }
        transform.localRotation = localTarget;
    }
}
