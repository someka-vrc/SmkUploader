using System;
using System.Windows.Media;
using Microsoft.Win32;

namespace SmkUploaderSetting.Helpers
{
    public static class AccentColorHelper
    {
        public static Color GetAccentColor()
        {
            // Windows 10/11のアクセントカラーはレジストリから取得可能
            // HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM\ColorizationColor
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\DWM");
                if (key != null)
                {
                    var value = key.GetValue("ColorizationColor");
                    if (value is int colorValue)
                    {
                        // DWORDはAARRGGBBの順
                        byte a = (byte)((colorValue >> 24) & 0xFF);
                        byte r = (byte)((colorValue >> 16) & 0xFF);
                        byte g = (byte)((colorValue >> 8) & 0xFF);
                        byte b = (byte)(colorValue & 0xFF);
                        return Color.FromArgb(a, r, g, b);
                    }
                }
            }
            catch { }
            // 取得できなければデフォルト色
            return Color.FromRgb(0, 120, 215); // Windows標準アクセント色
        }
    }
}
