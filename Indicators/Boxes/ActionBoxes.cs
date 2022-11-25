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
	public class ActionBoxes : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "ActionBoxes";
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
				A					= 1;
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

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="A", Order=1, GroupName="Parameters")]
		public int A
		{ get; set; }
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private Boxes.ActionBoxes[] cacheActionBoxes;
		public Boxes.ActionBoxes ActionBoxes(int a)
		{
			return ActionBoxes(Input, a);
		}

		public Boxes.ActionBoxes ActionBoxes(ISeries<double> input, int a)
		{
			if (cacheActionBoxes != null)
				for (int idx = 0; idx < cacheActionBoxes.Length; idx++)
					if (cacheActionBoxes[idx] != null && cacheActionBoxes[idx].A == a && cacheActionBoxes[idx].EqualsInput(input))
						return cacheActionBoxes[idx];
			return CacheIndicator<Boxes.ActionBoxes>(new Boxes.ActionBoxes(){ A = a }, input, ref cacheActionBoxes);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.Boxes.ActionBoxes ActionBoxes(int a)
		{
			return indicator.ActionBoxes(Input, a);
		}

		public Indicators.Boxes.ActionBoxes ActionBoxes(ISeries<double> input , int a)
		{
			return indicator.ActionBoxes(input, a);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.Boxes.ActionBoxes ActionBoxes(int a)
		{
			return indicator.ActionBoxes(Input, a);
		}

		public Indicators.Boxes.ActionBoxes ActionBoxes(ISeries<double> input , int a)
		{
			return indicator.ActionBoxes(input, a);
		}
	}
}

#endregion
