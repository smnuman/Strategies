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
namespace NinjaTrader.NinjaScript.Strategies.NT8ForumExamples
{
	public class TonyStrategyforShare : Strategy
	{
		private double EMA_Round_Up;
		private double EMA_Round_Down;

		private EMA EMA1;
		private RSI RSI1;
		private MACD MACD1;
		
		private bool OkToTrade;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "MainSimpleStrategyCODE";
				Calculate									= Calculate.OnEachTick;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= true;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				
				OkToTrade									= false;
				
				TakeProfit					= 9;
				StopLoss					= 6;
				EMA_Range					= 3;
				MACD_Fast					= 21;
				MACD_Slow					= 90;
				MACD_M					    = 9;
				EMA_Round_Up					= 1;
				EMA_Round_Down					= 1;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				EMA1				= EMA(Close, 20);
				RSI1				= RSI(Close, 34, 3);
				MACD1				= MACD(Close, Convert.ToInt32(MACD_Fast), Convert.ToInt32(MACD_Slow), Convert.ToInt32(MACD_M));
			}
		}

		protected override void OnBarUpdate()
		{
			if (State == State.Historical)
				return;
			
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 14)
				return;

			EMA_Round_Up = (EMA1[0] + (EMA_Range * TickSize)) ;
			EMA_Round_Down = (EMA1[0] + (EMA_Range * TickSize)) ;
			
			 // Set 2
			if ((RSI1.Default[0] > 55))
			{
				if ((Position.MarketPosition == MarketPosition.Flat))
					{
						EnterLong(Convert.ToInt32(DefaultQuantity), @"myEntry");
						Print(@"Ok... it does enter here");
						OkToTrade = true;
					}
			
				if (Position.MarketPosition == MarketPosition.Long && OkToTrade == true)
					{
						Print(@"Enters LONG CONDITION");
						ExitLongLimit(0, true, Convert.ToInt32(DefaultQuantity), (Position.AveragePrice + (TakeProfit * TickSize)) , @"myTarget", @"myEntry");
						ExitLongStopLimit(0, true, Convert.ToInt32(DefaultQuantity), 0, (Position.AveragePrice + (-StopLoss * TickSize)) , @"myStop", @"myEntry");
						OkToTrade = false;
					}	
				Print(@"Ok its skipping the logic");
				Print(Convert.ToString(Position.MarketPosition));
				Print(OkToTrade);
			}
			
			 // Set 3
			if ((RSI1.Default[0] < 45))
			{
				
				if ((Position.MarketPosition == MarketPosition.Flat))
					{					
						EnterShort(Convert.ToInt32(DefaultQuantity), @"myShort");
						OkToTrade = true;
					}	

				if (Position.MarketPosition == MarketPosition.Short && OkToTrade == true)
					{
						ExitShortLimit(0, true, Convert.ToInt32(DefaultQuantity), (Position.AveragePrice - (TakeProfit * TickSize)) , @"myProfit", @"myShort");
						ExitShortStopLimit(0, true, Convert.ToInt32(DefaultQuantity), 0, (Position.AveragePrice + (StopLoss * TickSize)) , @"myLoss", @"myShort");
//						Print(@"Enters SHORT CONDITION");
						OkToTrade = false;
					}	
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TakeProfit", Order=1, GroupName="Parameters")]
		public int TakeProfit
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StopLoss", Order=2, GroupName="Parameters")]
		public int StopLoss
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="EMA_Range", Order=3, GroupName="Parameters")]
		public int EMA_Range
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MACD_Fast", Order=4, GroupName="Parameters")]
		public int MACD_Fast
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MACD_Slow", Order=5, GroupName="Parameters")]
		public int MACD_Slow
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MACD_M", Order=6, GroupName="Parameters")]
		public int MACD_M
		{ get; set; }
		#endregion

	}
}
