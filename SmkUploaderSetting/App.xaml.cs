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
            // �A�N�Z���g�J���[���擾���ă��\�[�X�ɓo�^
            var accentColor = AccentColorHelper.GetAccentColor();
            var accentBrush = new SolidColorBrush(accentColor);
            accentBrush.Freeze();
            Resources["AccentBrush"] = accentBrush;
        }
    }
}
