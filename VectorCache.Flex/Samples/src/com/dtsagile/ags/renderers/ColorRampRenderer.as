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
	
	public class ColorRampRenderer extends SimpleRenderer
	{
		private var _symbols:Hashtable = new Hashtable();
		private var _seeds:Array = new Array();
			
		private var _fromColor:uint = 0x00FFFF;
		
		private var _toColor:uint = 0x000099;
		
		public function ColorRampRenderer(attribute:String = null, fromColor:uint = NaN, toColor:uint = NaN, numSteps:Number = NaN, defaultSymbol:Symbol = null)
		{
			super(symbol);
			
			this.attribute = attribute;
			
			this.numSteps = numSteps;
			
			if(!isNaN(fromColor) && fromColor > 0)
			{
				this.fromColor = fromColor;
			}
			
			if(!isNaN(toColor) && toColor > 0)
			{
				this.toColor = toColor;
			}
		}
		
		//--------------------------------------------------------------------------
		//
		//  Public Properties
		//		attribute:String;
		//		fromColor:uint
		//      toColor:uint
		//
		//--------------------------------------------------------------------------
		
		public var attribute:String;
		public var numSteps:Number;
		
		public function get toColor():uint
		{
			return _toColor;
		}

		public function set toColor(value:uint):void
		{
			if(!isNaN(value))
			{
				_toColor = value;
			}
		}

		public function get fromColor():uint
		{
			return _fromColor;
		}

		public function set fromColor(value:uint):void
		{
			if(!isNaN(value))
			{
				_fromColor = value;
			}
		}

		//--------------------------------------------------------------------------
		//
		//  Overridden methods
		//      getSymbol(graphic:Graphic):Symbol
		//
		//--------------------------------------------------------------------------
		
		private var _colorRamp:Array = null;
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
				var rampColor:uint;
				if(this.numSteps > 0 && _symbols.size <= this.numSteps)
				{
					if(!_colorRamp)
					{
						_colorRamp = ColorUtil.createColorRamp(this.fromColor, this.toColor, this.numSteps);
					}
					rampColor = _colorRamp[_symbols.size];
				}
				else
				{
					var seed:Number = this.getSeed();				
					rampColor = ColorUtil.interpolateColor(this.fromColor, this.toColor, seed);
				}
				symbol = this.getSymbolByGeometryType(graphic.geometry.type, rampColor);
				_symbols.add(symbolKey, symbol);
				
			}			
			
			return symbol;
		}
		
		private function getSymbolByGeometryType(geometryType:String, color:uint):Symbol
		{	
			var symbol:Symbol = null;
			
			switch(geometryType)
			{
				case Geometry.MAPPOINT:
				case Geometry.MULTIPOINT:
					symbol = new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_CIRCLE, 15, color, 1, 0, 0, 0, 
						new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, 0x000000, 0.8, 2));
					break;
				case Geometry.POLYGON:
					symbol = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, color, 0.8, 
						new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, 0xFFFFFF, 0.8, 2));
					break;
				case Geometry.POLYLINE:
					symbol = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, color, 0.8, 1);
					break;
				default:
					throw new Error('Geometry type ' + geometryType + ' is not supported.');
			}
			
			return symbol;
		}
		
		
		private function randomNumber(low:Number=1, high:Number=100):Number
		{
			return this.roundDec(((Math.random() * (1+high-low)) + low)/100,3);
		}
		
		private function roundDec(numIn:Number, decimalPlaces:int):Number 
		{
			var nExp:int = Math.pow(10,decimalPlaces) ; 
			var nRetVal:Number = Math.round(numIn * nExp) / nExp
			return nRetVal;
		}
		
		private function getSeed():Number
		{
			// Generate a NEW seed
			var seed:Number;
			
			if(numSteps <= 0 || _seeds.length >= numSteps)
			{
				do
				{
					seed = this.randomNumber();
				} while (_seeds.indexOf(seed) > -1);
			}
			else
			{
				seed = (_symbols.size > 0) ? (_symbols.size / numSteps) : 0.0;
			}
			trace(seed);
			
			// Cache the seed
			_seeds.push(seed);
			
			return seed;
		}
	}
}