using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;
using DTS.Common;
using System.Xml;

namespace DTS.Common.GIS
{
    public interface IGISFeature : IGISXMLSerializable, IGISJSONSerializable
    {
        Geometry Shape { get; set;}
        IGISAttributes Attributes { get; }
    }
}
