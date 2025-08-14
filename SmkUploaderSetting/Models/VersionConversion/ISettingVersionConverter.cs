namespace SmkUploaderSetting.Models.VersionConversion;

/// <summary>
/// �ݒ�l�I�u�W�F�N�g�����o�[�W��������V�o�[�W�����֕ϊ�����C���^�[�t�F�[�X
/// </summary>
public interface ISettingVersionConverter
{
    /// <summary>
    /// �ϊ����̃o�[�W����
    /// </summary>
    int FromVersion { get; }
    
    /// <summary>
    /// �ϊ���̃o�[�W����
    /// </summary>
    int ToVersion { get; }
    
    /// <summary>
    /// �ݒ�l��ϊ�����
    /// </summary>
    /// <param name="oldValues">�ϊ����̐ݒ�l</param>
    /// <returns>�ϊ���̐ݒ�l</returns>
    SettingValues Convert(SettingValues oldValues);
}