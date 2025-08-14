namespace SmkUploaderSetting.Models.VersionConversion;

/// <summary>
/// �o�[�W����0����o�[�W����1�ւ̕ϊ��N���X
/// </summary>
public class SettingVersionConverter_v0_to_v1 : ISettingVersionConverter
{
    public int FromVersion => 0;
    public int ToVersion => 1;

    public SettingValues Convert(SettingValues oldValues)
    {
        // �o�[�W����0�̓o�[�W����1�Ɠ����\���Ȃ̂ŁA���̂܂ܕԂ�
        // �����I�ɍ��ڂ̒ǉ���ύX���������ꍇ�͂����Ń}�b�s���O������
        return oldValues;
    }
}