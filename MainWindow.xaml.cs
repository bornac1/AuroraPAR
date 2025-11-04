using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AuroraPAR
{
    public partial class MainWindow : Window
    {
        private int qnh = 0;
        private readonly System.Timers.Timer timer;
        private ProfileView profileView;
        private HorizontalView horizontalView;
        private readonly Aurora aurora;
        private readonly Distance[] distances = new Distance[] {1, 2.5, 5, 10, 15, 20};
        private string dataPath = "runways.par";
        private Runway runway = new()
        {
            ICAO = "EDDF",
            Designator = "TEST",
            Elevation = 364,
            Heading = 70,
            Latitude = 50.02766757167606,
            Longitude = 8.534360261721835,
            LengthM = 1000,
            WidthM = 60,
            Distance = 10
        };
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            DistanceComboBox.ItemsSource = distances;
            DistanceComboBox.SelectedIndex = 1;//10 nm
            DistanceComboBox.SelectionChanged += DistanceComboBox_SelectionChanged;
            profileView = new(Vertical, runway);
            horizontalView = new(Horizontal, runway);
            aurora = new();
            this.Closing += MainWindow_Closing;
            timer = new();
            timer.Start();
            timer.Interval = 100;
            timer.Elapsed += Timer_Elapsed;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            aurora.Close();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RunwayComboBox.ItemsSource = await DataFile.GetRunways(dataPath);
            RunwayComboBox.SelectionChanged += RunwayComboBox_SelectionChanged;
            await aurora.Connect();
        }

        private void RunwayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] is Runway r)
            {
                runway = r;
                profileView = new(Vertical, runway);
                horizontalView = new(Horizontal, runway);
                int i = Array.IndexOf(distances, runway.Distance);
                if(i != -1)
                {
                    DistanceComboBox.SelectedIndex = i;
                }
            }
        }

        private void DistanceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ( e.AddedItems.Count > 0 && e.AddedItems[0] is Distance d)
            {
                runway.Distance = d;
            }
        }
        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            string[] callsigns = await aurora.GetTrafficList();
            int nqnh = await aurora.GetQNH(runway);
            if (nqnh != 0) {
                qnh = nqnh;
            }
            List<Aircraft> aircrafts = [];
            foreach(string callsign in callsigns)
            {
                Aircraft? aircraft = await aurora.GetTrafficPosition(callsign);
                if(aircraft != null)
                {
                    aircrafts.Add(aircraft);
                }
            }
            Draw(aircrafts);
            timer.Start();
        }
        private void Draw(List<Aircraft> aircrafts)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Vertical.Children.Clear();
                Horizontal.Children.Clear();
                DrawInfo();
                profileView.Draw(aircrafts);
                horizontalView.Draw(aircrafts);
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
        private void Window_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            int i = DistanceComboBox.SelectedIndex;
            if (e.Delta > 0)
            {
                if((i+1) < DistanceComboBox.Items.Count)
                {
                    DistanceComboBox.SelectedIndex = i + 1;
                }
            } else
            {
                if((i-1) >= 0)
                {
                    DistanceComboBox.SelectedIndex = i - 1;
                }
            }
        }
    }
}
