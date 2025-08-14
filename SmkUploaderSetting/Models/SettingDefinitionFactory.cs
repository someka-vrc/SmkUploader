using SmkUploaderSetting.Models;
using SmkUploaderSetting.Models.Validation;

namespace SmkUploaderSetting.Models;

/// <summary>
/// 設定定義のファクトリクラス
/// </summary>
public static class SettingDefinitionFactory
{
    /// <summary>
    /// 最新の設定定義を取得
    /// </summary>
    public static SettingDefinition GetLatestDefinition()
    {
        return GetVersion1Definition();
    }

    /// <summary>
    /// バージョン1の設定定義
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
                    Label = "リサイズ幅",
                    Description = "0ならリサイズしません。",
                    Type = SettingValueType.Int,
                    DefaultValue = 2048,
                    ValidationRules = new List<IValidationRule>
                    {
                        new RangeRule(0, 10000, "値は0以上10000以下である必要があります")
                    }
                },
                new()
                {
                    Key = "SMKU_MAX_HEIGHT",
                    Label = "リサイズ高さ",
                    Description = "0ならリサイズしません。",
                    Type = SettingValueType.Int,
                    DefaultValue = 2048,
                    ValidationRules = new List<IValidationRule>
                    {
                        new RangeRule(0, 10000, "値は0以上10000以下である必要があります")
                    }
                },
                new()
                {
                    Key = "SMKU_LOG_LEVEL",
                    Label = "ログレベル",
                    Description = "ログファイルへの出力レベル（Verbose(最も詳細), Information, Warning(推奨、高速), Error）",
                    Type = SettingValueType.String,
                    DefaultValue = "Warning",
                    ValidationRules = new List<IValidationRule>
                    {
                        new RequiredRule(),
                        new RegexRule("^(Verbose|Information|Warning|Error)$", "Verbose, Information, Warning, Error のいずれか")
                    }
                },
                new()
                {
                    Key = "SMKU_SET_CLIPBOARD",
                    Label = "クリップボード自動登録",
                    Description = "アップロード後に自動的にURLをクリップボードにコピーします。",
                    Type = SettingValueType.Bool,
                    DefaultValue = true,
                    ValidationRules = new List<IValidationRule>()
                },
                new()
                {
                    Key = "SMKU_NO_INTERACTIVE",
                    Label = "ノーインタラクティブモード",
                    Description = "エラーや警告が発生しても画面をすぐに閉じます。",
                    Type = SettingValueType.Bool,
                    DefaultValue = false,
                    ValidationRules = new List<IValidationRule>()
                },
                new()
                {
                    Key = "SMKU_RESULT_SOUND",
                    Label = "完了時音声",
                    Description = "完了時に音を鳴らします。",
                    Type = SettingValueType.Bool,
                    DefaultValue = true,
                    ValidationRules = new List<IValidationRule>()
                },
                new()
                {
                    Key = "SMKU_SERVICE",
                    Label = "画像ホスティングサービス",
                    Description = "アップロード先のサービス",
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
                    Label = "Gyazo アクセストークン",
                    Description = "",
                    Type = SettingValueType.String,
                    DefaultValue = "",
                    ValidationRules = new List<IValidationRule>
                    {
                        new ConditionalRequiredRule(
                            values => values.GetValueOrDefault("SMKU_SERVICE")?.ToString() == "GYAZO",
                            "画像ホスティングサービスが Gyazo のとき必須です"
                        )
                    }
                },
                new()
                {
                    Key = "SMKU_IMGBB_API_KEY",
                    Label = "imgBB APIキー",
                    Description = "",
                    Type = SettingValueType.String,
                    DefaultValue = "",
                    ValidationRules = new List<IValidationRule>
                    {
                        new ConditionalRequiredRule(
                            values => values.GetValueOrDefault("SMKU_SERVICE")?.ToString() == "IMGBB",
                            "画像ホスティングサービスが imgBB のとき必須です"
                        )
                    }
                },
                new()
                {
                    Key = "SMKU_IMGBB_EXPIRATION_HOURS",
                    Label = "imgBB ファイル有効期限",
                    Description = "時間単位で指定します。0なら無期限（最大180日）",
                    Type = SettingValueType.Int,
                    DefaultValue = 48,
                    ValidationRules = new List<IValidationRule>
                    {
                        new ConditionalRequiredRule(
                            values => values.GetValueOrDefault("SMKU_SERVICE")?.ToString() == "IMGBB",
                            "画像ホスティングサービスが imgBB のとき必須です"
                        ),
                        new RangeRule(0, 4320, "値は0以上4320以下である必要があります（0は無期限、最大180日）")
                    }
                },
                new()
                {
                    Key = "SMKU_FREEIMAGEHOST_API_KEY",
                    Label = "Freeimage.host APIキー",
                    Description = "",
                    Type = SettingValueType.String,
                    DefaultValue = "",
                    ValidationRules = new List<IValidationRule>
                    {
                        new ConditionalRequiredRule(
                            values => values.GetValueOrDefault("SMKU_SERVICE")?.ToString() == "IMGBB",
                            "画像ホスティングサービスが Freeimage.host のとき必須です"
                        )
                    }
                }
            }
        };
    }
}