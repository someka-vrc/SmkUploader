using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace SmkUploader.Service
{
    internal class ResizeImage : IDisposable
    {
        // private string? _tempFilePath; // 不要になるためコメントアウトまたは削除

        /// <summary>
        /// 画像を指定されたサイズにリサイズします。指定の幅と高さが元の画像のサイズ以上の場合は、元の画像のサイズを維持します。
        /// そうでない場合は、指定された幅と高さに収まるように縦横比を維持してリサイズします。
        /// </summary>
        /// <param name="image">リサイズ対象のImageオブジェクト</param>
        /// <param name="width">0以下の場合は無制限</param>
        /// <param name="height">0以下の場合は無制限</param>
        internal void Resize(Image image, int width, int height)
        {
            int srcW = image.Width;
            int srcH = image.Height;

            int targetW = width > 0 ? width : srcW;
            int targetH = height > 0 ? height : srcH;
            if (targetW >= srcW && targetH >= srcH)
            {
                // リサイズ不要: 元画像をそのまま返す
                Log.Debug("リサイズ不要: {Width}x{Height}", srcW, srcH);
            }
            else
            {
                double ratioW = width > 0 ? (double)width / srcW : double.MaxValue;
                double ratioH = height > 0 ? (double)height / srcH : double.MaxValue;
                double ratio = Math.Min(ratioW, ratioH);
                targetW = (int)Math.Round(srcW * ratio);
                targetH = (int)Math.Round(srcH * ratio);
                if (targetW <= 0) targetW = 1;
                if (targetH <= 0) targetH = 1;
                Log.Debug("リサイズ: {Width}x{Height} -> {TargetWidth}x{TargetHeight}", srcW, srcH, targetW, targetH);

                // 元のImageインスタンスを直接リサイズして返す
                image.Mutate(x => x.Resize(targetW, targetH));
            }
        }

        public void Dispose() { }
    }
}