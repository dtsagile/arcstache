package com.dtsagile.ags
{
	import com.esri.ags.Map;
	
	import flash.events.MouseEvent;
	import flash.net.URLRequest;
	import flash.net.navigateToURL;
	
	import mx.controls.Alert;
	import mx.controls.Image;
	import mx.core.FlexGlobals;
	
	import spark.components.Label;
	import spark.filters.GlowFilter;
	
	/**
	 * Custom map containing DTSAgile logo next to ESRI's. 
	 * @author jgermain
	 * 
	 */
	public final class DTSMap extends Map
	{
		override public function DTSMap()
		{
			super();
		}
		
		override protected function createChildren():void
		{
			super.createChildren();
			
			try
			{
				var glow:GlowFilter = new GlowFilter(0xFFFFFF,1,4,4,6);
				
				// ----------------------------------
				// DTSAgile logo
				// ----------------------------------
				const dtslogo:Image = new Image();
				dtslogo.source = 'assets/images/dtsagile-logo-vsm.png';
				dtslogo.width = 124;
				dtslogo.height = 35;
				dtslogo.alpha = 0.9;
				dtslogo.useHandCursor = true;
				dtslogo.buttonMode = true;
				dtslogo.filters = [glow];
				dtslogo.addEventListener(MouseEvent.CLICK, dtslogoClickHandler);

				// Above and centered over ESRI
				//dtslogo.setStyle("right", 30);
				//dtslogo.setStyle("bottom", 50);
				
				// Left of and inline with ESRI
				dtslogo.setStyle("right", 100);
				dtslogo.setStyle("bottom", 6);
				
				staticLayer.addElement(dtslogo);
								
//				// ----------------------------------
//				// Copyright
//				// ----------------------------------
//				var copyright:Label = new Label();
//				copyright.text = "© 2010 DTSAgile";	
//				copyright.styleName = "mapCopyrightLabel";
//				var copyrightGlow:GlowFilter = new GlowFilter(0xFFFFFF, 1, 4, 4, 4);
//				copyright.filters = [copyrightGlow];
//				
//				copyright.setStyle("right", 200);
//				copyright.setStyle("bottom", 5);
//				
//				staticLayer.addElement(copyright);
				
			}
			catch(error:Error)
			{
				Alert.show(error.message);
			}
		}
		
		private function dtslogoClickHandler(event:MouseEvent):void
		{
			navigateToURL(new URLRequest('http://dtsagile.com/'));
		}
	}
	
}