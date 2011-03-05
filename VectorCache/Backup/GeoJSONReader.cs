using System;
using System.Collections.Generic;
using System.Text;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.Features;
using System.IO;
using Jayrock.Json;

namespace DTS.Common.GIS
{
    public class GeoJSONReader
    {
        public static Geometry Read(JsonTextReader jreader)
        {
            if (jreader == null)
                throw new ArgumentNullException("reader", "A valid JSON reader object is required.");

            Geometry geometry = null;
            if (jreader.MoveToContent())
            {
                if (jreader.TokenClass == JsonTokenClass.Object)
                {
                    jreader.ReadToken(JsonTokenClass.Object);

                    //Read the 'type' property that indicates the type of the geometry or object
                    jreader.ReadMember();
                    string geometryType = jreader.ReadString();

                    switch (geometryType)
                    {
                        case "Point":
                            //Read the 'coordinates' property
                            jreader.ReadMember();
                            geometry = ReadPoint(jreader);
                            break;
                        case "MultiPoint":
                            //Read the 'coordinates' property
                            jreader.ReadMember();
                            geometry = ReadMultiPoint(jreader);
                            break;
                        case "LineString":
                            //Read the 'coordinates' property
                            jreader.ReadMember();
                            geometry = ReadLineString(jreader);
                            break;
                        case "MultiLineString":
                            //Read the 'coordinates' property
                            jreader.ReadMember();
                            geometry = ReadMultiLineString(jreader);
                            break;
                        case "Polygon":
                            //Read the 'coordinates' property
                            jreader.ReadMember();
                            geometry = ReadPolygon(jreader);
                            break;
                        case "MultiPolygon":
                            //Read the 'coordinates' property
                            jreader.ReadMember();
                            geometry = ReadMultiPolygon(jreader);
                            break;
                        case "GeometryCollection":
                            //Read the 'coordinates' property
                            jreader.ReadMember();
                            geometry = ReadGeometryCollection(jreader);
                            break;
                        default:
                            break;
                    }

                    jreader.ReadToken(JsonTokenClass.EndObject);
                }
            }
            return geometry;
        }

