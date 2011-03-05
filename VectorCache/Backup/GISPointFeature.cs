using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;
using System.Xml.Serialization;

namespace DTS.Common.GIS
{
    public abstract class GISPointFeature: GISFeature
    {
        public GISPointFeature(IGISAttributes attributes) : base(attributes) { }

        public Point FeatureShape
        {
            get
            {
                return Shape as Point;
            }
            set
            {
                Shape = value;
            }
        }
    }
}
