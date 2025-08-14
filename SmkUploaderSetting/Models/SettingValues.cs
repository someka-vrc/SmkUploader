using System.IO;
using SmkUploaderSetting.Models.VersionConversion;

namespace SmkUploaderSetting.Models;

/// <summary>
/// 実際の設定値を保持・操作するクラス
/// </summary>
public class SettingValues
{
    public Dictionary<string, object?> Values { get; set; } = new();

    /// <summary>
    /// iniファイルから設定値を読み込む
    /// </summary>
    public static SettingValues LoadFromIni(string path, SettingDefinition definition)
    {
        var values = new SettingValues();
        
        // ファイルが存在しない場合はデフォルト値で初期化
        if (!File.Exists(path))
        {
            values.ResetToDefault(definition);
            return values;
        }

        try
        {
            var lines = File.ReadAllLines(path);
            var fileValues = new Dictionary<string, string>();
            var fileVersion = 0; // デフォルトはバージョン0（旧形式）

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                // コメント行や空行は無視
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";"))
                    continue;

                var equalIndex = trimmedLine.IndexOf('=');
                if (equalIndex > 0)
                {
                    var key = trimmedLine.Substring(0, equalIndex).Trim();
                    var value = trimmedLine.Substring(equalIndex + 1).Trim();
                    
                    // バージョン情報をチェック
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

            // 定義に基づいて値を設定
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
                        // 変換に失敗した場合はデフォルト値を使用
                        values.Values[item.Key] = item.DefaultValue;
                    }
                }
                else
                {
                    // ファイルに項目が存在しない場合はデフォルト値を使用
                    values.Values[item.Key] = item.DefaultValue;
                }
            }

            // バージョン変換が必要な場合は実行
            if (fileVersion < definition.Version)
            {
                if (VersionConversionManager.CanConvert(fileVersion, definition.Version))
                {
                    values = VersionConversionManager.ConvertToVersion(values, fileVersion, definition.Version);
                }
                else
                {
                    // 変換できない場合はデフォルト値で初期化
                    values.ResetToDefault(definition);
                }
            }
        }
        catch
        {
            // ファイル読み込みに失敗した場合はデフォルト値で初期化
            values.ResetToDefault(definition);
        }

        return values;
    }

    /// <summary>
    /// 設定値をiniファイルに保存する
    /// </summary>
    public void SaveToIni(string path, SettingDefinition definition)
    {
        var lines = new List<string>();
        
        // バージョン情報を先頭に追加
        lines.Add($"SKMU_SETTINGS_VERSION={definition.Version}");
        lines.Add(""); // 空行
        
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
    /// デフォルト値にリセット
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
    /// 文字列値を指定された型に変換
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