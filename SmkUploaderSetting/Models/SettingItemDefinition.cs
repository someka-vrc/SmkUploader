using SmkUploaderSetting.Models.Validation;

namespace SmkUploaderSetting.Models;

/// <summary>
/// �e�ݒ荀�ڂ̒�`����ێ�����N���X
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