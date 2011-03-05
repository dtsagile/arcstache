using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.Specialized;

using System.Runtime.InteropServices;
using System.EnterpriseServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SOESupport;
using System.IO;
using ArcDeveloper;
using ArcStache.Extensions;

//TODO: sign the project (project properties > signing tab > sign the assembly)
//      this is strongly suggested if the dll will be registered using regasm.exe <your>.dll /codebase


namespace ArcStache
{
    [ComVisible(true)]
    [Guid("91EB05FE-7970-45F3-A51B-664310938EFF")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ArcStache.ArcStache")]
    public class VectorCache : ServicedComponent, IServerObjectExtension, IObjectConstruct, IRESTRequestHandler
    {
        private string soe_name;

        private IPropertySet configProps;
        private IServerObjectHelper serverObjectHelper;
        private ServerLogger logger;
        private IRESTRequestHandler reqHandler;

        private ComLogUtil _dtsLogger;

        private string _vectorCacheRootDirectory;

        private ISpatialReference _spatialReference;

        private Dictionary<int, esriGeometryType> _layerFeatureTypes = new Dictionary<int, esriGeometryType>();

        private const string NO_DATA_GEOJSON = "{\"type\":\"FeatureCollection\",\"features\":[]}";


        public VectorCache()
        {
            soe_name = "ArcStache";
            logger = new ServerLogger();            
            RestResource rootResource = CreateRestSchema();
            SoeRestImpl restImpl = new SoeRestImpl("arcstache", rootResource);
            reqHandler = (IRESTRequestHandler)restImpl;

        }


        public void Init(IServerObjectHelper pSOH)
        {
            serverObjectHelper = pSOH;
        }

        public void Shutdown()
        {
            const string methodName = "Shutdown";
            logger.LogMessage(ServerLogger.msgType.debug, methodName, 9999, "firing!");
            _dtsLogger.LogInfo(soe_name, methodName, "firing");
        }


        public void Construct(IPropertySet props)
        {
            const string methodName = "Construct";

            configProps = props;

            logger.LogMessage(ServerLogger.msgType.debug, methodName, 9999, "firing!");

            try
            {
                // Initialize our logger. Creates the folder and file if it doesn't already exist.
                _dtsLogger = new ComLogUtil();
                _dtsLogger.FileName = soe_name + "_Log.txt";
                _dtsLogger.LogInfo(soe_name, methodName, "DTSAgile logger initialized.");

                // Set the root cache directory the tiles should be written to
                // TODO:  Do we want the root location to be configurable??
                var rootDir =  @"C:\arcgis\" + soe_name;
                _vectorCacheRootDirectory = System.IO.Path.Combine(rootDir, this.CreateMapServiceCacheFolderName());

                this.ValidateMapServiceSpatialReference();
            }
            catch (Exception ex)
            {
                _dtsLogger.LogError(soe_name, methodName, "none", ex);
                logger.LogMessage(ServerLogger.msgType.error, methodName, 9999, "Failed to get ServerObject::ConfigurationName");
            }
        }




        #region IRESTRequestHandler Members

        public string GetSchema()
        {
            return reqHandler.GetSchema();
        }

        public byte[] HandleRESTRequest(string Capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties, out string responseProperties)
        {
            return reqHandler.HandleRESTRequest(Capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out responseProperties);
        }

        #endregion

        private RestResource CreateRestSchema()
        {
            RestResource rootRes = new RestResource("arcstache", false, RootResHandler);

            //RestResource layerResource = new RestResource("layerId", true, LayerResourceHandler, "GetInfo");
            //RestResource zoomResource = new RestResource("zoom", false, ZoomResourceHandler, "GetInfo");
            //RestResource colResource = new RestResource("x", false, ColResourceHandler, "GetInfo");
            //RestResource rowResource = new RestResource("y", true, RowResourceHandler, "GetInfo");


            RestOperation tileOperation = new RestOperation("tile",
                                                      new string[] { "l", "z", "x", "y", "jf" },
                                                      new string[] { "json", "html" },
                                                      VectorTileHandler);
            //colResource.resources.Add(rowResource);
            //zoomResource.resources.Add(colResource);            
            //layerResource.resources.Add(zoomResource);
            //rootRes.resources.Add(layerResource);
            //rowResource.operations.Add(tileOperation);

            rootRes.operations.Add(tileOperation);

            RestOperation emptytileOperation = new RestOperation("emptytile",
                                            new string[] { "l", "z", "x", "y", "jf" },
                                            new string[] { "json", "html" },
                                            EmptyVectorTileHandler);
            rootRes.operations.Add(emptytileOperation);

            return rootRes;
        }

        #region Handlers for the l/z/r/c urls - does not work
        private byte[] RootResHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        {
            responseProperties = null;
            JsonObject result = new JsonObject();
            return Encoding.UTF8.GetBytes(result.ToJson());
        }

        ///// <summary>
        ///// http://<service-url>/exts/ArcStache/<layerId>
        ///// Will return a list of all the layers that are configured to support vector tiling
        ///// </summary>
        ///// <param name="boundVariables"></param>
        ///// <param name="outputFormat"></param>
        ///// <param name="requestProperties"></param>
        ///// <param name="responseProperties"></param>
        ///// <returns></returns>
        //private byte[] LayerResourceHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        //{
        //    responseProperties = null;
        //    JsonObject result = new JsonObject();
        //    result.AddString("name", "LayerResourceHandler");
        //    return Encoding.UTF8.GetBytes(result.ToJson());
        //}

        ///// <summary>
        ///// http://<service-url>/exts/ArcStache/<layerId>/<zoom>
        ///// Returns a list of zoom levels used in this cache
        ///// </summary>
        ///// <param name="boundVariables"></param>
        ///// <param name="outputFormat"></param>
        ///// <param name="requestProperties"></param>
        ///// <param name="responseProperties"></param>
        ///// <returns></returns>
        //private byte[] ZoomResourceHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        //{
        //    responseProperties = null;
        //    JsonObject result = new JsonObject();
        //    result.AddString("name", "ZoomResourceHandler");
        //    return Encoding.UTF8.GetBytes(result.ToJson());
        //}

        ///// <summary>
        ///// Returns nothing by itself
        ///// </summary>
        ///// <param name="boundVariables"></param>
        ///// <param name="outputFormat"></param>
        ///// <param name="requestProperties"></param>
        ///// <param name="responseProperties"></param>
        ///// <returns></returns>
        //private byte[] ColResourceHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        //{
        //    responseProperties = null;
        //    JsonObject result = new JsonObject();
        //    result.AddString("name", "ColResourceHandler");
        //    return Encoding.UTF8.GetBytes(result.ToJson());
        //}

        ///// <summary>
        ///// Returns the tile
        ///// </summary>
        ///// <param name="boundVariables"></param>
        ///// <param name="outputFormat"></param>
        ///// <param name="requestProperties"></param>
        ///// <param name="responseProperties"></param>
        ///// <returns></returns>
        //private byte[] RowResourceHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        //{
        //    responseProperties = null;

        //    //pull the elements out of the resources
        //    int layerID = Convert.ToInt32(boundVariables["l"]);
        //    int zoom = Convert.ToInt32(boundVariables["z"]);
        //    int col = Convert.ToInt32(boundVariables["x"]);
        //    int row = Convert.ToInt32(boundVariables["y"]);

        //    IEnvelope e = TileUtil.GetEnvelopeFromZoomRowCol(zoom, row, col);

        //    JsonObject jObject = new JsonObject();
        //    jObject.AddString("envelope", String.Format("{0},{1},{2},{3}", e.XMin, e.XMax, e.YMin, e.YMax));
        //    return Encoding.UTF8.GetBytes(jObject.ToJson());


        //}
        #endregion

        /// <summary>
        /// Empty tile handler to support ArcGIS Flex API's TiledMapServiceLayer implementation which executes this call using a URL
        /// obtained from getTileUrl() method in the Flex layer implementation. 
        /// </summary>
        /// <param name="boundVariables">The bound variables.</param>
        /// <param name="operationInput">The operation input.</param>
        /// <param name="outputFormat">The output format.</param>
        /// <param name="requestProperties">The request properties.</param>
        /// <param name="responseProperties">The response properties.</param>
        /// <returns></returns>
        private byte[] EmptyVectorTileHandler(NameValueCollection boundVariables,
                                                  JsonObject operationInput,
                                                      string outputFormat,
                                                      string requestProperties,
                                                  out string responseProperties)
        {
            responseProperties = null;// "Content-Type:application/json";

            return StrToByteArray("{}");
        }

        /// <summary>
        /// Creates and Serves vector tiles
        /// </summary>
        /// <param name="boundVariables"></param>
        /// <param name="operationInput"></param>
        /// <param name="outputFormat"></param>
        /// <param name="requestProperties"></param>
        /// <param name="responseProperties"></param>
        /// <returns></returns>
        private byte[] VectorTileHandler(NameValueCollection boundVariables,
                                                  JsonObject operationInput,
                                                      string outputFormat,
                                                      string requestProperties,
                                                  out string responseProperties)
        {
            //responseProperties = null;
            responseProperties = null; //"Content-Type:application/json";
            ESRI.ArcGIS.SOESupport.AutoTimer timer = new AutoTimer();
            
            const string methodName = "VectorTileHandler";

            try
            {
                long? layerIndex;
                long? zoom;
                long? row;
                long? col;
                string jsonFormatParam;
                TileFormat jsonFormat = TileFormat.EsriJson; // Defaulting to EsriJson
                if (!operationInput.TryGetAsLong("l", out layerIndex))
                    throw new ArgumentNullException("layer");
                if (!operationInput.TryGetAsLong("z", out zoom))
                    throw new ArgumentNullException("zoom");
                if (!operationInput.TryGetAsLong("y", out row))
                    throw new ArgumentNullException("row");
                if (!operationInput.TryGetAsLong("x", out col))
                    throw new ArgumentNullException("col");
                if (operationInput.TryGetString("jf", out jsonFormatParam))
                {
                    if (!string.IsNullOrEmpty(jsonFormatParam))
                    {
                        jsonFormatParam = jsonFormatParam.ToLower().Trim();
                        Enum.GetNames(typeof(TileFormat)).ToList().ForEach(n =>
                        {
                            if (n.ToLower() == jsonFormatParam)
                            {
                                jsonFormat = (TileFormat)Enum.Parse(typeof(TileFormat), jsonFormatParam, true);
                            }
                        });
                    }
                }


                //System.Diagnostics.Debug.WriteLine(string.Format("l:{0}, r:{1}, c:{2}", zoom, row, col));

                // Check to see if the tile exists on disk...
                //  <cache-root>\<layerId>\<zoom>\<row>\<col>.esrijson; 
                //i.e. to be consistent with Esri tile caching structure
                string tilePath = string.Format(@"{0}\{1}\{2}\{3}\{4}.{5}", 
                    _vectorCacheRootDirectory, layerIndex, zoom.Value, 
                    row.Value, col.Value, jsonFormat.ToString().ToLower());
                if (File.Exists(tilePath))
                {
                    // Fetch tile contents from disk
                    _dtsLogger.LogInfo(soe_name, methodName, "Time: " + timer.Elapsed.ToString());
                    logger.LogMessage(ServerLogger.msgType.infoSimple, methodName, -1, "Time: " + timer.Elapsed.ToString());
                    return this.ReadTileFile(tilePath);
                }
                else
                {
                    // Write new files to disk

                    IMapServer3 mapServer = serverObjectHelper.ServerObject as IMapServer3;
                    if (mapServer == null)
                    {
                        throw new InvalidOperationException("Unable to access the map server.");
                    }

                    // Get the bbox. Returns an envelope in WGS84 (4326).
                    IEnvelope env102100 = TileUtil.GetEnvelopeFromZoomRowCol((int)zoom.Value, (int)row.Value, (int)col.Value);
                    //_dtsLogger.LogInfo(soe_name, methodName, this.GeometryToXml(env4326));

                   
                    // Convert envelope to polygon b/c QueryData does not support spatialfilter geometry using envelope 
                    IPolygon polygon102100 = this.CreatePolygonFromEnvelope(env102100);

                    // Use QueryData and generalize result geometries based on zoom level
                    IQueryResultOptions resultOptions = new QueryResultOptionsClass();
                    // i.e; IRecordSet to support BOTH json and geojson
                    resultOptions.Format = esriQueryResultFormat.esriQueryResultRecordSetAsObject; 
                    IMapTableDescription tableDescription = 
                        this.GetTableDescription(mapServer, (int)layerIndex, (int)zoom);

                    // Create spatial filter
                    ISpatialFilter spatialFilter = new SpatialFilterClass();
                    spatialFilter.Geometry = polygon102100;
                    spatialFilter.GeometryField = "Shape";
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    //TODO:  Subfields should be configurable
                    spatialFilter.SubFields = "*";

                    // Execute query
                    IQueryResult result = mapServer.QueryData(mapServer.DefaultMapName, 
                                        tableDescription, spatialFilter, resultOptions);

                    byte[] json = null;

                    // TODO:  Support writing tiles for no data
                    // Need a way to do this for GeoJson; parse Esri JSON recordset into GeoJson
                    if (result == null || !this.RecordsetHasFeatures(result.Object as IRecordSet))
                    {
                        // Write "no data" tile
                        if (jsonFormat == TileFormat.EsriJson)
                        {
                            resultOptions.Format = esriQueryResultFormat.esriQueryResultJsonAsMime;
                            result = mapServer.QueryData(mapServer.DefaultMapName, tableDescription, spatialFilter, resultOptions);
                            json = result.MimeData;
                        }
                        else  
                        {
                            json = StrToByteArray(NO_DATA_GEOJSON);
                        }
                    }
                    else
                    {
                        //We have features...
                        IRecordSet features = result.Object as IRecordSet;
                        
                        // Get geometry type for query layer
                        esriGeometryType geometryType = this.GetLayerGeometryType(mapServer.GetServerInfo(mapServer.DefaultMapName).MapLayerInfos, (int)layerIndex);
                        switch (geometryType)
                        {
                            case esriGeometryType.esriGeometryPoint:
                                // Do nothing... already intersected
                                json = ESRI.ArcGIS.SOESupport.Conversion.ToJson(features);
                                break;

                            case esriGeometryType.esriGeometryPolyline:
                                // Polylines must be clipped to envelope
                                IFeatureCursor cursor = null;
                                this.ClipFeaturesToTile(ref cursor, ref features, env102100);
                                json = ESRI.ArcGIS.SOESupport.Conversion.ToJson(features);
                                this.ReleaseComObject(cursor);
                                break;

                            case esriGeometryType.esriGeometryMultipoint:
                            case esriGeometryType.esriGeometryPolygon:
                                // Get IDs of features whose centroid is contained by tile envelope
                                List<int> ids = this.GetIdsOfContainedFeatureCentroids(features, polygon102100);
                                if (ids.Count == 0)
                                {
                                    // Write no data tile
                                    if (jsonFormat == TileFormat.EsriJson)
                                    {
                                        // Query to get empty featureset and serialize to disk
                                        resultOptions.Format = esriQueryResultFormat.esriQueryResultJsonAsMime;
                                        IQueryFilter queryFilter = new QueryFilterClass();
                                        queryFilter.SubFields = "*";
                                        queryFilter.WhereClause = "1=2";
                                        result = mapServer.QueryData(mapServer.DefaultMapName, tableDescription, queryFilter, resultOptions);
                                        json = result.MimeData;
                                    }
                                    else
                                    {
                                        json = StrToByteArray(NO_DATA_GEOJSON);
                                    }
                                }
                                else
                                {
                                    // Execute new query for IDs
                                    IQueryFilter queryFilter = new QueryFilterClass();
                                    queryFilter.SubFields = "*";
                                    // TODO:  Account for sql query syntax based on datasource
                                    //      FGDB/Shapefile then "OBJECTID"; quotes required
                                    //      PGDB then [OBJECTID]; brackets required
                                    //      SDE then OBJECTID; nothing but the fieldname
                                    queryFilter.WhereClause = string.Format("\"OBJECTID\" IN({0})", ids.ToDelimitedString<int>(",")); // FGDB
                                    result = mapServer.QueryData(mapServer.DefaultMapName, tableDescription, queryFilter, resultOptions);
                                    features = result.Object as IRecordSet;
                                    // Do some checking here...
                                    var featureCount = this.GetRecordsetFeatureCount(features);
                                    if (featureCount != ids.Count)
                                    {
                                        System.Diagnostics.Debug.WriteLine(string.Format("Query Problem:  ID search results IS NOT EQUAL to contained IDs count - [{0}:{1}]", featureCount, ids.Count));
                                        System.Diagnostics.Debug.WriteLine("Query:  " + queryFilter.WhereClause);
                                    }
                                    json = ESRI.ArcGIS.SOESupport.Conversion.ToJson(features);
                                }

                                break;
                            
                           
                            default:
                                throw new NotSupportedException(string.Format("Geometry type {0} is not supported by {1}", geometryType.ToString(), soe_name));
                        }
                    }


                    //store the json to disk
                    this.WriteTileFile(json, tilePath, (int)layerIndex, (int)zoom, (int)row);
                    _dtsLogger.LogInfo(soe_name, methodName,"Time: " + timer.Elapsed.ToString());
                    logger.LogMessage(ServerLogger.msgType.infoSimple,methodName,-1,"Time: " + timer.Elapsed.ToString());
                    return json;
                }
            }
            catch (Exception ex)
            {
                // Log the error 
                _dtsLogger.LogError(soe_name, methodName, "n/a", ex);
                logger.LogMessage(ServerLogger.msgType.error, methodName, 9999, ex.StackTrace);                
                return StrToByteArray("{}");
            }
        }

        /// <summary>
        /// Clip features in the recordset to the tile envelope
        /// </summary>
        /// <param name="featureCursor"></param>
        /// <param name="recordset"></param>
        /// <param name="tileEnvelope"></param>
        private void ClipFeaturesToTile(ref IFeatureCursor featureCursor, ref IRecordSet recordset, IEnvelope tileEnvelope)
        {
            //IFeatureCursor cursor = recordset.get_Cursor(true) as IFeatureCursor;
            featureCursor = recordset.get_Cursor(false) as IFeatureCursor;
            IFeature feature = featureCursor.NextFeature();                       
            ITopologicalOperator topologicalOperator = null;
            while (feature != null)
            {
                topologicalOperator = feature.Shape as ITopologicalOperator; // modifying shape so don't use ShapeCopy
                // TODO: verify clip geometry is retained in recordset ********************************
                topologicalOperator.Clip(tileEnvelope);
                feature = featureCursor.NextFeature();
            }
            
        }

        private List<int> GetIdsOfContainedFeatureCentroids(IRecordSet recordset, IPolygon tilePolygon)
        {
            List<int> ids = new List<int>();
            IFeatureCursor cursor = recordset.get_Cursor(true) as IFeatureCursor;
            IFeature feature = cursor.NextFeature();
            if (feature == null)
            {
                return ids;
            }

            if (feature.ShapeCopy.GeometryType != esriGeometryType.esriGeometryMultipoint && feature.ShapeCopy.GeometryType != esriGeometryType.esriGeometryPolygon)
            {
                throw new ArgumentException("Only multipoint and polygon geometry types are supported by this method.");
            }


            this.tracePolygon(tilePolygon);

            IRelationalOperator2 relationalOperator = tilePolygon as IRelationalOperator2;

            try
            {
                // Envelope must contain centriod
                IPoint centroid = null;
                while (feature != null)
                {
                    centroid = (feature.ShapeCopy as IArea).Centroid;
                    if (relationalOperator.Contains(centroid))
                    {
                        this.tracePolygonContainsPoint(tilePolygon, centroid, feature.OID);
                        ids.Add(feature.OID);
                    }
                    feature = cursor.NextFeature();
                }
                if (ids.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("OIDs with centroids contained:  " + ids.ToDelimitedString<int>(","));
                }
                return ids;
            }
            finally
            {
                this.ReleaseComObject(cursor);
                cursor = null;
            }
        }



        private esriGeometryType GetLayerGeometryType(IMapLayerInfos layerInfos, int layerIndex)
        {
            IMapLayerInfo3 layerInfo;

            // Find the index of the layer of interest.
            int layerCount = layerInfos.Count;
            for (int i = 0; i < layerCount; i++)
            {
                layerInfo = layerInfos.get_Element(i) as IMapLayerInfo3;
                if (layerInfo.ID == layerIndex)
                {
                    return this.GetGeometryTypeFromFields(layerInfo.Fields);
                }
            }

            throw new InvalidOperationException(string.Format("Unable to locate layerId [{0}] in map service. Please check the map service REST API to ensure this layer index exists.", layerIndex));
        }

        private esriGeometryType GetGeometryTypeFromFields(IFields fields)
        {
            IField field = null;
            for (int i = 0; i < fields.FieldCount; i++)
            {
                field = fields.get_Field(i);
                if (field.Type == esriFieldType.esriFieldTypeGeometry)
                {
                    return field.GeometryDef.GeometryType;
                }
            }

            throw new InvalidOperationException("Unable to locate geometry field for requested layer index.");
        }


        /// <summary>
        /// Create a table description, which includes generalization options
        /// </summary>
        /// <param name="mapServer"></param>
        /// <param name="layerId"></param>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        private IMapTableDescription GetTableDescription(IMapServer3 mapServer, int layerId, int zoomLevel)
        {
            ILayerDescriptions layerDescs = 
                mapServer.GetServerInfo(mapServer.DefaultMapName).DefaultMapDescription.LayerDescriptions;
            int count = layerDescs.Count;

            for (int i = 0; i < count; i++)
            {
                ILayerDescription3 layerDesc = (ILayerDescription3)layerDescs.get_Element(i);

                if (layerDesc.ID == layerId)
                {
                    layerDesc.LayerResultOptions = new LayerResultOptionsClass();
                    layerDesc.LayerResultOptions.GeometryResultOptions = new GeometryResultOptionsClass();
                    // Generalize geometries based on zoom level
                    layerDesc.LayerResultOptions.GeometryResultOptions.GeneralizeGeometries = true;
                    layerDesc.LayerResultOptions.GeometryResultOptions.MaximumAllowableOffset = 2000 / (zoomLevel + 1); 

                    return (IMapTableDescription)layerDesc;
                }
            }

            throw new ArgumentOutOfRangeException("layerID");
        }


        /// <summary>
        /// Create a polygon from a set of
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public IPolygon CreatePolygonFromEnvelope(IEnvelope envelope)
        {
            // Build a polygon from a sequence of points. 
            IGeometryBridge2 geometryBridge2 = new GeometryEnvironmentClass();
            IPointCollection4 pointCollection4 = new PolygonClass();

            // Define the point collection; i.e. polygon verticies
            List<WKSPoint> wksPoints = new List<WKSPoint>();
            wksPoints.Add(new WKSPoint() { X = envelope.LowerLeft.X, Y = envelope.LowerLeft.Y });
            wksPoints.Add(new WKSPoint() { X = envelope.UpperLeft.X, Y = envelope.UpperLeft.Y });
            wksPoints.Add(new WKSPoint() { X = envelope.UpperRight.X, Y = envelope.UpperRight.Y });
            wksPoints.Add(new WKSPoint() { X = envelope.LowerRight.X, Y = envelope.LowerRight.Y });

            geometryBridge2.SetWKSPoints(pointCollection4, wksPoints.ToArray());

            // Cast point collection to IPolygon
            IPolygon polygon = pointCollection4 as IPolygon;
            polygon.SpatialReference = envelope.SpatialReference;
            polygon.SimplifyPreserveFromTo();
            return polygon;
        }

        private IPoint GeographicToWebMercatorPoint(IPoint geographicPoint)
        {
            Point tempPoint = WebMercatorUtil.GeographicToWebMercator(geographicPoint.Y, geographicPoint.X);
            IPoint point = new PointClass();
            point.PutCoords(tempPoint.X, tempPoint.Y);
            point.SpatialReference = _spatialReference;
            return point;
        }

        private JsonObject GetErrorResponse(string message)
        {
            var error = new JsonObject();
            error.AddLong("code", 0);
            error.AddString("message", message);
            var response = new JsonObject();
            response.AddJsonObject("error", error);
            return response;
        }


        private byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        private byte[] ReadTileFile(string tilePath)
        {
            //read content from the file
            using (System.IO.FileStream textFile = new System.IO.FileStream(tilePath, FileMode.Open))
            {
                byte[] buffer = new byte[textFile.Length];
                textFile.Read(buffer, 0, buffer.Length);
                textFile.Close();
                return buffer;
            }
        }

        private void WriteTileFile(byte[] jsonContent, string tilePath, int layerId, int zoomLevel, int row)
        {
            // Create the directory path if it doesn't already exist
            this.CreateTilePath(layerId, zoomLevel, row);

            // Write to file
            using (System.IO.FileStream fs = new System.IO.FileStream(tilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(jsonContent);
                bw.Close();
            }
        }


        /// <summary>
        /// Build out the path to store the tiles following Esri tile caching structure
        /// </summary>
        /// <param name="layerId"></param>
        /// <param name="zoomLevel"></param>
        /// <param name="row"></param>
        private void CreateTilePath(int layerId, int zoomLevel, int row)
        {
            var subfolders = string.Format(@"{0}\{1}\{2}", layerId, zoomLevel, row);
            var tileFolderPath = System.IO.Path.Combine(_vectorCacheRootDirectory, subfolders);
            if (!Directory.Exists(tileFolderPath))
            {
                Directory.CreateDirectory(tileFolderPath);
            }
        }

        /// <summary>
        /// Creates the name of the map service cache folder that matches the naming 
        /// convention used by AGS as seen in the arcgisserver/arcgisoutput directory
        /// </summary>
        /// <returns></returns>
        private string CreateMapServiceCacheFolderName()
        {
            // Match folder 
            var folderName = string.Empty;
            var configName = serverObjectHelper.ServerObject.ConfigurationName;

            // Account for map service being published in a folder
            var isInFolder = configName.Contains("/");
            if (isInFolder)
            {
                folderName += configName.Replace("/", "_");
            }

            return folderName + "_MapServer";
        }

        private void ValidateMapServiceSpatialReference()
        {
            // Currently only supporting map services published in 102100
            IMapServer3 mapServer = serverObjectHelper.ServerObject as IMapServer3;
            if (mapServer == null)
                throw new Exception("Unable to access the map server.");

            IMapServerInfo mapServerInfo = mapServer.GetServerInfo(mapServer.DefaultMapName);
            _spatialReference = mapServerInfo.DefaultMapDescription.SpatialReference;
            var wkid = _spatialReference.FactoryCode;
            if (wkid != 102100 && wkid != 3857 && wkid != 102113)   // All interchangeable, though 102100 should supercede all
            {
                throw new NotSupportedException(string.Format("WKID {0} is not currently supported. Map service must be in Web Mercator Aux. Sphere (WKID: 102100, 102113, or 3857).", wkid));
            }
        }

        private ISpatialReference GetSpatialReferenceFromMapService(IMapServer3 mapServer)
        {
            IMapServerInfo mapServerInfo = mapServer.GetServerInfo(mapServer.DefaultMapName);
            return mapServerInfo.DefaultMapDescription.SpatialReference;
        }

        private esriUnits GetMapServiceUnits(IMapServer3 mapServer)
        {
            IMapServerInfo mapServerInfo = mapServer.GetServerInfo(mapServer.DefaultMapName);
            return mapServerInfo.MapUnits;
        }

        private string GeometryToXml(IGeometry geometry)
        {
            System.String elementURI = "http://www.esri.com/schemas/ArcGIS/9.2";

            // Create xml writer
            ESRI.ArcGIS.esriSystem.IXMLWriter xmlWriter = new ESRI.ArcGIS.esriSystem.XMLWriterClass();

            // Create xml stream
            ESRI.ArcGIS.esriSystem.IXMLStream xmlStream = new ESRI.ArcGIS.esriSystem.XMLStreamClass();

            // Explicit Cast for IStream and then write to stream 
            xmlWriter.WriteTo((ESRI.ArcGIS.esriSystem.IStream)xmlStream);

            // Serialize 
            ESRI.ArcGIS.esriSystem.IXMLSerializer xmlSerializer = new ESRI.ArcGIS.esriSystem.XMLSerializerClass();
            xmlSerializer.WriteObject(xmlWriter, null, null, "geometry", elementURI, geometry);

            return xmlStream.SaveToString();
        }

        private string SpatialReferenceToXml(ISpatialReference sr)
        {
            System.String elementURI = "http://www.esri.com/schemas/ArcGIS/9.2";

            // Create xml writer
            ESRI.ArcGIS.esriSystem.IXMLWriter xmlWriter = new ESRI.ArcGIS.esriSystem.XMLWriterClass();

            // Create xml stream
            ESRI.ArcGIS.esriSystem.IXMLStream xmlStream = new ESRI.ArcGIS.esriSystem.XMLStreamClass();

            // Explicit Cast for IStream and then write to stream 
            xmlWriter.WriteTo((ESRI.ArcGIS.esriSystem.IStream)xmlStream);

            // Serialize 
            ESRI.ArcGIS.esriSystem.IXMLSerializer xmlSerializer = new ESRI.ArcGIS.esriSystem.XMLSerializerClass();
            xmlSerializer.WriteObject(xmlWriter, null, null, "spatialreference", elementURI, sr);

            return xmlStream.SaveToString();
        }

        public static IGeoTransformation GetGeographicToWebMercatorTransformation()
        {
            ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            // esriSRGeoTransformation_NAD1983_To_WGS1984_5	1515	United States.
            return spatialReferenceFactory.CreateGeoTransformation((int)esriSRGeoTransformationType.esriSRGeoTransformation_NAD1983_To_WGS1984_5) as IGeoTransformation;
        }

        /// <summary>
        /// Releases the COM object.
        /// </summary>
        /// <param name="o">The COM object to be released.</param>
        public void ReleaseComObject(object comObject)
        {
            if ((comObject != null) && Marshal.IsComObject(comObject))
            {
                while (Marshal.ReleaseComObject(comObject) > 0) { }
            }
        }

        private bool RecordsetHasFeatures(IRecordSet recordset)
        {
            if (recordset == null) { return false; }

            ICursor cursor = recordset.get_Cursor(true);
            try
            {
                return (cursor.NextRow() != null);
            }
            finally
            {
                this.ReleaseComObject(cursor);
            }
        }

        private void tracePolygon(IPolygon polygon)
        {
            // Check the polygon IS the envelope
            if (!(polygon as IPolygon5).IsEnvelope)
            {
                System.Diagnostics.Debug.WriteLine("polygon IS NOT EQUAL to its envelope");
            }
            else
            {
                IEnvelope e = polygon.Envelope;
                var coords = string.Format("XMin: {0}, YMin: {1}, XMax: {2}, YMax: {3}", e.XMin, e.YMin, e.XMax, e.YMax);
                System.Diagnostics.Debug.WriteLine(coords);
            }



            // var sb = new StringBuilder();
            //IPointCollection points = polygon as IPointCollection;
            //IEnumVertex2 verticies = points.EnumVertices as IEnumVertex2;
            //verticies.Reset();

            ////Get the next vertex.
            //IPoint outVertex;
            //int partIndex;
            //int vertexIndex;

            //System.Diagnostics.Debug.WriteLine("----------------------------------");

            //verticies.Next(out outVertex, out partIndex, out vertexIndex);
            //while (outVertex != null)
            //{
            //    if (sb.Length > 0) { sb.Append(" | "); }
            //    sb.AppendFormat("X: {0}, Y: {1}", outVertex.X, outVertex.Y);
            //    verticies.Next(out outVertex, out partIndex, out vertexIndex);
            //}

            //System.Diagnostics.Debug.WriteLine(sb.ToString());

        }

        private void tracePolygonContainsPoint(IPolygon polygon, IPoint point, int objectId = -1)
        {
            // NOTE: Assumes 
            //      (polygon as IPolygon5).IsEnvelope == true

            var sb = new StringBuilder();
            IEnvelope e = polygon.Envelope;

            if (!(point.X >= e.XMin && point.X <= e.XMax))
            {
                sb.Append("Point X does not intersect tile");
            }
            if (!(point.Y >= e.YMin && point.Y <= e.YMax))
            {
                if (sb.Length > 0) { sb.Append(" AND "); }
                sb.Append("Point Y does not intersect tile");
            }

            if (sb.Length > 0)
            {
                if (objectId > -1) { sb.Insert(0, string.Format("{0}:  ", objectId)); }
                System.Diagnostics.Debug.WriteLine(sb.ToString());
            }
        }

        private int GetRecordsetFeatureCount(IRecordSet recordset)
        {
            if (recordset == null) { return 0; }

            ICursor cursor = recordset.get_Cursor(true);
            try
            {
                int i = 0;
                while (cursor.NextRow() != null)
                {
                    i++;
                }
                return i;
            }
            finally
            {
                this.ReleaseComObject(cursor);
            }
        }
    }
}
