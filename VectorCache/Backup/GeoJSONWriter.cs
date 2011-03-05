using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.Features;
using System.IO;
using Jayrock.Json;

namespace DTS.Common.GIS
{
    public class GeoJSONWriter
    {
        public static void Write(Geometry geometry, JsonTextWriter jwriter)
        {
            if (geometry == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            if (geometry is Point)
            {
                Write(geometry as Point, jwriter);
            }
            else if (geometry is LineString)
            {
                Write(geometry as LineString, jwriter);
            }
            else if (geometry is Polygon)
            {
                Write(geometry as Polygon, jwriter);
            }
            else if (geometry is MultiPoint)
            {
                Write(geometry as MultiPoint, jwriter);
            }
            else if (geometry is MultiLineString)
            {
                Write(geometry as MultiLineString, jwriter);
            }
            else if (geometry is MultiPolygon)
            {
                Write(geometry as MultiPolygon, jwriter);
            }
            else if (geometry is GeometryCollection)
            {
                Write(geometry as GeometryCollection, jwriter);
            }
        }

        public static void Write(Geometry geometry, TextWriter writer)
        {
            if (geometry == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(geometry, jwriter);
        }

        public static void Write(Coordinate coordinate, JsonTextWriter jwriter)
        {
            if (coordinate == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartArray();
            jwriter.WriteNumber(coordinate.X);
            jwriter.WriteNumber(coordinate.Y);
            if (!double.IsNaN(coordinate.Z))
                jwriter.WriteNumber(coordinate.Z);
            jwriter.WriteEndArray();
        }

        public static void Write(Coordinate coordinate, TextWriter writer)
        {
            if (coordinate == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(coordinate, jwriter);
        }

        public static void Write(Coordinate[] coordinates, JsonTextWriter jwriter)
        {

            if (coordinates == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartArray();

            foreach (Coordinate entry in coordinates)
            {
                Write(entry, jwriter);
            }
            jwriter.WriteEndArray();
        }

        public static void Write(Coordinate[] coordinates, TextWriter writer)
        {
            if (coordinates == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(coordinates, jwriter);
        }

        public static void Write(Point point, JsonTextWriter jwriter)
        {
            if (point == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();
            
                jwriter.WriteMember("type");
                jwriter.WriteString("Point");

                jwriter.WriteMember("coordinates");
                jwriter.WriteStartArray();
                    jwriter.WriteNumber(point.X);
                    jwriter.WriteNumber(point.Y);
                    if (!double.IsNaN(point.Z))
                        jwriter.WriteNumber(point.Z);
                jwriter.WriteEndArray();

            jwriter.WriteEndObject();
        }

        public static void Write(Point point, TextWriter writer)
        {
            if (point == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(point, jwriter);
        }

        public static void Write(MultiPoint points, JsonTextWriter jwriter)
        {
            if (points == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();

                jwriter.WriteMember("type");
                jwriter.WriteString("MultiPoint");

                jwriter.WriteMember("coordinates");
                jwriter.WriteStartArray();
                    foreach (Coordinate entry in points.Coordinates)
                    {
                        Write(entry, jwriter);
                    }
                jwriter.WriteEndArray();

            jwriter.WriteEndObject();
        }

        public static void Write(MultiPoint points, TextWriter writer)
        {
            if (points == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(points, jwriter);
        }

        public static void Write(LineString line, JsonTextWriter jwriter)
        {
            if (line == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();

                jwriter.WriteMember("type");
                jwriter.WriteString("LineString");

                jwriter.WriteMember("coordinates");
                jwriter.WriteStartArray();
                    foreach (Coordinate entry in line.Coordinates)
                    {
                        Write(entry, jwriter);
                    }
                jwriter.WriteEndArray();

            jwriter.WriteEndObject();
        }

        public static void Write(LineString line, TextWriter writer)
        {
            if (line == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(line, jwriter);
        }

        public static void Write(MultiLineString lines, JsonTextWriter jwriter)
        {
            if (lines == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();

                jwriter.WriteMember("type");
                jwriter.WriteString("MultiLineString");

                jwriter.WriteMember("coordinates");
                jwriter.WriteStartArray();
                    foreach (LineString line in lines.Geometries)
                    {
                        Write(line.Coordinates, jwriter);
                    }
                jwriter.WriteEndArray();


            jwriter.WriteEndObject();
        }

        public static void Write(MultiLineString lines, TextWriter writer)
        {
            if (lines == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(lines, jwriter);
        }

        public static void Write(Polygon area, JsonTextWriter jwriter)
        {
            if (area == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();

                jwriter.WriteMember("type");
                jwriter.WriteString("Polygon");

                jwriter.WriteMember("coordinates");
                jwriter.WriteStartArray();

                    //Write the exterior boundary or shell
                    Write(area.Shell.Coordinates, jwriter);

                    //Write all the holes
                    foreach (LineString hole in area.Holes)
                    {
                        Write(hole.Coordinates, jwriter);
                    }

                jwriter.WriteEndArray();


            jwriter.WriteEndObject();
        }

        public static void Write(Polygon area, TextWriter writer)
        {
            if (area == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(area, jwriter);
        }

        public static void Write(MultiPolygon areas, JsonTextWriter jwriter)
        {
            if (areas == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();

                jwriter.WriteMember("type");
                jwriter.WriteString("MultiPolygon");

                jwriter.WriteMember("coordinates");
                jwriter.WriteStartArray();

                    foreach (Polygon area in areas.Geometries)
                    {
                        jwriter.WriteStartArray();

                        //Write the exterior boundary or shell
                        Write(area.Shell.Coordinates, jwriter);

                        //Write all the holes
                        foreach (LineString hole in area.Holes)
                        {
                            Write(hole.Coordinates, jwriter);
                        }

                        jwriter.WriteEndArray();
                    }

                jwriter.WriteEndArray();


            jwriter.WriteEndObject();
        }

        public static void Write(MultiPolygon areas, TextWriter writer)
        {
            if (areas == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(areas, jwriter);
        }

        public static void Write(GeometryCollection geometries, JsonTextWriter jwriter)
        {
            if (geometries == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();

            jwriter.WriteMember("type");
            jwriter.WriteString("GeometryCollection");

                jwriter.WriteMember("geometries");
                jwriter.WriteStartArray();

                foreach (Geometry geometry in geometries.Geometries)
                {
                    Write(geometry, jwriter);
                }

                jwriter.WriteEndArray();

            jwriter.WriteEndObject();
        }

        public static void Write(GeometryCollection geometries, TextWriter writer)
        {
            if (geometries == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(geometries, jwriter);
        }

        public static void Write(Feature feature, JsonTextWriter jwriter)
        {
            if (feature == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();

            jwriter.WriteMember("type");
            jwriter.WriteString("Feature");

            jwriter.WriteMember("geometry");
            Write(feature.Geometry, jwriter);

            jwriter.WriteMember("properties");
            Write(feature.Attributes, jwriter);

            jwriter.WriteEndObject();
        }

        public static void Write(Feature feature, TextWriter writer)
        {
            if (feature == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(feature, jwriter);
        }

        public static void Write(IEnumerable<Feature> features, JsonTextWriter jwriter)
        {
            if (features == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();

            jwriter.WriteMember("type");
            jwriter.WriteString("FeatureCollection");

            jwriter.WriteMember("features");
            jwriter.WriteStartArray();

                foreach (Feature feature in features)
                {
                    Write(feature, jwriter);
                }

            jwriter.WriteEndArray();

            jwriter.WriteEndObject();
        }

        public static void Write(IEnumerable<Feature> features, TextWriter writer)
        {
            if (features == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(features, jwriter);
        }

        public static void Write(IAttributesTable attributes, JsonTextWriter jwriter)
        {
            if (attributes == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();

            string[] names = attributes.GetNames();
            foreach (string name in names)
            {
                jwriter.WriteMember(name);
                jwriter.WriteString(attributes[name].ToString());
            }

            jwriter.WriteEndObject();
        }

        public static void Write(IAttributesTable attributes, TextWriter writer)
        {
            if (attributes == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(attributes, jwriter);
        }

        public static void Write(IGISFeature feature, JsonTextWriter jwriter)
        {
            if (feature == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();

            jwriter.WriteMember("type");
            jwriter.WriteString("Feature");

            jwriter.WriteMember("geometry");
            Write(feature.Shape, jwriter);

            jwriter.WriteMember("properties");
            Write(feature.Attributes, jwriter);

            jwriter.WriteEndObject();
        }

        public static void Write(IGISFeature feature, TextWriter writer)
        {
            if (feature == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(feature, jwriter);
        }

        public static void Write(IGISAttributes attributes, JsonTextWriter jwriter, string[] nonSerializedFields)
        {
            if (attributes == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();

            IEnumerable<string> names = attributes.GetKeys();
            foreach (string name in names)
            {
                if (nonSerializedFields != null && Contains(nonSerializedFields, name))
                    continue;
                jwriter.WriteMember(name);
                jwriter.WriteString(attributes.GetValue(name).ToString());
            }

            jwriter.WriteEndObject();
        }

        public static void Write(IGISAttributes attributes, JsonTextWriter jwriter)
        {
            Write(attributes, jwriter, null);
        }

        public static void Write(IGISAttributes attributes, TextWriter writer, string[] nonSerializedFields)
        {
            if (attributes == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(attributes, jwriter, nonSerializedFields);
        }

        public static void Write(IGISAttributes attributes, TextWriter writer)
        {
            Write(attributes, writer, null);
        }

        public static void Write(IGISLayer features, JsonTextWriter jwriter)
        {
            if (features == null)
                return;
            if (jwriter == null)
                throw new ArgumentNullException("jwriter", "A valid JSON writer object is required.");

            jwriter.WriteStartObject();

                jwriter.WriteMember("type");
                jwriter.WriteString("FeatureCollection");

                jwriter.WriteMember("features");
                jwriter.WriteStartArray();

                    while (features.MoveNext())
                    {
                        Write(features.Current, jwriter);
                    }

                jwriter.WriteEndArray();

            jwriter.WriteEndObject();
        }

        public static void Write(IGISLayer features, TextWriter writer)
        {
            if (features == null)
                return;
            if (writer == null)
                throw new ArgumentNullException("writer", "A valid text writer object is required.");

            JsonTextWriter jwriter = new JsonTextWriter(writer);
            Write(features, jwriter);
        }

        private static bool Contains(string[] list, string value)
        {
            if (list == null)
                return false;
            foreach (string entry in list)
            {
                if (string.Compare(value, entry, false) == 0)
                    return true;
            }
            return false;
        }
    }
}
