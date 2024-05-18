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
namespace NinjaTrader.NinjaScript.Strategies.NT8ForumExamples
{
	public class FNTACDailyProfitLossLimit : Strategy
	{
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"free-ninjatrader-algo-code.com";
				Name										= "FNTACDailyProfitLossLimit";
				Calculate									= Calculate.OnEachTick;
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
			else if (State == State.Configure)
			{
			
			}			
		}	
		
	#region Dashboard //user controlled variables

		double dailyOpenTime = 83000;
		double dailyCloseTime = 153000;
		double dailyLossLimit = -500;
		double dailyProfitLimit = 500;
			
	#endregion

	#region Program Variables //variables that are handled programatically
			
		double currentDayProfit;		
		double previousRunningProfit;	
		bool eodUpkeep = true; //flag used to ensure that end of day upkeep only happens once per day
			
	#endregion
		
protected override void OnBarUpdate()
{	
				
		if (State != State.Realtime)
			return;			
		
		if (Bars.Count < BarsRequiredToTrade)			
			return;							
		
	#region Entry Logic
		
		if (currentDayProfit > dailyLossLimit && currentDayProfit < dailyProfitLimit) //only enter if profit/loss limit is not reached		
		{	
			
			if (ToTime(Time[0]) > dailyOpenTime && ToTime(Time[0]) < dailyCloseTime)	
			{
				//entry logic goes here
				
				Random rand = new Random();
			   	int random = rand.Next(1, 10); // creates a number between 1 and 10										
				
				if (random % 2 == 0) // if random is even
					EnterLong("long");
				
				if (random % 2 != 0) // if random is odd
					EnterShort("short");				
			}
		}
		
	#endregion			
		
	#region Exit when daily PL limit is reached
	
		if (currentDayProfit > dailyLossLimit && currentDayProfit < dailyProfitLimit) //if profit/loss limit is reached	exit positions
		{
					
			if (Position.MarketPosition == MarketPosition.Long) // if our new position is long
			{
				ExitLong("longExit", "long");
			}	
			
			if (Position.MarketPosition == MarketPosition.Short) // if our new position is short
			{
				ExitShort("ShortExit", "short");
			}			
			
		}
		
	#endregion		

} //end OnBarUpdate		

protected override void OnMarketData(MarketDataEventArgs marketDataUpdate)
{	
	currentDayProfit = SystemPerformance.RealTimeTrades.TradesPerformance.Currency.CumProfit - previousRunningProfit; // update daily profit
		
	#region Beginning of Day
		
		if (ToTime(marketDataUpdate.Time) == dailyOpenTime && eodUpkeep == true)
		{
			Print("Beginning of trading day.");			
			eodUpkeep = false;	
		}		
		
	#endregion	
	
	#region End of Day
		
		if (ToTime(marketDataUpdate.Time) > dailyCloseTime && eodUpkeep == false && Position.MarketPosition == MarketPosition.Flat) // if we're past EOD, and we haven't done EOD upkeep, and we're flat
		{			
			
			Print("End of trading day.");						
			eodUpkeep = true;
			previousRunningProfit = SystemPerformance.RealTimeTrades.TradesPerformance.Currency.CumProfit;							
			
		}
			
	#endregion	
	
}//end OnMarketData

} //end class
	
}

//This code was downloaded from free-ninjatrader-algo-code.com
