using SmkUploaderSetting.Models.Validation;

namespace SmkUploaderSetting.Models;

/// <summary>
/// 各設定項目の定義情報を保持するクラス
/// </summary>
public class SettingItemDefinition
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SettingValueType Type { get; set; }
    public object DefaultValue { get; set; } = new();
    public List<IValidationRule> ValidationRules { get; set; } = new();
}