using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Point = System.Windows.Point;

namespace TomShowsIt.Editor
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class TomShowsItEditor : Window
    {
        private readonly BitmapSource _originalImage;
        private readonly Bitmap _tomBmp;

        public TomShowsItEditor(BitmapSource originalImage)
        {
            InitializeComponent();
            Width = originalImage.Width;
            Height = originalImage.Height;
            EditorImage.Source = originalImage;
            _originalImage = originalImage;

            _tomBmp = new Bitmap("Tom.png");
        }

        public void AddTomRight(object? sender, MouseButtonEventArgs args)
        {
            var pos = args.GetPosition(EditorImage);
            AddTom(false, pos);
        }

        public void AddTomLeft(object? sender, MouseButtonEventArgs args)
        {
            var pos = args.GetPosition(EditorImage);
            AddTom(true, pos);
        }

        private void AddTom(bool flipped, Point pos)
        {
            var tomBmp = (Bitmap)_tomBmp.Clone();
            var newImage = (Bitmap)_originalImage.GetBitmap().Clone();

            var offsetY = 470;
            var offsetX = 0;
            var maxWidth = (int)(newImage.Width / 3);
            var maxHeight = newImage.Height - 50;

            if (flipped)
            {
                offsetX = tomBmp.Width;
                tomBmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                maxWidth = Math.Min(maxWidth, (int)pos.X);
            }
            else
            {
                maxWidth = Math.Min(maxWidth, (int)(newImage.Width - pos.X));
            }

            var scaleFactor = Math.Min(maxWidth / (float)tomBmp.Width, maxHeight / (float)tomBmp.Height);

            using var g = Graphics.FromImage(newImage);
            g.CompositingMode = CompositingMode.SourceOver;
            tomBmp.MakeTransparent();

            MessageBox.Show($"{(float)pos.X - offsetX * scaleFactor}\n{(float)pos.Y - offsetY * scaleFactor}\n{tomBmp.Width * scaleFactor}\n{tomBmp.Height * scaleFactor}");
            g.DrawImage(tomBmp, (float)pos.X - offsetX * scaleFactor, (float)pos.Y - offsetY * scaleFactor, tomBmp.Width * scaleFactor, tomBmp.Height * scaleFactor);

            EditorImage.Source = newImage.GetSource();
        }

        public void OnKeyPressed(object? sender, KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                DialogResult = true;
                Close();
            }
            else if (args.Key == Key.Escape)
            {
                Close();
            }
        }

        public Image GetResult()
        {
            using var ms = new MemoryStream();
            var bbe = new BmpBitmapEncoder();

            var bmp = (BitmapSource)EditorImage.Source;

            bbe.Frames.Add(BitmapFrame.Create(bmp));
            bbe.Save(ms);

            return Image.FromStream(ms);
        }
    }
}
