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
namespace NinjaTrader.NinjaScript.Strategies.Numan
{
	public class SimpleWAEentryV2Unlocked : Strategy
	{
		private bool entryOK;
		private bool standardEntry;
		private bool repeatEntry;
		
		private double EntryPrice;
		private double TargetPrice1;
		private double TargetPrice2;
		
		private int zeroBar, currentBar, lastBar;

		private NinjaTrader.NinjaScript.Indicators.Numan.WAE_Mod WAE;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Version 02: All Version 1( Stable - V1Prints) + more";
				Name										= "SimpleWAEentryV2Unlocked";
				Calculate									= Calculate.OnPriceChange;
				EntriesPerDirection							= 3;
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
				Sensitivity					= 300;
				PosQty_1					= 1;
				PosQty_2					= 1;
				OrderDelays					= 5;
				
				entryOK						= false;				
				LongTradeOn					= true;
				ShortTradeOn				= true;
				
				ProfitTargetON				= false;
				MultiProfitTargetON			= true;
				
				ProfitTarget_1		= 20;
				ProfitTarget_2		= 40;
				
				// bar indices
				zeroBar						= 0;
				currentBar					= 0;
				lastBar						= 1;
				
				EntryPrice					= 0;
				TargetPrice1				= 0;
				TargetPrice2				= 0;

			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				WAE				= WAE_Mod(Close, Convert.ToInt32(Sensitivity), 10, true, 9, 30, true, 9, 30, 2, 200);
			}
		}
		
		private bool check_price(MarketPosition M, int P)
		{
			bool result;
				
			switch (M)
			{
				case MarketPosition.Long:
					result = Close[zeroBar] >= ( Position.AveragePrice + P * TickSize );
					BackBrush = Brushes.LightSalmon ;
					Print(string.Format("{0} : (LONG) Entry Price [{1}], Exit at [{2}]",Convert.ToString(Times[0][0]), Position.AveragePrice, ( Position.AveragePrice + P * TickSize ) ));
					break;
				case MarketPosition.Short:
					result = Close[zeroBar] <= ( Position.AveragePrice - P * TickSize );
					BackBrush = Brushes.LightGreen ;
					Print(string.Format("{0} : (SHORT) Entry Price [{1}], Exit at [{2}]",Convert.ToString(Times[0][0]), Position.AveragePrice, ( Position.AveragePrice - P * TickSize ) ));
					break;
				default:
					result = false;
					Print(string.Format("{0} : ==== S K I P ==== : Market is Flat - Please check syntax",Convert.ToString(Times[0][0]) ));
					break;
			}
			return (result);
		}
		
		private bool entryexitClosed()
		{
			bool result;
			
			// Bars since last Entry and Exit OK
			result = ( (BarsSinceEntryExecution() == -1) || (BarsSinceEntryExecution() > OrderDelays) )
				 && ( (BarsSinceExitExecution() == -1) || (BarsSinceExitExecution() > OrderDelays) );
			return result;
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < BarsRequiredToTrade)
				return;

			#region -- 'exits' on Profit Target --
			if (ProfitTargetON)
			{
				// Set 1
				if ( check_price(MarketPosition.Long, ProfitTarget_1) )
				{
					ExitLong(Convert.ToInt32(PosQty_1), @"Target 1", @"Long1");
				}				
				 // Set 2
				if ( PosQty_2 > 0 && check_price(MarketPosition.Long, ProfitTarget_2) )
				{
					ExitLong(Convert.ToInt32(PosQty_2), @"Target 2", @"Long2");
				}
			 	// Set 3					
				if ( check_price(MarketPosition.Short, ProfitTarget_1) )
				{
					ExitShort(Convert.ToInt32(PosQty_1), @"Target 1", @"Short1");
				}				
				 // Set 4
				if ( PosQty_2 > 0 && check_price(MarketPosition.Short, ProfitTarget_2) )
				{
					ExitShort(Convert.ToInt32(PosQty_2), @"Target 2", @"Short2");
				}							
			}
			/// =================================================================
			#endregion
			
			#region -- 'exits' when End limit reached --
			 // Set 5
			if (
				( Position.MarketPosition == MarketPosition.Short )
				 // WAE TrendDown >= 0
				 && ( WAE.TrendDown[1] >= 0 )	// for confirmation, index '[1]' is used instead of '[0]'
				)
			{
				ExitShort(Convert.ToInt32(PosQty_2), @"ExitShort2", @"Short2");
				ExitShort(Convert.ToInt32(PosQty_1), @"ExitShort1", @"Short1");
				BackBrush = Brushes.LightSalmon;
				Print(Convert.ToString(Times[0][0]) + @"-SET 5 -Exit Shorts ASAP >> Trend going up at Reversal");
			}
			
			 // Set 6
			if ((Position.MarketPosition == MarketPosition.Long)
				// WAE TrendUp <= 0 
				&& ((WAE.TrendUp[1] <= 0)	// for confirmation, index '[1]' is used instead of '[0]'
				))
			{
				ExitLong(Convert.ToInt32(PosQty_2), @"ExitLong2", @"Long2");
				ExitLong(Convert.ToInt32(PosQty_1), @"ExitLong1", @"Long1");
				BackBrush = Brushes.SteelBlue;
				Print(Convert.ToString(Times[0][0]) + @"-SET 6  -Exit Longs ASAP >> Trend going down at Reversal");
			}
			/// =================================================================
			#endregion

			// If not the First tick of Bar then many entries will be done in the same bar
			if ( !IsFirstTickOfBar )	
				return;
			
			// Set 7 : Condition check for entry
			entryOK =  (
							( LongTradeOn == true )
							// Market position Flat
							&& ( Position.MarketPosition == MarketPosition.Flat )
							// Bars Since Last Entry and Exit OK
							&& ( entryexitClosed() == true )		 
						);
			
			// Waddah Attar Trend check
			standardEntry 	= entryOK && ( WAE.TrendUp[currentBar] >= WAE.TrendUp[lastBar] ); 
			
			// Waddah Attar explosion is still on.
			repeatEntry 	= entryOK && ( WAE.ExplosionLine[currentBar] >= WAE.ExplosionLine[lastBar] ); 
			
			// Set 8 : Executing the Long entries
			if ( standardEntry )
			{
				if 	( 
					  ( // WAExplosion CrossUp -> Explosion check
						(WAE.TrendUp[currentBar] > WAE.ExplosionLine[currentBar])
						&& (WAE.TrendUp[lastBar] < WAE.ExplosionLine[lastBar]) )
					 
					||( // WAExplosion CrissCrossUp
						(WAE.TrendUp[currentBar] > WAE.ExplosionLine[currentBar])
						&& (WAE.TrendDown[lastBar] < WAE.ExplosionLineDn[lastBar]))
					)
				{
					EnterLong(Convert.ToInt32(PosQty_1), @"Long1");
					EnterLong(Convert.ToInt32(PosQty_2), @"Long2");
					BackBrushAll = Brushes.Chartreuse;
				}
			}
			else if ( repeatEntry )
			{
				EnterLong(Convert.ToInt32(PosQty_1), @"Long1");
				EnterLong(Convert.ToInt32(PosQty_2), @"Long2");
				BackBrushAll = Brushes.LightSalmon;				
			}
			
			
			/// =================================================================

			// Set 9 : ommitted
			
			// Set 10 : Condition check for entry
			entryOK =  (
							( ShortTradeOn == true )
							// Market position Flat
							&& ( Position.MarketPosition == MarketPosition.Flat )
							// Bars Since Last Entry and Exit OK
							&& ( entryexitClosed() )
						);
			
			// Waddah Attar Trend check
			standardEntry 	= entryOK && ( WAE.TrendDown[currentBar] <= WAE.TrendDown[lastBar] ); 
			
			// Waddah Attar explosion is still on.
			repeatEntry 	= entryOK && ( WAE.ExplosionLineDn[currentBar] <= WAE.ExplosionLineDn[lastBar] ); 
			
			// Set 11 : Executing the Short entries
			if ( standardEntry )
			{
				if 	( 
					  ( // WAExplosion CrossDown -> Explosion check
						(WAE.TrendDown[0] < WAE.ExplosionLineDn[0])
				 		&& (WAE.TrendDown[1] > WAE.ExplosionLineDn[1]) )
					 
					||( // WAExplosion CrissCrossDown
						(WAE.TrendDown[0] < WAE.ExplosionLineDn[0])
				 		&& (WAE.TrendUp[1] > WAE.ExplosionLine[1]) )
					)
				{
					EnterShort(Convert.ToInt32(PosQty_1), @"Short1");
					EnterShort(Convert.ToInt32(PosQty_2), @"Short2");
					BackBrushAll = Brushes.LightGreen;
				}
			}
			else if ( repeatEntry )
			{
				EnterShort(Convert.ToInt32(PosQty_1), @"Short1");
				EnterShort(Convert.ToInt32(PosQty_2), @"Short2");
				BackBrushAll = Brushes.LawnGreen;				
			}
			/// =================================================================
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="Sensitivity", Description="Waddah Attar Explosion sensitivity", Order=1, GroupName="Parameters")]
		public int Sensitivity
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="PosQty_1", Order=2, GroupName="Parameters")]
		public int PosQty_1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="PosQty_2", Description="Leave zero if only one position is desired.", Order=3, GroupName="Parameters")]
		public int PosQty_2
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="OrderDelays", Description="Number of bars to wait before new order entry.", Order=4, GroupName="Parameters")]
		public int OrderDelays
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="LongTradeOn", Description="Allow Long trades?", Order=5, GroupName="Parameters")]
		public bool LongTradeOn
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="ShortTradeOn", Description="Allow Short trades? ", Order=6, GroupName="Parameters")]
		public bool ShortTradeOn
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="ProfitTargetON", Description="If you want to set the profit target yourself", Order=7, GroupName="Parameters")]
		public bool ProfitTargetON
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ProfitTarget_1", Order=8, GroupName="Parameters")]
		public int ProfitTarget_1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ProfitTarget_2", Order=9, GroupName="Parameters")]
		public int ProfitTarget_2
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="MultiProfitTargetON", Description="If multiple target acquisition is intended !!", Order=12, GroupName="Parameters")]
		public bool MultiProfitTargetON
		{ get; set; }
		#endregion

	}
}
