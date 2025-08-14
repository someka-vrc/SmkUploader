using Microsoft.Extensions.Configuration;
using Serilog;
using SmkUploader.App;

namespace SmkUploader.Infra;

internal static class InputProvider
{
    /// <summary>
    /// INIファイルと環境変数、コマンドライン引数から設定を取得します。
    /// コマンドライン引数は、 `--キー=ペア` 形式をオプションとして処理し、 `--キー` となっているものはフラグとして扱います、
    /// `--` で始まらない引数は位置引数として扱います。
    /// </summary>
    /// <param name="Config"></param>
    /// <param name="args"></param>
    /// <param name="isDev"></param>
    /// <returns></returns>
    internal static (IConfiguration Config, string[] PositionalArgs) GetConfigurationAndArgs(string[] args, bool isDev)
    {
        var iniPath = isDev
            ? Path.Combine(Directory.GetCurrentDirectory(), "SmkUploader", "settings.dev.ini")
            : Path.Combine(AppContext.BaseDirectory, "settings.ini");

        // 位置引数だけ集める
        var positionalArgs = args.Where(arg => !arg.StartsWith("--")).ToArray();

        // --付きの引数だけ処理する
        var commandLineArgs = args
            .Where(arg => arg.StartsWith("--"))
            .Select(arg => arg.Substring(2))
            .Select(arg => arg.Contains('=') ? arg : $"{arg}=true")
            .ToArray();

        try
        {
            var builder = new ConfigurationBuilder()
                .AddIniFile(iniPath, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables("SMKU_")
                .AddCommandLine(commandLineArgs);

            var config = builder.Build();

            return (config, positionalArgs);
        }
        catch (System.Exception ex)
        {
            Log.Error(ex, "INIファイルの読み込みに失敗しました。");
            // INIファイル防いでコケたかもしれないので外して再試行
            Log.Information("INIファイルの読み込みに失敗したため、INIファイルを無視して続行します。");
            Program.SetHasWarning();
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables("SMKU_")
                .AddCommandLine(commandLineArgs);

            var config = builder.Build();

            return (config, positionalArgs);
        }
    }

    /// <summary>
    /// 設定値バリデーション
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    internal static bool ValidateConfiguration(IConfiguration config)
    {
        var requiredKeys = new[] {
            "SMKU_MAX_WIDTH",
            "SMKU_MAX_HEIGHT",
            "SMKU_LOG_LEVEL",
            "SMKU_SET_CLIPBOARD",
            "SMKU_NO_INTERACTIVE",
            "SMKU_RESULT_SOUND",
            "SMKU_SERVICE",
        };
        foreach (var key in requiredKeys)
        {
            if (string.IsNullOrEmpty(config[key]))
            {
                Log.Error("設定が不正です。{Key}が設定されていません。", key);
                return false;
            }
        }

        var boolKeys = new[] {
            "SMKU_SET_CLIPBOARD",
            "SMKU_NO_INTERACTIVE",
            "SMKU_RESULT_SOUND",
        };
        foreach (var key in boolKeys)
        {
            if (!bool.TryParse(config[key], out _))
            {
                Log.Error("設定が不正です。{Key}が不正な値です。", key);
                return false;
            }
        }

        var integerKeys = new[] {
            "SMKU_MAX_WIDTH",
            "SMKU_MAX_HEIGHT",
        };
        foreach (var key in integerKeys)
        {
            if (!int.TryParse(config[key], out _))
            {
                Log.Error("設定が不正です。{Key}が不正な値です。", key);
                return false;
            }
        }

        return true;
    }
}