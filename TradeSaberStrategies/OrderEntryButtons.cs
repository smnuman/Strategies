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
namespace NinjaTrader.NinjaScript.Strategies.TradeSaberStrategies
{
	public class OrderEntryButtons : Strategy
	{	
		#region Position
		
		private double myDbl;
		
		private int positionSizeLong;
		private int positionSizeShort;
		
		private bool autoPositionSize;
		private bool customPositionSize;
		
		private double riskSize; // Got deleted. Shorts only, may need later. may delete later
		private double riskOffset;
		
		//RR Mode
		private double LongValues; 
		private double ShortValues;
			
		private double LongValuesOffset;
		private double ShortValuesOffset;
		
		//Tick Mode
		private double LongValuesOffsetTickMode;
		private double ShortValuesOffsetTickMode;
		
		private double LongValuesTickMode;
		private double ShortValuesTickMode;
		
		
		private int firstTargetPositionLong;
		private int secondTargetPositionLong;
		
		private int firstTargetPositionShort;
		private int secondTargetPositionShort;
		
		
		
		#endregion
		
		#region Custom Entries
		
		private bool activateMarket;
		private bool armMarket;
		private bool armMarketLong;
		private bool armMarketShort;
		
		private int  armMarketCount = 0;
		
		
		private bool enterClosePrice;
		
		//private bool enterBodyLongHL; May use Later
		//private bool enterBodyShortHL; May use Later
		
		#endregion
		
		#region Entry Offset
		
		private double entryAreaLong;
		private double entryAreaShort;
		
		private double entryAreaClose;
		
		private double percentageCalcEntry;
		private double priceCalcEntry;
		private double tickCalcEntry;
		private double candleBarOffsetEntry;
				
		private double enterLong;
		private double enterShort;
		
		private double enterCloseLong;
		private double enterCloseShort;
		
		#endregion
		
		#region Limit Entry Offset
	
		private double percentageCalcLimitLong;
		private double percentageCalcLimitShort;
		private double priceCalcLimit;
		private double tickCalcLimit;
		private double candleBarOffsetLimit;
				
		private double limitOffsetLong;
		private double limitOffsetShort;
		
		private double limitPriceSetLong;
		private double limitPriceSetShort;
		
		#endregion
		
		#region Stop Offset
		
		private double stopAreaLong;
		private double stopAreaShort;
		
		private double percentageCalcStop;
		private double priceCalcStop;
		private double tickCalcStop;
		private double candleBarOffsetStop;
				
		private double stopLong;
		private double stopShort;
		
		private double setStopLong;
		private double setStopShort;
		
		#endregion
		
		#region Initial Profit Targets
		
		private double setFirstTargetLong;
		private double setFinalTargetLong;
		
		private double setFirstTargetShort;
		private double setFinalTargetShort;
		#endregion
		
		#region Breakeven Offset
		
		private double breakevenAreaLong;
		private double breakevenAreaShort;
		
		private double percentageCalcBreakeven;
		private double priceCalcBreakeven;
		private double tickCalcBreakeven;
		private double candleBarOffsetBreakeven;
				
		private double breakevenLong;
		private double breakevenShort;
		
		private double setBreakevenLong;
		private double setBreakevenShort;
		
		#endregion
		
		#region Trail Offset
		
		private double trailAreaLong;
		private double trailAreaShort;
		
		private double percentageCalcTrail;
		private double priceCalcTrail;
		private double tickCalcTrail;
		private double candleBarOffsetTrail;
		
		private bool activeTrail;
		
		#endregion
		
		#region Management
		
		private bool myFreeTrade;//Regular Trade Profit/Sop
		private bool myFreeLimit;//Limit Orders
		
		//BE Area
		private bool breakevenAreaSetButton;
		private bool breakevenAreaSetAuto;
		private bool myFreeBEArea;
		private double breakevenAreaTrigger;
		private double breakevenAreaStopSet;
		
		//BE Actual
		private bool breakevenActualSetButton;
		private bool breakevenActualSetAuto;
		private bool myFreeBEActual;
		private double breakevenActualTrigger;
		private double breakevenActualStopSet;
		
		//Custom Stop
		private bool myFreeCustomStop;
		private double customStopTrigger;
		
		//Trail
		//private bool candleTrailSetButton;
		//private bool candleTrailSetAuto;
		private double candleTrailTrigger;
		
		private double candleTrailStopSetLong;
		private double candleTrailStopSetShort;
		
		private bool myFreeCandleTrail;
		private bool trailTriggeredCandle;
		//private bool myFreeCustomTrail;//Custom Trail
		
		//private bool myFreeCandleTrail;//Candle Trail
		
		#endregion
		
		#region Fib 
		
		private bool isFib;
		private int fibCount = 1;
		
		#endregion
	
		private bool countOnce;
		
		private bool myFillCheck;
		
		private int myPosition;
		
		private OrderManagement	managementType = OrderManagement.RR_Mode;
		
		public enum OrderManagement
		{
			RR_Mode,
			Tick_Mode,
		}
		
		private bool rrMode;
		private bool tickMode;
		
		private bool AddOffset = true;
		
		#region Button Clicked
		
		private bool longButtonHLClicked; 	//1
		private bool shortButtonHLClicked; 	//2 
			
		private bool longButtonMarketClicked;		//3
		private bool shortButtonMarketClicked;		//4
		
		private bool customLongClicked;				//5
		private bool customShortClicked;			//6
		
		
		private bool breakevenButtonClicked;		//8
		private bool trailButtonClicked;			//9
		
		private bool lineButtonClicked;				//10
		private bool longLineButtonClicked;
		private bool shortLineButtonClicked;
		
		private bool unlockButtonClicked;			//11
		
		private bool displayButtonClicked;			//16
		
		private bool fibButtonClicked;				//17
		
		#endregion
		
		#region Chart Trader Buttons
		
		private System.Windows.Controls.RowDefinition	addedRow;
		private Gui.Chart.ChartTab						chartTab;
		private Gui.Chart.Chart							chartWindow;
		private System.Windows.Controls.Grid			chartTraderGrid, chartTraderButtonsGrid, lowerButtonsGrid;
		private System.Windows.Controls.Button			longButtonHL, shortButtonHL, longButtonMarket, shortButtonMarket, customLong, customShort, myButton7, breakevenButton, trailButton, lineButton, unlockButton, myButton12, myButton13, myButton14, myButton15, displayButton, fibButton;
		private bool									panelActive;
		private System.Windows.Controls.TabItem			tabItem;
		
		#endregion
		
		#region TradeSaber Social
		
		private string author 								= "TradeSaber(Dre)";
		private string version 								= "Version 5.0.2 // December 2022";
		
		private string youtube								= "https://youtu.be/WvO8Gwx3Fh0"; 
		private string discord								= "https://discord.gg/2YU9GDme8j";
		private string tradeSaber							= "https://tradesaber.com/";
		
		private bool showSocials;
		
		private bool youtubeButtonClicked;
		private bool discordButtonClicked;
		private bool tradeSaberButtonClicked;
		
		private System.Windows.Controls.Button youtubeButton;
		private System.Windows.Controls.Button discordButton;
		private System.Windows.Controls.Button tradeSaberButton;
		
		
		private System.Windows.Controls.Grid myGrid29;
		
		#endregion
		
		protected override void OnStateChange()
		{
			/// 5.0.2 Bug Fixes
			/// Issue with certain Futures instruments needing more than 2 decimals. Not setting proper targets.
			/// Issue with the Close Position button not closing dual targets
			/// Issue with Breakeven, Trail Not working after hitting first target (On filled Position) ....>Partial fill management has been disabled due to issue with position size
			
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "OrderEntryButtons";
				Calculate									= Calculate.OnEachTick;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.UniqueEntries;
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
				
				#region Default Parameters
				
				//Position Size
				autoPositionSize							= true;
				MaxLossPerTrade								= 1000;
				customPositionSize							= false;
				CustomPositionAmount						= 5;
				
				//Custom Button
				armMarket									= false;
				enterClosePrice		 						= true;
				
				//Entry Offset
				PriceOffsetEntry							= 0;
				PercentageOffsetEntry						= 0;
				TickOffsetEntry								= 0;
				
				//Limit Order Offset (Offsets from Entry)
				PriceOffsetLimit							= 0;
				PercentageOffsetLimit						= 0;
				TickOffsetLimit								= 0;
				myFreeLimit									= true;
				useLimit									= false;
				
				//Dual Target Options
				dualTarget									= false;
				splitPercent								= 0.25;
				
				//Stop Offset
				stopLoss									= true;
				PriceOffsetStop								= 0;
				PercentageOffsetStop						= 0;
				TickOffsetStop								= 0;
				
				//Profit
				profitTarget								= true;
				FirstTargetRR								= 1;
				FinalTargetRR								= 5;
				
				
				//Management
				PriceOffsetBreakeven						= 0;
				PercentageOffsetBreakeven					= 0;
				TickOffsetBreakeven							= 0;
				
				breakevenAreaSetButton						= false;
				breakevenAreaSetAuto						= false;
				breakevenAreaTarget							= 1.0;
				
				breakevenActualSetButton					= true;
				breakevenActualSetAuto						= false;
				breakevenActualTarget						= 1.5;	
				
				///Custom Move - Later Use
				//customMoveStopSet							= false;
				//customMoveStopTarget						= 2.0;
				
				///customTrailSet							= false;
				///customTrailTarget						= 2.5;
				
				//TrailStop
				PriceOffsetTrail							= 0;
				PercentageOffsetTrail						= 0;
				TickOffsetTrail								= 0;
				
				
				candleTrailSetButton						= true;
				candleTrailSetAuto							= false;
				candleTrailTarget							= 1.0;							
							
				
				//Entry And Stop Line look back
				CandleLookBackEntry							= 1;
				CandleLookBackStop							= 1;
				
				CandleRange									= true;
				//AddOffset									= true;
				
		
				countOnce 									= true;
				

				DisplayText									= true; 
				
				showSocials 								= true;
				
				//Prints
				SystemPrint									= true;
				EntryPrints									= false;
				LimitPrints									= false;
				StopPrints									= false;
				PositionSizePrints							= false;
				ProfitTatgetPrints							= false;
				BreakevenPrints								= false;
				TrailPrints									= false;
				
				#endregion
			}
			else if (State == State.Configure)
			{
			}
			
			else if (State == State.DataLoaded)
			{				
				ClearOutputWindow();
				
				myDbl = Instrument.MasterInstrument.PointValue * Instrument.MasterInstrument.TickSize;
				
				
				switch (managementType)
				{
					case OrderManagement.RR_Mode:
					{
						rrMode 		= true;
						tickMode 	= false;
					}
					break;
					
					case OrderManagement.Tick_Mode:
					{
						rrMode 		= false;
						tickMode 	= true;
					}
					break;
				}
				
			}

