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
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies.NMNStrategies.Unlocked
{
	public class DEMASMACrossOverEntryUnlocked : Strategy
	{
		private DEMA DEMA1;
		private SMA SMA1;
		private DEMA DEMA2;
		private SMA SMA2;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Trade Entry with DEMA14 & SMA10 crossover. Exit at preset ticks or reverse signal!!";
				Name										= "DEMASMACrossOverEntryUnlocked";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 9;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlatSynchronizeAccount;
				TimeInForce									= TimeInForce.Day;
				TraceOrders									= true;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				Profit					= 100;
				StopLoss					= -50;
				Contracts					= 1;
				AmtProfitTarget					= 500;
				AmtLossLimit					= 500;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				DEMA1				= DEMA(Close, 14);
				SMA1				= SMA(Close, 10);
				DEMA2				= DEMA(Close, 14);
				SMA2				= SMA(Close, 10);
				DEMA1.Plots[0].Brush = Brushes.Brown;
				SMA1.Plots[0].Brush = Brushes.CornflowerBlue;
				AddChartIndicator(DEMA1);
				AddChartIndicator(SMA1);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
				return;

			 // Set 1
			if ((Position.MarketPosition == MarketPosition.Flat)
				 && (CrossAbove(DEMA1, SMA1, 1))
				 // Bar Encoses DEMA14 & SMA10
				 && ((Close[1] > Open[1])
				 && (Close[1] > DEMA2[1])
				 && (Close[1] > SMA2[1])
				 && (Open[1] < DEMA2[1])
				 && (Open[1] < SMA2[1])))
			{
				EnterLong(Convert.ToInt32(DefaultQuantity), @"MyEntryLong");
			}
			
			 // Set 2
			if ((Position.MarketPosition == MarketPosition.Flat)
				 && (CrossBelow(DEMA1, SMA1, 1))
				 // Bar Encoses DEMA14 & SMA10
				 && ((Close[1] < Open[1])
				 && (Close[1] < DEMA2[1])
				 && (Close[1] < SMA2[1])
				 && (Open[1] > DEMA2[1])
				 && (Open[1] > SMA2[1])))
			{
				EnterShort(Convert.ToInt32(DefaultQuantity), @"MyEntryShort");
			}
			
			 // Set 3
			if ((Position.MarketPosition != MarketPosition.Long)
				 && (Position.GetUnrealizedProfitLoss(PerformanceUnit.Currency, Close[0]) >= AmtProfitTarget))
			{
				ExitLong(Convert.ToInt32(DefaultQuantity), "ExitLong", @"MyEntryLong");
			}
			
			 // Set 4
			if ((Position.MarketPosition != MarketPosition.Short)
				 && (Position.GetUnrealizedProfitLoss(PerformanceUnit.Currency, Close[0]) >= AmtProfitTarget))
			{
				ExitShort(Convert.ToInt32(DefaultQuantity), "ExitShort", @"MyEntryShort");
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="Profit", Description="Profit Target in ticks.", Order=1, GroupName="Parameters")]
		public int Profit
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="StopLoss", Description="StopLoss in ticks", Order=2, GroupName="Parameters")]
		public int StopLoss
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Contracts", Description="Number of lots to trade with.", Order=3, GroupName="Parameters")]
		public int Contracts
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="AmtProfitTarget", Description="Target profit of a trade", Order=4, GroupName="Parameters")]
		public int AmtProfitTarget
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="AmtLossLimit", Description="Allowed Loss limit", Order=5, GroupName="Parameters")]
		public int AmtLossLimit
		{ get; set; }
		#endregion

	}
}
