using System;
using System.Collections.Generic;
using System.Text;

namespace DTS.Common.GIS
{
    public interface IGISLayer: IGISXMLSerializable, IGISJSONSerializable
    {
        string LayerName { get;}
        string KeyFieldName { get;}
        IGISFeature Current { get;}
        bool MoveNext();
        void Search(string queryString);
    }
}
