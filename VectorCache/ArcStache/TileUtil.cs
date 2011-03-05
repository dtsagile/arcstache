using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;

namespace ArcStache
{
    public static class TileUtil
    {
        /// <summary>
        /// Returns NW corner of the tile
        /// </summary>
        /// <param name="tile_x"></param>
        /// <param name="tile_y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static IPoint TileToWorldPos(long tile_x, long tile_y, long zoom)
        {
            IPoint p = new PointClass();
            double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

            p.X = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
            p.Y = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

            return p;
        }


        public static IEnvelope GetEnvelopeFromZoomRowCol(int zoom, int row, int col)
        {
            IEnvelope e = new EnvelopeClass();
            e.SpatialReference = GetSpatialReference(102100);
            IPoint llupperLeftPoint = TileToGeographic(col, row, zoom);
            IPoint lllowerRightPoint = TileToGeographic(col + 1, row + 1, zoom);

            var upperLeftPtWm = WebMercatorUtil.GeographicToWebMercator(llupperLeftPoint.Y, llupperLeftPoint.X);
            IPoint wmUpperLeftPoint = new PointClass();
            wmUpperLeftPoint.X = upperLeftPtWm.X;
            wmUpperLeftPoint.Y = upperLeftPtWm.Y;

            var lowerRightPtWm = WebMercatorUtil.GeographicToWebMercator(lllowerRightPoint.Y, lllowerRightPoint.X);
            IPoint wmLowerRightPoint = new PointClass();
            wmLowerRightPoint.X = lowerRightPtWm.X;
            wmLowerRightPoint.Y = lowerRightPtWm.Y;

            e.UpperLeft = wmUpperLeftPoint;
            e.LowerRight = wmLowerRightPoint;
            return e;
        }

        public static IPoint TileToGeographic(int tileX, int tileY, int zoom)
        {
            // From http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#C.23

            IPoint point = new PointClass();
            double n = Math.PI - ((2.0 * Math.PI * tileY) / Math.Pow(2.0, zoom));
            point.X = (float)((tileX / Math.Pow(2.0, zoom) * 360.0) - 180.0);
            point.Y = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));
            return point;
        }

        public static IPoint GeographicToTile(double lon, double lat, int zoom)
        {
            // From http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#C.23

            IPoint point = new PointClass();
            point.X = (float)((lon + 180.0) / 360.0 * (1 << zoom));
            point.Y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

            return point;
        }

        public static ISpatialReference GetSpatialReference(int wkid)
        {
            IGeometryServer2 geometryServer = new ESRI.ArcGIS.Geodatabase.GeometryServerClass();
            return geometryServer.FindSRByWKID(string.Empty, wkid, -1, true, true);
        }
    }
}
