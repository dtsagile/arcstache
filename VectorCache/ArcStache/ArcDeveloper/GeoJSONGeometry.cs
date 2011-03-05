//======================================================
#region  ArcDeveloper MIT License
//Copyright (c) 2007 ArcDeveoper.net
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
#endregion
//======================================================
// Author: ggoodrich
// Date Created: 2/8/2008 8:28:53 AM
//======================================================
// Author: ggoodrich
// Date Created: 2/8/2008 8:28:53 AM
// Description:
//
//======================================================
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
//using ArcDeveloper.REST.Core.Interfaces;
using ArcDeveloper.REST.Core.Model;
using ESRI.ArcGIS.Geometry;

namespace ArcDeveloper
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class GeoJSONGeometry:GeoJSONResponse
    {

        private string _coordinates;

        ///<summary>
        /// Default Constructor
        /// </summary>
        ///<remarks></remarks>
        public GeoJSONGeometry()
        {

        }


        

        [DataMember]
        public string coordinates
        {
            get
            {
               
                return _coordinates;
            }
            set { _coordinates = value; }
        }
       
        /// <summary>
        /// Create a geojson string from the geometry
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static GeoJSONGeometry CreateFromIGeometry(IGeometry geometry)
        {
            GeoJSONGeometry jsonGeom = new GeoJSONGeometry();
            jsonGeom.Type = geometry.GeometryType.ToString();
            StringBuilder sb = new StringBuilder();
            if (geometry.GeometryType!=esriGeometryType.esriGeometryPoint)
            {
                sb.Append("[");
            }
            //Need to work out how to easily rip the coords out of the IGeometry

            switch(geometry.GeometryType)
            {
                case esriGeometryType.esriGeometryPoint:
                    IPoint pt = (IPoint)geometry;
                    sb.Append( string.Format("[{0}, {1}]", Math.Round(pt.X, 5), Math.Round(pt.Y, 5)));
                    jsonGeom.Type = "Point";
                    break;

                case esriGeometryType.esriGeometryLine:
                    IPolyline line = geometry as IPolyline;
                    if (line == null) return null;

                    line.Densify(-1, -1); //make sure it's all straight line segments
                    line.Weed(20);  //weed out some vertices
                    line.SimplifyNetwork();  //make sure it is simple

                    
                    IPointCollection points = line as IPointCollection;
                    for (int i = 0; i < points.PointCount; i++)
                    {
                        IPoint point = points.get_Point(i);
                        if (sb.Length > 1)
                            sb.Append(",");
                        sb.Append(string.Format("[{0}, {1}]", Math.Round(point.X, 4), Math.Round(point.Y, 4)));
                    }
                   
                    jsonGeom.Type = "LineString";
                    break;

                case esriGeometryType.esriGeometryPolygon:
                     IPolygon4 poly = geometry as IPolygon4;
                    if (poly == null) return null;

                    poly.Densify(-1, -1); //make sure it is all straight line segments
                    poly.Weed(20); //weed out some vertices
                    poly.SimplifyPreserveFromTo(); //make sure it's simple                   

                    //We aren't gonna deal with interior rings right now (ie - no holes in polygons)
                    IGeometryBag multiRing = poly.ExteriorRingBag;
                    IEnumGeometry exteriorRingsEnum = multiRing as IEnumGeometry;
                    exteriorRingsEnum.Reset();
                    IRing currentExteriorRing = exteriorRingsEnum.Next() as IRing;
                    while (currentExteriorRing != null)
                    {
                        
                        if (!currentExteriorRing.IsClosed) currentExteriorRing.Close();

                        IPointCollection multiRingPoints = currentExteriorRing as IPointCollection;
                        for (int pointIdx = 0; pointIdx < multiRingPoints.PointCount; pointIdx++)
                        {
                            IPoint multiRingPoint = multiRingPoints.get_Point(pointIdx);
                            //coords.Add(new GisSharpBlog.NetTopologySuite.Geometries.Coordinate(Math.Round(multiRingPoint.X, 5), Math.Round(multiRingPoint.Y, 5)));
                            if (sb.Length > 1)
                                sb.Append(",");
                            sb.Append(string.Format("[{0}, {1}]", Math.Round(multiRingPoint.X, 4), Math.Round(multiRingPoint.Y, 4)));
                        }

                        currentExteriorRing = exteriorRingsEnum.Next() as IRing;
                    }
                    jsonGeom.Type = "Polygon";
                    break;

            }


            if (geometry.GeometryType != esriGeometryType.esriGeometryPoint)
            {
                sb.Append("]");
            }
            jsonGeom.coordinates = sb.ToString();
            return jsonGeom;
        }
        

 

        
    }
}
