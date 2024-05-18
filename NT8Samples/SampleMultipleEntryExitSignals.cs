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
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class SampleMultipleEntryExitSignals : Strategy
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= @"Simple multiple RSI/SMA cross over strategy.";
				Name						= "Sample multiple entry exit signals";
				Calculate					= Calculate.OnBarClose;
				EntriesPerDirection			= 1;
				EntryHandling				= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy = true;
          		ExitOnSessionCloseSeconds    = 30;
				IsFillLimitOnTouch			= false;
				MaximumBarsLookBack			= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution			= OrderFillResolution.Standard;
				Slippage					= 0;
				StartBehavior				= StartBehavior.WaitUntilFlat;
				TimeInForce					= TimeInForce.Gtc;
				TraceOrders					= false;
				RealtimeErrorHandling		= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling			= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade			= 20;
				Period						= 14;
				Smooth						= 3;
				SMAPeriod					= 5;
			}
			else if (State == State.DataLoaded)
			{
				AddChartIndicator(RSI(Period, Smooth));
				AddChartIndicator(SMA(SMAPeriod));
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < BarsRequiredToTrade)
				return;
			
			// Entry Signal 1: If RSI value crosses above 20
			if (CrossAbove(RSI(Period, Smooth), 20, 1))
			{
				// Placing a string between the parenthesis allows you to give unique identifiers to your entries.
				EnterLong("RSI Entry");
			}
			
			// Exit Signal 1: If RSI value crosses below 80
			if (CrossBelow(RSI(Period, Smooth), 80, 1))
				ExitLong("RSI Entry");
			
			// Entry Signal 2: If SMA crosses above the current close
			if (CrossAbove(SMA(SMAPeriod), Close, 1))
				EnterLong("SMA Entry");
			
			// Exit Signal 2: If SMA crosses below the current close
			if (CrossBelow(SMA(SMAPeriod), Close, 1))
				ExitLong("SMA Entry");
		}

		#region Properties
		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Period", Description="Numbers of bars used for RSIcalculations", Order=1, GroupName="Parameters")]
		public int Period
		{ get; set; }

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Smooth", Description="Number of bars for smoothing", Order=2, GroupName="Parameters")]
		public int Smooth
		{ get; set; }

		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="SMAPeriod", Description="Numbers of bars used for SMA calculations", Order=3, GroupName="Parameters")]
		public int SMAPeriod
		{ get; set; }
		#endregion
	}
}