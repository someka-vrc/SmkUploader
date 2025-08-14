namespace SmkUploaderSetting.Models;

/// <summary>
/// 設定全体の定義情報を保持するクラス
/// </summary>
public class SettingDefinition
{
    public int Version { get; set; }
    public List<SettingItemDefinition> Items { get; set; } = new();
}