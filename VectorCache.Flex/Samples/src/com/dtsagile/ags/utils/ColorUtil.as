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
package com.dtsagile.ags.utils
{
	/**
	 * Utility class for working with colors.
	 *  
	 * @author jgermain
	 * 
	 */
	public class ColorUtil
	{	
		/**
		 * Extracts the RGB values into an array of numbers; each 0-255.
		 * 
		 * @param color The color to be 
		 * @return Returns an array of RGB values
		 * 
		 */
		public static function toRgbArray(color:uint):Array
		{
			var r:Number = (color >> 16) & 0xFF;
			var g:Number = (color >> 8) & 0xFF;
			var b:Number = color & 0xFF;
			
			return [r,g,b];
		}
		
		public static function getRandomColor():uint
		{
			return 0x000000 + Math.random() * 0xFFFFFF;
		}
		
		/**
		 * Blends smoothly from one color value to another.
		 * This method written by Adobe as part of fl.motion.Color
		 * http://help.adobe.com/en_US/FlashPlatform/reference/actionscript/3/fl/motion/Color.html#interpolateColor()
		 * 
		 * @param fromColor The starting color value, in the 0xRRGGBB or 0xAARRGGBB format.
		 * @param toColor The ending color value, in the 0xRRGGBB or 0xAARRGGBB format.
		 * @param progress The percent of the transition as a decimal, where 0 is the start and 1 is the end.
		 * @return The interpolated color value, in the 0xRRGGBB or 0xAARRGGBB format.
		 * 
		 */
		public static function interpolateColor(fromColor:uint, toColor:uint, progress:Number):uint
		{
			var q:Number = 1-progress;
			var fromA:uint = (fromColor >> 24) & 0xFF;
			var fromR:uint = (fromColor >> 16) & 0xFF;
			var fromG:uint = (fromColor >>  8) & 0xFF;
			var fromB:uint =  fromColor        & 0xFF;
			
			var toA:uint = (toColor >> 24) & 0xFF;
			var toR:uint = (toColor >> 16) & 0xFF;
			var toG:uint = (toColor >>  8) & 0xFF;
			var toB:uint =  toColor        & 0xFF;
			
			var resultA:uint = fromA*q + toA*progress;
			var resultR:uint = fromR*q + toR*progress;
			var resultG:uint = fromG*q + toG*progress;
			var resultB:uint = fromB*q + toB*progress;
			var resultColor:uint = resultA << 24 | resultR << 16 | resultG << 8 | resultB;
			return resultColor;  
		}
		
		
		/**
		 * Returns an array of colors between and including Hex1 and Hex2.
		 * This method was adapted from code written by PiXELWiT and obtained from
		 * http://www.pixelwit.com/blog/2008/05/color-fading-array/
		 * 
		 * @param fromColor The starting color value, in the 0xRRGGBB
		 * @param toColor The ending color value, in the 0xRRGGBB
		 * @param steps The number of steps in output color ramp
		 * @return Array containing ramped colors including and between the from and 
		 * to colors.
		 * 
		 */
		public static function createColorRamp (fromColor:uint, toColor:uint, steps:Number):Array
		{
			//
			// Create an array to store all colors.
			var rampColors:Array = [fromColor];
			//
			// Break fromColor into RGB components.
			var r:uint = fromColor >> 16;
			var g:uint = fromColor >> 8 & 0xFF;
			var b:uint = fromColor & 0xFF;
			//
			// Determine RGB differences between fromColor and toColor.
			var rd:uint = (toColor >> 16)-r;
			var gd:uint = (toColor >> 8 & 0xFF)-g;
			var bd:uint = (toColor & 0xFF)-b;
			//
			steps++;
			// For each new color.
			for (var i:int=1; i<steps; i++){
				//
				// Determine where the color lies between the 2 end colors.
				var ratio:Number = i/steps;
				//
				// Calculate new color and add it to the array.
				rampColors.push((r+rd*ratio)<<16 | (g+gd*ratio)<<8 | (b+bd*ratio));
			}
			//
			// Add toColor to the array and return it.
			rampColors.push(toColor);
			return rampColors;
		}
	}
}