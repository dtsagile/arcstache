//////////////////////////////////////////////////////////////////////////////////////
//
//    Copyright (c) 2011 DTSAgile
//    
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//    The above copyright notice and this permission notice shall be included in
//    all copies or substantial portions of the Software.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//    THE SOFTWARE.
//
//////////////////////////////////////////////////////////////////////////////////////
package com.dtsagile.ags.layers
{	
	import com.adobe.utils.DictionaryUtil;
	import com.adobe.utils.StringUtil;
	import com.esri.ags.FeatureSet;
	import com.esri.ags.Graphic;
	import com.esri.ags.SpatialReference;
	import com.esri.ags.clusterers.IClusterer;
	import com.esri.ags.events.ExtentEvent;
	import com.esri.ags.events.LayerEvent;
	import com.esri.ags.geometry.*;
	import com.esri.ags.layers.*;
	import com.esri.ags.layers.supportClasses.*;
	import com.esri.ags.renderers.Renderer;
	import com.esri.ags.symbols.Symbol;
	import com.esri.utils.Hashtable;
	
	import flash.net.URLRequest;
	import flash.utils.Dictionary;
	import flash.utils.describeType;
	
	import mx.collections.ArrayCollection;
	import mx.collections.ArrayList;
	import mx.controls.Alert;
	import mx.rpc.Fault;
	import mx.rpc.events.FaultEvent;
	import mx.rpc.events.ResultEvent;
	import mx.rpc.http.HTTPService;
	import mx.utils.ObjectUtil;
	
	/**
	 * VectorTiledMapServiceLayer2
	 */
	public class VectorTiledMapServiceLayer extends TiledMapServiceLayer
	{
		//--------------------------------------------------------------------------
		//
		//  Constructor
		//
		//--------------------------------------------------------------------------
		
		/**
		 * Creates a new VectorTiledMapServiceLayer2 object.
		 */
		public function VectorTiledMapServiceLayer(url:String = null, layerId:Number = 0)
		{
			_fullExtent = new com.esri.ags.geometry.Extent(-20037508.3428, -20037508.3428, 20037508.3428, 20037508.3428, new com.esri.ags.SpatialReference(102100));
			_initialExtent = new com.esri.ags.geometry.Extent(-20037508.3428, -20037508.3428, 20037508.3428, 20037508.3428, new com.esri.ags.SpatialReference(102100));
			_spatialReference = new com.esri.ags.SpatialReference(102100);
			_tileInfo = new com.esri.ags.layers.supportClasses.TileInfo();
			
			super();
			
			buildTileInfo(); // to create our hardcoded tileInfo
			
			this.initializeGraphicsLayer();
			this.layerId = layerId;
			this.url = url;
			
			setLoaded(true); // Map will only use loaded layers
		}
		
		//--------------------------------------------------------------------------
		//
		//  Local Variables
		//
		//--------------------------------------------------------------------------
		
		// Property backings
		private var _url:String;
		private var _layerId:Number;
		private var _fullExtent:Extent;
		private var _initialExtent:Extent;
		private var _spatialReference:SpatialReference;
		private var _tileInfo:TileInfo = new TileInfo; // see buildTileInfo()
		private var _labelField:String = null;
		private var _labelFunction:Function; 
		private var _clusterer:IClusterer;
		private var _symbol:Symbol;
		private var _renderer:Renderer;	
		
		// Others
		private var _graphicsLayer:GraphicsLayer;
		private var _requestingTileUrl:URLRequest;
		private var _loadedTiles:Hashtable = new Hashtable();
		private var _zoomLevel:Number = NaN;
		
		//--------------------------------------------------------------------------
		//
		//  Public Properties
		//		layerId
		// 		url
		//		labelField
		//		labelFunction
		//		clusterer
		//		symbol
		//		renderer
		//--------------------------------------------------------------------------

		
		//----------------------------------
		//  layerId
		//----------------------------------
		
		/**
		 * The index of the layer in the map service this layer represents.
		 * The layer index may be obtained through viewing the map service's
		 * REST endpoint.
		 * @return 
		 * 
		 */
		public function get layerId():Number
		{
			//trace("layerId");
			return _layerId;
		}
		
		public function set layerId(value:Number):void
		{
			_layerId = value;
			this.invalidateProperties();
			return;
		}
		
		//----------------------------------
		//  url
		//----------------------------------
				
		/**
		 * The URL of the map service. For example:
		 * <code>http://myserver.com/ArcGIS/rest/services/myfolder/myservice/MapServer/</code>
		 * @return 
		 * 
		 */
		public function get url():String
		{
			//trace("url");
			return _url;
			//return "http://localhost/ArcGIS/rest/services/vectorcache/dev/MapServer/";
		}
		
		public function set url(value:String):void
		{
			// Append trailing "/"
			if(value && !StringUtil.endsWith(value, "/"))
			{
				value+="/";
			}
			
			if(value && value != this.url)
			{
				_url = value;
				this.invalidateProperties();
			}
		}
		
		//----------------------------------
		//  labelField
		//----------------------------------
		
		/**
		 *  The name of the field in the feature layer to display 
		 *  as the label. 
		 *  The <code>labelFunction</code> property overrides this property.
		 *
		 *  @default null
		 */
		public function get labelField():String
		{
			return _labelField;
		}
		
		/**
		 *  @private
		 */
		public function set labelField(value:String):void
		{
			if (value == _labelField) { return; }
			_labelField = value;
			invalidateGraphicsLayerGraphicsProperties();
		}
		
		//----------------------------------
		//  labelFunction
		//----------------------------------
	
		/**
		 *  A user-supplied function to run on each item to determine its label.  
		 *  The <code>labelFunction</code> property overrides 
		 *  the <code>labelField</code> property.
		 *
		 *  <p>You can supply a <code>labelFunction</code> that finds the 
		 *  appropriate fields and returns a displayable string. The 
		 *  <code>labelFunction</code> is also good for handling formatting and 
		 *  localization. </p>
		 *
		 *  <p>The label function takes a single argument which is the item in 
		 *  the data provider and returns a String.</p>
		 *  <pre>
		 *  myLabelFunction(item:Object):String</pre>
		 *
		 *  @default null
		 */
		public function get labelFunction():Function
		{
			return _labelFunction;
		}
		
		public function set labelFunction(value:Function):void
		{
			if (value == _labelFunction) { return; }
			_labelFunction = value;
			invalidateGraphicsLayerGraphicsProperties();
		}
		
		//----------------------------------
		//  clusterer
		//----------------------------------
			
		/**
		 * The graphics clusterer. 
		 * 
		 *  @default null
		 */
		public function get clusterer():com.esri.ags.clusterers.IClusterer
		{
			return _clusterer;
		}
		
		public function set clusterer(value:IClusterer):void
		{
			if(_clusterer == value) { return; }
			_clusterer = value;
			if(_graphicsLayer)
			{
				_graphicsLayer.clusterer = _clusterer;
			}
		}
		
		//----------------------------------
		//  symbol
		//----------------------------------
				
		/**
		 * The default symbol for the layer.
		 * @return 
		 * 
		 */
		public function get symbol(): Symbol
		{
			return _symbol;
		}
		
		public function set symbol(value:Symbol):void
		{
			if(_symbol == value) { return; }
			_symbol = value;
			if(_graphicsLayer)
			{
				_graphicsLayer.symbol = _symbol;
			}
		}
		
		//----------------------------------
		//  renderer
		//----------------------------------
		
		/**
		 * The renderer allows for the symbol of the graphic to be dynamically picked - for example based on attributes or position of the graphic, or based on the scale of the map, or anything else.
		 * @return 
		 * 
		 */
		public function get renderer(): Renderer
		{
			return _renderer;
		}
		
		public function set renderer(value:Renderer):void
		{
			if(_renderer == value) { return; }
			_renderer = value;
			if(_graphicsLayer)
			{
				_graphicsLayer.renderer = _renderer;
			}
		}
		
	
	
		
		//--------------------------------------------------------------------------
		//
		//  Overridden properties
		//      fullExtent()
		//      initialExtent()
		//      spatialReference()
		//      tileInfo()
		//      units()
		//
		//--------------------------------------------------------------------------
		
		
		//----------------------------------
		//  fullExtent
		//  - required to calculate the tiles to use
		//----------------------------------
		
		override public function get fullExtent():Extent
		{
			//trace('fullExtent');
			return _fullExtent;
		}
		
		//----------------------------------
		//  initialExtent
		//  - needed if Map doesn't have an extent
		//----------------------------------
		
		override public function get initialExtent():Extent
		{
			//trace('initialExtent');
			return _initialExtent;
		}
		
		//----------------------------------
		//  spatialReference
		//  - needed if Map doesn't have a spatialReference
		//----------------------------------
		
		override public function get spatialReference():SpatialReference
		{
			//trace('spatialReference');
			return _spatialReference;
		}
		
		//----------------------------------
		//  tileInfo
		//----------------------------------
		
		override public function get tileInfo():TileInfo
		{
			//trace('tileInfo');
			return _tileInfo;
		}
		
		//----------------------------------
		//  units
		//  - needed if Map doesn't have it set
		//----------------------------------
		
		override public function get units():String
		{
			//trace("units");
			return com.esri.ags.Units.METERS;
		}
		
		//----------------------------------
		//  visible
		//  - needed so we can turn on/off our graphics layer
		//----------------------------------
		
		public override function get visible():Boolean
		{
			return super.visible;
		}
		
		public override function set visible(value:Boolean):void
		{
			// Update base tiled layer
			if(super.visible != value)
			{
				super.visible = value;
			}
			
			// Update graphics layer
			if(_graphicsLayer)
			{
				if(_graphicsLayer.visible != value)
				{
					_graphicsLayer.visible = value;
				}
			}
		}
		
		//--------------------------------------------------------------------------
		//
		//  Overridden methods
		//      getTileURL(level:Number, row:Number, col:Number):URLRequest
		//
		//--------------------------------------------------------------------------
		
		override protected function getTileURL(level:Number, row:Number, col:Number):URLRequest
		{
			//			trace('getTileURL');
			//			// Use virtual cache directory
			//			// This assumes the cache's virtual directory is exposed, which allows you access
			//			// to tile from the Web server via the public cache directory.
			//			// Example of a URL for a tile retrieved from the virtual cache directory:
			//			// http://serverx.esri.com/arcgiscache/dgaerials/Layers/_alllayers/L01/R0000051f/C000004e4.jpg
			//			var url:String = _baseURL
			//				+ "/L" + padString(String(level), 2, "0")
			//				+ "/R" + padString(row.toString(16), 8, "0")
			//				+ "/C" + padString(col.toString(16), 8, "0") + ".png";
			//			trace(url);
			//			return new URLRequest(url);
			
			
			// Execute our call to SOE for vector tiles
			this.updateGraphicsLayer(level, row, col);
			
			// TiledMapServiceLayer calls this method so we've created a method
			// on the SOE to hand back en empty JSON object; i.e. "{}"
			var tileUrl:String = this.url + "exts/arcstache/emptytile?" 
				+ "l=" + this.layerId
				+ "&z=" + level 
				+ "&x=" + col 
				+ "&y=" + row 
				+ "&jf=esrijson"//; 
				+ "&f=json";
			
			trace('getTileURL:  ' + tileUrl);
			
			return new URLRequest(tileUrl);
		}
		
		protected override function commitProperties():void
		{
			trace('commitProperties');
			super.commitProperties();
			if(_graphicsLayer)
			{
				_graphicsLayer.visible = this.visible;
			}
		}	
		
		protected override function extentChangeHandler(event:ExtentEvent):void
		{
			super.extentChangeHandler(event);
			var levelChangeVerified:Boolean = (event.lod.level != _zoomLevel);
			if(event.levelChange && levelChangeVerified)
			{
				_zoomLevel = event.lod.level;
				this.clearTileCache();
			}
		}
		
		//--------------------------------------------------------------------------
		//
		//  Private Methods
		//
		//--------------------------------------------------------------------------
		
		private function buildTileInfo():void
		{
			_tileInfo.dpi = 96;
			_tileInfo.height = 256;
			_tileInfo.width = 256;
			_tileInfo.origin = new com.esri.ags.geometry.MapPoint(-20037508.3428, 20037508.3428);
			_tileInfo.spatialReference = new com.esri.ags.SpatialReference(102100);
			_tileInfo.lods = this.getArcGisOnlineLODs();
		}
		
		private function getArcGisOnlineLODs():Array
		{
			return [
				new LOD(0, 156543.033928, 591657527.591555),
				new LOD(1, 78271.5169639999, 295828763.795777),
				new LOD(2, 39135.7584820001, 147914381.897889),
				new LOD(3, 19567.8792409999, 73957190.948944),
				new LOD(4, 9783.93962049996, 36978595.474472),
				new LOD(5, 4891.96981024998, 18489297.737236),
				new LOD(6, 2445.98490512499, 9244648.868618),
				new LOD(7, 1222.99245256249, 4622324.434309),
				new LOD(8, 611.49622628138, 2311162.217155),
				new LOD(9, 305.748113140558, 1155581.108577),
				new LOD(10, 152.874056570411, 577790.554289),
				new LOD(11, 76.4370282850732, 288895.277144),
				new LOD(12, 38.2185141425366, 144447.638572),
				new LOD(13, 19.1092570712683, 72223.819286),
				new LOD(14, 9.55462853563415, 36111.909643),
				new LOD(15, 4.77731426794937, 18055.954822),
				new LOD(16, 2.38865713397468, 9027.977411),
				new LOD(17, 1.19432856685505, 4513.988705)
			];
		}
		
		private function clearTileCache():void
		{
			// We should be clearing the local cache whenever the zoom level changes!
			trace('clearTileCache');
			if(_graphicsLayer && _graphicsLayer.numGraphics > 0)
			{
				_graphicsLayer.clear();
			}
			if(_loadedTiles)
			{
				_loadedTiles.clear();
			}
		}
		
		private function initializeGraphicsLayer():void
		{
			if(map)
			{
				_graphicsLayer = new GraphicsLayer();
				_graphicsLayer.id = this.id + " Vectors";
				_graphicsLayer.alpha = this.alpha;
				
				// Set visible based on the base tiled layer
				_graphicsLayer.visible = super.visible;
				
				if(this.clusterer)
				{
					_graphicsLayer.clusterer = this.clusterer;
				}
				
				if(this.renderer)
				{
					_graphicsLayer.renderer = this.renderer;
				}
				
				if(this.symbol)
				{
					_graphicsLayer.symbol = this.symbol;
				}
				
				this.map.addLayer(_graphicsLayer);
				
				_zoomLevel = this.map.level;
			}
		}
		
		private function updateGraphicsLayer(level:Number, row:Number, col:Number):void
		{
			//Subclasses should dispatch a LayerEvent.UPDATE_END event when they are finished.
			
			if(!_graphicsLayer) {initializeGraphicsLayer();}
			
			// Check if we already have this tile loaded
			var tileKey:String = row + "::" + col;
			if(_loadedTiles.find(tileKey)) {
				return;
			}
			
			var parameters:Object = {
				l: this.layerId,
					z: level,
					x: col,
					y: row,
					jf: 'esrijson',
					f: 'json'
			};
			
			var service:HTTPService = new HTTPService();
			service.url = this.url + "exts/arcstache/tile";
			service.resultFormat = "text";
			service.addEventListener(ResultEvent.RESULT, tileRequestResult);
			service.addEventListener(FaultEvent.FAULT, tileRequestFault);
			service.send(parameters);
			//service.send();
			
			function tileRequestResult(event:ResultEvent):void
			{
				var result:Object = event.result;
				var featureSet:FeatureSet = FeatureSet.convertFromJSON(result.toString());
				
				// Store the tile so we don't re-query for it later, including EMPTY tiles
				_loadedTiles.add(tileKey, featureSet);
				
				if(featureSet.features.length > 0)
				{
					trace("feature count: " + featureSet.features.length);
					
					var features:ArrayCollection = new ArrayCollection(featureSet.features);
					
					// Assign labels if either labelField or labelFunction are set
					updateGraphicsLayerLabels(features);
					
					// Add to graphics layer
					(_graphicsLayer.graphicProvider as ArrayCollection).addAll(features);
					dispatchLayerEvent(LayerEvent.UPDATE_END);
					return;
				}
				trace('no features returned');
			}
			function tileRequestFault(event:FaultEvent):void
			{
				Alert.show(event.fault.faultString + "\n\n" + event.fault.getStackTrace());
				dispatchLayerEvent(LayerEvent.LOAD_ERROR);
				return;
			}
		}
		
		private function dispatchLayerEvent(type:String):void
		{
			dispatchEvent(new LayerEvent(type, this));
		}
		
		private function invalidateGraphicsLayerGraphicsProperties():void
		{
			if(!_graphicsLayer || _graphicsLayer.numChildren < 1)
			{
				return;
			}
			
			var features:ArrayCollection = _graphicsLayer.graphicProvider as ArrayCollection; 
			this.updateGraphicsLayerLabels(features);
		}
		
		private function updateGraphicsLayerLabels(features:ArrayCollection):void
		{
			// Iterate features and either call label function, or apply label, or clear label
			var g:Graphic = null;
			if(this.labelFunction)
			{
				for each(g in features)
				{
					this.labelFunction(g);
				}
			}
			else if(StringUtil.stringHasValue(this.labelField))
			{
				// Verify field exists
				if((features.getItemAt(0) as Graphic).attributes.hasOwnProperty(this.labelField))
				{
					for each(g in features)
					{
						g.toolTip = g.attributes[this.labelField] || "";
					}
				}
			}
			else
			{
				// Remove tooltips
				for each(g in features)
				{
					g.toolTip = "";
				}
			}
			
			// Refresh
			//_graphicsLayer.refresh();
		}

	}
	
}