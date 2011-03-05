using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;
using System.Xml.Serialization;

namespace DTS.Common.GIS
{
    public abstract class GISLineStringFeature : GISFeature
    {
        public GISLineStringFeature(IGISAttributes attributes) : base(attributes) { }

        public LineString FeatureShape
        {
            get
            {
                return Shape as LineString;
            }
            set
            {
                Shape = value;
            }
        }
    }
}