        public static Geometry Read(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("writer", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            return Read(jreader);
        }

        public static void Read(ref Coordinate coordinate, JsonTextReader jreader)
        {
            if (coordinate == null)
                throw new ArgumentNullException("coordinate", "A valid coordinate reference is required.");
            if (jreader == null)
                throw new ArgumentNullException("jreader", "A valid JSON reader object is required.");

            if (jreader.MoveToContent() && jreader.TokenClass == JsonTokenClass.Array)
            {
                jreader.ReadToken(JsonTokenClass.Array);
                coordinate.X = Convert.ToDouble(jreader.ReadNumber());
                coordinate.Y = Convert.ToDouble(jreader.ReadNumber());
                coordinate.Z = double.NaN;
                if (jreader.TokenClass == JsonTokenClass.Number)
                    coordinate.Z = Convert.ToDouble(jreader.ReadNumber());
                jreader.ReadToken(JsonTokenClass.EndArray);
            }
        }

        public static void Read(ref Coordinate coordinate, TextReader reader)
        {
            if (coordinate == null)
                throw new ArgumentNullException("coordinate", "A valid coordinate reference is required.");
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            Read(ref coordinate, jreader);
        }

        public static void Read(ref Coordinate[] coordinates, JsonTextReader jreader)
        {
            if (jreader == null)
                throw new ArgumentNullException("reader", "A valid JSON reader object is required.");

            if(jreader.MoveToContent() && jreader.TokenClass == JsonTokenClass.Array)
            {
                jreader.ReadToken(JsonTokenClass.Array);
                    List<Coordinate> list = new List<Coordinate>();
                    while (jreader.TokenClass == JsonTokenClass.Array)
                    {
                        Coordinate item = new Coordinate();
                        Read(ref item, jreader);
                        list.Add(item);
                    }
                jreader.ReadToken(JsonTokenClass.EndArray);

                coordinates = list.ToArray();
            }
        }

        public static void Read(ref Coordinate[] coordinates, TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            Read(ref coordinates, jreader);
        }

        private static Point ReadPoint(JsonTextReader jreader)
        {
            if (jreader == null)
                throw new ArgumentNullException("reader", "A valid JSON reader object is required.");

            Point point = null;
            if (jreader.TokenClass == JsonTokenClass.Array)
            {
                jreader.ReadToken(JsonTokenClass.Array);
                double x = Convert.ToDouble(jreader.ReadNumber());
                double y = Convert.ToDouble(jreader.ReadNumber());
                double z = double.NaN;
                if (jreader.TokenClass == JsonTokenClass.Number)
                    z = Convert.ToDouble(jreader.ReadNumber());
                jreader.ReadToken(JsonTokenClass.EndArray);

                point = new Point(x, y, z);
            }
            return point;
        }

        private static Point ReadPoint(ref Point point, TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            return ReadPoint(jreader);
        }

        private static MultiPoint ReadMultiPoint(JsonTextReader jreader)
        {
            if (jreader == null)
                throw new ArgumentNullException("reader", "A valid JSON reader object is required.");

            MultiPoint points = null;
            if (jreader.TokenClass == JsonTokenClass.Array)
            {
                jreader.ReadToken(JsonTokenClass.Array);
                List<Point> list = new List<Point>();
                while (jreader.TokenClass == JsonTokenClass.Array)
                {
                    Coordinate item = new Coordinate();
                    Read(ref item, jreader);
                    list.Add(new Point(item));
                }
                jreader.ReadToken(JsonTokenClass.EndArray);

                points = new MultiPoint(list.ToArray());
            }
            return points;
        }

        private static MultiPoint ReadMultiPoint(ref MultiPoint points, TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            return ReadMultiPoint(jreader);
        }

        private static LineString ReadLineString(JsonTextReader jreader)
        {
            if (jreader == null)
                throw new ArgumentNullException("reader", "A valid JSON reader object is required.");

            LineString line = null;
            if (jreader.TokenClass == JsonTokenClass.Array)
            {
                jreader.ReadToken(JsonTokenClass.Array);
                    List<Coordinate> list = new List<Coordinate>();
                    while (jreader.TokenClass == JsonTokenClass.Array)
                    {
                        Coordinate item = new Coordinate();
                        Read(ref item, jreader);
                        list.Add(item);
                    }
                jreader.ReadToken(JsonTokenClass.EndArray);

                line = new LineString(list.ToArray());
            }
            return line;
        }

        private static LineString ReadLineString(ref LineString line, TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            return ReadLineString(jreader);
        }

        private static MultiLineString ReadMultiLineString(JsonTextReader jreader)
        {
            if (jreader == null)
                throw new ArgumentNullException("reader", "A valid JSON reader object is required.");

            MultiLineString lines = null;
            if (jreader.TokenClass == JsonTokenClass.Array)
            {
                jreader.ReadToken(JsonTokenClass.Array);
                    List<LineString> list = new List<LineString>();
                    while (jreader.TokenClass == JsonTokenClass.Array)
                    {
                        list.Add(ReadLineString(jreader));
                    }
                jreader.ReadToken(JsonTokenClass.EndArray);

                lines = new MultiLineString(list.ToArray());
            }
            return lines;
        }

        private static MultiLineString ReadMultiLineString(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            return ReadMultiLineString(jreader);
        }

        private static Polygon ReadPolygon(JsonTextReader jreader)
        {
            if (jreader == null)
                throw new ArgumentNullException("reader", "A valid JSON reader object is required.");

            Polygon area = null;
            if (jreader.TokenClass == JsonTokenClass.Array)
            {
                jreader.ReadToken(JsonTokenClass.Array);

                //Read the outer shell
                LinearRing shell = null;
                if (jreader.TokenClass == JsonTokenClass.Array)
                {
                    Coordinate[] coordinates = new Coordinate[] { };
                    Read(ref coordinates, jreader);
                    shell = new LinearRing(coordinates);
                }

                //Read all the holes
                List<LinearRing> list = new List<LinearRing>();
                while (jreader.TokenClass == JsonTokenClass.Array)
                {
                    Coordinate[] coordinates = new Coordinate[] { };
                    Read(ref coordinates, jreader);
                    LinearRing hole = new LinearRing(coordinates);
                    list.Add(hole);
                }

                jreader.ReadToken(JsonTokenClass.EndArray);

                //An outer shell was found so a polygon can be created
                if (shell != null)
                {
                    if (list.Count > 0)
                        area = new Polygon(shell, list.ToArray());
                    else
                        area = new Polygon(shell);
                }
            }
            return area;
        }

        private static Polygon ReadPolygon(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            return ReadPolygon(jreader);
        }

        private static MultiPolygon ReadMultiPolygon(JsonTextReader jreader)
        {
            if (jreader == null)
                throw new ArgumentNullException("reader", "A valid JSON reader object is required.");

            MultiPolygon areas = null;
            if (jreader.TokenClass == JsonTokenClass.Array)
            {
                jreader.ReadToken(JsonTokenClass.Array);
                List<Polygon> polygons = new List<Polygon>();
                    while (jreader.TokenClass == JsonTokenClass.Array)
                    {
                        jreader.ReadToken(JsonTokenClass.Array);

                            //Read the outer shell
                            LinearRing shell = null;
                            if (jreader.TokenClass == JsonTokenClass.Array)
                            {
                                Coordinate[] coordinates = new Coordinate[] { };
                                Read(ref coordinates, jreader);
                                shell = new LinearRing(coordinates);
                            }

                            //Read all the holes
                            List<LinearRing> list = new List<LinearRing>();
                            while (jreader.TokenClass == JsonTokenClass.Array)
                            {
                                Coordinate[] coordinates = new Coordinate[] { };
                                Read(ref coordinates, jreader);
                                LinearRing hole = new LinearRing(coordinates);
                                list.Add(hole);
                            }

                        jreader.ReadToken(JsonTokenClass.EndArray);

                        //An outer shell was found so a polygon can be created
                        if (shell != null)
                        {
                            Polygon area = null;
                            if (list.Count > 0)
                                area = new Polygon(shell, list.ToArray());
                            else
                                area = new Polygon(shell);
                            polygons.Add(area);
                        }
                    }
                jreader.ReadToken(JsonTokenClass.EndArray);

                areas = new MultiPolygon(polygons.ToArray());
            }
            return areas;
        }

        private static MultiPolygon ReadMultiPolygon(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            return ReadMultiPolygon(jreader);
        }

        private static GeometryCollection ReadGeometryCollection(JsonTextReader jreader)
        {
            if (jreader == null)
                throw new ArgumentNullException("reader", "A valid JSON reader object is required.");

            GeometryCollection geometries = null;
            if (jreader.TokenClass == JsonTokenClass.Array)
            {
                jreader.ReadToken(JsonTokenClass.Array);
                    List<Geometry> list = new List<Geometry>();
                    while (jreader.TokenClass == JsonTokenClass.Object)
                    {
                        Geometry geometry = Read(jreader);
                        list.Add(geometry);
                    }
                jreader.ReadToken(JsonTokenClass.EndArray);

                geometries = new GeometryCollection(list.ToArray());
            }
            return geometries;
        }

        private static GeometryCollection ReadGeometryCollection(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            return ReadGeometryCollection(jreader);
        }

        public static Feature ReadFeature(JsonTextReader jreader)
        {
            if (jreader == null)
                throw new ArgumentNullException("jreader", "A valid JSON reader object is required.");

            Feature feature = new Feature();
            if (jreader.MoveToContent() && jreader.TokenClass == JsonTokenClass.Object)
            {
                jreader.ReadToken(JsonTokenClass.Object);

                //Read the 'Feature' as the type
                jreader.ReadMember(); //reads 'type'
                jreader.ReadString(); //reads 'Feature'

                //Read the 'geometry'
                jreader.ReadMember(); //reads 'geometry'
                feature.Geometry = Read(jreader); //reads the geometry value

                //Read the 'properties'
                jreader.ReadMember(); //reads 'properties'
                feature.Attributes = ReadAttributesTable(jreader);

                jreader.ReadToken(JsonTokenClass.EndObject);
            }
            return feature;
        }

        public static Feature ReadFeature(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            return ReadFeature(jreader);
        }

        public static void ReadGISAttributes(IGISAttributes attributes, JsonTextReader jreader)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes", "A valid attributes reference is required.");
            if (jreader == null)
                throw new ArgumentNullException("jreader", "A valid JSON reader object is required.");

            if (jreader.MoveToContent() && jreader.TokenClass == JsonTokenClass.Object)
            {
                jreader.ReadToken(JsonTokenClass.Object);

                while (jreader.TokenClass == JsonTokenClass.Member)
                {
                    string key = jreader.ReadMember();
                    string value = jreader.ReadString();

                    attributes.SetValue(key, value);
                }

                jreader.ReadToken(JsonTokenClass.EndObject);
            }
        }

