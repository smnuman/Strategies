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

namespace NinjaTrader.NinjaScript.Strategies.TradeSimpleStrategies
{
	public class PriceActionV4_1: Strategy
	//aspr
	{
		private const string SystemVersion 					= " V 4.1";
        private const string SystemName 					= "Price Action Order Entry";
		
	#region Private Variables
		
		#region Strings
		private const string LongTargetExit = "ExitTargetLong";
		private const string ShortTargetExit = "ExitTargetShort";
		
		private const string LongStopExit = "ExitStopLong";
		private const string ShortStopExit = "ExitStopShort";
		
		
		private const string LongLine = "LongEntryLine";
		private const string ShortLine = "ShortEntryLine";
		
		///------------------------------------------------------------------------------------------------------------------------------------------------
		
		private const string LongPos = "Open Long";
        private const string ShortPos = "Open Short";
		
        private const string ProfitLong1 = "Profit Long 1";
        private const string StopLong = "Stop Long";
		
        private const string ProfitShort1 = "Profit Short 1";
        private const string StopShort = "Stop Short";
    
        
		private const string BreakEvenShort = "BE Short";
        private const string BreakEvenLong = "BE Long";
		#endregion
	///-----------------------------------------------	
		#region Target Variables
		
		private TargetCalcRR	targetType	= TargetCalcRR.CandleRR;
		
		private double longStopPrice;
		private double longStopPrice2;
		private double longStopPrice3;
		
		private double longStopTrigger;
		private double longStopTrigger2;
		private double longStopTrigger3; 
		//TS

		private double longTargetPrice;
		private double longEntryPrice;
	///-----------------------------------------------	
		private double StopPriceShort;
		private double StopPriceShort2;
		private double StopPriceShort3;
		
		private double StopTriggerShort;
		private double StopTriggerShort2;
		private double StopTriggerShort3;
		
		private double TargetPriceShort;
		private double EntryPriceShort;
		
		#endregion
	///-----------------------------------------------		
		#region Entry/Stop Offset Variables
		
		private double entryOffsetPrice				= 0.03;
		private double entryOffsetPercentage 		= 0.10;
		
		private double stopOffsetPrice 				= 0.03;
		private double stopOffsetPercentage 		= 0.10;
		
		#endregion
	///-----------------------------------------------	
		#region EntryArea Variables
		
		private EntryArea	entryType	= EntryArea.HighLow;
		
		private double entryAreaLong;
		private double longOffsetBE;
		
		
		private double entryAreaShortpd;
		private double shortOffsetBE;
		
		
		private double longEntry;
		private double shortEntry;
		
		private double entryArea;
		private double entryAreaShort;
		
		private bool enterMarket;
		
		#endregion
	///-----------------------------------------------	
		#region Share Calc Variables
		
		private double maxLossPerTrade 				= 100;
		
		private ShareSizeRR	rrType	= ShareSizeRR.CandleRR; //ActualRR
		
		private bool myCandleRR;
		private bool myActualRR;
		
		private double userRiskA;
		private double userRiskF;
		private int positionSize;
		
		private double myDbl;
		
		private double offsetAdded;
		#endregion
	///-----------------------------------------------
		#region Profit Targets/Management Variables
		
		private double rrTarget 					= 5;
		private double breakevenTarget				= 0.7; //0.728;
		
		
		private bool beTarget1;
		private bool beTarget2;
		private bool beTarget3;
		private bool isTrailShort;
		
		
		private bool beLongTarget1;
		private bool beLongTarget2;
		private bool beLongTarget3;
		private bool isTrailLong;
		
		
		private double shortTarget3;
		private double longTarget3;
		
		private double breakevenTarget2 			= 1.0; //1.178;
		
		private double breakevenTarget3 			= 1.5;
		private double target3StopSet				= 1.0;	
		
		
		// Use for Long and Short
		private double trailTrigger					= 2.0;	//When the Trail is initially triggered
		private double trailFrequency 				= 0.3; //Triggers after this many R'S
		private double trailSize					= 0.3; //Sets Stops high by this much after Frequency is Triggered
		
		
		//Longs Only - Trail
		private double trailTriggerLong;
		private double trailFrequencyLong;
		private double trailSizeLong;
		private double longStopTrail;
		

		//Shorts Only - Trail
		private double trailTriggerShort;
		private double trailFrequencyShort;
		private double trailSizeShort;
		private double trailStopShort; 
		
		//Longs Only
		private bool triggered1;
		private bool triggered2;
		private bool triggered3;
		
		//Shorts only
		private bool triggeredShort1;
		private bool triggeredShort2;
		private bool triggeredShort3;
		
		//Use for Long and Short
		private bool userTarget1;
		private bool userTarget2;
		private bool userTarget3;
		private bool userTrailTarget;
		
		private double maxStopShort; 
		private double trailStopOffset; //short
		
		private double longTrailOffset;
		
		private TrailStop		trailType	= TrailStop.CloseCandle;
		
	///-----------------------------------------------
		
		private bool myFreeTradeLong;
		private bool myFreeTradeShort;
		
		private bool profitTarget;
		private bool stopLoss;
		
		
		private bool playbackMode;
		
		
		
		private double longEntryPercent;
		private double longOffset;
		private double shortEntryPercent;
		private double shortEntryOffset;
		private double shortEntryPrice;
		#endregion
	///-----------------------------------------------	
		#region Enums
		
		public enum EntryArea
		{
			HighLow,
			Close,
			Market,
		}
		
		public enum ShareSizeRR
	{
		ActualRR,
		CandleRR,	
	}

	public enum TargetCalcRR
	{
		ActualRR,
		CandleRR,
	}
	
	public enum TrailStop
	{
		CustomTrail,
		CloseCandle,
	}
		
		#endregion	
	///------------------------------------------------------------------------------------------------------------------------------------------------
		#region Button Variables
	
		private bool longButtonClicked;
		private bool shortButtonClicked;
		private System.Windows.Controls.Button longButton;
		private System.Windows.Controls.Button shortButton;
		private System.Windows.Controls.Grid myGrid;
		
		#endregion
	
		#endregion
			

		protected override void OnStateChange()
		{
			
			if (State == State.SetDefaults)
			{
				Description							= @"";
				Name								= SystemName + SystemVersion;
				Calculate							= Calculate.OnPriceChange;
				EntriesPerDirection					= 1;
				EntryHandling						= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy		= true;
				ExitOnSessionCloseSeconds			= 30;
				IsFillLimitOnTouch					= false;
				MaximumBarsLookBack					= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution					= OrderFillResolution.Standard;
				Slippage							= 0;
				StartBehavior						= StartBehavior.WaitUntilFlat;
				TimeInForce							= TimeInForce.Day;
				TraceOrders							= false;
				RealtimeErrorHandling				= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling					= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade					= 20;
				
				IsInstantiatedOnEachOptimizationIteration	= true;
				
				enterMarket									= false;
				
				userTarget1									= false;
				userTarget2 								= false;
				userTarget3 								= false;
				userTrailTarget 							= false;
				
				profitTarget 								= true;
				stopLoss 									= true;
				
				playbackMode								= false;
				
		
			}
			
			
			else if (State == State.Configure)
			{
			
			}

			
			else if (State == State.DataLoaded)
			{				
				
			ClearOutputWindow();
				
				myDbl = Instrument.MasterInstrument.PointValue * Instrument.MasterInstrument.TickSize;
				
			}
			
			#region LoadButtons
			
			else if (State == State.Historical)
			{
				if (UserControlCollection.Contains(myGrid))
					return;
				
				Dispatcher.InvokeAsync((() =>
				{
					myGrid = new System.Windows.Controls.Grid
					{
						Name = "MyCustomGrid", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top
					};
					
					System.Windows.Controls.ColumnDefinition column1 = new System.Windows.Controls.ColumnDefinition();
					System.Windows.Controls.ColumnDefinition column2 = new System.Windows.Controls.ColumnDefinition();
					
					myGrid.ColumnDefinitions.Add(column1);
					myGrid.ColumnDefinitions.Add(column2);
					
					longButton = new System.Windows.Controls.Button
					{
						Name = "LongButton", Content = "LONG", Foreground = Brushes.White, Background = Brushes.Green
					};
					
					shortButton = new System.Windows.Controls.Button
					{
						Name = "ShortButton", Content = "SHORT", Foreground = Brushes.Black, Background = Brushes.Red
					};
					
					longButton.Click += OnButtonClick;
					shortButton.Click += OnButtonClick;
					
					System.Windows.Controls.Grid.SetColumn(longButton, 0);
					System.Windows.Controls.Grid.SetColumn(shortButton, 1);
					
					myGrid.Children.Add(longButton);
					myGrid.Children.Add(shortButton);
					
					UserControlCollection.Add(myGrid);
				}));
			}
			else if (State == State.Terminated)
			{
				Dispatcher.InvokeAsync((() =>
				{
					if (myGrid != null)
					{
						if (longButton != null)
						{
							myGrid.Children.Remove(longButton);
							longButton.Click -= OnButtonClick;
							longButton = null;
						}
						if (shortButton != null)
						{
							myGrid.Children.Remove(shortButton);
							shortButton.Click -= OnButtonClick;
							shortButton = null;
						}
					}
				}));
			}	
			#endregion
			
		}
			#region OnPositionUpdate 
		
		protected override void OnPositionUpdate(Position position, double averagePrice, int quantity, MarketPosition marketPosition)
		{

			if (Position.MarketPosition == MarketPosition.Flat)
				
			{

				beTarget1 = false;
				beTarget2 = false;
				beTarget3 = false;
				isTrailShort = false;
				
				beLongTarget1 = false;
				beLongTarget2 = false;
				beLongTarget3 = false;
				isTrailLong   = false;	
					
			}
			
		
		}
	#endregion
		
		
		protected override void OnBarUpdate()
		{
			
			if (CurrentBars[0] < 2)
				return;
			
			if (BarsInProgress != 0) 
				return;
			
			if (State != State.Realtime ) 
				return;
			
			if (playbackMode == true)
			{
			if (Position.MarketPosition != MarketPosition.Flat)
				return;
			
			if((!longButtonClicked) && (!shortButtonClicked))
				return;
			}
			
			
		#region Entry Calculations
			
			#region Long Entry Calcs
			///Long Entry
			if(longButtonClicked)// && (IsFirstTickOfBar))
			{
				
			switch(entryType)
				
				{
					
			case EntryArea.HighLow:
					{
						
				 	entryArea = High[1];
						
					longEntryPercent = (High[1] - Low[1]) * entryOffsetPercentage;
			 		longEntryPrice = entryOffsetPrice;
			
					longOffset = Math.Max(longEntryPercent, longEntryPrice);
			
			 		//double longEntry = (entryArea +  longOffset);
					longEntry = (entryArea +  longOffset);
			
					//Print("Long Entry is: " + longEntry);
					//Print("entryArea: " + entryArea);
						
					}
					break;
				
			case EntryArea.Close:
					{
						
				 	entryArea = Close[1];
						
					longEntryPercent = (High[1] - Low[1]) * entryOffsetPercentage;
			 		longEntryPrice = entryOffsetPrice;
			
					longOffset = Math.Max(longEntryPercent, longEntryPrice);
			
			 		//double longEntry = (entryArea +  longOffset);
					longEntry = (entryArea +  longOffset);
			
					//Print("Long Entry is: " + longEntry);
					//Print("entryArea: " + entryArea);
						
					}
					break;
					
			case EntryArea.Market:
					{
					enterMarket = true;
					}
					break;
					
					
				}
				
				}
			
			#endregion
			
			#region Short Entry Calcs
			
			if(shortButtonClicked)// && (IsFirstTickOfBar))
			{
		
			switch(entryType)
				
				{
				
			case EntryArea.HighLow:
					{
					
					entryAreaShort = Low[1];
					
					shortEntryPercent = (High[1] - Low[1]) * entryOffsetPercentage;
					shortEntryPrice = entryOffsetPrice;
			
			
			 		shortEntryOffset = Math.Max(shortEntryPercent, shortEntryPrice);
			
					//double shortEntry = (entryAreaShort - shortEntryOffset);
					shortEntry = (entryAreaShort - shortEntryOffset);
					
					//Print("Short Entry is: " + shortEntry);
							
					}
					break;
				
			case EntryArea.Close:
					{
				
					entryAreaShort = Close[1];
						
					shortEntryPercent = (High[1] - Low[1]) * entryOffsetPercentage;
					shortEntryPrice = entryOffsetPrice;
			
			
			 		shortEntryOffset = Math.Max(shortEntryPercent, shortEntryPrice);
			
					//double shortEntry = (entryAreaShort - shortEntryOffset);
					shortEntry = (entryAreaShort - shortEntryOffset);
					
					//Print("Short Entry is: " + shortEntry);
					}
					break;
					
			case EntryArea.Market:
					{
					enterMarket = true;
					}
					break;
					
				}	
				
			}
			
			#endregion
				
		#endregion
		

			#region LongButtonLogic
			
			if  (
					(longButtonClicked)
				
						&& (Position.MarketPosition == MarketPosition.Flat)
						&& (PositionAccount.MarketPosition == MarketPosition.Flat)
				
							&& (Close[0] > longEntry)
								&& (enterMarket == false)
				)
			{
				
			#region Share Size Calc
				
			double offsetPercent = (High[1] - Low[1]) * entryOffsetPercentage;
			double offsetPrice = entryOffsetPrice;
			
			double offsetAdded = Math.Max(offsetPercent, offsetPrice);
			
			double userRiskA = maxLossPerTrade/( (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) / TickSize) * myDbl); // Actual RR
			
			//--------------
			double userRiskF = maxLossPerTrade/( ((High[1]-Low[1]) / TickSize) * myDbl); // Candle size RR

			
			
			switch (rrType)
			{
				// Calculates Shares with Offset included. 
				case ShareSizeRR.ActualRR:
				{
				
					positionSize = (Convert.ToInt32(userRiskA));
					break;
				}
				
			// Calculates Shares with only Candle Size
				case ShareSizeRR.CandleRR:
				{
					
					positionSize = (Convert.ToInt32(userRiskF));
					break;
					
				}
			
			 }
			Print("Position Size is : " + positionSize);
				
		#endregion
			
			#region Long Entry/Stop Calc
			///Long Entry Logic

		
			///Long Stop
			double percentStopLong = (High[1] - Low[1]) * stopOffsetPercentage;	
			double priceStopLong =  stopOffsetPrice;
				
			
			double maxStopLong = Math.Max(percentStopLong, priceStopLong);
				
			//Changes Initial Stop Price
			longStopPrice = (Low[1] - maxStopLong);
			
			
			entryAreaLong = entryArea;
			longOffsetBE = maxStopLong;
			
			#endregion
				
			#region Long Profit Target Calc
					switch (targetType)
			{
		
		///ActualRR Initial Final Target	
			// Calculates Shares with Offset included. 
				case TargetCalcRR.ActualRR:
				{
				
					longTargetPrice = (High[1] + offsetAdded) + (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * rrTarget);
								
				}
			
				break;
	
		///CandleRR Initial Final Target		
			// Calculates Targets with only Candle Size
				case TargetCalcRR.CandleRR:
				{
					
					longTargetPrice = High[1] + ((High[1] - Low[1]) * rrTarget);
		
				}
				
				break;
			}
				#endregion
			
			#region EnterLong
			
				EnterLong(positionSize, "MyEntryLong");
			
				myFreeTradeLong = true;
				
   			#endregion	
			
			#region Target Calculations
			
			switch (targetType)
			{
	///Actual RR B/E and Trail
				// Calculates Targets with Offset included.
				case TargetCalcRR.ActualRR:
			{
				
				//This will trigger when the Trail Stop will move up
				longStopTrigger = ( (High[1] + offsetAdded) + ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * breakevenTarget);
				
				longStopTrigger2 = ( (High[1] + offsetAdded) + ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * breakevenTarget2) ;
				
				longStopTrigger3 = ( (High[1] + offsetAdded) + ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * breakevenTarget3) ;
				
				longTarget3 = ( (High[1] + offsetAdded) + ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * target3StopSet);
				
				
			//----------------------------------------------------------------------------	Trail Stop Targets	
				//Trigger Price to activate Trail
				trailTriggerLong = ( (High[1] + offsetAdded) + ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * trailTrigger	); 
				
				//Frequency in R's
				trailFrequencyLong = ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * trailFrequency;
				//Set stop Loss at Current price - trailFrequency
				
				//used for Stops
				trailSizeLong = ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * trailSize;
				
				longStopTrail = (trailTriggerLong - trailSizeLong);
				
				
				
			}
			break;
			
 	///Candle RR B/E and Trail
				// Calculates Targets based on candle size.
				case TargetCalcRR.CandleRR:
			{
				//This will trigger when the Trail Stop will move up
				longStopTrigger = ( High[1] + (High[1] - Low[1]) * breakevenTarget);
				
				longStopTrigger2 = ( High[1] + (High[1] - Low[1]) * breakevenTarget2) ;
				
				longStopTrigger3 = ( High[1] + (High[1] - Low[1]) * breakevenTarget3) ;
				
				longTarget3 = (High[1] + (High[1] - Low[1]) * target3StopSet);
				
			//----------------------------------------------------------------------------	Trail Stop Targets	
				//Trigger Price to activate Trail
				trailTriggerLong = (High[1] + (High[1] - Low[1]) * trailTrigger	); 
				
				//Frequency in R's
				trailFrequencyLong = (High[1] - Low[1]) * trailFrequency;
				//Set stop Loss at Current price - trailFrequency
				
				//used for Stops
				trailSizeLong = (High[1] - Low[1]) * trailSize;
				
				longStopTrail = (trailTriggerLong - trailSizeLong);
					
			}
				break;
			}
			
				}
			#endregion
			
			#region Stop/Profit Set
			
			if (Position.MarketPosition == MarketPosition.Long && myFreeTradeLong == true)
			{
				if(stopLoss)
				{
					ExitLongStopMarket(0, true, Position.Quantity, longStopPrice, "MyStopLong", "MyEntryLong");
				}
				
				if(profitTarget)
				{
					ExitLongLimit(0, true, Position.Quantity, longTargetPrice, "MyTargetLong", "MyEntryLong");
				}
				
				myFreeTradeLong = false;
			}
			
			#endregion
		
			
			#region DrawLine LongEntry	
			if (
			 	(longButtonClicked)
					&& (Close[0] < longEntry)
				)
				
				{
					Draw.ExtendedLine(this, LongLine, 10, longEntry, 0, longEntry, Brushes.LimeGreen, DashStyleHelper.Dot, 2);
					//Print("Long Entry Price is: " + longEntry);
				}
			
			else if (!longButtonClicked)
				
				{
					RemoveDrawObject(LongLine);
				}
			
			#endregion
				
				
				#endregion
							
	 		#region ShortButtonLogic
			
				if (
						(shortButtonClicked)
				
							&& (Position.MarketPosition == MarketPosition.Flat)
							&& (PositionAccount.MarketPosition == MarketPosition.Flat)
					
								&& (Close[0] < shortEntry)
									&& (enterMarket == false)
									
					)
			{
			
			#region Share size Calc
				
			double offsetPercent = (High[1] - Low[1]) * entryOffsetPercentage;
			double offsetPrice = entryOffsetPrice;
			
			double offsetAdded = Math.Max(offsetPercent, offsetPrice);
			
			double userRiskA = maxLossPerTrade/( (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) / TickSize) * myDbl); // Actual RR
			
			//--------------
			double userRiskF = maxLossPerTrade/( ((High[1]-Low[1]) / TickSize) * myDbl); // Candle size RR

			
			
			switch (rrType)
			{
				// Calculates Shares with Offset included. 
				case ShareSizeRR.ActualRR:
				{
				
					positionSize = (Convert.ToInt32(userRiskA));
					break;
				}
				
			// Calculates Shares with only Candle Size
				case ShareSizeRR.CandleRR:
				{
					
					positionSize = (Convert.ToInt32(userRiskF));
					break;
					
				}
			
			 }
			Print("Position Size is : " + positionSize);
				
				#endregion
				
			#region Short Entry/Stop Calc
			
			
			///Short Stop 
			double percentStopShort = (High[1] - Low[1]) * stopOffsetPercentage;
			double priceStopShort = stopOffsetPrice;
				
			maxStopShort = Math.Max(percentStopShort, priceStopShort);
				
			//Changes Initial Stop Price
			StopPriceShort = (High[1] + maxStopShort);
			
			entryAreaShortpd = entryAreaShort;
			shortOffsetBE = maxStopShort;
			
			#endregion
				
			#region Short Profit Target Calc
			
			switch (targetType)
			{
			///ActualRR Initial Final Target
				// Calculates Shares with Offset included. 
				case TargetCalcRR.ActualRR:
				{
				
					TargetPriceShort = (Low[1] - offsetAdded) - (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * rrTarget);
					Print("Offset Added :  " + offsetAdded);
						Print("TargetPriceShort :  "  +TargetPriceShort);		
				}
			
				break;
		///CandleRR Initial Final Target		
			// Calculates Targets with only Candle Size
				case TargetCalcRR.CandleRR:
				{
					
					TargetPriceShort = Low[1] - ((High[1] - Low[1]) * rrTarget);
		Print("TargetPriceShort :  "  +TargetPriceShort);	
				}
				
				break;
			}
				#endregion
				
			#region EnterShort
			
				EnterShort(positionSize, "MyEntryShort");
			
				myFreeTradeShort = true;
			
			#endregion
				
			#region Target Calculations
			
			switch (targetType)
			{
		/// ActualRR B/E and Trail 
			// Calculates Shares with Offset included. 
				case TargetCalcRR.ActualRR:
				{
			//----------------------------------------------------------------------------	B/E and Stop Move Targets	
				//This will trigger when the Trail Stop will move up
				StopTriggerShort = ((Low[1] - offsetAdded) - ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * breakevenTarget);
				
				StopTriggerShort2 = ((Low[1] - offsetAdded) - ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * breakevenTarget2);
				
				StopTriggerShort3 = ((Low[1] - offsetAdded) - ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * breakevenTarget3);
				
				shortTarget3 = ((Low[1] - offsetAdded) - ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * target3StopSet);
				
			//----------------------------------------------------------------------------	Trail Stop Targets				
				//Trigger Price to activate Trail
				trailTriggerShort = ((Low[1] - offsetAdded) - ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * trailTrigger);
				
				//Frequency in R's
				trailFrequencyShort = (((High[1] + offsetAdded) + offsetAdded) - (Low[1] - offsetAdded)) * trailFrequency;
				
				//used for Stops
				trailSizeShort = ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * trailSize;
				
					
				trailStopShort = (trailTriggerShort + trailSizeShort);
					
					Print("Offset Added :  " + offsetAdded);
					
				}	
				
				break;
				
				
		///CandlelRR B/E and Trail	
			// Calculates Targets with only Candle Size
				case TargetCalcRR.CandleRR:
				{
				
			//----------------------------------------------------------------------------	B/E and Stop Move Targets	
				//This will trigger when the Trail Stop will move up
				StopTriggerShort = (Low[1] - (High[1] - Low[1]) * breakevenTarget);
				
				StopTriggerShort2 = (Low[1] - (High[1] - Low[1]) * breakevenTarget2);
				
				StopTriggerShort3 = (Low[1] - (High[1] - Low[1]) * breakevenTarget3);
				
				shortTarget3 = (Low[1] - (High[1] - Low[1]) * target3StopSet);
				
			//----------------------------------------------------------------------------	Trail Stop Targets				
				//Trigger Price to activate Trail
				trailTriggerShort = (Low[1] - (High[1] - Low[1]) * trailTrigger);
				
				//Frequency in R's
				trailFrequencyShort = (High[1] - Low[1]) * trailFrequency;
				
				//used for Stops
				trailSizeShort = (High[1] - Low[1]) * trailSize;
				
					
				trailStopShort = (trailTriggerShort + trailSizeShort);
					
				
					
			}
				break;
				
			}
				
			}
			#endregion
			
