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
namespace NinjaTrader.NinjaScript.Strategies.Numan
{
	public class SimpleWAEentryNewUnocked : Strategy
	{
		private NinjaTrader.NinjaScript.Indicators.Numan.WAE_Mod WAE;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "SimpleWAEentryNewUnocked";
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
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				
				Sensitivity					= 300;
				Quantity					= 1;
				Fixed_rr					= true;
				Risk					= 20;
				Reward					= 100;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				WAE				= WAE_Mod(Close, Convert.ToInt32(Sensitivity), 10, true, 9, 30, true, 9, 30, 2, 200);
				if (Fixed_rr)
				{
					SetStopLoss("", CalculationMode.Ticks, Risk, false);
					SetProfitTarget("", CalculationMode.Ticks, Reward);					
				}

			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < BarsRequiredToTrade)
				return;

			 // Set 1 : Enter Long trade
			if ((Position.MarketPosition == MarketPosition.Flat)
				 && (CrossAbove(WAE.TrendUp, WAE.ExplosionLine, 1)))
			{
				EnterLongLimit(Convert.ToInt32(Quantity), GetCurrentBid());
			}
			
			 // Set 2 : Enter Short trade
			if ((Position.MarketPosition == MarketPosition.Flat)
				 && (CrossBelow(WAE.TrendDown, WAE.ExplosionLineDn, 1)))
			{
				EnterShortLimit(Convert.ToInt32(Quantity), GetCurrentAsk());
			}
			
			 // Set 3 : Long Trend reversed -> Reverse
			if ((Position.MarketPosition == MarketPosition.Long)
				 && (WAE.TrendUp[0] <= 0))
			{	// Entry() methods will reverse the position automatically
				EnterShortLimit(Convert.ToInt32(Quantity), GetCurrentAsk());
			}
			
			 // Set 4 : Short Trend reversed -> Reverse
			if ((Position.MarketPosition == MarketPosition.Short)
				 && (WAE.TrendDown[0] >= 0))
			{	// Entry() methods will reverse the position automatically
				EnterLongLimit(Convert.ToInt32(Quantity), GetCurrentBid());			
			}
						
			 // Set 5 : Long Explosion Died -> Exit
			if ((Position.MarketPosition == MarketPosition.Long)
				 && (WAE.TrendUp[0] <= WAE.ExplosionLine[0]))
			{	
				ExitLongLimit(Convert.ToInt32(Quantity), GetCurrentBid());
			}
			
			 // Set 6 : Short Explosion Died -> Exit
			if ((Position.MarketPosition == MarketPosition.Short)
				 && (WAE.TrendDown[0] >= WAE.ExplosionLineDn[0]))
			{			
				ExitShortLimit(Convert.ToInt32(Quantity), GetCurrentAsk());
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Sensitivity", Description="Waddah Attar Explosion sensitivity", Order=1, GroupName="Parameters")]
		public int Sensitivity
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Quantity", Order=2, GroupName="Parameters")]
		public int Quantity
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Fixed_rr", Description="Fixed Risk Reward", Order=3, GroupName="Parameters")]
		public bool Fixed_rr
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Risk", Description="Risk amount in ticks", Order=4, GroupName="Parameters")]
		public int Risk
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Reward", Order=5, GroupName="Parameters")]
		public int Reward
		{ get; set; }
		#endregion

	}
}
