namespace SmkUploaderSetting.Models.Validation;

/// <summary>
/// ���l�͈̓`�F�b�N�̃o���f�[�V�������[��
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
        ErrorMessage = errorMessage ?? $"�l��{min}�ȏ�{max}�ȉ��ł���K�v������܂�";
    }

    public ValidationResult Validate(object? value, IReadOnlyDictionary<string, object?> allValues)
    {
        if (value == null)
            return ValidationResult.Success; // �K�{�`�F�b�N�͕ʂ̃��[���ōs��

        if (!double.TryParse(value.ToString(), out var numericValue))
            return ValidationResult.Error("���l����͂��Ă�������");

        if (numericValue < Min || numericValue > Max)
            return ValidationResult.Error(ErrorMessage);

        return ValidationResult.Success;
    }
}