        public static void ReadGISAttributes(IGISAttributes attributes, TextReader reader)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes", "A valid attributes reference is required.");
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            ReadGISAttributes(attributes, jreader);
        }

        public static void ReadGISFeature(IGISFeature feature, JsonTextReader jreader)
        {
            if (feature == null)
                throw new ArgumentNullException("feature", "A valid feature reference is required.");
            if (jreader == null)
                throw new ArgumentNullException("jreader", "A valid JSON reader object is required.");

            if (jreader.MoveToContent() && jreader.TokenClass == JsonTokenClass.Object)
            {
                jreader.ReadToken(JsonTokenClass.Object);

                //Read the 'Feature' as the type
                jreader.ReadMember(); //reads 'type'
                jreader.ReadString(); //reads 'Feature'

                //Read the 'geometry'
                jreader.ReadMember(); //reads 'geometry'
                feature.Shape = Read(jreader); //reads the geometry value

                //Read the 'properties'
                jreader.ReadMember(); //reads 'properties'
                ReadGISAttributes(feature.Attributes, jreader);

                jreader.ReadToken(JsonTokenClass.EndObject);
            }
        }

        public static void ReadGISFeature(IGISFeature feature, TextReader reader)
        {
            if (feature == null)
                throw new ArgumentNullException("feature", "A valid feature reference is required.");
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            ReadGISFeature(feature, jreader);
        }

