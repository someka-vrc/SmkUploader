namespace SmkUploaderSetting.Models.VersionConversion;

/// <summary>
/// バージョン0からバージョン1への変換クラス
/// </summary>
public class SettingVersionConverter_v0_to_v1 : ISettingVersionConverter
{
    public int FromVersion => 0;
    public int ToVersion => 1;

    public SettingValues Convert(SettingValues oldValues)
    {
        // バージョン0はバージョン1と同じ構造なので、そのまま返す
        // 将来的に項目の追加や変更があった場合はここでマッピングを実装
        return oldValues;
    }
}