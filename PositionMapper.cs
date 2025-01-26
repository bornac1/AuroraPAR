using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraPAR
{
    internal class PositionMapper
    {
        // Maps the aircraft's position to a 2D canvas
        public static (double x, double y) MapToCanvas(
            double aircraftLat, double aircraftLon, double aircraftAlt,
            double runwayLat, double runwayLon, double runwayAlt,
            double magneticHeading, double magneticDeclination,
            double xScale, double yScale)
        {
            // Step 1: Calculate true heading of the runway
            double trueHeading = magneticHeading + magneticDeclination;

            // Step 2: Calculate horizontal distance and bearing
            double distanceNM = CoordinateMapper.CalculateDistance(runwayLat, runwayLon, aircraftLat, aircraftLon);
            double bearing = CoordinateMapper.CalculateBearing(runwayLat, runwayLon, aircraftLat, aircraftLon);

            // Step 3: Align distance to runway's true heading
            double alongTrackDistance = distanceNM * Math.Cos(CoordinateMapper.ToRadians(bearing - trueHeading));
            double crossTrackDistance = distanceNM * Math.Sin(CoordinateMapper.ToRadians(bearing - trueHeading));

            // Step 4: Map altitude difference to vertical position
            double verticalDistance = aircraftAlt - runwayAlt;

            // Step 5: Scale to canvas coordinates
            double x = alongTrackDistance * xScale;  // X-axis: Along the runway
            double y = verticalDistance * yScale;    // Y-axis: Altitude

            return (x, y);
        }
    }
}
