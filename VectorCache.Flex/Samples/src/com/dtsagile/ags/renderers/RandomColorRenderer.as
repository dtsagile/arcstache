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
package com.dtsagile.ags.renderers
{
	import com.dtsagile.ags.utils.ColorUtil;
	import com.esri.ags.Graphic;
	import com.esri.ags.geometry.Geometry;
	import com.esri.ags.renderers.SimpleRenderer;
	import com.esri.ags.renderers.UniqueValueRenderer;
	import com.esri.ags.renderers.supportClasses.UniqueValueInfo;
	import com.esri.ags.symbols.SimpleFillSymbol;
	import com.esri.ags.symbols.SimpleLineSymbol;
	import com.esri.ags.symbols.SimpleMarkerSymbol;
	import com.esri.ags.symbols.Symbol;
	import com.esri.utils.Hashtable;
	
	import mx.utils.UIDUtil;
	
	public class RandomColorRenderer extends SimpleRenderer
	{
		private var _symbols:Hashtable = new Hashtable();
		
		public var attribute:String;
		
		public function RandomColorRenderer(attribute:String=null, defaultSymbol:Symbol=null)
		{
			super(symbol);
			this.attribute = attribute;
		}
		
		public override function getSymbol(graphic:Graphic):Symbol
		{
			// Keeping it simple...
			// 	If attribute supplied, then we roll a "unique value renderer",
			// 	Otherwise, each graphic gets a random color
			
			var symbol:Symbol = null;
			
			// Cache our symbols so when layer props are invalidated, we don't apply new colors!
			var symbolKey:String = null;
			if(this.attribute && graphic.attributes.hasOwnProperty(this.attribute))
			{
				// Unique value renderer
				symbolKey = graphic.attributes[this.attribute];
			}
			else
			{
				// Simple renderer
				symbolKey = UIDUtil.getUID(graphic);
			}
			
			
			if(_symbols.find(symbolKey))
			{
				symbol = _symbols.find(symbolKey);
			}
			else
			{
				symbol = this.getSymbolByGeometryType(graphic.geometry.type);
				_symbols.add(symbolKey, symbol);
			}			
			
			return symbol;
		}
		
		private function getSymbolByGeometryType(geometryType:String):Symbol
		{	
			var symbol:Symbol = null;
			var randomColor:uint = ColorUtil.getRandomColor();
			
			switch(geometryType)
			{
				case Geometry.MAPPOINT:
				case Geometry.MULTIPOINT:
					symbol = new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_CIRCLE, 15, randomColor, 1, 0, 0, 0, 
								new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, 0x000000, 0.8, 2));
					break;
				case Geometry.POLYGON:
					symbol = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, randomColor, 0.5, 
								new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, 0xFFFFFF, 0.8, 2));
					break;
				case Geometry.POLYLINE:
					symbol = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, randomColor, 0.8, 2);
					break;
				default:
					throw new Error('Geometry type ' + geometryType + ' is not supported.');
			}
			
			return symbol;
		}
	}
}