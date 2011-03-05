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
// Date Created: 2/8/2008 8:27:37 AM
//======================================================
// Author: ggoodrich
// Date Created: 2/8/2008 8:27:37 AM
// Description:
//
//======================================================
using System.Runtime.Serialization;
using System.Text;
using ESRI.ArcGIS.Geodatabase;



namespace ArcDeveloper
{
    /// <summary>
    /// Represents a GeoJSON feature, per the GeoJSON spec
    /// </summary>
    [DataContract]
    public class GeoJSONFeature 
    {
     

        private GeoJSONGeometry _geometry;
        private string _id;
        private string _properties;
        public string Type { get; set; }
     

       
        ///<summary>
        /// Default Constructor
        /// </summary>
        ///<remarks></remarks>
        public GeoJSONFeature()
        {
            Type = "Feature";
        }

     
        /// <summary>
        /// The ID of the feature
        /// </summary>
        /// <value>The id.</value>
        [DataMember]
        public string id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// The geometry of the feature
        /// </summary>
        /// <value>The geometry.</value>
        [DataMember]
        public GeoJSONGeometry geometry
        {
            get { return _geometry; }
            set { _geometry = value; }
        }
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [DataMember]
        public string properties
        {
            get { return _properties; }
            set { _properties = value; }
        }
      

        /// <summary>
        /// Creates A GeoJSON feature from an IFeature
        /// </summary>
        /// <param name="feat">The feature.</param>
        /// <returns>A GeoJSON Feature</returns>
        public static GeoJSONFeature CreateFromIFeature(IFeature feat)
        {
            GeoJSONFeature jsonFeature = new GeoJSONFeature();
            jsonFeature.id = feat.OID.ToString();
            jsonFeature.geometry = GeoJSONGeometry.CreateFromIGeometry(feat.Shape);
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            
            for (int i = 0; i < feat.Fields.FieldCount; i++)
            {
                ESRI.ArcGIS.Geodatabase.IField fld = feat.Fields.get_Field(i);
                switch (fld.Name.ToUpper())
                {
                    case "SHAPE":                        
                        break;
                    default:
                        string val = "null";
                        if (feat.get_Value(i) != null)
                            val = feat.get_Value(i).ToString();
                        if(sb.Length > 1)
                            sb.Append(",");
                        sb.Append(string.Format("\"{0}\":\"{1}\"", fld.Name, val));
                        
                        break;
                }
            }

            jsonFeature.properties = sb.ToString();            
            sb.Append("}");
            jsonFeature.properties = sb.ToString();
            return jsonFeature;
        }

       


    }
}