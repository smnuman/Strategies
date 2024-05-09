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
	public class DailyLossLimitExample : Strategy
	{
		private double currentPnL;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Prevents new entries after PnL is less than the LossLimit";
				Name										= "DailyLossLimitExample";
				Calculate									= Calculate.OnBarClose;
				BarsRequiredToTrade							= 1;

				LossLimit									= 500;
			}
			else if (State == State.DataLoaded)
			{
				ClearOutputWindow();
				SetStopLoss("long1", CalculationMode.Ticks, 5, false);
			}
		}

		protected override void OnBarUpdate()
		{
			// at the start of a new session, reset the currentPnL for a new day of trading
			if (Bars.IsFirstBarOfSession)
				currentPnL = 0;

			// if flat and below the loss limit of the day enter long
			if (Position.MarketPosition == MarketPosition.Flat && currentPnL > -LossLimit)
			{
				EnterLong(DefaultQuantity, "long1");
			}

			// if in a position and the realized day's PnL plus the position PnL is greater than the loss limit then exit the order
			if (Position.MarketPosition == MarketPosition.Long
					&& (currentPnL + Position.GetUnrealizedProfitLoss(PerformanceUnit.Currency, Close[0])) <= -LossLimit)
			{
				//Print((currentPnL+Position.GetProfitLoss(Close[0], PerformanceUnit.Currency)) + " - " + -LossLimit);
				// print to the output window if the daily limit is hit in the middle of a trade
				Print("daily limit hit, exiting order " + Time[0].ToString());
				ExitLong("Daily Limit Exit", "long1");
			}
		}

		protected override void OnPositionUpdate(Position position, double averagePrice, int quantity, MarketPosition marketPosition)
		{
			if (Position.MarketPosition == MarketPosition.Flat && SystemPerformance.AllTrades.Count > 0)
			{
				// when a position is closed, add the last trade's Profit to the currentPnL
				currentPnL += SystemPerformance.AllTrades[SystemPerformance.AllTrades.Count - 1].ProfitCurrency;

				// print to output window if the daily limit is hit
				if (currentPnL <= -LossLimit)
				{
					Print("daily limit hit, no new orders" + Time[0].ToString());
				}
			}
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(0, double.MaxValue)]
		[Display(ResourceType = typeof(Custom.Resource), Name="LossLimit", Description="Amount of dollars of acceptable loss", Order=1, GroupName="NinjaScriptStrategyParameters")]
		public double LossLimit
		{ get; set; }
		#endregion

	}
}
