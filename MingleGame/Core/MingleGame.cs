using CustomPlayerEffects;
using CustomRendering;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MEC;
using MingleGame.Tools;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace MingleGame.Core;

public sealed class MingleGame
{
    #region Fields

    private readonly CustomEventsHandler _eventsHandler = new EventsHandler();

    private readonly HashSet<Player> _lastRoundSurvivors = [];

    private CoroutineHandle _eventCoroutine;
    private CoroutineHandle _checkCoroutine;

    private MingleGameLocation? _gameLocation;

    public const RoleTypeId playersRoleType = RoleTypeId.ClassD;
    public const ushort minimumPlayersRequired = 2;

    #endregion

    #region Properties

    internal Config Config => Plugin.Instance.Config!;

    public static MingleGame Instance => field ??= new MingleGame();

    public float CalmPartDuration { get; private set; }

    public float DangerPartDuration
    {
        get
        {
            if (field <= 0f)
            {
                var clip = AudioClipStorage.AudioClips[AudioClipNames.DangerPart];
                field = clip.Samples.Length / (float)clip.SampleRate;
            }
            return field;
        }
    }

    public ushort CurrentRound { get; private set; }

    public int RequiredPlayersInRoom { get; private set; }

    public bool IsActive => _eventCoroutine.IsRunning || _checkCoroutine.IsRunning;

    public bool AreEndConditionsCompleted => Players.Count() < minimumPlayersRequired;

    public IEnumerable<Player> Players => Player.List.Where(p => p.Role == playersRoleType).ToArray();

    #endregion

    #region Methods

    private MingleGame() { }

    public void StartEvent()
    {
        var name = nameof(MingleGame);

        if (!AudioClipStorage.AudioClips.ContainsKey(AudioClipNames.CalmPart) || !AudioClipStorage.AudioClips.ContainsKey(AudioClipNames.DangerPart))
            throw new FileNotFoundException($"Couldn't start {name}: required audio clip(-s) is not loaded. Specify path in the config file.");

        if (IsActive)
            throw new InvalidOperationException($"Couldn't start {name}: it's already active.");

        if (Players.Count() < minimumPlayersRequired)
            throw new InvalidOperationException($"Minimum {minimumPlayersRequired} players as {playersRoleType} required to start {name}.");

        if (!Round.IsRoundInProgress)
            throw new InvalidOperationException($"Couldn't start {name}: round has to be started.");

        try
        {
            Initialize();

            Round.IsLocked = true;

            _eventCoroutine = Timing.RunCoroutine(RunGame());
            _checkCoroutine = Timing.RunCoroutine(UpdatePlayers());

            Logger.Info("Event started.");
        }
        catch
        {
            Clear();
            throw;
        }
    }

    public void EndEvent()
    {
        if (!IsActive)
            throw new InvalidOperationException($"Couldn't end {nameof(MingleGame)}: it's not active.");

        try
        {
            OnEnd();
        }
        finally
        {
            Clear();
            Logger.Info("Event ended.");
        }
    }

    internal void OnPlayerLeft(Player player)
    {
        foreach (var room in _gameLocation!.rooms)
            room.PlayersInRoom.Remove(player);
    }

    private void Initialize()
    {
        _gameLocation = new MingleGameLocation(Config.LocationSpawnPosition, Quaternion.identity);
        _gameLocation.OnStart();

        CustomHandlersManager.RegisterEventsHandler(_eventsHandler);
    }

    private void Clear()
    {
        CustomHandlersManager.UnregisterEventsHandler(_eventsHandler);
        Timing.KillCoroutines(_eventCoroutine, _checkCoroutine);

        Round.IsLocked = !Config.DisableRoundLockOnEnd;

        _gameLocation?.Destroy();
        _gameLocation = null;

        _lastRoundSurvivors.Clear();
    }

