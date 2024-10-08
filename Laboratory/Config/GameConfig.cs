namespace Laboratory.Config;

public static class GameConfig
{
    public static bool DisableEndGame { get; set; } = true;
    public static bool DisableMeetings { get; set; } = true;
    public static bool DisableKillButton { get; set; } = true;
    public static bool DisableVentButton { get; set; } = true;
    public static bool DisableSabotageButton { get; set; } = true;
    public static bool DisableReportButton { get; set; } = true;
    public static bool DisableTaskArrows { get; set; } = true;
    public static bool DisableTaskPanel { get; set; } = true;
    public static bool DisableIntroCutscene { get; set; } = true;
    public static bool DisableVents { get; set; } = true;

    private static bool _customSaveData = true;

    public static bool CustomSaveData
    {
        get => _customSaveData;
        // ReSharper disable once UnusedMember.Global
        set
        {
            Message("Setting CustomSaveData to " + value);
            _customSaveData = value;
        }
    }
}
