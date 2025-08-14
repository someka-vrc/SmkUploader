using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using SmkUploaderSetting.Models;
using System.Reflection;

namespace SmkUploaderSetting.ViewModels;

/// <summary>
/// 設定全体のViewModel
/// </summary>
public class SettingViewModel : ViewModelBase
{
    private const string ConfigFileName = "settings.ini";
    
    private bool _canSave = true;
    private string _statusMessage = string.Empty;
    private bool _showStatus = false;

    public ObservableCollection<SettingItemViewModel> Items { get; } = new();
    
    public bool CanSave
    {
        get => _canSave;
        set => SetProperty(ref _canSave, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool ShowStatus
    {
        get => _showStatus;
        set => SetProperty(ref _showStatus, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand ResetCommand { get; }

    private readonly SettingDefinition _definition;
    private SettingValues _settingValues;

    // サービス利用規約URL（SMKU_SERVICEの値に応じて）
    public string? SelectedServiceTermsUrl
    {
        get
        {
            var service = ServiceValue;
            return service switch
            {
                "GYAZO" => "https://gyazo.com/doc/terms/ja",
                "IMGBB" => "https://imgbb.com/tos",
                "FREEIMAGEHOST" => "https://freeimage.host/page/tos",
                _ => null
            };
        }
    }

    // 現在選択中のサービス値（SMKU_SERVICEのValueAsString）
    private string? ServiceValue =>
        Items.FirstOrDefault(x => x.Key == "SMKU_SERVICE")?.ValueAsString;

    // バージョン情報（csprojの<Version>を表示）
    public string Version { get; } =
        (Attribute.GetCustomAttribute(
            Assembly.GetExecutingAssembly(),
            typeof(AssemblyInformationalVersionAttribute)
        ) as AssemblyInformationalVersionAttribute)?.InformationalVersion
        ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString()
        ?? "1.0.0";

    public SettingViewModel()
    {
        _definition = SettingDefinitionFactory.GetLatestDefinition();
        _settingValues = new SettingValues();
        
        SaveCommand = new RelayCommand(Save, () => CanSave);
        ResetCommand = new RelayCommand(Reset);
        
        Load();

        // SMKU_SERVICEの変更時にSelectedServiceTermsUrlの変更通知
        var serviceItem = Items.FirstOrDefault(x => x.Key == "SMKU_SERVICE");
        if (serviceItem != null)
        {
            serviceItem.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SettingItemViewModel.Value) ||
                    e.PropertyName == nameof(SettingItemViewModel.ValueAsString))
                {
                    OnPropertyChanged(nameof(SelectedServiceTermsUrl));
                }
            };
        }
    }

    /// <summary>
    /// 設定ファイルを読み込み、UIを構築
    /// </summary>
    public void Load()
    {
        try
        {
            var configPath = Path.Combine(AppContext.BaseDirectory, ConfigFileName);
            _settingValues = SettingValues.LoadFromIni(configPath, _definition);

            Items.Clear();
            foreach (var itemDef in _definition.Items)
            {
                var itemViewModel = new SettingItemViewModel(itemDef);
                
                // 全体の値を取得するデリゲートを設定
                itemViewModel.GetAllValues = () => GetAllValues();
                
                // 読み込んだ値を設定
                if (_settingValues.Values.TryGetValue(itemDef.Key, out var value))
                {
                    itemViewModel.Value = value;
                }
                
                // 値変更時のバリデーション再実行を設定
                itemViewModel.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(SettingItemViewModel.Value) || 
                        e.PropertyName == nameof(SettingItemViewModel.ValueAsString))
                    {
                        // 値変更時は全項目のバリデーションを再実行
                        ValidateAllItems();
                        UpdateCanSave();
                    }
                };
                
                Items.Add(itemViewModel);
            }
            
            ValidateAllItems();
            UpdateCanSave();
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"設定の読み込みに失敗しました: {ex.Message}");
        }
    }

    /// <summary>
    /// 設定をファイルに保存
    /// </summary>
    public void Save()
    {
        try
        {
            // ViewModelの値をSettingValuesに反映
            foreach (var item in Items)
            {
                _settingValues.Values[item.Key] = item.Value;
            }
            
            var configPath = Path.Combine(AppContext.BaseDirectory, ConfigFileName);
            _settingValues.SaveToIni(configPath, _definition);
            
            ShowStatusMessage("保存しました");
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"保存に失敗しました: {ex.Message}");
        }
    }

    /// <summary>
    /// デフォルト値にリセット
    /// </summary>
    public void Reset()
    {
        var result = MessageBox.Show(
            "すべての設定をデフォルト値にリセットしますか？",
            "確認",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
            
        if (result == MessageBoxResult.Yes)
        {
            _settingValues.ResetToDefault(_definition);
            
            foreach (var item in Items)
            {
                item.Value = _settingValues.Values[item.Key];
            }
            
            ValidateAllItems();
            UpdateCanSave();
        }
    }

    /// <summary>
    /// 全項目のバリデーションを実行
    /// </summary>
    private void ValidateAllItems()
    {
        foreach (var item in Items)
        {
            item.Validate();
        }
    }

    /// <summary>
    /// 保存可能かどうかを更新
    /// </summary>
    private void UpdateCanSave()
    {
        CanSave = Items.All(item => !item.HasError);
    }

    /// <summary>
    /// 全項目の値を取得
    /// </summary>
    private IReadOnlyDictionary<string, object?> GetAllValues()
    {
        return Items.ToDictionary(item => item.Key, item => item.Value);
    }

    /// <summary>
    /// エラーメッセージを表示
    /// </summary>
    private void ShowErrorMessage(string message)
    {
        MessageBox.Show(message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    /// <summary>
    /// ステータスメッセージを一定時間表示
    /// </summary>
    private async void ShowStatusMessage(string message)
    {
        StatusMessage = message;
        ShowStatus = true;
        
        await Task.Delay(3000); // 3秒後に非表示
        
        ShowStatus = false;
    }
}