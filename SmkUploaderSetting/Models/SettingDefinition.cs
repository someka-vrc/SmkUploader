namespace SmkUploaderSetting.Models;

/// <summary>
/// �ݒ�S�̂̒�`����ێ�����N���X
/// </summary>
public class SettingDefinition
{
    public int Version { get; set; }
    public List<SettingItemDefinition> Items { get; set; } = new();
}