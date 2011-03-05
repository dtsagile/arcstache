using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace DTS.Common.GIS
{
    public class InMemoryGISLineStringFeature : InMemoryGISFeature
    {
        LineString _shape;

        public InMemoryGISLineStringFeature() { }

        public InMemoryGISLineStringFeature(Coordinate[] points)
        {
            Shape = new LineString(points);
        }

        public override Geometry Shape
        {
            get
            {
                return _shape;
            }
            set
            {
                if (value == null || value is LineString)
                    _shape = value as LineString;
                else
                    throw new ArgumentException("Shape should be of type [LineString]");
            }
        }

        public LineString FeatureShape
        {
            get { return Shape as LineString; }
            set { Shape = value; }
        }
    }
}