			else if (State == State.Historical)
			{
				#region Chart Trader Buttons Load
				
				if (ChartControl != null)
				{
					ChartControl.Dispatcher.InvokeAsync(() =>
					{
						CreateWPFControls();
					});
				}
				
				#endregion
				
				#region Range/Unlock Buttons Load
				/* Removed -> Moved to chart trader
				
				if (UserControlCollection.Contains(myGrid))
					return;
				
				Dispatcher.InvokeAsync((() =>
				{
					myGrid = new System.Windows.Controls.Grid
					{
						Name = "MyCustomGrid", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top
					};
				
					System.Windows.Controls.ColumnDefinition column1 = new System.Windows.Controls.ColumnDefinition();
					System.Windows.Controls.ColumnDefinition column2 = new System.Windows.Controls.ColumnDefinition();
					
					myGrid.ColumnDefinitions.Add(column1);
					myGrid.ColumnDefinitions.Add(column2);
					
					lineButton = new System.Windows.Controls.Button
					{
						Name = "longLineButton", Content = "LongRange", Foreground = Brushes.Black, Background = Brushes.Green
					};
					
					unlockButton = new System.Windows.Controls.Button
					{
						Name = "unlockButton", Content = "Unlocked", Foreground = Brushes.Black, Background = Brushes.White
					};
				
					lineButton.Click += OnButtonClick;
					unlockButton.Click += OnButtonClick;
					
					System.Windows.Controls.Grid.SetColumn(lineButton, 0);
					System.Windows.Controls.Grid.SetColumn(unlockButton, 1);
					
					myGrid.Children.Add(lineButton);
					myGrid.Children.Add(unlockButton);
					
					UserControlCollection.Add(myGrid);
					
					longLineButtonClicked 	= true;
					unlockButtonClicked		= true;
					
				}));
				*/
				#endregion
				
				#region TradeSaber Socials
			
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
					System.Windows.Controls.ColumnDefinition column3 = new System.Windows.Controls.ColumnDefinition();
					
					myGrid29.ColumnDefinitions.Add(column1);
					myGrid29.ColumnDefinitions.Add(column2);
					myGrid29.ColumnDefinitions.Add(column3);
					
					youtubeButton = new System.Windows.Controls.Button
					{
						Name = "YoutubeButton", Content = "Youtube", Foreground = Brushes.White, Background = Brushes.Red
					};
					
					discordButton = new System.Windows.Controls.Button
					{
						Name = "DiscordButton", Content = "Discord", Foreground = Brushes.White, Background = Brushes.RoyalBlue
					};
					
					tradeSaberButton = new System.Windows.Controls.Button
					{
						Name = "TradeSaberButton", Content = "TradeSaber", Foreground = Brushes.White, Background = Brushes.DarkOrange
					};
					
					youtubeButton.Click += OnButtonClick;
					discordButton.Click += OnButtonClick;
					tradeSaberButton.Click += OnButtonClick;
					
					System.Windows.Controls.Grid.SetColumn(youtubeButton, 0);
					System.Windows.Controls.Grid.SetColumn(discordButton, 1);
					System.Windows.Controls.Grid.SetColumn(tradeSaberButton, 2);
					
					myGrid29.Children.Add(youtubeButton);
					myGrid29.Children.Add(discordButton);
					myGrid29.Children.Add(tradeSaberButton);
					
					UserControlCollection.Add(myGrid29);
				}));
			}
		#endregion
			}
			
			else if (State == State.Terminated)
			{
				#region Chart Trader Termninate
				
				if (ChartControl != null)
				{
					ChartControl.Dispatcher.InvokeAsync(() =>
					{
						DisposeWPFControls();
					});
				}
				
				#endregion
				
				#region Terminate Range/Unlock Buttons
				/* Removed -> Moved to chart trader
				
				Dispatcher.InvokeAsync((() =>
				{
					if (myGrid != null)
					{
						if (lineButton != null)
						{
							myGrid.Children.Remove(lineButton);
							lineButton.Click -= OnButtonClick;
							lineButton = null;
						}
						
						if (unlockButton != null)
						{
							myGrid.Children.Remove(unlockButton);
							unlockButton.Click -= OnButtonClick;
							unlockButton = null;
						}
						
					}
				}));
					*/
				#endregion
				
				#region Terminate TradeSaber Socials
			
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
						
						if (tradeSaberButton != null)
						{
							myGrid29.Children.Remove(tradeSaberButton);
							tradeSaberButton.Click -= OnButtonClick;
							tradeSaberButton = null;
						}		
					}
				}));
			}
		#endregion
			}
				
		}
		
		#region Button Click Events
	
		#region Buy High Button 1
		
		protected void Button1Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			if(longButtonHL.Content == "BUY HIGH" && Position.MarketPosition == MarketPosition.Flat && customLongClicked == false)
			{
				if (unlockButtonClicked == false)
				{
					countOnce = true;
				}
				
				longButtonHL.Content = "Live";
				
				
				RemoveDrawObject("EntryLine");
				RemoveDrawObject("StopLine");
				
				Draw.HorizontalLine(this, "EntryLine", enterLong, Brushes.Green);
				Draw.HorizontalLine(this, "StopLine", stopLong, Brushes.Red);
				
				longButtonHLClicked = true;

				return;
			}
			
			if (longButtonHL.Content == "Live")
			{
				ForceRefresh();
				longButtonHL.Content = "BUY HIGH";
				longButtonHLClicked = false;
				
				RemoveDrawObject("EntryLine");
				RemoveDrawObject("StopLine");
				
				return;
			}
		}
		
		#endregion
		
		#region Sell Low Button 2

		protected void Button2Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			if(shortButtonHL.Content == "SELL LOW" && Position.MarketPosition == MarketPosition.Flat && customShortClicked == false)
			{
				if (unlockButtonClicked == false)
				{
					countOnce = true;
				}
				
				shortButtonHL.Content = "Live";
				
				RemoveDrawObject("EntryLine");
				RemoveDrawObject("StopLine");
				
				Draw.HorizontalLine(this, "EntryLine", enterShort, Brushes.Green);
				Draw.HorizontalLine(this, "StopLine", stopShort, Brushes.Red);
				
				shortButtonHLClicked = true;

				return;
			}
			
			if (shortButtonHL.Content == "Live")
			{
				ForceRefresh();
				shortButtonHL.Content = "SELL LOW";
				shortButtonHLClicked = false;
				
				RemoveDrawObject("EntryLine");
				RemoveDrawObject("StopLine");
				
				return;
			}
		}
		
		#endregion
		
		#region Buy Market Button 3
		
		protected void Button3Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				countOnce = true;
				longButtonMarketClicked = true;
			}		
			return;
		}
		
		#endregion
		
		#region Sell Market Button 4
		
		protected void Button4Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				countOnce = true;
				shortButtonMarketClicked = true;
			}		
			return;
		}		
		
		#endregion
		
		#region Custom Long Button 5
		
		protected void Button5Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			
			#region Arm Market
			
			if (armMarket)
			{
				if (customLong.Content == "Custom Long" && Position.MarketPosition == MarketPosition.Flat && longButtonHLClicked == false )
				{
					if (unlockButtonClicked == false)
					{
						countOnce = true;
						Print("unlockButtfalse " + activateMarket);
					}
					
					if (unlockButtonClicked == true)
					{
						armMarket = true;
						
						Print("unlockButtTrue " + activateMarket);
					}
					customLong.Content = "MarketArmed";
					customLong.Background	= Brushes.Orange;
					customLongClicked = true;
					
					Print(customLongClicked);
					return;
				}	
				
				if (customLong.Content == "MarketArmed" && Position.MarketPosition == MarketPosition.Flat)
				{
					customLong.Content = "Custom Long";
					customLong.Background	= Brushes.MediumSeaGreen; 
					customLongClicked = false;
					armMarketCount = 0;
					Print(customLongClicked);
					
					return;
				}	
			}
			
			
			#endregion
			
			#region Close Price
			
			if (enterClosePrice)
			{
				if (customLong.Content == "Custom Long" && Position.MarketPosition == MarketPosition.Flat && longButtonHLClicked == false)
				{
					if (unlockButtonClicked == false)
					{
						countOnce = true;
					}
					
					customLong.Content = "Previous Close L";
					customLong.Background	= Brushes.Orange;
					customLongClicked = true;
					
					Print(customLongClicked);
					return;
				}	
				
				if (customLong.Content == "Previous Close L" && Position.MarketPosition == MarketPosition.Flat)
				{
					customLong.Content = "Custom Long";
					customLong.Background	= Brushes.MediumSeaGreen; 
					customLongClicked = false;
					
					Print(customLongClicked);
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					return;
				}	
			}
			
			
			#endregion
		}
		
		#endregion
		
		#region Custom Short Button 6
		
		protected void Button6Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			
			#region Arm Market
			if (armMarket)
			{
				if (customShort.Content == "Custom Short" && Position.MarketPosition == MarketPosition.Flat && shortButtonHLClicked == false)
				{
					if (unlockButtonClicked == false)
					{
						countOnce = true;
						Print("unlockButtfalse " + activateMarket);
					}
					
					if (unlockButtonClicked == true)
					{
						armMarket = true;
						
						Print("unlockButtTrue " + activateMarket);
					}
					customShort.Content = "MarketArmed";
					customShort.Background	= Brushes.Orange;
					customShortClicked = true;
					Print(customShortClicked);
					return;
				}	
				
				if (customShort.Content == "MarketArmed" && Position.MarketPosition == MarketPosition.Flat)
				{
					customShort.Content = "Custom Short";
					customShort.Background	= Brushes.IndianRed; 
					customShortClicked = false;
					armMarketCount = 0;
					Print(customShortClicked);
					
					return;
				}	
			}
			
			
			#endregion
			
			#region Close Price
			
			if (enterClosePrice)
			{
				if (customShort.Content == "Custom Short" && Position.MarketPosition == MarketPosition.Flat && shortButtonHLClicked == false)
				{
					if (unlockButtonClicked == false)
				{
					countOnce = true;
				}
					customShort.Content = "Previous Close S";
					customShort.Background	= Brushes.Orange;
					customShortClicked = true;
				
					Print(customShortClicked);
					return;
				}	
				
				if (customShort.Content == "Previous Close S" && Position.MarketPosition == MarketPosition.Flat)
				{
					customShort.Content = "Custom Short";
					customShort.Background	= Brushes.IndianRed; 
					customShortClicked = false;
					Print(customShortClicked);
					
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					return;
				}	
			}
			#endregion
										
		}		
		
		#endregion
		
		#region Close Position Button 7
		
		protected void Button7Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();

			ExitLong(Position.Quantity);
			ExitShort(Position.Quantity);						
		}		
		
		#endregion //Swapped Position after
		
		#region Breakeven Button 8
		
		protected void Button8Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			if (Position.MarketPosition == MarketPosition.Long && Close[0] > Position.AveragePrice)
			{
				breakevenButtonClicked = true;
				
				if (breakevenAreaSetButton)
				{
					myFreeBEArea = true;
				}
				
				if (breakevenActualSetButton)
				{
					myFreeBEActual = true;
				}	
			}
			
				if (Position.MarketPosition == MarketPosition.Short && Close[0] < Position.AveragePrice)
			{
				breakevenButtonClicked = true;
				
				if (breakevenAreaSetButton)
				{
					myFreeBEArea = true;
				}
				
				if (breakevenActualSetButton)
				{
					myFreeBEActual = true;
				}	
			}
			return;							
		}		
		
		#endregion
		
		#region Trail Button 9 // Needs custom added
		
		protected void Button9Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			
			if (candleTrailSetButton)
			{
				if (trailButton.Content == "Trail Stop" && Position.MarketPosition != MarketPosition.Flat)
				{
					trailButton.Content = "Trail Active!";
					trailButton.Background	= Brushes.Goldenrod; 
					
					myFreeCandleTrail 	= true;
					trailButtonClicked 	= true;
					activeTrail 		= true;
					
					Print("trailButtonClicked " + trailButtonClicked);
					return;
					
				}
				
				if (trailButton.Content == "Trail Active!" && Position.MarketPosition != MarketPosition.Flat)
				{
					trailButton.Content = "Trail Stop";
					trailButton.Background	= Brushes.LightGray; 
					
					trailButtonClicked = false;
					activeTrail = false;
					
					return;
				}
			}
			
				
				if (candleTrailSetButton)
				{
					myFreeCandleTrail = true;
				}
			
			return;							
		}		
		
		#endregion
		
		#region Range Line Button 10
		
		protected void Button10Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			
			#region Hide -> Long
			
			if (lineButton.Content == "HideRange")
			{
				lineButton.Content = "LongRange";
				lineButton.Foreground = Brushes.Black; 
				lineButton.Background = Brushes.Green;
				
				longLineButtonClicked = true;
				shortLineButtonClicked = false;
				countOnce = true;
				
				if (AddOffset)
					{
						RemoveDrawObject("EntryLine");
						RemoveDrawObject("StopLine");
						
						Draw.HorizontalLine(this, "EntryLine", enterLong, Brushes.Green);
						Draw.HorizontalLine(this, "StopLine", stopLong, Brushes.Red);
					}
					
					else if (AddOffset == false)
					{
						RemoveDrawObject("EntryLine");
						RemoveDrawObject("StopLine");
						
						Draw.HorizontalLine(this, "EntryLine", entryAreaLong, Brushes.Green);
						Draw.HorizontalLine(this, "StopLine", stopAreaLong, Brushes.Red);
					}
			
					return;
			}
			
			#endregion
			
			#region Long -> Short
			
			if (lineButton.Content == "LongRange") 
			{
				lineButton.Content = "ShortRange";
				lineButton.Foreground = Brushes.White; 
				lineButton.Background = Brushes.Red;
				
				longLineButtonClicked = false;
				shortLineButtonClicked = true;
				countOnce = true;
				
				
					if (AddOffset)
					{
						RemoveDrawObject("EntryLine");
						RemoveDrawObject("StopLine");
						
						Draw.HorizontalLine(this, "EntryLine", enterShort, Brushes.Green);
						Draw.HorizontalLine(this, "StopLine", stopShort, Brushes.Red);
					}
					
					else if (AddOffset == false)
					{
						RemoveDrawObject("EntryLine");
						RemoveDrawObject("StopLine");
						
						Draw.HorizontalLine(this, "EntryLine", entryAreaShort, Brushes.Green);
						Draw.HorizontalLine(this, "StopLine", stopAreaShort, Brushes.Red);
					}
					
				return;
			}
			
			#endregion
			
			#region Short -> Hide
			
			if (lineButton.Content == "ShortRange")
			{
				lineButton.Content = "HideRange";
				lineButton.Foreground = Brushes.White; 
				lineButton.Background = Brushes.Black;
				
				RemoveDrawObject("EntryLine");
				RemoveDrawObject("StopLine");
				
				
				longLineButtonClicked = false;
				shortLineButtonClicked = false;
				countOnce = true;
				
				return;
			}
			
			#endregion
			
		}		
		
		#endregion
		
		#region Unlocked Button 11
		
		protected void Button11Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			
			if (unlockButton.Content == "Unlocked")
			{
				unlockButton.Content = "Locked";
				unlockButton.Foreground = Brushes.White; 
				unlockButton.Background = Brushes.Black;
				
				unlockButtonClicked = true;

				return;
			}
			
			if (unlockButton.Content == "Locked")	
			{
				unlockButton.Content = "Unlocked";
				unlockButton.Foreground = Brushes.Black; 
				unlockButton.Background = Brushes.White;
				
				unlockButtonClicked = false;
				countOnce = true;

				return;
			}
		}		
		
		#endregion
		
		#region EntryLine ++ Button 12
		
		protected void Button12Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			CandleLookBackEntry ++;
			armMarketCount --;
			
			countOnce = true;
			Print("CandleLookBackEntry " + CandleLookBackEntry);
			return;
		}		
		
		#endregion
		
		#region EntryLine -- Button 13
		
		protected void Button13Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			
			if (CandleLookBackEntry > 1)
			{
				CandleLookBackEntry --;
				armMarketCount --;
			
				countOnce = true;
				Print("CandleLookBackEntry " + CandleLookBackEntry);
				return;
			}								
		}		
		
		#endregion
		
		#region StopLine ++ Button 14
		
		protected void Button14Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			
			if (rrMode)
			{
				CandleLookBackStop ++;
				armMarketCount --;
			
				countOnce = true;
				Print("CandleLookBackStop " + CandleLookBackStop);
				return;
			}
			
			if (tickMode)
			{
				TickOffsetStop ++;
				
				countOnce = true;
				Print("Stop From Entry (Ticks) " + TickOffsetStop);
				return;
			}
			
		}		
		
		#endregion
		
		#region StopLine -- Button 15
		
		protected void Button15Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			
			if (rrMode)
			{
				if (CandleLookBackStop > 1)
				{
					CandleLookBackStop --;
					armMarketCount --;
			
					countOnce = true;
					Print("CandleLookBackStop " + CandleLookBackStop);
					return;
				}							
			}
			
			if (tickMode)
			{
				if ((TickOffsetStop) > 1)
				{
					TickOffsetStop --;
					
					countOnce = true;
					Print("Stop From Entry (Ticks) " + TickOffsetStop);
					return;
				}
			}
							
		}		
		
		#endregion
		
		#region Display Button 16
		
		protected void Button16Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			
			if (displayButton.Content == "Show Display" && DisplayText == true)
			{
				displayButton.Content = "Hide Display";
				displayButton.Foreground = Brushes.White; 
				displayButton.Background = Brushes.Black;
				
				DisplayText = false;
				Print(DisplayText);
				RemoveDrawObject("TextBox");
				
				//countOnce = true;
				return;
			}
			
			if (displayButton.Content == "Hide Display" && DisplayText == false)
			{
				displayButton.Content = "Show Display";
				displayButton.Foreground = Brushes.Black; 
				displayButton.Background = Brushes.White;
				
				DisplayText = true;
				Print(DisplayText);
				
				if (longButtonHLClicked == true || longButtonMarketClicked == true || longLineButtonClicked == true || customLongClicked == true || Position.MarketPosition == MarketPosition.Long)
				{
					#region Draw Lines / Text Long Offset
		
						#region RR mode
						
					if (rrMode)
					{
						if (DisplayText)
						{
							Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(enterLong, 2) + ("("+CandleLookBackEntry+")")
							+ "\nStop Line: " + Math.Round(stopLong, 2) + ("("+CandleLookBackStop+")")
							
							+ "\n\nMax Loss($): " + MaxLossPerTrade	
							+ "\nRange($): " + LongValuesOffset
							+ "\nRange(Ticks): " + LongValuesOffset / TickSize
							+ "\nPosition Size: " + positionSizeLong	
						
							,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
						}
					}
						
						#endregion 
					
						#region Tick Mode
					
					if (tickMode)
					{
						if (DisplayText)
						{
							Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(enterLong, 2) + ("("+CandleLookBackEntry+")")
							+ "\nStop Line: " + Math.Round(enterLong - candleBarOffsetStop, 2) + ("("+candleBarOffsetStop / TickSize+")")
							
							+ "\n\nMax Loss($): " + MaxLossPerTrade	
							+ "\nRange($): " + candleBarOffsetStop
							+ "\nRange(Ticks): " + candleBarOffsetStop / TickSize
							+ "\nPosition Size: " + positionSizeLong	
						
							,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
						}
					}
				
					#endregion
					
					#endregion
				}
				
				if (shortButtonHLClicked == true || shortButtonMarketClicked == true || shortLineButtonClicked == true || customShortClicked == true || Position.MarketPosition == MarketPosition.Short)
				{
					#region Draw Lines / Text Short Offset
				
						#region RR Mode
						
				if (rrMode)
				{
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(enterShort, 2) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round(stopShort, 2) + ("("+CandleLookBackStop+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + ShortValuesOffset
						+ "\nRange(Ticks): " + ShortValuesOffset / TickSize
						+ "\nPosition Size: " + positionSizeShort	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
					
				}
						
					#endregion
					
						#region Tick Mode
						
				if (tickMode)
				{					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(enterShort, 2) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round(enterShort + candleBarOffsetStop, 2) + ("("+candleBarOffsetStop / TickSize+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + candleBarOffsetStop
						+ "\nRange(Ticks): " + candleBarOffsetStop / TickSize
						+ "\nPosition Size: " + positionSizeShort	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
					
				}
						
					#endregion
					
				#endregion
				}
				
				//countOnce = true;
				return;
			}
												
		}		
		
		#endregion
		
		#region Fib Button 17
		
		protected void Button17Click(object sender, RoutedEventArgs e)
		{
			ForceRefresh();
			
			if (fibButton.Content == "HideFib")
			{
				fibButton.Content = "ShowFib";
				fibButton.Foreground = Brushes.Black; 
				fibButton.Background = Brushes.White;
				
				fibButtonClicked = true;
				
				if (fibButtonClicked && Position.MarketPosition == MarketPosition.Long)
				{
					if (rrMode)
					{
						Draw.FibonacciRetracements(this, "FibLong", false, fibCount, setStopLong, fibCount, enterLong);
					}
				
					if (tickMode)
					{
						Draw.FibonacciRetracements(this, "FibLong", false, fibCount, Position.AveragePrice - candleBarOffsetStop, fibCount, Position.AveragePrice);
					}
				}
			
			if (fibButtonClicked && Position.MarketPosition == MarketPosition.Short)
			{
				if (rrMode)
				{
					Draw.FibonacciRetracements(this, "FibShort", false, fibCount, setStopShort, fibCount, enterShort);
				}
				
				if (tickMode)
				{
					Draw.FibonacciRetracements(this, "FibShort", false, fibCount, Position.AveragePrice + candleBarOffsetStop, fibCount, Position.AveragePrice);
				}
			}
					
				return;
			}
			
			if (fibButton.Content == "ShowFib")
			{
				fibButton.Content = "HideFib";
				fibButton.Foreground = Brushes.White; 
				fibButton.Background = Brushes.Black;
				
				fibButtonClicked = false;
				
				if ((!fibButtonClicked) || (Position.MarketPosition == MarketPosition.Flat))
				
				{
					RemoveDrawObject("FibLong");
					RemoveDrawObject("FibShort");
				}
				
				return;
			}
		}		
		
		#endregion
		
		#endregion
		
		#region Button Controls
		
		protected void CreateWPFControls()
		{
			#region Create WPF Controls
			
				#region Button Grid
			
			chartWindow				= Window.GetWindow(ChartControl.Parent) as Gui.Chart.Chart;

			// if not added to a chart, do nothing
			if (chartWindow == null)
				return;

			// this is the entire chart trader area grid
			chartTraderGrid			= (chartWindow.FindFirst("ChartWindowChartTraderControl") as Gui.Chart.ChartTrader).Content as System.Windows.Controls.Grid;

			// this grid contains the existing chart trader buttons
			chartTraderButtonsGrid	= chartTraderGrid.Children[0] as System.Windows.Controls.Grid;

			// this grid is to organize stuff below
			lowerButtonsGrid = new System.Windows.Controls.Grid();

			//Makes 2 Columns
			lowerButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());			
			lowerButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());		
			
			//Makes Rows
			lowerButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition()	); //Buy High / Sell Low
			lowerButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition()	); //Market
			lowerButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition()	); //Custom
			lowerButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition()	); //Close Position
			lowerButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition()	); //Management
			lowerButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition()	); //EntryLines 
			lowerButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition()	); //EntryLine +-
			lowerButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition()	); //StopLine +-
			lowerButtonsGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition()	); //Display / Fib
			
			addedRow	= new System.Windows.Controls.RowDefinition() { Height = new GridLength(240) };
				

			// this style (provided by NinjaTrader_MichaelM) gives the correct default minwidth (and colors) to make buttons appear like chart trader buttons
			Style basicButtonStyle	= Application.Current.FindResource("BasicEntryButton") as Style;
			
			#endregion

				#region Button Content
			
				longButtonHL = new System.Windows.Controls.Button()//1
				{		
					Name			= "longButtonHL",
					Content			= "BUY HIGH",
					Height			= 25,
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
						
				};				

				shortButtonHL = new System.Windows.Controls.Button()//2
				{		
					Name			= "shortButtonHL",
					Content			= "SELL LOW",
					Height			= 25,
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				longButtonMarket = new System.Windows.Controls.Button()//3
				{		
					
					Content			= string.Format("Buy Market"),
					Height			= 25,
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				shortButtonMarket = new System.Windows.Controls.Button()//4
				{		
					
					Content			= string.Format("Short Market"),
					Height			= 25,
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				customLong = new System.Windows.Controls.Button()//5
				{		
					
					Content			= "Custom Long",
					Height			= 25,
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				customShort = new System.Windows.Controls.Button()//6
				{		
					
					Content			= "Custom Short",
					Height			= 25,
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				myButton7 = new System.Windows.Controls.Button()//7
				{		
					
					Content			= string.Format("Close Position"),
					Height			= 25,
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				breakevenButton = new System.Windows.Controls.Button()//8
				{		
					
					Content			= string.Format("BreakEven"),
					Height			= 25, 
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				trailButton = new System.Windows.Controls.Button()//9
				{		
					
					Content			= "Trail Stop",
					Height			= 25, 
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				lineButton = new System.Windows.Controls.Button()//10
				{		
					
					Content			= "HideRange",
					Height			= 25, 
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				unlockButton = new System.Windows.Controls.Button()//11
				{		
					
					Content			= "Unlocked",
					Height			= 25, 
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
			
				myButton12 = new System.Windows.Controls.Button()//12
				{		
					
					Content			= string.Format("EntryLine ++"),
					Height			= 25, 
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				myButton13 = new System.Windows.Controls.Button()//13
				{		
					
					Content			= string.Format("EntryLine --"),
					Height			= 25, 
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				myButton14 = new System.Windows.Controls.Button()//14
				{		
					
					Content			= string.Format("StopLine ++"),
					Height			= 25, 
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				myButton15 = new System.Windows.Controls.Button()//15
				{		
					
					Content			= string.Format("StopLine --"),
					Height			= 25, 
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				
				
				
				if (DisplayText)
				{
					displayButton = new System.Windows.Controls.Button()//16
					{		
						
						Content			= "Show Display",
						Height			= 25, 
						Margin			= new Thickness(5,0,5,0),
						Padding			= new Thickness(0,0,0,0),
						Style			= basicButtonStyle
					};		
				}
				
				if (DisplayText == false)
				{
					displayButton = new System.Windows.Controls.Button()//16 (2)
					{		
						
						Content			= "Hide Display",
						Height			= 25, 
						Margin			= new Thickness(5,0,5,0),
						Padding			= new Thickness(0,0,0,0),
						Style			= basicButtonStyle
					};		
				}
				
				
				
				fibButton = new System.Windows.Controls.Button()//17
				{		
					
					Content			= "HideFib",
					Height			= 25, 
					Margin			= new Thickness(5,0,5,0),
					Padding			= new Thickness(0,0,0,0),
					Style			= basicButtonStyle
				};		
				#endregion
				
				#region Button Colors
					
				//1. Buy High
					longButtonHL.Background	= Brushes.ForestGreen;
					longButtonHL.BorderBrush	= Brushes.Black;
					longButtonHL.Foreground    = Brushes.Black;	
					longButtonHL.BorderThickness = new Thickness(2.0);
					longButtonHL.IsEnabled = true;

				//2. Sell Low
					shortButtonHL.Background	= Brushes.Firebrick;
					shortButtonHL.BorderBrush	= Brushes.Black;	
					shortButtonHL.Foreground    = Brushes.White;	
					shortButtonHL.BorderThickness = new Thickness(2.0);			
				
				//3. Long Market
					longButtonMarket.Background	= Brushes.MediumSeaGreen;
					longButtonMarket.BorderBrush	= Brushes.Black;	
					longButtonMarket.Foreground    = Brushes.Black;	
					longButtonMarket.BorderThickness = new Thickness(2.0);				
				
				//.4 Short Market
					shortButtonMarket.Background	= Brushes.IndianRed;
					shortButtonMarket.BorderBrush	= Brushes.Black;	
					shortButtonMarket.Foreground    = Brushes.White;	
					shortButtonMarket.BorderThickness = new Thickness(2.0);		
				
				//5. Custom Long
					customLong.Background	= Brushes.MediumSeaGreen;
					customLong.BorderBrush	= Brushes.Black;	
					customLong.Foreground    = Brushes.Black;	
					customLong.BorderThickness = new Thickness(2.0);	
				
				//6. Custom Short
					customShort.Background	= Brushes.IndianRed;
					customShort.BorderBrush	= Brushes.Black;	
					customShort.Foreground    = Brushes.White;	
					customShort.BorderThickness = new Thickness(2.0);	
				
				//7. Close Position - Swithced Position after
					myButton7.Background	= Brushes.DarkOrange;
					myButton7.BorderBrush	= Brushes.Black;	
					myButton7.Foreground    = Brushes.Black;	
					myButton7.BorderThickness = new Thickness(2.0);
				
				//8. Breakeven
					breakevenButton.Background	= Brushes.LightGray;
					breakevenButton.BorderBrush	= Brushes.Black;	
					breakevenButton.Foreground    = Brushes.DarkGreen;	
					breakevenButton.BorderThickness = new Thickness(2.0);
				
				//9. Trail
					trailButton.Background	= Brushes.LightGray;
					trailButton.BorderBrush	= Brushes.Black;	
					trailButton.Foreground    = Brushes.DarkGreen;	
					trailButton.BorderThickness = new Thickness(2.0);
				
				//10. Line
					lineButton.Background	= Brushes.Black;
					lineButton.BorderBrush	= Brushes.Black;	
					lineButton.Foreground    = Brushes.White;	
					lineButton.BorderThickness = new Thickness(2.0);
				
				//11. Unlocked
					unlockButton.Background	= Brushes.White;
					unlockButton.BorderBrush	= Brushes.Black;	
					unlockButton.Foreground    = Brushes.Black;	
					unlockButton.BorderThickness = new Thickness(2.0);
				
				//12. EntryLine ++
					myButton12.Background	= Brushes.PaleGreen;
					myButton12.BorderBrush	= Brushes.Black;	
					myButton12.Foreground    = Brushes.DarkGreen;	
					myButton12.BorderThickness = new Thickness(2.0);
				
				//13. EntryLine --
					myButton13.Background	= Brushes.PaleGreen;
					myButton13.BorderBrush	= Brushes.Black;	
					myButton13.Foreground    = Brushes.Firebrick;	
					myButton13.BorderThickness = new Thickness(2.0);
				
				//14. StopLine ++
					myButton14.Background	= Brushes.LightSalmon;
					myButton14.BorderBrush	= Brushes.Black;	
					myButton14.Foreground    = Brushes.DarkGreen;	
					myButton14.BorderThickness = new Thickness(2.0);
				
				//15. StopLine --
					myButton15.Background	= Brushes.LightSalmon;
					myButton15.BorderBrush	= Brushes.Black;	
					myButton15.Foreground    = Brushes.Firebrick;	
					myButton15.BorderThickness = new Thickness(2.0);
				
				//16.
				if (DisplayText)
				{
					displayButton.Background	= Brushes.White;
					displayButton.BorderBrush	= Brushes.Black;	
					displayButton.Foreground    = Brushes.Black;	
					displayButton.BorderThickness = new Thickness(2.0);
				}
				
				if (DisplayText == false)
				{
					displayButton.Background	= Brushes.Black;
					displayButton.BorderBrush	= Brushes.Black;	
					displayButton.Foreground    = Brushes.White;	
					displayButton.BorderThickness = new Thickness(2.0);
				}
					
				
				//17.
					fibButton.Background	= Brushes.Black;
					fibButton.BorderBrush	= Brushes.Black;	
					fibButton.Foreground    = Brushes.White;	
					fibButton.BorderThickness = new Thickness(2.0);
				
			
			#endregion	
				
				#region Button Click
				
					longButtonHL.Click +=  Button1Click;
					shortButtonHL.Click +=  Button2Click;
					
					longButtonMarket.Click +=  Button3Click;
					shortButtonMarket.Click +=  Button4Click;	
				
			
					customLong.Click +=  Button5Click;
					customShort.Click +=  Button6Click;
				
					myButton7.Click +=  Button7Click;
				
					breakevenButton.Click +=  Button8Click;
					trailButton.Click +=  Button9Click;
				
					lineButton.Click +=  Button10Click;
					unlockButton.Click +=  Button11Click;
				
					myButton12.Click +=  Button12Click;
				
					myButton13.Click +=  Button13Click;
				
					myButton14.Click +=  Button14Click;
					myButton15.Click +=  Button15Click;
				
					displayButton.Click +=  Button16Click;
					fibButton.Click +=  Button17Click;
				
				#endregion
				
				#region Button Location
				
					//High Low				
					System.Windows.Controls.Grid.SetColumn(longButtonHL, 0);				
					System.Windows.Controls.Grid.SetColumn(shortButtonHL, 1);
					
					//Market
					System.Windows.Controls.Grid.SetColumn(longButtonMarket, 0);				
					System.Windows.Controls.Grid.SetRow(longButtonMarket, 1);					
							
					System.Windows.Controls.Grid.SetColumn(shortButtonMarket, 1);				
					System.Windows.Controls.Grid.SetRow(shortButtonMarket, 1);						
				
					//Custom
					System.Windows.Controls.Grid.SetColumn(customLong, 0);				
					System.Windows.Controls.Grid.SetRow(customLong, 2);
				
					System.Windows.Controls.Grid.SetColumn(customShort, 1);				
					System.Windows.Controls.Grid.SetRow(customShort, 2);
				
					//Breakeven
					System.Windows.Controls.Grid.SetColumn(breakevenButton, 0);				
					System.Windows.Controls.Grid.SetRow(breakevenButton, 3);
				
					//Trail
					System.Windows.Controls.Grid.SetColumn(trailButton, 1);				
					System.Windows.Controls.Grid.SetRow(trailButton, 3);
				
					//Close
					System.Windows.Controls.Grid.SetColumn(myButton7, 1);				
					System.Windows.Controls.Grid.SetRow(myButton7, 4);
				
					//Range
					System.Windows.Controls.Grid.SetColumn(lineButton, 0);				
					System.Windows.Controls.Grid.SetRow(lineButton, 5);
				
					//Unlocked
					System.Windows.Controls.Grid.SetColumn(unlockButton, 1);				
					System.Windows.Controls.Grid.SetRow(unlockButton, 5);
				
					//EntryLine ++
					System.Windows.Controls.Grid.SetColumn(myButton12, 0);				
					System.Windows.Controls.Grid.SetRow(myButton12, 6);
					
					//EntryLine --
					System.Windows.Controls.Grid.SetColumn(myButton13, 1);				
					System.Windows.Controls.Grid.SetRow(myButton13, 6);
				
					//StopLine ++
					System.Windows.Controls.Grid.SetColumn(myButton14, 0);				
					System.Windows.Controls.Grid.SetRow(myButton14, 7);
				
					//StopLine --
					System.Windows.Controls.Grid.SetColumn(myButton15, 1);				
					System.Windows.Controls.Grid.SetRow(myButton15, 7);
				
					//Display
					System.Windows.Controls.Grid.SetColumn(displayButton, 0);				
					System.Windows.Controls.Grid.SetRow(displayButton, 8);
				
					//Fib
					System.Windows.Controls.Grid.SetColumn(fibButton, 1);				
					System.Windows.Controls.Grid.SetRow(fibButton, 8);
				
				#endregion
				
				#region Add Buttons
				
					lowerButtonsGrid.Children.Add(longButtonHL);							
					lowerButtonsGrid.Children.Add(shortButtonHL);
				
				
					lowerButtonsGrid.Children.Add(longButtonMarket);
					lowerButtonsGrid.Children.Add(shortButtonMarket);				
				
					lowerButtonsGrid.Children.Add(customLong);
					lowerButtonsGrid.Children.Add(customShort);	
				
					lowerButtonsGrid.Children.Add(myButton7);
				
					lowerButtonsGrid.Children.Add(breakevenButton);
				
					lowerButtonsGrid.Children.Add(trailButton);
				
					lowerButtonsGrid.Children.Add(lineButton);
					lowerButtonsGrid.Children.Add(unlockButton);
				
					lowerButtonsGrid.Children.Add(myButton12);
					lowerButtonsGrid.Children.Add(myButton13);
				
					lowerButtonsGrid.Children.Add(myButton14);
					lowerButtonsGrid.Children.Add(myButton15);
				
					lowerButtonsGrid.Children.Add(displayButton);
					lowerButtonsGrid.Children.Add(fibButton);
				
				#endregion
				
			if (TabSelected())
				InsertWPFControls();

			chartWindow.MainTabControl.SelectionChanged += TabChangedHandler;
			
			#endregion
		}
	
		public void DisposeWPFControls() 
		{
			#region Dispose
			
			if (chartWindow != null)
				chartWindow.MainTabControl.SelectionChanged -= TabChangedHandler;

			if (longButtonHL != null)
				longButtonHL.Click -= Button1Click;

			if (shortButtonHL != null)
				shortButtonHL.Click -= Button2Click;
			
			if (longButtonMarket != null)
				longButtonMarket.Click -= Button3Click;
			
			if (shortButtonMarket != null)
				shortButtonMarket.Click -= Button4Click;
			
			if (customLong != null)
				customLong.Click -= Button5Click;
			
			if (customShort != null)
				customShort.Click -= Button6Click;
			
			if (myButton7 != null)
				myButton7.Click -= Button7Click;
			
			if (breakevenButton != null)
				breakevenButton.Click -= Button8Click;
				
			if (trailButton != null)
				trailButton.Click -= Button9Click;	
			
			if (lineButton != null)
				lineButton.Click -= Button10Click;
			
			if (unlockButton != null)
				unlockButton.Click -= Button11Click;
			
			if (myButton12 != null)
				myButton12.Click -= Button12Click;
			
			if (myButton13 != null)
				myButton13.Click -= Button13Click;
			
			if (myButton14 != null)
				myButton14.Click -= Button14Click;
			
			if (myButton15 != null)
				myButton15.Click -= Button15Click;
			
			if (displayButton != null)
				displayButton.Click -= Button16Click;
			
			if (fibButton != null)
				fibButton.Click -= Button17Click;

			RemoveWPFControls();
			
			#endregion
		}
		
		public void InsertWPFControls()
		{
			#region Insert WPF
			
			if (panelActive)
				return;
			
			// add a new row (addedRow) for our lowerButtonsGrid below the ask and bid prices and pnl display			
			chartTraderGrid.RowDefinitions.Add(addedRow);
			System.Windows.Controls.Grid.SetRow(lowerButtonsGrid, (chartTraderGrid.RowDefinitions.Count - 1));
			chartTraderGrid.Children.Add(lowerButtonsGrid);

			panelActive = true;
			
			#endregion
		}
		
		#endregion
		
	
		protected override void OnBarUpdate()
		{
			if (State != State.Realtime ) 
				return;
			
			if (countOnce && Position.MarketPosition == MarketPosition.Flat && unlockButtonClicked == false)
			{
				
				if (CandleRange == false)
				{
					#region Entry Offset
					
			if (longButtonHLClicked == true || longButtonMarketClicked == true || longLineButtonClicked == true || (customLongClicked == true && enterClosePrice == false)
				|| shortButtonHLClicked == true || shortButtonMarketClicked == true || shortLineButtonClicked == true || (customShortClicked == true && enterClosePrice == false)
				)				
			{
				//HL
				entryAreaLong		= High[CandleLookBackEntry];
				entryAreaShort		= Low[CandleLookBackEntry];	
			}
				
			if ((customLongClicked || customShortClicked) && enterClosePrice == true)
			{
				//Previous Close Price	
				entryAreaLong	= Close[CandleLookBackEntry];
				entryAreaShort	= Close[CandleLookBackEntry];	
			}
				
					
			//Adds offset to your entry area. Gives user customization.
			percentageCalcEntry 	= ((High[CandleLookBackEntry] - Low[CandleLookBackEntry]) * PercentageOffsetEntry);
			priceCalcEntry 			= PriceOffsetEntry;
			tickCalcEntry			= TickOffsetEntry * TickSize;
			
			//Picks the highest of the 3 numbers
			candleBarOffsetEntry = Math.Max(percentageCalcEntry, Math.Max(priceCalcEntry, tickCalcEntry));
			
			//Add both of them together to define final entry point
			enterLong = entryAreaLong + candleBarOffsetEntry;
			enterShort = entryAreaShort - candleBarOffsetEntry;
			
			
			#region Entry Prints
			
			if (SystemPrint)
			{
				if (EntryPrints)
				{
					Print("percentageCalcEntry " + percentageCalcEntry + " " + Time[CandleLookBackEntry]);
					Print("priceCalcEntry " + priceCalcEntry + " " + Time[CandleLookBackEntry]);
					Print("tickCalcEntry " + tickCalcEntry + " " + Time[CandleLookBackEntry]);
					
					Print("candleBarOffsetEntry " + candleBarOffsetEntry + " " + Time[CandleLookBackEntry]);
					
					Print("enterLong " + enterLong + " " + Time[CandleLookBackEntry]);
					Print("enterShort " + enterShort + " " + Time[CandleLookBackEntry]);
				}
			}
			#endregion
			
			#endregion
			
					#region Stop Offset
			
			//Define what area you will set a stop (If it is based on the chart)
			stopAreaLong		= Low[CandleLookBackStop];
			stopAreaShort		= High[CandleLookBackStop];
			
			//Adds offset to your stop area. Gives user customization.
			percentageCalcStop 		= ((High[CandleLookBackStop] - Low[CandleLookBackStop]) * PercentageOffsetStop);
			priceCalcStop 			= PriceOffsetStop;
			tickCalcStop			= TickOffsetStop * TickSize;
			
			//Picks the highest of the 3 numbers
			candleBarOffsetStop = Math.Max(percentageCalcStop, Math.Max(priceCalcStop, tickCalcStop));
			
			//Add both of them together to define final stop point
			stopLong = stopAreaLong - candleBarOffsetStop;
			stopShort = stopAreaShort + candleBarOffsetStop;
			
			#region StopPrints
			
			if (SystemPrint)
			{
				if (StopPrints)
				{
					Print("percentageCalcStop " + percentageCalcStop + " " + Time[CandleLookBackStop]);
					Print("priceCalcStop " + priceCalcStop + " " + Time[CandleLookBackStop]);
					Print("tickCalcStop " + tickCalcStop + " " + Time[CandleLookBackStop]);
				
					Print("candleBarOffsetStop " + candleBarOffsetStop + " " + Time[CandleLookBackStop]);
					
					Print("stopLong " + stopLong + " " + Time[CandleLookBackStop]);
					Print("stopShort " + stopShort + " " + Time[CandleLookBackStop]);
				}
			}
			
			#endregion
			
			#endregion
			
					#region Breakeven Offset
			
			//Define what area you will set a breakeven (If it is based on the chart)
			breakevenAreaLong		= entryAreaLong;
			breakevenAreaShort		= entryAreaShort;
			
			//Adds offset to your breakeven area. Gives user customization.
			percentageCalcBreakeven 	= ((entryAreaLong - stopAreaLong) * PercentageOffsetBreakeven);
			priceCalcBreakeven 			= PriceOffsetBreakeven;
			tickCalcBreakeven			= TickOffsetBreakeven * TickSize;
			
			//Picks the highest of the 3 numbers
			candleBarOffsetBreakeven = Math.Max(percentageCalcBreakeven, Math.Max(priceCalcBreakeven, tickCalcBreakeven));
			
			//Add both of them together to define final breakeven point
			//breakevenLong = breakevenAreaLong - candleBarOffsetBreakeven;
			//breakevenShort = breakevenAreaShort + candleBarOffsetBreakeven;
			
			#region BreakevenPrints
			
			if (SystemPrint)
			{
				if (BreakevenPrints)
				{
					Print("percentageCalcBreakeven " + percentageCalcBreakeven + " " + Time[1]);
					Print("priceCalcBreakeven " + priceCalcBreakeven + " " + Time[1]);
					Print("tickCalcBreakeven " + tickCalcBreakeven + " " + Time[1]);
				
					Print("candleBarOffsetBreakeven " + candleBarOffsetBreakeven + " " + Time[1]);
					
					Print("breakevenLong " + breakevenLong + " " + Time[1]);
					Print("breakevenShort " + breakevenShort + " " + Time[1]);
				}
			}
			
			#endregion
			
			#endregion
			
					#region Range Values
			
			LongValuesOffset 	= Math.Round(enterLong - stopLong, 4);
			ShortValuesOffset	= Math.Round(stopShort - enterShort, 4);
			
			LongValues 			= Math.Round(entryAreaLong - stopAreaLong, 4);
			ShortValues			= Math.Round(stopAreaShort - entryAreaShort, 4);
			
			#endregion
			
					#region Limit Entry Offset
			
			if (useLimit)
			{
				percentageCalcLimitLong		= LongValuesOffset * PercentageOffsetLimit;
				percentageCalcLimitShort	= ShortValuesOffset * PercentageOffsetLimit;
				priceCalcLimit				= PriceOffsetLimit;
				tickCalcLimit				= TickOffsetLimit * TickSize;
				
				limitOffsetLong				= Math.Max(percentageCalcLimitLong, Math.Max(priceCalcLimit, tickCalcLimit));	
				limitOffsetShort			= Math.Max(percentageCalcLimitShort, Math.Max(priceCalcLimit, tickCalcLimit));
				
				if (AddOffset)
				{
					limitPriceSetLong			= enterLong + limitOffsetLong;
					limitPriceSetShort			= enterShort - limitOffsetShort;
				}
				
				else if (AddOffset == false)
				{
					limitPriceSetLong			= entryAreaLong + limitOffsetLong;
					limitPriceSetShort			= entryAreaShort - limitOffsetShort;
				}
				
				#region Limit Prints
			
			if (SystemPrint)
			{
				if (LimitPrints)
				{
					Print("percentageCalcLimitLong " + percentageCalcLimitLong + " " + Time[0]);
					Print("percentageCalcLimitShort " + percentageCalcLimitShort + " " + Time[0]);
					Print("priceCalcLimit " + priceCalcLimit + " " + Time[0]);
					Print("tickCalcLimit " + tickCalcLimit + " " + Time[0]);
					
					Print("limitOffsetLong " + limitOffsetLong + " " + Time[0]);
					Print("limitPriceSetShort " + limitPriceSetShort + " " + Time[0]);
					
					Print("limitPriceSetLong " + limitPriceSetLong + " " + Time[0]);
					Print("limitPriceSetShort " + limitPriceSetShort + " " + Time[0]);
				}
			}
			#endregion
			}
			
			
			#endregion
			
			
					#region Offset Added Logic
			
			if (AddOffset) ///Update Prints
			{	
				//Long Trades
				if (longButtonHLClicked == true || longButtonMarketClicked == true || longLineButtonClicked == true || customLongClicked == true)
				{
						
					#region Position Size Long
				
				if (autoPositionSize)
				{
					if (rrMode)
					{
						riskOffset = MaxLossPerTrade / ( ( (LongValuesOffset) / TickSize) * myDbl);
					}
					
					if (tickMode)
					{
						riskOffset = MaxLossPerTrade / ( ( (candleBarOffsetStop) / TickSize) * myDbl);
					}
				}
				
				else if (customPositionSize)
				{
					riskOffset = CustomPositionAmount;
				}
				
				positionSizeLong = (Convert.ToInt32(riskOffset));
				
				
					//Single Target
					if (dualTarget == false)
					{
						positionSizeLong = (Convert.ToInt32(riskOffset));
						
						if (positionSizeLong < 1)
						{
							positionSizeLong = 1;
						}
					}
					
					//Dual Target
					if (dualTarget)
					{
						
					firstTargetPositionLong = (Convert.ToInt32(positionSizeLong * splitPercent));
						
						if (firstTargetPositionLong < 1)
						{
							firstTargetPositionLong = 1;
						}
				
					secondTargetPositionLong = positionSizeLong - firstTargetPositionLong; 
						
						if(secondTargetPositionLong < 1)
						{
							secondTargetPositionLong = 1;
						}	
					}
					
						#region Position Size Prints
						
						if (SystemPrint)
						{
							if (PositionSizePrints)
							{
								Print("positionSizeLong "+ positionSizeLong + " " + Time[0]);
								Print("firstTargetPositionLong "+ firstTargetPositionLong + " " + Time[0]);
								Print("secondTargetPositionLong "+ secondTargetPositionLong + " " + Time[0]);
							}
						}
						
						#endregion
							
					#endregion
			
					#region Set Stop/Profit Long
				
				if (Low[0] > stopLong)
				{
					setStopLong = stopLong;
					
					setFirstTargetLong = enterLong + (LongValuesOffset * FirstTargetRR);
					setFinalTargetLong = enterLong + (LongValuesOffset * FinalTargetRR);
				}
				
				else if (Low[0] <= stopLong)
				{
					setStopLong = Low[0] - candleBarOffsetStop;
					
					setFirstTargetLong = enterLong + ((enterLong - setStopLong) * FirstTargetRR);
					setFinalTargetLong = enterLong + ((enterLong - setStopLong) * FinalTargetRR);
				}
				
				#region Profit Target Long Prints
				
				if (SystemPrint)
				{
					if (ProfitTatgetPrints)
					{
						Print("setStopLong "+ setStopLong + " " + Time[0]);
						Print("setFirstTargetLong "+ setFirstTargetLong + " " + Time[0]);
						Print("setFinalTargetLong "+ setFinalTargetLong + " " + Time[0]);		
					}
				}
				
				#endregion
				
				
				#endregion
				
					#region Draw Lines / Text Long Offset
		
						#region RR mode
						
					if (rrMode)
					{
						RemoveDrawObject("EntryLine");
						RemoveDrawObject("StopLine");
					
						Draw.HorizontalLine(this, "EntryLine", enterLong, Brushes.Green);
						Draw.HorizontalLine(this, "StopLine", stopLong, Brushes.Red);
					
						if (DisplayText)
						{
							Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(enterLong, 4) + ("("+CandleLookBackEntry+")")
							+ "\nStop Line: " + Math.Round(stopLong, 4) + ("("+CandleLookBackStop+")")
							
							+ "\n\nMax Loss($): " + MaxLossPerTrade	
							+ "\nRange($): " + LongValuesOffset
							+ "\nRange(Ticks): " + LongValuesOffset / TickSize
							+ "\nPosition Size: " + positionSizeLong	
						
							,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
						}
					}
						
						#endregion 
					
						#region Tick Mode
					
					if (tickMode)
					{
						RemoveDrawObject("EntryLine");
						RemoveDrawObject("StopLine");
					
						Draw.HorizontalLine(this, "EntryLine", enterLong, Brushes.Green);
						Draw.HorizontalLine(this, "StopLine", enterLong - candleBarOffsetStop, Brushes.Red);
					
						if (DisplayText)
						{
							Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(enterLong, 4) + ("("+CandleLookBackEntry+")")
							+ "\nStop Line: " + Math.Round(enterLong - candleBarOffsetStop, 4) + ("("+candleBarOffsetStop / TickSize+")")
							
							+ "\n\nMax Loss($): " + MaxLossPerTrade	
							+ "\nRange($): " + candleBarOffsetStop
							+ "\nRange(Ticks): " + candleBarOffsetStop / TickSize
							+ "\nPosition Size: " + positionSizeLong	
						
							,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
						}
					}
				
					#endregion
					
					#endregion
										
				}
				
				
				//Short Trades
				if (shortButtonHLClicked == true || shortButtonMarketClicked == true || shortLineButtonClicked == true || customShortClicked == true)
				{
				
					#region Position Size Short
				
				if (autoPositionSize)
				{
					if (rrMode)
					{
						riskOffset = MaxLossPerTrade / ( ( (ShortValuesOffset) / TickSize) * myDbl);
					}
					
					if (tickMode)
					{
						riskOffset = MaxLossPerTrade / ( ( (candleBarOffsetStop) / TickSize) * myDbl);
					}
				}
				
				else if (customPositionSize)
				{
					riskOffset = CustomPositionAmount;
				}
				
				positionSizeShort = (Convert.ToInt32(riskOffset));
				
				
					//Single Target
					if (dualTarget == false)
					{
						positionSizeShort = (Convert.ToInt32(riskOffset));
						
						if (positionSizeShort < 1)
						{
							positionSizeShort = 1;
						}
					}
					
					//Dual Target
					if (dualTarget)
					{
						
					firstTargetPositionShort = (Convert.ToInt32(positionSizeShort * splitPercent));
						
						if (firstTargetPositionShort < 1)
						{
							firstTargetPositionShort = 1;
						}
				
					secondTargetPositionShort = positionSizeShort - firstTargetPositionShort; 
						
						if(secondTargetPositionShort < 1)
						{
							secondTargetPositionShort = 1;
						}	
					}
					
						#region Position Size Prints
						
						if (SystemPrint)
						{
							if (PositionSizePrints)
							{
								Print("positionSizeShort "+ positionSizeShort + " " + Time[0]);
								Print("firstTargetPositionShort "+ firstTargetPositionShort + " " + Time[0]);
								Print("secondTargetPositionShort "+ secondTargetPositionShort + " " + Time[0]);
							}
						}
						
						#endregion
						
					
					#endregion
				
					#region Set Stop/Profit Short
				
				if (High[0] < stopShort)
				{
					setStopShort = stopShort;
					
					setFirstTargetShort = enterShort - (ShortValuesOffset * FirstTargetRR);
					setFinalTargetShort = enterShort - (ShortValuesOffset * FinalTargetRR);
				}
				
				else if (High[0] >= stopShort)
				{
					setStopShort = High[0] + candleBarOffsetStop;
					
					setFirstTargetShort = enterShort - ((setStopShort - enterShort) * FirstTargetRR);
					setFinalTargetShort = enterShort - ((setStopShort - enterShort) * FinalTargetRR);
				}
				
				
				#region Profit Target Prints
						
						if (SystemPrint)
						{
							if (ProfitTatgetPrints)
							{
								Print("setStopShort "+ setStopShort + " " + Time[0]);
								Print("setFirstTargetShort "+ setFirstTargetShort + " " + Time[0]);
								Print("setFinalTargetShort "+ setFinalTargetShort + " " + Time[0]);
							}
						}
						
						#endregion
				
				#endregion
					
					#region Draw Lines / Text Short Offset
				
						#region RR Mode
						
				if (rrMode)
				{
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					
					
					Draw.HorizontalLine(this, "EntryLine", enterShort, Brushes.Green);
					Draw.HorizontalLine(this, "StopLine", stopShort, Brushes.Red);
					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(enterShort, 4) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round(stopShort, 4) + ("("+CandleLookBackStop+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + ShortValuesOffset
						+ "\nRange(Ticks): " + ShortValuesOffset / TickSize
						+ "\nPosition Size: " + positionSizeShort	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
					
				}
						
					#endregion
					
						#region Tick Mode
						
				if (tickMode)
				{
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					
					
					Draw.HorizontalLine(this, "EntryLine", enterShort, Brushes.Green);
					Draw.HorizontalLine(this, "StopLine", enterShort + candleBarOffsetStop, Brushes.Red);
					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(enterShort, 4) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round(enterShort + candleBarOffsetStop, 4) + ("("+candleBarOffsetStop / TickSize+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + candleBarOffsetStop
						+ "\nRange(Ticks): " + candleBarOffsetStop / TickSize
						+ "\nPosition Size: " + positionSizeShort	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
					
				}
						
					#endregion
					
				#endregion
				
				}
			}
			
			#endregion	
			
					#region W/O Offset 
			/* Removed for simplicity of use
			
			if (AddOffset == false)
			{	
				//Long Trades
				if (longButtonHLClicked == true || longButtonMarketClicked == true || longLineButtonClicked == true || customLongClicked == true)
				{
				
					#region Position Size Long
				
				if (autoPositionSize)
				{
					riskOffset = MaxLossPerTrade / ( ( (LongValues) / TickSize) * myDbl);
				}
				
				else if (customPositionSize)
				{
					riskOffset = CustomPositionAmount;
				}
				
				positionSize = (Convert.ToInt32(riskOffset));
				
				
					//Single Target
					if (dualTarget == false)
					{
						positionSize = (Convert.ToInt32(riskOffset));
						
						if (positionSize < 1)
						{
							positionSize = 1;
						}
					}
					
					//Dual Target
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
					
						#region Position Size Prints
						
						if (SystemPrint)
						{
							if (PositionSizePrints)
							{
								Print("positionSize "+ positionSize + " " + Time[0]);
								Print("firstTargetPosition "+ firstTargetPosition + " " + Time[0]);
								Print("secondTargetPosition "+ secondTargetPosition + " " + Time[0]);
							}
						}
						
						#endregion
						
					
					
				
					#endregion
					
					#region Set Stop/Profit Long		
					
				if (Low[0] > stopAreaLong)
				{
					setStopLong = stopAreaLong;
				}
				
				else if (Low[0] <= stopAreaLong)
				{
					setStopLong = Low[0];
				}
				
				setFirstTarget = entryAreaLong + (LongValues * FirstTargetRR);
				setFinalTarget = entryAreaLong + (LongValues * FinalTargetRR);
				
				#endregion
				
					#region Draw Lines / Text Long W/O Offset
				
					#region RR mode
				
				if (rrMode)
				{
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					
					
					Draw.HorizontalLine(this, "EntryLine", entryAreaLong, Brushes.Green);
					Draw.HorizontalLine(this, "StopLine", stopAreaLong, Brushes.Red);
					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(entryAreaLong, 2) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round(stopAreaLong, 2) + ("("+CandleLookBackStop+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + LongValues	
						+ "\nRange(Ticks): " + LongValues / TickSize
						+ "\nPosition Size: " + positionSize	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
				}
					
					#endregion
				
					#region Tick mode
				
				if (tickMode)
				{
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					
					
					Draw.HorizontalLine(this, "EntryLine", entryAreaLong, Brushes.Green);
					Draw.HorizontalLine(this, "StopLine", entryAreaLong - candleBarOffsetStop, Brushes.Red);
					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(entryAreaLong, 2) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round(entryAreaLong - candleBarOffsetStop, 2) + ("("+candleBarOffsetStop / TickSize+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + candleBarOffsetStop	
						+ "\nRange(Ticks): " + candleBarOffsetStop / TickSize
						+ "\nPosition Size: " + positionSize	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
				}
					
					#endregion
					
					#endregion
				
				}
				
				//Short Trades
				if (shortButtonHLClicked == true || shortButtonMarketClicked == true || shortLineButtonClicked == true || customShortClicked == true)
				{
				
					#region Position Size Short
				
				if (autoPositionSize)
				{
					riskOffset = MaxLossPerTrade / ( ( (ShortValues) / TickSize) * myDbl);
				}
				
				else if (customPositionSize)
				{
					riskOffset = CustomPositionAmount;
				}
				
				positionSize = (Convert.ToInt32(riskOffset));
				
				
					//Single Target
					if (dualTarget == false)
					{
						positionSize = (Convert.ToInt32(riskOffset));
						
						if (positionSize < 1)
						{
							positionSize = 1;
						}
					}
					
					//Dual Target
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
					
						#region Position Size Prints
						
						if (SystemPrint)
						{
							if (PositionSizePrints)
							{
								Print("positionSize "+ positionSize + " " + Time[0]);
								Print("firstTargetPosition "+ firstTargetPosition + " " + Time[0]);
								Print("secondTargetPosition "+ secondTargetPosition + " " + Time[0]);
							}
						}
						
						#endregion
						
					
					#endregion
					
					#region Set Stop/Profit Short		
					
				if (High[0] < stopAreaShort)
				{
					setStopShort = stopAreaShort;
				}
				
				else if (High[0] >= stopAreaShort)
				{
					setStopShort = High[0];
				}
				
				setFirstTarget = entryAreaShort - (ShortValues * FirstTargetRR);
				setFinalTarget = entryAreaShort - (ShortValues * FinalTargetRR);
				
				#endregion
				
					#region Draw Lines / Text Short W/O Offset
				
					#region RR mode
				
				if (rrMode)
				{
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					
					
					Draw.HorizontalLine(this, "EntryLine", entryAreaShort, Brushes.Green);
					Draw.HorizontalLine(this, "StopLine", stopAreaShort, Brushes.Red);
					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(entryAreaShort, 2) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round(stopAreaShort, 2) + ("("+CandleLookBackStop+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + ShortValues	
						+ "\nRange(Ticks): " + ShortValues / TickSize
						+ "\nPosition Size: " + positionSize	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
				}
				
					#endregion
				
					#region Tick mode
				
				if (tickMode)
				{
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					
					
					Draw.HorizontalLine(this, "EntryLine", entryAreaShort, Brushes.Green);
					Draw.HorizontalLine(this, "StopLine", entryAreaShort + candleBarOffsetStop, Brushes.Red);
					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(entryAreaShort, 2) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round( entryAreaShort + candleBarOffsetStop, 2) + ("("+candleBarOffsetStop / TickSize+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + candleBarOffsetStop	
						+ "\nRange(Ticks): " + candleBarOffsetStop / TickSize
						+ "\nPosition Size: " + positionSize	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
				}
				
					#endregion
				
					#endregion
				
				}
				
			}
			*/
			#endregion //Removed for now
			
				}
			
				if (CandleRange)
				{
					#region Entry Offset
	
			if (longButtonHLClicked == true || longButtonMarketClicked == true || longLineButtonClicked == true || (customLongClicked == true && enterClosePrice == false)
				|| shortButtonHLClicked == true || shortButtonMarketClicked == true || shortLineButtonClicked == true || (customShortClicked == true && enterClosePrice == false)
				)		
			{	
				//HL
				entryAreaLong		= MAX(High, CandleLookBackEntry)[1];
				entryAreaShort		= MIN(Low, CandleLookBackEntry)[1];
			}	
			
			if ((customLongClicked || customShortClicked) && enterClosePrice == true)
			{
				//Previous Close Price	
				entryAreaLong		= MAX(Close, CandleLookBackEntry)[1];
				entryAreaShort		= MAX(Close, CandleLookBackEntry)[1];
			}
			
			//Adds offset to your entry area. Gives user customization.
			percentageCalcEntry 	= ((entryAreaLong - entryAreaShort) * PercentageOffsetEntry);
			priceCalcEntry 			= PriceOffsetEntry;
			tickCalcEntry			= TickOffsetEntry * TickSize;
			
			//Picks the highest of the 3 numbers
			candleBarOffsetEntry = Math.Max(percentageCalcEntry, Math.Max(priceCalcEntry, tickCalcEntry));
			
			//Add both of them together to define final entry point
			enterLong = entryAreaLong + candleBarOffsetEntry;
			enterShort = entryAreaShort - candleBarOffsetEntry;
							
			
			#region Entry Prints
			
			if (SystemPrint)
			{
				if (EntryPrints)
				{
					Print("entryAreaLong " + entryAreaLong + " " + Time[1]);
					Print("entryAreaShort " + entryAreaShort + " " + Time[1]);
					
					Print("percentageCalcEntry Range " + percentageCalcEntry + " " + Time[1]);
					Print("priceCalcEntry Range  " + priceCalcEntry + " " + Time[1]);
					Print("tickCalcEntry Range  " + tickCalcEntry + " " + Time[1]);
					
					Print("candleBarOffsetEntry Range " + candleBarOffsetEntry + " " + Time[1]);
					
					Print("enterLong  " + enterLong + " " + Time[1]);
					Print("enterShort "  + enterShort + " " + Time[1]);
				}
			}
			#endregion
			
			#endregion
			
					#region Stop Offset
			
			//Define what area you will set a stop (If it is based on the chart)
			stopAreaLong		= MIN(Low, CandleLookBackStop)[1];
			stopAreaShort		= MAX(High, CandleLookBackStop)[1];
			
			//Adds offset to your stop area. Gives user customization.
			percentageCalcStop 		= ((stopAreaShort - stopAreaLong) * PercentageOffsetStop);
			priceCalcStop 			= PriceOffsetStop;
			tickCalcStop			= TickOffsetStop * TickSize;
			
			//Picks the highest of the 3 numbers
			candleBarOffsetStop = Math.Max(percentageCalcStop, Math.Max(priceCalcStop, tickCalcStop));
			
			//Add both of them together to define final stop point
			stopLong = stopAreaLong - candleBarOffsetStop;
			stopShort = stopAreaShort + candleBarOffsetStop;
			
			#region StopPrints
			
			if (SystemPrint)
			{
				if (StopPrints)
				{
					Print("percentageCalcStop Range " + percentageCalcStop + " " + Time[1]);
					Print("priceCalcStop Range " + priceCalcStop + " " + Time[1]);
					Print("tickCalcStop Range " + tickCalcStop + " " + Time[1]);
				
					Print("candleBarOffsetStop Range " + candleBarOffsetStop + " " + Time[1]);
					
					Print("stopLong Range " + stopLong + " " + Time[1]);
					Print("stopShort Range " + stopShort + " " + Time[1]);
				}
			}
			
			#endregion
			
			#endregion
			
					#region Breakeven Offset
			
			//Define what area you will set a breakeven (If it is based on the chart - Different from 'Actual Breakeven')
			breakevenAreaLong		= entryAreaLong;
			breakevenAreaShort		= entryAreaShort;
			
			//Adds offset to your breakeven area. Gives user customization.
			percentageCalcBreakeven 	= ((entryAreaLong - stopAreaLong) * PercentageOffsetBreakeven);
			priceCalcBreakeven 			= PriceOffsetBreakeven;
			tickCalcBreakeven			= TickOffsetBreakeven * TickSize;
			
			//Picks the highest of the 3 numbers
			candleBarOffsetBreakeven = Math.Max(percentageCalcBreakeven, Math.Max(priceCalcBreakeven, tickCalcBreakeven));
			
			#region BreakevenPrints
			
			if (SystemPrint)
			{
				if (BreakevenPrints)
				{
					Print("percentageCalcBreakeven Range " + percentageCalcBreakeven + " " + Time[1]);
					Print("priceCalcBreakeven Range " + priceCalcBreakeven + " " + Time[1]);
					Print("tickCalcBreakeven Range " + tickCalcBreakeven + " " + Time[1]);
				
					Print("candleBarOffsetBreakeven Range " + candleBarOffsetBreakeven + " " + Time[1]);
					
					Print("breakevenLong Range " + breakevenLong + " " + Time[1]);
					Print("breakevenShort Range " + breakevenShort + " " + Time[1]);
				}
			}
			
			#endregion
			
			#endregion
			
					#region Range Values
			
			LongValuesOffset 	= Math.Round(enterLong - stopLong, 4);
			ShortValuesOffset	= Math.Round(stopShort - enterShort, 4);
			
			LongValues 			= Math.Round(entryAreaLong - stopAreaLong, 4);
			ShortValues			= Math.Round(stopAreaShort - entryAreaShort, 4);
			/*
			LongValuesOffset 	= Math.Round(enterLong - stopLong, 2);
			ShortValuesOffset	= Math.Round(stopShort - enterShort, 2);
			
			LongValues 			= Math.Round(entryAreaLong - stopAreaLong, 2);
			ShortValues			= Math.Round(stopAreaShort - entryAreaShort, 2);
			
			///////////////////////////////////////////////////////
			LongValuesOffsetTickMode	= Math.Round(enterLong - (enterLong - candleBarOffsetStop), 2);
			ShortValuesOffsetTickMode	= Math.Round((enterShort + candleBarOffsetStop) - enterShort, 2);
			
			LongValuesTickMode			= Math.Round(entryAreaLong - (entryAreaLong - candleBarOffsetStop), 2);
			ShortValuesTickMode			= Math.Round((entryAreaShort + candleBarOffsetStop) - entryAreaShort, 2);
			
			
			Print("LongValuesOffsetTickMode " + LongValuesOffsetTickMode);		
			Print("ShortValuesOffsetTickMode " + ShortValuesOffsetTickMode);
			
			Print("LongValuesTickMode " + LongValuesTickMode);
			Print("ShortValuesTickMode " + ShortValuesTickMode);
		*/	
			#endregion
					
					#region Limit Entry Offset
			
			if (useLimit)
			{
				percentageCalcLimitLong		= LongValuesOffset * PercentageOffsetLimit;
				percentageCalcLimitShort	= ShortValuesOffset * PercentageOffsetLimit;
				priceCalcLimit				= PriceOffsetLimit;
				tickCalcLimit				= TickOffsetLimit * TickSize;
				
				limitOffsetLong				= Math.Max(percentageCalcLimitLong, Math.Max(priceCalcLimit, tickCalcLimit));	
				limitOffsetShort			= Math.Max(percentageCalcLimitShort, Math.Max(priceCalcLimit, tickCalcLimit));
				
				if (AddOffset)
				{
					limitPriceSetLong			= enterLong + limitOffsetLong;
					limitPriceSetShort			= enterShort - limitOffsetShort;
				}
				
				else if (AddOffset == false)
				{
					limitPriceSetLong			= entryAreaLong + limitOffsetLong;
					limitPriceSetShort			= entryAreaShort - limitOffsetShort;
				}
				
				#region Limit Prints
			
			if (SystemPrint)
			{
				if (LimitPrints)
				{
					Print("percentageCalcLimitLong " + percentageCalcLimitLong + " " + Time[0]);
					Print("percentageCalcLimitShort " + percentageCalcLimitShort + " " + Time[0]);
					Print("priceCalcLimit " + priceCalcLimit + " " + Time[0]);
					Print("tickCalcLimit " + tickCalcLimit + " " + Time[0]);
					
					Print("limitOffsetLong " + limitOffsetLong + " " + Time[0]);
					Print("limitPriceSetShort " + limitPriceSetShort + " " + Time[0]);
					
					Print("limitPriceSetLong " + limitPriceSetLong + " " + Time[0]);
					Print("limitPriceSetShort " + limitPriceSetShort + " " + Time[0]);
				}
			}
			#endregion
			}
				#endregion
			
					
					#region Offset Added Logic
			
			if (AddOffset) ///Update Prints
			{	
				//Long Trades
				if (longButtonHLClicked == true || longButtonMarketClicked == true || longLineButtonClicked == true || customLongClicked == true)
				{
						
					#region Position Size Long
				
				if (autoPositionSize)
				{
					if (rrMode)
					{
						riskOffset = MaxLossPerTrade / ( ( (LongValuesOffset) / TickSize) * myDbl);
					}
					
					if (tickMode)
					{
						riskOffset = MaxLossPerTrade / ( ( (candleBarOffsetStop) / TickSize) * myDbl);
					}
				}
				
				else if (customPositionSize)
				{
					riskOffset = CustomPositionAmount;
				}
				
				positionSizeLong = (Convert.ToInt32(riskOffset));
				
				
					//Single Target
					if (dualTarget == false)
					{
						positionSizeLong = (Convert.ToInt32(riskOffset));
						
						if (positionSizeLong < 1)
						{
							positionSizeLong = 1;
						}
					}
					
					//Dual Target
					if (dualTarget)
					{
						
					firstTargetPositionLong = (Convert.ToInt32(positionSizeLong * splitPercent));
						
						if (firstTargetPositionLong < 1)
						{
							firstTargetPositionLong = 1;
						}
				
					secondTargetPositionLong = positionSizeLong - firstTargetPositionLong; 
						
						if(secondTargetPositionLong < 1)
						{
							secondTargetPositionLong = 1;
						}	
					}
					
						#region Position Size Prints
						
						if (SystemPrint)
						{
							if (PositionSizePrints)
							{
								Print("positionSizeLong "+ positionSizeLong + " " + Time[0]);
								Print("firstTargetPositionLong "+ firstTargetPositionLong + " " + Time[0]);
								Print("secondTargetPositionLong "+ secondTargetPositionLong + " " + Time[0]);
							}
						}
						
						#endregion
						
					
					#endregion
				
					#region Set Stop/Profit Long
				
				if (Low[0] > stopLong)
				{
					setStopLong = stopLong;
					
					setFirstTargetLong = enterLong + (LongValuesOffset * FirstTargetRR);
					setFinalTargetLong = enterLong + (LongValuesOffset * FinalTargetRR);
				}
				
				else if (Low[0] <= stopLong)
				{
					setStopLong = Low[0] - candleBarOffsetStop;
					
					setFirstTargetLong = enterLong + ((enterLong - setStopLong) * FirstTargetRR);
					setFinalTargetLong = enterLong + ((enterLong - setStopLong) * FinalTargetRR);
				}
				
	
				#region Profit Target Long Prints
				
				if (SystemPrint)
				{
					if (ProfitTatgetPrints)
					{
						Print("setStopLong "+ setStopLong + " " + Time[0]);
						Print("setFirstTargetLong "+ setFirstTargetLong + " " + Time[0]);
						Print("setFinalTargetLong "+ setFinalTargetLong + " " + Time[0]);		
					}
				}
				
				#endregion
				
				#endregion
				
					#region Draw Lines / Text Long Offset
		
						#region RR mode
						
					if (rrMode)
					{
						RemoveDrawObject("EntryLine");
						RemoveDrawObject("StopLine");
					
						Draw.HorizontalLine(this, "EntryLine", enterLong, Brushes.Green);
						Draw.HorizontalLine(this, "StopLine", stopLong, Brushes.Red);
					
						if (DisplayText)
						{
							Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(enterLong, 4) + ("("+CandleLookBackEntry+")")
							+ "\nStop Line: " + Math.Round(stopLong, 4) + ("("+CandleLookBackStop+")")
							
							+ "\n\nMax Loss($): " + MaxLossPerTrade	
							+ "\nRange($): " + LongValuesOffset
							+ "\nRange(Ticks): " + LongValuesOffset / TickSize
							+ "\nPosition Size: " + positionSizeLong	
						
							,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
						}
					}
						
						#endregion 
					
						#region Tick Mode
					
					if (tickMode)
					{
						RemoveDrawObject("EntryLine");
						RemoveDrawObject("StopLine");
					
						Draw.HorizontalLine(this, "EntryLine", enterLong, Brushes.Green);
						Draw.HorizontalLine(this, "StopLine", enterLong - candleBarOffsetStop, Brushes.Red);
					
						if (DisplayText)
						{
							Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(enterLong, 4) + ("("+CandleLookBackEntry+")")
							+ "\nStop Line: " + Math.Round(enterLong - candleBarOffsetStop, 4) + ("("+candleBarOffsetStop / TickSize+")")
							
							+ "\n\nMax Loss($): " + MaxLossPerTrade	
							+ "\nRange($): " + candleBarOffsetStop
							+ "\nRange(Ticks): " + candleBarOffsetStop / TickSize
							+ "\nPosition Size: " + positionSizeLong	
						
							,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
						}
					}
				
					#endregion
					
					#endregion
										
				}
				
				
				//Short Trades
				if (shortButtonHLClicked == true || shortButtonMarketClicked == true || shortLineButtonClicked == true || customShortClicked == true)
				{
				
					#region Position Size Short
				
				if (autoPositionSize)
				{
					if (rrMode)
					{
						riskOffset = MaxLossPerTrade / ( ( (ShortValuesOffset) / TickSize) * myDbl);
					}
					
					if (tickMode)
					{
						riskOffset = MaxLossPerTrade / ( ( (candleBarOffsetStop) / TickSize) * myDbl);
					}
				}
				
				else if (customPositionSize)
				{
					riskOffset = CustomPositionAmount;
				}
				
				positionSizeShort = (Convert.ToInt32(riskOffset));
				
				
					//Single Target
					if (dualTarget == false)
					{
						positionSizeShort = (Convert.ToInt32(riskOffset));
						
						if (positionSizeShort < 1)
						{
							positionSizeShort = 1;
						}
					}
					
					//Dual Target
					if (dualTarget)
					{
						
					firstTargetPositionShort = (Convert.ToInt32(positionSizeShort * splitPercent));
						
						if (firstTargetPositionShort < 1)
						{
							firstTargetPositionShort = 1;
						}
				
					secondTargetPositionShort = positionSizeShort - firstTargetPositionShort; 
						
						if(secondTargetPositionShort < 1)
						{
							secondTargetPositionShort = 1;
						}	
					}
					
						#region Position Size Prints
						
						if (SystemPrint)
						{
							if (PositionSizePrints)
							{
								Print("positionSizeShort "+ positionSizeShort + " " + Time[0]);
								Print("firstTargetPositionShort "+ firstTargetPositionShort + " " + Time[0]);
								Print("secondTargetPositionShort "+ secondTargetPositionShort + " " + Time[0]);
							}
						}
						
						#endregion
						
					
					#endregion
				
					#region Set Stop/Profit Short
				
				if (High[0] < stopShort)
				{
					setStopShort = stopShort;
				
					setFirstTargetShort = enterShort - (ShortValuesOffset * FirstTargetRR);
					setFinalTargetShort = enterShort - (ShortValuesOffset * FinalTargetRR);
				}
				
				else if (High[0] >= stopShort)
				{
					setStopShort = High[0] + candleBarOffsetStop;
					
					setFirstTargetShort = enterShort - ((setStopShort - enterShort) * FirstTargetRR);
					setFinalTargetShort = enterShort - ((setStopShort - enterShort) * FinalTargetRR);
				}
				
				
				#region Profit Target Prints
						
						if (SystemPrint)
						{
							if (ProfitTatgetPrints)
							{
								Print("setStopShort "+ setStopLong + " " + Time[0]);
								Print("setFirstTargetShort "+ setFirstTargetShort + " " + Time[0]);
								Print("setFinalTargetShort "+ setFinalTargetShort + " " + Time[0]);
							}
						}
						
						#endregion
				
				#endregion
					
					#region Draw Lines / Text Short Offset
				
						#region RR Mode
						
				if (rrMode)
				{
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					
					
					Draw.HorizontalLine(this, "EntryLine", enterShort, Brushes.Green);
					Draw.HorizontalLine(this, "StopLine", stopShort, Brushes.Red);
					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(enterShort, 4) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round(stopShort, 4) + ("("+CandleLookBackStop+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + ShortValuesOffset
						+ "\nRange(Ticks): " + ShortValuesOffset / TickSize
						+ "\nPosition Size: " + positionSizeShort	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
					
				}
						
					#endregion
					
						#region Tick Mode
						
				if (tickMode)
				{
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					
					
					Draw.HorizontalLine(this, "EntryLine", enterShort, Brushes.Green);
					Draw.HorizontalLine(this, "StopLine", enterShort + candleBarOffsetStop, Brushes.Red);
					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(enterShort, 4) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round(enterShort + candleBarOffsetStop, 4) + ("("+candleBarOffsetStop / TickSize+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + candleBarOffsetStop
						+ "\nRange(Ticks): " + candleBarOffsetStop / TickSize
						+ "\nPosition Size: " + positionSizeShort	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
					
				}
						
					#endregion
					
				#endregion
				
				}
			}
			
			#endregion	
			
					#region W/O Offset 
			/* Removed for simplicity of use
			if (AddOffset == false)
			{	
				//Long Trades
				if (longButtonHLClicked == true || longButtonMarketClicked == true || longLineButtonClicked == true || customLongClicked == true)
				{
				
					#region Position Size Long
				
				if (autoPositionSize)
				{
					riskOffset = MaxLossPerTrade / ( ( (LongValues) / TickSize) * myDbl);
				}
				
				else if (customPositionSize)
				{
					riskOffset = CustomPositionAmount;
				}
				
				positionSize = (Convert.ToInt32(riskOffset));
				
				
					//Single Target
					if (dualTarget == false)
					{
						positionSize = (Convert.ToInt32(riskOffset));
						
						if (positionSize < 1)
						{
							positionSize = 1;
						}
					}
					
					//Dual Target
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
					
						#region Position Size Prints
						
						if (SystemPrint)
						{
							if (PositionSizePrints)
							{
								Print("positionSize "+ positionSize + " " + Time[0]);
								Print("firstTargetPosition "+ firstTargetPosition + " " + Time[0]);
								Print("secondTargetPosition "+ secondTargetPosition + " " + Time[0]);
							}
						}
						
						#endregion
						
					
					
				
					#endregion
					
					#region Set Stop/Profit Long		
					
				if (Low[0] > stopAreaLong)
				{
					setStopLong = stopAreaLong;
				}
				
				else if (Low[0] <= stopAreaLong)
				{
					setStopLong = Low[0];
				}
				
				setFirstTarget = entryAreaLong + (LongValues * FirstTargetRR);
				setFinalTarget = entryAreaLong + (LongValues * FinalTargetRR);
				
				#endregion
				
					#region Draw Lines / Text Long W/O Offset
				
					#region RR mode
				
				if (rrMode)
				{
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					
					
					Draw.HorizontalLine(this, "EntryLine", entryAreaLong, Brushes.Green);
					Draw.HorizontalLine(this, "StopLine", stopAreaLong, Brushes.Red);
					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(entryAreaLong, 2) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round(stopAreaLong, 2) + ("("+CandleLookBackStop+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + LongValues	
						+ "\nRange(Ticks): " + LongValues / TickSize
						+ "\nPosition Size: " + positionSize	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
				}
					
					#endregion
				
					#region Tick mode
				
				if (tickMode)
				{
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					
					
					Draw.HorizontalLine(this, "EntryLine", entryAreaLong, Brushes.Green);
					Draw.HorizontalLine(this, "StopLine", entryAreaLong - candleBarOffsetStop, Brushes.Red);
					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(entryAreaLong, 2) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round(entryAreaLong - candleBarOffsetStop, 2) + ("("+candleBarOffsetStop / TickSize+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + candleBarOffsetStop	
						+ "\nRange(Ticks): " + candleBarOffsetStop / TickSize
						+ "\nPosition Size: " + positionSize	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
				}
					
					#endregion
					
					#endregion
				
				}
				
				//Short Trades
				if (shortButtonHLClicked == true || shortButtonMarketClicked == true || shortLineButtonClicked == true || customShortClicked == true)
				{
				
					#region Position Size Short
				
				if (autoPositionSize)
				{
					riskOffset = MaxLossPerTrade / ( ( (ShortValues) / TickSize) * myDbl);
				}
				
				else if (customPositionSize)
				{
					riskOffset = CustomPositionAmount;
				}
				
				positionSize = (Convert.ToInt32(riskOffset));
				
				
					//Single Target
					if (dualTarget == false)
					{
						positionSize = (Convert.ToInt32(riskOffset));
						
						if (positionSize < 1)
						{
							positionSize = 1;
						}
					}
					
					//Dual Target
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
					
						#region Position Size Prints
						
						if (SystemPrint)
						{
							if (PositionSizePrints)
							{
								Print("positionSize "+ positionSize + " " + Time[0]);
								Print("firstTargetPosition "+ firstTargetPosition + " " + Time[0]);
								Print("secondTargetPosition "+ secondTargetPosition + " " + Time[0]);
							}
						}
						
						#endregion
						
					
					#endregion
					
					#region Set Stop/Profit Short		
					
				if (High[0] < stopAreaShort)
				{
					setStopShort = stopAreaShort;
				}
				
				else if (High[0] >= stopAreaShort)
				{
					setStopShort = High[0];
				}
				
				setFirstTarget = entryAreaShort - (ShortValues * FirstTargetRR);
				setFinalTarget = entryAreaShort - (ShortValues * FinalTargetRR);
				
				#endregion
				
					#region Draw Lines / Text Short W/O Offset
				
					#region RR mode
				
				if (rrMode)
				{
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					
					
					Draw.HorizontalLine(this, "EntryLine", entryAreaShort, Brushes.Green);
					Draw.HorizontalLine(this, "StopLine", stopAreaShort, Brushes.Red);
					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(entryAreaShort, 2) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round(stopAreaShort, 2) + ("("+CandleLookBackStop+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + ShortValues	
						+ "\nRange(Ticks): " + ShortValues / TickSize
						+ "\nPosition Size: " + positionSize	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
				}
				
					#endregion
				
					#region Tick mode
				
				if (tickMode)
				{
					RemoveDrawObject("EntryLine");
					RemoveDrawObject("StopLine");
					
					
					Draw.HorizontalLine(this, "EntryLine", entryAreaShort, Brushes.Green);
					Draw.HorizontalLine(this, "StopLine", entryAreaShort + candleBarOffsetStop, Brushes.Red);
					
					if (DisplayText)
					{
						Draw.TextFixed(this, "TextBox", "Entry Line: " + Math.Round(entryAreaShort, 2) + ("("+CandleLookBackEntry+")")
						+ "\nStop Line: " + Math.Round( entryAreaShort + candleBarOffsetStop, 2) + ("("+candleBarOffsetStop / TickSize+")")
							
						+ "\n\nMax Loss($): " + MaxLossPerTrade	
						+ "\nRange($): " + candleBarOffsetStop	
						+ "\nRange(Ticks): " + candleBarOffsetStop / TickSize
						+ "\nPosition Size: " + positionSize	
						
						,TextPosition.BottomLeft, Brushes.White, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Gold, Brushes.Black, 100);
					}
				}
				
					#endregion
				
					#endregion
				
				}
				
			}
			*/
			#endregion //Removed for now
			
				}
			
				//ChartControl.Dispatcher.InvokeAsync((Action)(() =>
					//{
						//NinjaTrader.Gui.Tools.QuantityUpDown quantitySelector = (Window.GetWindow(ChartControl.Parent).FindFirst("ChartTraderControlQuantitySelector") as NinjaTrader.Gui.Tools.QuantityUpDown);

						//quantitySelector.Value = positionSize;
					//}));
				
				#region Arm Market Order (Unlocked Lines) //Ensures Stop and Position size are calculated before triggering
				
				if ((customLongClicked || customShortClicked) && armMarket == true && enterClosePrice == false)
					{
						armMarketCount ++;
						Print(armMarketCount);
					}
					
				if ((customLongClicked || customShortClicked)  && enterClosePrice == false && armMarketCount > 1) //&& armMarket == true)	
					{
						activateMarket = true;
						Print("activateMarket Calcs " + activateMarket);
						Print(armMarketCount);
					}
					
				if ((customLongClicked || customShortClicked) && armMarket == true && enterClosePrice == false && armMarketCount < 1) 
				{
					armMarketCount = 1;
				}

				#endregion
						
				countOnce = false;
			}
			
			#region Arm Market Order (Locked Lines) //Stop and Position Size are already calculated
			
			if (IsFirstTickOfBar)
			{	
				if (unlockButtonClicked && (customLongClicked || customShortClicked) && enterClosePrice == false)//armMarket == true && )
				{
					activateMarket = true;
					Print("activateMarket Calcs Locked " + activateMarket);
				}
			}
			
			#endregion
			
			if (unlockButtonClicked == false)
			{
				if (IsFirstTickOfBar)
				{
					countOnce = true;
				}
			}
			
			if (IsFirstTickOfBar || (trailButtonClicked && IsFirstTickOfBar))
			{
				#region Trail Stop new bar Update
				
				//If the strategy is in a position. This allows the trail stop to keep making calculations and continue moving every new candle
				if (Position.MarketPosition != MarketPosition.Flat)
				{
					myFreeCandleTrail = true;
					
					if (SystemPrint)
					{
						if (TrailPrints)
						{
							Print("myFree Trail First Tick " + myFreeCandleTrail + " " + Time[1]);
						}
					}
				}

				#endregion
			}
			
			if ((IsFirstTickOfBar) && (isFib == true))
			{
				fibCount ++;
			}
			
			
			/// Entries
		
			#region H/L Long
			
			if (
				(longButtonHLClicked == true)
					&& (customLongClicked == false)
						&& (Position.MarketPosition == MarketPosition.Flat)
							&& (Close[0] >= enterLong)
								&& (myFreeLimit)
				)
			{
				#region No Limit
				//Enters With a Market Order After Entry Price has been crossed (Essentially a Stop Market Order)
				if (useLimit == false)
				{
					if (dualTarget == false)
					{
						EnterLong(positionSizeLong, "MyEntryLong");
					}
					
					else if (dualTarget)
					{
						EnterLong(firstTargetPositionLong, "MyEntryLong");	
						
						EnterLong(secondTargetPositionLong, "MyEntryLong2");
					}
				}
				
				#endregion
				
				#region Use Limits
				//Enters With Limit Orders After Entry Price has been crossed (Essentially a Stop Limit Order)
				if (useLimit)
				{
					if (dualTarget == false)
					{
						EnterLongLimit(positionSizeLong, limitPriceSetLong, "MyEntryLong");
					}
					
					else if (dualTarget)
					{
						EnterLongLimit(firstTargetPositionLong, limitPriceSetLong, "MyEntryLong");
						
						EnterLongLimit(secondTargetPositionLong, limitPriceSetLong, "MyEntryLong2");
					}
				}
				
				#endregion
				
				isFib 					= true;
				myFreeTrade			 	= true;
				longButtonHLClicked 	= false;
			}
			
			#endregion
			
			#region H/L Short
			
			if (
				(shortButtonHLClicked == true)
					&& (customLongClicked == false)
						&& (Position.MarketPosition == MarketPosition.Flat)
							&& (Close[0] <= enterShort)
								&& (myFreeLimit)
				)
			{
				#region No Limit
				//Enters With a Market Order After Entry Price has been crossed (Essentially a Stop Market Order)
				if (useLimit == false)
				{
					if (dualTarget == false)
					{
						EnterShort(positionSizeShort, "MyEntryShort");
					}
					
					else if (dualTarget)
					{
						EnterShort(firstTargetPositionShort, "MyEntryShort");	
						
						EnterShort(secondTargetPositionShort, "MyEntryShort2");
					}
				}
				
				#endregion
				
				#region Use Limits
				//Enters With Limit Orders After Entry Price has been crossed (Essentially a Stop Limit Order)
				if (useLimit)
				{
					if (dualTarget == false)
					{
						EnterShortLimit(positionSizeShort, limitPriceSetShort, "MyEntryShort");
					}
					
					else if (dualTarget)
					{
						EnterShortLimit(firstTargetPositionShort, limitPriceSetShort, "MyEntryShort");
						
						EnterShortLimit(secondTargetPositionShort, limitPriceSetShort, "MyEntryShort2");
					}
				}
				
				#endregion
				
				isFib 					= true;
				myFreeTrade 			= true;
				shortButtonHLClicked	= false;
			}
			
			#endregion
			
			
			#region Market Long
			
			if (
				(longButtonMarketClicked == true)
					&& (Position.MarketPosition == MarketPosition.Flat)
				)
			{
				#region Market Long Entry
				
					if (dualTarget == false)
					{
						EnterLong(positionSizeLong, "MyEntryLong");
					}
					
					else if (dualTarget)
					{
						EnterLong(firstTargetPositionLong, "MyEntryLong");	
						
						EnterLong(secondTargetPositionLong, "MyEntryLong2");
					}
					
				#endregion
					
				isFib 					= true;	
				myFreeTrade 			= true;
				longButtonMarketClicked = false;
					
			}
			
			#endregion
			
			#region Market Short
			
			if (
				(shortButtonMarketClicked == true)
					&& (Position.MarketPosition == MarketPosition.Flat)
				)
			{
				#region Market Short Entry
				
					if (dualTarget == false)
					{
						EnterShort(positionSizeShort, "MyEntryShort");
					}
					
					else if (dualTarget)
					{
						EnterShort(firstTargetPositionShort, "MyEntryShort");	
						
						EnterShort(secondTargetPositionShort, "MyEntryShort2");
					}
					
				#endregion
				
				isFib 						= true;	
				myFreeTrade 				= true;
				shortButtonMarketClicked	= false;		
			}
			
			#endregion
			
			
			#region Close Long
			
			if (
				(customLongClicked == true)
					&& (armMarket == false)
						&& (Position.MarketPosition == MarketPosition.Flat)
							&& (Close[0] >= enterLong)
								&& (myFreeLimit)
				)
			{
				#region No Limit
				//Enters With a Market Order After Entry Price has been crossed (Essentially a Stop Market Order)
				if (useLimit == false)
				{
					if (dualTarget == false)
					{
						EnterLong(positionSizeLong, "MyEntryLong");
					}
					
					else if (dualTarget)
					{
						EnterLong(firstTargetPositionLong, "MyEntryLong");	
						
						EnterLong(secondTargetPositionLong, "MyEntryLong2");
					}
				}
				
				#endregion
				
				#region Use Limits
				//Enters With Limit Orders After Entry Price has been crossed (Essentially a Stop Limit Order)
				if (useLimit)
				{
					if (dualTarget == false)
					{
						EnterLongLimit(positionSizeLong, limitPriceSetLong, "MyEntryLong");
					}
					
					else if (dualTarget)
					{
						EnterLongLimit(firstTargetPositionLong, limitPriceSetLong, "MyEntryLong");
						
						EnterLongLimit(secondTargetPositionLong, limitPriceSetLong, "MyEntryLong2");
					}
				}
				
				#endregion
				
				isFib 					= true;
				myFreeTrade 			= true;
				customLongClicked 		= false;
			}
			
			#endregion
			
			#region Close Short
			
			if (
				(customShortClicked == true)
					&& (armMarket == false)
						&& (Position.MarketPosition == MarketPosition.Flat)
							&& (Close[0] <= enterShort)
								&& (myFreeLimit)
				)
			{
				#region No Limit
				//Enters With a Market Order After Entry Price has been crossed (Essentially a Stop Market Order)
				if (useLimit == false)
				{
					if (dualTarget == false)
					{
						EnterShort(positionSizeShort, "MyEntryShort");
					}
					
					else if (dualTarget)
					{
						EnterShort(firstTargetPositionShort, "MyEntryShort");	
						
						EnterShort(secondTargetPositionShort, "MyEntryShort2");
					}
				}
				
				#endregion
				
				#region Use Limits
				//Enters With Limit Orders After Entry Price has been crossed (Essentially a Stop Limit Order)
				if (useLimit)
				{
					if (dualTarget == false)
					{
						EnterShortLimit(positionSizeShort, limitPriceSetShort, "MyEntryShort");
					}
					
					else if (dualTarget)
					{
						EnterShortLimit(firstTargetPositionShort, limitPriceSetShort, "MyEntryShort");
						
						EnterShortLimit(secondTargetPositionShort, limitPriceSetShort, "MyEntryShort2");
					}
				}
				
				#endregion
				
				isFib 					= true;
				myFreeTrade 			= true;
				customShortClicked 		= false;
			}
			
			#endregion
			
			
			if (activateMarket == true)
			{
				
				#region Arm Market Long
				
				if (
					(customLongClicked == true)
						&& (armMarket)
					)
				{
					#region Arm Market Long Entry
					
						if (dualTarget == false)
						{
							EnterLong(positionSizeLong, "MyEntryLong");
						}
						
						else if (dualTarget)
						{
							EnterLong(firstTargetPositionLong, "MyEntryLong");	
							
							EnterLong(secondTargetPositionLong, "MyEntryLong2");
						}
						
					#endregion
					
					isFib 					= true;	
					myFreeTrade 			= true;
					customLongClicked 		= false;
							
				}
				
				#endregion
				
				#region Arm Market Short
				
				if (
					(customShortClicked == true)
						&& (armMarket)
					)
				{
					#region Arm Market Short Entry
					
						if (dualTarget == false)
						{
							EnterShort(positionSizeShort, "MyEntryShort");
						}
						
						else if (dualTarget)
						{
							EnterShort(firstTargetPositionShort, "MyEntryShort");	
							
							EnterShort(secondTargetPositionShort, "MyEntryShort2");
						}
					
					#endregion
						
					isFib 					= true;	
					myFreeTrade 			= true;
					customShortClicked 		= false;
		
				}
				
				#endregion
				
			}
			
			
			///Set Stop/Profit/ Management Targets
			if (rrMode)
			{
				//Long
				#region Filled Long Position	
			
				#region Stop/Profits			
			
				if (Position.MarketPosition == MarketPosition.Long && myFreeTrade == true && Position.Quantity == positionSizeLong)
				{
					if (dualTarget == false)
					{
						if (stopLoss)
						{
							ExitLongStopMarket(0, true, Position.Quantity, setStopLong, "MyStopLong", "MyEntryLong");
						}
						
						if (profitTarget)
						{
							ExitLongLimit(0, true, Position.Quantity, setFinalTargetLong, "MyTargetLong", "MyEntryLong");
						}
					}
				
				if (dualTarget)
				{
					if(stopLoss)
					{
						ExitLongStopMarket(0, true, Position.Quantity, setStopLong, "MyStopLong", "MyEntryLong");
						ExitLongStopMarket(0, true, Position.Quantity, setStopLong, "MyStopLong2", "MyEntryLong2");
					}
					
					if(profitTarget)
					{
						ExitLongLimit(0, true, Position.Quantity, setFirstTargetLong, "MyTargetLong", "MyEntryLong");
						ExitLongLimit(0, true, Position.Quantity, setFinalTargetLong, "MyTargetLong2", "MyEntryLong2");
					}	
				}
				
				#endregion
				
				#region Management Targets
				
				//Breakeven Area
				breakevenAreaTrigger = enterLong + (enterLong - stopLong) * breakevenAreaTarget;
				breakevenAreaStopSet = breakevenAreaLong - candleBarOffsetBreakeven;
			
				if (breakevenAreaSetAuto)
				{
					myFreeBEArea = true;
				}
				
				//Breakeven Actual
				breakevenActualTrigger = enterLong + (enterLong - stopLong) * breakevenActualTarget;
				breakevenActualStopSet = Position.AveragePrice - candleBarOffsetBreakeven;
				
				if (breakevenActualSetAuto)
				{
					myFreeBEActual = true;
				}
				
				//Candle Trail Stop
				candleTrailTrigger = enterLong + (enterLong - stopLong) * candleTrailTarget;
				//candleTrailStopSet = Low[1]; We set the stop later for new candle.
				
				if (candleTrailSetAuto)
				{
					myFreeCandleTrail = true;
					activeTrail = true;
				}
				
				#endregion
				
				#region Management Prints
				
				if (SystemPrint)
				{
					#region Breakeven
					
					if (BreakevenPrints)
					{
						Print("breakeven(Area)Trigger " + breakevenAreaTrigger + " " +  Time[1]);
						Print("breakeven(Area)StopSet " + breakevenAreaStopSet + " " + Time[1]);
						Print("myFreeBE(Area) " + myFreeBEArea + " " + Time[1]);
						
						Print("breakeven(Actual)Trigger " + breakevenActualTrigger + " " +  Time[1]);
						Print("breakeven(Actual)StopSet " + breakevenActualStopSet + " " + Time[1]);
						Print("myFreeBE(Actual) " + myFreeBEActual + " " + Time[1]);
					}
					
					#endregion
				}
				
				#endregion
				
				myFreeTrade = false;
				
				myFillCheck = true;
			}
				
			#endregion
				
				#region Partial Fill Long Position
			
				#region Stop/Profits
				
			if (Position.MarketPosition == MarketPosition.Long && myFreeTrade == true && Position.Quantity < positionSizeLong && IsFirstTickOfBar)
			{
				if (dualTarget == false)
					{
						if (stopLoss)
						{
							ExitLongStopMarket(0, true, Position.Quantity, setStopLong, "MyStopLong", "MyEntryLong"); 
						}
						
						if (profitTarget)
						{
							ExitLongLimit(0, true, Position.Quantity, setFinalTargetLong, "MyTargetLong", "MyEntryLong");
						}
					}
				
				if (dualTarget)
				{
					if(stopLoss)
					{
						ExitLongStopMarket(0, true, Position.Quantity, setStopLong, "MyStopLong", "MyEntryLong");
						ExitLongStopMarket(0, true, Position.Quantity, setStopLong, "MyStopLong2", "MyEntryLong2");
					}
					
					if(profitTarget)
					{
						ExitLongLimit(0, true, Position.Quantity, setFirstTargetLong, "MyTargetLong", "MyEntryLong");
						ExitLongLimit(0, true, Position.Quantity, setFinalTargetLong, "MyTargetLong2", "MyEntryLong2");
					}	
				}
				
				#endregion
				
				#region Management Targets
				
				//Breakeven Area
				breakevenAreaTrigger = enterLong + (enterLong - stopLong) * breakevenAreaTarget;
				breakevenAreaStopSet = breakevenAreaLong - candleBarOffsetBreakeven;
			
				if (breakevenAreaSetAuto)
				{
					myFreeBEArea = true;
				}
				
				//Breakeven Actual
				breakevenActualTrigger = enterLong + (enterLong - stopLong) * breakevenActualTarget;
				breakevenActualStopSet = Position.AveragePrice - candleBarOffsetBreakeven;
				
				if (breakevenActualSetAuto)
				{
					myFreeBEActual = true;
				}
				
				//Candle Trail Stop
				candleTrailTrigger = enterLong + (enterLong - stopLong) * candleTrailTarget;
				//candleTrailStopSet = Low[1]; We set the stop later for new candle.
				
				if (candleTrailSetAuto)
				{
					myFreeCandleTrail = true;
					activeTrail = true;
				}
				
				#endregion
				
				#region Management Prints
				
				if (SystemPrint)
				{
					#region Breakeven
					
					if (BreakevenPrints)
					{
						Print("breakevenAreaTrigger " + breakevenAreaTrigger + " " +  Time[1]);
						Print("breakevenAreaStopSet " + breakevenAreaStopSet + " " + Time[1]);
						Print("myFreeBEArea " + myFreeBEArea + " " + Time[1]);
					}
					
					#endregion
				}
				
				#endregion
				
				myFreeTrade = false;
			}
			
			#endregion
			

				//Short 
				#region Filled Short Position	
			
				#region Stop/Profits
			
				if (Position.MarketPosition == MarketPosition.Short && myFreeTrade == true && Position.Quantity == positionSizeShort)
				{
					if (dualTarget == false)
					{
						if (stopLoss)
						{
							ExitShortStopMarket(0, true, Position.Quantity, setStopShort, "MyStopShort", "MyEntryShort");
						}
						
						if (profitTarget)
						{
							ExitShortLimit(0, true, Position.Quantity, setFinalTargetShort, "MyTargetShort", "MyEntryShort");
						}
					}
				
				if (dualTarget)
				{
					if(stopLoss)
					{
						ExitShortStopMarket(0, true, Position.Quantity, setStopShort, "MyStopShort", "MyEntryShort");
						ExitShortStopMarket(0, true, Position.Quantity, setStopShort, "MyStopShort2", "MyEntryShort2");
					}
					
					if(profitTarget)
					{
						ExitShortLimit(0, true, Position.Quantity, setFirstTargetShort, "MyTargetShort", "MyEntryShort");
						ExitShortLimit(0, true, Position.Quantity, setFinalTargetShort, "MyTargetShort2", "MyEntryShort2");
					}	
				}
				
				#endregion
				
				#region Management Targets
				
				//Breakeven Area
				breakevenAreaTrigger = enterShort - (stopShort - enterShort) * breakevenAreaTarget;
				breakevenAreaStopSet = breakevenAreaShort + candleBarOffsetBreakeven;
			
				if (breakevenAreaSetAuto)
				{
					myFreeBEArea = true;
				}
				
				//Breakeven Actual
				breakevenActualTrigger = enterShort - (stopShort - enterShort) * breakevenActualTarget;
				breakevenActualStopSet = Position.AveragePrice + candleBarOffsetBreakeven;
				
				if (breakevenActualSetAuto)
				{
					myFreeBEActual = true;
				}
				
				//Candle Trail Stop
				candleTrailTrigger = enterShort - (stopShort - enterShort) * candleTrailTarget;
				
				
				if (candleTrailSetAuto)
				{
					myFreeCandleTrail = true;
					activeTrail = true;
				}
				
				#endregion
				
				#region Management Prints
				
				if (SystemPrint)
				{
					#region Breakeven
					
					if (BreakevenPrints)
					{
						Print("breakeven(Area)Trigger " + breakevenAreaTrigger + " " +  Time[1]);
						Print("breakeven(Area)StopSet " + breakevenAreaStopSet + " " + Time[1]);
						Print("myFreeBE(Area) " + myFreeBEArea + " " + Time[1]);
						
						Print("breakeven(Actual)Trigger " + breakevenActualTrigger + " " +  Time[1]);
						Print("breakeven(Actual)StopSet " + breakevenActualStopSet + " " + Time[1]);
						Print("myFreeBE(Actual) " + myFreeBEActual + " " + Time[1]);
					}
					
					#endregion
				}
				
				#endregion
				
				myFreeTrade = false;
				
				myFillCheck = true;
			}
				
			#endregion
				
				#region Partial Fill Short Position
				
				#region Stop/Profits
				
			if (Position.MarketPosition == MarketPosition.Short && myFreeTrade == true && Position.Quantity < positionSizeShort && IsFirstTickOfBar)
			{
				if (dualTarget == false)
					{
						if (stopLoss)
						{
							ExitShortStopMarket(0, true, Position.Quantity, setStopShort, "MyStopShort", "MyEntryShort"); 
						}
						
						if (profitTarget)
						{
							ExitShortLimit(0, true, Position.Quantity, setFinalTargetShort, "MyTargetShort", "MyEntryShort");
						}
					}
				
				if (dualTarget)
				{
					//positionSizeShort = myPosition;
					
					if(stopLoss)
					{		
						ExitShortStopMarket(0, true, Position.Quantity, setStopShort, "MyStopShort", "MyEntryShort");
						ExitShortStopMarket(0, true, Position.Quantity, setStopShort, "MyStopShort2", "MyEntryShort2");
						
						Print("after Fill: " + Position.Quantity);
					}
					
					if(profitTarget)
					{
						ExitShortLimit(0, true, Position.Quantity, setFirstTargetShort, "MyTargetShort", "MyEntryShort");
						ExitShortLimit(0, true, Position.Quantity, setFinalTargetShort, "MyTargetShort2", "MyEntryShort2");
					}	
				}
				
				#endregion
				
				#region Management Targets
				
				//Breakeven Area
				breakevenAreaTrigger = enterShort - (stopShort - enterShort) * breakevenAreaTarget;
				breakevenAreaStopSet = breakevenAreaShort + candleBarOffsetBreakeven;
			
				if (breakevenAreaSetAuto)
				{
					myFreeBEArea = true;
				}
				
				//Breakeven Actual
				breakevenActualTrigger = enterShort - (stopShort - enterShort) * breakevenActualTarget;
				breakevenActualStopSet = Position.AveragePrice + candleBarOffsetBreakeven;
				
				if (breakevenActualSetAuto)
				{
					myFreeBEActual = true;
				}
				
				//Candle Trail Stop
				candleTrailTrigger = enterShort - (stopShort - enterShort) * candleTrailTarget;
				//candleTrailStopSet = Low[1]; We set the stop later for new candle.
				
				if (candleTrailSetAuto)
				{
					myFreeCandleTrail = true;
					activeTrail = true;
				}
				
				#endregion
				
				#region Management Prints
				
				if (SystemPrint)
				{
					#region Breakeven
					
					if (BreakevenPrints)
					{
						Print("breakeven(Area)Trigger " + breakevenAreaTrigger + " " +  Time[1]);
						Print("breakeven(Area)StopSet " + breakevenAreaStopSet + " " + Time[1]);
						Print("myFreeBE(Area) " + myFreeBEArea + " " + Time[1]);
						
						Print("breakeven(Actual)Trigger " + breakevenActualTrigger + " " +  Time[1]);
						Print("breakeven(Actual)StopSet " + breakevenActualStopSet + " " + Time[1]);
						Print("myFreeBE(Actual) " + myFreeBEActual + " " + Time[1]);
					}
					
					#endregion
				}
				
				#endregion
				
				myFreeTrade = false;
			}
			
			#endregion
			
			}
			
			if (tickMode)
			{
				//Long
				#region Filled Long Position	
			
				#region Stop/Profits			
			
				if (Position.MarketPosition == MarketPosition.Long && myFreeTrade == true && Position.Quantity == positionSizeLong)
				{
					if (dualTarget == false)
					{
						if (stopLoss)
						{
							ExitLongStopMarket(0, true, Position.Quantity, Position.AveragePrice - candleBarOffsetStop, "MyStopLong", "MyEntryLong");
						}
						
						if (profitTarget)
						{
							ExitLongLimit(0, true, Position.Quantity, Position.AveragePrice + (FinalTargetRR * TickSize), "MyTargetLong", "MyEntryLong");
						}
					}
				
				if (dualTarget)
				{
					if(stopLoss)
					{
						ExitLongStopMarket(0, true, Position.Quantity, Position.AveragePrice - candleBarOffsetStop, "MyStopLong", "MyEntryLong");
						ExitLongStopMarket(0, true, Position.Quantity, Position.AveragePrice - candleBarOffsetStop, "MyStopLong2", "MyEntryLong2");
					}
					
					if(profitTarget)
					{
						ExitLongLimit(0, true, Position.Quantity, Position.AveragePrice + (FirstTargetRR * TickSize),  "MyTargetLong", "MyEntryLong");
						ExitLongLimit(0, true, Position.Quantity, Position.AveragePrice + (FinalTargetRR * TickSize), "MyTargetLong2", "MyEntryLong2");
					}	
				}
				
				#endregion
				
				#region Management Targets
				
				//Breakeven Area
				breakevenAreaTrigger = Position.AveragePrice + (breakevenAreaTarget * TickSize);
				breakevenAreaStopSet = breakevenAreaLong - candleBarOffsetBreakeven;
			
				if (breakevenAreaSetAuto)
				{
					myFreeBEArea = true;
				}
				
				//Breakeven Actual
				breakevenActualTrigger = Position.AveragePrice + (breakevenActualTarget * TickSize);
				breakevenActualStopSet = Position.AveragePrice - candleBarOffsetBreakeven;
				
				if (breakevenActualSetAuto)
				{
					myFreeBEActual = true;
				}
				
				//Candle Trail Stop
				candleTrailTrigger = Position.AveragePrice + (candleTrailTarget * TickSize);
				
				if (candleTrailSetAuto)
				{
					myFreeCandleTrail = true;
					activeTrail = true;
				}
				
				#endregion
				
				#region Management Prints
				
				if (SystemPrint)
				{
					#region Breakeven
					
					if (BreakevenPrints)
					{
						Print("breakeven(Area)Trigger " + breakevenAreaTrigger + " " +  Time[1]);
						Print("breakeven(Area)StopSet " + breakevenAreaStopSet + " " + Time[1]);
						Print("myFreeBE(Area) " + myFreeBEArea + " " + Time[1]);
						
						Print("breakeven(Actual)Trigger " + breakevenActualTrigger + " " +  Time[1]);
						Print("breakeven(Actual)StopSet " + breakevenActualStopSet + " " + Time[1]);
						Print("myFreeBE(Actual) " + myFreeBEActual + " " + Time[1]);
					}
					
					#endregion
				}
				
				#endregion
				
				myFreeTrade = false;
				
				myFillCheck = true;
			}
				
			#endregion
				
				#region Partial Fill Long Position
			
				#region Stop/Profit
				
			if (Position.MarketPosition == MarketPosition.Long && myFreeTrade == true && Position.Quantity < positionSizeLong && IsFirstTickOfBar)
			{
				if (dualTarget == false)
					{
						if (stopLoss)
						{
							ExitLongStopMarket(0, true, Position.Quantity, Position.AveragePrice - candleBarOffsetStop, "MyStopLong", "MyEntryLong"); 
						}
						
						if (profitTarget)
						{
							ExitLongLimit(0, true, Position.Quantity, Position.AveragePrice + (FinalTargetRR * TickSize), "MyTargetLong", "MyEntryLong");
						}
					}
				
				if (dualTarget)
				{
					if(stopLoss)
					{
						ExitLongStopMarket(0, true, Position.Quantity, Position.AveragePrice - candleBarOffsetStop, "MyStopLong", "MyEntryLong");
						ExitLongStopMarket(0, true, Position.Quantity, Position.AveragePrice - candleBarOffsetStop, "MyStopLong2", "MyEntryLong2");
					}
					
					if(profitTarget)
					{
						ExitLongLimit(0, true, Position.Quantity, Position.AveragePrice + (FirstTargetRR * TickSize), "MyTargetLong", "MyEntryLong");
						ExitLongLimit(0, true, Position.Quantity, Position.AveragePrice + (FinalTargetRR * TickSize), "MyTargetLong2", "MyEntryLong2");
					}	
				}
				
				#endregion
				
				#region Management Targets
				
				//Breakeven Area
				breakevenAreaTrigger = Position.AveragePrice + (breakevenAreaTarget * TickSize);
				breakevenAreaStopSet = breakevenAreaLong - candleBarOffsetBreakeven;
			
				if (breakevenAreaSetAuto)
				{
					myFreeBEArea = true;
				}
				
				//Breakeven Actual
				breakevenActualTrigger = Position.AveragePrice + (breakevenActualTarget * TickSize);
				breakevenActualStopSet = Position.AveragePrice - candleBarOffsetBreakeven;
				
				if (breakevenActualSetAuto)
				{
					myFreeBEActual = true;
				}
				
				//Candle Trail Stop
				candleTrailTrigger = Position.AveragePrice + (candleTrailTarget * TickSize);
				
				if (candleTrailSetAuto)
				{
					myFreeCandleTrail = true;
					activeTrail = true;
				}
				
				#endregion
				
				#region Management Prints
				
				if (SystemPrint)
				{
					#region Breakeven
					
					if (BreakevenPrints)
					{
						Print("breakevenAreaTrigger " + breakevenAreaTrigger + " " +  Time[1]);
						Print("breakevenAreaStopSet " + breakevenAreaStopSet + " " + Time[1]);
						Print("myFreeBEArea " + myFreeBEArea + " " + Time[1]);
					}
					
					#endregion
				}
				
				#endregion
				
				myFreeTrade = false;
			}
			
			#endregion
			

				//Short 
				#region Filled Short Position	
			
				#region Stop/Profits
			
				if (Position.MarketPosition == MarketPosition.Short && myFreeTrade == true && Position.Quantity == positionSizeShort)
				{
					if (dualTarget == false)
					{
						if (stopLoss)
						{
							ExitShortStopMarket(0, true, Position.Quantity, Position.AveragePrice + candleBarOffsetStop, "MyStopShort", "MyEntryShort");
						}
						
						if (profitTarget)
						{
							ExitShortLimit(0, true, Position.Quantity, Position.AveragePrice - (FinalTargetRR * TickSize), "MyTargetShort", "MyEntryShort");
						}
					}
				
				if (dualTarget)
				{				
			
					if(stopLoss)
					{
						ExitShortStopMarket(0, true, Position.Quantity, Position.AveragePrice + candleBarOffsetStop, "MyStopShort", "MyEntryShort");
						ExitShortStopMarket(0, true, Position.Quantity, Position.AveragePrice + candleBarOffsetStop, "MyStopShort2", "MyEntryShort2");
					}
					
					if(profitTarget)
					{
						ExitShortLimit(0, true, Position.Quantity, Position.AveragePrice - (FirstTargetRR * TickSize), "MyTargetShort", "MyEntryShort");
						ExitShortLimit(0, true, Position.Quantity, Position.AveragePrice - (FinalTargetRR * TickSize), "MyTargetShort2", "MyEntryShort2");
					}	
				}
				
				#endregion
				
				#region Management Targets
				
				//Breakeven Area
				breakevenAreaTrigger = Position.AveragePrice - (breakevenAreaTarget * TickSize);
				breakevenAreaStopSet = breakevenAreaShort + candleBarOffsetBreakeven;
			
				if (breakevenAreaSetAuto)
				{
					myFreeBEArea = true;
				}
				
				//Breakeven Actual
				breakevenActualTrigger = Position.AveragePrice - (breakevenActualTarget * TickSize);
				breakevenActualStopSet = Position.AveragePrice + candleBarOffsetBreakeven;
				
				if (breakevenActualSetAuto)
				{
					myFreeBEActual = true;
				}
				
				//Candle Trail Stop
				candleTrailTrigger = Position.AveragePrice - (candleTrailTarget * TickSize);
				
				if (candleTrailSetAuto)
				{
					myFreeCandleTrail = true;
					activeTrail = true;
				}
				
				#endregion
				
				#region Management Prints
				
				if (SystemPrint)
				{
					#region Breakeven
					
					if (BreakevenPrints)
					{
						Print("breakeven(Area)Trigger " + breakevenAreaTrigger + " " +  Time[1]);
						Print("breakeven(Area)StopSet " + breakevenAreaStopSet + " " + Time[1]);
						Print("myFreeBE(Area) " + myFreeBEArea + " " + Time[1]);
						
						Print("breakeven(Actual)Trigger " + breakevenActualTrigger + " " +  Time[1]);
						Print("breakeven(Actual)StopSet " + breakevenActualStopSet + " " + Time[1]);
						Print("myFreeBE(Actual) " + myFreeBEActual + " " + Time[1]);
					}
					
					#endregion
				}
				
				#endregion
				
				myFreeTrade = false;
				
				myFillCheck = true;
			}
				
			#endregion
				
				#region Partial Fill Short Position
				
				#region Stop/Profits
				
			if (Position.MarketPosition == MarketPosition.Short && myFreeTrade == true && Position.Quantity < positionSizeShort && IsFirstTickOfBar)
			{
				
				
				if (dualTarget == false)
					{
						if (stopLoss)
						{
							ExitShortStopMarket(0, true, Position.Quantity, Position.AveragePrice + candleBarOffsetStop, "MyStopShort", "MyEntryShort");
						}
						
						if (profitTarget)
						{
							ExitShortLimit(0, true, Position.Quantity, Position.AveragePrice - (FinalTargetRR * TickSize), "MyTargetShort", "MyEntryShort");
						}
					}
				
				if (dualTarget)
				{

					
					if(stopLoss)
					{
						ExitShortStopMarket(0, true, Position.Quantity, Position.AveragePrice + candleBarOffsetStop, "MyStopShort", "MyEntryShort");
						ExitShortStopMarket(0, true, Position.Quantity, Position.AveragePrice + candleBarOffsetStop, "MyStopShort2", "MyEntryShort2");
					}
					
					if(profitTarget)
					{
						ExitShortLimit(0, true, Position.Quantity, Position.AveragePrice - (FirstTargetRR * TickSize), "MyTargetShort", "MyEntryShort");
						ExitShortLimit(0, true, Position.Quantity, Position.AveragePrice - (FinalTargetRR * TickSize), "MyTargetShort2", "MyEntryShort2");
					}	
					
					
				}
				
				#endregion
				
				#region Management Targets
				
				//Breakeven Area
				breakevenAreaTrigger = Position.AveragePrice - (breakevenAreaTarget * TickSize);
				breakevenAreaStopSet = breakevenAreaShort + candleBarOffsetBreakeven;
			
				if (breakevenAreaSetAuto)
				{
					myFreeBEArea = true;
				}
				
				//Breakeven Actual
				breakevenActualTrigger = Position.AveragePrice - (breakevenActualTarget * TickSize);
				breakevenActualStopSet = Position.AveragePrice + candleBarOffsetBreakeven;
				
				if (breakevenActualSetAuto)
				{
					myFreeBEActual = true;
				}
				
				//Candle Trail Stop
				candleTrailTrigger = Position.AveragePrice - (candleTrailTarget * TickSize);
				
				if (candleTrailSetAuto)
				{
					myFreeCandleTrail = true;
					activeTrail = true;
				}
				
				#endregion
				
				#region Management Prints
				
				if (SystemPrint)
				{
					#region Breakeven
					
					if (BreakevenPrints)
					{
						Print("breakeven(Area)Trigger " + breakevenAreaTrigger + " " +  Time[1]);
						Print("breakeven(Area)StopSet " + breakevenAreaStopSet + " " + Time[1]);
						Print("myFreeBE(Area) " + myFreeBEArea + " " + Time[1]);
						
						Print("breakeven(Actual)Trigger " + breakevenActualTrigger + " " +  Time[1]);
						Print("breakeven(Actual)StopSet " + breakevenActualStopSet + " " + Time[1]);
						Print("myFreeBE(Actual) " + myFreeBEActual + " " + Time[1]);
					}
					
					#endregion
				}
				
				#endregion
				
				myFreeTrade = false;
			}
			
			#endregion
			
			}
			
			
			///Management Logic - Breakeven/Trail Stop
		
			#region Breakeven Area
			
				#region Long
			
			if (
				(Position.MarketPosition == MarketPosition.Long)
					&& ((Close[0] >= breakevenAreaTrigger) || (breakevenButtonClicked))
						&& (myFreeBEArea == true)
				)
			{
				#region Filled Long Position	
			
				if (Position.MarketPosition == MarketPosition.Long && myFreeBEArea == true && Position.Quantity == positionSizeLong)
				{
					if (dualTarget == false)
					{
						ExitLongStopMarket(0, true, Position.Quantity, breakevenAreaStopSet, "MyStopLong", "MyEntryLong");
					}
				
					if (dualTarget)
					{
						ExitLongStopMarket(0, true, firstTargetPositionLong, breakevenAreaStopSet, "MyStopLong", "MyEntryLong");
						ExitLongStopMarket(0, true, secondTargetPositionLong, breakevenAreaStopSet, "MyStopLong2", "MyEntryLong2");	
					}
				
				breakevenButtonClicked = false;
				myFreeBEArea = false;
			}
				
			#endregion
				
				#region Partial Fill Long Position
				
			if (Position.MarketPosition == MarketPosition.Long && myFreeBEArea == true && Position.Quantity < positionSizeLong)// && IsFirstTickOfBar)
			{
				if (dualTarget == false)
					{
						ExitLongStopMarket(0, true, Position.Quantity, breakevenAreaStopSet, "MyStopLong", "MyEntryLong");
					}
					
				if (dualTarget == true && myFillCheck == true)
				{
					ExitLongStopMarket(0, true, Position.Quantity, breakevenAreaStopSet, "MyStopLong", "MyEntryLong");
					ExitLongStopMarket(0, true, Position.Quantity, breakevenAreaStopSet, "MyStopLong2", "MyEntryLong2");
				}
			
				breakevenButtonClicked = false;
				myFreeBEArea = false;;
			}
			
			#endregion
			}
			
				#endregion
			
				#region Short
			
			if (
				(Position.MarketPosition == MarketPosition.Short)
					&& ((Close[0] <= breakevenAreaTrigger) || (breakevenButtonClicked))
						&& (myFreeBEArea == true)
				)
			{
				#region Filled Short Position	
			
				if (Position.MarketPosition == MarketPosition.Short && myFreeBEArea == true && Position.Quantity == positionSizeShort)
				{
					if (dualTarget == false)
					{
						ExitShortStopMarket(0, true, Position.Quantity, breakevenAreaStopSet, "MyStopShort", "MyEntryShort");
					}
				
				if (dualTarget)
				{
						ExitShortStopMarket(0, true, firstTargetPositionShort, breakevenAreaStopSet, "MyStopShort", "MyEntryShort");
						ExitShortStopMarket(0, true, secondTargetPositionShort, breakevenAreaStopSet, "MyStopShort2", "MyEntryShort2");	
				}
				
				breakevenButtonClicked = false;
				myFreeBEArea = false;
			}
				
			#endregion
				
				#region Partial Fill Short Position
				
			if (Position.MarketPosition == MarketPosition.Short && myFreeBEArea == true && Position.Quantity < positionSizeShort)
			{
				if (dualTarget == false)
				{
					ExitShortStopMarket(0, true, Position.Quantity, breakevenAreaStopSet, "MyStopShort", "MyEntryShort");
				}
					
				if (dualTarget == true && myFillCheck == true)
				{
					ExitShortStopMarket(0, true, Position.Quantity, breakevenAreaStopSet, "MyStopShort", "MyEntryShort");
					ExitShortStopMarket(0, true, Position.Quantity, breakevenAreaStopSet, "MyStopShort2", "MyEntryShort2");
				}
			
				breakevenButtonClicked = false;
				myFreeBEArea = false;;
			}
			
			#endregion
			}
			
				#endregion
			
			#endregion
			
			#region Breakeven Actual
			
				#region Long
			
			if (
				(Position.MarketPosition == MarketPosition.Long)
					&& ((Close[0] >= breakevenActualTrigger) || (breakevenButtonClicked))
						&& (myFreeBEActual == true)
				)
			{
				#region Filled Long Position	
			
				if (Position.MarketPosition == MarketPosition.Long && myFreeBEActual == true && Position.Quantity == positionSizeLong)
				{
					if (dualTarget == false)
					{
						ExitLongStopMarket(0, true, Position.Quantity, breakevenActualStopSet, "MyStopLong", "MyEntryLong");
					}
				
				if (dualTarget)
				{
						ExitLongStopMarket(0, true, firstTargetPositionLong, breakevenActualStopSet, "MyStopLong", "MyEntryLong");
						ExitLongStopMarket(0, true, secondTargetPositionLong, breakevenActualStopSet, "MyStopLong2", "MyEntryLong2");	
				}
				
				breakevenButtonClicked = false;
				myFreeBEActual = false;
			}
				
			#endregion
				
				#region Partial Fill Long Position
				
			if (Position.MarketPosition == MarketPosition.Long && myFreeBEActual == true && Position.Quantity < positionSizeLong)// && IsFirstTickOfBar)
			{
				if (dualTarget == false)
					{
						ExitLongStopMarket(0, true, Position.Quantity, breakevenActualStopSet, "MyStopLong", "MyEntryLong");
					}
					
				if (dualTarget == true && myFillCheck == true)
				{
					ExitLongStopMarket(0, true, Position.Quantity, breakevenActualStopSet, "MyStopLong", "MyEntryLong");
					ExitLongStopMarket(0, true, Position.Quantity, breakevenActualStopSet, "MyStopLong2", "MyEntryLong2");
				}
				
				breakevenButtonClicked = false;
				myFreeBEActual = false;
			}
			
			#endregion
			}
			
				#endregion
			
				#region Short
			
			if (
				(Position.MarketPosition == MarketPosition.Short)
					&& ((Close[0] <= breakevenActualTrigger) || (breakevenButtonClicked))
						&& (myFreeBEActual == true) 
				)
			{
				#region Filled Short Position	
			
				if (Position.MarketPosition == MarketPosition.Short && myFreeBEActual == true && Position.Quantity == positionSizeShort)
				{
					if (dualTarget == false)
					{
						ExitShortStopMarket(0, true, Position.Quantity, breakevenActualStopSet, "MyStopShort", "MyEntryShort");
					}
				
					if (dualTarget)
					{
						ExitShortStopMarket(0, true, firstTargetPositionShort, breakevenActualStopSet, "MyStopShort", "MyEntryShort");
						ExitShortStopMarket(0, true, secondTargetPositionShort, breakevenActualStopSet, "MyStopShort2", "MyEntryShort2");	
					}
				
				breakevenButtonClicked = false;
				myFreeBEActual = false;
			}
				
			#endregion
				
				#region Partial Fill Short Position
				
			if (Position.MarketPosition == MarketPosition.Short && myFreeBEActual == true && Position.Quantity < positionSizeShort) //&& IsFirstTickOfBar)
			{
				if (dualTarget == false)
					{
						ExitShortStopMarket(0, true, Position.Quantity, breakevenActualStopSet, "MyStopShort", "MyEntryShort");
					}
				
				if (dualTarget == true && myFillCheck == true)
				{
					ExitShortStopMarket(0, true, Position.Quantity, breakevenActualStopSet, "MyStopShort", "MyEntryShort");
					ExitShortStopMarket(0, true, Position.Quantity, breakevenActualStopSet, "MyStopShort2", "MyEntryShort2");
					
					Print("At Breakeven: " + Position.Quantity);
				}
				
				breakevenButtonClicked = false;
				myFreeBEActual = false;
			}
			
			#endregion
			}
			
				#endregion
				
			#endregion
			
			#region Custom Stop Move
			///Will add this at a later date
			#endregion
			
			#region Trail Stop
			
			if (
				(myFreeCandleTrail == true && activeTrail)
					
				&& ((Position.MarketPosition == MarketPosition.Long && (Close[0] >= candleTrailTrigger || trailButtonClicked))
					
					|| (Position.MarketPosition == MarketPosition.Short && (Close[0] <= candleTrailTrigger || trailButtonClicked)))
				)
			
				{
				
					#region Trail Offset
			
					//Define the area where stop will be set			
					trailAreaLong		= Low[1];
					trailAreaShort		= High[1];
					
					//Adds offset to your trail stop area. Gives user customization.		
					percentageCalcTrail 		= ((High[2] - Low[2]) * PercentageOffsetTrail);
					priceCalcTrail 			= PriceOffsetTrail;
					tickCalcTrail			= TickOffsetTrail * TickSize;
					
					//Picks the highest of the 3 numbers			
					candleBarOffsetTrail = Math.Max(percentageCalcTrail, Math.Max(priceCalcTrail, tickCalcTrail));
					
					//Add both of them together to define final entry point			
					candleTrailStopSetLong = trailAreaLong - candleBarOffsetTrail;
					candleTrailStopSetShort = trailAreaShort + candleBarOffsetTrail;
					
					#region Prints
					
					if (SystemPrint)
					{
						if (TrailPrints)
						{
							Print("Current Trail Price Offset is :  " + priceCalcTrail + " " + Time[1]);
							Print("Current Trail Percent Offset is :  " + percentageCalcTrail + " " + Time[1]);
							Print("Current Trail Tick Offset is :  " + tickCalcTrail + " " + Time[1]);
							
							Print("Current Trail Highest Offset Selected is :  " + candleBarOffsetTrail + " " + Time[1]);
							
							Print("candleTrailStopSetLong is :  " + candleTrailStopSetLong + " " + Time[1]);	
							Print("candleTrailStopSetShort is :  " + candleTrailStopSetShort + " " + Time[1]);	
						
							Print("myFree Trail Offset " + myFreeCandleTrail + " " + Time[1]);
						}
						
					}
					#endregion
			
			
					#endregion 	
					
					trailTriggeredCandle = true; //Allows condition to move stop freely. 
					myFreeCandleTrail = false; //Sets bool back to false. Needs to wait another candle for calculations to happen again	
				}	
		
				#region Long Trail
				
				#region Filled Long Position	
			
				if (Position.MarketPosition == MarketPosition.Long && trailTriggeredCandle == true && Position.Quantity == positionSizeLong && (Low[1] > Low[2]) && Open[0] > Low[1])
				{
					Print("Open[0] " + Open[0]);
					Print("High[1]" + High[1]);
					if (dualTarget == false)
					{
						ExitLongStopMarket(0, true, Position.Quantity, candleTrailStopSetLong , "MyStopLong", "MyEntryLong");
					}
				
					if (dualTarget)
					{
						ExitLongStopMarket(0, true, firstTargetPositionLong, candleTrailStopSetLong, "MyStopLong", "MyEntryLong");
						ExitLongStopMarket(0, true, secondTargetPositionLong, candleTrailStopSetLong, "MyStopLong2", "MyEntryLong2");	
					}
				
				//trailButtonClicked 		= false;
				trailTriggeredCandle 	= false;
			}
				
			#endregion
				
				#region Partial Fill Long Position
				
			if (Position.MarketPosition == MarketPosition.Long && trailTriggeredCandle == true && Position.Quantity < positionSizeLong && (Low[1] > Low[2]) && Open[0] > Low[1])
			{
				if (dualTarget == false)
				{
					ExitLongStopMarket(0, true, Position.Quantity, candleTrailStopSetLong, "MyStopLong", "MyEntryLong");
				}
				
				if (dualTarget == true && myFillCheck == true)
				{
					ExitLongStopMarket(0, true, Position.Quantity, candleTrailStopSetLong, "MyStopLong", "MyEntryLong");
					ExitLongStopMarket(0, true, Position.Quantity, candleTrailStopSetLong, "MyStopLong2", "MyEntryLong2");
				}
				
				trailTriggeredCandle 	= false;
			}
			
			#endregion
			
				#endregion
			
				#region Short Trail
				
				#region Filled Short Position	
			
				if (Position.MarketPosition == MarketPosition.Short && trailTriggeredCandle == true && Position.Quantity == positionSizeShort && (High[1] < High[2]) && Open[0] < High[1])
				{
					if (dualTarget == false)
					{
						ExitShortStopMarket(0, true, Position.Quantity, candleTrailStopSetShort , "MyStopShort", "MyEntryShort");
					}
				
				if (dualTarget)
				{
						ExitShortStopMarket(0, true, firstTargetPositionShort, candleTrailStopSetShort, "MyStopShort", "MyEntryShort");
						ExitShortStopMarket(0, true, secondTargetPositionShort, candleTrailStopSetShort, "MyStopShort2", "MyEntryShort2");	
				}
				
				//trailButtonClicked 		= false;
				trailTriggeredCandle 	= false;
			}
				
			#endregion
				
				#region Partial Fill Short Position
				
			if (Position.MarketPosition == MarketPosition.Short && trailTriggeredCandle == true && Position.Quantity < positionSizeShort && (High[1] < High[2]) && Open[0] < High[1])
			{
				if (dualTarget == false)
				{
					ExitShortStopMarket(0, true, Position.Quantity, candleTrailStopSetShort, "MyStopShort", "MyEntryShort");
				}
					
				if (dualTarget == true && myFillCheck == true)
				{
					ExitShortStopMarket(0, true, Position.Quantity, candleTrailStopSetShort, "MyStopShort", "MyEntryShort");
					ExitShortStopMarket(0, true, Position.Quantity, candleTrailStopSetShort, "MyStopShort2", "MyEntryShort2");
				}
				
				trailTriggeredCandle 	= false;

			}
			
			#endregion
			
				#endregion
				
			#endregion
			
		}
		
	
		protected override void OnPositionUpdate(Position position, double averagePrice, int quantity, MarketPosition marketPosition)
		{
			//myPosition = Position.Quantity;
			
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				countOnce 		= true;
				activateMarket 	= false;
				myFillCheck 	= false;
			}
			
			if (Position.MarketPosition != MarketPosition.Flat)
			{
				RemoveDrawObject("EntryLine");
				RemoveDrawObject("StopLine");
			}
			
			#region Trail Stop Cancel
			
			if (Position.MarketPosition == MarketPosition.Flat) 
			{
				myFreeCandleTrail = false;
				
				if (SystemPrint)
				{
					if (TrailPrints)
					{
						Print("myFree Trail ON Position " + myFreeCandleTrail + " " + Time[1]);
					}
				}
				
			}
			
			#endregion
			
			#region Fib Levels Updates
			
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				fibCount 	= 1;
				isFib 		= false;;
				
				RemoveDrawObject("FibLong");
				RemoveDrawObject("FibShort");
			}
			
			if (fibButtonClicked && Position.MarketPosition == MarketPosition.Long)
			{
				if (rrMode)
				{
					Draw.FibonacciRetracements(this, "FibLong", false, fibCount, setStopLong, fibCount, enterLong);
				}
				
				if (tickMode)
				{
					Draw.FibonacciRetracements(this, "FibLong", false, fibCount, Position.AveragePrice - candleBarOffsetStop, fibCount, Position.AveragePrice);
				}
				
			}
			
			if (fibButtonClicked && Position.MarketPosition == MarketPosition.Short)
			{
				if (rrMode)
				{
					Draw.FibonacciRetracements(this, "FibShort", false, fibCount, setStopShort, fibCount, enterShort);
				}
				
				if (tickMode)
				{
					Draw.FibonacciRetracements(this, "FibShort", false, fibCount, Position.AveragePrice + candleBarOffsetStop, fibCount, Position.AveragePrice);
				}
			}
			
			#endregion
			
			
		}
		
			
		protected override void OnExecutionUpdate(Execution execution, string executionId, double price, int quantity, MarketPosition marketPosition, string orderId, DateTime time)
		{
			#region Button Event Updates
			
			//Buy High
			if (Position.MarketPosition != MarketPosition.Flat)
				{
					ChartControl.Dispatcher.InvokeAsync(new Action(() => 
			 {
	            longButtonHL.Content = "BUY HIGH";
				longButtonHLClicked = false;
	        }));
					
				}
			
			//Sell Low
			if (Position.MarketPosition != MarketPosition.Flat)
				{
					ChartControl.Dispatcher.InvokeAsync(new Action(() => 
			 {
	            shortButtonHL.Content = "SELL LOW";
				shortButtonHLClicked = false;
	        }));
					
				}
				
			//Custom Long	
			if (Position.MarketPosition != MarketPosition.Flat)
				{
					ChartControl.Dispatcher.InvokeAsync(new Action(() => 
			 {
	            customLong.Content = "Custom Long";
				customLong.Background	= Brushes.MediumSeaGreen; 
				armMarketCount = 0;
				customLongClicked = false;
	        }));
					
				}
				
			//Custom Short	
			if (Position.MarketPosition != MarketPosition.Flat)
				{
					ChartControl.Dispatcher.InvokeAsync(new Action(() => 
			 {
	            customShort.Content = "Custom Short";
				customShort.Background	= Brushes.IndianRed; 
				armMarketCount = 0; 
				customShortClicked = false;
	        }));
		
				}	
				
				
			if (Position.MarketPosition == MarketPosition.Flat)
				{
					ChartControl.Dispatcher.InvokeAsync(new Action(() => 
			 {
	            trailButton.Content = "Trail Stop";
				trailButton.Background	= Brushes.LightGray; 
				trailButtonClicked = false;
	        }));
		
				}		
				
				#endregion
		}
			
		protected void RemoveWPFControls()
		{
			#region Remove WPF
			
			if (!panelActive)
				return;
			
			if (chartTraderButtonsGrid != null || lowerButtonsGrid != null)
			{
				chartTraderGrid.Children.Remove(lowerButtonsGrid);
				chartTraderGrid.RowDefinitions.Remove(addedRow);
			}

			panelActive = false;
			
			#endregion
		}
		
		private bool TabSelected()
		{
			#region TabSelcected 
			
			bool tabSelected = false;

			// loop through each tab and see if the tab this indicator is added to is the selected item
			foreach (System.Windows.Controls.TabItem tab in chartWindow.MainTabControl.Items)
				if ((tab.Content as Gui.Chart.ChartTab).ChartControl == ChartControl && tab == chartWindow.MainTabControl.SelectedItem)
					tabSelected = true;

			return tabSelected;
				
			#endregion
		}
		
		private void TabChangedHandler(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			#region TabHandler
			
			if (e.AddedItems.Count <= 0)
				return;

			tabItem = e.AddedItems[0] as System.Windows.Controls.TabItem;
			if (tabItem == null)
				return;

			chartTab = tabItem.Content as Gui.Chart.ChartTab;
			if (chartTab == null)
				return;

			if (TabSelected())
				InsertWPFControls();
			else
				RemoveWPFControls();
			
			#endregion
		}
		
		
		#region Button Click Events
		
		private void OnButtonClick(object sender, RoutedEventArgs rea)
		{
			System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
			
			#region TradeSaber Socials
			
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
				
				if (button == tradeSaberButton && button.Name == "TradeSaberButton" && button.Content == "TradeSaber")
				{	
					System.Diagnostics.Process.Start(tradeSaber);
					return;
				}
			}
			
			#endregion
		}
		
		#endregion
		
		
		#region Properties
			
		#region 01. Position Size
		
		[Display(Name = "Order Management Method", GroupName = "01. Position Size", Description="", Order = 0)]
		public OrderManagement	ManagementType
		{
			get { return managementType; }
			set { managementType = value; }
		}	
		
		
		
		[RefreshProperties(RefreshProperties.All)]
		[NinjaScriptProperty]
		[Display(Name = "Auto Position Size", Order = 1, GroupName = "01. Position Size")]
		public bool AutoPositionSize
		{
		    get
		   {
		      return autoPositionSize;
		   }
		   set
		   {
		      if (value == true)
		      {
		         CustomPositionSize = false;
		      }
		      autoPositionSize = value;
		    }
		}

		[NinjaScriptProperty]
		[Range(1, double.MaxValue)]
		[Display(Name="Risk Size($)", Order=2, GroupName="01. Position Size")]
		public double MaxLossPerTrade
		{ get; set; }
		
		[RefreshProperties(RefreshProperties.All)]
		[NinjaScriptProperty]
		[Display(Name = "Custom Position Size", Order = 3, GroupName = "01. Position Size")]
		public bool CustomPositionSize
		{
		   get
		   {
		      return customPositionSize;
		   }
		   set
		   {
		      if (value == true)
		      {
		         AutoPositionSize = false;
		      }
		      customPositionSize = value;
		   }
		}
		
		[Range(1, int.MaxValue)]
		[NinjaScriptProperty]
		[Display(Name="Custom Position Amount", Order=4, GroupName="01. Position Size")]
		public int CustomPositionAmount
		{ get; set; }
		
		
		[NinjaScriptProperty]
		[Display(Name="Use Candle Range", Order=5, GroupName="01. Position Size")]
		public bool CandleRange
		{ get; set; }
		
		#endregion
	
		#region 02. Entry / Limit
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Entry Candle Used", Order=0, GroupName="02. Entry")]
		public int CandleLookBackEntry
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Price Offset Entry", Order=1, GroupName="02. Entry")]
		public double PriceOffsetEntry
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Percentage Offset Entry", Order=2, GroupName="02. Entry")]
		public double PercentageOffsetEntry
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Tick Offset Entry", Order=3, GroupName="02. Entry")]
		public int TickOffsetEntry
		{ get; set; }

		
		//Limit Orders
		[NinjaScriptProperty]
		[Display(Name="Limit Order Entry", Order=4, GroupName="02A. Limit Order")]
		public bool useLimit
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Price Offset Limit", Order=5, GroupName="02A. Limit Order")]
		public double PriceOffsetLimit
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Percentage Offset Limit", Order=6, GroupName="02A. Limit Order")]
		public double PercentageOffsetLimit
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Tick Offset Limit", Order=7, GroupName="02A. Limit Order")]
		public int TickOffsetLimit
		{ get; set; }
		
		#endregion
	
		#region 03. Stop
		
		[NinjaScriptProperty]
		[Display(Name="Set Stop Loss", Order=0, GroupName="03. Stop")]
		public bool stopLoss
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Stop Candle Used", Order=1, GroupName="03. Stop")]
		public int CandleLookBackStop
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Price Offset Stop", Order=2, GroupName="03. Stop")]
		public double PriceOffsetStop
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Percentage Offset Stop", Order=3, GroupName="03. Stop")]
		public double PercentageOffsetStop
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Tick Offset Stop", Order=4, GroupName="03. Stop")]
		public int TickOffsetStop
		{ get; set; }

		#endregion
	
		#region 04. Profit Targets
		
		[NinjaScriptProperty]
		[Display(Name="Set Profit Target", Order=0, GroupName="04. Target")]
		public bool profitTarget
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Final Target", Order=2, GroupName="04. Target")]
		public double FinalTargetRR
		{ get; set; }
		
		#endregion
		
		#region 04A. Dual Target
		
		[NinjaScriptProperty]
		[Display(Name="DualTarget", Order=1, GroupName="04A. Dual Target")]
		public bool dualTarget
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Dual Entry Split Percent", Order=2, GroupName="04A. Dual Target")]
		public double splitPercent
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="First Target", Order=3, GroupName="04A. Dual Target")]
		public double FirstTargetRR
		{ get; set; }
		
		#endregion
		
		#region 05. Custom Button 
		
		[RefreshProperties(RefreshProperties.All)]
		[NinjaScriptProperty]
		[Display(Name = "'Close' Price", Order = 2, GroupName = "05. Custom Button")]
		public bool EnterClosePrice
		{
		    get
		   {
		      return enterClosePrice;
		   }
		   set
		   {
		      if (value == true)
		      {
		         ArmMarket = false;
		      }
		      enterClosePrice = value;
		    }
		}

		
		
		[RefreshProperties(RefreshProperties.All)]
		[NinjaScriptProperty]
		[Display(Name = "Arm Market (New Candle)", Order = 3, GroupName = "05. Custom Button")]
		public bool ArmMarket
		{
		   get
		   {
		      return armMarket;
		   }
		   set
		   {
		      if (value == true)
		      {
		         EnterClosePrice = false;
		      }
		      armMarket = value;
		   }
		}
		
		///
		
		/*
		[NinjaScriptProperty]
		[Display(Name="Add Offset", Order=3, GroupName="01. Risk Parameters")]
		public bool AddOffset
		{ get; set; }
		*/
		#endregion
		
		#region 06. Breakeven Offset
		
		//Breakeven Offset
		[NinjaScriptProperty]
		[Display(Name="Price Offset Breakeven", Order=1, GroupName="06. Breakeven Offset")]
		public double PriceOffsetBreakeven
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Percentage Offset Breakeven", Order=2, GroupName="06. Breakeven Offset")]
		public double PercentageOffsetBreakeven
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Tick Offset Breakeven", Order=3, GroupName="06. Breakeven Offset")]
		public int TickOffsetBreakeven
		{ get; set; }
		
		#endregion
		
		#region 06A. Breakeven Area
		
		//Breakeven Area
		[RefreshProperties(RefreshProperties.All)]
		[NinjaScriptProperty]
		[Display(Name="Breakeven (Area) Button", Order=4, GroupName="06A. Breakeven Area")]
		public bool BreakevenAreaSetButton
		{
		    get
		   {
		      return breakevenAreaSetButton;
		   }
		   set
		   {
		      if (value == true)
		      {
		         BreakevenActualSetButton = false;
		      }
		      breakevenAreaSetButton = value;
		    }
		}
		
		
		[RefreshProperties(RefreshProperties.All)]
		[NinjaScriptProperty]
		[Display(Name="Breakeven (Area) Set Auto", Order=5, GroupName="06A. Breakeven Area")]
		public bool BreakevenAreaSetAuto
		{
		    get
		   {
		      return breakevenAreaSetAuto;
		   }
		   set
		   {
		      if (value == true)
		      {
		         BreakevenActualSetAuto = false;
		      }
		      breakevenAreaSetAuto = value;
		    }
		}
		
		[NinjaScriptProperty]
		[Display(Name="Breakeven (Area) Target", Order=6, GroupName="06A. Breakeven Area")]
		public double breakevenAreaTarget	
		{ get; set; }
		
		#endregion
		
		#region 06B. Breakeven Actual
		
		//Breakeven Actual
		[RefreshProperties(RefreshProperties.All)]
		[NinjaScriptProperty]
		[Display(Name=" Breakeven (Actual) Button", Order=7, GroupName="06B. Breakeven Actual")]	
		public bool BreakevenActualSetButton
		{
		   get
		   {
		      return  breakevenActualSetButton;
		   }
		   set
		   {
		      if (value == true)
		      {
		         BreakevenAreaSetButton = false;
		      }
		       breakevenActualSetButton = value;
		   }
		}
		
		
		
		[RefreshProperties(RefreshProperties.All)]
		[NinjaScriptProperty]
		[Display(Name="Breakeven (Actual) Set Auto", Order=8, GroupName="06B. Breakeven Actual")]	
		public bool BreakevenActualSetAuto
		{
		   get
		   {
		      return breakevenActualSetAuto;
		   }
		   set
		   {
		      if (value == true)
		      {
		         BreakevenAreaSetAuto = false;
		      }
		      breakevenActualSetAuto = value;
		   }
		}
		
		
		[NinjaScriptProperty]
		[Display(Name="Breakeven (Actual) Target", Order=9, GroupName="06B. Breakeven Actual")]
		public double breakevenActualTarget	
		{ get; set; }
		
		#endregion
		
		#region 07. Trail Stop Offset

		//Trail Offset
		[NinjaScriptProperty]
		[Display(Name="Price Offset Trail", Order=1, GroupName="07. Trail Stop Offset")]
		public double PriceOffsetTrail
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Percentage Offset Trail", Order=2, GroupName="07. Trail Stop Offset")]
		public double PercentageOffsetTrail
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Tick Offset Trail", Order=3, GroupName="07. Trail Stop Offset")]
		public int TickOffsetTrail
		{ get; set; }
		
		#endregion
		
		#region 07A. Candle Trail
		
		[RefreshProperties(RefreshProperties.All)]
		[NinjaScriptProperty]
		[Display(Name = "Candle Trail (Button)", Order = 1, GroupName = "07A. Trail Stop")]
		public bool candleTrailSetButton
		{ get; set; }
		
		[RefreshProperties(RefreshProperties.All)]
		[NinjaScriptProperty]
		[Display(Name = "Candle Trail (Auto)", Order = 2, GroupName = "07A. Trail Stop")]
		public bool candleTrailSetAuto
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Candle Trail Target", Order=3, GroupName="07A. Trail Stop")]
		public double candleTrailTarget	
		{ get; set; }
		
		#endregion

		#region 9 Display Text Box
		
		[NinjaScriptProperty]
		[Display(Name="Display Text", Order=1, GroupName="9 Display Text Box")]
		public bool DisplayText
		{ get; set; }
		
		#endregion
		
		#region For Later Use
		/*
		//custom Move Stop
		[NinjaScriptProperty]
		[Display(Name="customMoveStopSet", Order=15, GroupName="10. Management")]
		public bool customMoveStopSet
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="customeMoveStopTarget", Order=16, GroupName="10. Management")]
		public double customeMoveStopTarget	
		{ get; set; }
		
		//Custom Trail
		[NinjaScriptProperty]
		[Display(Name="customTrailSet", Order=7, GroupName="10. Trail Stop2")]
		public bool customTrailSet
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="customTrailTarget", Order=8, GroupName="10. Trail Stop2")]
		public double customTrailTarget	
		{ get; set; }
		*/
		#endregion
		
		#region 29. TradeSaber Socials
		
		[NinjaScriptProperty]
		[Display(Name = "Show Social Media Buttons", Description = "", Order = 0, GroupName = "29. TradeSaber Socials")]
		public bool ShowSocials 
		{
		 	get{return showSocials;} 
			set{showSocials = (value);} 
		}
		
		[NinjaScriptProperty]
		[Display(Name="Explanation Video", Order=1, GroupName="29. TradeSaber Socials")]
		public  string Youtube
		{
		 	get{return youtube;} 
			set{youtube = (value);} 
		}
		
		[NinjaScriptProperty]
		[Display(Name="Discord Link", Order=2, GroupName="29. TradeSaber Socials")]
		public  string Discord
		{
		 	get{return discord;} 
			set{discord = (value);} 
		}
		
		[NinjaScriptProperty]
		[Display(Name="TradeSaber Link", Order=3, GroupName="29. TradeSaber Socials")]
		public  string TradeSaber
		{
		 	get{return tradeSaber;} 
			set{tradeSaber = (value);} 
		}
		
		[NinjaScriptProperty]
		[ReadOnly(true)]
		[Display(Name = "Author", GroupName = "29. TradeSaber Socials", Order = 4)]
		public string Author
		{
		 	get{return author;} 
			set{author = (value);} 
		}
		
		[NinjaScriptProperty]
		[ReadOnly(true)]
		[Display(Name = "Version", GroupName = "29. TradeSaber Socials", Order = 5)]
		public string Version
		{
		 	get{return version;} 
			set{version = (value);} 
		}
		
		#endregion
		
		#region 99. Prints
		
		[NinjaScriptProperty]
		[Display(Name="SystemPrint", Order=1, GroupName="99. Prints")]
		public bool SystemPrint
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="EntryPrints", Order=3, GroupName="99. Prints")]
		public bool EntryPrints
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="LimitPrints", Order=4, GroupName="99. Prints")]
		public bool LimitPrints
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="StopPrints", Order=5, GroupName="99. Prints")]
		public bool StopPrints
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="PositionSizePrints", Order=5, GroupName="99. Prints")]
		public bool PositionSizePrints
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="ProfitTatgetPrintss", Order=6, GroupName="99. Prints")]
		public bool ProfitTatgetPrints
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="BreakevenPrints", Order=7, GroupName="99. Prints")]
		public bool BreakevenPrints
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="TrailPrints", Order=8, GroupName="99. Prints")]
		public bool TrailPrints
		{ get; set; }
		//ProfitTatgetPrints
		#endregion
		
		#endregion
		
	}
}

