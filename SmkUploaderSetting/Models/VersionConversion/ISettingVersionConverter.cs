namespace SmkUploaderSetting.Models.VersionConversion;

/// <summary>
/// 設定値オブジェクトを旧バージョンから新バージョンへ変換するインターフェース
/// </summary>
public interface ISettingVersionConverter
{
    /// <summary>
    /// 変換元のバージョン
    /// </summary>
    int FromVersion { get; }
    
    /// <summary>
    /// 変換先のバージョン
    /// </summary>
    int ToVersion { get; }
    
    /// <summary>
    /// 設定値を変換する
    /// </summary>
    /// <param name="oldValues">変換元の設定値</param>
    /// <returns>変換後の設定値</returns>
    SettingValues Convert(SettingValues oldValues);
}