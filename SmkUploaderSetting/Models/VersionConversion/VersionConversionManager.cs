namespace SmkUploaderSetting.Models.VersionConversion;

/// <summary>
/// バージョン変換を管理するクラス
/// </summary>
public static class VersionConversionManager
{
    private static readonly List<ISettingVersionConverter> _converters = new()
    {
        new SettingVersionConverter_v0_to_v1()
    };

    /// <summary>
    /// 指定されたバージョンから最新バージョンへ変換する
    /// </summary>
    /// <param name="values">変換元の設定値</param>
    /// <param name="fromVersion">変換元のバージョン</param>
    /// <param name="targetVersion">変換先のバージョン</param>
    /// <returns>変換後の設定値</returns>
    public static SettingValues ConvertToVersion(SettingValues values, int fromVersion, int targetVersion)
    {
        if (fromVersion == targetVersion)
            return values;

        var currentValues = values;
        var currentVersion = fromVersion;

        while (currentVersion < targetVersion)
        {
            var converter = _converters.FirstOrDefault(c => c.FromVersion == currentVersion);
            if (converter == null)
            {
                throw new InvalidOperationException($"バージョン {currentVersion} からの変換パスが見つかりません");
            }

            currentValues = converter.Convert(currentValues);
            currentVersion = converter.ToVersion;
        }

        return currentValues;
    }

    /// <summary>
    /// 指定されたバージョンから最新バージョンまでの変換パスが存在するかチェック
    /// </summary>
    /// <param name="fromVersion">変換元のバージョン</param>
    /// <param name="targetVersion">変換先のバージョン</param>
    /// <returns>変換可能な場合はtrue</returns>
    public static bool CanConvert(int fromVersion, int targetVersion)
    {
        if (fromVersion == targetVersion)
            return true;

        var currentVersion = fromVersion;
        while (currentVersion < targetVersion)
        {
            var converter = _converters.FirstOrDefault(c => c.FromVersion == currentVersion);
            if (converter == null)
                return false;

            currentVersion = converter.ToVersion;
        }

        return true;
    }
}