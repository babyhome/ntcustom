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

#endregion



#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		
		private futuresAnalytica.AnalyticaChart3[] cacheAnalyticaChart3;

		
		public futuresAnalytica.AnalyticaChart3 AnalyticaChart3(int stackLevels, int maxDays, Brush cellColor, Brush highColor, Brush currColor, Brush markColor, float markOpacity, Brush textColor, int textSize, Brush askColor, Brush bidColor, double minImbalance, double minRatio, bool usingMoreThanOneTickPerLevel, bool autoScroll, bool setOulineColor, bool showDelta, bool showPercentChangeInDelta, int deltaTextSize, bool showBidAsk, bool showUnfinished, bool fadeCells, bool fadeText)
		{
			return AnalyticaChart3(Input, stackLevels, maxDays, cellColor, highColor, currColor, markColor, markOpacity, textColor, textSize, askColor, bidColor, minImbalance, minRatio, usingMoreThanOneTickPerLevel, autoScroll, setOulineColor, showDelta, showPercentChangeInDelta, deltaTextSize, showBidAsk, showUnfinished, fadeCells, fadeText);
		}


		
		public futuresAnalytica.AnalyticaChart3 AnalyticaChart3(ISeries<double> input, int stackLevels, int maxDays, Brush cellColor, Brush highColor, Brush currColor, Brush markColor, float markOpacity, Brush textColor, int textSize, Brush askColor, Brush bidColor, double minImbalance, double minRatio, bool usingMoreThanOneTickPerLevel, bool autoScroll, bool setOulineColor, bool showDelta, bool showPercentChangeInDelta, int deltaTextSize, bool showBidAsk, bool showUnfinished, bool fadeCells, bool fadeText)
		{
			if (cacheAnalyticaChart3 != null)
				for (int idx = 0; idx < cacheAnalyticaChart3.Length; idx++)
					if (cacheAnalyticaChart3[idx].stackLevels == stackLevels && cacheAnalyticaChart3[idx].maxDays == maxDays && cacheAnalyticaChart3[idx].cellColor == cellColor && cacheAnalyticaChart3[idx].highColor == highColor && cacheAnalyticaChart3[idx].currColor == currColor && cacheAnalyticaChart3[idx].markColor == markColor && cacheAnalyticaChart3[idx].markOpacity == markOpacity && cacheAnalyticaChart3[idx].textColor == textColor && cacheAnalyticaChart3[idx].textSize == textSize && cacheAnalyticaChart3[idx].askColor == askColor && cacheAnalyticaChart3[idx].bidColor == bidColor && cacheAnalyticaChart3[idx].minImbalance == minImbalance && cacheAnalyticaChart3[idx].minRatio == minRatio && cacheAnalyticaChart3[idx].UsingMoreThanOneTickPerLevel == usingMoreThanOneTickPerLevel && cacheAnalyticaChart3[idx].autoScroll == autoScroll && cacheAnalyticaChart3[idx].setOulineColor == setOulineColor && cacheAnalyticaChart3[idx].showDelta == showDelta && cacheAnalyticaChart3[idx].ShowPercentChangeInDelta == showPercentChangeInDelta && cacheAnalyticaChart3[idx].deltaTextSize == deltaTextSize && cacheAnalyticaChart3[idx].showBidAsk == showBidAsk && cacheAnalyticaChart3[idx].showUnfinished == showUnfinished && cacheAnalyticaChart3[idx].fadeCells == fadeCells && cacheAnalyticaChart3[idx].fadeText == fadeText && cacheAnalyticaChart3[idx].EqualsInput(input))
						return cacheAnalyticaChart3[idx];
			return CacheIndicator<futuresAnalytica.AnalyticaChart3>(new futuresAnalytica.AnalyticaChart3(){ stackLevels = stackLevels, maxDays = maxDays, cellColor = cellColor, highColor = highColor, currColor = currColor, markColor = markColor, markOpacity = markOpacity, textColor = textColor, textSize = textSize, askColor = askColor, bidColor = bidColor, minImbalance = minImbalance, minRatio = minRatio, UsingMoreThanOneTickPerLevel = usingMoreThanOneTickPerLevel, autoScroll = autoScroll, setOulineColor = setOulineColor, showDelta = showDelta, ShowPercentChangeInDelta = showPercentChangeInDelta, deltaTextSize = deltaTextSize, showBidAsk = showBidAsk, showUnfinished = showUnfinished, fadeCells = fadeCells, fadeText = fadeText }, input, ref cacheAnalyticaChart3);
		}

	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		
		public Indicators.futuresAnalytica.AnalyticaChart3 AnalyticaChart3(int stackLevels, int maxDays, Brush cellColor, Brush highColor, Brush currColor, Brush markColor, float markOpacity, Brush textColor, int textSize, Brush askColor, Brush bidColor, double minImbalance, double minRatio, bool usingMoreThanOneTickPerLevel, bool autoScroll, bool setOulineColor, bool showDelta, bool showPercentChangeInDelta, int deltaTextSize, bool showBidAsk, bool showUnfinished, bool fadeCells, bool fadeText)
		{
			return indicator.AnalyticaChart3(Input, stackLevels, maxDays, cellColor, highColor, currColor, markColor, markOpacity, textColor, textSize, askColor, bidColor, minImbalance, minRatio, usingMoreThanOneTickPerLevel, autoScroll, setOulineColor, showDelta, showPercentChangeInDelta, deltaTextSize, showBidAsk, showUnfinished, fadeCells, fadeText);
		}


		
		public Indicators.futuresAnalytica.AnalyticaChart3 AnalyticaChart3(ISeries<double> input , int stackLevels, int maxDays, Brush cellColor, Brush highColor, Brush currColor, Brush markColor, float markOpacity, Brush textColor, int textSize, Brush askColor, Brush bidColor, double minImbalance, double minRatio, bool usingMoreThanOneTickPerLevel, bool autoScroll, bool setOulineColor, bool showDelta, bool showPercentChangeInDelta, int deltaTextSize, bool showBidAsk, bool showUnfinished, bool fadeCells, bool fadeText)
		{
			return indicator.AnalyticaChart3(input, stackLevels, maxDays, cellColor, highColor, currColor, markColor, markOpacity, textColor, textSize, askColor, bidColor, minImbalance, minRatio, usingMoreThanOneTickPerLevel, autoScroll, setOulineColor, showDelta, showPercentChangeInDelta, deltaTextSize, showBidAsk, showUnfinished, fadeCells, fadeText);
		}
	
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		
		public Indicators.futuresAnalytica.AnalyticaChart3 AnalyticaChart3(int stackLevels, int maxDays, Brush cellColor, Brush highColor, Brush currColor, Brush markColor, float markOpacity, Brush textColor, int textSize, Brush askColor, Brush bidColor, double minImbalance, double minRatio, bool usingMoreThanOneTickPerLevel, bool autoScroll, bool setOulineColor, bool showDelta, bool showPercentChangeInDelta, int deltaTextSize, bool showBidAsk, bool showUnfinished, bool fadeCells, bool fadeText)
		{
			return indicator.AnalyticaChart3(Input, stackLevels, maxDays, cellColor, highColor, currColor, markColor, markOpacity, textColor, textSize, askColor, bidColor, minImbalance, minRatio, usingMoreThanOneTickPerLevel, autoScroll, setOulineColor, showDelta, showPercentChangeInDelta, deltaTextSize, showBidAsk, showUnfinished, fadeCells, fadeText);
		}


		
		public Indicators.futuresAnalytica.AnalyticaChart3 AnalyticaChart3(ISeries<double> input , int stackLevels, int maxDays, Brush cellColor, Brush highColor, Brush currColor, Brush markColor, float markOpacity, Brush textColor, int textSize, Brush askColor, Brush bidColor, double minImbalance, double minRatio, bool usingMoreThanOneTickPerLevel, bool autoScroll, bool setOulineColor, bool showDelta, bool showPercentChangeInDelta, int deltaTextSize, bool showBidAsk, bool showUnfinished, bool fadeCells, bool fadeText)
		{
			return indicator.AnalyticaChart3(input, stackLevels, maxDays, cellColor, highColor, currColor, markColor, markOpacity, textColor, textSize, askColor, bidColor, minImbalance, minRatio, usingMoreThanOneTickPerLevel, autoScroll, setOulineColor, showDelta, showPercentChangeInDelta, deltaTextSize, showBidAsk, showUnfinished, fadeCells, fadeText);
		}

	}
}

#endregion
