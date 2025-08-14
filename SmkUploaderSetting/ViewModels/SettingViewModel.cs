using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using SmkUploaderSetting.Models;
using System.Reflection;

namespace SmkUploaderSetting.ViewModels;

/// <summary>
/// �ݒ�S�̂�ViewModel
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

    // �T�[�r�X���p�K��URL�iSMKU_SERVICE�̒l�ɉ����āj
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

    // ���ݑI�𒆂̃T�[�r�X�l�iSMKU_SERVICE��ValueAsString�j
    private string? ServiceValue =>
        Items.FirstOrDefault(x => x.Key == "SMKU_SERVICE")?.ValueAsString;

    // �o�[�W�������icsproj��<Version>��\���j
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

        // SMKU_SERVICE�̕ύX����SelectedServiceTermsUrl�̕ύX�ʒm
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
    /// �ݒ�t�@�C����ǂݍ��݁AUI���\�z
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
                
                // �S�̂̒l���擾����f���Q�[�g��ݒ�
                itemViewModel.GetAllValues = () => GetAllValues();
                
                // �ǂݍ��񂾒l��ݒ�
                if (_settingValues.Values.TryGetValue(itemDef.Key, out var value))
                {
                    itemViewModel.Value = value;
                }
                
                // �l�ύX���̃o���f�[�V�����Ď��s��ݒ�
                itemViewModel.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(SettingItemViewModel.Value) || 
                        e.PropertyName == nameof(SettingItemViewModel.ValueAsString))
                    {
                        // �l�ύX���͑S���ڂ̃o���f�[�V�������Ď��s
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
            ShowErrorMessage($"�ݒ�̓ǂݍ��݂Ɏ��s���܂���: {ex.Message}");
        }
    }

    /// <summary>
    /// �ݒ���t�@�C���ɕۑ�
    /// </summary>
    public void Save()
    {
        try
        {
            // ViewModel�̒l��SettingValues�ɔ��f
            foreach (var item in Items)
            {
                _settingValues.Values[item.Key] = item.Value;
            }
            
            var configPath = Path.Combine(AppContext.BaseDirectory, ConfigFileName);
            _settingValues.SaveToIni(configPath, _definition);
            
            ShowStatusMessage("�ۑ����܂���");
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"�ۑ��Ɏ��s���܂���: {ex.Message}");
        }
    }

    /// <summary>
    /// �f�t�H���g�l�Ƀ��Z�b�g
    /// </summary>
    public void Reset()
    {
        var result = MessageBox.Show(
            "���ׂĂ̐ݒ���f�t�H���g�l�Ƀ��Z�b�g���܂����H",
            "�m�F",
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
    /// �S���ڂ̃o���f�[�V���������s
    /// </summary>
    private void ValidateAllItems()
    {
        foreach (var item in Items)
        {
            item.Validate();
        }
    }

    /// <summary>
    /// �ۑ��\���ǂ������X�V
    /// </summary>
    private void UpdateCanSave()
    {
        CanSave = Items.All(item => !item.HasError);
    }

    /// <summary>
    /// �S���ڂ̒l���擾
    /// </summary>
    private IReadOnlyDictionary<string, object?> GetAllValues()
    {
        return Items.ToDictionary(item => item.Key, item => item.Value);
    }

    /// <summary>
    /// �G���[���b�Z�[�W��\��
    /// </summary>
    private void ShowErrorMessage(string message)
    {
        MessageBox.Show(message, "�G���[", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    /// <summary>
    /// �X�e�[�^�X���b�Z�[�W����莞�ԕ\��
    /// </summary>
    private async void ShowStatusMessage(string message)
    {
        StatusMessage = message;
        ShowStatus = true;
        
        await Task.Delay(3000); // 3�b��ɔ�\��
        
        ShowStatus = false;
    }
}