    private IEnumerator<float> RunGame()
    {
        yield return Timing.WaitForOneFrame;

        foreach (var player in Players)
            player.SendHint(Config.InfoStrings.DoorInteractionHint, 10f);

        while (!AreEndConditionsCompleted)
        {
            OnStartingGameRound();
            yield return Timing.WaitForSeconds(1f);

            OnStartingCalmPart();
            yield return Timing.WaitForSeconds(CalmPartDuration);

            OnStartingDangerPart();
            yield return Timing.WaitForSeconds(DangerPartDuration);

            OnEndingGameRound();
            yield return Timing.WaitForSeconds(2.5f);
        }

        EndEvent();
    }

    private IEnumerator<float> UpdatePlayers()
    {
        yield return Timing.WaitForOneFrame;

        while (!AreEndConditionsCompleted)
        {
            foreach (var player in Player.ReadyList)
                player.GetEffect<FogControl>()?.SetFogType(FogType.None);

            yield return Timing.WaitForSeconds(1f);
        }
    }

    private void OnStartingGameRound()
    {
        CurrentRound++;

        foreach (var player in Players)
            player.Position = _gameLocation!.playerSpawnPoint.position;
    }

    private void OnStartingCalmPart()
    {
        var clip = AudioClipStorage.AudioClips[AudioClipNames.CalmPart];
        var clipDuration = clip.Samples.Length / (float)clip.SampleRate;
        CalmPartDuration = Random.Range(clipDuration / 2, clipDuration);

        _gameLocation!.OnStartingCalmPart(CalmPartDuration);
    }

    private void OnStartingDangerPart()
    {
        SetRandomRequiredPlayersAmountInRoom();

        var message = Config.InfoStrings.RequiredPlayers.Replace("{players}", RequiredPlayersInRoom.ToString());

        foreach (var player in Player.ReadyList)
            player.SendHint(message, 5);

        _gameLocation!.OnStartingDangerPart(DangerPartDuration);
    }

    private void OnEndingGameRound()
    {
        _lastRoundSurvivors.Clear();

        foreach (var room in _gameLocation!.safeRooms)
        {
            if (room.HasRequiredPlayersAmount)
                room.PlayersInRoom.ForEach(p => _lastRoundSurvivors.Add(p));
        }

        foreach (var player in Players)
        {
            if (!_lastRoundSurvivors.Contains(player))
                player.Kill(Config.InfoStrings.GameDeathReason);
        }

        RequiredPlayersInRoom = 0;
        _gameLocation!.OnEndingGameRound();
    }

    private void SetRandomRequiredPlayersAmountInRoom()
    {
        var playersAmount = Players.Count();
        var maxPlayersInRoom = playersAmount > Config.MaxPlayersAmountPerRoom 
                                ? Config.MaxPlayersAmountPerRoom 
                                : playersAmount - 1;

        RequiredPlayersInRoom = maxPlayersInRoom == 1 ? maxPlayersInRoom : Random.Range(1, maxPlayersInRoom + 1);

        var safeRoomsAmount = playersAmount / RequiredPlayersInRoom;

        if (safeRoomsAmount > _gameLocation!.rooms.Count)
            safeRoomsAmount = _gameLocation.rooms.Count;

        if (safeRoomsAmount > 1)
            safeRoomsAmount -= 1;

        var rooms = _gameLocation!.rooms.ToList();
        for (int i = 0; i < safeRoomsAmount; i++)
        {
            var randomRoom = rooms.PullRandomItem();
            _gameLocation!.safeRooms.Add(randomRoom);
        }
    }

    private void OnEnd()
    {
        var winner = Players.Count() <= 1 ? Players.SingleOrDefault() : null;

        foreach (var player in Player.ReadyList)
        {
            if (!player.IsOverwatchEnabled)
                player.SetRole(RoleTypeId.Spectator);
        }
        winner?.SetRole(RoleTypeId.Tutorial);

        var winnerString = winner == null ? "undefined" : winner.Nickname;
        var gameEndMessage = Config.InfoStrings.GameEnd.Replace("{winner}", winnerString);

        Server.SendBroadcast(gameEndMessage, 10, shouldClearPrevious: true);
        Logger.Info($"Winner: {winnerString}");
    }

    #endregion
}