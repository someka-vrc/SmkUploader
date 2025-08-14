using System.Text.RegularExpressions;

namespace SmkUploaderSetting.Models.Validation;

/// <summary>
/// ���K�\���`�F�b�N�̃o���f�[�V�������[��
/// </summary>
public class RegexRule : IValidationRule
{
    public string Pattern { get; set; }
    public string PatternDescription { get; set; }
    public string ErrorMessage { get; set; }
    private readonly Regex _regex;

    public RegexRule(string pattern, string patternDescription, string? errorMessage = null)
    {
        Pattern = pattern;
        PatternDescription = patternDescription;
        ErrorMessage = errorMessage ?? $"{patternDescription}����͂��Ă�������";
        _regex = new Regex(pattern);
    }

    public ValidationResult Validate(object? value, IReadOnlyDictionary<string, object?> allValues)
    {
        if (value == null)
            return ValidationResult.Success; // �K�{�`�F�b�N�͕ʂ̃��[���ōs��

        var stringValue = value.ToString();
        if (string.IsNullOrEmpty(stringValue))
            return ValidationResult.Success;

        if (!_regex.IsMatch(stringValue))
            return ValidationResult.Error(ErrorMessage);

        return ValidationResult.Success;
    }
}