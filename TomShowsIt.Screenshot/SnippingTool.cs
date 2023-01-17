using System.Windows.Media.Imaging;
using System.Windows;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Size = System.Drawing.Size;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace TomShowsIt.Screenshot
{
    public static class SnippingTool
    {
        private static readonly double BackgroundOpacity = 0.5;
        private static readonly Brush SelectionRectangleBorderBrush = Brushes.CadetBlue;

        public static BitmapSource? CaptureRegion()
        {
            var bitmap = CaptureAllScreens();

            var left = SystemParameters.VirtualScreenLeft;
            var top = SystemParameters.VirtualScreenTop;
            var right = left + SystemParameters.VirtualScreenWidth;
            var bottom = right + SystemParameters.VirtualScreenHeight;

            var window = new RegionSelectionWindow
            {
                BackgroundImage =
                {
                    Source = bitmap,
                    Opacity = BackgroundOpacity,
                },
                InnerBorder =
                {
                    BorderBrush = SelectionRectangleBorderBrush
                },
                Left = left,
                Top = top,
                Width = right - left,
                Height = bottom - top
            };

            window.ShowDialog();

            if (window.SelectedRegion == null)
            {
                return null;
            }

            return GetBitmapRegion(bitmap, window.SelectedRegion.Value);
        }

        private static BitmapSource CaptureAllScreens()
        {
            return CaptureRegion(new Rect(SystemParameters.VirtualScreenLeft,
                                          SystemParameters.VirtualScreenTop,
                                          SystemParameters.VirtualScreenWidth,
                                          SystemParameters.VirtualScreenHeight));
        }

        private static BitmapSource CaptureRegion(Rect rect)
        {
            using var bitmap = new Bitmap((int)rect.Width, (int)rect.Height, PixelFormat.Format32bppArgb);

            var graphics = Graphics.FromImage(bitmap);

            graphics.CopyFromScreen((int)rect.X, (int)rect.Y, 0, 0, new Size((int)rect.Size.Width, (int)rect.Size.Height),
                                    CopyPixelOperation.SourceCopy);

            return bitmap.ToBitmapSource();
        }

        private static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(stream.ToArray());
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        private static BitmapSource? GetBitmapRegion(BitmapSource bitmap, Rect rect)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return null;
            }

            return new CroppedBitmap(bitmap, new Int32Rect
            {
                X = (int)rect.X,
                Y = (int)rect.Y,
                Width = (int)rect.Width,
                Height = (int)rect.Height
            });
        }
    }
}
