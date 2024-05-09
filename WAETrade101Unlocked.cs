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
	public class WAETrade101Unlocked : Strategy
	{
		private int Last_trade;
		private bool SetSLPT;

		private NinjaTrader.NinjaScript.Indicators.Lo.WaddahAttarExplosion WAE;
		
		private Series<double> green;
		private Series<double> red;
		private Series<double> brown;
		private Series<int> longs;
		private Series<int> shorts;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "WAETrade101Unlocked";
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
				Target					= 10;
				Stop					= 10;
				MACD_Fast				= 20;
				MACD_Slow				= 40;
				MACD_Smooth				= 7;
				Sensitivity				= 150;
				StDev_Bars				= 20;
				DeadZone				= 20;
				Repeat_Trades			= 1;
				LotSize					= 1;
				Start_Time				= DateTime.Parse("09:00", System.Globalization.CultureInfo.InvariantCulture);
				End_Time				= DateTime.Parse("21:00", System.Globalization.CultureInfo.InvariantCulture);
				Last_trade				= 0;
				SetSLPT					= false;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				green 	= new Series<double>(this);
				red 	= new Series<double>(this);
				brown 	= new Series<double>(this);
				longs 	= new Series<int>(this);
				shorts 	= new Series<int>(this);
				
				WAE	= WaddahAttarExplosion(Close, Convert.ToInt32(Sensitivity), Convert.ToInt32(MACD_Fast), true, Convert.ToInt32(MACD_Smooth), Convert.ToInt32(MACD_Slow), true, Convert.ToInt32(MACD_Smooth), Convert.ToInt32(StDev_Bars), 2, DeadZone);
				
//				DefaultQuantity = LotSize;
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			green[0]	= WAE.TrendUp[0];
			red[0] 		= WAE.TrendDown[0];
			brown[0] 	= WAE.ExplosionLine[0];
			longs[0] 	= 0;
			shorts[0] 	= 0;
			
			if (CurrentBars[0] < BarsRequiredToTrade)
				return;

			 // Set 2
			if (
				 // Long Reversed
				((green[0] < green[2])
				 && (green[2] < green[1]))
				 // Short Reversed
				 || ((red[0] < red[2])
				 && (red[2] < red[1])))
			{
				ExitLong(Convert.ToInt32(DefaultQuantity), "", "");
				ExitShort(Convert.ToInt32(DefaultQuantity), "", "");
			}
			
			 // Set 3
			if ((green[0] > green[1])
				 && (green[0] > brown[0])
				 && (green[0] > DeadZone)
				 && (Times[0][0].TimeOfDay >= Start_Time.TimeOfDay)
				 && (Times[0][0].TimeOfDay < End_Time.TimeOfDay))
			{
				longs[0] = 1;
			}
			
			 // Set 4
			if ((red[0] > red[1])
				 && (red[0] > brown[0])
				 && (red[0] > DeadZone)
				 && (Times[0][0].TimeOfDay >= Start_Time.TimeOfDay)
				 && (Times[0][0].TimeOfDay < End_Time.TimeOfDay))
			{
				shorts[0] = 1;
			}
			
			 // Set 5
			if ((longs[0] > longs[1])
				 // Repeat Filter
				 && ((Repeat_Trades == 1)
				 || (Last_trade != 1)))
			{
				Last_trade = 1;
				EnterLong(Convert.ToInt32(DefaultQuantity), "");
				SetSLPT = true;
			}
			
			 // Set 6
			if ((Position.MarketPosition == MarketPosition.Long)
				 && (Last_trade == 1)
				 && (SetSLPT == true))
			{
				ExitLongStopLimit(Convert.ToInt32(DefaultQuantity),0, (Position.AveragePrice - (Stop * TickSize)) , @"STOP", "");
				ExitLongLimit(Convert.ToInt32(DefaultQuantity), (Position.AveragePrice + (Target * TickSize)), @"Target", "" );
				SetSLPT = false;
			}
			
			 // Set 7
			if ((shorts[0] > shorts[1])
				 // Repeat Filter
				 && ((Repeat_Trades == 1)
				 || (Last_trade != -1)))
			{
				Last_trade = -1;
				EnterShort(Convert.ToInt32(DefaultQuantity), "");
				SetSLPT = true;
			}
			
			 // Set 8
			if ((Position.MarketPosition == MarketPosition.Short)
				 && (Last_trade == -1)
				 && (SetSLPT == true))
			{
				ExitShortStopLimit(Convert.ToInt32(DefaultQuantity), (Position.AveragePrice + (Stop * TickSize)), 0 , @"STOP", "");
				ExitShortLimit(Convert.ToInt32(DefaultQuantity), (Position.AveragePrice + (Target * TickSize)), @"Target", "" );
				SetSLPT = false;
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Target", Order=1, GroupName="Parameters")]
		public int Target
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Stop", Order=2, GroupName="Parameters")]
		public int Stop
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MACD_Fast", Order=3, GroupName="Parameters")]
		public int MACD_Fast
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MACD_Slow", Order=4, GroupName="Parameters")]
		public int MACD_Slow
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MACD_Smooth", Order=5, GroupName="Parameters")]
		public int MACD_Smooth
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Sensitivity", Order=6, GroupName="Parameters")]
		public int Sensitivity
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StDev_Bars", Order=7, GroupName="Parameters")]
		public int StDev_Bars
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="DeadZone", Order=8, GroupName="Parameters")]
		public int DeadZone
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Repeat_Trades", Description="1=YES  ,  2=NO", Order=9, GroupName="Parameters")]
		public int Repeat_Trades
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Quamtity", Description="Trade Lot size", Order=10, GroupName="Parameters")]
		public int LotSize
		{ get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="Start_Time", Order=11, GroupName="Parameters")]
		public DateTime Start_Time
		{ get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="End_Time", Order=12, GroupName="Parameters")]
		public DateTime End_Time
		{ get; set; }
		#endregion

	}
}
