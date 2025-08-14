namespace SmkUploaderSetting.Models.Validation;

/// <summary>
/// バリデーションルールのインターフェース
/// </summary>
public interface IValidationRule
{
    /// <summary>
    /// バリデーションを実行する
    /// </summary>
    /// <param name="value">検証対象の値</param>
    /// <param name="allValues">全体の設定値（他項目の値を参照する場合に使用）</param>
    /// <returns>バリデーション結果</returns>
    ValidationResult Validate(object? value, IReadOnlyDictionary<string, object?> allValues);
}