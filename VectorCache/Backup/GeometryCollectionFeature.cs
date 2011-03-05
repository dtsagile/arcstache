using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace DTS.Common.GIS
{
    public class GeometryCollectionFeature: GISFeature
    {
        GeometryCollection _shape;

        public GeometryCollectionFeature(IGISAttributes attributes) : base(attributes) { }

        public override Geometry Shape
        {
            get
            {
                return _shape;
            }
            set
            {
                if (value == null || value is GeometryCollection)
                    _shape = value as GeometryCollection;
                else
                    throw new ArgumentException("Shape should be of type [GeometryCollection]");
            }
        }

        public GeometryCollection FeatureShape
        {
            get
            {
                return Shape as GeometryCollection;
            }
            set
            {
                Shape = value;
            }
        }
    }
}
