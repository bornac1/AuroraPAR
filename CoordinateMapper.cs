using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraPAR
{
    using System;

    internal class CoordinateMapper
    {
        private const double EarthRadiusNM = 3440.0; // Earth's radius in nautical miles

        // Converts degrees to radians
        public static double ToRadians(double degrees) => degrees * Math.PI / 180.0;

        // Calculates distance between two points (lat/lon) in nautical miles
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusNM * c; // Distance in nautical miles
        }

        // Calculates initial bearing between two points (lat/lon)
        public static double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
        {
            double dLon = ToRadians(lon2 - lon1);
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) -
                       Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);

            double bearing = Math.Atan2(y, x);
            return (bearing * 180.0 / Math.PI + 360.0) % 360.0; // Bearing in degrees
        }

        // Rotates a point (distanceX, distanceY) by a given angle (in degrees)
        public static (double, double) RotatePoint(double distanceX, double distanceY, double angleDegrees)
        {
            double angleRad = ToRadians(angleDegrees);
            double cosTheta = Math.Cos(angleRad);
            double sinTheta = Math.Sin(angleRad);

            double x = distanceX * cosTheta - distanceY * sinTheta;
            double y = distanceX * sinTheta + distanceY * cosTheta;

            return (x, y);
        }
    }

}
