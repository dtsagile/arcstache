using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArcStache
{

    /// <summary>
    /// Utility class for storing XY coordinate pairs as double.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Gets or sets the X or Longitude coordinate value.
        /// </summary>
        /// <value>The X or Longitude coordinate value.</value>
        public double X { get; set; }
        /// <summary>
        /// Gets or sets the Y or Latitude coordinate value.
        /// </summary>
        /// <value>The Y or Latitude coordinate value.</value>
        public double Y { get; set; }
    }


    /// <summary>
    /// Utility class for projecting coordinates from geographic (4326) to web mercator (102100) and back.
    /// </summary>
    public static class WebMercatorUtil
    {
        static double DEGREES_PER_RADIANS = 180 / Math.PI;
        static double RADIANS_PER_DEGREES = Math.PI / 180;
        static int RADIUS = 6378137;
        static double PI_OVER_2 = Math.PI / 2;
        
        public static int GEOGRAPHIC_WKID = 4326;
        public static int WEBMERCATOR_WKID = 102100;

        /// <summary>
        /// Converts a coordinate pair in geographic projection (WKID 4326) to web mercator auxillary sphere (102100).
        /// </summary>
        /// <param name="latitude">The latitude coordinate.</param>
        /// <param name="longitude">The longitude coordinate.</param>
        /// <returns></returns>
        public static Point GeographicToWebMercator(double latitude, double longitude)
        {
            var x = LongitudeToX(longitude);
            var y = LatitudeToY(latitude);
            return new Point() { X = x, Y = y };
        }

        /// <summary>
        /// Converts a coordinate pair in web mercator auxillary sphere projection (WKID 102100) to geographic (4326).
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns></returns>
        public static Point WebMercatorToGeographic(double x, double y)
        {
            var longitude = XToLongitude(x);
            var latitude = YToLatitude(y);
            return new Point() { X = longitude, Y = latitude };
        }



        public static double LatitudeToY(double latitude)
        {
            var loc1 = latitude * RADIANS_PER_DEGREES;
            var loc2 = Math.Sin(loc1);
            return RADIUS * 0.5 * Math.Log((1 + loc2) / (1 - loc2));
        }

        public static double LongitudeToX(double longitude)
        {
            return longitude * RADIANS_PER_DEGREES * RADIUS;
        }

        public static double XToLongitude(double x)
        {
            var loc1 = x / RADIUS;
            var loc2 = loc1 * DEGREES_PER_RADIANS;
            var loc3 = Math.Floor((loc2 + 180) / 360);
            return loc2 - loc3 * 360;
        }

        public static double YToLatitude(double y)
        {
            var loc1 = PI_OVER_2 - 2 * Math.Atan(Math.Exp(-1 * y / RADIUS));
            return loc1 * DEGREES_PER_RADIANS;
        }


    }
}