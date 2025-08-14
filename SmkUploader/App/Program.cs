
using System.Media;
using Microsoft.Extensions.Configuration;
using Serilog;
using SixLabors.ImageSharp;
using SmkUploader.Infra;
using SmkUploader.Service;
using SmkUploader.Service.Uploader;

namespace SmkUploader.App;

public class Program
{
    private const string SuccessSoundPath = @"C:\Windows\Media\Windows Notify System Generic.wav";
    private const string ErrorSoundPath = @"C:\Windows\Media\Windows Critical Stop.wav";
    public static string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
    private static bool HasWarning = false;
    internal static void SetHasWarning() => HasWarning = true;

    [STAThread]
    public static int Main(string[] args)
    {

        // Shiftキー押下検知 設定アプリを開いて自身は終了
        if ((System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift) == System.Windows.Forms.Keys.Shift)
        {
            try
            {
                var exePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SmkUploaderSetting.exe");
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to start new process: {ex.Message}");
            }
            return 0;
        }

        // 開発・本番判定
        var isDev = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SMKU_ENV"));
        if (isDev)
        {
            Console.WriteLine("開発モードで実行中です。");
        }

        // 基盤初期化（ロガー・ログディレクトリ等）
        LoggerInitializer.InitializeLoggerAndCleanupLogs(isDev);
        // アプリ終了時にロガーをフラッシュ
        AppDomain.CurrentDomain.ProcessExit += (_, __) => Log.CloseAndFlush();

        Log.Information("--- SmkUploader ver.{Version} ---", Version);

        if (args.Contains("--help") || args.Contains("-h"))
        {
            PrintUsage();
            return 0;
        }
        if (args.Contains("--version") || args.Contains("-v"))
        {
            Console.WriteLine(Version);
            return 0;
        }

        // 設定と位置引数の取得
        var (config, positionalArgs) = InputProvider.GetConfigurationAndArgs(args, isDev);
        LoggerInitializer.SetFileLogLevel(config.GetValue<string>("SMKU_LOG_LEVEL"));

        // 設定値
        Log.Debug("オプション:");
        config.AsEnumerable().ToList().ForEach(kvp => Log.Debug("  {Key}: {Value}", kvp.Key, kvp.Value));

        // 位置引数
        Log.Debug("引数: {Args}", string.Join(", ", positionalArgs));

        if (!InputProvider.ValidateConfiguration(config))
        {
            Log.Error("設定が不正です。");
            return InteractiveExit(config, 9);
        }

        try
        {
            using var uploader = UploaderFactory.Create(config, isDev);

            // 入力画像の取得
            var inputImage = new InputImage();
            using var image = inputImage.Get(positionalArgs.FirstOrDefault(), out var ext);

            // リサイズ処理
            using var resizeImage = new ResizeImage();
            resizeImage.Resize(
                image,
                config.GetValue<int>("SMKU_MAX_WIDTH"),
                config.GetValue<int>("SMKU_MAX_HEIGHT")
            );

            // アップローダーでサポートされていない形式の場合は警告を出し、最初のサポートされている拡張子を使用
            if (!uploader.SupportedExtensions.Contains(ext))
            {
                ext = uploader.SupportedExtensions[0]; // サポートされている拡張子の最初のものを使用
                Log.Warning("画像形式が不明またはアップローダーがサポートしていないため、 {Extension} を使用します。描画の不具合が発生する可能性があります。アップローダーがサポートしている形式: {SupportedExtensions}", ext, string.Join(", ", uploader.SupportedExtensions));
                SetHasWarning();
            }
            SixLabors.ImageSharp.Formats.IImageEncoder encoder = ext.ToLower() switch
            {
                ".jpg" or ".jpeg" => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder(),
                ".png" => new SixLabors.ImageSharp.Formats.Png.PngEncoder(),
                ".gif" => new SixLabors.ImageSharp.Formats.Gif.GifEncoder(),
                ".bmp" => new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder(),
                ".tga" => new SixLabors.ImageSharp.Formats.Tga.TgaEncoder(),
                _ => new SixLabors.ImageSharp.Formats.Png.PngEncoder(), // fallback
            };
            // 画像をMemoryStreamに保存し、アップローダーに渡す
            using var ms = new MemoryStream();
            image.Save(ms, encoder);
            ms.Position = 0;
            var uploadedUrl = uploader.UploadAsync(ms, $"upload{ext}", config, isDev).GetAwaiter().GetResult();
            Log.Debug("アップロード後画像URL: {Url}", uploadedUrl);
            Console.WriteLine($"{uploadedUrl}");

            // クリップボード登録 or ポーズ
            var setClipboard = config.GetValue<bool>("SMKU_SET_CLIPBOARD");
            if (setClipboard)
            {
                var thread = new System.Threading.Thread(() => System.Windows.Forms.Clipboard.SetText(uploadedUrl));
                thread.SetApartmentState(System.Threading.ApartmentState.STA);
                thread.Start();
                thread.Join();
                Log.Debug("URLをクリップボードにコピーしました。");
                if (HasWarning)
                {
                    // 警告があるときは画面が閉じないようにしてあげる
                    return InteractiveExit(config, 1);
                }
                else
                {
                    return InteractiveExit(config, 0);
                }
            }
            else
            {
                return InteractiveExit(config, 0);
            }
        }
        catch (ImageParseException ex)
        {
            Log.Error(ex, "入力画像の解析に失敗しました。 {Message}", ex.Message);
            return InteractiveExit(config, 9);
        }
        catch (UploaderException ex)
        {
            Log.Error(ex, "アップローダーエラーが発生しました。 {Message}", ex.Message);
            Console.Error.WriteLine($"Uploader error: {ex.Message}");
            return InteractiveExit(config, 9);
        }
        catch (ArgumentException ex)
        {
            Log.Error(ex, "設定または入力画像が不正です。 {Message}", ex.Message);
            PrintUsage();
            return InteractiveExit(config, 9);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "予期しないエラー: {Message}", ex.Message);
            return InteractiveExit(config, 9);
        }
    }

    private static int InteractiveExit(IConfiguration config, int exitCode = 0)
    {
        if (!config.GetValue<bool>("SMKU_NO_INTERACTIVE"))
        {
            if (exitCode == 0)
            {
                if (File.Exists(SuccessSoundPath))
                {
                    new SoundPlayer(SuccessSoundPath).PlaySync();
                }
            }
            else
            {
                if (File.Exists(ErrorSoundPath))
                {
                    new SoundPlayer(ErrorSoundPath).PlaySync();
                }
                Console.WriteLine("エンターキーを押すと終了します...");
                Console.ReadLine();
            }
        }
        return exitCode;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("画像をアップロードしURLを取得するツールです。画像の指定方法は複数あります:");
        Console.WriteLine("  - クリップボードに登録されたファイル・画像");
        Console.WriteLine("  - クリップボードに登録された パス/URL のテキスト");
        Console.WriteLine("  - コマンドライン引数での パス/URL の指定");
        Console.WriteLine("");
        Console.WriteLine("Shiftキーを押しながら起動すると設定画面が開きます。");
        Console.WriteLine("");
        Console.WriteLine("詳細はドキュメントサイトを参照してください。");
        Console.WriteLine("https://docs.google.com/document/d/1Lng8lue28fDLf1T5D6NUervUHl-wxoci__3t3xWaQc4");
        Console.WriteLine("");
        Console.WriteLine("コマンド書式: SmkUploader.exe [options] [image_path|url]");
        Console.WriteLine("");
        Console.WriteLine("オプション:");
        Console.WriteLine("  --help,-h    このヘルプメッセージを表示");
        Console.WriteLine("  --version,-v バージョン情報を表示");
    }
}
