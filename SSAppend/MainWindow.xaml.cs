﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;

namespace SSAppend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        private List<Bitmap> _screenshots = new List<Bitmap>();
        private Bitmap finalImage;
        #endregion Properties
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            CaptureScreen();
        }

        private void CaptureScreen()
        {
            this.Hide();
            System.Threading.Thread.Sleep(300);

            System.Drawing.Rectangle area = SelectArea();
            if (area.Width > 0 && area.Height > 0)
            {
                Bitmap screenshot = CaptureWindow(area);
                _screenshots.Add(screenshot);
                finalImage = AppendCaptures();
                Clipboard.SetImage(BitmapCovert(finalImage));
                imgPreview.Source = BitmapCovert(finalImage);
            }
            this.Show();
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            _screenshots.Clear();
            finalImage = null;
            imgPreview.Source = null;
        }

        private System.Drawing.Rectangle SelectArea()
        {
            using (var selection = new FormSelection())
            {
                return selection.SelectArea();
            }
        }

        private Bitmap CaptureWindow(System.Drawing.Rectangle area)
        {
            Bitmap screenshot = new Bitmap(area.Width, area.Height);
            using (Graphics gfx = Graphics.FromImage(screenshot))
            {
                gfx.CopyFromScreen(area.Location, System.Drawing.Point.Empty, area.Size);
            }
            return screenshot;
        }

        private Bitmap AppendCaptures()
        {
            if (_screenshots.Count == 0) return null;

            int maxWidth = 0;
            int totalHeight = 0;

            foreach (var img in _screenshots)
            {
                maxWidth = Math.Max(maxWidth, img.Width);
                totalHeight += img.Height;
            }

            Bitmap result = new Bitmap(maxWidth, totalHeight);
            using (Graphics g = Graphics.FromImage(result))
            {
                int yOffset = 0;
                foreach (var img in _screenshots)
                {
                    g.DrawImage(img, 0, yOffset);
                    yOffset += img.Height;
                }
            }
            return result;
        }

        private BitmapImage BitmapCovert(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
