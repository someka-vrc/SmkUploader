using System.Diagnostics;

namespace SmkUploaderSetting.Models.Validation;

/// <summary>
/// 他項目の値に応じて必須かどうかを判定するバリデーションルール
/// </summary>
public class ConditionalRequiredRule : IValidationRule
{
    public Func<IReadOnlyDictionary<string, object?>, bool> Condition { get; set; }
    public string ErrorMessage { get; set; }

    public ConditionalRequiredRule(Func<IReadOnlyDictionary<string, object?>, bool> condition, string errorMessage)
    {
        Condition = condition;
        ErrorMessage = errorMessage;
    }

    public ValidationResult Validate(object? value, IReadOnlyDictionary<string, object?> allValues)
    {
        // 条件に該当しない場合は検証をスキップ
        if (!Condition(allValues))
            return ValidationResult.Success;

        // 条件に該当する場合は必須チェックを実行
        if (value == null)
            return ValidationResult.Error(ErrorMessage);

        var stringValue = value.ToString();
        if (string.IsNullOrWhiteSpace(stringValue))
            return ValidationResult.Error(ErrorMessage);

        return ValidationResult.Success;
    }
}