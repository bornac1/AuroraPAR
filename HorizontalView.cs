using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AuroraPAR
{
    internal class HorizontalView(Canvas canvas, Runway runway)
    {
        private double xscale = 1;
        private double yscale = 1;
        Canvas Canvas = canvas;
        private Runway Runway = runway;
        private void CalculateScale()
        {
            xscale = (Canvas.ActualWidth - 50) / (Runway.Distance+Runway.LengthNM);
            yscale = (Canvas.ActualHeight - 50) / (Runway.Distance * Math.Tan(20*double.Pi/180));
        }
        public void Draw(List<Aircraft> aircrafts)
        {
            CalculateScale();
            Line runwayh = new()
            {
                X1 = 0,
                Y1 = Canvas.ActualHeight / 2,
                X2 = Runway.LengthNM * xscale,
                Y2 = Canvas.ActualHeight / 2,
                Stroke = Brushes.Green,
                StrokeThickness = 3
            };
            Canvas.Children.Add(runwayh);
            Line runwayv = new()
            {
                X1 = Runway.LengthNM * xscale,
                Y1 = (Canvas.ActualHeight / 2) - (Runway.WidthNM * yscale),
                X2 = Runway.LengthNM * xscale,
                Y2 = (Canvas.ActualHeight / 2) + (Runway.WidthNM * yscale),
                Stroke = Brushes.Green,
                StrokeThickness = 3
            };
            Canvas.Children.Add(runwayv);
            Line upper = new()
            {
                X1 = 0,
                Y1 = Canvas.ActualHeight / 2,
                X2 = (Runway.LengthNM + Runway.Distance) * xscale,
                Y2 = (Canvas.ActualHeight / 2) - ((Runway.Distance * Math.Tan((10) * double.Pi / 180)) * yscale),
                Stroke = Brushes.CadetBlue,
                StrokeThickness = 3
            };
            Canvas.Children.Add(upper);
            Line lower = new()
            {
                X1 = 0,
                Y1 = Canvas.ActualHeight / 2,
                X2 = (Runway.LengthNM + Runway.Distance) * xscale,
                Y2 = (Canvas.ActualHeight / 2) + ((Runway.Distance * Math.Tan((10) * double.Pi / 180)) * yscale),
                Stroke = Brushes.CadetBlue,
                StrokeThickness = 3
            };
            Canvas.Children.Add(lower);
            Line axis = new()
            {
                X1 = Runway.LengthNM * xscale,
                Y1 = Canvas.ActualHeight / 2,
                X2 = (Runway.LengthNM + Runway.Distance) * xscale,
                Y2 = Canvas.ActualHeight / 2,
                Stroke = Brushes.Yellow,
                StrokeThickness = 2
            };
            Canvas.Children.Add(axis);
            Line axisP15 = new()
            {
                X1 = Runway.LengthNM * xscale,
                Y1 = Canvas.ActualHeight / 2,
                X2 = (Runway.LengthNM + Runway.Distance) * xscale,
                Y2 = (Canvas.ActualHeight / 2) + (Runway.Distance * Math.Tan((1.5) * double.Pi / 180)*yscale),
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            Canvas.Children.Add(axisP15);
            Line axisM15 = new()
            {
                X1 = Runway.LengthNM * xscale,
                Y1 = Canvas.ActualHeight / 2,
                X2 = (Runway.LengthNM + Runway.Distance) * xscale,
                Y2 = (Canvas.ActualHeight / 2) - (Runway.Distance * Math.Tan((1.5) * double.Pi / 180) * yscale),
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            Canvas.Children.Add(axisM15);
            for (int i = 1; i <= 10; i++)
            {
                SolidColorBrush stroke = (i % 5 == 0) ? Brushes.Orange : Brushes.Green;
                double x = (i * Runway.Distance / 10.0 + Runway.LengthNM) * xscale;

                Line distance = new()
                {
                    X1 = x,
                    Y1 = Canvas.ActualHeight / 2 + ((x / xscale) / (Runway.LengthNM + Runway.Distance) * Runway.Distance * Math.Tan(10 * Math.PI / 180) * yscale),
                    X2 = x,
                    Y2 = Canvas.ActualHeight / 2 - ((x / xscale) / (Runway.LengthNM + Runway.Distance) * Runway.Distance * Math.Tan(10 * Math.PI / 180) * yscale),
                    Stroke = stroke,
                    StrokeThickness = 1
                };

                Canvas.Children.Add(distance);
            }
            foreach (Aircraft aircraft in aircrafts)
            {
                if (aircraft.IsDisplayed(Runway))
                {
                    DrawAircraft(aircraft);
                }
            }
        }
        private void DrawAircraft(Aircraft aircraft)
        {
            Ellipse elipse = new()
            {
                Height = 10,
                Width = 10,
                Stroke = Brushes.White,
                Fill = Brushes.White
            };
            double top = (Canvas.ActualHeight / 2) - aircraft.LateralOffset(Runway) * yscale - 5;
            if (top < 0) return;
            Canvas.SetTop(elipse, top);
            Canvas.SetLeft(elipse, (aircraft.Distance(Runway) + Runway.LengthNM) * xscale - 5);
            Canvas.Children.Add(elipse);
            TextBlock textBlock = new()
            {
                Text = $"{aircraft.Callsign}\n{aircraft.Altitude}\n\n{(aircraft.BearingFromRunway(runway) - runway.Heading + 360) % 360 - 180}°",
                FontSize = 12,
                Foreground = Brushes.White
            };
            Canvas.SetTop(textBlock, (Canvas.ActualHeight / 2)-(aircraft.LateralOffset(Runway) * yscale + 35) - 5);
            Canvas.SetLeft(textBlock, ((aircraft.Distance(Runway) + Runway.LengthNM) * xscale) - 15);
            Canvas.Children.Add(textBlock);
        }
    }
}
