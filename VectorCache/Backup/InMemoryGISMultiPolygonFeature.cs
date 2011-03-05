using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace DTS.Common.GIS
{
    public class InMemoryGISMultiPolygonFeature: InMemoryGISFeature
    {
        MultiPolygon _shape;

        public InMemoryGISMultiPolygonFeature() { }

        #region Constructors

        public InMemoryGISMultiPolygonFeature(Polygon[] polygons)
        {
            Shape = new MultiPolygon(polygons);
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
                if (value == null || value is MultiPolygon)
                    _shape = value as MultiPolygon;
                else
                    throw new ArgumentException("Shape should be of type [MultiPolygon]");
            }
        }

        public MultiPolygon FeatureShape
        {
            get { return Shape as MultiPolygon; }
            set { Shape = value; }
        }
    }
}
