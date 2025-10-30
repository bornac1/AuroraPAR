﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace AuroraPAR
{
    internal class Runway
    {
        public string ICAO { get; set; } = "ZZZZ";
        public string Designator { get; set; } = "00";
        /// <summary>
        /// Heading in deg.
        /// </summary>
        public double Heading { get; set; } = 0;
        /// <summary>
        /// Elevation in ft.
        /// </summary>
        public double Elevation { get; set; } = 0;
        /// <summary>
        /// Plus is north.
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Plus is east.
        /// </summary>
        public double Longitude { get; set; }
        private double _length = 0;
        /// <summary>
        /// Length in meters.
        /// </summary>
        public double LengthM { get { return _length; } set { _length = value; } }
        /// <summary>
        /// Length in nautical miles.
        /// </summary>
        public double LengthNM { get { return LengthM / 1852; } set { _length = value / 1852; } }
        /// <summary>
        /// Glide slope angle in degrees.
        /// </summary>
        public double GlideSlope { get; set; } = 3.0;
        /// <summary>
        /// Threshold crossing height.
        /// </summary>
        public double TCH { get; set; } = 50;
        /// <summary>
        /// Distance from the runway to be displayed in NM.
        /// </summary>
        public double Distance { get; set; } = 10.0;
        private double _width = 0;
        /// <summary>
        /// Width in meters.
        /// </summary>
        public double WidthM { get { return _width; } set { _width = value; } }
        /// <summary>
        /// Width in nautical miles.
        /// </summary>
        public double WidthNM { get { return WidthM / 1852; } set { _width = value / 1852; } }

        public override string ToString()
        {
            return $"{ICAO} {Designator}";
        }

    }
    internal class Aircraft
    {
        public string Callsign { get; set; } = "ABC1234";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; } = 0;
        /// <summary>
        /// Distance to runway.
        /// </summary>
        /// <param name="runway">Runway.</param>
        /// <returns>Distance in NM.</returns>
        public double Distance(Runway runway)
        {
            double lat1Rad = Latitude*double.Pi/180;
            double lon1Rad = Longitude * double.Pi / 180;
            double lat2Rad = runway.Latitude * double.Pi / 180;
            double lon2Rad = runway.Longitude*double.Pi/180;
            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;
            // Haversine formula
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return (c * 6371 * 1000 / 1852);
        }
        public double LateralOffset(Runway runway)
        {
            // Calculate initial bearing from runway to aircraft
            double bearingToAircraft = BearingFromRunway(runway);

            double R = 6371e3; // Earth radius in meters

            // Angular distance from runway to aircraft (radians)
            double delta13 = Distance(runway) * 1852 / R; // Distance(runway) returns NM, convert to meters then to radians

            // Bearings in radians
            double theta13 = bearingToAircraft * Math.PI / 180.0; // runway -> aircraft
            double theta12 = runway.Heading * Math.PI / 180.0;     // runway heading

            // Cross-track distance formula (meters)
            double xt = Math.Asin(Math.Sin(delta13) * Math.Sin(theta13 - theta12)) * R;

            // Convert meters back to NM
            double lateralOffsetNM = xt / 1852.0;

            return lateralOffsetNM;
        }
        public double BearingFromRunway(Runway runway)
        {
            double lat1Rad = runway.Latitude * Math.PI / 180;
            double lon1Rad = runway.Longitude * Math.PI / 180;
            double lat2Rad = Latitude * Math.PI / 180;
            double lon2Rad = Longitude * Math.PI / 180;

            double dLon = lon2Rad - lon1Rad;
            double y = Math.Sin(dLon) * Math.Cos(lat2Rad);
            double x = Math.Cos(lat1Rad) * Math.Sin(lat2Rad) -
                       Math.Sin(lat1Rad) * Math.Cos(lat2Rad) * Math.Cos(dLon);
            double bearingToAircraft = (Math.Atan2(y, x) * 180 / Math.PI + 360) % 360;
            return bearingToAircraft;
        }
        public bool IsDisplayed(Runway runway)
        {
            double diff = BearingFromRunway(runway) - 180;
            //Prevent opposite runway
            if (Math.Abs(diff) <= 10) return false;
            //Prevent far away aircrafts
            if (Distance(runway) > runway.Distance) return false;
            //Prevent aircrafts below runway elevation
            if (Altitude <= runway.Elevation) return false;
            return true;
        }
    }
    internal struct Distance(double distance)
    {
        double distance = distance;
        public static implicit operator double(Distance d) => d.distance;
        public static implicit operator Distance(int distance) => new Distance(distance);
        public static implicit operator Distance(double distance) => new Distance(distance);
        public override string ToString()
        {
            return $"{distance} nm";
        }
    }

    internal class DataFile
    {
        //Format: ICAO;DESIGNATOR;HEADING;ELEVATION;LATITUDE;LONGITUDE;LENGTH IN METERS;WIDTH IN METERS;GLIDE SLOPE;TCH;DEFAULT DISTANCE
        public static async Task<Runway[]> GetRunways(string path)
        {
            List<Runway> runways = [];
            string[] data = await System.IO.File.ReadAllLinesAsync(path);
            foreach (string line in data)
            {
                string[] linedata = line.Replace('.', ',').Split(';');
                if (linedata.Length > 9)
                {
                    runways.Add(new()
                    {
                        ICAO = linedata[0],
                        Designator = linedata[1],
                        Heading = Double.Parse(linedata[2]),
                        Elevation = Double.Parse(linedata[3]),
                        Latitude = Double.Parse(linedata[4]),
                        Longitude = Double.Parse(linedata[5]),
                        LengthM = Double.Parse(linedata[6]),
                        WidthM = Double.Parse(linedata[7]),
                        GlideSlope = Double.Parse(linedata[8]),
                        TCH = Double.Parse(linedata[9]),
                        Distance = Double.Parse(linedata[10])
                    });
                }
            }
            return runways.ToArray();
        }
    }
}
