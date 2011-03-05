using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Jayrock.Json;

namespace DTS.Common.GIS
{
    public interface IGISJSONSerializable
    {
        void ToJSON(JsonTextWriter jwriter);
        void FromJSON(JsonTextReader jreader);
    }
}
