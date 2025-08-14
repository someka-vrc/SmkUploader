namespace SmkUploaderSetting.Models.Validation;

/// <summary>
/// 数値範囲チェックのバリデーションルール
/// </summary>
public class RangeRule : IValidationRule
{
    public double Min { get; set; }
    public double Max { get; set; }
    public string ErrorMessage { get; set; }

    public RangeRule(double min, double max, string? errorMessage = null)
    {
        Min = min;
        Max = max;
        ErrorMessage = errorMessage ?? $"値は{min}以上{max}以下である必要があります";
    }

    public ValidationResult Validate(object? value, IReadOnlyDictionary<string, object?> allValues)
    {
        if (value == null)
            return ValidationResult.Success; // 必須チェックは別のルールで行う

        if (!double.TryParse(value.ToString(), out var numericValue))
            return ValidationResult.Error("数値を入力してください");

        if (numericValue < Min || numericValue > Max)
            return ValidationResult.Error(ErrorMessage);

        return ValidationResult.Success;
    }
}