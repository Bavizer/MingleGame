namespace MingleGame.Other;

internal readonly struct Hints
{
    internal static readonly string contactInfo = $"<voffset=-100><pos=-500><b>To make a <color=green><u>suggestion</u></color> or report a <color=red><u>bug</u></color>, contact the author:</voffset>"
        + $"<voffset=-140><pos=-150>Discord: <color=purple>{Plugin.Instance.DiscordContactUsername}</color></pos></voffset>"
        + $"<voffset=-180><pos=-450>GitHub: <color=purple><u>{Plugin.GITHUB_REPOSITORY_LINK}</u></color></pos></voffset>"
        + "<voffset=0><pos=0>ㅤ</pos></voffset>"; // Normalize the text position

    public Hints() { }
}
