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
	public class ReversalStrategy : Strategy
	{
		private NinjaTrader.NinjaScript.Indicators.TradeSaber.ReversalTS ReversalBar;
		private NinjaTrader.NinjaScript.Indicators.TradeSaber.EngulfingBarTS EngulfingBar;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "ReversalStrategy";
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
				TraceOrders									= true;
				RealtimeErrorHandling						= RealtimeErrorHandling.IgnoreAllErrors;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				Quantity					= 1;
				StopTicks					= 20;
				TargetTicks					= 40;
				QtyAdd						= 1;
				OffsetTicks					= 3;
				Backtest					= false;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				ReversalBar				= ReversalTS(Close, 0, 0, 1, true, Brushes.MediumTurquoise, Brushes.MediumPurple, true, Brushes.Green, true);
				EngulfingBar			= EngulfingBarTS(Close, 0, 0, 0, false, false, Brushes.MediumTurquoise, Brushes.MediumPurple, true, Brushes.LightBlue, Brushes.Plum, false, Brushes.Green, true, Brushes.Green, true, @"https://youtu.be/IqMJCp8N0-4", @"https://discord.gg/2YU9GDme8j", @"https://tradesaber.com/", @"TradeSaber(Dre)", @"Version 2.0 // June 2023");
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
				return;
			
			if ( !Backtest && (State != State.Realtime) )
				return;
			
			if (BarsSinceEntryExecution() >= 80)
			{
				ExitLong();
				ExitShort();
			}

			 // Set 1 : Long Entry
			if ((ReversalBar[1] == 1))
			{
//				ExitShort(Convert.ToInt32(Quantity), @"Stop", @"Short");
				EnterLongLimit(Convert.ToInt32(Quantity), (Close[1] + (OffsetTicks * TickSize)) , @"Long");
			}
			
			 // Set 2 : Short entry
			if (ReversalBar[1] == -1)
			{
//				ExitLong(Convert.ToInt32(Quantity), @"Stop", @"Long");
				EnterShortLimit(Convert.ToInt32(Quantity), (Close[1] - (OffsetTicks * TickSize)) , @"Short");
			}
			
//			 // Set 3 : Additional Trade  -- Long
//			if ((EngulfingBar.ContiOutsideBar[0] == 1)
//				 && (QtyAdd > 0)
//				 && (Position.MarketPosition == MarketPosition.Long)
//				)
//			{
//				EnterLong(Convert.ToInt32(QtyAdd), @"Long");
//			}
			
//			 // Set 4 : Long Entry on Conti Bar
//			if ((EngulfingBar.ContiOutsideBar[0] == 1)
//				 && (QtyAdd == 0)
//				 && (Position.MarketPosition != MarketPosition.Long)
//				)
//			{
//				EnterLongLimit(Convert.ToInt32(Quantity), (GetCurrentAsk(0) + (OffsetTicks * TickSize)) , @"Long");
//			}
			
//			 // Set 5 : Additional Trade -- Short
//			if ((EngulfingBar.ContiOutsideBar[0] == -1)
//				 && (QtyAdd > 0)
//				 && (Position.MarketPosition == MarketPosition.Short)
//				)
//			{
//				EnterShort(Convert.ToInt32(QtyAdd), @"Short");
//			}
			
//			 // Set 6 : Short Entry on Conti Bar
//			if ((EngulfingBar.ContiOutsideBar[0] == -1)
//				 && (QtyAdd == 0)
//				 && (Position.MarketPosition != MarketPosition.Short)
//				)
//			{
//				EnterShortLimit(Convert.ToInt32(Quantity), (GetCurrentBid(0) + (OffsetTicks * TickSize)) , @"Short");
//			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Quantity", Order=1, GroupName="Trade")]
		public int Quantity
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StopTicks", Order=2, GroupName="Trade")]
		public int StopTicks
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="TargetTicks", Order=3, GroupName="Trade")]
		public int TargetTicks
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="QtyAdd", Order=4, GroupName="Trade")]
		public int QtyAdd
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="OffsetTicks", Order=5, GroupName="Trade")]
		public int OffsetTicks
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Want to Backtest", Order=6, GroupName="Backtest")]
		public bool Backtest
		{ get; set; }
		#endregion

	}
}
