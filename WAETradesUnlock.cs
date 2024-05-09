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
	public class WAETradesUnlock : Strategy
	{
		private NinjaTrader.NinjaScript.Indicators.Lo.WaddahAttarExplosion WAE;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Trading with Waddah Attar Explosion";
				Name										= "WAETradesUnlock";
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
				
				Profit					= 300;
				Stop					= 50;
				Contracts				= 1;
				
				WAESensitivity			= 150;
				WAEFastLength			= 10;
				WAESlowLength			= 30;
				WAEFastSmooth			= true;
				WAESlowSmooth			= true;
				WAEFastSmoothLength		= 9;
				WAESlowSmoothLength		= 9;
				WAEChannelLength		= 30;
				WAEMult					= 2;
				WAEDeadZone				= 200;
				
				DefaultQuantity			= Contracts;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				WAE				= WaddahAttarExplosion(Close, Convert.ToInt32(WAESensitivity), Convert.ToInt32(WAEFastLength), WAEFastSmooth, Convert.ToInt32(WAEFastSmoothLength), Convert.ToInt32(WAESlowLength), WAESlowSmooth, Convert.ToInt32(WAESlowSmoothLength), Convert.ToInt32(WAEChannelLength), WAEMult, WAEDeadZone);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
				return;

			if (Position.MarketPosition == MarketPosition.Flat)
			{
//				if ( (CrossAbove(WAE.TrendUp, WAE.ExplosionLine, 1))
//					|| ((WAE.TrendUp[0] > WAE.TrendUp[1])
//				 		&& (WAE.ExplosionLine[0] >= WAE.ExplosionLine[1]))
//				 	)
				if (Close[0] > High[1]) EnterLong();
								
//				else if ( (CrossBelow(WAE.TrendDown, WAE.ExplosionLineDn, 1))
//						|| ((WAE.TrendDown[0] < WAE.TrendDown[1])
//				 			&& (WAE.ExplosionLineDn[0] <= WAE.ExplosionLineDn[1]))
//						)
				if (Close[0] < High[1]) EnterShort();
				
			}
			
			if (Position.MarketPosition == MarketPosition.Long)
			{
//				if ( (CrossBelow(WAE.TrendUp, WAE.ExplosionLine, 1))
//					|| ((WAE.TrendUp[0] < WAE.TrendUp[1])
//				 		&& (WAE.ExplosionLine[0] < WAE.ExplosionLine[1]))
//				 	)
				if (IsFirstTickOfBar) ExitLong();
			}
			
			if (Position.MarketPosition == MarketPosition.Short)
			{
//				if ( (CrossAbove(WAE.TrendDown, WAE.ExplosionLineDn, 1))
//						|| ((WAE.TrendDown[0] > WAE.TrendDown[1])
//				 			&& (WAE.ExplosionLineDn[0] > WAE.ExplosionLineDn[1]))
//						)
				if (IsFirstTickOfBar) ExitShort();
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Profit", Description="Take Profit in ticks", Order=1, GroupName="Parameters")]
		public int Profit
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Stop", Description="StopLoss in ticks", Order=2, GroupName="Parameters")]
		public int Stop
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Contracts", Description="Number of contracts", Order=3, GroupName="Parameters")]
		public int Contracts
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAESensitivity", Description="Waddah Attar Explosion Sensitivity", Order=4, GroupName="Parameters")]
		public int WAESensitivity
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAEFastLength", Description="WAE Fast MA Length", Order=5, GroupName="Parameters")]
		public int WAEFastLength
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAESlowLength", Description="WAE Slow MA Length", Order=6, GroupName="Parameters")]
		public int WAESlowLength
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="WAEFastSmooth", Description="WAE Fast MA Smoothing", Order=7, GroupName="Parameters")]
		public bool WAEFastSmooth
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="WAESlowSmooth", Description="WAE Slow MA Smoothing", Order=8, GroupName="Parameters")]
		public bool WAESlowSmooth
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAEFastSmoothLength", Description="WAE Fast MA Smoothing Length", Order=9, GroupName="Parameters")]
		public int WAEFastSmoothLength
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAESlowSmoothLength", Description="WAE Slow MA Smoothing Length", Order=10, GroupName="Parameters")]
		public int WAESlowSmoothLength
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAEChannelLength", Description="Band Channel Length", Order=11, GroupName="Parameters")]
		public int WAEChannelLength
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.001, double.MaxValue)]
		[Display(Name="WAEMult", Description="WAE StDev. Multiplier", Order=12, GroupName="Parameters")]
		public double WAEMult
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAEDeadZone", Description="WAE DeadZone Value", Order=13, GroupName="Parameters")]
		public int WAEDeadZone
		{ get; set; }
		#endregion

	}
}
