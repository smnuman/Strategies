//
// Copyright (C) 2021, NinjaTrader LLC <www.ninjatrader.com>
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component
// Coded by NinjaTrader_ChelseaB
//
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

namespace NinjaTrader.NinjaScript.Strategies.OrderManagementExamples
{
	public class UnmanagedOCOBracketExample : Strategy
	{
		private bool				exitOnCloseWait;
		private Order				longStopEntry, shortStopEntry;
		private string				ocoString;
		private SessionIterator		sessionIterator;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description								= @"Demonstrates placing two opposing entry orders simultaneously linked with OCO, to bracket the market. While unmanaged orders can be submitted historically, this demonstration is designed to work in real-time.";
				Name									= "UnmanagedOCOBracketExample";
				Calculate								= Calculate.OnPriceChange;
				IsExitOnSessionCloseStrategy			= true;
				ExitOnSessionCloseSeconds				= 30;
				IsUnmanaged								= true;
			}
			else if (State == State.Historical)
			{
				sessionIterator		= new SessionIterator(Bars);
				exitOnCloseWait		= false;
			}
			else if (State == State.Realtime)
			{
				// this needs to be run at least once before orders start getting placed.
				// I could do this when CurrentBar is 0 in OnBarUpdate,
				// but since this script only runs in real-time, I can trigger it once as the script transistions to real-time
				sessionIterator.GetNextSession(Time[0], true);
			}
		}

		protected override void OnBarUpdate() { }

		private void AssignOrderToVariable(ref Order order)
		{
			// Assign Order variable from OnOrderUpdate() to ensure the assignment occurs when expected.
			// This is more reliable than assigning the return Order object from the submission method as the assignment is not guaranteed to be complete if it is referenced immediately after submitting
			if (order.Name == "longStopEntry" && longStopEntry != order)
				longStopEntry = order;

			if (order.Name == "shortStopEntry" && shortStopEntry != order)
				shortStopEntry = order;
		}

		// prevents entry orders after the exit on close until the start of the new session
		private bool ExitOnCloseWait(DateTime tickTime)
		{
			// the sessionIterator only needs to be updated when the session changes (after its first update)
			if (Bars.IsFirstBarOfSession)
				sessionIterator.GetNextSession(Time[0], true);

			// if after the exit on close, prevent new orders until the new session
			if (tickTime >= sessionIterator.ActualSessionEnd.AddSeconds(-ExitOnSessionCloseSeconds) && tickTime <= sessionIterator.ActualSessionEnd)
				exitOnCloseWait = true;

			// an exit on close occurred in the previous session, reset for a new entry on the first bar of a new session
			if (exitOnCloseWait && Bars.IsFirstBarOfSession)
				exitOnCloseWait = false;

			return exitOnCloseWait;
		}

		protected override void OnExecutionUpdate(Cbi.Execution execution, string executionId, double price, int quantity,
			Cbi.MarketPosition marketPosition, string orderId, DateTime time)
		{
			// if the long entry filled, place a profit target and stop loss to protect the order
			if (longStopEntry != null && execution.Order == longStopEntry)
			{
				// generate a new oco string for the protective stop and target
				ocoString = string.Format("unmanageexitdoco{0}", DateTime.Now.ToString("hhmmssffff"));
				// submit a protective profit target order
				SubmitOrderUnmanaged(0, OrderAction.Sell, OrderType.Limit, 1, (High[0] + 20 * TickSize), 0, ocoString, "longProfitTarget");
				// submit a protective stop loss order
				SubmitOrderUnmanaged(0, OrderAction.Sell, OrderType.StopMarket, 1, 0, (Low[0] - 10 * TickSize), ocoString, "longStopLoss");
			}

			// reverse the order types and prices for a short
			else if (shortStopEntry != null && execution.Order == shortStopEntry)
			{
				ocoString = string.Format("unmanageexitdoco{0}", DateTime.Now.ToString("hhmmssffff"));
				SubmitOrderUnmanaged(0, OrderAction.BuyToCover, OrderType.Limit, 1, (Low[0] - 20 * TickSize), 0, ocoString, "shortProfitTarget");
				SubmitOrderUnmanaged(0, OrderAction.BuyToCover, OrderType.StopMarket, 1, 0, (High[0] + 10 * TickSize), ocoString, "shortStopLoss");
			}

			// I didn't use Order variables to track the stop loss and profit target, but I could have
			// Instead, I detect the orders when the fill by their signalName
			// (the execution.Name is the signalName provided with the order)

			// when the long profit or stop fills, set the long entry to null to allow a new entry
			else if (execution.Name == "longProfitTarget" || execution.Name == "longStopLoss" || execution.Name == "shortProfitTarget" || execution.Name == "shortStopLoss")
			{
				longStopEntry	= null;
				shortStopEntry	= null;
			}
		}

		protected override void OnMarketData(MarketDataEventArgs marketDataUpdate)
		{
			// only places orders in real time
			if (State != State.Realtime || ExitOnCloseWait(marketDataUpdate.Time))
				return;			
			
			// require both entry orders to be null to begin the entry bracket
			// entry orders are set to null if the entry is cancelled due to oco or when the exit order exits the trade
			// if the Order variables for the entries are null, no trade is in progress, place a new order in real time
			if (longStopEntry == null && shortStopEntry == null)
			{
				// generate a unique oco string based on the time
				// oco means that when one entry fills, the other entry is automatically cancelled
				// in OnExecution we will protect these orders with our version of a stop loss and profit target when one of the entry orders fills
				ocoString		= string.Format("unmanagedentryoco{0}", DateTime.Now.ToString("hhmmssffff"));
				longStopEntry	= SubmitOrderUnmanaged(0, OrderAction.Buy, OrderType.StopMarket, 1, 0, (High[0] + 15 * TickSize), ocoString, "longStopEntry");
				shortStopEntry	= SubmitOrderUnmanaged(0, OrderAction.SellShort, OrderType.StopMarket, 1, 0, (Low[0] - 15 * TickSize), ocoString, "shortStopEntry");
			}
		}

		protected override void OnOrderUpdate(Cbi.Order order, double limitPrice, double stopPrice,
			int quantity, int filled, double averageFillPrice,
			Cbi.OrderState orderState, DateTime time, Cbi.ErrorCode error, string comment)
		{
			AssignOrderToVariable(ref order);
			// when both orders are cancelled set to null for a new entry
			// if the exit on close fills, also reset for a new entry
			if ((longStopEntry != null && longStopEntry.OrderState == OrderState.Cancelled && shortStopEntry != null && shortStopEntry.OrderState == OrderState.Cancelled) || (order.Name == "Exit on session close" && order.OrderState == OrderState.Filled))
			{
				longStopEntry	= null;
				shortStopEntry	= null;
			}
		}
	}
}
