using System;
using System.Collections.Generic;
using System.Text;

namespace DTS.Common.GIS
{
    public interface IGISAttributes : IGISXMLSerializable, IGISJSONSerializable
    {
        object GetValue(string attribute);
        void SetValue(string attribute, object value);
        IEnumerable<string> GetKeys();
    }
}
