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
    internal class ProfileView(Canvas canvas, Runway runway)
    {
        private double xscale = 1;
        private double yscale = 1;
        private Canvas canvas = canvas;
        private Runway _runway = runway;
        public Runway Runway
        {
            get { return _runway; }
            set { _runway = value; }
        }
        private void CalculateScale()
        {
            xscale = canvas.ActualWidth / (1.1*_runway.Distance+2);
            yscale = (canvas.ActualHeight-50) / (((1.1*_runway.Distance) * Math.Tan((_runway.GlideSlope + 5) * double.Pi / 180)* 6076.11549)+2);
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
            Canvas.SetBottom(elipse, aircraft.Altitde * yscale);
            Canvas.SetLeft(elipse, (aircraft.Distance(_runway) + _runway.LengthNM) * xscale);
            canvas.Children.Add(elipse);
            TextBlock textBlock = new()
            {
                Text = $"{aircraft.Callsign}\n{aircraft.Altitde}",
                FontSize = 12,
                Foreground = Brushes.White
            };
            Canvas.SetBottom(textBlock, (aircraft.Altitde * yscale + 15));
            Canvas.SetLeft(textBlock, ((aircraft.Distance(_runway) + _runway.LengthNM) * xscale)-10);
            canvas.Children.Add(textBlock);
        }
        public void Draw(List<Aircraft> aircrafts)
        {
            CalculateScale();
            Line horizontal = new()
            {
                X1 = _runway.LengthNM*xscale,
                Y1 = canvas.ActualHeight - 20,
                X2 = (_runway.Distance+_runway.LengthNM) * xscale,
                Y2 = canvas.ActualHeight - 20,
                Stroke = Brushes.Green,
                StrokeThickness = 2
            };
            canvas.Children.Add(horizontal);
            Line runwayh = new()
            {
                X1 = 0,
                Y1 = canvas.ActualHeight - 20,
                X2 = _runway.LengthNM * xscale,
                Y2 = canvas.ActualHeight - 20,
                Stroke = Brushes.Green,
                StrokeThickness = 3
            };
            canvas.Children.Add(runwayh);
            Line runwayv = new()
            {
                X1 = _runway.LengthNM * xscale,
                Y1 = canvas.ActualHeight - 20,
                X2 = _runway.LengthNM * xscale,
                Y2 = canvas.ActualHeight - ((_runway.LengthNM * Math.Tan((_runway.GlideSlope + 5) * double.Pi / 180) * 6076.11549) * yscale + 20),
                Stroke = Brushes.Green,
                StrokeThickness = 3
            };
            canvas.Children.Add(runwayv);
            Line upper = new()
            {
                X1 = 0,
                Y1 = canvas.ActualHeight - 20,
                X2 = (_runway.LengthNM+Runway.Distance) * xscale,
                Y2 = canvas.ActualHeight - (((_runway.LengthNM+_runway.Distance) * Math.Tan((_runway.GlideSlope + 5) * double.Pi / 180) * 6076.11549) * yscale + 20),
                Stroke = Brushes.CadetBlue,
                StrokeThickness = 3
            };
            canvas.Children.Add(upper);
            Line glidepath = new()
            {
                X1 = _runway.LengthNM * xscale,
                Y1 = canvas.ActualHeight - (50*yscale + 20),
                X2 = (_runway.Distance + _runway.LengthNM) * xscale,
                Y2 = canvas.ActualHeight-((_runway.Distance * Math.Tan(_runway.GlideSlope * double.Pi / 180) * 6076.11549)*yscale + 20),
                Stroke = Brushes.Yellow,
                StrokeThickness = 2
            };
            canvas.Children.Add(glidepath);
            Line glidepathM05 = new()
            {
                X1 = _runway.LengthNM * xscale,
                Y1 = canvas.ActualHeight - (50 * yscale + 20),
                X2 = (_runway.Distance + _runway.LengthNM) * xscale,
                Y2 = canvas.ActualHeight - ((_runway.Distance * Math.Tan((_runway.GlideSlope-0.5) * double.Pi / 180) * 6076.11549) * yscale + 20),
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            canvas.Children.Add(glidepathM05);
            Line glidepathP05 = new()
            {
                X1 = _runway.LengthNM * xscale,
                Y1 = canvas.ActualHeight - (50 * yscale + 20),
                X2 = (_runway.Distance + _runway.LengthNM) * xscale,
                Y2 = canvas.ActualHeight - ((_runway.Distance * Math.Tan((_runway.GlideSlope + 0.5) * double.Pi / 180) * 6076.11549) * yscale + 20),
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            canvas.Children.Add(glidepathP05);
            for (int i = 1; i<=10; i++)
            {
                SolidColorBrush stroke;
                if (i % 5 == 0)
                {
                    stroke = Brushes.Orange;  
                } else
                {
                    stroke = Brushes.Green;
                }
                Line distance = new()
                {
                    X1 = (i*_runway.Distance/10+_runway.LengthNM)*xscale,
                    Y1 = canvas.ActualHeight-20,
                    X2 = (i*_runway.Distance/10 + _runway.LengthNM) * xscale,
                    Y2 = canvas.ActualHeight - (((_runway.LengthNM+i*_runway.Distance/10) * Math.Tan((_runway.GlideSlope+5) * double.Pi / 180) * 6076.11549) * yscale + 20),
                    Stroke = stroke,
                    StrokeThickness = 1
                };
                canvas.Children.Add(distance);
                TextBlock textBlock = new()
                {
                    Text = $"{i*_runway.Distance/10}NM",
                    FontSize = 12,
                    Foreground = Brushes.Yellow
                };
                Canvas.SetBottom(textBlock, 0);
                Canvas.SetLeft(textBlock, ((i * _runway.Distance / 10 + _runway.LengthNM) * xscale-10));
                canvas.Children.Add(textBlock);
            }
            foreach (Aircraft aircraft in aircrafts)
            {
                DrawAircraft(aircraft);
            }
        }
    }
}
