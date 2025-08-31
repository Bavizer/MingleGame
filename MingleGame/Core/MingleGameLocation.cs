using AdminToys;
using LabApi.Features.Wrappers;
using MingleGame.Tools;
using Mirror;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using System.Collections.Generic;
using UnityEngine;

namespace MingleGame.Core;

internal class MingleGameLocation
{
    #region Fields

    private readonly SchematicObject _location;
    private readonly Transform _main;

    private readonly List<Room> _rooms = [];

    private readonly PrimitiveObjectToy[] _platformColliders;

    private readonly LightSourceToy[] _allLights;
    private readonly LightSourceToy[] _topLights;
    private readonly Color _defaultLightsColor = new(1f, 1f, 0.3f);
    private readonly Color[] _dangerPartColors = [Color.cyan, Color.green, Color.red, Color.magenta, Color.blue, Color.yellow];

    private readonly AudioPlayer _audioPlayer;

    internal readonly Transform playerSpawnPoint;

    internal readonly List<Room> safeRooms = [];

    public IReadOnlyCollection<Room> rooms;

    #endregion

    #region Methods

    internal MingleGameLocation(Vector3 spawnPosition, Quaternion rotation)
    {
        _location = ObjectSpawner.SpawnSchematic("MingleGame", spawnPosition, rotation);

        _main = _location.transform.Find("Main");
        _allLights = _main.GetComponentsInChildren<LightSourceToy>();
        _topLights = _main.Find("Top").GetComponentsInChildren<LightSourceToy>();
        playerSpawnPoint = _main.Find("SpawnPoint");

        _platformColliders = _location.transform.Find("Center/PlatformColliders").GetComponentsInChildren<PrimitiveObjectToy>();

        var roomsParent = _main.Find("Middle/Rooms");

        foreach (Transform room in roomsParent.transform)
        {
            if (room.name.StartsWith("Room"))
                _rooms.Add(room.gameObject.AddComponent<Room>());
        }

        rooms = _rooms.ToArray();

        _audioPlayer = AudioPlayer.CreateOrGet("MingleGameAudioPlayer", onIntialCreation: obj =>
        {
            obj.AddSpeaker("MingleGameSpeaker", isSpatial: false, maxDistance: 50f);
            obj.SetSpeakerPosition("MingleGameSpeaker", _location.transform.position);
            obj.transform.parent = _location.transform;
        });
    }

    internal void Destroy() => _location.Destroy();

    internal void OnStart() => _allLights.ForEach(l => l.NetworkLightIntensity = 0f);

    internal void OnStartingCalmPart(float duration)
    {
        _audioPlayer.RemoveAllClips();
        _audioPlayer.AddClip(AudioClipNames.CalmPart);

        _platformColliders.ForEach(o => o.NetworkPrimitiveFlags = PrimitiveFlags.Collidable);

        foreach (var room in rooms)
        {
            room.CloseDoor();
            room.IsDoorLocked = true;
        }

        foreach (var ragdoll in Ragdoll.List)
            NetworkServer.Destroy(ragdoll.Base.gameObject);

        foreach (var pickup in Pickup.List)
            NetworkServer.Destroy(pickup.Base.gameObject);

        LocationTools.Rotate(_main, Vector3.up * 15f, duration);
        _allLights.ForEach(l => LocationTools.SetLightIntensity(l, 25f, 5f));
    }

    internal void OnStartingDangerPart(float duration)
    {
        _audioPlayer.RemoveAllClips();
        _audioPlayer.AddClip(AudioClipNames.DangerPart);

        _platformColliders.ForEach(o => o.NetworkPrimitiveFlags = PrimitiveFlags.None);

        foreach (var room in rooms)
            room.Light.NetworkLightIntensity = 0f;

        foreach (var room in safeRooms)
        {
            room.IsDoorLocked = false;
            room.OpenDoor();
            room.Light.NetworkLightIntensity = 25f;
        }

        _topLights.ForEach(l => LocationTools.SwitchLightColors(l, _dangerPartColors, 0.15f, duration));
    }

    internal void OnEndingGameRound()
    {
        _topLights.ForEach(l => l.NetworkLightColor = _defaultLightsColor);
        _allLights.ForEach(l => LocationTools.SetLightIntensity(l, 0f, 2f));

        _audioPlayer.RemoveAllClips();

        safeRooms.Clear();
    }

    #endregion
}