			#region Stop/Profit Set
			
			if (Position.MarketPosition == MarketPosition.Short && myFreeTradeShort == true)
			{
				if(stopLoss)
				{
				ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort, "MyStopShort", "MyEntryShort");
				}
				
				if(profitTarget)
				{
				ExitShortLimit(0, true, Position.Quantity, TargetPriceShort, "MyTargetShort", "MyEntryShort");
				}
				
				myFreeTradeShort = false;
			}
			#endregion
			
		
			#region DrawLine ShortEntry
			if (
				(shortButtonClicked)
					&& (Close[0] > shortEntry)
					)
				
			{
				Draw.ExtendedLine(this, ShortLine, 10, shortEntry, 0, shortEntry, Brushes.Red, DashStyleHelper.Dot, 2);
			}
			
		
			else if (!shortButtonClicked)
				
			{
				RemoveDrawObject(ShortLine);
			}
				
			#endregion
			
			
				#endregion
			
	///--------------------------------		
			
			#region LongButtonLogicMarket
			
			if  (
					(longButtonClicked)
				
						&& (Position.MarketPosition == MarketPosition.Flat)
						&& (PositionAccount.MarketPosition == MarketPosition.Flat)
				
								&& (enterMarket == true)
				)
			{
				
			#region Share Size Calc
				
			double offsetPercent = (High[1] - Low[1]) * entryOffsetPercentage;
			double offsetPrice = entryOffsetPrice;
			
			double offsetAdded = Math.Max(offsetPercent, offsetPrice);
			
			double userRiskA = maxLossPerTrade/( (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) / TickSize) * myDbl); // Actual RR
			
			//--------------
			double userRiskF = maxLossPerTrade/( ((High[1]-Low[1]) / TickSize) * myDbl); // Candle size RR

			
			
			switch (rrType)
			{
				// Calculates Shares with Offset included. 
				case ShareSizeRR.ActualRR:
				{
				
					positionSize = (Convert.ToInt32(userRiskA));
					break;
				}
				
			// Calculates Shares with only Candle Size
				case ShareSizeRR.CandleRR:
				{
					
					positionSize = (Convert.ToInt32(userRiskF));
					break;
					
				}
			
			 }
			Print("Position Size is : " + positionSize);
				
		#endregion
			
			#region Long Entry/Stop Calc
			///Long Entry Logic

		
			///Long Stop
			double percentStopLong = (High[1] - Low[1]) * stopOffsetPercentage;	
			double priceStopLong =  stopOffsetPrice;
				
			
			double maxStopLong = Math.Max(percentStopLong, priceStopLong);
				
			//Changes Initial Stop Price
			longStopPrice = Math.Min((Low[1] - maxStopLong), (Low[0] - maxStopLong));
			
			
			entryAreaLong = entryArea;
			longOffsetBE = maxStopLong;
			
			#endregion
				
			#region Long Profit Target Calc
					switch (targetType)
			{
		
		///ActualRR Initial Final Target	
			// Calculates Shares with Offset included. 
				case TargetCalcRR.ActualRR:
				{
				
					longTargetPrice = (High[1] + offsetAdded) + (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * rrTarget);
								
				}
			
				break;
	
		///CandleRR Initial Final Target		
			// Calculates Targets with only Candle Size
				case TargetCalcRR.CandleRR:
				{
					
					longTargetPrice = High[1] + ((High[1] - Low[1]) * rrTarget);
		
				}
				
				break;
			}
				#endregion
			
			#region EnterLong
			
				EnterLong(positionSize, "MyEntryLong");
			
				myFreeTradeLong = true;
				
   			#endregion	
			
			#region Target Calculations
			
			switch (targetType)
			{
	///Actual RR B/E and Trail
				// Calculates Targets with Offset included.
				case TargetCalcRR.ActualRR:
			{
				
				//This will trigger when the Trail Stop will move up
				longStopTrigger = ( (High[1] + offsetAdded) + ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * breakevenTarget);
				
				longStopTrigger2 = ( (High[1] + offsetAdded) + ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * breakevenTarget2) ;
				
				longStopTrigger3 = ( (High[1] + offsetAdded) + ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * breakevenTarget3) ;
				
				longTarget3 = ( (High[1] + offsetAdded) + ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * target3StopSet);
				
				
			//----------------------------------------------------------------------------	Trail Stop Targets	
				//Trigger Price to activate Trail
				trailTriggerLong = ( (High[1] + offsetAdded) + ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * trailTrigger	); 
				
				//Frequency in R's
				trailFrequencyLong = ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * trailFrequency;
				//Set stop Loss at Current price - trailFrequency
				
				//used for Stops
				trailSizeLong = ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * trailSize;
				
				longStopTrail = (trailTriggerLong - trailSizeLong);
				
				
				
			}
			break;
			
 	///Candle RR B/E and Trail
				// Calculates Targets based on candle size.
				case TargetCalcRR.CandleRR:
			{
				//This will trigger when the Trail Stop will move up
				longStopTrigger = ( High[1] + (High[1] - Low[1]) * breakevenTarget);
				
				longStopTrigger2 = ( High[1] + (High[1] - Low[1]) * breakevenTarget2) ;
				
				longStopTrigger3 = ( High[1] + (High[1] - Low[1]) * breakevenTarget3) ;
				
				longTarget3 = (High[1] + (High[1] - Low[1]) * target3StopSet);
				
			//----------------------------------------------------------------------------	Trail Stop Targets	
				//Trigger Price to activate Trail
				trailTriggerLong = (High[1] + (High[1] - Low[1]) * trailTrigger	); 
				
				//Frequency in R's
				trailFrequencyLong = (High[1] - Low[1]) * trailFrequency;
				//Set stop Loss at Current price - trailFrequency
				
				//used for Stops
				trailSizeLong = (High[1] - Low[1]) * trailSize;
				
				longStopTrail = (trailTriggerLong - trailSizeLong);
					
			}
				break;
			}
			
				}
			#endregion
			
			#region Stop/Profit Set
			
			if (Position.MarketPosition == MarketPosition.Long && myFreeTradeLong == true)
			{
				if(stopLoss)
				{
					ExitLongStopMarket(0, true, Position.Quantity, longStopPrice, "MyStopLong", "MyEntryLong");
				}
				
				if(profitTarget)
				{
					ExitLongLimit(0, true, Position.Quantity, longTargetPrice, "MyTargetLong", "MyEntryLong");
				}
				
				myFreeTradeLong = false;
			}
			
			#endregion
		
			
			#region DrawLine LongEntry	
			if (
			 	(longButtonClicked)
					&& (Close[0] < longEntry)
				)
				
				{
					Draw.ExtendedLine(this, LongLine, 10, longEntry, 0, longEntry, Brushes.LimeGreen, DashStyleHelper.Dot, 2);
					//Print("Long Entry Price is: " + longEntry);
				}
			
			else if (!longButtonClicked)
				
				{
					RemoveDrawObject(LongLine);
				}
			
			#endregion
				
				
				#endregion
							
	 		#region ShortButtonLogicMarket
			
				if (
						(shortButtonClicked)
				
							&& (Position.MarketPosition == MarketPosition.Flat)
							&& (PositionAccount.MarketPosition == MarketPosition.Flat)
					
									&& (enterMarket == true)
									
					)
			{
			
			#region Share size Calc
				
			double offsetPercent = (High[1] - Low[1]) * entryOffsetPercentage;
			double offsetPrice = entryOffsetPrice;
			
			double offsetAdded = Math.Max(offsetPercent, offsetPrice);
			
			double userRiskA = maxLossPerTrade/( (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) / TickSize) * myDbl); // Actual RR
			
			//--------------
			double userRiskF = maxLossPerTrade/( ((High[1]-Low[1]) / TickSize) * myDbl); // Candle size RR

			
			
			switch (rrType)
			{
				// Calculates Shares with Offset included. 
				case ShareSizeRR.ActualRR:
				{
				
					positionSize = (Convert.ToInt32(userRiskA));
					break;
				}
				
			// Calculates Shares with only Candle Size
				case ShareSizeRR.CandleRR:
				{
					
					positionSize = (Convert.ToInt32(userRiskF));
					break;
					
				}
			
			 }
			Print("Position Size is : " + positionSize);
				
				#endregion
				
			#region Short Entry/Stop Calc
			
			
			///Short Stop 
			double percentStopShort = (High[1] - Low[1]) * stopOffsetPercentage;
			double priceStopShort = stopOffsetPrice;
				
			maxStopShort = Math.Max(percentStopShort, priceStopShort);
				
			//Changes Initial Stop Price
			StopPriceShort = Math.Max((High[1] + maxStopShort), (High[0] + maxStopShort));
			
			entryAreaShortpd = entryAreaShort;
			shortOffsetBE = maxStopShort;
			
			#endregion
				
			#region Short Profit Target Calc
			
			switch (targetType)
			{
			///ActualRR Initial Final Target
				// Calculates Shares with Offset included. 
				case TargetCalcRR.ActualRR:
				{
				
					TargetPriceShort = (Low[1] - offsetAdded) - (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * rrTarget);
					Print("Offset Added :  " + offsetAdded);
						Print("TargetPriceShort :  "  +TargetPriceShort);		
				}
			
				break;
		///CandleRR Initial Final Target		
			// Calculates Targets with only Candle Size
				case TargetCalcRR.CandleRR:
				{
					
					TargetPriceShort = Low[1] - ((High[1] - Low[1]) * rrTarget);
		Print("TargetPriceShort :  "  +TargetPriceShort);	
				}
				
				break;
			}
				#endregion
				
			#region EnterShort
			
				EnterShort(positionSize, "MyEntryShort");
			
				myFreeTradeShort = true;
			
			#endregion
				
			#region Target Calculations
			
			switch (targetType)
			{
		/// ActualRR B/E and Trail 
			// Calculates Shares with Offset included. 
				case TargetCalcRR.ActualRR:
				{
			//----------------------------------------------------------------------------	B/E and Stop Move Targets	
				//This will trigger when the Trail Stop will move up
				StopTriggerShort = ((Low[1] - offsetAdded) - ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * breakevenTarget);
				
				StopTriggerShort2 = ((Low[1] - offsetAdded) - ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * breakevenTarget2);
				
				StopTriggerShort3 = ((Low[1] - offsetAdded) - ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * breakevenTarget3);
				
				shortTarget3 = ((Low[1] - offsetAdded) - ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * target3StopSet);
				
			//----------------------------------------------------------------------------	Trail Stop Targets				
				//Trigger Price to activate Trail
				trailTriggerShort = ((Low[1] - offsetAdded) - ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * trailTrigger);
				
				//Frequency in R's
				trailFrequencyShort = (((High[1] + offsetAdded) + offsetAdded) - (Low[1] - offsetAdded)) * trailFrequency;
				
				//used for Stops
				trailSizeShort = ((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * trailSize;
				
					
				trailStopShort = (trailTriggerShort + trailSizeShort);
					
					Print("Offset Added :  " + offsetAdded);
					
				}	
				
				break;
				
				
		///CandlelRR B/E and Trail	
			// Calculates Targets with only Candle Size
				case TargetCalcRR.CandleRR:
				{
				
			//----------------------------------------------------------------------------	B/E and Stop Move Targets	
				//This will trigger when the Trail Stop will move up
				StopTriggerShort = (Low[1] - (High[1] - Low[1]) * breakevenTarget);
				
				StopTriggerShort2 = (Low[1] - (High[1] - Low[1]) * breakevenTarget2);
				
				StopTriggerShort3 = (Low[1] - (High[1] - Low[1]) * breakevenTarget3);
				
				shortTarget3 = (Low[1] - (High[1] - Low[1]) * target3StopSet);
				
			//----------------------------------------------------------------------------	Trail Stop Targets				
				//Trigger Price to activate Trail
				trailTriggerShort = (Low[1] - (High[1] - Low[1]) * trailTrigger);
				
				//Frequency in R's
				trailFrequencyShort = (High[1] - Low[1]) * trailFrequency;
				
				//used for Stops
				trailSizeShort = (High[1] - Low[1]) * trailSize;
				
					
				trailStopShort = (trailTriggerShort + trailSizeShort);
					
				
					
			}
				break;
				
			}
				
			}
			#endregion
			
			#region Stop/Profit Set
			
			if (Position.MarketPosition == MarketPosition.Short && myFreeTradeShort == true)
			{
				if(stopLoss)
				{
				ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort, "MyStopShort", "MyEntryShort");
				}
				
				if(profitTarget)
				{
				ExitShortLimit(0, true, Position.Quantity, TargetPriceShort, "MyTargetShort", "MyEntryShort");
				}
				
				myFreeTradeShort = false;
			}
			#endregion
			
		
			#region DrawLine ShortEntry
			if (
				(shortButtonClicked)
					&& (Close[0] > shortEntry)
					)
				
			{
				Draw.ExtendedLine(this, ShortLine, 10, shortEntry, 0, shortEntry, Brushes.Red, DashStyleHelper.Dot, 2);
			}
			
		
			else if (!shortButtonClicked)
				
			{
				RemoveDrawObject(ShortLine);
			}
				
			#endregion
			
			
				#endregion
		
		}	
			

			#region Breakeven Logic	
		
		protected override void OnMarketData(MarketDataEventArgs marketDataUpdate)
			{
	
    	if (marketDataUpdate.MarketDataType == MarketDataType.Last)
			{
	  
		#region ShortBreakEven
			
			#region Short BE w/Offset
		if (userTarget1)
		{
			if (
				(Position.MarketPosition == MarketPosition.Short)
				 	&& (StopPriceShort != 0)
				 		&& (marketDataUpdate.Price <= StopTriggerShort)
							&& (beTarget2 != true)
							&& (beTarget3 != true)
							&& (isTrailShort != true)
				
								&& (triggeredShort1 != true)
				)
			
			{
				beTarget1 = true;
				beTarget2 = false;
				beTarget3 = false;
				isTrailShort = false;
			}
			
			
			if (beTarget1)
				
			
			{

				StopPriceShort = entryAreaShortpd + shortOffsetBE;

				ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort, "MyStopShort", "MyEntryShort");
				
				Print("Setting Stop ="+StopPriceShort);
				Print("entry AreaBE ="+ entryAreaShortpd);
				Print("shortOffsetBE ="+ shortOffsetBE);
				
				Print("Target 1: BE1 ="+ beTarget1);
				Print("Target 1: BE2 ="+ beTarget2);
				Print("Target 1: BE3 ="+ beTarget3);
				
				beTarget1 = false;
				triggeredShort1 = true;

				
			}
		}
		#endregion
		
			#region Short Actual BE
		
		if (userTarget2)
		{
			
				if (
					(Position.MarketPosition == MarketPosition.Short)
						&& (StopPriceShort != 0)
				  			&&(marketDataUpdate.Price <= StopTriggerShort2)
								&&(beTarget3 != true)
								&& (isTrailShort != true)
					
									&& (triggeredShort2 != true)
					)			
				
				{
					beTarget1 = false;
					beTarget2 = true;
					beTarget3 = false;
					isTrailShort = false;
				}
				
				if (beTarget2)
					
				{
				
				
				///Sets Stop to Actual B/E
				StopPriceShort2 = (Position.AveragePrice) ;

				ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort2, "MyStopShort", "MyEntryShort");
				
				Print("Short Target 2: BE1 ="+ beTarget1);
				Print("Short Target 2: BE2 ="+ beTarget2);
				Print("Short Target 2: BE3 ="+ beTarget3);
			
					beTarget2 = false;
					triggeredShort2 = true;
			}
			
		}
		#endregion
		
			#region Short Custom Stop Move
		 
		if (userTarget3)
		{
		
				if (
					(Position.MarketPosition == MarketPosition.Short)
						&& (StopPriceShort != 0)
				  			&&(marketDataUpdate.Price <= StopTriggerShort3)
								&& (isTrailShort != true)
					
									&& (triggeredShort3 != true)
					)
				
				{
					beTarget1 = false;
					beTarget2 = false;
					beTarget3 = true;
					isTrailShort = false;
				}
				
				if (beTarget3)
					
				{
					StopPriceShort3 = shortTarget3;
				
				ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort3, "MyStopShort", "MyEntryShort");
				
				Print("Target 3: BE1 ="+ beTarget1);
				Print("Target 3: BE2 ="+ beTarget2);
				Print("Target 3: BE3 ="+ beTarget3);
					
					beTarget3 = false;
					triggeredShort3 = true;
			
				}
		}
			
		#endregion
	
			#region Short Trail Stop
		
		switch (trailType)
			{
				case TrailStop.CustomTrail:
				{
		
		if (userTrailTarget)
		{
			
				if (
					(Position.MarketPosition == MarketPosition.Short)
						&& (StopPriceShort != 0)
				  			&&(marketDataUpdate.Price <= trailTriggerShort)
					)
				
				{
					beTarget1 = false;
					beTarget2 = false;
					beTarget3 = false;
					isTrailShort = true;
					
				}
				
				if (isTrailShort)
					
				{
					
					trailStopShort = (marketDataUpdate.Price + (trailSizeShort));
				 	
					trailTriggerShort = (marketDataUpdate.Price - (trailFrequencyShort));
				
				ExitShortStopMarket(0, true, Position.Quantity, trailStopShort, "MyStopShort", "MyEntryShort");
				
					Print("Target 3: BE1 ="+ beTarget1);
					Print("Target 3: BE2 ="+ beTarget2);
					Print("Target 3: BE3 ="+ beTarget3);
						
					Print("Trail Stop short ="+ trailStopShort);
					Print("Trail trigger short ="+ trailTriggerShort);
						
					isTrailShort = false;	
			
				}
		}
	
				}
				break;
				
	///----------------------------------------------------------------------------Close Candle Trail	
				
			case TrailStop.CloseCandle:
			{
		if (userTrailTarget)
		{
			
				if (
					(Position.MarketPosition == MarketPosition.Short)
						&& (StopPriceShort != 0)
				  			&&(marketDataUpdate.Price <= trailTriggerShort)
					)
				
				{
					beTarget1 = false;
					beTarget2 = false;
					beTarget3 = false;
					isTrailShort = true;
					
				}
				
		if (isTrailShort)	
		{
					
			if(IsFirstTickOfBar)
			{
				
				double trailOffsetPer = (High[1] - Low[1]) * entryOffsetPercentage;
				double trailOffsetPrice = entryOffsetPrice;
			
				double trailOffsetAdded = Math.Max(trailOffsetPer, trailOffsetPrice);
				
				trailStopOffset = trailOffsetAdded;	
					
				trailStopShort = (High[1]) + trailStopOffset ;
			 
					//trailTriggerShort = (marketDataUpdate.Price - (trailFrequencyShort));
				
				ExitShortStopMarket(0, true, Position.Quantity, trailStopShort, "MyStopShort", "MyEntryShort");
				
					Print("Target 3: BE1 ="+ beTarget1);
					Print("Target 3: BE2 ="+ beTarget2);
					Print("Target 3: BE3 ="+ beTarget3);
						
					Print("Trail Stop short ="+ trailStopShort);
					Print("Trail trigger short ="+ trailTriggerShort);
						
						Print("trailStopOffset Trail Logic ="+ trailStopOffset);
						
					isTrailShort = false;	
			}	

		}
		}
			}
			break;
			
			}		
			#endregion
					
		#endregion
				
		
		#region LongBreakeven
				
			#region Long BE w/Offset
			
		if (userTarget1)
		{
				
			if (
				(Position.MarketPosition == MarketPosition.Long)
					&& (longStopPrice != 0)
						&& (marketDataUpdate.Price >= longStopTrigger)
							&& (beLongTarget2 != true)
							&& (beLongTarget3 != true)
							&& (isTrailLong != true)
				
								&& (triggered1 != true)				
				)
			{
				beLongTarget1 = true;
				beLongTarget2 = false;
				beLongTarget3 = false;
				isTrailLong = false;
			}
			
			
			if (beLongTarget1)// && (userTarget1))
			
			{
			
				longStopPrice = entryAreaLong - longOffsetBE;
				
				Print("Setting Stop ="+ longStopPrice);
				Print("entry AreaBE ="+ entryAreaLong);
				Print("longOffsetBE ="+ longOffsetBE);
				
				Print("Target 1: BE1 ="+ beLongTarget1);
				Print("Target 1: BE2 ="+ beLongTarget2);
				Print("Target 1: BE3 ="+ beLongTarget3);
				
				ExitLongStopMarket(0, true, Position.Quantity, longStopPrice, "MyStopLong", "MyEntryLong");
				
				beLongTarget1 = false;
				triggered1 = true;
				
				Print("LongStopPrice New ="+ longStopPrice);
				Print("LongStopTrigger New ="+ longStopTrigger);
				Print("Target 1 New Bool: BE1 ="+ beLongTarget1);
			}
		}
		
		#endregion
	
			#region Long Actual BE
		
		if (userTarget2)
		{
			if (
					(Position.MarketPosition == MarketPosition.Long)
						&& (longStopPrice != 0)
				  			&&(marketDataUpdate.Price >= longStopTrigger2)
								&&(beLongTarget3 != true)
								&& (isTrailLong != true)
									
									&& (triggered2 != true)					
				)
				
				{
					beLongTarget1 = false;
					beLongTarget2 = true;
					beLongTarget3 = false;
					isTrailLong = false;
				}
				
				if (beLongTarget2)
					
				{
				
				
				///Sets Location of Stop AFTER first target is triggered. B/E STOP
				longStopPrice2 = (Position.AveragePrice) ;

				ExitLongStopMarket(0, true, Position.Quantity, longStopPrice2, "MyStopLong", "MyEntryLong");
					
				Print("Long Target 2: BE1 ="+ beLongTarget1);
				Print("Long Target 2: BE2 ="+ beLongTarget2);
				Print("Long Target 2: BE3 ="+ beLongTarget3);
					
					beLongTarget2 = false;
					triggered2 = true;
			
				}
			}
		#endregion
			
			#region Long Custom Stop Move
		
		if (userTarget3)
		{
		
			if (
					(Position.MarketPosition == MarketPosition.Long)
					&& (longStopPrice != 0)
				  		&& (marketDataUpdate.Price >= longStopTrigger3)
							&& (isTrailLong != true)
								
								&& (triggered3 != true)				
				)
				
				{
					beLongTarget1 = false;
					beLongTarget2 = false;
					beLongTarget3 = true;
					isTrailLong = false;
				}
				
				if (beLongTarget3)
					
				{
					longStopPrice3 = longTarget3;
				
				ExitLongStopMarket(0, true, Position.Quantity, longStopPrice3, "MyStopLong", "MyEntryLong");
				
				Print("Target 3: BE1 ="+ beLongTarget1);
				Print("Target 3: BE2 ="+ beLongTarget2);
				Print("Target 3: BE3 ="+ beLongTarget3);
					
					beLongTarget3 = false;
					triggered3 = true;
				}	
		}
		
		#endregion
					
			#region Long Trail Stop
		
		switch (trailType)
			{
				case TrailStop.CustomTrail:
				{
		
		if (userTrailTarget)	
		{
			
			if (
					(Position.MarketPosition == MarketPosition.Long)
						&& (longStopPrice != 0)
				  			&&(marketDataUpdate.Price >= trailTriggerLong)
								
				)
				
				{
					beLongTarget1 = false;
					beLongTarget2 = false;
					beLongTarget3 = false;
					isTrailLong = true;	

					Print("Trail Bool: Upper ="+ isTrailLong);
					
				}

				if (isTrailLong)
				
					
				{
					
					longStopTrail = (marketDataUpdate.Price - (trailSizeLong)) ;
					
					trailTriggerLong = (marketDataUpdate.Price + (trailFrequencyLong)); 
					
				ExitLongStopMarket(0, true, Position.Quantity, longStopTrail, "MyStopLong", "MyEntryLong");
				
			
					Print("trail Trigger Price  ="+ trailTriggerLong);
					Print("trail Stop Price ="+ longStopTrail);
					Print("trail Frequency =" + trailFrequencyLong);

					isTrailLong = false;

					Print("Trail Bool: Lower ="+ isTrailLong);
				}	
			
			}
				}
				break;
		
		///-----------------------------------------------
				/// 
				
			case TrailStop.CloseCandle:
				{
				
		if (userTrailTarget)	
		{
			
			if (
					(Position.MarketPosition == MarketPosition.Long)
						&& (longStopPrice != 0)
				  			&&(marketDataUpdate.Price >= trailTriggerLong)
								
				)
				
				{
					beLongTarget1 = false;
					beLongTarget2 = false;
					beLongTarget3 = false;
					isTrailLong = true;	

					//Print("Trail Bool: Upper ="+ isTrailLong);
					
				}

				if (isTrailLong)
				
					
				{
					if (IsFirstTickOfBar)
					{
						
					double trailOffsetPer = (High[1] - Low[1]) * entryOffsetPercentage;
						
					double trailOffsetPrice = entryOffsetPrice;
			
					double trailOffsetAdded = Math.Max(trailOffsetPer, trailOffsetPrice);
						
					longTrailOffset = trailOffsetAdded;
						
					longStopTrail = ((Low[1]) - longTrailOffset);
					
					//trailTriggerLong = (marketDataUpdate.Price + (trailFrequencyLong)); 
					
				ExitLongStopMarket(0, true, Position.Quantity, longStopTrail, "MyStopLong", "MyEntryLong");
				
			
					Print("trail Trigger Price  ="+ trailTriggerLong);
					Print("trail Stop Price ="+ longStopTrail);
					Print("trail Frequency =" + trailFrequencyLong);

					isTrailLong = false;

					Print("Trail Bool: Lower ="+ isTrailLong);
					}
				}	
			
			}
				}
				break;
				
		}
		}
		
		#endregion
	
		#endregion 
		
			}
	 
		
		#endregion
		
		
		#region ButtonClickEvent
		
		private void OnButtonClick(object sender, RoutedEventArgs rea)
		{
			System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
			if (button == longButton && button.Name == "LongButton" && button.Content == "LONG")
			{
				button.Content = "LIVE Long";
				button.Name = "LiveLongButton";
				longButtonClicked = true;
				return;
			}
			
			if (button == shortButton && button.Name == "ShortButton" && button.Content == "SHORT")
				
			{
				button.Content = "LIVE Short";
				button.Name = "LiveShortButton";
				shortButtonClicked = true;
				return;
			}
			
			if (button == longButton && button.Name == "LiveLongButton" && button.Content == "LIVE Long")
			{
				button.Content = "LONG";
				button.Name = "LongButton";
				longButtonClicked = false;
				RemoveDrawObject(LongLine);
				return;
			}
			
			if (button == shortButton && button.Name == "LiveShortButton" && button.Content == "LIVE Short")
			
			{
				button.Content = "SHORT";
				button.Name = "ShortButton";
				shortButtonClicked = false;
				RemoveDrawObject(ShortLine);
				return;
			}
				
		}
		

		
			protected override void OnExecutionUpdate(Execution execution, string executionId, double price, int quantity, MarketPosition marketPosition, string orderId, DateTime time)
{
	 if (Position.MarketPosition != MarketPosition.Flat)
		
	 {
	 	
		ChartControl.Dispatcher.InvokeAsync(new Action(() => 
		 {
            longButton.Content = "Disabled L";
			longButtonClicked = false;
			RemoveDrawObject(LongLine);
			 
			
			shortButton.Content = "Disabled S";
			shortButtonClicked = false;
			RemoveDrawObject(ShortLine);
			
        }));
				
	 }
	 
	 if (Position.MarketPosition == MarketPosition.Flat)
		
	 {
	 	
		Dispatcher.InvokeAsync((() =>
				{
					myGrid = new System.Windows.Controls.Grid
					{
						Name = "MyCustomGrid", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top
					};
					
					System.Windows.Controls.ColumnDefinition column1 = new System.Windows.Controls.ColumnDefinition();
					System.Windows.Controls.ColumnDefinition column2 = new System.Windows.Controls.ColumnDefinition();
					
					myGrid.ColumnDefinitions.Add(column1);
					myGrid.ColumnDefinitions.Add(column2);
					
					longButton = new System.Windows.Controls.Button
					{
						Name = "LongButton", Content = "LONG", Foreground = Brushes.White, Background = Brushes.Green
					};
					
					shortButton = new System.Windows.Controls.Button
					{
						Name = "ShortButton", Content = "SHORT", Foreground = Brushes.Black, Background = Brushes.Red
					};
					
					longButton.Click += OnButtonClick;
					shortButton.Click += OnButtonClick;
					
					System.Windows.Controls.Grid.SetColumn(longButton, 0);
					System.Windows.Controls.Grid.SetColumn(shortButton, 1);
					
					myGrid.Children.Add(longButton);
					myGrid.Children.Add(shortButton);
					
					UserControlCollection.Add(myGrid);
				}));
				
	 }
	
	 else if (State == State.Terminated)
			{
				Dispatcher.InvokeAsync((() =>
				{
					if (myGrid != null)
					{
						if (longButton != null)
						{
							myGrid.Children.Remove(longButton);
							longButton.Click -= OnButtonClick;
							longButton = null;
						}
						if (shortButton != null)
						{
							myGrid.Children.Remove(shortButton);
							shortButton.Click -= OnButtonClick;
							shortButton = null;
						}
					}
				}));
			}	
} 

		#endregion
		

			#region Properties
		
		#region Entry Offset
