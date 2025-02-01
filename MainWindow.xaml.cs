using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AuroraPAR
{
    public partial class MainWindow : Window
    {
        private int runway = 5;
        private int qnh = 1024;
        private System.Timers.Timer timer;
        private ProfileView profileView;
        public MainWindow()
        {
            InitializeComponent();
            profileView = new(Vertical);
            timer = new();
            timer.Start();
            timer.Interval = 100;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Draw();
            timer.Start();
        }
        private void Draw()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Vertical.Children.Clear();
                DrawInfo();
                profileView.Draw();
            });
        }
        private void DrawInfo()
        {
            TextBlock info = new()
            {
                Text = $"RWY {runway}\nQNH {qnh}",
                FontSize = 14,
                Foreground = Brushes.White
            };
            Canvas.SetLeft(info, 0);
            Canvas.SetTop(info, 0);
            Vertical.Children.Add(info);
        }
    }
}
