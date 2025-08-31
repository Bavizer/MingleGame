using System.ComponentModel;
using UnityEngine;

namespace MingleGame;

internal class Config
{
    private const byte _defaultMaxPlayersAmountPerRoom = 4;

    [Description("Position for game location to spawn.")]
    public Vector3 LocationSpawnPosition { get; set; } = new(15f, 240f, 60f);

    [Description("Max possible players amount to be selected as required amount for each room.")]
    public byte MaxPlayersAmountPerRoom
    {
        get;
        set => field = value == 0 ? _defaultMaxPlayersAmountPerRoom : value;
    } = _defaultMaxPlayersAmountPerRoom;

    [Description("Disable round lock after event ending? (true/false)")]
    public bool DisableRoundLockOnEnd { get; set; } = false;

    public AudioPaths AudioPaths { get; set; } = new();

    public InfoStrings InfoStrings { get; set; } = new();
}

internal struct AudioPaths
{
    [Description("Game's calm part music path (.ogg only)")]
    public string CalmPart { get; set; } = "CalmPart.ogg";

    [Description("Game's danger part music path (.ogg only)")]
    public string DangerPart { get; set; } = "DangerPart.ogg";

    public AudioPaths() { }
}

internal struct InfoStrings
{
    public string DoorInteractionHint { get; set; } = "<b><color=green>Press NoClip key <color=orange>(ALT by default)</color> to open/close doors</color></b>";

    public string FullRoomDoorInteraction { get; set; } = "<b>Room already has <color=orange>required players amount!</color></b>";

    public string LockedDoor { get; set; } = "<b>Door is <color=red>locked!</color></b>";

    public string GameDeathReason { get; set; } = "<b><color=red>Didn't find a room with required players amount</color></b>";

    public string RequiredPlayers { get; set; } = "<color=red><b><size=80><voffset=800>{players} Player(-s)</voffset></size></b></color>";

    public string GameEnd { get; set; } = "<b>Event <color=orange>\"Mingle Game\"</color> has been ended\nWinner is <color=red>{winner}</color></b>";

    public InfoStrings() { }
}