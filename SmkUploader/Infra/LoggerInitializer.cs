using Serilog;
using Serilog.Core;
using System.Globalization;


namespace SmkUploader.Infra;


public static class LoggerInitializer
{
    /// <summary>
    /// ファイル出力用のログレベルスイッチ
    /// </summary>
    private static readonly LoggingLevelSwitch FileLoggingLevelSwitch = new(Serilog.Events.LogEventLevel.Warning);

    /// <summary>
    /// ファイル出力のログレベルを文字列（"Warning"など）で動的に変更する
    /// </summary>
    /// <param name="levelName">Serilog.Events.LogEventLevelの名前（例: "Warning", "Information"）</param>
    internal static void SetFileLogLevel(string? levelName)
    {
        if (!string.IsNullOrEmpty(levelName) && Enum.TryParse<Serilog.Events.LogEventLevel>(levelName, true, out var level))
        {
            FileLoggingLevelSwitch.MinimumLevel = level;
        }
    }

    internal static void InitializeLoggerAndCleanupLogs(bool isDev)
    {
        // ログ出力先ディレクトリ決定
        string logsDir = isDev
            ? Path.Combine(Directory.GetCurrentDirectory(), "Logs")
            : Path.Combine(AppContext.BaseDirectory, "Logs");

        // Serilog構成
        string logFilePath = Path.Combine(logsDir, ".log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(
                outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}",
                restrictedToMinimumLevel: isDev
                    ? Serilog.Events.LogEventLevel.Verbose
                    : Serilog.Events.LogEventLevel.Information
            )
            .WriteTo.File(
                logFilePath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 7,
                levelSwitch: FileLoggingLevelSwitch
            )
            .CreateLogger();

        // ディレクトリがなければ作成
        if (!Directory.Exists(logsDir))
        {
            Directory.CreateDirectory(logsDir);
        }

        // アプリ終了時にロガーをフラッシュ
        AppDomain.CurrentDomain.ProcessExit += (_, __) => Log.CloseAndFlush();

        // 古いログ削除
        TryCleanupOldLogs(logsDir);
    }

    private static void TryCleanupOldLogs(string logsDir)
    {
        try
        {
            var oldLogs = Directory.GetFiles(logsDir, "*.log")
                .Where(f =>
                {
                    var fileName = Path.GetFileNameWithoutExtension(f);
                    if (DateTime.TryParseExact(fileName, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        return date < DateTime.Today.AddDays(-7);
                    }
                    return false;
                })
                .ToList();
            foreach (var oldLog in oldLogs)
            {
                try { File.Delete(oldLog); }
                catch (Exception ex) { Log.Warning(ex, "ログ削除失敗: {File}", oldLog); }
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "古いログ削除処理で例外");
        }
    }
}
