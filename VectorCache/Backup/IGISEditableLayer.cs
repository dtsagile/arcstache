using System;
using System.Collections.Generic;
using System.Text;

namespace DTS.Common.GIS
{
    public interface IGISEditableLayer
    {
        void Add(IGISFeature feature);
        void Update(IGISFeature feature);
        void Delete(IGISFeature feature);
    }
}
