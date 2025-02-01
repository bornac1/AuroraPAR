using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AuroraPAR
{
    public partial class MainWindow : Window
    {
        private int qnh = 1024;
        private readonly System.Timers.Timer timer;
        private readonly ProfileView profileView;
        private readonly Distance[] distances = new Distance[] {5, 10, 15, 20};
        private Runway runway = new()
        {
            Designator = "05",
            Elevation = 0,
            Latitude = 0,
            Longitude = 0,
            LengthM = 1000,
            Distance = 10
        };
        public MainWindow()
        {
            InitializeComponent();
            DistanceComboBox.ItemsSource = distances;
            DistanceComboBox.SelectedIndex = 1;//10 nm
            DistanceComboBox.SelectionChanged += DistanceComboBox_SelectionChanged;
            profileView = new(Vertical, runway);
            timer = new();
            timer.Start();
            timer.Interval = 100;
            timer.Elapsed += Timer_Elapsed;
        }
        private void DistanceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            runway.Distance = (Distance)e.AddedItems[0];
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
                Aircraft aircraft1 = new()
                {
                    Latitude = 0.1,
                    Longitude = 0,
                    Altitde = 2000
                };
                Aircraft aircraft2 = new()
                {
                    Latitude = 0.15,
                    Longitude = 0,
                    Altitde = 3200
                };
                profileView.Draw([aircraft1, aircraft2]);
            });
        }
        private void DrawInfo()
        {
            TextBlock info = new()
            {
                Text = $"RWY {runway.Designator}\nQNH {qnh}",
                FontSize = 14,
                Foreground = Brushes.White
            };
            Canvas.SetLeft(info, 0);
            Canvas.SetTop(info, 0);
            Vertical.Children.Add(info);
        }
    }
}
