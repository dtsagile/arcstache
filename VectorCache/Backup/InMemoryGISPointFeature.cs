using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace DTS.Common.GIS
{
    public class InMemoryGISPointFeature : InMemoryGISFeature
    {
        Point _shape;

        #region "Constructors"

        public InMemoryGISPointFeature()
        {
            Shape = new Point(0, 0);
        }

        public InMemoryGISPointFeature(double x, double y)
        {
            Shape = new Point(x, y);
        }

        public InMemoryGISPointFeature(double x, double y, double z)
        {
            Shape = new Point(x, y, z);
        }

        public InMemoryGISPointFeature(Coordinate coord)
        {
            Shape = new Point(coord);
        }

        #endregion

        public override Geometry Shape
        {
            get
            {
                return _shape;
            }
            set
            {
                if (value == null || value is Point)
                    _shape = value as Point;
                else
                    throw new ArgumentException("Shape should be of type [Point]");
            }
        }

        public Point FeatureShape
        {
            get { return _shape as Point; }
            set { Shape = value; }
        }
    }
}
