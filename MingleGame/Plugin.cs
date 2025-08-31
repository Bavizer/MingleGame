using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using System;
using MingleGame.Tools;

namespace MingleGame;

internal class Plugin : Plugin<Config>
{
#nullable disable
    public static Plugin Instance { get; private set; }
#nullable restore

    public override string Author => "Bavizer";

    public override string Name => "MingleGame";

    public override string Description => "A plugin, that implements the Mingle Game inspired by Netflix series \"Squid Game: Season 2\"";

    public override Version Version => new(1, 0, 0);

    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    public override void Enable()
    {
        Instance = this;

        AudioClipStorage.LoadClip(@Config.AudioPaths.CalmPart, AudioClipNames.CalmPart);
        AudioClipStorage.LoadClip(@Config.AudioPaths.DangerPart, AudioClipNames.DangerPart);
    }

    public override void Disable()
    {
        var @event = Core.MingleGame.Instance;

        if (@event.IsActive)
            @event.EndEvent();
    }
}