//private EntryArea	entryType	= EntryArea.highLow;


		[Display(Name = "Entry Order Type", GroupName = "1. Entry Parameters", Description="Enter at break of High/Low of previous candle, Enter at break of previous Close", Order = 0)]
		public EntryArea EntryType
		{
			get { return entryType; }
			set { entryType = value; }
		}

		/// User input variable. This variable will allow user to set the Entry a custom amount past IB High or Low
		[Display(Name = "Price - Entry Offset", GroupName = "1. Entry Parameters", Order = 1)]
		public double EntryOffsetPrice
		{
			get{return entryOffsetPrice;}
			set{entryOffsetPrice = (value);}
		}
		
		
		/// User input variable. This variable will allow user to set the Entry a custom amount past IB High or Low
		[Display(Name = "Percentage - Entry Offset", GroupName = "1. Entry Parameters", Order = 2)]
		public double EntryOffsetPercentage
		{
			get{return entryOffsetPercentage;}
			set{entryOffsetPercentage = (value);}
		}
		
		#endregion
		
		#region Stop Offset
		
		[NinjaScriptProperty]
		[Display(Name = "Set Stop Loss", Description = "", Order = 0, GroupName = "2. Stop Offset")]
		public bool StopLoss 
		{
		 	get{return stopLoss;} 
			set{stopLoss = (value);} 
		}
		
		/// User input variable. This variable will allow user to set the Stop a custom amount past IB High or Low
		[Display(Name = "Price - Stop Offset", GroupName = "2. Stop Offset", Order = 1)]
		public double StopOffsetPrice 
		{
			get{return stopOffsetPrice ;}
			set{stopOffsetPrice  = (value);}
		}
		
		
			/// User input variable. This variable will allow user to set the Stop a custom amount past IB High or Low
		[Display(Name = "Percentage - Stop Offset", GroupName = "2. Stop Offset", Order = 2)]
		public double StopOffsetPercentage 
		{
			get{return stopOffsetPercentage ;}
			set{stopOffsetPercentage  = (value);}
		}
		#endregion
		
		#region 3. Risk
		[Display(Name = "Share Calculation Method", GroupName = "3. Risk", Description="ActualRR includes offset, CandleRR uses only Candle Range", Order = 0)]
	public ShareSizeRR RRType
		{
			get { return rrType; }
			set { rrType = value; }
		}
		
		/// Allows user to custom select Loss risk per trade 
		[Display(Name = "MaxLossPerTrade($)", GroupName = "3. Risk", Order = 1)]
		public double MaxLossPerTrade
		{
			get{return maxLossPerTrade;}
			set{maxLossPerTrade = (value);}
		}
		
		#endregion
		
		#region 4. Targets/Management
		
		[NinjaScriptProperty]
		[Display(Name = "Set Profit Target", Description = "", Order = 0, GroupName = "4. Management - Profit Targets")]
		public bool ProfitTarget 
		{
		 	get{return profitTarget;} 
			set{profitTarget = (value);} 
		}
		
		[Display(Name = "Target Calculation Method", GroupName = "4. Management - Profit Targets", Description="ActualRR includes offset, CandleRR uses only Candle Range", Order = 1)]
		public TargetCalcRR TargetType
		{
			get { return targetType; }
			set { targetType = value; }
		}	
		
			/// Allows user to select custom R:R Target 
		[Display(Name = "Final Profit Target", GroupName = "4. Management - Profit Targets", Order = 2)]
		public double RRTarget
		{
			get{return rrTarget;}
			set{rrTarget = (value);}
		}
		
	///----------------------------------------------------------------------------	
		
			[NinjaScriptProperty]
		[Display(Name = "Breakeven w/ Offset", Description = "", Order = 1, GroupName = "4.1 Offset Breakeven")]
		public bool UserTarget1 
		{
		 	get{return userTarget1;} 
			set{userTarget1 = (value);} 
		}
		
				/// Once this target is hit. A Breakeven Stop will be set
		[Display(Name = "Breakeven (w/ Offset)", GroupName = "4.1 Offset Breakeven", Order = 2)]
		public double BreakevenTarget
		{
			get{return breakevenTarget;}
			set{breakevenTarget = (value);}
		}
		
	///----------------------------------------------------------------------------	
		
		[NinjaScriptProperty]
		[Display(Name = "Breakeven (Actual Entry)", Description = "", Order = 3, GroupName = "4.2 Actual Breakeven")]
		public bool UserTarget2 
		{
		 	get{return userTarget2;} 
			set{userTarget2 = (value);} 
		}
		
		[Display(Name = "Breakeven (Actual Entry)", GroupName = "4.2 Actual Breakeven", Order = 4)]
		public double BreakevenTarget2
		{
			get{return breakevenTarget2;}
			set{breakevenTarget2 = (value);}
		}
	
	///----------------------------------------------------------------------------
		
		[NinjaScriptProperty]
		[Display(Name = "Custom Stop Set", Description = "", Order = 5, GroupName = "4.3 Move Stop")]
		public bool UserTarget3 
		{
		 	get{return userTarget3;} 
			set{userTarget3 = (value);} 
		}
		
		
		[Display(Name = "Custom Stop (Trigger Target)", GroupName = "4.3 Move Stop", Order = 6)]
		public double BreakevenTarget3
		{
			get{return breakevenTarget3;}
			set{breakevenTarget3 = (value);}
		}
		
		[Display(Name = "Custom Stop (Sets Stop Price)", GroupName = "4.3 Move Stop", Order = 7)]
		public double Target3StopSet
		{
			get{return target3StopSet;}
			set{target3StopSet = (value);}
		}
	
	///----------------------------------------------------------------------------	
	
		[NinjaScriptProperty]
		[Display(Name = "Trail Stop", Description = "", Order = 0, GroupName = "4.4 Trail Stop")]
		public bool UserTrailTarget 
		{
		 	get{return userTrailTarget;} 
			set{userTrailTarget = (value);} 
		}
		
		
		[Display(Name = "Trail Stop Method", GroupName = "4.4 Trail Stop", Description="CloseCandle trails each new closing candle. Custom can be user customizeable", Order = 1)]
		public TrailStop	TrailType
		{
			get { return trailType; }
			set { trailType = value; }
		}	
		
		
		
		[Display(Name = "Trail Stop Trigger Target", GroupName = "4.4 Trail Stop", Description="Trail Stop will Trigger when it reaches this RR Target from Entry", Order = 2)]
		public double TrailTrigger
		{
			get{return trailTrigger;}
			set{trailTrigger = (value);}
		}
		
		[Display(Name = "Trail Stop Trigger Frequency(CustomTrail Only)", GroupName = "4.4 Trail Stop", Description="Distance to new Target. Every time price reaches this new target, Stop Will move up", Order = 10)]
		public double TrailFrequency
		{
			get{return trailFrequency;}
			set{trailFrequency = (value);}
		}

		[Display(Name = "CustomTrail Stop Size(CustomTrail Only)", GroupName = "4.4 Trail Stop", Description="Once Price Reaches 'Frequency Target', Stop will be this RR under the newest target reached", Order = 11)]
		public double TrailSize
		{
			get{return trailSize;}
			set{trailSize = (value);}
		}

		
		#endregion 
		
		#region 5. Playback Mode
		[NinjaScriptProperty]
		[Display(Name = "Playback Mode Only", Description = "Only Activate when using Playback Mode", Order = 0, GroupName = "5. Playback Mode Enabled")]
		public bool PlaybackMode 
		{
		 	get{return playbackMode;} 
			set{playbackMode = (value);} 
		}
		
		#endregion
		
		#endregion
	}
}
