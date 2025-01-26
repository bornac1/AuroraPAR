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

        public MainWindow()
        {
            InitializeComponent();
            InitializeRunway();
            UpdateAircraftPosition(40.786, -73.870, 4200.0); // Sample aircraft position
        }

        // Draw the runway and glideslope on the canvas
        private void InitializeRunway()
        {
            // Draw the runway line (horizontal)
            var runwayLine = new Line
            {
                X1 = 10,
                Y1 = 300,
                X2 = 510,
                Y2 = 300,
                Stroke = Brushes.Green,
                StrokeThickness = 3
            };
            RunwayCanvas.Children.Add(runwayLine);

            // Calculate altitude difference for the glideslope over 10 NM
            double altitudeDifferencePerNM = Math.Tan(GlideslopeAngle * Math.PI / 180) * 6076; // Feet per NM
            double totalAltitudeDrop = altitudeDifferencePerNM * 10; // Altitude drop over 10 NM

            // Draw the glideslope
            var glideslopeLine = new Line
            {
                X1 = 10,
                Y1 = 300, // Start at runway altitude
                X2 = 510, // Extend to 10 NM
                Y2 = 300 - (totalAltitudeDrop * YScale),
                Stroke = Brushes.Yellow,
                StrokeThickness = 2
            };
            RunwayCanvas.Children.Add(glideslopeLine);

            // Add runway label
            RunwayLabel.Text = $"Runway 16 (True Heading: {MagneticHeading + MagneticDeclination}°)";
        }

        // Update the aircraft position dynamically
        private void UpdateAircraftPosition(double aircraftLat, double aircraftLon, double aircraftAlt)
        {
            // Map the aircraft position to canvas coordinates
            var (x, y) = PositionMapper.MapToCanvas(
                aircraftLat, aircraftLon, aircraftAlt,
                RunwayLat, RunwayLon, RunwayAlt,
                MagneticHeading, MagneticDeclination,
                XScale, YScale
            );

            // Adjust Y for canvas (inverted Y-axis)
            y = 300 - y;

            // Create a dot to represent the aircraft
            var aircraftDot = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.White
            };

            // Position the aircraft dot on the canvas
            Canvas.SetLeft(aircraftDot, x + 10); // Offset by 10 for runway start
            Canvas.SetTop(aircraftDot, y - 5); // Offset by 5 to center dot
            RunwayCanvas.Children.Add(aircraftDot);

            // Update aircraft status
            AircraftStatusLabel.Text = $"Aircraft Position: Lat={aircraftLat:F4}, Lon={aircraftLon:F4}, Alt={aircraftAlt} ft";
        }
    }
}