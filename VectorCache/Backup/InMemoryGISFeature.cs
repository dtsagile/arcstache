using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace DTS.Common.GIS
{
    public abstract class InMemoryGISFeature: GISFeature
    {
        public InMemoryGISFeature()
            : base(new GISSerializableDictionary()) { }

    }
}
