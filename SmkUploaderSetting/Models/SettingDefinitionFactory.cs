using SmkUploaderSetting.Models;
using SmkUploaderSetting.Models.Validation;

namespace SmkUploaderSetting.Models;

/// <summary>
/// �ݒ��`�̃t�@�N�g���N���X
/// </summary>
public static class SettingDefinitionFactory
{
    /// <summary>
    /// �ŐV�̐ݒ��`���擾
    /// </summary>
    public static SettingDefinition GetLatestDefinition()
    {
        return GetVersion1Definition();
    }

    /// <summary>
    /// �o�[�W����1�̐ݒ��`
    /// </summary>
    public static SettingDefinition GetVersion1Definition()
    {
        return new SettingDefinition
        {
            Version = 1,
            Items = new List<SettingItemDefinition>
            {
                new()
                {
                    Key = "SMKU_MAX_WIDTH",
                    Label = "���T�C�Y��",
                    Description = "0�Ȃ烊�T�C�Y���܂���B",
                    Type = SettingValueType.Int,
                    DefaultValue = 2048,
                    ValidationRules = new List<IValidationRule>
                    {
                        new RangeRule(0, 10000, "�l��0�ȏ�10000�ȉ��ł���K�v������܂�")
                    }
                },
                new()
                {
                    Key = "SMKU_MAX_HEIGHT",
                    Label = "���T�C�Y����",
                    Description = "0�Ȃ烊�T�C�Y���܂���B",
                    Type = SettingValueType.Int,
                    DefaultValue = 2048,
                    ValidationRules = new List<IValidationRule>
                    {
                        new RangeRule(0, 10000, "�l��0�ȏ�10000�ȉ��ł���K�v������܂�")
                    }
                },
                new()
                {
                    Key = "SMKU_LOG_LEVEL",
                    Label = "���O���x��",
                    Description = "���O�t�@�C���ւ̏o�̓��x���iVerbose(�ł��ڍ�), Information, Warning(�����A����), Error�j",
                    Type = SettingValueType.String,
                    DefaultValue = "Warning",
                    ValidationRules = new List<IValidationRule>
                    {
                        new RequiredRule(),
                        new RegexRule("^(Verbose|Information|Warning|Error)$", "Verbose, Information, Warning, Error �̂����ꂩ")
                    }
                },
                new()
                {
                    Key = "SMKU_SET_CLIPBOARD",
                    Label = "�N���b�v�{�[�h�����o�^",
                    Description = "�A�b�v���[�h��Ɏ����I��URL���N���b�v�{�[�h�ɃR�s�[���܂��B",
                    Type = SettingValueType.Bool,
                    DefaultValue = true,
                    ValidationRules = new List<IValidationRule>()
                },
                new()
                {
                    Key = "SMKU_NO_INTERACTIVE",
                    Label = "�m�[�C���^���N�e�B�u���[�h",
                    Description = "�G���[��x�����������Ă���ʂ������ɕ��܂��B",
                    Type = SettingValueType.Bool,
                    DefaultValue = false,
                    ValidationRules = new List<IValidationRule>()
                },
                new()
                {
                    Key = "SMKU_RESULT_SOUND",
                    Label = "����������",
                    Description = "�������ɉ���炵�܂��B",
                    Type = SettingValueType.Bool,
                    DefaultValue = true,
                    ValidationRules = new List<IValidationRule>()
                },
                new()
                {
                    Key = "SMKU_SERVICE",
                    Label = "�摜�z�X�e�B���O�T�[�r�X",
                    Description = "�A�b�v���[�h��̃T�[�r�X",
                    Type = SettingValueType.String,
                    DefaultValue = "GYAZO",
                    ValidationRules = new List<IValidationRule>
                    {
                        new RequiredRule(),
                        new RegexRule("^(GYAZO|IMGBB|FREEIMAGEHOST)$", "GYAZO/IMGBB/FREEIMAGEHOST")
                    }
                },
                new()
                {
                    Key = "SMKU_GYAZO_TOKEN",
                    Label = "Gyazo �A�N�Z�X�g�[�N��",
                    Description = "",
                    Type = SettingValueType.String,
                    DefaultValue = "",
                    ValidationRules = new List<IValidationRule>
                    {
                        new ConditionalRequiredRule(
                            values => values.GetValueOrDefault("SMKU_SERVICE")?.ToString() == "GYAZO",
                            "�摜�z�X�e�B���O�T�[�r�X�� Gyazo �̂Ƃ��K�{�ł�"
                        )
                    }
                },
                new()
                {
                    Key = "SMKU_IMGBB_API_KEY",
                    Label = "imgBB API�L�[",
                    Description = "",
                    Type = SettingValueType.String,
                    DefaultValue = "",
                    ValidationRules = new List<IValidationRule>
                    {
                        new ConditionalRequiredRule(
                            values => values.GetValueOrDefault("SMKU_SERVICE")?.ToString() == "IMGBB",
                            "�摜�z�X�e�B���O�T�[�r�X�� imgBB �̂Ƃ��K�{�ł�"
                        )
                    }
                },
                new()
                {
                    Key = "SMKU_IMGBB_EXPIRATION_HOURS",
                    Label = "imgBB �t�@�C���L������",
                    Description = "���ԒP�ʂŎw�肵�܂��B0�Ȃ疳�����i�ő�180���j",
                    Type = SettingValueType.Int,
                    DefaultValue = 48,
                    ValidationRules = new List<IValidationRule>
                    {
                        new ConditionalRequiredRule(
                            values => values.GetValueOrDefault("SMKU_SERVICE")?.ToString() == "IMGBB",
                            "�摜�z�X�e�B���O�T�[�r�X�� imgBB �̂Ƃ��K�{�ł�"
                        ),
                        new RangeRule(0, 4320, "�l��0�ȏ�4320�ȉ��ł���K�v������܂��i0�͖������A�ő�180���j")
                    }
                },
                new()
                {
                    Key = "SMKU_FREEIMAGEHOST_API_KEY",
                    Label = "Freeimage.host API�L�[",
                    Description = "",
                    Type = SettingValueType.String,
                    DefaultValue = "",
                    ValidationRules = new List<IValidationRule>
                    {
                        new ConditionalRequiredRule(
                            values => values.GetValueOrDefault("SMKU_SERVICE")?.ToString() == "IMGBB",
                            "�摜�z�X�e�B���O�T�[�r�X�� Freeimage.host �̂Ƃ��K�{�ł�"
                        )
                    }
                }
            }
        };
    }
}