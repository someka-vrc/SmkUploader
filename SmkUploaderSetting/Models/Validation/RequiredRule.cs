namespace SmkUploaderSetting.Models.Validation;

/// <summary>
/// �K�{�`�F�b�N�̃o���f�[�V�������[��
/// </summary>
public class RequiredRule : IValidationRule
{
    public string ErrorMessage { get; set; } = "���̍��ڂ͕K�{�ł�";

    public RequiredRule(string? errorMessage = null)
    {
        if (!string.IsNullOrEmpty(errorMessage))
            ErrorMessage = errorMessage;
    }

    public ValidationResult Validate(object? value, IReadOnlyDictionary<string, object?> allValues)
    {
        if (value == null)
            return ValidationResult.Error(ErrorMessage);

        var stringValue = value.ToString();
        if (string.IsNullOrWhiteSpace(stringValue))
            return ValidationResult.Error(ErrorMessage);

        return ValidationResult.Success;
    }
}