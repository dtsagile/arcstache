using System;
using System.Collections.Generic;
using System.Text;
using ArcStache;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace ArcStache.Tests
{
    [TestFixture]
    public class Util
    {


        [Test]
        public void Tile_0_0_0_Returns_Entire_World()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Server);
                   
            IEnvelope e = TileUtil.GetEnvelopeFromZoomRowCol(0, 0, 0);
            Assert.AreEqual(e.XMax, 180);
            Assert.AreEqual(e.XMin, -180);
            Assert.AreApproximatelyEqual(e.YMax, 85.0511, 0.001);
            Assert.AreApproximatelyEqual(e.YMin, -85.0511, 0.001);           
        }


        [Test]
        public void Tile_13_3083_1702()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Server);

            IPoint p1 = TileUtil.TileToGeographic(1702, 3083, 13);
            IPoint p2 = TileUtil.TileToGeographic(1703, 3084, 13);
            

            IEnvelope e = TileUtil.GetEnvelopeFromZoomRowCol(13, 1702, 3083);
            Assert.AreApproximatelyEqual(e.UpperLeft.X, p1.X, 0.001);
            Assert.AreApproximatelyEqual(e.LowerRight.X, p2.X, 0.001);
            Assert.AreApproximatelyEqual(e.UpperLeft.Y, p1.Y, 0.001);
            Assert.AreApproximatelyEqual(e.LowerRight.Y, p2.Y, 0.001);
        }

        [Test]
        public void TileToWorldPosition()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Server);
            IPoint p = TileUtil.TileToWorldPos(1702, 3083,13);
            Assert.AreApproximatelyEqual<double, double>(p.X, -105.20, 0.1);
            Assert.AreApproximatelyEqual<double, double>(p.Y, 40.61, 0.1);

        }


        [Test]
        public void TileToGeographic()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Server);
            IPoint p = TileUtil.TileToGeographic(1702, 3083, 13);
            Assert.AreApproximatelyEqual<double, double>(p.X, -105.20, 0.1);
            Assert.AreApproximatelyEqual<double, double>(p.Y, 40.61, 0.1);

        }

        [Test]
        public void TileToGeographicPoints()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Server);
            IPoint p1 = TileUtil.TileToGeographic(1702, 3083, 13);
            IPoint p2 = TileUtil.TileToGeographic(1703, 3084, 13);
            Assert.AreApproximatelyEqual<double, double>(p1.X, -105.20, 0.1);
            Assert.AreApproximatelyEqual<double, double>(p1.Y, 40.61, 0.1);            

        }



    }
}
