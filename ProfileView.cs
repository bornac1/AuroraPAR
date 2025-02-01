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
    internal class ProfileView
    {
        private double xscale = 1;
        private double yscale = 1;
        private Canvas canvas;
        private Runway runway;
        public ProfileView(Canvas canvas)
        {
            this.canvas = canvas;
            runway = new()
            {
                Designator = "05",
                Elevation = 0,
                Latitude = 0,
                Longitude = 0,
                LengthM = 1000,
                Distance = 10
            };
        }
        private void CalculateScale()
        {
            xscale = canvas.ActualWidth / (1.1*runway.Distance+2);
            yscale = (canvas.ActualHeight-50) / (((1.1*runway.Distance) * Math.Tan(runway.GlideSlope * double.Pi / 180)* 6076.11549)+2);
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
            Canvas.SetLeft(elipse, (aircraft.Distance(runway) + runway.LengthNM) * xscale);
            canvas.Children.Add(elipse);
            TextBlock textBlock = new TextBlock()
            {
                Text = $"{aircraft.Callsign}\n{aircraft.Altitde}",
                FontSize = 12,
                Foreground = Brushes.White
            };
            Canvas.SetBottom(textBlock, (aircraft.Altitde * yscale + 15));
            Canvas.SetLeft(textBlock, ((aircraft.Distance(runway) + runway.LengthNM) * xscale)-10);
            canvas.Children.Add(textBlock);
        }
        public void Draw()
        {
            CalculateScale();
            Line horizontal = new()
            {
                X1 = runway.LengthNM*xscale,
                Y1 = canvas.ActualHeight - 20,
                X2 = (runway.Distance+runway.LengthNM) * xscale,
                Y2 = canvas.ActualHeight - 20,
                Stroke = Brushes.Green,
                StrokeThickness = 2
            };
            canvas.Children.Add(horizontal);
            Line glidepath = new()
            {
                X1 = runway.LengthNM * xscale,
                Y1 = canvas.ActualHeight - (50*yscale + 20),
                X2 = (runway.Distance + runway.LengthNM) * xscale,
                Y2 = canvas.ActualHeight-((runway.Distance * Math.Tan(runway.GlideSlope * double.Pi / 180) * 6076.11549)*yscale + 20),
                Stroke = Brushes.Yellow,
                StrokeThickness = 2
            };
            canvas.Children.Add(glidepath);
            for(int i = 1; i<=10; i++)
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
                    X1 = (i*runway.Distance/10+runway.LengthNM)*xscale,
                    Y1 = canvas.ActualHeight-20,
                    X2 = (i*runway.Distance/10 + runway.LengthNM) * xscale,
                    Y2 = canvas.ActualHeight - (((i*runway.Distance/10) * Math.Tan(runway.GlideSlope * double.Pi / 180) * 6076.11549) * yscale + 20),
                    Stroke = stroke,
                    StrokeThickness = 1
                };
                canvas.Children.Add(distance);
                TextBlock textBlock = new()
                {
                    Text = $"{i*runway.Distance/10}NM",
                    FontSize = 12,
                    Foreground = Brushes.Yellow
                };
                Canvas.SetBottom(textBlock, 0);
                Canvas.SetLeft(textBlock, ((i * runway.Distance / 10 + runway.LengthNM) * xscale-10));
                canvas.Children.Add(textBlock);
            }
            DrawAircraft(new()
            {
                Latitude = 0.1,
                Longitude = 0,
                Altitde = 2000
            });
        }
    }
}