        public static IAttributesTable ReadAttributesTable(JsonTextReader jreader)
        {
            if (jreader == null)
                throw new ArgumentNullException("jreader", "A valid JSON reader object is required.");

            IAttributesTable attributes = null;
            if (jreader.MoveToContent() && jreader.TokenClass == JsonTokenClass.Object)
            {
                attributes = new AttributesTable();
                jreader.ReadToken(JsonTokenClass.Object);

                    while (jreader.TokenClass == JsonTokenClass.Member)
                    {
                        string key = jreader.ReadMember();
                        string value = jreader.ReadString();

                        if (!attributes.Exists(key))
                            attributes.AddAttribute(key, null);
                        attributes[key] = value;
                    }

                jreader.ReadToken(JsonTokenClass.EndObject);
            }
            return attributes;
        }

        public static IAttributesTable ReadAttributesTable(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            return ReadAttributesTable(jreader);
        }

        public static IEnumerable<Feature> ReadFeatureCollection(JsonTextReader jreader)
        {
            if (jreader == null)
                throw new ArgumentNullException("jreader", "A valid JSON reader object is required.");

            List<Feature> features = new List<Feature>();
            if (jreader.MoveToContent() && jreader.TokenClass == JsonTokenClass.Object)
            {
                jreader.ReadToken(JsonTokenClass.Object);

                //Read the 'FeatureCollection' as the type
                jreader.ReadMember(); //reads 'type'
                jreader.ReadString(); //reads 'FeatureCollection'

                //Read the 'features' property
                jreader.ReadMember(); //reads 'features'
                if (jreader.TokenClass == JsonTokenClass.Array)
                {
                    jreader.ReadToken(JsonTokenClass.Array);
                    
                    while (jreader.TokenClass == JsonTokenClass.Object)
                    {
                        features.Add(ReadFeature(jreader));
                    }

                    jreader.ReadToken(JsonTokenClass.EndArray);
                }

                jreader.ReadToken(JsonTokenClass.EndObject);
                return features;
            }
            return null;
        }

        public static IEnumerable<Feature> ReadFeatureCollection(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "A valid text reader object is required.");

            JsonTextReader jreader = new JsonTextReader(reader);
            return ReadFeatureCollection(jreader);
        }
    }
}
