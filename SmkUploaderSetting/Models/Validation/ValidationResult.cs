namespace SmkUploaderSetting.Models.Validation;

/// <summary>
/// �o���f�[�V�������ʂ�\���N���X
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public static ValidationResult Success => new() { IsValid = true };
    public static ValidationResult Error(string message) => new() { IsValid = false, ErrorMessage = message };
}