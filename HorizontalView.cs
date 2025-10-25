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
        Canvas canvas = canvas;
        Runway _runway = runway;
        public Runway Runway
        {
            get { return _runway; }
            set { _runway = value; }
        }
        private void CalculateScale()
        {
            xscale = canvas.ActualWidth / (1.1 * _runway.Distance + 2);
            yscale = (canvas.ActualHeight - 0) / ((1.1 * _runway.Distance) + 2);
        }
        public void Draw(List<Aircraft> aircrafts)
        {
            CalculateScale();
            Line runwayh = new()
            {
                X1 = 0,
                Y1 = canvas.ActualHeight / 2,
                X2 = _runway.LengthNM * xscale,
                Y2 = canvas.ActualHeight / 2,
                Stroke = Brushes.Green,
                StrokeThickness = 3
            };
            canvas.Children.Add(runwayh);
            Line runwayv = new()
            {
                X1 = _runway.LengthNM * xscale,
                Y1 = (canvas.ActualHeight / 2) - (_runway.WidthNM * yscale),
                X2 = _runway.LengthNM * xscale,
                Y2 = (canvas.ActualHeight / 2) + (_runway.WidthNM * yscale),
                Stroke = Brushes.Green,
                StrokeThickness = 3
            };
            canvas.Children.Add(runwayv);
            Line upper = new()
            {
                X1 = 0,
                Y1 = canvas.ActualHeight / 2,
                X2 = (_runway.LengthNM + Runway.Distance) * xscale,
                Y2 = canvas.ActualHeight - (((_runway.LengthNM + _runway.Distance) * Math.Tan((10) * double.Pi / 180)) * yscale),
                Stroke = Brushes.CadetBlue,
                StrokeThickness = 3
            };
            canvas.Children.Add(upper);
            Line lower = new()
            {
                X1 = 0,
                Y1 = canvas.ActualHeight / 2,
                X2 = (_runway.LengthNM + Runway.Distance) * xscale,
                Y2 = ((_runway.LengthNM + _runway.Distance) * Math.Tan((10) * double.Pi / 180)) * yscale,
                Stroke = Brushes.CadetBlue,
                StrokeThickness = 3
            };
            canvas.Children.Add(lower);
            Line axis = new()
            {
                X1 = _runway.LengthNM * xscale,
                Y1 = canvas.ActualHeight / 2,
                X2 = (_runway.LengthNM + Runway.Distance) * xscale,
                Y2 = canvas.ActualHeight / 2,
                Stroke = Brushes.Yellow,
                StrokeThickness = 2
            };
            canvas.Children.Add(axis);
            for (int i = 1; i <= 10; i++)
            {
                SolidColorBrush stroke = (i % 5 == 0) ? Brushes.Orange : Brushes.Green;
                double x = (i * _runway.Distance / 10 + _runway.LengthNM) * xscale;
                Line distance = new()
                {
                    X1 = x,
                    Y1 = canvas.ActualHeight / 2 + (x / ((_runway.LengthNM + _runway.Distance) * xscale) * (((_runway.LengthNM + _runway.Distance) * Math.Tan(10 * Math.PI / 180)) * yscale - canvas.ActualHeight / 2)),
                    X2 = x,
                    Y2 = canvas.ActualHeight / 2 + (x / ((_runway.LengthNM + _runway.Distance) * xscale) * (canvas.ActualHeight - (((_runway.LengthNM + _runway.Distance) * Math.Tan(10 * Math.PI / 180)) * yscale) - canvas.ActualHeight / 2)),
                    Stroke = stroke,
                    StrokeThickness = 1
                };
                canvas.Children.Add(distance);
            }
            foreach (Aircraft aircraft in aircrafts)
            {
                if (aircraft.IsDisplayed(runway))
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
            Canvas.SetTop(elipse, (canvas.ActualHeight / 2)-aircraft.LateralOffset(_runway)*yscale);
            Canvas.SetLeft(elipse, (aircraft.Distance(_runway) + _runway.LengthNM) * xscale);
            canvas.Children.Add(elipse);
            TextBlock textBlock = new()
            {
                //TODO: remove debug
                Text = $"{aircraft.Callsign}\n{aircraft.Altitude}\n\n{(aircraft.BearingFromRunway(_runway) - _runway.Heading + 360) % 360}°={aircraft.LateralOffset(_runway)}nm",
                FontSize = 12,
                Foreground = Brushes.White
            };
            Canvas.SetTop(textBlock, (canvas.ActualHeight / 2)-(aircraft.LateralOffset(runway) * yscale + 35));
            Canvas.SetLeft(textBlock, ((aircraft.Distance(_runway) + _runway.LengthNM) * xscale) - 10);
            canvas.Children.Add(textBlock);
        }
    }
}
