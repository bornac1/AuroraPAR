using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AuroraPAR
{
    public partial class MainWindow : Window
    {
        // Scaling factors for the canvas
        private const double XScale = 50.0; // Pixels per NM
        private const double YScale = 0.01; // Pixels per foot

        // Runway details
        private readonly double RunwayLat = 40.776927;
        private readonly double RunwayLon = -73.873966;
        private readonly double RunwayAlt = 12.0; // feet
        private readonly double MagneticHeading = 40.0;
        private readonly double MagneticDeclination = -13.0;

        // Glideslope angle (in degrees)
        private readonly double GlideslopeAngle = 3.0; // Default to 3 degrees
        private readonly double RunwayWidth = 6000.0; // in meters

        // Profile and Overhead View parameters
        private double ProfileViewWidth = 510;
        private double OverheadViewWidth = 510;

        public MainWindow()
        {
            InitializeComponent();
            InitializeRunway();
            UpdateAircraftPosition(40.786, -73.870, 4200.0); // Sample aircraft position
        }

        // Draw the runway and glideslope on the canvas
        private void InitializeRunway()
        {
            // Top Graph: Profile View
            DrawProfileView();

            // Bottom Graph: Overhead View
            DrawOverheadView();

            // Add runway label
            var rwyLabel = new TextBlock
            {
                Text = "RWY 16",
                Foreground = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(rwyLabel, 10);
            Canvas.SetTop(rwyLabel, 20);
            RunwayCanvas.Children.Add(rwyLabel);

            // Add QNH and STS label below the RWY label
            var qnhLabel = new TextBlock
            {
                Text = "QNH 1010\nSTS OK",
                Foreground = Brushes.White,
                FontSize = 14
            };
            Canvas.SetLeft(qnhLabel, 10);  // Align with RWY label
            Canvas.SetTop(qnhLabel, 40);  // Move below the RWY label
            RunwayCanvas.Children.Add(qnhLabel);
        }

        private void DrawProfileView()
        {
            // Calculate altitude difference for the glideslope over 10 NM
            double altitudeDifferencePerNM = Math.Tan(GlideslopeAngle * Math.PI / 180) * 6076; // Feet per NM
            double totalAltitudeDrop = altitudeDifferencePerNM * 10; // Altitude drop over 10 NM

            // Draw the runway line (horizontal) in the profile view with a lighter blue
            var runwayLine = new Line
            {
                X1 = 10,
                Y1 = 300,
                X2 = ProfileViewWidth,
                Y2 = 300,
                Stroke = new SolidColorBrush(Color.FromArgb(255, 173, 216, 230)),  // Light Blue
                StrokeThickness = 3 // Adjust thickness here
            };
            RunwayCanvas.Children.Add(runwayLine);

            // Draw the glideslope (yellow line)
            var glideslopeLine = new Line
            {
                X1 = 10,
                Y1 = 300, // Start at runway altitude
                X2 = ProfileViewWidth, // Extend to 10 NM
                Y2 = 300 - (totalAltitudeDrop * YScale),
                Stroke = Brushes.Yellow,
                StrokeThickness = 3 // Make it as thick as the green runway line
            };
            RunwayCanvas.Children.Add(glideslopeLine);

            // Add distance markers and labels (vertical dashed lines)
            for (int i = 1; i <= 10; i++)
            {
                var markerLine = new Line
                {
                    X1 = 10 + (i * (ProfileViewWidth / 10)), // Scaling for Profile View width
                    Y1 = 300,
                    X2 = 10 + (i * (ProfileViewWidth / 10)),
                    Y2 = 300 - (totalAltitudeDrop * YScale),
                    Stroke = (i == 5 || i == 10) ? Brushes.Orange : Brushes.Green, // Orange for 5 and 10, Green for others
                    StrokeDashArray = new DoubleCollection { 4, 4 },
                    StrokeThickness = 1
                };
                RunwayCanvas.Children.Add(markerLine);

                var markerLabel = new TextBlock
                {
                    Text = $"{i}NM",
                    Foreground = Brushes.White,
                    FontSize = 12
                };
                Canvas.SetLeft(markerLabel, 10 + (i * (ProfileViewWidth / 10)) - 10);
                Canvas.SetTop(markerLabel, 310);
                RunwayCanvas.Children.Add(markerLabel);
            }
        }

        private void DrawOverheadView()
        {
            // Use the already existing RunwayWidth in meters
            double runwayWidthMeters = RunwayWidth;

            // Calculate the runway width in pixels based on XScale
            double runwayWidthPixels = runwayWidthMeters * XScale; // Scaling runway width in pixels

            // Ensure that the overhead view has the same width as the profile view
            double scaledRunwayWidth = Math.Min(runwayWidthPixels, OverheadViewWidth); // Limit width to the overhead view width

            // Calculate the vertical height for the green line at the left end (scale based on runway width)
            double greenLineHeightPixels = runwayWidthMeters * YScale; // Convert runway width in meters to pixels for height

            // Draw the runway line (horizontal) in the overhead view with yellow
            var runwayLine = new Line
            {
                X1 = 10,
                Y1 = 500,  // Keep Y constant to ensure the line is horizontal
                X2 = 10 + scaledRunwayWidth,  // Length based on the width of the runway
                Y2 = 500,  // Keep Y constant to ensure the line is horizontal
                Stroke = Brushes.Yellow,  // Yellow color for overhead view
                StrokeThickness = 3 // Match the thickness of the yellow line in the profile view
            };
            RunwayCanvas.Children.Add(runwayLine);

            // Add distance markers (vertical dashed lines) scaled to the width of the runway
            for (int i = 1; i <= 10; i++)
            {
                var markerLine = new Line
                {
                    X1 = 10 + (i * (scaledRunwayWidth / 10)), // Adjust for scaling
                    Y1 = 450,
                    X2 = 10 + (i * (scaledRunwayWidth / 10)),
                    Y2 = 550,
                    Stroke = (i == 5 || i == 10) ? Brushes.Orange : Brushes.Green, // Orange for 5 and 10, Green for others
                    StrokeDashArray = new DoubleCollection { 4, 4 },
                    StrokeThickness = 1
                };
                RunwayCanvas.Children.Add(markerLine);

                var markerLabel = new TextBlock
                {
                    Text = $"{i}NM",
                    Foreground = Brushes.White,
                    FontSize = 12
                };
                Canvas.SetLeft(markerLabel, 10 + (i * (scaledRunwayWidth / 10)) - 10);
                Canvas.SetTop(markerLabel, 560);
                RunwayCanvas.Children.Add(markerLabel);
            }

            // Add a thicker green line at the left end (vertical height based on runway width)
            // Add a thicker green line at the left end (vertical height based on runway width)
            var leftGreenLine = new Line
            {
                X1 = 10,  // Keep X constant to make the line vertical
                Y1 = 490, // Start position at the bottom of the canvas
                X2 = 10,  // Same X position, ensuring the line is vertical
                Y2 = 490 - greenLineHeightPixels,  // Vertical length based on runway width
                Stroke = Brushes.Green,
                StrokeThickness = 6  // Make it as thick as the profile view's green line
            };
            RunwayCanvas.Children.Add(leftGreenLine);
        }

        // Update the aircraft position dynamically
        private void UpdateAircraftPosition(double aircraftLat, double aircraftLon, double aircraftAlt)
        {
            // Clear previous aircraft positions
            RunwayCanvas.Children.Clear();
            InitializeRunway();

            // Update Profile View (Top Graph)
            var (x, y) = PositionMapper.MapToCanvas(
                aircraftLat, aircraftLon, aircraftAlt,
                RunwayLat, RunwayLon, RunwayAlt,
                MagneticHeading, MagneticDeclination,
                XScale, YScale
            );
            y = 300 - y; // Adjust for canvas inversion

            var aircraftDotProfile = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.White
            };
            Canvas.SetLeft(aircraftDotProfile, x + 10);
            Canvas.SetTop(aircraftDotProfile, y - 5);
            RunwayCanvas.Children.Add(aircraftDotProfile);

            // Add aircraft label to the profile view
            var aircraftLabelProfile = new TextBlock
            {
                Text = $"A1234\n{aircraftAlt:F0} ft",
                Foreground = Brushes.White,
                FontSize = 12
            };
            Canvas.SetLeft(aircraftLabelProfile, x + 20);
            Canvas.SetTop(aircraftLabelProfile, y - 15);
            RunwayCanvas.Children.Add(aircraftLabelProfile);

            // Update Overhead View (Bottom Graph)
            var aircraftDotOverhead = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.White
            };
            Canvas.SetLeft(aircraftDotOverhead, x + 10);
            Canvas.SetTop(aircraftDotOverhead, 500 - 5);
            RunwayCanvas.Children.Add(aircraftDotOverhead);

            // Add aircraft label to the overhead view
            var aircraftLabelOverhead = new TextBlock
            {
                Text = $"A1234\n{aircraftAlt:F0} ft",
                Foreground = Brushes.White,
                FontSize = 12
            };
            Canvas.SetLeft(aircraftLabelOverhead, x + 20);
            Canvas.SetTop(aircraftLabelOverhead, 510);  // Position below the aircraft dot
            RunwayCanvas.Children.Add(aircraftLabelOverhead);

            // Update aircraft status
            AircraftStatusLabel.Text = $"Aircraft Position: Lat={aircraftLat:F4}, Lon={aircraftLon:F4}, Alt={aircraftAlt} ft";
        }
    }
}
