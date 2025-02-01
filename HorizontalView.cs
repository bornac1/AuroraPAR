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
                Y1 = canvas.ActualHeight/2,
                X2 = _runway.LengthNM * xscale,
                Y2 = canvas.ActualHeight / 2,
                Stroke = Brushes.Green,
                StrokeThickness = 3
            };
            canvas.Children.Add(runwayh);
            Line runwayv = new()
            {
                X1 = _runway.LengthNM * xscale,
                Y1 = (canvas.ActualHeight / 2) - (_runway.WidthNM *yscale),
                X2 = _runway.LengthNM * xscale,
                Y2 = (canvas.ActualHeight / 2) + (_runway.WidthNM *yscale),
                Stroke = Brushes.Green,
                StrokeThickness = 3
            };
            canvas.Children.Add(runwayv);
            Line upper = new()
            {
                X1 = 0,
                Y1 = canvas.ActualHeight / 2,
                X2 = (_runway.LengthNM + Runway.Distance) * xscale,
                Y2 = canvas.ActualHeight-(((_runway.LengthNM + _runway.Distance) * Math.Tan((10) * double.Pi / 180)) * yscale),
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
        }
    }
}
