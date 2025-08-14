namespace SmkUploaderSetting.Models.VersionConversion;

/// <summary>
/// �o�[�W�����ϊ����Ǘ�����N���X
/// </summary>
public static class VersionConversionManager
{
    private static readonly List<ISettingVersionConverter> _converters = new()
    {
        new SettingVersionConverter_v0_to_v1()
    };

    /// <summary>
    /// �w�肳�ꂽ�o�[�W��������ŐV�o�[�W�����֕ϊ�����
    /// </summary>
    /// <param name="values">�ϊ����̐ݒ�l</param>
    /// <param name="fromVersion">�ϊ����̃o�[�W����</param>
    /// <param name="targetVersion">�ϊ���̃o�[�W����</param>
    /// <returns>�ϊ���̐ݒ�l</returns>
    public static SettingValues ConvertToVersion(SettingValues values, int fromVersion, int targetVersion)
    {
        if (fromVersion == targetVersion)
            return values;

        var currentValues = values;
        var currentVersion = fromVersion;

        while (currentVersion < targetVersion)
        {
            var converter = _converters.FirstOrDefault(c => c.FromVersion == currentVersion);
            if (converter == null)
            {
                throw new InvalidOperationException($"�o�[�W���� {currentVersion} ����̕ϊ��p�X��������܂���");
            }

            currentValues = converter.Convert(currentValues);
            currentVersion = converter.ToVersion;
        }

        return currentValues;
    }

    /// <summary>
    /// �w�肳�ꂽ�o�[�W��������ŐV�o�[�W�����܂ł̕ϊ��p�X�����݂��邩�`�F�b�N
    /// </summary>
    /// <param name="fromVersion">�ϊ����̃o�[�W����</param>
    /// <param name="targetVersion">�ϊ���̃o�[�W����</param>
    /// <returns>�ϊ��\�ȏꍇ��true</returns>
    public static bool CanConvert(int fromVersion, int targetVersion)
    {
        if (fromVersion == targetVersion)
            return true;

        var currentVersion = fromVersion;
        while (currentVersion < targetVersion)
        {
            var converter = _converters.FirstOrDefault(c => c.FromVersion == currentVersion);
            if (converter == null)
                return false;

            currentVersion = converter.ToVersion;
        }

        return true;
    }
}