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
namespace NinjaTrader.NinjaScript.Strategies.NMNStrategies
{
	public class DEMASMACrossOver : Strategy
	{
		private bool TradeOn;	/// Toggles trading feature : allows when holds true
		private bool deBug;

		private DEMA DEMA14;	/// Going to use  DEMA for 14 period
		private SMA SMA10;		/// Going to use  SMA for 10 period

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "DEMASMACrossOver";
				Calculate									= Calculate.OnBarClose;
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
				
				lookBackPeriod			= 1;
				
				TradeOn					= false;
				deBug					= true;		/// Manual tweek!!
			}
			else if (State == State.Configure)
			{
				ClearOutputWindow();
			}
			else if (State == State.DataLoaded)
			{				
				DEMA14				= DEMA(Close, 14);
				SMA10				= SMA(Close, 10);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < BarsRequiredToTrade)
				return;
			
			if (!IsFirstTickOfBar)
				return;				// reduces number of output for now!
			
			#region | Trade setups
			/// Check market conditions
			bool marketConditionOK = (Position.MarketPosition == MarketPosition.Flat)	// No trade is on
										&& (TradeOn == false)							// No trade is allowed yet
										// At least 1 bar is passed after the last trade exit OR no trade was done ever!
										&& (BarsSinceExitExecution() > 1 || BarsSinceExitExecution() == -1) 
				 						;
			/// Check market exit conditions
			bool marketExitConditionOK = (Position.MarketPosition != MarketPosition.Flat) ;	// not used yet !!
			
			/// Check Entry setups 
			bool longSetup 	= (CrossAbove(DEMA14, SMA10, lookBackPeriod));
			bool shortSetUp = (CrossBelow(DEMA14, SMA10, lookBackPeriod));
			#endregion
			
			#region | debug Prints
			if (deBug)
			{
				Print(string.Format("\n{0} | Market condition (Position.MarketPosition == MarketPosition.Flat) is = {1}",Time[0], (Position.MarketPosition == MarketPosition.Flat)));
				Print(string.Format("_____________________ | Position.MarketPosition <{0}> == MarketPosition.Flat <{1}>", Position.MarketPosition, MarketPosition.Flat ));
				Print(string.Format("_____________________ | TradeOn = <{0}> i.e., (TradeOn == false) is {1}", (TradeOn), (TradeOn == false) ));
				Print(string.Format("_____________________ | BarsSinceExitExecution() <{0}>", BarsSinceExitExecution() ));
				Print(string.Format("\n_____________________ | longSetup <{0}>", (CrossAbove(DEMA14, SMA10, lookBackPeriod)) ));
				Print(string.Format("_____________________ | shortSetUp <{0}>", (CrossBelow(DEMA14, SMA10, lookBackPeriod)) ));
			}
			else Print("Strategy Debugging is halted manually");
			#endregion
			
			#region | Exit Trades
			if ( (Position.MarketPosition == MarketPosition.Long) && (shortSetUp) )
			{
				ExitLong();
				TradeOn = true;
				Print(string.Format("\n_____________________ | EXIT Long  = <{0}>", (shortSetUp) ));
			}
			if ( (Position.MarketPosition == MarketPosition.Short) && (longSetup) )
			{
				ExitShort();
				TradeOn = true;
				Print(string.Format("\n_____________________ | EXIT Short = <{0}>", (longSetup) ));
			}
			#endregion
						
			#region | Trade entries
			// Set 1			
			if (marketConditionOK) TradeOn = true; // Allowing to trade from now on
			
			 // Set 2
			if (TradeOn && longSetup )
			{
				EnterLong(Convert.ToInt32(DefaultQuantity), "");
				TradeOn = false; // revoking trade allowance after entry
				Print(string.Format("\n_____________________ | Entering Long <{0}>", longSetup ));
			}
			
			 // Set 3
			if (TradeOn && shortSetUp )
			{
				EnterShort(Convert.ToInt32(DefaultQuantity), "");
				TradeOn = false; // revoking trade allowance after entry
				Print(string.Format("\n_____________________ | Entering Short <{0}>", shortSetUp ));
			}
			#endregion			

			#region | Trade report creation - Not complete yet !! 
			/// Numan :: Copied from: https://ninjatrader.com/support/helpGuides/nt8/trade.htm
			if (SystemPerformance.RealTimeTrades.Count > 0)
			{
				// Check to make sure there is at least one trade in the collection
				Trade lastTrade = SystemPerformance.RealTimeTrades[SystemPerformance.RealTimeTrades.Count - 1];
				
				// Calculate the PnL for the last completed real-time trade
				double lastProfitCurrency = lastTrade.ProfitCurrency;
				
				// Store the quantity of the last completed real-time trade
				double lastTradeQty = lastTrade.Quantity;
				
				// Pring the PnL to the NinjaScript Output window
				Print("The last trade's profit in currency is " + lastProfitCurrency);
				// The trade profit is quantity aware, we can easily print the profit per traded unit as well
				Print("The last trade's profit in currency per traded unit is " + (lastProfitCurrency / lastTradeQty));
			}
			#endregion
		}
		
		
		#region Properties
		[Range(0, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Lookback period", GroupName = "NinjaScriptStrategyParameters", Order = 0)]
		public int lookBackPeriod
		{ get; set; }
		#endregion
	}
}
