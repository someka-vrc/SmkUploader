namespace SmkUploaderSetting.Models.Validation;

/// <summary>
/// �o���f�[�V�������[���̃C���^�[�t�F�[�X
/// </summary>
public interface IValidationRule
{
    /// <summary>
    /// �o���f�[�V���������s����
    /// </summary>
    /// <param name="value">���ؑΏۂ̒l</param>
    /// <param name="allValues">�S�̂̐ݒ�l�i�����ڂ̒l���Q�Ƃ���ꍇ�Ɏg�p�j</param>
    /// <returns>�o���f�[�V��������</returns>
    ValidationResult Validate(object? value, IReadOnlyDictionary<string, object?> allValues);
}