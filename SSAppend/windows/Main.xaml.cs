using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace SSAppend
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        #region Properties
        private List<Bitmap> _screenshots = new List<Bitmap>();
        private Bitmap finalImage;
        #endregion Properties

        #region Constants
        #endregion Constants
        public Main()
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

            using (var selection = new FormSelection())
            {
                selection.ShowDialog();
                System.Drawing.Rectangle area = new System.Drawing.Rectangle(
                    Math.Min(selection.StartPoint.X, selection.EndPoint.X),
                    Math.Min(selection.StartPoint.Y, selection.EndPoint.Y),
                    Math.Abs(selection.EndPoint.X - selection.StartPoint.X),
                    Math.Abs(selection.EndPoint.Y - selection.StartPoint.Y)
                );
                if (area.Width > 0 && area.Height > 0)
                {
                    Bitmap screenshot = CaptureWindow(area);
                    _screenshots.Add(screenshot);
                    finalImage = AppendCaptures();
                    Clipboard.SetImage(BitmapCovert(finalImage));
                    imgPreview.Source = BitmapCovert(finalImage);
                }
            }

            this.Show();
        }


        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            _screenshots.Clear();
            finalImage = null;
            imgPreview.Source = null;
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
        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (finalImage == null) return;
            Clipboard.SetImage(BitmapCovert(finalImage));
        }

        private void Image_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (finalImage == null) return;

            string date = DateTime.Now.ToString("yyyy-MM-dd HHmmss");

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp",
                Title = "Save image",
                FileName = "ScreenShot " + date,
                DefaultExt = ".png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    finalImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show("Image saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving the image: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
