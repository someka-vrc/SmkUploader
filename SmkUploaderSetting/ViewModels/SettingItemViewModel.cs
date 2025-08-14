using SmkUploaderSetting.Models;
using SmkUploaderSetting.Models.Validation;

namespace SmkUploaderSetting.ViewModels;

/// <summary>
/// �e���ڂ̒l�E�G���[��ԁEUI�o�C���f�B���O�p�v���p�e�B������ViewModel
/// </summary>
public class SettingItemViewModel : ViewModelBase
{
    private object? _value;
    private string _error = string.Empty;

    public SettingItemDefinition Definition { get; }
    
    public object? Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                System.Diagnostics.Debug.WriteLine($"Value property changed: {Key} = '{value}' ({value?.GetType()})");
                // �����̃o���f�[�V�����͊O��������s�����̂ł����ł͎��s���Ȃ�
                OnPropertyChanged(nameof(ValueAsString));
            }
        }
    }

    public string ValueAsString
    {
        get => Value?.ToString() ?? string.Empty;
        set
        {
            System.Diagnostics.Debug.WriteLine($"ValueAsString setter called: {Key} = '{value}'");
            
            // ������^�̏ꍇ�͒��ڐݒ�
            if (Definition.Type == SettingValueType.String)
            {
                Value = value;
                return;
            }
            
            // ���̑��̌^�̏ꍇ�͕ϊ������݂�
            try
            {
                object convertedValue = Definition.Type switch
                {
                    SettingValueType.Int => string.IsNullOrEmpty(value) ? 0 : int.Parse(value),
                    SettingValueType.Double => string.IsNullOrEmpty(value) ? 0.0 : double.Parse(value),
                    SettingValueType.Bool => string.IsNullOrEmpty(value) ? false : bool.Parse(value),
                    _ => value
                };
                Value = convertedValue;
            }
            catch
            {
                // �ϊ��Ɏ��s�����ꍇ�͌��݂̒l���ێ�
            }
        }
    }

    public string Error
    {
        get => _error;
        set 
        {
            if (SetProperty(ref _error, value))
            {
                // HasError�͌v�Z�v���p�e�B�Ȃ̂ŁAError���ύX���ꂽ�Ƃ��ɒʒm����
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrEmpty(Error);

    // UI�\���p�v���p�e�B
    public string Key => Definition.Key;
    public string Label => Definition.Label;
    public string DisplayLabel => $"{Definition.Label} ({Definition.Key})";
    public string Description => Definition.Description;
    public SettingValueType Type => Definition.Type;
    public bool IsBoolType => Definition.Type == SettingValueType.Bool;
    public bool IsTextType => Definition.Type != SettingValueType.Bool;
    public bool IsLogLevel => Key == "SMKU_LOG_LEVEL";
    public bool IsService => Key == "SMKU_SERVICE";

    // �o���f�[�V�����p�ɑS�̂̒l���擾���邽�߂̃f���Q�[�g
    public Func<IReadOnlyDictionary<string, object?>>? GetAllValues { get; set; }

    public SettingItemViewModel(SettingItemDefinition definition)
    {
        Definition = definition;
        _value = definition.DefaultValue;
    }

    /// <summary>
    /// �o���f�[�V���������s
    /// </summary>
    public void Validate()
    {
        var allValues = GetAllValues?.Invoke() ?? new Dictionary<string, object?>();
        
        foreach (var rule in Definition.ValidationRules)
        {
            var result = rule.Validate(Value, allValues);
            if (!result.IsValid)
            {
                Error = result.ErrorMessage;
                return;
            }
        }
        
        Error = string.Empty;
    }

    /// <summary>
    /// �^�ɉ����ĕ�����l��ϊ�
    /// </summary>
    public void SetValueFromString(string stringValue)
    {
        try
        {
            object convertedValue = Definition.Type switch
            {
                SettingValueType.Int => int.Parse(stringValue),
                SettingValueType.Double => double.Parse(stringValue),
                SettingValueType.Bool => bool.Parse(stringValue),
                SettingValueType.String => stringValue,
                _ => stringValue
            };
            Value = convertedValue;
        }
        catch
        {
            // �ϊ��Ɏ��s�����ꍇ�̓f�t�H���g�l��ݒ�
            Value = Definition.DefaultValue;
        }
    }
}