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
namespace NinjaTrader.NinjaScript.Strategies.YouTuber.DRE
{
	public class DreReversalStrategy : Strategy
	{
		#region Reversal pattern
		
		private double percentageCalc, priceCalc, tickCalc, candleBarOffset;
		private bool currentBullRev, currentBearRev;
		
		#endregion

		#region -- Entry Offset --
		
		private double entryAreaLong, entryAreaShort;
		private double percentageCalcEntry, priceCalcEntry, tickCalcEntry, candleBarOffsetEntry;
		private double enterLong, enterShort;
		
		#endregion

		#region -- Stop Offset --
		
		private double stopAreaLong, stopAreaShort;
		private double percentageCalcStop, priceCalcStop, tickCalcStop, candleBarOffsetStop;
		private double stopLong, stopShort;
		
		#endregion
		
		#region -- Breakeven --
		
		private double 	breakevenTriggerLong, breakevenTriggerShort;
		private bool 	myFreeBELong, myFreeBEShort;
		private double 	breakevenLong, breakevenShort;
		
		#endregion
		
		#region -- Trailstop --
		
		private double trailAreaLong, trailAreaShort;
		private double percentageCalcTrail, priceCalcTrail, tickCalcTrail, candleBarOffsetTrail;
		private double trailLong, trailShort;
		
		private double trailTriggerLong, trailTriggerShort;
		
		private bool myFreeTrail, trailTriggeredCandle;
		
		#endregion
		
		private bool countOnce;
		private int currentCount;
		
		private bool myFreeTradeLong, myFreeTradeShort;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "DREReversalStrategy";
				Calculate									= Calculate.OnPriceChange;
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
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				
				#region Parameter defaults
				
				// Reversal Candle Offset
				PriceOffset									= 0.01;
				PercentageOffset							= 0;
				TickOffset									= 1;
				
				//Entry Offset
				PriceOffsetEntry							= 0.01;
				PercentageOffsetEntry						= 0;
				TickOffsetEntry								= 1;
				
				//Stop Offset
				PriceOffsetStop								= 0.01;
				PercentageOffsetStop						= 0;
				TickOffsetStop								= 1;
				
				// Trail Offset
				PriceOffsetTrail							= 0.01;
				PercentageOffsetTrail						= 0;
				TickOffsetTrail								= 1;
				TrailTriggerAmount							= 20;
				
				// Breakeven
				BreakevenTriggerAmount						= 10;
				
				// Daily PnLs
				DailyProfitLimit							= 1000;
				DailyLossLimit								= -500;
				X											= 5;
				Start										= DateTime.Parse("09:30", System.Globalization.CultureInfo.InvariantCulture);
				End											= DateTime.Parse("16:00", System.Globalization.CultureInfo.InvariantCulture);
				
				// Position Size
				PositionSize								= 2;
				
				ProfitTargetTicks							= 50;
				
				
				StopLoss									= true;
				ProfitTarget								= true;
				SetBreakeven								= true;
				SetTrail									= true;
				
				countOnce									= true;
				
				SystemPrint									= true;
				
				#endregion
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{
				ClearOutputWindow();
			}
		}

