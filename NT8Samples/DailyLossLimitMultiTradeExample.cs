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
namespace NinjaTrader.NinjaScript.Strategies.NT8Samples
{
	public class DailyLossLimitMultiTradeExample : Strategy
	{
		private double		currentPnL;
		private Order[]		entryOrders;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "DailyLossLimitMultiTradeExample";
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
				BarsRequiredToTrade							= 1;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;

				LossLimit									= 500;
			}
			else if (State == State.DataLoaded)
			{
				ClearOutputWindow();
				entryOrders = new Order[3];
			}
		}

		protected override void OnBarUpdate()
		{
			if (Bars.IsFirstBarOfSession)
				currentPnL = 0;

			//Print(string.Format("{0} | currentPnL: {1}", Time[0], currentPnL));			
			// if flat and below the loss limit of the day enter long
			if (entryOrders[1] == null && currentPnL > -LossLimit)
			{
				SetStopLoss("long1", CalculationMode.Price, 1, false);
				entryOrders[1] = EnterLong(DefaultQuantity, "long1");
			}

			// you can make the entries individually or in a loop or any logic
			if (entryOrders[2] == null && currentPnL >= -LossLimit)
			{
				SetStopLoss("long1", CalculationMode.Price, 1, false);
				entryOrders[2] = EnterLong(DefaultQuantity, "long2");
			}

			// look to see if any orders are open
			bool allNull = true;
			for (int i = 0; i < entryOrders.Length; i++)
				if (entryOrders[i] != null)
					allNull = false;

			if (allNull == false)
				Print(string.Format("{0} | Unrealized: {1} | DailyLimit: {2}", Time[0], (currentPnL + Position.GetUnrealizedProfitLoss(PerformanceUnit.Currency, Close[0])), (-LossLimit)));

			// if in a position and the realized day's PnL plus the position PnL is greater than the loss limit then exit the order
			// for accuracy this could be done in a 1 tick series or in OnMarketData()
			if (allNull == false && (currentPnL + Position.GetUnrealizedProfitLoss(PerformanceUnit.Currency, Close[0])) <= (-LossLimit))
			{
				// print to the output window if the daily limit is hit in the middle of a trade
				Print("daily limit hit, exiting order " + Time[0].ToString());
				ExitLong("DailyLossLimit", "");
			}
		}

		protected override void OnPositionUpdate(Position position, double averagePrice, int quantity, MarketPosition marketPosition)
		{
			// loop through all entry orders
			for (int i = 0; i < entryOrders.Length; i++)
			{
				// if this order is not null and is filled
				if (entryOrders[i] != null && entryOrders[i].OrderState == OrderState.Filled)
				{
					// look through all all trades to see if this order matches of those trades entry orders
					foreach (Trade thisTrade in SystemPerformance.AllTrades)
					{
						// if found, collect the performance, set the IOrder to null and break the loop
						if (thisTrade.Entry.Order == entryOrders[i])
						{
							currentPnL += thisTrade.ProfitCurrency;
							Print(string.Format("{0} | tradePnL: {1} | currentPnL: {2}", entryOrders[i].Time, thisTrade.ProfitCurrency, currentPnL));
							entryOrders[i] = null;
							break;
						}
					}
				}
			}
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name="LossLimit", Description="Amount of dollars of acceptable loss", Order=1, GroupName="NinjaScriptStrategyParameters")]
		public int LossLimit
		{ get; set; }
		#endregion

	}
}
