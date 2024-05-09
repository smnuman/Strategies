//Version 6.7.5
//Removed Bar size variable
//added a cancel button
//made cancel and flatten button bigger
//added a momentum trade button




//Version 6.7.4 
//Added Limit orders and removed exit code

//Version 6.7.3 Previous version change log at end of script
//Added direction switches on button click
//Cleaned up entry code
//Changed Controller flat visuals (text box)

// Version 6.7.3 
//Fixed order entry to calculate on each tick
//removed atm print output
//using limit order at close of last candle for better price entry
	
	
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
 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class ControllerV675 : Strategy
	{
	
		private NinjaTrader.Gui.Chart.ChartTab		chartTab;
		private NinjaTrader.Gui.Chart.Chart			chartWindow;
		System.Windows.Controls.Grid				chartTraderGrid;
		System.Windows.Controls.Grid				chartTraderButtonsGrid;
		System.Windows.Controls.Grid				lowerButtonsGrid;
		System.Windows.Controls.Button				longButton, shortButton, flatButton, cancelButton, momlongButton, momshortButton;
		private bool								panelActive;
		private System.Windows.Controls.TabItem		tabItem;
        System.Windows.Controls.Grid                evenlowerButtonsGrid;
		System.Windows.Controls.Grid                lowestButtonsGrid;
		System.Windows.Controls.Grid                cancelButtonsGrid;
        public bool                                 goShort, gomomShort;
        public bool                                 goLong, gomomLong;
      //ATM Variables Next
		private string  atmStrategyId			= string.Empty;
		private string  orderId					= string.Empty;
		private bool	isAtmStrategyCreated	= false;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Semi Automatic Trading, Chart Trader Buttons";
				Name										= "Controller V6.7.5 Orderflow Scalper";
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
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
                goShort = false;
                goLong = false;
				gomomShort = false;
                gomomLong = false;

				
			}
			
			
			else if (State == State.Historical)
			{
				if (ChartControl != null)
				{
					ChartControl.Dispatcher.InvokeAsync((Action)(() =>
					{
						CreateWPFControls();
					}));
				}
			}
			else if (State == State.Terminated)
			{
				if (ChartControl != null)
				{
					ChartControl.Dispatcher.InvokeAsync((Action)(() =>
					{
						DisposeWPFControls();
					}));
				}
			}
        }

		protected void longClick(object sender, RoutedEventArgs e)
		{
            goLong = true;
            goShort = false;
			gomomLong = false;
			gomomShort = false;
            Draw.TextFixed(this, "infobox", "Go Long Clicked", TextPosition.TopRight, Brushes.Green, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
			
		}


		protected void shortClick(object sender, RoutedEventArgs e)
		{
			goShort = true;
            goLong = false;
			gomomLong = false;
			gomomShort = false;
			Draw.TextFixed(this, "infobox", "Go Short Clicked", TextPosition.TopRight, Brushes.Red, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
		}

		protected void flatClick(object sender, RoutedEventArgs e)
		{
        	AtmStrategyClose(atmStrategyId);
			goLong = false;
			goShort = false;
			gomomLong = false;
			gomomShort = false;
			Draw.TextFixed(this, "infobox", "Controller Flat", TextPosition.TopRight, Brushes.Black, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);	
		}

		//NEW
		protected void cancelClick(object sender, RoutedEventArgs e)
		{
//        	AtmStrategyClose(atmStrategyId);
			goLong = false;
			goShort = false;
			gomomLong = false;
			gomomShort = false;
			Draw.TextFixed(this, "infobox", "Controller Flat", TextPosition.TopRight, Brushes.Black, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);	
		}
		
		protected void momlongClick(object sender, RoutedEventArgs e)
		{
			gomomLong = true;
			goLong = false;
			goShort = false;
			gomomShort = false;
			Draw.TextFixed(this, "infobox", "Momentum Long Clicked", TextPosition.TopRight, Brushes.LimeGreen, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
			
		}
		
		protected void momshortClick(object sender, RoutedEventArgs e)
		{
			gomomShort = true;
			gomomLong = false;
			goLong = false;
			goShort = false;
			Draw.TextFixed(this, "infobox", "Momentum Short Clicked", TextPosition.TopRight, Brushes.Tomato, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
		}
		
		protected void CreateWPFControls()
		{
			chartWindow				= System.Windows.Window.GetWindow(ChartControl.Parent) as Chart;
		
			chartTraderGrid			= (chartWindow.FindFirst("ChartWindowChartTraderControl") as ChartTrader).Content as System.Windows.Controls.Grid;
			
			chartTraderButtonsGrid	= chartTraderGrid.Children[0] as System.Windows.Controls.Grid;

			lowerButtonsGrid = new System.Windows.Controls.Grid();
			System.Windows.Controls.Grid.SetColumnSpan(lowerButtonsGrid, 3);

			lowerButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(206) });
			
            evenlowerButtonsGrid = new System.Windows.Controls.Grid();
			System.Windows.Controls.Grid.SetColumnSpan(evenlowerButtonsGrid, 3);

			evenlowerButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(102) });
			evenlowerButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(2) });
			evenlowerButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(102) });
          
			//NEW
			lowestButtonsGrid = new System.Windows.Controls.Grid();
			System.Windows.Controls.Grid.SetColumnSpan(lowestButtonsGrid, 3);

			lowestButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(102) });
			lowestButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(2) });
			lowestButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(102) });
			
			cancelButtonsGrid = new System.Windows.Controls.Grid();
			System.Windows.Controls.Grid.SetColumnSpan(cancelButtonsGrid, 3);

			cancelButtonsGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(206) });
			//NEW
			flatButton = new System.Windows.Controls.Button()
			{
				Background		= Brushes.DimGray,
				BorderBrush		= Brushes.Black,
				Content			= "FLATTEN EVERYTHING",
				Height			= 45
			};
			flatButton.Click += flatClick;
			
			lowerButtonsGrid.Children.Add(flatButton);
			System.Windows.Controls.Grid.SetColumn(flatButton, 2);

            longButton = new System.Windows.Controls.Button()
			{
				Background		= Brushes.Green,
				BorderBrush		= Brushes.Black,
				Content			= "EOB",
				Height			= 30
			};
			longButton.Click += longClick;
			
			evenlowerButtonsGrid.Children.Add(longButton);
			System.Windows.Controls.Grid.SetColumn(longButton, 0);
			
			shortButton = new System.Windows.Controls.Button()
			{
				Background		= Brushes.Red,
				BorderBrush		= Brushes.Black,
				Content			= "EOB",
				Height			= 30
			};
			shortButton.Click += shortClick;
			
			evenlowerButtonsGrid.Children.Add(shortButton);
			System.Windows.Controls.Grid.SetColumn(shortButton, 2);
			//NEW
			momlongButton = new System.Windows.Controls.Button()
			{
				Background		= Brushes.Green,
				BorderBrush		= Brushes.Black,
				Content			= "MOMENTUM",
				Height			= 30
			};
			momlongButton.Click += momlongClick;
			
			lowestButtonsGrid.Children.Add(momlongButton);
			System.Windows.Controls.Grid.SetColumn(momlongButton, 0);
			
			momshortButton = new System.Windows.Controls.Button()
			{
				Background		= Brushes.Red,
				BorderBrush		= Brushes.Black,
				Content			= "MOMENTUM",
				Height			= 30
			};
			momshortButton.Click += momshortClick;
			
			lowestButtonsGrid.Children.Add(momshortButton);
			System.Windows.Controls.Grid.SetColumn(momshortButton, 2);
			
			cancelButton = new System.Windows.Controls.Button()
			{
				Background		= Brushes.DimGray,
				BorderBrush		= Brushes.Black,
				Content			= "CANCEL",
				Height			= 45
			};
			cancelButton.Click += cancelClick;
			
			cancelButtonsGrid.Children.Add(cancelButton);
			System.Windows.Controls.Grid.SetColumn(cancelButton, 2);
			
			//NEW
			if (TabSelected())
				InsertWPFControls();

			chartWindow.MainTabControl.SelectionChanged += TabChangedHandler;
		}

		public void DisposeWPFControls()
		{
			if (longButton != null)
				longButton.Click -= longClick;
				
			if (shortButton != null)
				shortButton.Click -= shortClick;
				
			if (flatButton != null)
				flatButton.Click -= flatClick;
			//NEW
			if (momlongButton != null)
				momlongButton.Click -= momlongClick;
				
			if (momshortButton != null)
				momshortButton.Click -= momshortClick;
				
			if (cancelButton != null)
				cancelButton.Click -= cancelClick;
			RemoveWPFControls();
		}
		private System.Windows.Controls.RowDefinition topShelf, bottomShelf, cancelShelf, momentumShelf;
		public void InsertWPFControls()
		{
			if (panelActive)
				return;
                
			topShelf = new System.Windows.Controls.RowDefinition() { Height = new GridLength(61) };
			chartTraderGrid.RowDefinitions.Add(topShelf);
			System.Windows.Controls.Grid.SetRow(lowerButtonsGrid, (chartTraderGrid.RowDefinitions.Count - 1));
			chartTraderGrid.Children.Add(lowerButtonsGrid);
            
            bottomShelf = new System.Windows.Controls.RowDefinition() { Height = new GridLength(31) };
			chartTraderGrid.RowDefinitions.Add(bottomShelf);
			System.Windows.Controls.Grid.SetRow(evenlowerButtonsGrid, (chartTraderGrid.RowDefinitions.Count - 1));
			chartTraderGrid.Children.Add(evenlowerButtonsGrid);
			//NEW
			cancelShelf = new System.Windows.Controls.RowDefinition() { Height = new GridLength(61) };
			chartTraderGrid.RowDefinitions.Add(cancelShelf);
			System.Windows.Controls.Grid.SetRow(cancelButtonsGrid, (chartTraderGrid.RowDefinitions.Count - 1));
			chartTraderGrid.Children.Add(cancelButtonsGrid);
            
            momentumShelf = new System.Windows.Controls.RowDefinition() { Height = new GridLength(31) };
			chartTraderGrid.RowDefinitions.Add(momentumShelf);
			System.Windows.Controls.Grid.SetRow(lowestButtonsGrid, (chartTraderGrid.RowDefinitions.Count - 1));
			chartTraderGrid.Children.Add(lowestButtonsGrid);
			
			panelActive = true;
		}

		protected override void OnBarUpdate() 
        {
			// Make sure this strategy does not execute against historical data
			if(State == State.Historical)
				return;
			//Variable Testing
//			if (goLong)
//			{
//				Print(@"goLong = True");
//			}
			
//			if (!goLong)
//			{
//				Print(@"goLong = False");
//			}
			
            if (goLong && orderId.Length == 0 && atmStrategyId.Length == 0 && Close[1] > Open[1])  //&& Close[2] < Open[2] 
            {
//				Print(@"Entry LONG Criteria Valid");
				isAtmStrategyCreated = false;  // reset atm strategy created check to false
				atmStrategyId = GetAtmStrategyUniqueId();
				orderId = GetAtmStrategyUniqueId();
//				AtmStrategyCreate(OrderAction.Buy, OrderType.Market,0, 0, TimeInForce.Day, orderId, "Controller ATM", atmStrategyId, (atmCallbackErrorCode, atmCallBackId) => {
				AtmStrategyCreate(OrderAction.Buy, OrderType.Limit,Close[1], 0, TimeInForce.Day, orderId, "Controller ATM", atmStrategyId, (atmCallbackErrorCode, atmCallBackId) => {
					//check that the atm strategy create did not result in error, and that the requested atm strategy matches the id in callback
					if (atmCallbackErrorCode == ErrorCode.NoError && atmCallBackId == atmStrategyId)
						isAtmStrategyCreated = true;
				});
				Draw.TextFixed(this, "infobox", "Long Position Held", TextPosition.TopRight, Brushes.Green, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
                goLong = false;
            }

            if  (goShort && orderId.Length == 0 && atmStrategyId.Length == 0  && Close[1] < Open[1])  //&& Close[2] > Open[2]
	        {
//				Print(@"Entry Short Criteria Valid");
				isAtmStrategyCreated = false;  // reset atm strategy created check to false
				atmStrategyId = GetAtmStrategyUniqueId();
				orderId = GetAtmStrategyUniqueId();
				AtmStrategyCreate(OrderAction.Sell, OrderType.Limit, Close[1], 0, TimeInForce.Day, orderId, "Controller ATM", atmStrategyId, (atmCallbackErrorCode, atmCallBackId) => {
					//check that the atm strategy create did not result in error, and that the requested atm strategy matches the id in callback
					if (atmCallbackErrorCode == ErrorCode.NoError && atmCallBackId == atmStrategyId)
						isAtmStrategyCreated = true;
				});
            	Draw.TextFixed(this, "infobox", "go short Clicked", TextPosition.TopRight, Brushes.Red, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
            	goShort = false;
            }
			
			//NEW
			if (gomomLong && orderId.Length == 0 && atmStrategyId.Length == 0 && Close[2] > Open[2] && Close[1] > Open[1])  
            {
//				Print(@"Entry LONG Criteria Valid");
				isAtmStrategyCreated = false;  // reset atm strategy created check to false
				atmStrategyId = GetAtmStrategyUniqueId();
				orderId = GetAtmStrategyUniqueId();
//				AtmStrategyCreate(OrderAction.Buy, OrderType.Market,0, 0, TimeInForce.Day, orderId, "Controller ATM", atmStrategyId, (atmCallbackErrorCode, atmCallBackId) => {
				AtmStrategyCreate(OrderAction.Buy, OrderType.Limit,Close[1], 0, TimeInForce.Day, orderId, "Controller ATM", atmStrategyId, (atmCallbackErrorCode, atmCallBackId) => {
					//check that the atm strategy create did not result in error, and that the requested atm strategy matches the id in callback
					if (atmCallbackErrorCode == ErrorCode.NoError && atmCallBackId == atmStrategyId)
						isAtmStrategyCreated = true;
				});
				Draw.TextFixed(this, "infobox", "Momentum Long Clicked", TextPosition.TopRight, Brushes.Green, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
                gomomLong = false;
			//NEW	
            	}
			
			if  (gomomShort && orderId.Length == 0 && atmStrategyId.Length == 0  && Close[2] < Open[2] && Close[1] < Open[1])  //&& Close[2] > Open[2]
	        {
//				Print(@"Entry Short Criteria Valid");
				isAtmStrategyCreated = false;  // reset atm strategy created check to false
				atmStrategyId = GetAtmStrategyUniqueId();
				orderId = GetAtmStrategyUniqueId();
				AtmStrategyCreate(OrderAction.Sell, OrderType.Limit, Close[1], 0, TimeInForce.Day, orderId, "Controller ATM", atmStrategyId, (atmCallbackErrorCode, atmCallBackId) => {
					//check that the atm strategy create did not result in error, and that the requested atm strategy matches the id in callback
					if (atmCallbackErrorCode == ErrorCode.NoError && atmCallBackId == atmStrategyId)
						isAtmStrategyCreated = true;
				});
            	Draw.TextFixed(this, "infobox", "Momentum Short Clicked", TextPosition.TopRight, Brushes.Red, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);
            	goShort = false;
            }
			if ((Position.MarketPosition == MarketPosition.Flat) && !goLong && !goShort)
			{
				Draw.TextFixed(this, "infobox", "Controller Flat", TextPosition.TopRight, Brushes.Black, new Gui.Tools.SimpleFont("Arial", 25), Brushes.Transparent, Brushes.Transparent, 100);	
			}
			
			
			// Check that atm strategy was created before checking other properties
			if (!isAtmStrategyCreated)
				return;

			// Check for a pending entry order
			if (orderId.Length > 0)
			{
			
				string[] status = GetAtmStrategyEntryOrderStatus(orderId);

				// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
				if (status.GetLength(0) > 0)
				{
					// Print out some information about the order to the output window
//					Print("The entry order average fill price is: " + status[0]);
//					Print("The entry order filled amount is: " + status[1]);
//					Print("The entry order order state is: " + status[2]);

					// If the order state is terminal, reset the order id value
					if (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected")
						orderId = string.Empty;
				}
			} // If the strategy has terminated reset the strategy id
			else if (atmStrategyId.Length > 0 && GetAtmStrategyMarketPosition(atmStrategyId) == Cbi.MarketPosition.Flat)
				atmStrategyId = string.Empty;

//			if (atmStrategyId.Length > 0)
//			{
//				// You can change the stop price
//				if (GetAtmStrategyMarketPosition(atmStrategyId) != MarketPosition.Flat)
//					AtmStrategyChangeStopTarget(0, Low[0] - 3 * TickSize, "STOP1", atmStrategyId);

				// Print some information about the strategy to the output window, please note you access the ATM strategy specific position object here
				// the ATM would run self contained and would not have an impact on your NinjaScript strategy position and PnL
//				Print("The current ATM Strategy market position is: " + GetAtmStrategyMarketPosition(atmStrategyId));
//				Print("The current ATM Strategy position quantity is: " + GetAtmStrategyPositionQuantity(atmStrategyId));
//				Print("The current ATM Strategy average price is: " + GetAtmStrategyPositionAveragePrice(atmStrategyId));
//				Print("The current ATM Strategy Unrealized PnL is: " + GetAtmStrategyUnrealizedProfitLoss(atmStrategyId));
//			}
			
			
			
        }

		protected void RemoveWPFControls()
		{
			if (!panelActive)
				return;
			
			if (chartTraderButtonsGrid != null || lowerButtonsGrid != null)
			{
				chartTraderGrid.Children.Remove(lowerButtonsGrid);
				chartTraderGrid.RowDefinitions.Remove(topShelf);
			}

            if (chartTraderButtonsGrid != null || evenlowerButtonsGrid != null)
			{
				chartTraderGrid.Children.Remove(evenlowerButtonsGrid);
				chartTraderGrid.RowDefinitions.Remove(bottomShelf);
			}
			//NEW
			if (chartTraderButtonsGrid != null || cancelButtonsGrid != null)
			{
				chartTraderGrid.Children.Remove(cancelButtonsGrid);
				chartTraderGrid.RowDefinitions.Remove(cancelShelf);
			}

            if (chartTraderButtonsGrid != null || lowestButtonsGrid != null)
			{
				chartTraderGrid.Children.Remove(lowestButtonsGrid);
				chartTraderGrid.RowDefinitions.Remove(momentumShelf);
			}
			
			panelActive = false;
		}

		private bool TabSelected()
		{
			bool tabSelected = false;

			// loop through each tab and see if the tab this indicator is added to is the selected item
			foreach (System.Windows.Controls.TabItem tab in chartWindow.MainTabControl.Items)
				if ((tab.Content as ChartTab).ChartControl == ChartControl && tab == chartWindow.MainTabControl.SelectedItem)
					tabSelected = true;

			return tabSelected;
		}

		private void TabChangedHandler(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count <= 0)
				return;

			tabItem = e.AddedItems[0] as System.Windows.Controls.TabItem;
			if (tabItem == null)
				return;

			chartTab = tabItem.Content as NinjaTrader.Gui.Chart.ChartTab;
			if (chartTab == null)
				return;

			if (TabSelected())
				InsertWPFControls();
			else
				RemoveWPFControls();
		}
		

		
		
	}
}


//Version 6.7.2 Change Log
//Added flat infobox
//added previous candle close for entry ====> WORKING!!!
//Needs switch for for trade direction change, Old direction staying valid!
//Capital letters for button text
//Version 6.7.2 Complete!!

