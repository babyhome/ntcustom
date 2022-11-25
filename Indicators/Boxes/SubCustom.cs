#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

namespace JTRS
{
	/*
	 * this custom class that maps the Windows media brush with the sharpdx brush (for easy looping in OnRenderTargetChanged)
	 * has to be outside of the indicator class to work correctly with the expandable object class wrapper for the LevelIIColors collection
	 */
	 public class DXMediaMap
	 {
		public SharpDX.Direct2D1.Brush 		DXBrush;
		public System.Windows.Media.Brush 	MediaBrush;
	 }
}

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.Boxes
{
	public class SubCustom : Indicator
	{
		// JtRealStats : Indicator

		private int 									arrowHeight, arrowWidth, barWidth;
		private List<LadderRow> 						askRows, bidRows, rows;
		private int 									askTotY, bidTotY;
		private int 									askVol, bidVol, totAVol, totBVol;
		private int                                     currentX, currentX1, currentY, currentY1, offset, priceY;
		private Dictionary<string, JTRS.DXMediaMap> 	dxmBrushes;
		private double									heightPerTick;
		private int										histogramOpacity;
		private int										idx, priceOffset, barWidthScaler;
		private int										largestVol;
		private int										lastBar, lastTickVol;
		private DateTime 								lastRefresh;
		private SharpDX.Direct2D1.AntialiasMode 		previousAntialiasMode;
		private SharpDX.Vector2[] 						reuseGFeometryVectors;
		private SharpDX.RectangleF 						reuseRect;
		private SharpDX.Vector2 						reuseLineVector1, reuseLineVector2;
		private SharpDX.DirectWrite.TextFormat 			stringFont, stringBFont;
		private int 									tickCnt, ticksRemaining;
		private int 									tickVolHigherThanClose, tickVolLowerThanClose, tickVolEqualClose;
		private int 									volHigherHeight, volLowerHeight, volSameHeight;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Realtime level II / Tick Volume Version 1.5.3";
				Name										= "SubCustom";
				Calculate									= Calculate.OnEachTick;
				IsOverlay									= true;
				DisplayInDataBox							= false;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= false;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= false;
				Aa					= Brushes.Orange;

				// This is an expanable class property that contains brushes
				LevelIIColors = new LevelIIColorCollection()
				{
					Level01Color 							= Brushes.Yellow,
					Level02Color							= Brushes.DarkOrange,
					Level03Color							= Brushes.DarkRed,
					Level04Color							= Brushes.Green,
					Level05Color							= Brushes.Blue,
					Level06Color							= Brushes.Aqua,
					Level07Color							= Brushes.DeepPink,
					Level08Color							= Brushes.DarkMagenta,
					Level09Color							= Brushes.DarkGoldenrod,
					Level10Color							= Brushes.DarkTurquoise
				};

				// for ease of creating SharpDx brushes, I'm adding to an array and looping instead of making a sharpdx brush variable for each brush property
				dxmBrushes 	= new Dictionary<string, JTRS.DXMediaMap>();

				foreach (string brushName in new string[] { "ask", "bid", "higherVol", "lowerVol", "outline", "priceline", "sameVol", "text", "backColor" })
					dxmBrushes.Add(brushName, new JTRS.DXMediaMap());

				// try and use the chart background color from the skin as the default backColor (unless its null in which case use Brushes.DarkGray)
				dxmBrushes["backColor"].MediaBrush 	= System.Windows.Application.Current.TryFindResource("ChartControl.ChartBackground") as SolidColorBrush ?? Brushes.DarkGray;

				AskColor 					= Brushes.DarkRed;
				BidColor 					= Brushes.LimeGreen;
				DrawPriceLine 				= true;
				HigherVolColor 				= Brushes.Aqua;
				HistogramOpacity 			= 100;
				LowerVolColor 				= Brushes.DeepPink;
				MaxLevels 					= 10;
				OutlineColor 				= Brushes.Black;
				OUtlineHistBars 			= false;
				PriceLineColor 				= Brushes.Black;
				RefreshDelay 				= 1000;
				SameVolColor 				= Brushes.LightGray;
				ShowBidAskVolume 			= true;
				ShowHighLowSameVolume 		= true;
				ShowMarketDepth 			= true;
				ShowDepthHistogram 			= true;
				StatsPosition 				= TextPosition.TopLeft;
				ShowStats 					= true;
				ShowTickVolume 				= true;
				// try and use the chart text color from skin as default TextColor (unless its null in which case use Brushes.White)
				TextColor 					= System.Windows.Application.Current.TryFindResource("ChartControl.ChartText") as SolidColorBrush ?? Brushes.White;

			}
			else if (State == State.DataLoaded)
			{
				SetZOrder(-1);

				arrowHeight 				= 12;
				arrowWidth 					= 14;

				askRows 					= new List<LadderRow>();
				bidRows 					= new List<LadderRow>();
				reuseRect 					= new SharpDX.RectangleF();

				reuseGFeometryVectors 		= new SharpDX.Vector2[4] { new SharpDX.Vector2(), new SharpDX.Vector2(), new SharpDX.Vector2(), new SharpDX.Vector2() };

				reuseLineVector1 			= new SharpDX.Vector2();
				reuseLineVector2 			= new SharpDX.Vector2();

				stringFont 					= new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, "Arial", 8f);
				stringBFont 				= new SharpDX.DirectWrite.TextFormat(Core.Globals.DirectWriteFactory, "Arial", SharpDX.DirectWrite.FontWeight.Bold, SharpDX.DirectWrite.FontStyle.Normal, 9f);
			}
			else if (State == State.Realtime)
			{
				lastRefresh 	= DateTime.Now;
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnMarketData(MarketDataEventArgs marketDataUpdate)
		{
			
		}

		protected override void OnMarketDepth(MarketDepthEventArgs marketDepthUpdate)
		{
			
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
			
		}

		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
			priceY = ((ChartPanel.Y + ChartPanel.H) - ((int)(((Close.GetValueAt(CurrentBar) - chartScale.MinValue) / chartScale.MaxMinusMin * ChartPanel.H)) - 1));

			if (ShowTickVolume)
			{
				currentX 	= (ChartPanel.X + ChartPanel.W - 13);

				// Draw the high/low/same volume bars
				if (ShowHighLowSameVolume)
				{
					UpdateRect(ref reuseRect, currentX, ChartPanel.Y, 13, ChartPanel.H);
					RenderTarget.FillRectangle(reuseRect, dxmBrushes["backColor"].DXBrush);

					heightPerTick 	= (double)ChartPanel.H / Bars.GetVolume(CurrentBar);

					if (heightPerTick > 0)
					{
						volHigherHeight 	= (int)(heightPerTick * tickVolHigherThanClose);
						volLowerHeight 		= (int)(heightPerTick * tickVolLowerThanClose);
						volSameHeight 		= (int)(heightPerTick * tickVolEqualClose);
						offset 				= (ChartPanel.H - volLowerHeight - volHigherHeight - volSameHeight) / 2;

						UpdateRect(ref reuseRect, currentX, offset, 11, volHigherHeight);
						RenderTarget.FillRectangle(reuseRect, dxmBrushes["higherVol"].DXBrush); 	// g2g

						UpdateRect(ref reuseRect, currentX, (offset + volHigherHeight), 11, volSameHeight);
						RenderTarget.FillRectangle(reuseRect, dxmBrushes["sameVol"].DXBrush);

						UpdateRect(ref reuseRect, currentX, (offset + volHigherHeight + volSameHeight), 11, volLowerHeight);
						RenderTarget.FillRectangle(reuseRect, dxmBrushes["lowerVol"].DXBrush);

						UpdateRect(ref reuseRect, currentX, (ChartPanel.X - 12 + ChartPanel.W), ChartPanel.Y, 100, 100);
						RenderTarget.DrawText(tickVolHigherThanClose.ToString(), stringBFont, reuseRect, dxmBrushes["text"].DXBrush);

					}
				}

				// draw the bid/ask volume bars
				if (ShowBidAskVolume)
				{
					askVol 	= (int)GetCurrentAskVolume();
					bidVol 	= (int)GetCurrentBidVolume();

					UpdateRect(ref reuseRect, (currentX + 3), (priceY - askVol), 5, askVol);
					RenderTarget.DrawRectangle(reuseRect, dxmBrushes["outline"].DXBrush);

				}
			}

		}





		#region Properties
		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Aa", Order=1, GroupName="Parameters")]
		public Brush Aa
		{ get; set; }

		[Browsable(false)]
		public string AaSerializable
		{
			get { return Serialize.BrushToString(Aa); }
			set { Aa = Serialize.StringToBrush(value); }
		}

		//Fixit
		private const string GROUP_NAME_1 	= "Parameters";
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class LevelIIColorCollection : ICloneable
		{
			[Browsable(false), XmlIgnore]
			public JTRS.DXMediaMap[] DxmBrushes;

			public LevelIIColorCollection()
			{
				DxmBrushes 	= new JTRS.DXMediaMap[10];

				for (int i = 0; i < 10; i++)
					DxmBrushes[i] = new JTRS.DXMediaMap();
			}

			public object Clone()
			{
				LevelIIColorCollection newCollection = new LevelIIColorCollection()
				{
					Level01Color	= Level01Color,
					Level02Color	= Level02Color,
					Level03Color	= Level03Color,
					Level04Color	= Level04Color,
					Level05Color	= Level05Color,
					Level06Color	= Level06Color,
					Level07Color	= Level07Color,
					Level08Color	= Level08Color,
					Level09Color	= Level09Color,
					Level10Color	= Level10Color
				};

				return newCollection;
			}

			public override string ToString()
			{
				return string.Empty;
			}

			[XmlIgnore]
			[Display(Name = "Level 1 Color", Order = 6, GroupName = GROUP_NAME_1)]
			public Brush Level01Color
			{
				get { return DxmBrushes[0].MediaBrush; }
				set { DxmBrushes[0].MediaBrush = value; }
			}

			[Browsable(false)]
			public string Level01ColorSerializable
			{
				get { return Serialize.BrushToString(Level01Color); }
				set { Level01Color = Serialize.StringToBrush(value); }
			}

			[XmlIgnore]
			[Display(Name = "Level 2 Color", Order = 7, GroupName = GROUP_NAME_1)]
			public Brush Level02Color
			{
				get { return DxmBrushes[1].MediaBrush; }
				set { DxmBrushes[1].MediaBrush = value; }
			}

			[Browsable(false)]
			public string Level02ColorSerializable
			{
				get { return Serialize.BrushToString(Level02Color); }
				set { Level02Color = Serialize.StringToBrush(value); }
			}

			[XmlIgnore]
			[Display(Name = "Level 3 Color", Order = 8, GroupName = GROUP_NAME_1)]
			public Brush Level03Color
			{
				get { return DxmBrushes[2].MediaBrush; }
				set { DxmBrushes[2].MediaBrush = value; }
			}

			[Browsable(false)]
			public string Level03ColorSerializable
			{
				get { return Serialize.BrushToString(Level03Color); }
				set { Level03Color = Serialize.StringToBrush(value); }
			}

			[XmlIgnore]
			[Display(Name = "Level 4 Color", Order = 9, GroupName = GROUP_NAME_1)]
			public Brush Level04Color
			{
				get { return DxmBrushes[3].MediaBrush; }
				set { DxmBrushes[3].MediaBrush = value; }
			}

			[Browsable(false)]
			public string Level04ColorSerializable
			{
				get { return Serialize.BrushToString(Level04Color); }
				set { Level04Color = Serialize.StringToBrush(value); }
			}

			[XmlIgnore]
			[Display(Name = "Level 5 Color", Order = 10, GroupName = GROUP_NAME_1)]
			public Brush Level05Color
			{
				get { return DxmBrushes[4].MediaBrush; }
				set { DxmBrushes[4].MediaBrush = value; }
			}

			[Browsable(false)]
			public string Level05ColorSerializable
			{
				get { return Serialize.BrushToString(Level05Color); }
				set { Level05Color = Serialize.StringToBrush(value); }
			}

			[XmlIgnore]
			[Display(Name = "Level 6 Color", Order = 11, GroupName = GROUP_NAME_1)]
			public Brush Level06Color
			{
				get { return DxmBrushes[5].MediaBrush; }
				set { DxmBrushes[5].MediaBrush = value; }
			}

			[Browsable(false)]
			public string Level06ColorSerializable
			{
				get { return Serialize.BrushToString(Level06Color); }
				set { Level06Color = Serialize.StringToBrush(value); }
			}

			[XmlIgnore]
			[Display(Name = "Level 7 Color", Order = 12, GroupName = GROUP_NAME_1)]
			public Brush Level07Color
			{
				get { return DxmBrushes[6].MediaBrush; }
				set { DxmBrushes[6].MediaBrush = value; }
			}

			[Browsable(false)]
			public string Level07ColorSerializable
			{
				get { return Serialize.BrushToString(Level07Color); }
				set { Level07Color = Serialize.StringToBrush(value); }
			}

			[XmlIgnore]
			[Display(Name = "Level 8 Color", Order = 13, GroupName = GROUP_NAME_1)]
			public Brush Level08Color
			{
				get { return DxmBrushes[7].MediaBrush; }
				set { DxmBrushes[7].MediaBrush = value; }
			}

			[Browsable(false)]
			public string Level08ColorSerializable
			{
				get { return Serialize.BrushToString(Level08Color); }
				set { Level08Color = Serialize.StringToBrush(value); }
			}

			[XmlIgnore]
			[Display(Name = "Level 9 Color", Order = 13, GroupName = GROUP_NAME_1)]
			public Brush Level09Color
			{
				get { return DxmBrushes[8].MediaBrush; }
				set { DxmBrushes[8].MediaBrush = value; }
			}

			[Browsable(false)]
			public string Level09ColorSerializable
			{
				get { return Serialize.BrushToString(Level09Color); }
				set { Level09Color = Serialize.StringToBrush(value); }
			}

			[XmlIgnore]
			[Display(Name = "Level 10 Color", Order = 14, GroupName = GROUP_NAME_1)]
			public Brush Level10Color
			{
				get { return DxmBrushes[9].MediaBrush; }
				set { DxmBrushes[9].MediaBrush = value; }
			}

			[Browsable(false)]
			public string Level10ColorSerializable
			{
				get { return Serialize.BrushToString(Level10Color); }
				set { Level10Color = Serialize.StringToBrush(value); }
			}

		}

		[XmlIgnore]
		[Display(Name = "AskColor", Description = "Color for Ask Volume", Order = 1, GroupName = GROUP_NAME_1)]
		public Brush AskColor
		{
			get { return dxmBrushes["ask"].MediaBrush; }
			set { dxmBrushes["ask"].MediaBrush = value; }
		}

		[Browsable(false)]
		public string AskColorSerializable
		{
			get { return Serialize.BrushToString(AskColor); }
			set { AskColor = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name = "BidColor", Description = "Color for Bid Volume", Order = 2, GroupName = GROUP_NAME_1)]
		public Brush BidColor
		{
			get { return dxmBrushes["bid"].MediaBrush; }
			set { dxmBrushes["bid"].MediaBrush = value; }
		}

		[Browsable(false)]
		public string BidColorSerializable
		{
			get { return Serialize.BrushToString(BidColor); }
			set { BidColor = Serialize.StringToBrush(value); }
		}

		[NinjaScriptProperty]
		[Display(Name = "DrawPriceLine", Description = "Draw a line at the current price level. Makes it easier to see price movement when you have a larger right margin", Order = 3, GroupName = GROUP_NAME_1)]
		public bool DrawPriceLine
		{ get; set; }

		[XmlIgnore]
		[Display(Name = "HigherVolColor", Description = "Color for the volume higher", Order = 4, GroupName = GROUP_NAME_1)]
		public Brush HigherVolColor
		{
			get { return dxmBrushes["higherVol"].MediaBrush; }
			set { dxmBrushes["higherVol"].MediaBrush = value; }
		}

		[Browsable(false)]
		public string HigherVolColorSerializable
		{
			get { return Serialize.BrushToString(HigherVolColor); }
			set { HigherVolColor = Serialize.StringToBrush(value); }
		}

		[Range(0, 100)]
		[Display(Name = "HistogramOpacity", Description = "Histogram opacity", Order = 5, GroupName = GROUP_NAME_1)]
		public int HistogramOpacity
		{
			get { return HistogramOpacity; }
			set
			{
				HistogramOpacity = value;
				SetOpacity();
			}
		}

		[NinjaScriptProperty]
		[Display(Name = "Level II Colors", Description = "Level II Colors", Order = 6, GroupName = GROUP_NAME_1)]
		public LevelIIColorCollection LevelIIColors
		{ get; set; }

		[XmlIgnore]
		[Display(Name = "LowerVolColor", Description = "Color for the volume lower", Order = 16, GroupName = GROUP_NAME_1)]
		public Brush LowerVolColor
		{
			get { return dxmBrushes["lowerVol"].MediaBrush; }
			set { dxmBrushes["lowerVol"].MediaBrush = value; }
		}

		[Browsable(false)]
		public string LowerVolColorSerializable
		{
			get { return Serialize.BrushToString(LowerVolColor); }
			set { LowerVolColor = Serialize.StringToBrush(value); }
		}

		[Range(0, 10)]
		[Display(Name = "MaxLevels", Description = "Max Levels to Display", Order = 17, GroupName = GROUP_NAME_1)]
		public int MaxLevels
		{
			get;
			set;
		}

		[XmlIgnore]
		[Display(Name = "OutlineColor", Description = "Color used to outline bars when outlining is enable", Order = 18, GroupName = GROUP_NAME_1)]
		public Brush OutlineColor
		{
			get { return dxmBrushes["outline"].MediaBrush; }
			set { dxmBrushes["outline"].MediaBrush = value; }
		}

		[Browsable(false)]
		public string OutlineColorSerializable
		{
			get { return Serialize.BrushToString(OutlineColor); }
			set { OutlineColor = Serialize.StringToBrush(value); }
		}

		[NinjaScriptProperty]
		[Display(Name = "OutlineHistBars", Description = "Outline histogram bars for visibility", Order = 19, GroupName = GROUP_NAME_1)]
		public bool OUtlineHistBars
		{ get; set; }

		[XmlIgnore]
		[Display(Name = "PriceLineColor", Description = "Price line color", Order = 20, GroupName = GROUP_NAME_1)]
		public Brush PriceLineColor
		{
			get { return dxmBrushes["priceline"].MediaBrush; }
			set { dxmBrushes["priceline"].MediaBrush = value; }
		}

		[Browsable(false)]
		public string PriceLineColorSerializable
		{
			get { return Serialize.BrushToString(PriceLineColor); }
			set { PriceLineColor = Serialize.StringToBrush(value); }
		}

		[Range(0, double.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name = "RefreshDelay", Description = "Level II refresh delay in milliseconds.\r\nNOTE: This will be the number of milliseconds that the Level II data will lag real-time. This may be necessary if your CPU utilization increases too much.", Order = 21, GroupName = GROUP_NAME_1)]
		public double RefreshDelay
		{ get; set; }

		[XmlIgnore]
		[Display(Name = "SameVolColor", Description = "Color for volume same", Order = 22, GroupName = GROUP_NAME_1)]
		public Brush SameVolColor
		{
			get { return dxmBrushes["sameVol"].MediaBrush; }
			set { dxmBrushes["sameVol"].MediaBrush = value; }
		}

		[Browsable(false)]
		public string SameVolColorSerializable
		{
			get { return Serialize.BrushToString(SameVolColor); }
			set { SameVolColor = Serialize.StringToBrush(value); }
		}

		[NinjaScriptProperty]
		[Display(Name = "ShowBidAskVolume", Description = "Show current bid/ask volume", Order = 23, GroupName = GROUP_NAME_1)]
		public bool ShowBidAskVolume
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name = "ShowHighLowSameVolume", Description = "Show volume higher, lower and same as last bar close price", Order = 24, GroupName = GROUP_NAME_1)]
		public bool ShowHighLowSameVolume
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name = "Show market depth", Description = "Show Level II indicator", Order = 25, GroupName = GROUP_NAME_1)]
		public bool ShowMarketDepth
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name = "Show depth histogram", Description = "Show Level II histogram", Order = 26, GroupName = GROUP_NAME_1)]
		public bool ShowDepthHistogram
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name = "Stats position", Description = "Position on Price panel where statics will be displayed", Order = 27, GroupName = GROUP_NAME_1)]
		public TextPosition StatsPosition
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name = "Show stats", Description = "Show Volume/Tick statistics on charts", Order = 28, GroupName = GROUP_NAME_1)]
		public bool ShowStats
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name = "Show tick volume", Description = "Show Tick volume indicator", Order = 29, GroupName = GROUP_NAME_1)]
		public bool ShowTickVolume
		{ get; set; }

		[XmlIgnore]
		[Display(Name = "Text color", Description = "Color used to outline bars when outlining is enable", Order = 18, GroupName = GROUP_NAME_1)]
		public Brush TextColor
		{
			get { return dxmBrushes["text"].MediaBrush; }
			set { dxmBrushes["text"].MediaBrush = value; }
		}

		[Browsable(false)]
		public string TextColorSerializable
		{
			get { return Serialize.BrushToString(TextColor); }
			set { TextColor = Serialize.StringToBrush(value); }
		}
		#endregion // Properties

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private Boxes.SubCustom[] cacheSubCustom;
		public Boxes.SubCustom SubCustom(Brush aa)
		{
			return SubCustom(Input, aa);
		}

		public Boxes.SubCustom SubCustom(ISeries<double> input, Brush aa)
		{
			if (cacheSubCustom != null)
				for (int idx = 0; idx < cacheSubCustom.Length; idx++)
					if (cacheSubCustom[idx] != null && cacheSubCustom[idx].Aa == aa && cacheSubCustom[idx].EqualsInput(input))
						return cacheSubCustom[idx];
			return CacheIndicator<Boxes.SubCustom>(new Boxes.SubCustom(){ Aa = aa }, input, ref cacheSubCustom);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.Boxes.SubCustom SubCustom(Brush aa)
		{
			return indicator.SubCustom(Input, aa);
		}

		public Indicators.Boxes.SubCustom SubCustom(ISeries<double> input , Brush aa)
		{
			return indicator.SubCustom(input, aa);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.Boxes.SubCustom SubCustom(Brush aa)
		{
			return indicator.SubCustom(Input, aa);
		}

		public Indicators.Boxes.SubCustom SubCustom(ISeries<double> input , Brush aa)
		{
			return indicator.SubCustom(input, aa);
		}
	}
}

#endregion
