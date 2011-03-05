using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;
using System.Xml.Serialization;

namespace DTS.Common.GIS
{
    public abstract class GISPolygonFeature : GISFeature
    {
        public GISPolygonFeature(IGISAttributes attributes) : base(attributes) { }

        public Polygon FeatureShape
        {
            get
            {
                return Shape as Polygon;
            }
            set
            {
                Shape = value;
            }
        }
    }
}
