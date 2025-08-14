using System.Windows;
using System.Windows.Navigation;
using System.Diagnostics;

namespace SmkUploaderSetting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // 利用規約リンククリック時にブラウザで開く
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch { /* 必要ならエラー処理 */ }
            e.Handled = true;
        }
    }
}