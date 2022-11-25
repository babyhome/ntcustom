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

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.Boxes
{
	public class ICTFVG : Indicator
	{
		enum FVGType
		{
			R, S
		}

		class FVG
		{
			public double 	upperPrice;
			public double 	lowerPrice;
			public string 	tag;
			public FVGType 	type;

			public FVG(string tag, FVGType type, double lowerPrice, double upperPrice)
			{
				this.tag 	= tag;
				this.type 	= type;
				this.lowerPrice 	= lowerPrice;
				this.upperPrice 	= upperPrice;
			}
		}

		private List<FVG> fvgList = new List<FVG>();
		private ATR atr;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Fair Value Gap (ICT)";
				Name										= "ICTFVG";
				Calculate									= Calculate.OnEachTick;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				MaxBars 					= 500;

				UpBrush 			= Brushes.DarkGreen;
				DownBrush 			= Brushes.Maroon;
				ATRPeriod 			= 10;
				ImpulseFactor 		= 1.1;
				
			}
			else if (State == State.Configure)
			{
				atr 	= ATR(ATRPeriod);
			}
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
			if (CurrentBar < (Bars.Count - MaxBars)) return;

			RemoveInvalidatedFVGs();

			// FVG only applies if there's been an impulse move
			if (Math.Abs(High[1] - Low[1]) >= ImpulseFactor * atr.Volume[0])
			{
				// Fair value gap while going UP
				// Low[0] > High[2]
				if (Low[0] > High[2])
				{
					string tag 	= "FVGUP" + CurrentBar;
					Draw.Rectangle(this, tag, false, 2, Low[0], -100000, High[2], UpBrush, UpBrush, 13, true);
					fvgList.Add(new FVG(tag, FVGType.S, Low[0], High[2]));
				}

				// Fair value gap while going DOWN
				// High[0] < Low[2]
				if (High[0] < Low[2])
				{
					string tag = "FVGDOWN" + CurrentBar;
					Draw.Rectangle(this, "FVGDOWN" + CurrentBar, false, 2, Low[2], -100000, High[0], DownBrush, DownBrush, 13, true);
					fvgList.Add(new FVG(tag, FVGType.R, High[0], Low[2]));
				}
			}
		}

		private void RemoveInvalidatedFVGs()
		{
			List<FVG> invalidated = new List<FVG>();

			foreach( FVG fvg in fvgList)
			{
				if (fvg.type == FVGType.R && Close[0] > fvg.upperPrice)
				{
					if (DrawObjects[fvg.tag] != null)
					{
						invalidated.Add(fvg);
					}
				}
				else if (fvg.type == FVGType.S && Close[0] < fvg.lowerPrice)
				{
					if (DrawObjects[fvg.tag] != null)
					{
						invalidated.Add(fvg);
					}
				}
			}

			foreach (FVG fvg in invalidated)
			{
				fvgList.Remove(fvg);

				if (DrawObjects[fvg.tag] != null)
				{
					RemoveDrawObject(fvg.tag);
				}
			}
		}

		public const string GROUP_NAME = "Parameters";

		#region Properties
		[NinjaScriptProperty]
		[Range(3, int.MaxValue)]
		[Display(Name ="Max Lookback Bars", Order = 100, GroupName = GROUP_NAME)]
		public int MaxBars
		{ get; set; }

		[NinjaScriptProperty]
		[Range(3, int.MaxValue)]
		[Display(Name = "ATR Period (To Detect Impulse Moves)", Order = 200, GroupName = GROUP_NAME)]
		public int ATRPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name = "ATRs in Impulse Move", Order = 300, GroupName = GROUP_NAME)]
		public double ImpulseFactor
		{ get; set; }
		
		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name = "Bearish FVG Color", Order = 400, GroupName = GROUP_NAME)]
		public Brush DownBrush
		{ get; set; }

		[Browsable(false)]
		public string DownBrushSerializable
		{
			get { return Serialize.BrushToString(DownBrush); }
			set { DownBrush = Serialize.StringToBrush(value); }
		}

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name = "Bullish FVG Color", Order = 500, GroupName = GROUP_NAME)]
		public Brush UpBrush
		{ get; set; }

		[Browsable(false)]
		public string UpBrushSerializable
		{
			get { return Serialize.BrushToString(UpBrush); }
			set { UpBrush = Serialize.StringToBrush(value); }
		}

		#endregion Properties
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private Boxes.ICTFVG[] cacheICTFVG;
		public Boxes.ICTFVG ICTFVG(int maxBars, int aTRPeriod, double impulseFactor, Brush downBrush, Brush upBrush)
		{
			return ICTFVG(Input, maxBars, aTRPeriod, impulseFactor, downBrush, upBrush);
		}

		public Boxes.ICTFVG ICTFVG(ISeries<double> input, int maxBars, int aTRPeriod, double impulseFactor, Brush downBrush, Brush upBrush)
		{
			if (cacheICTFVG != null)
				for (int idx = 0; idx < cacheICTFVG.Length; idx++)
					if (cacheICTFVG[idx] != null && cacheICTFVG[idx].MaxBars == maxBars && cacheICTFVG[idx].ATRPeriod == aTRPeriod && cacheICTFVG[idx].ImpulseFactor == impulseFactor && cacheICTFVG[idx].DownBrush == downBrush && cacheICTFVG[idx].UpBrush == upBrush && cacheICTFVG[idx].EqualsInput(input))
						return cacheICTFVG[idx];
			return CacheIndicator<Boxes.ICTFVG>(new Boxes.ICTFVG(){ MaxBars = maxBars, ATRPeriod = aTRPeriod, ImpulseFactor = impulseFactor, DownBrush = downBrush, UpBrush = upBrush }, input, ref cacheICTFVG);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.Boxes.ICTFVG ICTFVG(int maxBars, int aTRPeriod, double impulseFactor, Brush downBrush, Brush upBrush)
		{
			return indicator.ICTFVG(Input, maxBars, aTRPeriod, impulseFactor, downBrush, upBrush);
		}

		public Indicators.Boxes.ICTFVG ICTFVG(ISeries<double> input , int maxBars, int aTRPeriod, double impulseFactor, Brush downBrush, Brush upBrush)
		{
			return indicator.ICTFVG(input, maxBars, aTRPeriod, impulseFactor, downBrush, upBrush);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.Boxes.ICTFVG ICTFVG(int maxBars, int aTRPeriod, double impulseFactor, Brush downBrush, Brush upBrush)
		{
			return indicator.ICTFVG(Input, maxBars, aTRPeriod, impulseFactor, downBrush, upBrush);
		}

		public Indicators.Boxes.ICTFVG ICTFVG(ISeries<double> input , int maxBars, int aTRPeriod, double impulseFactor, Brush downBrush, Brush upBrush)
		{
			return indicator.ICTFVG(input, maxBars, aTRPeriod, impulseFactor, downBrush, upBrush);
		}
	}
}

#endregion
