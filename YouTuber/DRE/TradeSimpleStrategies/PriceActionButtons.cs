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
	public class PriceActionButtons: Strategy
	//aspr
	{
		private const string SystemVersion 					= " V 4.2";
        private const string SystemName 					= "Price Action Order Entry";
		
	#region Private Variables
		
		#region Strings
		private const string LongTargetExit = "ExitTargetLong";
		private const string ShortTargetExit = "ExitTargetShort";
		
		private const string LongStopExit = "ExitStopLong";
		private const string ShortStopExit = "ExitStopShort";
		
		
		private const string LongLine = "LongEntryLine";
		private const string ShortLine = "ShortEntryLine";
		
		private string youtube								= "https://www.youtube.com/playlist?list=PLtzm5NK258sq-rkX-rolEtefgfX-uE44I";
		private string discord								= "https://discord.gg/2YU9GDme8j";
		
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
		private double longStopPrice3;//TS
		
		private double longStopTrigger;
		private double longStopTrigger2;
		private double longStopTrigger3; 
		
		private double longTargetPrice;
		private double longTargetPrice2;
		
		private double longEntryPrice;
	///-----------------------------------------------	
		private double StopPriceShort;
		private double StopPriceShort2;
		private double StopPriceShort3;
		
		private double StopTriggerShort;
		private double StopTriggerShort2;
		private double StopTriggerShort3;
		
		private double TargetPriceShort;
		private double TargetPriceShort2;
		
		private double EntryPriceShort;
	///-----------------------------------------------	
		private TargetsSet	targetAmount	= TargetsSet.OneTarget;
		
		private bool dualTarget;
		
		private int firstTargetPosition;
		private	int secondTargetPosition;
	
		private double splitPercent = 0.25;
		#endregion
	///-----------------------------------------------		
		#region Entry/Stop Offset Variables
		
		private double entryOffsetPrice				= 0.03;
		private double entryOffsetPercentage 		= 0.10;
		private int entryOffsetTick					= 1;
		
		//longEntryTick 		= tickOffset * TickSize;
		
		private double stopOffsetPrice 				= 0.03;
		private double stopOffsetPercentage 		= 0.10;
		private int stopOffsetTick					= 1;
		
		#endregion
	///-----------------------------------------------	
		#region EntryArea Variables
		
		private EntryArea	entryType	= EntryArea.HighLow;
		
		private double entryAreaLong;
		private double longOffsetBE;
		private double longEntryTick;
		
		
		private double entryAreaShortpd;
		private double shortOffsetBE;
		private double shortEntryTick; 
		
		
		private double longEntry;
		private double shortEntry;
		
		private double entryArea;
		private double entryAreaShort;
		
		private bool enterMarket;
		
		private bool useLimit;
		private double limitOffsetPercentage 		= 0;
		private double limitOffsetPrice				= 0;
		private int limitOffsetTick					= 0;
		
		private double longLimitPrice;
		private double shortLimitPrice;
		
		#endregion
	///-----------------------------------------------	
		#region Share Calc Variables
		
		private PositionCalc	posCalcType	= PositionCalc.LastBarPos;
		private int customQTY = 10;
		
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
		private double rrTarget2					= 2;
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
		private bool myFreeLimit;
		private bool myFreeLimitShort;
		
		
		
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
		#region Fib Level Variables
		
		private bool showFibLevels;
		
		private double fibStop;
		private double fibEntry;
		
		private double fibStopShort;
		private double fibEntryShort;
		
		private double longEntryAreaFib;
		private double longStopAreaFib;
		
		private double shortEntryAreaFib;
		private double shortStopAreaFib;
		
		private bool isThisBar;
		private int thisBar = 1;
		
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
	
	public enum  PositionCalc
	{
		LastBarPos,
		CustomPos,	
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
		
	public enum TargetsSet
	{
		OneTarget,
		TwoTargets,
	}	
		#endregion	
	///------------------------------------------------------------------------------------------------------------------------------------------------
		#region Button Variables
	
		private bool longButtonClicked;
		private bool shortButtonClicked;
		private bool fibButtonClicked;
	
		private System.Windows.Controls.Button longButton;
		private System.Windows.Controls.Button shortButton;
		private System.Windows.Controls.Button fibButton;
	
		private System.Windows.Controls.Grid myGrid;
		private System.Windows.Controls.Grid myGrid2;
	
	
		private bool showSocials;
		
		private bool youtubeButtonClicked;
		private bool discordButtonClicked;
		
		private System.Windows.Controls.Button youtubeButton;
		private System.Windows.Controls.Button discordButton;
		
		
		private System.Windows.Controls.Grid myGrid29;
	
		
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
				EntryHandling						= EntryHandling.UniqueEntries;
				IsExitOnSessionCloseStrategy		= true;
				ExitOnSessionCloseSeconds			= 30;
				IsFillLimitOnTouch					= false;
				MaximumBarsLookBack					= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution					= OrderFillResolution.Standard;
				Slippage							= 0;
				StartBehavior						= StartBehavior.WaitUntilFlat;
				TimeInForce							= TimeInForce.Day;
				TraceOrders							= true;
				RealtimeErrorHandling				= RealtimeErrorHandling.StopCancelClose;
				
				StopTargetHandling					= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade					= 20;
				
				IsInstantiatedOnEachOptimizationIteration	= true;
				
				enterMarket									= false;
				useLimit									= true;
				
				userTarget1									= false;
				userTarget2 								= false;
				userTarget3 								= false;
				userTrailTarget 							= false;
				
				profitTarget 								= true;
				stopLoss 									= true;
				dualTarget 									= false;
				
				showFibLevels								= false;
				isThisBar									= false;
				
				playbackMode								= false;
				
				myFreeLimit 								= true;
				myFreeLimitShort 							= true;
				
				///Will show buttons with links to Youtube/Discord.
				showSocials									= true;
					
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
					//System.Windows.Controls.ColumnDefinition column3 = new System.Windows.Controls.ColumnDefinition();
					
					myGrid.ColumnDefinitions.Add(column1);
					myGrid.ColumnDefinitions.Add(column2);
					//myGrid.ColumnDefinitions.Add(column3);
					
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
				
		//----------------------------------------------------------------------------------------	
			if(showFibLevels)
			{
				
				Dispatcher.InvokeAsync((() =>
				{
					myGrid2 = new System.Windows.Controls.Grid
					{
						Name = "MyCustomGrid2", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top
					};
				
					System.Windows.Controls.ColumnDefinition column3 = new System.Windows.Controls.ColumnDefinition();
					myGrid.ColumnDefinitions.Add(column3);
					
					fibButton = new System.Windows.Controls.Button
					{
						Name = "FibButton", Content = "FibLevelsOn", Foreground = Brushes.Black, Background = Brushes.White
					};
				
					fibButton.Click += OnButtonClick;
					
					myGrid2.Children.Add(fibButton);
					
					UserControlCollection.Add(myGrid2);
					
					fibButtonClicked = true;
				}));
			}
			
		#region TradeSimple Socials
			
			if (showSocials)
			{
				if (UserControlCollection.Contains(myGrid29))
					return;
				
				Dispatcher.InvokeAsync((() =>
				{
					myGrid29 = new System.Windows.Controls.Grid
					{
						Name = "MyCustomGrid", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Bottom
					};
					
					System.Windows.Controls.ColumnDefinition column1 = new System.Windows.Controls.ColumnDefinition();
					System.Windows.Controls.ColumnDefinition column2 = new System.Windows.Controls.ColumnDefinition();
					
					myGrid29.ColumnDefinitions.Add(column1);
					myGrid29.ColumnDefinitions.Add(column2);
					
					youtubeButton = new System.Windows.Controls.Button
					{
						Name = "YoutubeButton", Content = "Youtube", Foreground = Brushes.White, Background = Brushes.Red
					};
					
					discordButton = new System.Windows.Controls.Button
					{
						Name = "DiscordButton", Content = "Discord", Foreground = Brushes.White, Background = Brushes.RoyalBlue
					};
					
					youtubeButton.Click += OnButtonClick;
					discordButton.Click += OnButtonClick;
					
					System.Windows.Controls.Grid.SetColumn(youtubeButton, 0);
					System.Windows.Controls.Grid.SetColumn(discordButton, 1);
					
					myGrid29.Children.Add(youtubeButton);
					myGrid29.Children.Add(discordButton);
					
					UserControlCollection.Add(myGrid29);
				}));
			}
		#endregion	
			
		#region useLater	
		//-----------------------------------------------------------------------		
				ChartControl.Dispatcher.InvokeAsync((Action)(() =>
			{
				NinjaTrader.Gui.Tools.QuantityUpDown quantitySelector = (Window.GetWindow(ChartControl.Parent).FindFirst("ChartTraderControlQuantitySelector") as NinjaTrader.Gui.Tools.QuantityUpDown);

				quantitySelector.Value = customQTY;
			}));
				
		//-----------------------------------------------------------------------	
		#endregion
			
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
				
			if(showFibLevels)
			{
				Dispatcher.InvokeAsync((() =>
				{
					if (myGrid2 != null)
					{
						if (fibButton != null)
						{
							myGrid2.Children.Remove(fibButton);
							fibButton.Click -= OnButtonClick;
							fibButton = null;
						}
						
					}
				}));
					
			}	
		#region TradeSimple Socials
			
			if (showSocials)
			{
				Dispatcher.InvokeAsync((() =>
				{
					if (myGrid29 != null)
					{
						if (youtubeButton != null)
						{
							myGrid29.Children.Remove(youtubeButton);
							youtubeButton.Click -= OnButtonClick;
							youtubeButton = null;
						}
						
						if (discordButton != null)
						{
							myGrid29.Children.Remove(discordButton);
							discordButton.Click -= OnButtonClick;
							discordButton = null;
						}
								
					}
				}));
			}
		#endregion	
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
					
				thisBar 	= 1;
				isThisBar 	= false;
				
				myFreeLimit 		= true;
				myFreeLimitShort 	= true;
			}
			
			if((fibButtonClicked==true) && (Position.MarketPosition == MarketPosition.Long))
			{
				Draw.FibonacciRetracements(this, "FibLong", false, thisBar, fibStop, thisBar, fibEntry);
			}
			
			if((fibButtonClicked==true) && (Position.MarketPosition == MarketPosition.Short))
			{
				Draw.FibonacciRetracements(this, "FibShort", false, thisBar, fibStopShort, thisBar, fibEntryShort);
			}
			
			if (Position.MarketPosition == MarketPosition.Flat)
				
				{
					RemoveDrawObject("FibLong");
					RemoveDrawObject("FibShort");
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
			
		
			if ((IsFirstTickOfBar) && (isThisBar == true))
			{
				thisBar ++;
				
				Print("This Bar is :  " + thisBar);
				Print("Is This Bar is :  " + isThisBar);
			}
			
			if (IsFirstTickOfBar)
			{
				myFreeLimit 		= true;
				myFreeLimitShort	= true;
			}
			
			
		#region Stop/Profit Set - Long 
	
		#region	Stop/Profit Set - Long Filled Position	
			
		if (Position.Quantity == positionSize)
		{	
			if (Position.MarketPosition == MarketPosition.Long && myFreeTradeLong == true)
			{
			
			
			if (dualTarget == false)
			{
					if (stopLoss)
					{
						ExitLongStopMarket(0, true, Position.Quantity, longStopPrice, "MyStopLong", "MyEntryLong"); //positionSize instead of Position.Q
					}
					
					if (profitTarget)
					{
						ExitLongLimit(0, true, Position.Quantity, longTargetPrice, "MyTargetLong", "MyEntryLong");
					}
			}
				
			if (dualTarget)
			{
					if(stopLoss)
					{
						ExitLongStopMarket(0, true, Position.Quantity, longStopPrice, "MyStopLong", "MyEntryLong");
						ExitLongStopMarket(0, true, Position.Quantity, longStopPrice, "MyStopLong2", "MyEntryLong2");
					}
					
					if(profitTarget)
					{
						ExitLongLimit(0, true, Position.Quantity, longTargetPrice, "MyTargetLong", "MyEntryLong");
						ExitLongLimit(0, true, Position.Quantity, longTargetPrice2, "MyTargetLong2", "MyEntryLong2");
					}	
			}
				myFreeTradeLong = false;
			}
		}	
		
		#endregion
		
		#region Stop/Profit Set - Long Partial Fill Position
		
		else if ((Position.MarketPosition == MarketPosition.Long) && (Position.Quantity < positionSize) && (IsFirstTickOfBar))
		{	
			
			if (Position.MarketPosition == MarketPosition.Long && myFreeTradeLong == true)
			{
			
			
			if (dualTarget == false)
			{
					if (stopLoss)
					{
						ExitLongStopMarket(0, true, Position.Quantity, longStopPrice, "MyStopLong", "MyEntryLong"); //positionSize instead of Position.Q
					}
					
					if (profitTarget)
					{
						ExitLongLimit(0, true, Position.Quantity, longTargetPrice, "MyTargetLong", "MyEntryLong");
					}		
			}
				
			if (dualTarget)
			{
					if(stopLoss)
					{
						ExitLongStopMarket(0, true, Position.Quantity, longStopPrice, "MyStopLong", "MyEntryLong");
						ExitLongStopMarket(0, true, Position.Quantity, longStopPrice, "MyStopLong2", "MyEntryLong2");
					}
					
					if(profitTarget)
					{
						ExitLongLimit(0, true, Position.Quantity, longTargetPrice, "MyTargetLong", "MyEntryLong");
						ExitLongLimit(0, true, Position.Quantity, longTargetPrice2, "MyTargetLong2", "MyEntryLong2");
					}	
			}
				myFreeTradeLong = false;
			}
		}	
		
		#endregion
		
			#endregion
		
		#region Stop/Profit Set - Short
		
		#region	Stop/Profit Set - Short Filled Position
			
		if (Position.Quantity == positionSize) // Need to add to Long
		{	
			
			if (Position.MarketPosition == MarketPosition.Short && myFreeTradeShort == true)
			{
				
			if (dualTarget == false)
			{
					if(stopLoss)
					{
						ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort, "MyStopShort", "MyEntryShort");
					}
					
					if(profitTarget)
					{
						ExitShortLimit(0, true, Position.Quantity, TargetPriceShort, "MyTargetShort", "MyEntryShort");
					}
			}	
			
			if (dualTarget)
			{
					if(stopLoss)
					{
						ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort, "MyStopShort", "MyEntryShort");
						ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort, "MyStopShort2", "MyEntryShort2");	
					}
					
					if(profitTarget)
					{
						ExitShortLimit(0, true, Position.Quantity, TargetPriceShort, "MyTargetShort", "MyEntryShort");
						ExitShortLimit(0, true, Position.Quantity, TargetPriceShort2, "MyTargetShort2", "MyEntryShort2");
					}
			}
				myFreeTradeShort = false;
			}
			
		}
		#endregion
		
		#region Stop/Profit Set - Short Partial Fill Position
		
		else if ((Position.MarketPosition == MarketPosition.Short) && (Position.Quantity < positionSize) && (IsFirstTickOfBar))
		{
				if (Position.MarketPosition == MarketPosition.Short && myFreeTradeShort == true)
			{
				
			if (dualTarget == false)
			{
					if(stopLoss)
					{
						ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort, "MyStopShort", "MyEntryShort");
					}
					
					if(profitTarget)
					{
						ExitShortLimit(0, true, Position.Quantity, TargetPriceShort, "MyTargetShort", "MyEntryShort");
					}
			}	
			
			if (dualTarget)
			{
					if(stopLoss)
					{
						ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort, "MyStopShort", "MyEntryShort");
						ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort, "MyStopShort2", "MyEntryShort2");	
					}
					
					if(profitTarget)
					{
						ExitShortLimit(0, true, Position.Quantity, TargetPriceShort, "MyTargetShort", "MyEntryShort");
						ExitShortLimit(0, true, Position.Quantity, TargetPriceShort2, "MyTargetShort2", "MyEntryShort2");
					}
			}
				myFreeTradeShort = false;
			}
		}
				
			#endregion
		
			#endregion
			
		
			if ((Position.MarketPosition != MarketPosition.Flat))
				return;
			
			if((!longButtonClicked) && (!shortButtonClicked))
				return;
			
			
					
		#region Entry Calculations
			
			#region Long Entry Calcs
			///Long Entry
			if((longButtonClicked) && (myFreeLimit))
			{
				
			switch(entryType)
				
				{
					
			case EntryArea.HighLow:
					{
						
				 	entryArea = High[1];
						
					longEntryPercent 	= (High[1] - Low[1]) * entryOffsetPercentage;
			 		longEntryPrice 		= entryOffsetPrice;
					longEntryTick 		= entryOffsetTick * TickSize;
			
					longOffset = Math.Max(longEntryPercent, Math.Max(longEntryPrice, longEntryTick));
						
			 		//double longEntry = (entryArea +  longOffset);
					longEntry = (entryArea +  longOffset);
						//myFreeLimit = true;
						//Print("myFreeLimit Entry Area  " + myFreeLimit);
					}
					break;
				
			case EntryArea.Close:
					{
						
				 	entryArea = Close[1];
						
					longEntryPercent 	= (High[1] - Low[1]) * entryOffsetPercentage;
			 		longEntryPrice		= entryOffsetPrice;
					longEntryTick 		= entryOffsetTick * TickSize;
			
					longOffset = Math.Max(longEntryPercent, Math.Max(longEntryPrice, longEntryTick));
			
			 		//double longEntry = (entryArea +  longOffset);
					longEntry = (entryArea +  longOffset);
		
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
			
			if((shortButtonClicked) && (myFreeLimitShort))
			{
		
			switch(entryType)
				
				{
				
			case EntryArea.HighLow:
					{
					
					entryAreaShort = Low[1];
					
					shortEntryPercent 		= (High[1] - Low[1]) * entryOffsetPercentage;
					shortEntryPrice 		= entryOffsetPrice;
					shortEntryTick 			= entryOffsetTick * TickSize;	
			
			
			 		shortEntryOffset = Math.Max(shortEntryPercent, Math.Max(shortEntryPrice, shortEntryTick));
			
					shortEntry = (entryAreaShort - shortEntryOffset);
			
							
					}
					break;
				
			case EntryArea.Close:
					{
				
					entryAreaShort = Close[1];
						
					shortEntryPercent 		= (High[1] - Low[1]) * entryOffsetPercentage;
					shortEntryPrice 		= entryOffsetPrice;
					shortEntryTick 			= entryOffsetTick * TickSize;	
			
			
			 		shortEntryOffset = Math.Max(shortEntryPercent, Math.Max(shortEntryPrice, shortEntryTick));
			
					shortEntry = (entryAreaShort - shortEntryOffset);
		
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
				
							&& (Close[0] >= longEntry)
								&& (enterMarket == false)
								
									&& (myFreeLimit)
				)
			{
					
			#region Share Size Calc
				
			switch (posCalcType)
			{
				case PositionCalc.LastBarPos:
				{
				
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
				break;
				}
				
			case PositionCalc.CustomPos:
				{
					positionSize = customQTY;
					break;
				}
				
			}
			
			if (dualTarget)
						{
						
					firstTargetPosition = (Convert.ToInt32(positionSize * splitPercent));
					if (firstTargetPosition < 1)
					{
						firstTargetPosition = 1;
					}
				
					secondTargetPosition = positionSize - firstTargetPosition; 
					if(secondTargetPosition < 1)
					{
						secondTargetPosition = 1;
					}
				
					//Print ("first Pos : " + firstTargetPosition);
					//Print ("second Pos : " + secondTargetPosition);
						}
			
				//Print ("PosSize : " + positionSize);
		#endregion
			
			#region Long Entry/Stop Calc
			///Long Entry Logic

		
			///Long Stop
			double percentStopLong 	= (High[1] - Low[1]) * stopOffsetPercentage;	
			double priceStopLong	= stopOffsetPrice;
			double tickStopLong		= stopOffsetTick * TickSize;			
				
			
			double maxStopLong = Math.Max(percentStopLong, Math.Max(priceStopLong, tickStopLong));
				
			//Changes Initial Stop Price
			longStopPrice = (Low[1] - maxStopLong);
						
		///--------Limit Price Offset---------
						
			double percentLimitLong 	= (High[1] - Low[1]) * limitOffsetPercentage;	
			double priceLimitLong		= limitOffsetPrice;
			double tickLimitLong		= limitOffsetTick * TickSize;			
				
			
			double maxLimitLong = Math.Max(percentLimitLong, Math.Max(priceLimitLong, tickLimitLong));
				
			//Changes Initial Stop Price
			longLimitPrice = (longEntry + maxLimitLong);			
						//Print("Long Limit Price is: " + longLimitPrice );
		///--------------------------------------------	
			
			entryAreaLong = entryArea;
			longOffsetBE = maxStopLong;
			
			longStopAreaFib = Low[1]; //used for fib Line for now
			longEntryAreaFib = High[1];//used for fib Line for now
			
			#endregion
				
			#region Long Profit Target Calc
					switch (targetType)
			{
		
		///ActualRR Initial Final Target	
			// Calculates Shares with Offset included. 
				case TargetCalcRR.ActualRR:
				{
					longTargetPrice = (High[1] + offsetAdded) + (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * rrTarget);
					longTargetPrice2 = (High[1] + offsetAdded) + (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * rrTarget2);
					
					fibStop = longStopAreaFib - maxStopLong;
					fibEntry = longEntryAreaFib + offsetAdded;
					
					isThisBar = true;
					int thisBar = 1;
								
				}
			
				break;
	
		///CandleRR Initial Final Target		
			// Calculates Targets with only Candle Size
				case TargetCalcRR.CandleRR:
				{		
					longTargetPrice = High[1] + ((High[1] - Low[1]) * rrTarget);
					longTargetPrice2 = High[1] + ((High[1] - Low[1]) * rrTarget2);
					
					fibStop = longStopAreaFib;
					fibEntry = longEntryAreaFib;
					
					isThisBar = true;
					int thisBar = 1;
		
				}
				
				break;
			}
				#endregion
			
			#region EnterLong
		
		//Enters With a Market Order After Entry Price has been crossed (Essentially a Stop Market Order)	
		if (useLimit == false)
		{
			if (dualTarget == false)
			{
				EnterLong(positionSize, "MyEntryLong");
			}
			
			else if (dualTarget)
			{
			
				EnterLong(secondTargetPosition, "MyEntryLong");
			
				EnterLong(firstTargetPosition, "MyEntryLong2");
				
				
			}
		}
		
		//Enters With Limit Orders After Entry Price has been crossed (Essentially a Stop Limit Order)
		if (useLimit)
		{
			if (dualTarget == false)
			{
				EnterLongLimit(positionSize, longLimitPrice, "MyEntryLong");
			}
			
			else if (dualTarget)
			{
			
				EnterLongLimit(secondTargetPosition, longLimitPrice, "MyEntryLong");
			
				EnterLongLimit(firstTargetPosition, longLimitPrice, "MyEntryLong2");
				
				
				Print("EnterLongStop1  " + secondTargetPosition);
				Print("EnterLongStop2  " + firstTargetPosition);
				
			}
		}
					myFreeLimit 		= false;
					myFreeTradeLong 	= true;
		
		//Print("myFreeLimit After Trade  " + myFreeLimit);
				
   			#endregion	
			
			#region Target Calculations
			
			switch (targetType)
			{
	///Actual RR B/E and Trail
				// Calculates Targets with Offset included.
				case TargetCalcRR.ActualRR:
			{
				entryAreaLong = High[1];
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
				entryAreaLong = High[1];
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
					
								&& (Close[0] <= shortEntry)
									&& (enterMarket == false)
					
										&& (myFreeLimitShort)
									
					)
			{
			
			#region Share size Calc
				
				switch (posCalcType)
			{
				case PositionCalc.LastBarPos:
				{
				
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
				break;
				}
				
			case PositionCalc.CustomPos:
				{
					positionSize = customQTY;
					break;
				}
				
			}
			
			if (dualTarget)
						{
						
					firstTargetPosition = (Convert.ToInt32(positionSize * splitPercent));
					if (firstTargetPosition < 1)
					{
						firstTargetPosition = 1;
					}
				
					secondTargetPosition = positionSize - firstTargetPosition; 
					if(secondTargetPosition < 1)
					{
						secondTargetPosition = 1;
					}
				
						}
			
				
				#endregion
				
			#region Short Entry/Stop Calc
			
			
			///Short Stop 
			double percentStopShort = (High[1] - Low[1]) * stopOffsetPercentage;
			double priceStopShort 	= stopOffsetPrice;
			double tickStopShort	= stopOffsetTick * TickSize;
				
			maxStopShort = Math.Max(percentStopShort, Math.Max(priceStopShort, tickStopShort));
				
			//Changes Initial Stop Price
			StopPriceShort = (High[1] + maxStopShort);
						
			///--------Limit Price Offset---------
						
			double percentLimitShort	= (High[1] - Low[1]) * limitOffsetPercentage;	
			double priceLimitShort		= limitOffsetPrice;
			double tickLimitShort		= limitOffsetTick * TickSize;			
				
			
			double maxLimitShort = Math.Max(percentLimitShort, Math.Max(priceLimitShort, tickLimitShort));
				
			//Changes Initial Stop Price
			shortLimitPrice = (shortEntry - maxLimitShort);			
						Print("ShortLimit Price is: " + shortLimitPrice );
		///--------------------------------------------				
						
			
			entryAreaShortpd = entryAreaShort;
			shortOffsetBE = maxStopShort;
			
			shortStopAreaFib = High[1]; //used for fib Line for now
			shortEntryAreaFib = Low[1];//used for fib Line for now
			
			#endregion
				
			#region Short Profit Target Calc
			
			switch (targetType)
			{
			///ActualRR Initial Final Target
				// Calculates Shares with Offset included. 
				case TargetCalcRR.ActualRR:
				{
					TargetPriceShort = (Low[1] - offsetAdded) - (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * rrTarget);
					TargetPriceShort2 = (Low[1] - offsetAdded) - (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * rrTarget2);
					
					fibStopShort = shortStopAreaFib + maxStopShort;
					fibEntryShort = shortEntryAreaFib - offsetAdded;
					
					isThisBar = true;
					int thisBar = 1;
					
				}
			
				break;
		///CandleRR Initial Final Target		
			// Calculates Targets with only Candle Size
				case TargetCalcRR.CandleRR:
				{
					TargetPriceShort = Low[1] - ((High[1] - Low[1]) * rrTarget);
					TargetPriceShort2 = Low[1] - ((High[1] - Low[1]) * rrTarget2);
					
					fibStopShort = shortStopAreaFib;
					fibEntryShort = shortEntryAreaFib;
					
					isThisBar = true;
					int thisBar = 1;
			
				}
				
				break;
			}
				#endregion
			
			
				
			#region EnterShort
		
		if (useLimit == false)
		{
			if (dualTarget == false)
			{
				EnterShort(positionSize, "MyEntryShort");
			}
			
			if (dualTarget)
			{
				EnterShort(secondTargetPosition, "MyEntryShort");
				EnterShort(firstTargetPosition, "MyEntryShort2");
			}
		}
		
		if (useLimit)
		{
			if (dualTarget == false)
			{
				EnterShortLimit(positionSize, shortLimitPrice, "MyEntryShort");
			}
			
			if (dualTarget)
			{
				EnterShortLimit(secondTargetPosition, shortLimitPrice, "MyEntryShort");
				EnterShortLimit(firstTargetPosition, shortLimitPrice, "MyEntryShort2");
			}
		}
			myFreeTradeShort 	= true;
			myFreeLimitShort	= false;
			
			#endregion
				
			#region Target Calculations
			
			switch (targetType)
			{
		/// ActualRR B/E and Trail 
			// Calculates Shares with Offset included. 
				case TargetCalcRR.ActualRR:
				{
			//----------------------------------------------------------------------------	B/E and Stop Move Targets	
				entryAreaShortpd = Low[1];	
					
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
				entryAreaShortpd = Low[1];		
					
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
		// private PositionCalc	posCalcType	= PositionCalc.LastBarPos;		
			#region Share Size Calc
				
			switch (posCalcType)
			{
				case PositionCalc.LastBarPos:
				{
				
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
				break;
				}
				
			case PositionCalc.CustomPos:
				{
					positionSize = customQTY;
					break;
				}
				
			}
			
			if (dualTarget)
						{
						
					firstTargetPosition = (Convert.ToInt32(positionSize * splitPercent));
					if (firstTargetPosition < 1)
					{
						firstTargetPosition = 1;
					}
				
					secondTargetPosition = positionSize - firstTargetPosition; 
					if(secondTargetPosition < 1)
					{
						secondTargetPosition = 1;
					}
				
				
				//		Print("SecondPos " + secondPos);
				Print("1st " + firstTargetPosition);
				Print("2nd " + secondTargetPosition);
								
				Print("Position Size is : " + positionSize);
					
						}
			

		#endregion
			
			#region Long Entry/Stop Calc
			///Long Entry Logic

		
			///Long Stop
			
			double percentStopLong 	= (High[1] - Low[1]) * stopOffsetPercentage;	
			double priceStopLong	= stopOffsetPrice;
			double tickStopLong		= stopOffsetTick * TickSize;			
				
			
			double maxStopLong = Math.Max(percentStopLong, Math.Max(priceStopLong, tickStopLong));
				
			//Changes Initial Stop Price
			longStopPrice = Math.Min((Low[1] - maxStopLong), (Low[0] - maxStopLong)); //market orders going Long when price is below where Stop would be set with other methods
			
			
			entryAreaLong = entryArea;
			longOffsetBE = maxStopLong;
			
			longStopAreaFib = Low[1]; //used for fib Line for now
			longEntryAreaFib = High[1];//used for fib Line for now
			#endregion
				
			#region Long Profit Target Calc
					switch (targetType)
			{
		
		///ActualRR Initial Final Target	
			// Calculates Shares with Offset included. 
				case TargetCalcRR.ActualRR:
				{
					longTargetPrice = (High[1] + offsetAdded) + (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * rrTarget);
					longTargetPrice2 = (High[1] + offsetAdded) + (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * rrTarget2);
					
					fibStop = longStopAreaFib - maxStopLong;
					fibEntry = longEntryAreaFib + offsetAdded;
					
					isThisBar = true;
					int thisBar = 1;
				}
			
				break;
	
		///CandleRR Initial Final Target		
			// Calculates Targets with only Candle Size
				case TargetCalcRR.CandleRR:
				{
					longTargetPrice = High[1] + ((High[1] - Low[1]) * rrTarget);
					longTargetPrice2 = High[1] + ((High[1] - Low[1]) * rrTarget2);
					
					fibStop = longStopAreaFib;
					fibEntry = longEntryAreaFib;
					
					isThisBar = true;
					int thisBar = 1;
				}
				
				break;
			}
				#endregion
			
			#region EnterLong
			
			if (dualTarget == false)
			{
				EnterLong(positionSize, "MyEntryLong");
			}
			
			//EnterLong(positionSize, "MyEntryLong");
			if (dualTarget)
			{
			
				EnterLong(secondTargetPosition, "MyEntryLong");
			
				EnterLong(firstTargetPosition, "MyEntryLong2");
			}
			
					myFreeTradeLong = true;
				
   			#endregion	
			
			#region Target Calculations
			
			switch (targetType)
			{
	///Actual RR B/E and Trail
				// Calculates Targets with Offset included.
				case TargetCalcRR.ActualRR:
			{
				entryAreaLong = High[1];
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
				entryAreaLong = High[1];
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
			
			#region Stop/Profit Set commented
			/*
			if (Position.MarketPosition == MarketPosition.Long && myFreeTradeLong == true)
			{
			
			if (dualTarget == false)
			{
				if(stopLoss)
				{
					ExitLongStopMarket(0, true, Position.Quantity, longStopPrice, "MyStopLong", "MyEntryLong");
				}
				
				if(profitTarget)
				{
					ExitLongLimit(0, true, Position.Quantity, longTargetPrice, "MyTargetLong", "MyEntryLong");
				}
			}
				
			if (dualTarget)
			{
				
				if(stopLoss)
				{
					ExitLongStopMarket(0, true, secondTargetPosition, longStopPrice, "MyStopLong", "MyEntryLong");
					
					ExitLongStopMarket(0, true, firstTargetPosition, longStopPrice, "MyStopLong2", "MyEntryLong2");
				}
				
				if(profitTarget)
				{
					ExitLongLimit(0, true, secondTargetPosition, longTargetPrice, "MyTargetLong", "MyEntryLong");
					
					ExitLongLimit(0, true, firstTargetPosition, longTargetPrice2, "MyTargetLong2", "MyEntryLong2");
				}
			}	
				myFreeTradeLong = false;
			}
			*/
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
				
			switch (posCalcType)
			{
				case PositionCalc.LastBarPos:
				{
				
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
				break;
				}
				
			case PositionCalc.CustomPos:
				{
					positionSize = customQTY;
					break;
				}
				
			}
			
			if (dualTarget)
						{
						
					firstTargetPosition = (Convert.ToInt32(positionSize * splitPercent));
					if (firstTargetPosition < 1)
					{
						firstTargetPosition = 1;
					}
				
					secondTargetPosition = positionSize - firstTargetPosition; 
					if(secondTargetPosition < 1)
					{
						secondTargetPosition = 1;
					}
				
				
				//		Print("SecondPos " + secondPos);
				Print("1st " + firstTargetPosition);
				Print("2nd " + secondTargetPosition);
								
				Print("Position Size is : " + positionSize);
					
						}
			
				
				#endregion
				
			#region Short Entry/Stop Calc
			
			
			///Short Stop 
			double percentStopShort = (High[1] - Low[1]) * stopOffsetPercentage;
			double priceStopShort 	= stopOffsetPrice;
			double tickStopShort	= stopOffsetTick * TickSize;
				
			maxStopShort = Math.Max(percentStopShort, Math.Max(priceStopShort, tickStopShort));
				
			//Changes Initial Stop Price
			StopPriceShort = Math.Max((High[1] + maxStopShort), (High[0] + maxStopShort));
			
			entryAreaShortpd = entryAreaShort;
			shortOffsetBE = maxStopShort;
			
			shortStopAreaFib = High[1]; //used for fib Line for now
			shortEntryAreaFib = Low[1];//used for fib Line for now
			
			#endregion
				
			#region Short Profit Target Calc
			
			switch (targetType)
			{
			///ActualRR Initial Final Target
				// Calculates Shares with Offset included. 
				case TargetCalcRR.ActualRR:
				{
					TargetPriceShort = (Low[1] - offsetAdded) - (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * rrTarget);
					TargetPriceShort2 = (Low[1] - offsetAdded) - (((High[1] + offsetAdded) - (Low[1] - offsetAdded)) * rrTarget2);
					
					fibStopShort = shortStopAreaFib + maxStopShort;
					fibEntryShort = shortEntryAreaFib - offsetAdded;
					
					isThisBar = true;
					int thisBar = 1;
							
				}
			
				break;
		///CandleRR Initial Final Target		
			// Calculates Targets with only Candle Size
				case TargetCalcRR.CandleRR:
				{	
					TargetPriceShort = Low[1] - ((High[1] - Low[1]) * rrTarget);
					TargetPriceShort2 = Low[1] - ((High[1] - Low[1]) * rrTarget2);
					
					fibStopShort = shortStopAreaFib;
					fibEntryShort = shortEntryAreaFib;
					
					isThisBar = true;
					int thisBar = 1;
				
				}
				
				break;
			}
				#endregion
				
			#region EnterShort
			
			if (dualTarget == false)
			{
				EnterShort(positionSize, "MyEntryShort");
			}
			
			if (dualTarget)
			{
				EnterShort(secondTargetPosition, "MyEntryShort");
				EnterShort(firstTargetPosition, "MyEntryShort2");
			}
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
				entryAreaShortpd = Low[1];	
					
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
				entryAreaShortpd = Low[1];		
					
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
			
			
			#region Stop/Profit Set commented
			/*
			
			
			if (Position.MarketPosition == MarketPosition.Short && myFreeTradeShort == true)
			{
				
			if (dualTarget == false)
			{
				if(stopLoss)
				{
				ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort, "MyStopShort", "MyEntryShort");
				}
				
				if(profitTarget)
				{
				ExitShortLimit(0, true, Position.Quantity, TargetPriceShort, "MyTargetShort", "MyEntryShort");
				}
			}	
			
			if (dualTarget)
			{
				if(stopLoss)
				{
				ExitShortStopMarket(0, true, secondTargetPosition, StopPriceShort, "MyStopShort", "MyEntryShort");
				ExitShortStopMarket(0, true, firstTargetPosition, StopPriceShort, "MyStopShort2", "MyEntryShort2");	
				}
				
				if(profitTarget)
				{
				ExitShortLimit(0, true, secondTargetPosition, TargetPriceShort, "MyTargetShort", "MyEntryShort");
				ExitShortLimit(0, true, firstTargetPosition, TargetPriceShort2, "MyTargetShort2", "MyEntryShort2");
				}
			}
				myFreeTradeShort = false;
			}
			
			*/
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

				
		if ((Position.Quantity == secondTargetPosition || Position.Quantity == firstTargetPosition) || (Position.Quantity == positionSize))
		{		
			if (dualTarget == false)
			{
				ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort, "MyStopShort", "MyEntryShort");
			}
			
			if (dualTarget)
			{
				ExitShortStopMarket(0, true, secondTargetPosition, StopPriceShort, "MyStopShort", "MyEntryShort");
				ExitShortStopMarket(0, true, firstTargetPosition, StopPriceShort, "MyStopShort2", "MyEntryShort2");
			}
		}	
		
		else if (Position.Quantity < positionSize)
		{		
			if (dualTarget == false)
			{
				ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort, "MyStopShort", "MyEntryShort");
			}
		}	
		
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

		if ((Position.Quantity == secondTargetPosition || Position.Quantity == firstTargetPosition) || (Position.Quantity == positionSize))
		{					
			if (dualTarget == false)
			{
				ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort2, "MyStopShort", "MyEntryShort");
			}
			
			if(dualTarget)
			{
				ExitShortStopMarket(0, true, secondTargetPosition, StopPriceShort2, "MyStopShort", "MyEntryShort");
				ExitShortStopMarket(0, true, firstTargetPosition, StopPriceShort2, "MyStopShort2", "MyEntryShort2");
			}
		}
		
		else if (Position.Quantity < positionSize)  
		{
			if (dualTarget == false)
			{
				ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort2, "MyStopShort", "MyEntryShort");
			}
		}
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
		
		if ((Position.Quantity == secondTargetPosition || Position.Quantity == firstTargetPosition) || (Position.Quantity == positionSize))
		{				
			if (dualTarget == false)
			{
				ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort3, "MyStopShort", "MyEntryShort");
			}
			
			if (dualTarget)
			{
				ExitShortStopMarket(0, true, secondTargetPosition, StopPriceShort3, "MyStopShort", "MyEntryShort");
				ExitShortStopMarket(0, true, firstTargetPosition, StopPriceShort3, "MyStopShort2", "MyEntryShort2");
			}
		}
		
		else if (Position.Quantity < positionSize)  
		{		
			if (dualTarget == false)
			{
				ExitShortStopMarket(0, true, Position.Quantity, StopPriceShort3, "MyStopShort", "MyEntryShort");
			}
		}
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
			
		if ((Position.Quantity == secondTargetPosition || Position.Quantity == firstTargetPosition) || (Position.Quantity == positionSize))
		{				
			if (dualTarget == false)
			{
				ExitShortStopMarket(0, true, Position.Quantity, trailStopShort, "MyStopShort", "MyEntryShort");
			}
			
			if (dualTarget)
			{
				ExitShortStopMarket(0, true, secondTargetPosition, trailStopShort, "MyStopShort", "MyEntryShort");
				ExitShortStopMarket(0, true, firstTargetPosition, trailStopShort, "MyStopShort2", "MyEntryShort2");
			}
		}
		
		else if (Position.Quantity < positionSize)  
		{		
			if (dualTarget == false)
			{
				ExitShortStopMarket(0, true, Position.Quantity, trailStopShort, "MyStopShort", "MyEntryShort");
			}
		}
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
			 
		if ((Position.Quantity == secondTargetPosition || Position.Quantity == firstTargetPosition) || (Position.Quantity == positionSize))
		{			
			if (dualTarget == false)
			{
				ExitShortStopMarket(0, true, Position.Quantity, trailStopShort, "MyStopShort", "MyEntryShort");
			}
			
			if (dualTarget)
			{
				ExitShortStopMarket(0, true, secondTargetPosition, trailStopShort, "MyStopShort", "MyEntryShort");
				ExitShortStopMarket(0, true, firstTargetPosition, trailStopShort, "MyStopShort2", "MyEntryShort2");
			}
		}
		
		else if (Position.Quantity < positionSize)  
		{		
			if (dualTarget == false)
			{
				ExitShortStopMarket(0, true, Position.Quantity, trailStopShort, "MyStopShort", "MyEntryShort");
			}
		}	
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
				Print ("Pos size BE : " + positionSize);
				
				longStopPrice = entryAreaLong - longOffsetBE;
				
				Print("Setting Stop ="+ longStopPrice);
				Print("entry AreaBE ="+ entryAreaLong);
				Print("longOffsetBE ="+ longOffsetBE);
				
				Print("Target 1: BE1 ="+ beLongTarget1);
				Print("Target 1: BE2 ="+ beLongTarget2);
				Print("Target 1: BE3 ="+ beLongTarget3);
				
		if ((Position.Quantity == secondTargetPosition || Position.Quantity == firstTargetPosition) || (Position.Quantity == positionSize))
		{		
			if (dualTarget == false)
			{
				ExitLongStopMarket(0, true, Position.Quantity, longStopPrice, "MyStopLong", "MyEntryLong");
			}
				
			if (dualTarget)
			{
				ExitLongStopMarket(0, true, secondTargetPosition, longStopPrice, "MyStopLong", "MyEntryLong");
				ExitLongStopMarket(0, true, firstTargetPosition, longStopPrice, "MyStopLong2", "MyEntryLong2");
				
				Print ("first Pos : " + firstTargetPosition);
				Print ("second Pos : " + secondTargetPosition);
			}	
		}			
		
		else if ((Position.Quantity < positionSize))// && (IsFirstTickOfBar))
		{		
			if (dualTarget == false)
			{
				ExitLongStopMarket(0, true, Position.Quantity, longStopPrice, "MyStopLong", "MyEntryLong");
				Print("LongStopPrice New in ="+ longStopPrice);
				Print("Pos Quant New ="+ Position.Quantity);
			}
		}	
			/*
		if (dualTarget)
			{
				ExitLongStopMarket(0, true, Position.Quantity/2, longStopPrice, "MyStopLong", "MyEntryLong");
				ExitLongStopMarket(0, true, Position.Quantity/2, longStopPrice, "MyStopLong2", "MyEntryLong2");
				//Print ("first Pos : " + firstTargetPosition);
					//Print ("second Pos : " + secondTargetPosition);
			}	
			*/
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
				longStopPrice2 = (Position.AveragePrice);
					
		if ((Position.Quantity == secondTargetPosition || Position.Quantity == firstTargetPosition) || (Position.Quantity == positionSize))
		{			
			if (dualTarget == false)
				{
					ExitLongStopMarket(0, true, Position.Quantity, longStopPrice2, "MyStopLong", "MyEntryLong");
				}
				
			if (dualTarget)
			{
				ExitLongStopMarket(0, true, secondTargetPosition, longStopPrice2, "MyStopLong", "MyEntryLong");
				ExitLongStopMarket(0, true, firstTargetPosition, longStopPrice2, "MyStopLong2", "MyEntryLong2");
			}	
			
		}
		
		else if (Position.Quantity < positionSize)
		{
			if (dualTarget == false)
				{
					ExitLongStopMarket(0, true, Position.Quantity, longStopPrice2, "MyStopLong", "MyEntryLong");
				}
		}	
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
			
		if ((Position.Quantity == secondTargetPosition || Position.Quantity == firstTargetPosition) || (Position.Quantity == positionSize))
		{			
			if (dualTarget == false)
			{
				ExitLongStopMarket(0, true, Position.Quantity, longStopPrice3, "MyStopLong", "MyEntryLong");
			}		
			
			if (dualTarget)
			{
				ExitLongStopMarket(0, true, secondTargetPosition, longStopPrice3, "MyStopLong", "MyEntryLong");
				ExitLongStopMarket(0, true, firstTargetPosition, longStopPrice3, "MyStopLong2", "MyEntryLong2");
			}
		}
		
		else if (Position.Quantity < positionSize)  
		{
			if (dualTarget == false)
			{
				ExitLongStopMarket(0, true, Position.Quantity, longStopPrice3, "MyStopLong", "MyEntryLong");
			}		
		}	
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
			
		if ((Position.Quantity == secondTargetPosition || Position.Quantity == firstTargetPosition) || (Position.Quantity == positionSize))
		{			
			if (dualTarget == false)
			{
				ExitLongStopMarket(0, true, Position.Quantity, longStopTrail, "MyStopLong", "MyEntryLong");
			}
			
			if (dualTarget)
			{
				ExitLongStopMarket(0, true, secondTargetPosition, longStopTrail, "MyStopLong", "MyEntryLong");
				ExitLongStopMarket(0, true, firstTargetPosition, longStopTrail, "MyStopLong2", "MyEntryLong2");
			}
		}
		
		else if (Position.Quantity < positionSize)  
		{
			if (dualTarget == false)
			{
				ExitLongStopMarket(0, true, Position.Quantity, longStopTrail, "MyStopLong", "MyEntryLong");
			}
		}
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
					
		if ((Position.Quantity == secondTargetPosition || Position.Quantity == firstTargetPosition) || (Position.Quantity == positionSize))
		{			
			if (dualTarget == false)
			{
				ExitLongStopMarket(0, true, Position.Quantity, longStopTrail, "MyStopLong", "MyEntryLong");
			}
			
			if (dualTarget)
			{
				ExitLongStopMarket(0, true, secondTargetPosition, longStopTrail, "MyStopLong", "MyEntryLong");
				ExitLongStopMarket(0, true, firstTargetPosition, longStopTrail, "MyStopLong2", "MyEntryLong2");
			}
			
		}	
		
		else if (Position.Quantity < positionSize)
		{
			if (dualTarget == false)
			{
				ExitLongStopMarket(0, true, Position.Quantity, longStopTrail, "MyStopLong", "MyEntryLong");
			}
		}	
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
			
		//-------------------------------Fib Button Added-----------------------------------------------------
			
			if (button == fibButton && button.Name == "FibButton" && button.Content == "FibLevelsOn")
			{
				button.Content = "FibLevelsOFF";
				button.Name = "FibButtonOff";
				fibButtonClicked = false;
				RemoveDrawObject("FibLong");
				Print("FibButtonClicked bool: " + fibButtonClicked);
				
				if ((!fibButtonClicked) || (Position.MarketPosition == MarketPosition.Flat))
				
				{
					RemoveDrawObject("FibLong");
					RemoveDrawObject("FibShort");
				}
				
				return;
			}
			
			if (button == fibButton && button.Name == "FibButtonOff" && button.Content == "FibLevelsOFF")
			{
				button.Content = "FibLevelsOn";
				button.Name = "FibButton";
				fibButtonClicked = true;
				Print("FibButtonClicked bool: " + fibButtonClicked);
				
				
				if((fibButtonClicked==true) && (Position.MarketPosition == MarketPosition.Long))
			{
				Draw.FibonacciRetracements(this, "FibLong", false, thisBar, fibStop, thisBar, fibEntry);
			}
			
				if((fibButtonClicked==true) && (Position.MarketPosition == MarketPosition.Short))
			{
				Draw.FibonacciRetracements(this, "FibShort", false, thisBar, fibStopShort, thisBar, fibEntryShort);
			}
				
				return;
			}
		
		//-------------------------------TradeSimple Socials-----------------------------------------------------	
			
			if (showSocials)
			{
				if (button == youtubeButton && button.Name == "YoutubeButton" && button.Content == "Youtube")
				{
					System.Diagnostics.Process.Start(youtube);
					return;
				}
				
				if (button == discordButton && button.Name == "DiscordButton" && button.Content == "Discord")
				{	
					System.Diagnostics.Process.Start(discord);
					return;
				}
			}
			
			
		}
		#endregion
		
		#region Button Disabled Event
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
		
		#region 1. Entry Offset

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
		
		/// User input variable. This variable will allow user to set the Entry a custom amount past IB High or Low
		[Display(Name = "Ticks - Entry Offset", GroupName = "1. Entry Parameters", Order = 3)]
		public int EntryOffsetTick
		{
			get{return entryOffsetTick;}
			set{entryOffsetTick = (value);}
		}
	
	/// Limit Order Parameters	
		
		[NinjaScriptProperty]
		[Display(Name = "Use Limit Order Entry", Description = "Sets a Limit Order to enter a trade.", Order = 4, GroupName = "1. Entry Parameters")]
		public bool UseLimit 
		{
		 	get{return useLimit;} 
			set{useLimit = (value);} 
		}
		
		/// User input variable. This variable will allow user to set the Entry a custom amount past IB High or Low
		[Display(Name = "Price - Limit Order offset", GroupName = "1. Entry Parameters", Order = 5)]
		public double LimitOffsetPrice
		{
			get{return limitOffsetPrice;}
			set{limitOffsetPrice = (value);}
		}
		
		
		/// User input variable. This variable will allow user to set the Entry a custom amount past IB High or Low
		[Display(Name = "Percentage - Limit Order Offset", GroupName = "1. Entry Parameters", Order = 6)]
		public double LimitOffsetPercentage
		{
			get{return limitOffsetPercentage;}
			set{limitOffsetPercentage = (value);}
		}
		
		/// User input variable. This variable will allow user to set the Entry a custom amount past IB High or Low
		[Display(Name = "Ticks - Limit Order Offset", GroupName = "1. Entry Parameters", Order = 7)]
		public int LimitOffsetTick
		{
			get{return limitOffsetTick;}
			set{limitOffsetTick = (value);}
		}
				
		
		#endregion
		
		#region 2. Stop Offset
		
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
		
		[Display(Name = "Ticks - Stop Offset", GroupName = "2. Stop Offset", Order = 3)]
		public int StopOffsetTick
		{
			get{return stopOffsetTick ;}
			set{stopOffsetTick  = (value);}
		}
		#endregion
	
		#region 3. Risk
		[Display(Name = "Position Size Method", GroupName = "3. Risk", Description="ActualRR includes offset, CandleRR uses only Candle Range", Order = 0)]
		public PositionCalc PosCalcType
		{
			get { return posCalcType; }
			set { posCalcType = value; }
		}
		
		[Display(Name = "CustomPos - Size Amount", GroupName = "3. Risk", Order = 1)]
		[Range(1, int.MaxValue)]
		public int CustomQTY
		{
			get{return customQTY;}
			set{customQTY = (value);}
		}
		
		
		[Display(Name = "Share Calculation Method", GroupName = "3. Risk", Description="ActualRR includes offset, CandleRR uses only Candle Range", Order = 2)]
		public ShareSizeRR RRType
		{
			get { return rrType; }
			set { rrType = value; }
		}
			
		/// Allows user to custom select Loss risk per trade 
		[Display(Name = "MaxLossPerTrade($)", GroupName = "3. Risk", Order = 3)]
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
		[Range(0, double.MaxValue)]
		[Display(Name = "Final Profit Target", GroupName = "4. Management - Profit Targets", Order = 2)]
		public double RRTarget
		{
			get{return rrTarget;}
			set{rrTarget = (value);}
		}
		
		[NinjaScriptProperty]
		[Display(Name = "Set a second Target", Description = "", Order = 3, GroupName = "4. Management - Profit Targets")]
		public bool DualTarget 
		{
		 	get{return dualTarget;} 
			set{dualTarget = (value);} 
		}
		
		/// Allows user to custom select Loss risk per trade
		[Range(0, 1.0)]
		[Display(Name = "First Target Share Percentage", GroupName = "4. Management - Profit Targets", Description = "Splits your profit targets. This is the percentage of the first target from the total", Order = 4)]
		public double SplitPercent
		{
			get{return splitPercent;}
			set{splitPercent = (value);}
		}
		
		[Range(0, double.MaxValue)] 
		[Display(Name = "First Profit Target", GroupName = "4. Management - Profit Targets", Order = 5)]
		public double RRTarget2
		{
			get{return rrTarget2;}
			set{rrTarget2 = (value);}
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
		
		#region 5. Fib Button
		[NinjaScriptProperty]
		[Display(Name = "ShowFibButton", Description = "Fib", Order = 0, GroupName = "5. Show Fib")]
		public bool ShowFibLevels 
		{
		 	get{return showFibLevels;} 
			set{showFibLevels = (value);} 
		}
		#endregion
		
		#region 9. Links/Buttons
		
		[NinjaScriptProperty]
		[Display(Name = "Show Social Media Buttons", Description = "", Order = 0, GroupName = "9. TradeSimple Dre's Links")]
		public bool ShowSocials 
		{
		 	get{return showSocials;} 
			set{showSocials = (value);} 
		}
		
		[NinjaScriptProperty]
		[Display(Name="Explanation Video", Order=1, GroupName="9. TradeSimple Dre's Links")]
		public  string Youtube
		{
		 	get{return youtube;} 
			set{youtube = (value);} 
		}
		
		[NinjaScriptProperty]
		[Display(Name="Discord Link", Order=2, GroupName="9. TradeSimple Dre's Links")]
		public  string Discord
		{
		 	get{return discord;} 
			set{discord = (value);} 
		}
		
		
		#endregion
		
		#endregion
		}
	}	