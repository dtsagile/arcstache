using System;
using System.Collections.Generic;
using System.Text;

namespace DTS.Common.GIS
{
    public class InMemoryGISPointLayer: InMemoryGISLayer
    {
        public InMemoryGISPointLayer(string layerName, string keyFieldName) : base(layerName, keyFieldName) { }

        public override InMemoryGISFeature CreateFeature()
        {
            return new InMemoryGISPointFeature();
        }
    }
}
