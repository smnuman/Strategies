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
namespace NinjaTrader.NinjaScript.Strategies
{
	public class SampleAtmReversal : Strategy
	{	
		// Modifications: 04-18-2019, Change single bool from isAtmStrategyCreated to isLongAtmStrategyCreated and isShortAtmStrategyCreated.  Changed code to keep the bools sperate.
		// Reason for the modification was to prevent having multiple orders at once.
		
		#region Variables		
        private string	longAtmId				= string.Empty; // Atm Id for long.
		private string	longOrderId				= string.Empty; // Order Id for long.
		private string	shortAtmId				= string.Empty; // Atm Id for short.
		private string	shortOrderId			= string.Empty; // Order Id for short.
		private bool 	isLongAtmStrategyCreated 	= false;
		private bool	isShortAtmStrategyCreated	= false;
        #endregion
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "SampleAtmReversal";
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
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
			}
		}

		protected override void OnBarUpdate()
		{
			// HELP DOCUMENTATION REFERENCE: Please see the Help Guide section "Using ATM Strategies"

			// Make sure this strategy does not execute against historical data
			if(State == State.Historical)
				return;			
			
			// Check any pending long or short orders by their Order Id and if the ATM has terminated.
			// Check for a pending long order.
			if (longOrderId.Length > 0)
			{
				// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements.
				string[] status = GetAtmStrategyEntryOrderStatus(longOrderId);
				if (status.GetLength(0) > 0)
				{
					// If the order state is terminal, reset the order id value.
					if (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected")
						longOrderId = string.Empty;
				}
			} // If the strategy has terminated reset the strategy id.
			else if (longAtmId.Length > 0 && GetAtmStrategyMarketPosition(longAtmId) == Cbi.MarketPosition.Flat)
			{
				longAtmId = string.Empty;
				isLongAtmStrategyCreated = false;
			}
			
			// Check for a pending short order.
			if (shortOrderId.Length > 0)
			{
				// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements.
				string[] status = GetAtmStrategyEntryOrderStatus(shortOrderId);
				if (status.GetLength(0) > 0)
				{
					// If the order state is terminal, reset the order id value.
					if (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected")
						shortOrderId = string.Empty;
				}
			} // If the strategy has terminated reset the strategy id.
			else if (shortAtmId.Length > 0 && GetAtmStrategyMarketPosition(shortAtmId) == Cbi.MarketPosition.Flat)
			{
				shortAtmId = string.Empty;
				isShortAtmStrategyCreated = false;
			}
			// End check.
			
			// Entries.
			// **** YOU MUST HAVE AN ATM STRATEGY TEMPLATE NAMED 'AtmStrategyTemplate' CREATED IN NINJATRADER (SUPERDOM FOR EXAMPLE) FOR THIS TO WORK ****
			// Enter long if Close is greater than Open.
			if(Close[0] > Open[0])
			{
			//	Print("Long condition at : "+Time[0]);
				// If there is a short ATM Strategy running close it.
				if(shortAtmId.Length != 0 && isShortAtmStrategyCreated)
				{
					AtmStrategyClose(shortAtmId);
					isShortAtmStrategyCreated = false;
				}
				// Ensure no other long ATM Strategy is running.
				if(longOrderId.Length == 0 && longAtmId.Length == 0 && !isLongAtmStrategyCreated)
				{
					longOrderId = GetAtmStrategyUniqueId();
					longAtmId = GetAtmStrategyUniqueId();
					AtmStrategyCreate(OrderAction.Buy, OrderType.Market, 0, 0, TimeInForce.Day, longOrderId, "AtmStrategyTemplate", longAtmId, (atmCallbackErrorCode, atmCallBackId) => { 
						//check that the atm strategy create did not result in error, and that the requested atm strategy matches the id in callback
						if (atmCallbackErrorCode == ErrorCode.NoError && atmCallBackId == longAtmId) 
							isLongAtmStrategyCreated = true;
					});
				}
			}
			
			// Enter short if Close is less than Open.
			if(Close[0] < Open[0])
			{
			//	Print("Short condition at " + Time[0]);
				// If there is a long ATM Strategy running close it.
				if(longAtmId.Length != 0  && isLongAtmStrategyCreated)
				{
					AtmStrategyClose(longAtmId);
					isLongAtmStrategyCreated = false;
				}
				// Ensure no other short ATM Strategy is running.
				if(shortOrderId.Length == 0 && shortAtmId.Length == 0  && !isShortAtmStrategyCreated)
				{
					shortOrderId = GetAtmStrategyUniqueId();
					shortAtmId = GetAtmStrategyUniqueId();
					AtmStrategyCreate(OrderAction.SellShort, OrderType.Market, 0, 0, TimeInForce.Day, shortOrderId, "AtmStrategyTemplate", shortAtmId, (atmCallbackErrorCode, atmCallBackId) => { 
						//check that the atm strategy create did not result in error, and that the requested atm strategy matches the id in callback
						if (atmCallbackErrorCode == ErrorCode.NoError && atmCallBackId == shortAtmId) 
							isShortAtmStrategyCreated = true;
					});
				}
			}
			// End entries.
		}
	}
}
