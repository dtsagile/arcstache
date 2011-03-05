using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace DTS.Common.GIS
{
    public class InMemoryGISPolygonFeature : InMemoryGISFeature
    {
        Polygon _shape;

        public InMemoryGISPolygonFeature() { }

        #region Constructors

        public InMemoryGISPolygonFeature(LinearRing outerShell)
        {
            Shape = new Polygon(outerShell);
        }

        public InMemoryGISPolygonFeature(LinearRing outerShell, LinearRing[] innerHoles)
        {
            Shape = new Polygon(outerShell, innerHoles);
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
                if (value == null || value is Polygon)
                    _shape = value as Polygon;
                else
                    throw new ArgumentException("Shape should be of type [Polygon]");
            }
        }

        public Polygon FeatureShape
        {
            get { return Shape as Polygon; }
            set { Shape = value; }
        }
    }
}
