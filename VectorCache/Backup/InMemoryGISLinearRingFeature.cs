using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace DTS.Common.GIS
{
    public class InMemoryGISLinearRingFeature : InMemoryGISFeature
    {
        LinearRing _shape;

        public InMemoryGISLinearRingFeature() { }

        public InMemoryGISLinearRingFeature(Coordinate[] points)
        {
            Shape = new LinearRing(points);
        }

        public override Geometry Shape
        {
            get
            {
                return _shape;
            }
            set
            {
                if (value == null || value is LinearRing)
                    _shape = value as LinearRing;
                else
                {
                    if (value is LineString)
                    {
                        LineString line = value as LineString;
                        if(line.Coordinates.Length == 0 ||line.IsClosed)
                            _shape = new LinearRing(line.Coordinates);
                    }
                    else
                        throw new ArgumentException("Shape should be of type [LinearRing]");
                }
            }
        }

        public LinearRing FeatureShape
        {
            get { return Shape as LinearRing; }
            set { Shape = value; }
        }
    }
}
