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
	public class WAEMarkerUnlocked : Strategy
	{
		private NinjaTrader.NinjaScript.Indicators.Numan.WAE_Mod WAE;
		private NinjaTrader.NinjaScript.Indicators.ATR dzATR;
		private Series<int> HiExplosion;
		private Series<int> LoExplosion;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Trade Marking with Waddah Attar Explosion";
				Name										= "WAEMarkerUnlocked";
				Calculate									= Calculate.OnPriceChange;
				EntriesPerDirection							= 9;
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
				Sensitivity						= 150;
				
				FastLength						= 10;
				FastSmooth						= true;
				FastSmoothLength				= 9;
				
				SlowLength						= 30;
				SlowSmooth						= true;
				SlowSmoothLength				= 9;
				
				ChannelLength					= 30;
				Mult							= 2.0;
				DeadZone						= 200;
				
//				DZATRPeriod						= 21;
								
				SET1					= true;
				SET2					= true;
				SET3					= true;
				SET4					= true;
				SET5					= true;
				SET6					= true;
				SET7					= true;
				SET8					= true;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				HiExplosion = new Series<int>(this);
				LoExplosion = new Series<int>(this);			
				WAE			= WAE_Mod(Close, Sensitivity, FastLength, FastSmooth, FastSmoothLength, SlowLength, SlowSmooth, SlowSmoothLength, ChannelLength, Mult, DeadZone);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 2)
				return;

			 // Set 1
			if (
				 // WAEExplosionUp
				((WAE.ExplosionLine[0] >= WAE.ExplosionLine[1])
				 && (WAE.ExplosionLine[1] >= WAE.ExplosionLine[2])
				 && (WAE.ExplosionLine[2] > 0)
				 && (WAE.TrendUp[0] > WAE.ExplosionLine[0]))
				 && (SET1 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"L", 0, (Low[0] + (-5 * TickSize)) );
			}
			
			 // Set 2
			if ((CrossAbove(WAE.TrendDown, WAE.ExplosionLineDn, 1))
				 && (WAE.TrendDown[1] != 0)
				 && (SET2 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"L2", 0, (Low[0] + (-5 * TickSize)) );
			}
			
			 // Set 3
			if (
				 // WAE Price action -- Long
				((WAE.TrendDown[0] > WAE.TrendDown[1])
				 && (WAE.TrendDown[1] < WAE.TrendDown[2])
				 && (WAE.TrendDown[2] < WAE.TrendDown[0]))
				 && (SET3 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"L3", 0, (Low[0] + (-5 * TickSize)) );
			}
			
			 // Set 4
			if (
				 // WAEExplosionDn
				((WAE.ExplosionLineDn[0] <= WAE.ExplosionLineDn[1])
				 && (WAE.ExplosionLineDn[1] <= WAE.ExplosionLineDn[2])
				 && (WAE.ExplosionLineDn[2] < 0)
				 && (WAE.TrendDown[0] < WAE.ExplosionLineDn[0]))
				 && (SET4 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"S", 0, (High[0] + (5 * TickSize)) );
			}
			
			 // Set 5
			if ((CrossBelow(WAE.TrendUp, WAE.ExplosionLine, 1))
				 && (WAE.TrendUp[1] != 0)
				 && (SET5 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"S2", 0, (High[0] + (5 * TickSize)) );
			}
			
			 // Set 6
			if (
				 // WAE Price action -- Short
				((WAE.TrendUp[0] < WAE.TrendUp[1])
				 && (WAE.TrendUp[1] > WAE.TrendUp[2])
				 && (WAE.TrendUp[2] > WAE.TrendUp[0])
				 && (SET6 == true))
				 // Candle Price action -- Short [Green-Red bar]
				 && (SET6 == true)
				 && (SET6 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"S3", 0, (High[0] + (5 * TickSize)) );
			}
			
			 // Set 7
			if ((WAE.TrendUp[0] > WAE.TrendUp[1])
				 && (Close[0] < Close[1])
				 && (SET7 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"X", 0, (High[0] + (5 * TickSize)) );
			}
			
			 // Set 8
			if ((WAE.TrendDown[0] < WAE.TrendDown[1])
				 && (Close[0] > Close[1])
				 && (SET8 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"X", 0, (Low[0] + (-5 * TickSize)) );
			}
			
			 // Set 9
			if ((WAE.TrendDown[1] < WAE.TrendDown[2])
				 && (Close[1] > Close[2])
				 && (Open[0] > Close[0])
				 && (SET8 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"S+", 0, (High[0] + (5 * TickSize)) );
			}
			
			 // Set 10
			if ((WAE.TrendUp[1] > WAE.TrendUp[2])
				 && (WAE.ExplosionLine[0] < WAE.ExplosionLine[1])
				 && (Close[1] < Close[2])
				 && (Open[0] >= Close[0])
				 && (SET7 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"S4", 0, (High[0] + (5 * TickSize)) );
			}
			
			 // Set 11
			if ((WAE.TrendDown[1] < WAE.TrendDown[2])
				 && (WAE.ExplosionLineDn[0] <= WAE.ExplosionLineDn[1])
				 && (Close[1] > Close[2])
				 && (Open[0] <= Close[0])
				 && (SET8 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"S5", 0, (High[0] + (5 * TickSize)) );
			}
			
			 // Set 12
			if ((WAE.TrendUp[1] > WAE.TrendUp[2])
				 && (Close[1] < Close[2])
				 && (Open[0] < Close[0])
				 && (SET7 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"L+", 0, (Low[0] + (-5 * TickSize)) );
			}
			
			 // Set 13
			if ((WAE.TrendUp[1] > WAE.TrendUp[2])
				 && (WAE.ExplosionLine[0] >= WAE.ExplosionLine[1])
				 && (Close[1] < Close[2])
				 && (Open[0] >= Close[0])
				 && (SET7 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"L4", 0, (Low[0] + (-5 * TickSize)) );
			}
			
			 // Set 14
			if ((WAE.TrendDown[1] < WAE.TrendDown[2])
				 && (WAE.ExplosionLineDn[0] > WAE.ExplosionLineDn[1])
				 && (Close[1] > Close[2])
				 && (Open[0] <= Close[0])
				 && (SET8 == true))
			{
				Draw.Text(this, Convert.ToString(CurrentBars[0]), @"L5", 0, (Low[0] + (-5 * TickSize)) );
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Sensitivity", Description="Sensitivity", Order=1, GroupName="Waddah Attar Parameters")]
		public int Sensitivity
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="FastLength", Description="FastEMA Length", Order=2, GroupName="Waddah Attar Parameters")]
		public int FastLength
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="FastSmooth", Description="Smoothen FastEMA", Order=3, GroupName="Waddah Attar Parameters")]
		public bool FastSmooth
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="FastSmoothLength", Description="FastEMA Smooth Length", Order=4, GroupName="Waddah Attar Parameters")]
		public int FastSmoothLength
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="SlowLength", Description="SlowEMA Length", Order=5, GroupName="Waddah Attar Parameters")]
		public int SlowLength
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="SlowSmooth", Description="Smoothen SlowEMA", Order=6, GroupName="Waddah Attar Parameters")]
		public bool SlowSmooth
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="SlowSmoothLength", Description="Smoothen SlowEMA Length", Order=7, GroupName="Waddah Attar Parameters")]
		public int SlowSmoothLength
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ChannelLength", Description="BB Channel Length", Order=8, GroupName="Waddah Attar Parameters")]
		public int ChannelLength
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="Mult", Description="BB Stdev Multiplier", Order=9, GroupName="Waddah Attar Parameters")]
		public double Mult
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.1, int.MaxValue)]
		[Display(Name="DeadZone", Description="No trade zone threshold", Order=10, GroupName="Waddah Attar Parameters")]
		public double DeadZone
		{ get; set; }

//		[NinjaScriptProperty]
//		[Range(1, int.MaxValue)]
//		[Display(Name="DZATRPeriod", Description="ATR period for 'No trade'/Deadzone threshold", Order=11, GroupName="Parameters")]
//		public double DZATRPeriod
//		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="SET1", Order=1, GroupName="Marker Parameters")]
		public bool SET1
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="SET2", Order=2, GroupName="Marker Parameters")]
		public bool SET2
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="SET3", Order=3, GroupName="Marker Parameters")]
		public bool SET3
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="SET4", Order=4, GroupName="Marker Parameters")]
		public bool SET4
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="SET5", Order=5, GroupName="Marker Parameters")]
		public bool SET5
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="SET6", Order=6, GroupName="Marker Parameters")]
		public bool SET6
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="SET7", Order=7, GroupName="Marker Parameters")]
		public bool SET7
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="SET8", Order=8, GroupName="Marker Parameters")]
		public bool SET8
		{ get; set; }
		#endregion

	}
}
