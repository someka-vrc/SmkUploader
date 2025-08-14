using System.Diagnostics;

namespace SmkUploaderSetting.Models.Validation;

/// <summary>
/// �����ڂ̒l�ɉ����ĕK�{���ǂ����𔻒肷��o���f�[�V�������[��
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
        // �����ɊY�����Ȃ��ꍇ�͌��؂��X�L�b�v
        if (!Condition(allValues))
            return ValidationResult.Success;

        // �����ɊY������ꍇ�͕K�{�`�F�b�N�����s
        if (value == null)
            return ValidationResult.Error(ErrorMessage);

        var stringValue = value.ToString();
        if (string.IsNullOrWhiteSpace(stringValue))
            return ValidationResult.Error(ErrorMessage);

        return ValidationResult.Success;
    }
}