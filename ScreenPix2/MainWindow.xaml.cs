using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ScreenPix2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private DispatcherTimer _timer;
        private int _imgWidth = 25;
        private int _imgHeight = 25;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMicroseconds(10);
            _timer.Tick += UpdateImage;
        }

        private void UpdateImage(object? sender, EventArgs e)
        {
            var position = MouseHelper.GetCursorPosition();
            using (System.Drawing.Bitmap img = ImageHelper.GetScreenshot((int)position.X - _imgWidth / 2, (int)position.Y - _imgHeight / 2, _imgWidth, _imgHeight))
            {
                MakeCrosshair(img);

                var pixel = img.GetPixel(img.Width / 2, img.Height / 2);
                string rgb = $"{pixel.R},{pixel.G},{pixel.B}";
                string hex = "#" + pixel.R.ToString("X2") + pixel.G.ToString("X2") + pixel.B.ToString("X2");

                ColorCanvas.Background = GetBrushFromHex(hex);
                RGBColorTextblock.Text = rgb;
                HexColorTextblock.Text = hex;
                ScreenshotImage.Source = ImageHelper.BitmapToImageSource(img);
            }
        }

        private void MakeCrosshair(System.Drawing.Bitmap img)
        {
            Point center = new Point(img.Width / 2, img.Height / 2);
            img.SetPixel((int)center.X - 1, (int)center.Y, System.Drawing.Color.Black);
            img.SetPixel((int)center.X + 1, (int)center.Y, System.Drawing.Color.Black);
            img.SetPixel((int)center.X, (int)center.Y - 1, System.Drawing.Color.Black);
            img.SetPixel((int)center.X, (int)center.Y + 1, System.Drawing.Color.Black);
        }

        private Brush GetBrushFromHex(string hex)
        {
            BrushConverter bc = new BrushConverter();
            var brush = bc.ConvertFrom(hex);
            if (brush == null)
                throw new ArgumentNullException($"Error! Could not convert {hex} into a brush!");
            return (Brush)brush;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S)
            {
                if (_timer.IsEnabled)
                {
                    StatusBorder.BorderBrush = Brushes.Red;
                    _timer.Stop();
                }
                else
                {
                    StatusBorder.BorderBrush = Brushes.Transparent;
                    _timer.Start();
                }
            }
            else if (e.Key == Key.Up)
            {
                var position = MouseHelper.GetCursorPosition();
                SetCursorPos((int)position.X, (int)position.Y - 1);
            }
            else if (e.Key == Key.Down)
            {
                var position = MouseHelper.GetCursorPosition();
                SetCursorPos((int)position.X, (int)position.Y + 1);
            }
            else if (e.Key == Key.Left)
            {
                var position = MouseHelper.GetCursorPosition();
                SetCursorPos((int)position.X - 1, (int)position.Y);
            }
            else if (e.Key == Key.Right)
            {
                var position = MouseHelper.GetCursorPosition();
                SetCursorPos((int)position.X + 1, (int)position.Y);
            }
        }
    }
}
