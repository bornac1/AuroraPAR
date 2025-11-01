﻿using System;
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
        private Canvas Canvas = canvas;
        public Runway Runway = runway;
        private void CalculateScale()
        {
            xscale = (Canvas.ActualWidth - 50) / (Runway.Distance+Runway.LengthNM);
            yscale = (Canvas.ActualHeight - 50) / (((Runway.Distance+Runway.LengthNM) * Math.Tan((Runway.GlideSlope + 5) * double.Pi / 180)* 6076.11549)+2);
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
            Canvas.SetBottom(elipse, (aircraft.Altitude-Runway.Elevation) * yscale + 2.5);
            Canvas.SetLeft(elipse, (aircraft.Distance(Runway) + Runway.LengthNM) * xscale - 5);
            Canvas.Children.Add(elipse);
            TextBlock textBlock = new()
            {
                Text = $"{aircraft.Callsign}\n{aircraft.Altitude}",
                FontSize = 12,
                Foreground = Brushes.White
            };
            Canvas.SetBottom(textBlock, (aircraft.Altitude * yscale + 17.5));
            Canvas.SetLeft(textBlock, ((aircraft.Distance(Runway) + Runway.LengthNM) * xscale)-15);
            Canvas.Children.Add(textBlock);
        }
        public void Draw(List<Aircraft> aircrafts)
        {
            CalculateScale();
            Line horizontal = new()
            {
                X1 = Runway.LengthNM*xscale,
                Y1 = Canvas.ActualHeight - 20,
                X2 = (Runway.Distance+Runway.LengthNM) * xscale,
                Y2 = Canvas.ActualHeight - 20,
                Stroke = Brushes.Green,
                StrokeThickness = 2
            };
            Canvas.Children.Add(horizontal);
            Line runwayh = new()
            {
                X1 = 0,
                Y1 = Canvas.ActualHeight - 20,
                X2 = Runway.LengthNM * xscale,
                Y2 = Canvas.ActualHeight - 20,
                Stroke = Brushes.Green,
                StrokeThickness = 3
            };
            Canvas.Children.Add(runwayh);
            Line runwayv = new()
            {
                X1 = Runway.LengthNM * xscale,
                Y1 = Canvas.ActualHeight - 20,
                X2 = Runway.LengthNM * xscale,
                Y2 = Canvas.ActualHeight - ((Runway.LengthNM * Math.Tan((Runway.GlideSlope + 5) * double.Pi / 180) * 6076.11549) * yscale + 20),
                Stroke = Brushes.Green,
                StrokeThickness = 3
            };
            Canvas.Children.Add(runwayv);
            Line upper = new()
            {
                X1 = 0,
                Y1 = Canvas.ActualHeight - 20,
                X2 = (Runway.LengthNM+Runway.Distance) * xscale,
                Y2 = Canvas.ActualHeight - (((Runway.LengthNM+Runway.Distance) * Math.Tan((Runway.GlideSlope + 5) * double.Pi / 180) * 6076.11549) * yscale + 20),
                Stroke = Brushes.CadetBlue,
                StrokeThickness = 3
            };
            Canvas.Children.Add(upper);
            Line glidepath = new()
            {
                X1 = Runway.LengthNM * xscale,
                Y1 = Canvas.ActualHeight - (Runway.TCH * yscale + 20),
                X2 = (Runway.Distance + Runway.LengthNM) * xscale,
                Y2 = Canvas.ActualHeight-((Runway.Distance * Math.Tan(Runway.GlideSlope * double.Pi / 180) * 6076.11549 + Runway.TCH)*yscale + 20),
                Stroke = Brushes.Yellow,
                StrokeThickness = 2
            };
            Canvas.Children.Add(glidepath);
            Line glidepathM05 = new()
            {
                X1 = Runway.LengthNM * xscale,
                Y1 = Canvas.ActualHeight - (Runway.TCH * yscale + 20),
                X2 = (Runway.Distance + Runway.LengthNM) * xscale,
                Y2 = Canvas.ActualHeight - ((Runway.Distance * Math.Tan((Runway.GlideSlope-0.5) * double.Pi / 180) * 6076.11549 + Runway.TCH) * yscale + 20),
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            Canvas.Children.Add(glidepathM05);
            Line glidepathP05 = new()
            {
                X1 = Runway.LengthNM * xscale,
                Y1 = Canvas.ActualHeight - (Runway.TCH * yscale + 20),
                X2 = (Runway.Distance + Runway.LengthNM) * xscale,
                Y2 = Canvas.ActualHeight - ((Runway.Distance * Math.Tan((Runway.GlideSlope + 0.5) * double.Pi / 180) * 6076.11549 + Runway.TCH) * yscale + 20),
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            Canvas.Children.Add(glidepathP05);
            Line MDH = new()
            {
                X1 = 0,
                Y1 = Canvas.ActualHeight - (Runway.MDH * yscale + 20),
                X2 = (Runway.LengthNM + ((Runway.MDH - Runway.TCH) / 6076.11549) / Math.Tan(Runway.GlideSlope * double.Pi / 180)) * xscale,
                Y2 = Canvas.ActualHeight - (Runway.MDH * yscale + 20),
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            Canvas.Children.Add(MDH);
            Line MAPt = new()
            {
                X1 = (Runway.LengthNM + ((Runway.MDH - Runway.TCH) / 6076.11549) / Math.Tan(Runway.GlideSlope * double.Pi / 180)) * xscale,
                Y1 = Canvas.ActualHeight - 20,
                X2 = (Runway.LengthNM + ((Runway.MDH - Runway.TCH) / 6076.11549) / Math.Tan(Runway.GlideSlope * double.Pi / 180)) * xscale,
                Y2 = Canvas.ActualHeight - (Runway.MDH * yscale + 20),
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            Canvas.Children.Add(MAPt);
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
                    X1 = (i*Runway.Distance/10+Runway.LengthNM)*xscale,
                    Y1 = Canvas.ActualHeight-20,
                    X2 = (i*Runway.Distance/10 + Runway.LengthNM) * xscale,
                    Y2 = Canvas.ActualHeight - (((Runway.LengthNM+i*Runway.Distance/10) * Math.Tan((Runway.GlideSlope+5) * double.Pi / 180) * 6076.11549) * yscale + 20),
                    Stroke = stroke,
                    StrokeThickness = 1
                };
                Canvas.Children.Add(distance);
                TextBlock textBlock = new()
                {
                    Text = $"{i*Runway.Distance/10}NM",
                    FontSize = 12,
                    Foreground = Brushes.Yellow
                };
                Canvas.SetBottom(textBlock, 0);
                Canvas.SetLeft(textBlock, ((i * Runway.Distance / 10 + Runway.LengthNM) * xscale-10));
                Canvas.Children.Add(textBlock);
            }
            foreach (Aircraft aircraft in aircrafts)
            {
                if (aircraft.IsDisplayed(runway))
                {
                    DrawAircraft(aircraft);
                }
            }
        }
    }
}
