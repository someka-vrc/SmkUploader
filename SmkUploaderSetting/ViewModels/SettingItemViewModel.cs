using SmkUploaderSetting.Models;
using SmkUploaderSetting.Models.Validation;

namespace SmkUploaderSetting.ViewModels;

/// <summary>
/// 各項目の値・エラー状態・UIバインディング用プロパティを持つViewModel
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
                // 自分のバリデーションは外部から実行されるのでここでは実行しない
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
            
            // 文字列型の場合は直接設定
            if (Definition.Type == SettingValueType.String)
            {
                Value = value;
                return;
            }
            
            // その他の型の場合は変換を試みる
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
                // 変換に失敗した場合は現在の値を維持
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
                // HasErrorは計算プロパティなので、Errorが変更されたときに通知する
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrEmpty(Error);

    // UI表示用プロパティ
    public string Key => Definition.Key;
    public string Label => Definition.Label;
    public string DisplayLabel => $"{Definition.Label} ({Definition.Key})";
    public string Description => Definition.Description;
    public SettingValueType Type => Definition.Type;
    public bool IsBoolType => Definition.Type == SettingValueType.Bool;
    public bool IsTextType => Definition.Type != SettingValueType.Bool;
    public bool IsLogLevel => Key == "SMKU_LOG_LEVEL";
    public bool IsService => Key == "SMKU_SERVICE";

    // バリデーション用に全体の値を取得するためのデリゲート
    public Func<IReadOnlyDictionary<string, object?>>? GetAllValues { get; set; }

    public SettingItemViewModel(SettingItemDefinition definition)
    {
        Definition = definition;
        _value = definition.DefaultValue;
    }

    /// <summary>
    /// バリデーションを実行
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
    /// 型に応じて文字列値を変換
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
            // 変換に失敗した場合はデフォルト値を設定
            Value = Definition.DefaultValue;
        }
    }
}