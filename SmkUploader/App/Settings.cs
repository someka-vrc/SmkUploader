using Microsoft.Extensions.Configuration;

namespace SmkUploader.App
{
    public class Settings
    {
        public MetaSettings Meta { get; } = new MetaSettings();
        public BasicSettings Basic { get; } = new BasicSettings();
    }

    public class MetaSettings
    {
        [ConfigurationKeyName("SMKU_SETTING_VERSION")]
        public int SettingVersion { get; set; } = 0;
    }

    public class BasicSettings
    {
        [ConfigurationKeyName("SMKU_MAX_WIDTH")]
        public int MaxWidth { get; set; } = 2048;

        [ConfigurationKeyName("SMKU_MAX_HEIGHT")]
        public int MaxHeight { get; set; } = 2048;

        [ConfigurationKeyName("SMKU_LOG_LEVEL")]
        public string LogLevel { get; set; } = "Warning";

        [ConfigurationKeyName("SMKU_SET_CLIPBOARD")]
        public bool SetClipboard { get; set; } = true;

        [ConfigurationKeyName("SMKU_NO_INTERACTIVE")]
        public bool NoInteractive { get; set; } = false;

        [ConfigurationKeyName("SMKU_RESULT_SOUND")]
        public bool ResultSound { get; set; } = true;

        [ConfigurationKeyName("SMKU_SERVICE")]
        public string Service { get; set; } = "GYAZO";
    }
}