		protected override void OnPositionUpdate(Cbi.Position position, double averagePrice, 
			int quantity, Cbi.MarketPosition marketPosition)
		{
			if (Position.Quantity == PositionSize)
			{
				currentCount++;
				
				if (SystemPrint)
				{
					Print("currentCount " + currentCount );
				}
			}
			
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				myFreeTrail = false;
				
		Print("myFree Trail ON Position update  " + myFreeTrail);
			}
		}

		protected override void OnBarUpdate()
		{
			#region if Returns
			
			if (CurrentBars[0] < BarsRequiredToTrade)
			{
				return;
			}
			
			if (State != State.Realtime)
			{
				return;
			}
			
			if (Bars.BarsSinceNewTradingDay < 1)
			{
				return;
			}
			
			if ((SystemPerformance.RealTimeTrades.TradesPerformance.Currency.CumProfit > DailyProfitLimit) || (SystemPerformance.RealTimeTrades.TradesPerformance.Currency.CumProfit < DailyLossLimit))
			{
				return;
			}
			
			if (Bars.IsFirstBarOfSession)
			{
				currentCount = 0;
			}
			
			#endregion

			if (countOnce)
			{
				#region Reversal Offset
				
				percentageCalc	= ((High[2] - Low[2])) * PercentageOffset;
				priceCalc		= PriceOffset;
				tickCalc		= TickOffsetEntry * TickSize;
				
				candleBarOffset = Math.Max(percentageCalc, Math.Max(priceCalc, tickCalc));
				
				if (SystemPrint)
				{
					Print("percentageCalc " + percentageCalc);
					Print("priceCalc " + priceCalc);
					Print("tickCalc " + tickCalc);
		
					Print("candleBarOffset " + candleBarOffset);				
				}
				
				#endregion
				
				#region Entry Offset
				
				entryAreaLong		= High[1];
				entryAreaShort		= Low[1];
				
				percentageCalcEntry	= ((High[1] - Low[1])) * PercentageOffsetEntry;
				priceCalcEntry		= PriceOffsetEntry;
				tickCalcEntry		= TickOffsetEntry * TickSize;
				
				candleBarOffsetEntry = Math.Max(percentageCalcEntry, Math.Max(priceCalcEntry, tickCalcEntry));
				
				enterLong	= entryAreaLong	 + candleBarOffsetEntry;
				enterShort	= entryAreaShort - candleBarOffsetEntry;
				
				if (SystemPrint)
				{
					Print("percentageCalcEntry " + percentageCalcEntry);
					Print("priceCalcEntry " + priceCalcEntry);
					Print("tickCalcEntry " + tickCalcEntry);
		
					Print("candleBarOffsetEntry " + candleBarOffsetEntry);				
				}
				
				#endregion
				
				#region Stop Offset
				
				stopAreaLong		= Low[1];
				stopAreaShort		= High[1];
				
				percentageCalcStop	= ((High[1] - Low[1])) * PercentageOffsetStop;
				priceCalcStop		= PriceOffsetStop;
				tickCalcStop		= TickOffsetStop * TickSize;
				
				candleBarOffsetStop = Math.Max(percentageCalcStop, Math.Max(priceCalcStop, tickCalcStop));
				
				stopLong	= stopAreaLong	- candleBarOffsetStop;
				stopShort	= stopAreaShort + candleBarOffsetStop;
				
				if (SystemPrint)
				{
					Print("percentageCalcStop " + percentageCalcStop);
					Print("priceCalcStop " + priceCalcStop);
					Print("tickCalcStop " + tickCalcStop);
		
					Print("candleBarOffsetStop " + candleBarOffsetStop);				
				}
				
				#endregion
				
				#region Reversal Logics
				
				currentBullRev = (
									((Low[1] + (candleBarOffset)) <= Low[2])	// this bar-low is at the specified distance below the the previous bar-low
									&& (Close[1] >= Close[2])		//  the green bar closes above the previous red bar close - confirmed!
									&& (Open[1] < Close[1])			//  this is a GREEN bar -  confirmed!
									&& (Open[2] > Close[2])			//  this is a RED bar - confirmed!
								);
				
				currentBearRev = (
									((High[1] - (candleBarOffset)) >= High[2]) 	// this is a bar-high above the specified distance of the previous bar-high
									&& (Close[1] <= Close [2])		// this red bar closes above the previous green bar's close - confirmed!
									&& (Open[1] > Close[1])			// this is a RED bar - confirmed!
									&& (Open[2] < Close[2])			// this is a GREEN bar - confirmed !
								);
				
				#endregion
				
				countOnce = false;				
			}
			
			if (IsFirstTickOfBar)
			{
				countOnce = true;
				
				if (Position.MarketPosition != MarketPosition.Flat)
				{
					myFreeTrail = true;
			Print("myFree Trail at First Tick " + myFreeTrail);
				}
			}			
						
			#region Bull Trade
			
			if (
				(currentBullRev)
				&& (Close[0] >= enterLong)
				&& (Position.MarketPosition == MarketPosition.Flat)
				&& ((Time[0].TimeOfDay >= Start.TimeOfDay) && (Time[0].TimeOfDay <= End.TimeOfDay))
				&& (currentCount < X)
				
				&& (BarsSinceExitExecution("MyStopLong") > 1 || BarsSinceExitExecution("MyStopLong") == -1)
				&& (BarsSinceExitExecution("MyTargetLong") > 1 || BarsSinceExitExecution("MyTargetLong") == -1)
				)
			{
				EnterLong(PositionSize, "MyEntryLong");
				
				myFreeTradeLong = true;
			}
			
			if (Position.MarketPosition == MarketPosition.Long && myFreeTradeLong == true)
			{
				if (StopLoss)
				{
					ExitLongStopMarket(0, true, Position.Quantity, stopLong, "MyStopLong", "MyEntryLong");
				}
				
				if (ProfitTarget)
				{
					ExitLongLimit(0, true, Position.Quantity, Position.AveragePrice + (TickSize * ProfitTargetTicks), "MyTargetLong", "MyEntryLong");
				}
				
				breakevenTriggerLong = Position.AveragePrice + (TickSize * BreakevenTriggerAmount);
				myFreeBELong = true;
				
				trailTriggerLong = Position.AveragePrice + (TickSize * TrailTriggerAmount);
				myFreeTrail = true;
				
				myFreeTradeLong = false;
			}
			
			#endregion			
			
			#region Bear Trade
			
			if (
				(currentBearRev)
				&& (Close[0] <= enterShort)
				&& (Position.MarketPosition == MarketPosition.Flat)
				&& ((Time[0].TimeOfDay >= Start.TimeOfDay) && (Time[0].TimeOfDay <= End.TimeOfDay))
				&& (currentCount < X)
				
				&& (BarsSinceExitExecution("MyStopShort") > 1 || BarsSinceExitExecution("MyStopShort") == -1)
				&& (BarsSinceExitExecution("MyTargetShort") > 1 || BarsSinceExitExecution("MyTargetShort") == -1)
				)
			{
				EnterShort(PositionSize, "MyEntryShort");
				
				myFreeTradeShort = true;
			}
			
			if (Position.MarketPosition == MarketPosition.Short && myFreeTradeShort == true)
			{
				if (StopLoss)
				{
					ExitShortStopMarket(0, true, Position.Quantity, stopShort, "MyStopShort", "MyEntryShort");
				}
				
				if (ProfitTarget)
				{
					ExitShortLimit(0, true, Position.Quantity, Position.AveragePrice + (TickSize * ProfitTargetTicks), "MyTargetShort", "MyEntryShort");
				}
				
				breakevenTriggerShort = Position.AveragePrice - (TickSize * BreakevenTriggerAmount);
				myFreeBEShort =  true;
				
				trailTriggerShort = Position.AveragePrice - (TickSize * TrailTriggerAmount);
				myFreeTrail = true;
								
				myFreeTradeShort = false;
			}
			
			#endregion
		}

		protected override void OnMarketData(MarketDataEventArgs marketDataUpdate)
		{
			if (marketDataUpdate.MarketDataType == MarketDataType.Last)
			{
				if (SetBreakeven)
				{
				
					#region Long Breakeven
					
					if (
						(Position.MarketPosition == MarketPosition.Long)
						&& (marketDataUpdate.Price >= breakevenTriggerLong)
						&& (myFreeBELong ==  true)
						)
					{
						breakevenLong = Position.AveragePrice;
						ExitLongStopMarket(0, true, Position.Quantity, breakevenLong, "myStopLong", "myEntryLong");
						myFreeBELong = false;
					}
					
					#endregion
					
					#region Short Breakeven
				
					if (
						(Position.MarketPosition == MarketPosition.Short)
						&& (marketDataUpdate.Price >= breakevenTriggerShort)
						&& (myFreeBEShort ==  true)
						)
					{
						breakevenShort = Position.AveragePrice;
						ExitShortStopMarket(0, true, Position.Quantity, breakevenShort, "myStopShort", "myEntryShort");
						myFreeBEShort = false;
					}
				
					#endregion
					
				}
				
				if (SetTrail)
				{
					if (
						(myFreeTrail == true)
						&& ( ((Position.MarketPosition == MarketPosition.Short) && (marketDataUpdate.Price <= trailTriggerShort))
							|| ((Position.MarketPosition == MarketPosition.Long) && (marketDataUpdate.Price >= trailTriggerLong)) )
						)
					{
						#region Trail Offset
						
						trailAreaLong		= High[1];
						trailAreaShort		= Low[1];
						
						percentageCalcTrail	= ((High[2] - Low[2])) * PercentageOffsetTrail;
						priceCalcTrail		= PriceOffsetTrail;
						tickCalcTrail		= TickOffsetTrail * TickSize;
						
						candleBarOffsetTrail = Math.Max(percentageCalcTrail, Math.Max(priceCalcTrail, tickCalcTrail));
						
						trailLong	= trailAreaLong	 + candleBarOffsetTrail;
						trailShort	= trailAreaShort - candleBarOffsetTrail;
						
						#region Prints
						if (SystemPrint)
						{
							Print("Current Trail Percent Offset is :  " + percentageCalcTrail);
							Print("Current Trail Price Offset is :  " + priceCalcTrail);
							Print("Cuirrent Trail Tick Offset is :  " + tickCalcTrail);
				
							Print("Current Trail Highest Offset Selected is :  " + candleBarOffsetTrail);	
							
							Print("Trail Long Price is:  " + trailLong);
							Print("Trail Short price is :  " + trailShort);
							
							Print("myFree Trail Offset " + myFreeTrail);
						}
						#endregion
						
						#endregion
						
						trailTriggeredCandle = true;
						myFreeTrail = false;
						
							Print("myFree Trail Offset After " + myFreeTrail);							
					}
					
					#region Long Trail Stop
					
					if (
						(Position.MarketPosition == MarketPosition.Long)
						&& ( trailTriggeredCandle )
						&& (Low[1] > Low[2])
						)
					{
						ExitLongStopMarket(0, true, Position.Quantity, trailLong, "MyStopLong", "myEntryLong");
						trailTriggeredCandle = false;
					}
					
					#endregion						
					
					#region Short Trail Stop
					
					if (
						(Position.MarketPosition == MarketPosition.Short)
						&& ( trailTriggeredCandle )
						&& (High[1] < High[2])
						)
					{
						ExitShortStopMarket(0, true, Position.Quantity, trailShort, "MyStopShort", "myEntryShort");
						trailTriggeredCandle = false;
					}
					
					#endregion
				}
			
			}
		}

		#region Properties
		[NinjaScriptProperty]
		[Display(Name="Price Offset", Order=101, GroupName="1. RFeversal Candle Offset")]
		public double PriceOffset
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Percentage Offset", Order=102, GroupName="1. RFeversal Candle Offset")]
		public double PercentageOffset
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Tick Offset", Order=103, GroupName="1. RFeversal Candle Offset")]
		public int TickOffset
		{ get; set; }
		
		//

		[NinjaScriptProperty]
		[Display(Name="Pric eOffset Entry", Order=201, GroupName="2. Entry Offset")]
		public double PriceOffsetEntry
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Percentage Offset Entry", Order=202, GroupName="2. Entry Offset")]
		public double PercentageOffsetEntry
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Tick Offset Entry", Order=203, GroupName="2. Entry Offset")]
		public int TickOffsetEntry
		{ get; set; }
		
		//

		[NinjaScriptProperty]
		[Display(Name="Set Stop Loss", Order=300, GroupName="3. Stop Offset")]
		public bool StopLoss
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Price Offset Stop", Order=301, GroupName="3. Stop Offset")]
		public double PriceOffsetStop
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Percentage Offset Stop", Order=302, GroupName="3. Stop Offset")]
		public double PercentageOffsetStop
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Tick Offset Stop", Order=303, GroupName="3. Stop Offset")]
		public int TickOffsetStop
		{ get; set; }
		
		//
		
		[NinjaScriptProperty]
		[Display(Name="Set Profit Target", Order=400, GroupName="4. Profit Target")]
		public bool ProfitTarget
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Position Size", Order=401, GroupName="4. Profit Target")]
		public int PositionSize
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Final Profit", Order=402, GroupName="4. Profit Target")]
		public int ProfitTargetTicks
		{ get; set; }
		
		//

		[NinjaScriptProperty]
		[Display(Name="Set Breakeven", Order=500, GroupName="5. Breakeven")]
		public bool SetBreakeven
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Breakeven Trigger Amount", Order=501, GroupName="5. Breakeven")]
		public int BreakevenTriggerAmount
		{ get; set; }
		
		//

		[NinjaScriptProperty]
		[Display(Name="Set Trail", Order=600, GroupName="6A. Trail Stop")]
		public bool SetTrail
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Trail Trigger Amount", Order=601, GroupName="6A. Trail Stop")]
		public int TrailTriggerAmount
		{ get; set; }
		
		//
		
		[NinjaScriptProperty]
		[Display(Name="Price Offset Trail", Order=602, GroupName="6B. Trail Stop")]
		public double PriceOffsetTrail
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Percentage Offset Trail", Order=603, GroupName="6B. Trail Stop")]
		public double PercentageOffsetTrail
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Tick Offset Trail", Order=604, GroupName="6B. Trail Stop")]
		public int TickOffsetTrail
		{ get; set; }
		
		//

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="Start", Order=701, GroupName="7. Order Limits")]
		public DateTime Start
		{ get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="End", Order=702, GroupName="7. Order Limits")]
		public DateTime End
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Daily Profit Limit", Order=703, GroupName="7. Order Limits")]
		public int DailyProfitLimit
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Daily Loss Limit", Order=704, GroupName="7. Order Limits")]
		public int DailyLossLimit
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Max Trade Count", Order=705, GroupName="7. Order Limits")]
		public int X
		{ get; set; }

		//

		[NinjaScriptProperty]
		[Display(Name="System Print", Order=26, GroupName="Parameters")]
		public bool SystemPrint
		{ get; set; }
		#endregion
	}
}
