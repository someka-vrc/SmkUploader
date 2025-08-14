using System.Windows;
using System.Windows.Media;
using SmkUploaderSetting.Helpers;

namespace SmkUploaderSetting
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // アクセントカラーを取得してリソースに登録
            var accentColor = AccentColorHelper.GetAccentColor();
            var accentBrush = new SolidColorBrush(accentColor);
            accentBrush.Freeze();
            Resources["AccentBrush"] = accentBrush;
        }
    }
}
