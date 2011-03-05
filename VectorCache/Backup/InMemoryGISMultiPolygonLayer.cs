using System;
using System.Collections.Generic;
using System.Text;

namespace DTS.Common.GIS
{
    public class InMemoryGISMultiPolygonLayer: InMemoryGISLayer
    {
        public InMemoryGISMultiPolygonLayer(string layerName, string keyFieldName) : base(layerName, keyFieldName) { }

        public override InMemoryGISFeature CreateFeature()
        {
            return new InMemoryGISMultiPolygonFeature();
        }
    }
}
