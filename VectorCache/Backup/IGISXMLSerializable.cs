using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DTS.Common;

namespace DTS.Common.GIS
{
    public interface IGISXMLSerializable
    {
        void ToXML(XmlWriter writer);
        void FromXML(XmlReader reader);
    }
}
