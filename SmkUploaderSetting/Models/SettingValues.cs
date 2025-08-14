using System.IO;
using SmkUploaderSetting.Models.VersionConversion;

namespace SmkUploaderSetting.Models;

/// <summary>
/// ���ۂ̐ݒ�l��ێ��E���삷��N���X
/// </summary>
public class SettingValues
{
    public Dictionary<string, object?> Values { get; set; } = new();

    /// <summary>
    /// ini�t�@�C������ݒ�l��ǂݍ���
    /// </summary>
    public static SettingValues LoadFromIni(string path, SettingDefinition definition)
    {
        var values = new SettingValues();
        
        // �t�@�C�������݂��Ȃ��ꍇ�̓f�t�H���g�l�ŏ�����
        if (!File.Exists(path))
        {
            values.ResetToDefault(definition);
            return values;
        }

        try
        {
            var lines = File.ReadAllLines(path);
            var fileValues = new Dictionary<string, string>();
            var fileVersion = 0; // �f�t�H���g�̓o�[�W����0�i���`���j

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                // �R�����g�s���s�͖���
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";"))
                    continue;

                var equalIndex = trimmedLine.IndexOf('=');
                if (equalIndex > 0)
                {
                    var key = trimmedLine.Substring(0, equalIndex).Trim();
                    var value = trimmedLine.Substring(equalIndex + 1).Trim();
                    
                    // �o�[�W���������`�F�b�N
                    if (key == "SKMU_SETTINGS_VERSION")
                    {
                        if (int.TryParse(value, out var version))
                        {
                            fileVersion = version;
                        }
                    }
                    else
                    {
                        fileValues[key] = value;
                    }
                }
            }

            // ��`�Ɋ�Â��Ēl��ݒ�
            foreach (var item in definition.Items)
            {
                if (fileValues.TryGetValue(item.Key, out var stringValue))
                {
                    try
                    {
                        var convertedValue = ConvertValue(stringValue, item.Type);
                        values.Values[item.Key] = convertedValue;
                    }
                    catch
                    {
                        // �ϊ��Ɏ��s�����ꍇ�̓f�t�H���g�l���g�p
                        values.Values[item.Key] = item.DefaultValue;
                    }
                }
                else
                {
                    // �t�@�C���ɍ��ڂ����݂��Ȃ��ꍇ�̓f�t�H���g�l���g�p
                    values.Values[item.Key] = item.DefaultValue;
                }
            }

            // �o�[�W�����ϊ����K�v�ȏꍇ�͎��s
            if (fileVersion < definition.Version)
            {
                if (VersionConversionManager.CanConvert(fileVersion, definition.Version))
                {
                    values = VersionConversionManager.ConvertToVersion(values, fileVersion, definition.Version);
                }
                else
                {
                    // �ϊ��ł��Ȃ��ꍇ�̓f�t�H���g�l�ŏ�����
                    values.ResetToDefault(definition);
                }
            }
        }
        catch
        {
            // �t�@�C���ǂݍ��݂Ɏ��s�����ꍇ�̓f�t�H���g�l�ŏ�����
            values.ResetToDefault(definition);
        }

        return values;
    }

    /// <summary>
    /// �ݒ�l��ini�t�@�C���ɕۑ�����
    /// </summary>
    public void SaveToIni(string path, SettingDefinition definition)
    {
        var lines = new List<string>();
        
        // �o�[�W��������擪�ɒǉ�
        lines.Add($"SKMU_SETTINGS_VERSION={definition.Version}");
        lines.Add(""); // ��s
        
        foreach (var item in definition.Items)
        {
            if (Values.TryGetValue(item.Key, out var value))
            {
                lines.Add($"{item.Key}={value}");
            }
        }

        File.WriteAllLines(path, lines);
    }

    /// <summary>
    /// �f�t�H���g�l�Ƀ��Z�b�g
    /// </summary>
    public void ResetToDefault(SettingDefinition definition)
    {
        Values.Clear();
        foreach (var item in definition.Items)
        {
            Values[item.Key] = item.DefaultValue;
        }
    }

    /// <summary>
    /// ������l���w�肳�ꂽ�^�ɕϊ�
    /// </summary>
    private static object ConvertValue(string stringValue, SettingValueType type)
    {
        return type switch
        {
            SettingValueType.Int => int.Parse(stringValue),
            SettingValueType.Double => double.Parse(stringValue),
            SettingValueType.Bool => bool.Parse(stringValue),
            SettingValueType.String => stringValue,
            _ => stringValue
        };
    }
}