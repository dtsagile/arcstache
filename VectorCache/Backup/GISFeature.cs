using System;
using System.Collections.Generic;
using System.Text;
using DTS.Common;
using System.Xml;
using System.Xml.Serialization;
using GisSharpBlog.NetTopologySuite.Geometries;
using System.IO;
using GisSharpBlog.NetTopologySuite.IO;
using Jayrock.Json;

namespace DTS.Common.GIS
{
    public abstract class GISFeature : IGISFeature
    {
        IGISAttributes _attributes;

        public GISFeature(IGISAttributes attributes) 
        {
            _attributes = attributes;
        }

        #region IGISFeature Members

        [XmlIgnore]
        public IGISAttributes Attributes
        {
            get
            {
                return _attributes;
            }
        }

        public abstract Geometry Shape{get;set;}

        

        #endregion

        #region IGISXMLSerializable Members

        public void ToXML(XmlWriter writer)
        {
            writer.WriteStartElement("Feature");

            if (Shape != null)
                writer.WriteElementString("Geometry", Shape.ToText());

            if (Attributes != null)
                Attributes.ToXML(writer);

            writer.WriteEndElement();
        }

        public void FromXML(XmlReader reader)
        {
            if (reader.IsStartElement("Feature"))
            {
                reader.ReadStartElement("Feature");

                //Read the geometry
                if (reader.IsStartElement("Geometry"))
                {
                    reader.ReadStartElement("Geometry");
                    string wkt = reader.ReadString();
                    WKTReader wktReader = new WKTReader();
                    Shape = wktReader.Read(wkt);
                    reader.ReadEndElement();//Read [Geometry] end
                }

                //Read the attributes
                if (Attributes != null)
                {
                    Attributes.FromXML(reader);
                }
                else
                {
                    throw new NullReferenceException("Cannot read attribute values into feature. The [Attributes] property is NULL.");
                }

                reader.ReadEndElement();//Read [Feature] end
            }
        }

        #endregion

        #region IGISJSONSerializable Members

        public void ToJSON(JsonTextWriter jwriter)
        {
            GeoJSONWriter.Write(this, jwriter);
        }

        public void FromJSON(JsonTextReader jreader)
        {
            GeoJSONReader.ReadGISFeature(this as IGISFeature, jreader);
        }

        #endregion

        #region "Static Members"

        #region "Specific Shapes to XML"

        public static void ToXML(Polygon[] polygons, XmlWriter writer)
        {
            writer.WriteStartElement("Polygons");

            foreach (Polygon polygon in polygons)
            {
                ToXML(polygon, writer);
            }

            writer.WriteEndElement();
        }

        public static string ToXML(Polygon[] polygons)
        {
            StringBuilder text = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(text);

            ToXML(polygons, writer);
            writer.Flush();

            return text.ToString();
        }

        public static void ToXML(Polygon polygon, XmlWriter writer)
        {
            writer.WriteStartElement("Polygon");

            writer.WriteStartElement("Shell");
            ToXML(polygon.Shell, writer);
            writer.WriteEndElement();

            writer.WriteStartElement("Holes");
            ToXML(polygon.Holes, writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public static string ToXML(Polygon polygon)
        {
            StringBuilder text = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(text);

            ToXML(polygon, writer);
            writer.Flush();

            return text.ToString();
        }

        public static void ToXML(LinearRing[] rings, XmlWriter writer)
        {
            writer.WriteStartElement("LinearRings");

            foreach (LinearRing ring in rings)
            {
                ToXML(ring, writer);
            }

            writer.WriteEndElement();
        }

        public static string ToXML(LinearRing[] rings)
        {
            StringBuilder text = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(text);

            ToXML(rings, writer);
            writer.Flush();

            return text.ToString();
        }

        public static void ToXML(LinearRing ring, XmlWriter writer)
        {
            writer.WriteStartElement("LinearRing");

            ToXML(ring.Coordinates, writer);

            writer.WriteEndElement();
        }

        public static string ToXML(LinearRing ring)
        {
            StringBuilder text = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(text);

            ToXML(ring, writer);
            writer.Flush();

            return text.ToString();
        }

        public static void ToXML(LineString[] lines, XmlWriter writer)
        {
            writer.WriteStartElement("LineStrings");

            foreach (LineString line in lines)
            {
                ToXML(line, writer);
            }

            writer.WriteEndElement();
        }

        public static string ToXML(LineString[] lines)
        {
            StringBuilder text = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(text);

            ToXML(lines, writer);
            writer.Flush();

            return text.ToString();
        }

        public static void ToXML(LineString line, XmlWriter writer)
        {
            writer.WriteStartElement("LineString");

            ToXML(line.Coordinates, writer);

            writer.WriteEndElement();
        }

        public static string ToXML(LineString line)
        {
            StringBuilder text = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(text);

            ToXML(line, writer);
            writer.Flush();

            return text.ToString();
        }

        public static string ToXML(Coordinate[] coords)
        {
            StringBuilder text = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(text);
            ToXML(coords, writer);
            writer.Flush();
            return text.ToString();
        }

        public static void ToXML(Coordinate[] coords, XmlWriter writer)
        {
            writer.WriteStartElement("Coordinates");
            foreach (Coordinate coord in coords)
            {
                ToXML(coord, writer);
            }
            writer.WriteEndElement();
        }

        public static string ToXML(Coordinate coord)
        {
            StringBuilder text = new StringBuilder();
            TextWriter tw = new StringWriter(text);
            XmlWriter writer = XmlWriter.Create(text);
            ToXML(coord, writer);
            writer.Flush();
            return text.ToString();
        }

        public static void ToXML(Coordinate coord, XmlWriter writer)
        {
            writer.WriteStartElement("Coordinate");
            writer.WriteElementString("X", coord.X.ToString());
            writer.WriteElementString("Y", coord.Y.ToString());
            writer.WriteElementString("Z", coord.Z.ToString());
            writer.WriteEndElement();
        }

        #endregion

        #endregion
    }
}
