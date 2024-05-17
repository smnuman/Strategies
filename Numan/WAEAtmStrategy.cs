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
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Strategies.Numan
{
	#region // Properties display preference
	[Gui.CategoryOrder("Trade Params", 1)]
	[Gui.CategoryOrder("Trading Method (choose at least one!)", 2)]
	[Gui.CategoryOrder("Troubleshooting", 3)]
	[Gui.CategoryOrder("Atm Strategy", 4)] 
	#endregion
	
	public class WAEAtmStrategy : Strategy
	{
		private NinjaTrader.NinjaScript.Indicators.Numan.WAE_Mod WAE;
		private NinjaTrader.NinjaScript.Indicators.TradeSaber.ReversalTS reversal;
		
		/// <summary>
		/// Addition by Numan
		#region Waddah Attar Explosion Variables
		
		private double	Mult, 
						DeadZone;
		
		private int 	Sensitivity,
						FastLength,
						SlowLength,
						FastSmoothLength,
						SlowSmoothLength,
						ChannelLength,
						DZATRPeriod;

		private bool 	FastSmooth, 
						SlowSmooth;
		
		#endregion
		private string thisOrderName;
		#region ATM Strategy dropdown addition;
		
		private string  atmStrategyId			= string.Empty;
		private string  orderId					= string.Empty;
		private bool	isAtmStrategyCreated	= false;
		
		#endregion
		/// </summary>

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description	= @"Waddah Attar Explosion Trades using ATM Strategy";
				Name		= @"WAE Atm Strategy";
				EntriesPerDirection							= 1;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				// This strategy has been designed to take advantage of performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration = false;
				
				#region Waddah Attar Explosion Variables 
				Mult 				= 2;
				DeadZone 			= 200;
				
				Sensitivity 		= 150;
				FastLength 			= 10;
				SlowLength 			= 30; 
				FastSmoothLength 	= 9; 
				SlowSmoothLength 	= 9; 
				ChannelLength 		= 30 ;
				
				FastSmooth 			= true; 
				SlowSmooth 			= true ;
				#endregion // WAE default values
				
				#region Trading variables
				Profit					= 100;
				Stop					= 50;
				Contracts				= 2;
				lookbackPeriod			= 2;
				
				allowReversal			= true;			/// Reversal	: TradeSabre's ReversalTS signals an opportunity.
				allowTrend				= true;			/// Trend		: WAE shows a trend to follow.
				allowContraTrend		= false;		/// ContraqTrend: when a trend in WAE shows the opposite opportunity.
				allowCrossOver			= true;			/// CrossOver	: WAE-Trend's crossover with the WAE-Explosion > showing a strong trend.
				allowHighRiskProfit		= false;		/// HighRisk	: Allowing Pullbacks for higher trailing drawdown > watch the danger!!.
				
				DefaultQuantity 		= Contracts; 	/// Default quantity is reset to user quantity.
				IncludeCommission 		= true;  		/// Makes use of default 'commission' used for the account.
				
				UseATMStrategy			= true;
				AtmStrategy				= @"Choose an ATM ";
				debug					= true;
				directiondetails		= true;
				conditionDetails		= false;
				slptSetup				= true;
				#endregion // Trading default values
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{
				//clear the output window as soon as the bars data is loaded
    			ClearOutputWindow();
				
				WAE							= WAE_Mod(Close, Sensitivity, FastLength, FastSmooth, FastSmoothLength, SlowLength, SlowSmooth, SlowSmoothLength, ChannelLength, Mult, DeadZone);
				
				reversal					= ReversalTS(Close, 0, 0, 1, true, Brushes.DarkCyan, Brushes.Indigo, true, Brushes.Green, false);
				reversal.Plots[0].Brush 	= Brushes.Transparent;
				
				AddChartIndicator(WAE); 
				AddChartIndicator(reversal); 
			}
		}

		protected override void OnBarUpdate()
		{
			#region Return Blocks
			if (BarsInProgress != 0) 
				return;

			if (CurrentBar < BarsRequiredToTrade)
				return;
			
			// Make sure this strategy does not execute against historical data : ATM strategy won't permit!!
			if(UseATMStrategy && State == State.Historical)
				return;
			#endregion

			#region Time/Session setup
			// Check if RTH
			bool market_open = ToTime(Time[0]) >= 093000 && ToTime(Time[0]) <= 153000;
			// Check if ETH
			bool pre_market_open = (ToTime(Time[0]) >= 000000 && ToTime(Time[0]) <= 093000) || (ToTime(Time[0]) > 180000 && ToTime(Time[0]) <= 235959 ) ;
			#endregion // Imports : Numan
			
			#region --- Trade Setups ---
			
			#region // Entry conditions  : Waddah Attar Explosion setup
			string tradeMsg = "";
			#region /// Entry Condition Pieces
			/// ======================================================
			#region /// Long condition peices
			/// Set 1 : 			Strong TrendUp just Crossed above Explosion :: Dark Green WAE Bars above zero line - Long entry
			/// 					OR TrendDown Crossed the ExplosionDn up :: Orange WAE Bars below zero line approaching zero! - Long entry
			bool WAExUP1 		= (	CrossAbove(WAE.TrendUp, WAE.ExplosionLine, lookbackPeriod) );
			bool WAExUP2		= ( CrossAbove(WAE.TrendDown, WAE.ExplosionLineDn, lookbackPeriod) && false ); // !!! deactivated !!!
			bool WAExUP			=  WAExUP1 || WAExUP2 ;
			
			tradeMsg			+= ( WAExUP1 ? "\nEntry Class: WAExUP1" : (WAExUP2 ? "\nEntry Class: WAExUP2": "") );
			
			/// Set 2 : 			TrendUp is increasing as well as the Explosion line is increasing
			bool WAETrendUp 	= (  	(WAE.TrendUp[1]			> WAE.TrendUp[2])
							  		&&	(WAE.ExplosionLine[1]	> WAE.ExplosionLine[2])
									&& 	(WAE.TrendUp[1]			> WAE.ExplosionLine[1]) );
			
			tradeMsg			+= (WAETrendUp ? "\nEntry Class: WAETrendUp" : "") ;
			
			/// Set 2A : 			Continuous up-run of WAE <= not always favourable
			bool WAEupRun		= (  	(WAE.TrendUp[2]			> WAE.TrendUp[3] )
							  		&&	(WAE.TrendUp[1]			> WAE.TrendUp[2] )
							  		&&	(WAE.TrendUp[3]			> 0 )
									&&	(WAE.ExplosionLine[0]	> WAE.ExplosionLine[1])) ;
			
			tradeMsg			+= (WAEupRun ? "\nEntry Class: WAEupRun" : "") ;
			
			/// Set 2B : 			TrendDn is getting smaller with the Explosion closing to zeroline : Orange WAE Bars below Zero - Earlier Long entry
			/// 					Finding early runner : Long  	// from below zeroLine
			/// 
			#endregion // with messages
			#region /// Short condition peices
			/// Set 3 : 			Strong TrendDown Just crossed the Explosion below :: Red WAE Bars below zeroLine - Short entry
			bool WAExDn1 		= (	CrossBelow(WAE.TrendDown, WAE.ExplosionLineDn, lookbackPeriod) );
			bool WAExDn2		= ( CrossBelow(WAE.TrendUp, WAE.ExplosionLine, lookbackPeriod) && false ); // !!! deactivated !!!
			bool WAExDn			= WAExDn1 || WAExDn2 ;
			
			tradeMsg			+= ( WAExDn1 ? "\nEntry Class: WAExDn1" : (WAExDn2 ? "\nEntry Class: WAExDn2": "") );
			
			/// Set 4 : 			TrendDn is decreasing as well as the explosionDn line is decreasing
			bool WAETrendDown 	= ( 	(WAE.TrendDown[1] 		< WAE.TrendDown[2])
							 		&&	(WAE.ExplosionLineDn[1] < WAE.ExplosionLineDn[2])
									&& 	(WAE.TrendDown[1] 		< WAE.ExplosionLineDn[1]) );
			
			tradeMsg			+= (WAETrendDown ? "\nEntry Class: WAETrendDown" : "") ;
			
			/// Set 4A : 			Continuous down-run of WAE <= not always favourable
			bool WAEdownRun		= (  	(WAE.TrendDown[2]		< WAE.TrendDown[3] )
							  		&&	(WAE.TrendDown[1]		< WAE.TrendDown[2] )
							  		&&	(WAE.TrendDown[3]		< 0 )
									&&	(WAE.ExplosionLineDn[0]	< WAE.ExplosionLineDn[1])) ;
			
			tradeMsg			+= (WAEdownRun ? "\nEntry Class: WAEdownRun" : "") ;
			
			/// Set 4B : 			TrendUp is getting smaller with the explosion closing to the zeroLine :: Lime WAE Bars above zeroLine -  earlier Short Entry
			/// 					Finding early runner : Short  	// from above zeroLine
			#endregion // with messages
			/// ======================================================
			#endregion
			
			#region -- Reversal Conditions --
			///						Reversal pattern spotted: when current down bar is above the previous down bar and the down bar before 
			/// 					but the previous down bar is below the one before!
			bool WAEReversalUp	= (reversal[0] == 1);	// using TradeSabre's free ReversalTS indicator; make sure to have it installed as an Indicator.
									/// old school way
//									(	(WAE.TrendDown[1] > WAE.TrendDown[2])
//									&& (WAE.TrendDown[1] > WAE.TrendDown[3])
//									&& (WAE.TrendDown[3] > WAE.TrendDown[2]) ) ;
			
			tradeMsg			+= (WAEReversalUp ? "\nEntry Class: WAEReversalUp" : "") ;
			
			///						Reversal pattern spotted: when current up bar is below the previous up bar and the up bar before 
			/// 					but the previous bar is above the one before!
			bool WAEReversalDn	= (reversal[0] == -1);	// using TradeSabre's free ReversalTS indicator; make sure to have it installed as an Indicator.
									/// old school way
//									(	(WAE.TrendUp[1] < WAE.TrendUp[2])
//									&& (WAE.TrendUp[1] < WAE.TrendUp[3])
//									&& (WAE.TrendUp[3] < WAE.TrendUp[2]) ) ;			
			
			tradeMsg			+= (WAEReversalDn ? "\nEntry Class: WAEReversalDn" : "") ;
			
			#endregion // Active with TradeSabre's ReversalTS
			
			#region // Prints - debug : Numan  
			if (debug && conditionDetails)
			{
				Print(string.Format("\n{0} | (CrossAbove(WAE.TrendUp, WAE.ExplosionLine, lookbackPeriod)): {1}", Time[0], WAExUP));
				Print(string.Format("_____________________ | (WAE.TrendUp[0] < WAE.TrendUp[1]): {0}, << WAE.TrendUp[0]={1} < WAE.TrendUp[1]={2} >>", (WAE.TrendUp[0] < WAE.TrendUp[1]), WAE.TrendUp[0], WAE.TrendUp[1]));
				Print(string.Format("_____________________ | (WAE.ExplosionLine[0] < WAE.ExplosionLine[1]): {0}, << WAE.ExplosionLine[0]={1} < WAE.ExplosionLine[1]={2} >>", (WAE.ExplosionLine[0] < WAE.ExplosionLine[1]),WAE.ExplosionLine[0], WAE.ExplosionLine[1]));
				Print(string.Format("_____________________ | **** WAETrendUp: {0}", WAETrendUp));
				Print(string.Format("\n{0} | (CrossBelow(WAE.TrendDown, WAE.ExplosionLineDn, lookbackPeriod)): {1}", Time[0], WAExDn));
				Print(string.Format("_____________________ | (WAE.TrendDown[0] > WAE.TrendDown[1]): {0}, <<WAE.TrendDown[0]={1} > WAE.TrendDown[1]={2}>>", (WAE.TrendDown[0] > WAE.TrendDown[1]), WAE.TrendDown[0], WAE.TrendDown[1]));
				Print(string.Format("_____________________ | (WAE.ExplosionLineDn[0] > WAE.ExplosionLineDn[1]): {0}, <<WAE.ExplosionLineDn[0]={1} > WAE.ExplosionLineDn[1]={2}>>", (WAE.ExplosionLineDn[0] > WAE.ExplosionLineDn[1]),WAE.ExplosionLineDn[0], WAE.ExplosionLineDn[1]));
				Print(string.Format("_____________________ | **** WAETrendDown: {0}", WAETrendDown));
				
			}
			#endregion
			
			bool WAE_up = (	(allowCrossOver && WAExUP) || (allowReversal && WAEReversalUp) || (allowTrend && WAETrendUp)	|| (WAEupRun && allowHighRiskProfit) ) ;
			bool WAE_dn = (	(allowCrossOver && WAExDn) || (allowReversal && WAEReversalDn) || (allowTrend && WAETrendDown)	|| (WAEdownRun && allowHighRiskProfit) ) ;
			
			#region // Prints - debug : Numan  
			if (debug && directiondetails)
			{
				string printMsg = "";
				Print(string.Format("\n{0} | Conditions found: {1}", Time[0], ( WAE_up? "for LONG": (WAE_dn? "for SHORT" : "None :: -F-L-A-T-")) ));
				if (WAE_up)
				{
					printMsg = (WAExUP ? "WAE Cross above" :(WAEReversalUp? "WAE Reversal (Up)" :(WAETrendUp ? "WAE Trending Up" : "No conditions found yet!") ) ) ;
					Print(string.Format("_____________________ Entered LONG  | {0} because <{1}>", WAE_up, printMsg));
				}
				else if (WAE_dn)
				{
					printMsg = (WAExDn? "WAE Cross below" : (WAEReversalDn ? "WAE Reversal (Down)" : (WAETrendDown ? "WAE Trending Down" : "No conditions found yet!") ) );
					Print(string.Format("_____________________ Entered SHORT | {0} because <{1}>", WAE_dn, printMsg ));
				}
				else 
				{
					Print(string.Format("\n_____________________ LONG  | WAE_up: {0}", WAE_up ));
					Print(string.Format("_____________________             | WAExup: {0}", WAExUP ));
					Print(string.Format("_____________________             | WAEReversalUp: {0}", WAEReversalUp ));
					Print(string.Format("_____________________             | WAETrendUp: {0}", WAETrendUp ));						
					Print(string.Format("\n_____________________ SHORT | WAE_dn: {0}", WAE_dn ));
					Print(string.Format("_____________________              | WAExDn: {0}", WAExDn ));
					Print(string.Format("_____________________              | WAEReversalDn: {0}", WAEReversalDn ));
					Print(string.Format("_____________________              | WAETrendDown: {0}", WAETrendDown ));	
				}

				
			}
			#endregion
			
			#endregion
			
			#region // Exit conditions   : Waddah Attar Explosion got controlled.
			string exitMsg = "-" ;
			
			bool WAETrendUpDec 	= ( (WAE.ExplosionLine[0]	<=	WAE.ExplosionLine[1])
							 		&& 	(WAE.TrendUp[0] 	<	WAE.TrendUp[1]) );
			bool WAETrendUpDead = CrossBelow(WAE.TrendUp, WAE.ExplosionLine, lookbackPeriod) ;
			bool WAE_up_close 	= (	/// Set 5 : TrendUp & Explosion both are declining :: time to get out - Exit Long entry
									(	WAETrendUpDec )
								||	/// Set 7 : TrendUp hiding between the explosion & zeroLine :: end of the bull run - Exit Long entry
									(  WAETrendUpDead )
								|| 	/// Reversal down
									( WAEReversalDn ) );
			
			exitMsg				+= WAETrendUpDec ? "Exit on: WAETrendUpDec" : ( WAETrendUpDead ? "Exit on: WAETrendUpDead" : (WAEReversalDn ? "Exit on: WAEReversalDn" : ""));
			
			bool WAETrendDnInc	= (WAE.ExplosionLineDn[0] >= 	WAE.ExplosionLineDn[1])
							 		&& 	(WAE.TrendDown[0] > 	WAE.TrendDown[1]);
			bool WAETrendDnDead	= CrossAbove(WAE.TrendUp, WAE.ExplosionLineDn, lookbackPeriod) ;
			bool WAE_dn_close = (  /// Set 6 : TrendDown & Explosion both are converging to the zeroLine :: end of Bull Strength - Exit Short
									(  WAETrendDnInc )
								|| /// Set 8 : TrenDown WAE bar hiding between the Explosion and zeroLine :: end of Bear Strength - Exit Short entries
									(  WAETrendDnDead )
								|| 	/// Reversal up
									(  WAEReversalUp ) );
			
			exitMsg				+= WAETrendDnInc ? "Exit on: WAETrendDnInc" : ( WAETrendDnDead ? "Exit on: WAETrendDnDead" : (WAEReversalUp ? "Exit on: WAEReversalUp" : ""));
			
			#endregion
			
			#region // Trailing Drawdown : closing/exiting whenever needed to reduce the DrawDown value
			bool WAE_PullBack = !allowHighRiskProfit // && IsFirstTickOfBar
								&& ( /// Finding Pullback : Long
									 (WAE.TrendUp[1] 	<	WAE.TrendUp[2])
									|| /// Finding Pullback : Short
									 (WAE.TrendDown[1] 	> 	WAE.TrendDown[2])
									);
			#endregion
			
			#endregion
			
			#region --- Trades with ATM attachment ---
			if (UseATMStrategy && (Position.MarketPosition == MarketPosition.Flat))
			{
				#region // Long Trade 
				/// Mods by Numan
				/// ---- need to modify & optimize more
				if (WAE_up)
				{
					Print(string.Format("\n *** {0} \n ***", tradeMsg));
					tradeMsg = "-" ; /// reset 'tradeMsg' !
					// Submits an entry limit order at the current low price to initiate an ATM Strategy if both order id and strategy id are in a reset state
					// **** YOU MUST HAVE AN ATM STRATEGY TEMPLATE NAMED 'AtmStrategyTemplate' CREATED IN NINJATRADER (SUPERDOM FOR EXAMPLE) FOR THIS TO WORK ****
					if (orderId.Length == 0 && atmStrategyId.Length == 0) /// Entry conditions are taken outside completely to accommodate the Short option
					{
						isAtmStrategyCreated 	= false;  // reset atm strategy created check to false
						atmStrategyId 			= GetAtmStrategyUniqueId();
						orderId 				= GetAtmStrategyUniqueId();
						
						AtmStrategyCreate(	OrderAction.Buy,
											OrderType.Limit,
											GetCurrentAsk(),
											GetCurrentBid(),
											TimeInForce.Day,
											orderId,
											AtmStrategy,
											atmStrategyId,
											(atmCallbackErrorCode, atmCallBackId) => {
							//check that the atm strategy create did not result in error, and that the requested atm strategy matches the id in callback
	//						if (atmCallbackErrorCode == ErrorCode.NoError && atmCallBackId == atmStrategyId)
	//							isAtmStrategyCreated = true;
							isAtmStrategyCreated = (atmCallbackErrorCode == ErrorCode.NoError && atmCallBackId == atmStrategyId);
						});
					}
		
					// Check that atm strategy was created before checking other properties
					if (isAtmStrategyCreated)
					{
						/// Upon successful opening of an ATM Strategy print the entry criteria
						/// 
						Print(string.Format("{0} : Strategy launched Atm : \'{1}\'", Time[0], AtmStrategy));
						
						// Check for a pending entry order
						if (orderId.Length > 0)
						{
							string[] status = GetAtmStrategyEntryOrderStatus(orderId);
			
							// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
							if (status.GetLength(0) > 0)
							{
								if (debug)
								{
									// Print out some information about the order to the output window
									Print(Time[0] + ": ATM : _____________ LONG ___________________"); 
									Print(Time[0] + ": ATM : Average fill price is: " + status[0]);
									Print(Time[0] + ": ATM : Order filled amount is: " + status[1]);
									Print(Time[0] + ": ATM : Order state is: " + status[2]);
								}
			
								// If the order state is terminal, reset the order id value
								if (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected")
									orderId = string.Empty;
							}
						} // If the strategy has terminated reset the strategy id
						else if (atmStrategyId.Length > 0 && GetAtmStrategyMarketPosition(atmStrategyId) == Cbi.MarketPosition.Flat)
							atmStrategyId = string.Empty;
			
						if (atmStrategyId.Length > 0)
						{
//							thisOrderName = "Stop1" ; //+ CurrentBar;
//							// You can change the stop price
//							if (GetAtmStrategyMarketPosition(atmStrategyId) != MarketPosition.Flat)
//								AtmStrategyChangeStopTarget(0, Low[0] - 3 * TickSize, thisOrderName, atmStrategyId);
							
							if (debug && slptSetup)
							{
								// Print some information about the strategy to the output window, please note you access the ATM strategy specific position object here
								// the ATM would run self contained and would not have an impact on your NinjaScript strategy position and PnL
								Print(string.Format("{0} : ATm | SL-PT reset :____________________ {1} _____________________", Time[0], GetAtmStrategyMarketPosition(atmStrategyId)) );
								Print(string.Format("{0} : ATM | SL-PT reset : position quantity is {1}, average price is {2},   Unrealized PnL [{3}]", "--------------------", GetAtmStrategyPositionQuantity(atmStrategyId), GetAtmStrategyPositionAveragePrice(atmStrategyId), GetAtmStrategyUnrealizedProfitLoss(atmStrategyId)));
							}
		
						}						
					}
					
				}
	
				#endregion
				
				#region // Short Trade
				/// Addition by Numan
				/// ---- need to modify & optimize here
				/// Hint: merge with previous 'if' for 'Long'
				if (WAE_dn)
				{
					Print(string.Format("\n *** {0} \n ***", tradeMsg));
					tradeMsg = "-";	/// reset 'tradeMsg' !
					// Submits an entry limit order at the current low price to initiate an ATM Strategy if both order id and strategy id are in a reset state
					// **** YOU MUST HAVE AN ATM STRATEGY TEMPLATE NAMED 'AtmStrategyTemplate' CREATED IN NINJATRADER (SUPERDOM FOR EXAMPLE) FOR THIS TO WORK ****
					if (orderId.Length == 0 && atmStrategyId.Length == 0)
					{
						isAtmStrategyCreated 	= false;  // reset atm strategy created check to false
						atmStrategyId 			= GetAtmStrategyUniqueId();
						orderId 				= GetAtmStrategyUniqueId();
						
						AtmStrategyCreate(	OrderAction.Sell, 
											OrderType.Limit, 
											GetCurrentBid(), 
											GetCurrentAsk(), 
											TimeInForce.Day, 
											orderId, 
											AtmStrategy, 
											atmStrategyId, 
											(atmCallbackErrorCode, atmCallBackId) => {
							//check that the atm strategy create did not result in error, and that the requested atm strategy matches the id in callback
							isAtmStrategyCreated = (atmCallbackErrorCode == ErrorCode.NoError && atmCallBackId == atmStrategyId) ;
						});
					}
		
					// Check that atm strategy was created before checking other properties
					if (isAtmStrategyCreated)
					{
						/// Upon successful opening of an ATM Strategy print the entry criteria
						/// 
						Print(string.Format("{0} : Strategy launched Atm : \'{1}\'", Time[0], AtmStrategy));

						if (orderId.Length > 0)
						{
							string[] status = GetAtmStrategyEntryOrderStatus(orderId);
			
							// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
							if (status.GetLength(0) > 0)
							{
								if(debug)
								{
									// Print out some information about the order to the output window
									Print(Time[0] + ": ATM : _____________ SHORT ___________________");
									Print(Time[0] + ": ATM : Average fill price is: " + status[0]);
									Print(Time[0] + ": ATM : Order filled amount is: " + status[1]);
									Print(Time[0] + ": ATM : Order state is: " + status[2]);								
								}
	
								// If the order state is terminal, reset the order id value
								if (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected")
									orderId = string.Empty;
							}
							
						} // If the strategy has terminated reset the strategy id
						else if (atmStrategyId.Length > 0 && GetAtmStrategyMarketPosition(atmStrategyId) == Cbi.MarketPosition.Flat)
							atmStrategyId = string.Empty;
			
						if (atmStrategyId.Length > 0)
						{
//							thisOrderName = "Stop1" ; // + CurrentBar;
//							// You can change the stop price
//							if (GetAtmStrategyMarketPosition(atmStrategyId) != MarketPosition.Flat)
//								AtmStrategyChangeStopTarget(High[0] + 3 * TickSize, 0, thisOrderName, atmStrategyId);
	
							if (debug && slptSetup)
							{
								// Print some information about the strategy to the output window, please note you access the ATM strategy specific position object here
								// the ATM would run self contained and would not have an impact on your NinjaScript strategy position and PnL
								Print(string.Format("{0} : ATm | SL-PT reset : ____________________ {1} _____________________", Time[0], GetAtmStrategyMarketPosition(atmStrategyId)) );
								Print(string.Format("{0} : ATM | SL-PT reset : position quantity is {1}, average price is {2},   Unrealized PnL [{3}]", "--------------------", GetAtmStrategyPositionQuantity(atmStrategyId), GetAtmStrategyPositionAveragePrice(atmStrategyId), GetAtmStrategyUnrealizedProfitLoss(atmStrategyId)));
							}
	
						}						
					}
	
				}
	
				#endregion				
			}
			#endregion
			
			#region // ATM Closure on reversal/exit condition
			
//			if (atmStrategyId.Length>0)
//			{			
//				bool 	closeAtmStrategyNow = false;
				
//				double 	profitAmt 	= GetAtmStrategyUnrealizedProfitLoss(atmStrategyId);
//				bool 	profitable 	= profitAmt > 0;				
////				closeAtmStrategyNow	= ( WAE_up_close || WAE_dn_close || (WAE_PullBack ) );//pullback && !profitable
				
//				if (isAtmStrategyCreated && closeAtmStrategyNow)
//				{
//					if ( AtmStrategyClose(atmStrategyId) ) 
//					{
//						Print(string.Format("\n{0} : ATM Strategy ({1}) is closed down successfully.\n _____________________ Exit Reason <<{2}>>", Time[0], atmStrategyId, exitMsg));					
//					}
//					else
//					{
//						Print(string.Format("\n{0} : Failed closing ATM strategy with Id ({1}) \n_____________________ OR No ATM Strategy found!!\n_____________________ Exit Reason <<{2}>>", Time[0], atmStrategyId, exitMsg));
//					}					

//				}				
//			}
//			else
//			{
//				Print( string.Format("\n{0} : AtmStrategy ID empty => No ATM Strategy launched\n{1}\n{2}", Time[0], tradeMsg, exitMsg) );
//			}

			#endregion // work on later
		}
		
		#region Properties
		
		#region Trading Parameters
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Profit", Description="Profit Target in ticks", Order=101, GroupName="Trade Params")]
		public int Profit
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Stop", Description="StopLoss in ticks", Order=102, GroupName="Trade Params")]
		public int Stop
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Contracts", Description="Number of contracts to trade", Order=103, GroupName="Trade Params")]
		public int Contracts
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Lookback Period", Description="Number of bars to lookback for crossing", Order=104, GroupName="Trade Params")]
		public int lookbackPeriod
		{ get; set; }

		// 
		
		[NinjaScriptProperty]
		[Display(Name="Allow reversal trade?", Description="If you do not have trailing drawdowns", Order=105, GroupName="Trading Method (choose at least one!)")]
		public bool allowReversal
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Allow trend trading?", Description="If you do not have trailing drawdowns", Order=106, GroupName="Trading Method (choose at least one!)")]
		public bool allowTrend
		{ get; set; }
				
		[NinjaScriptProperty]
		[Display(Name="Allow contra-trend trading?", Description="If you do not have trailing drawdowns", Order=106, GroupName="Trading Method (choose at least one!)")]
		public bool allowContraTrend
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Allow WAE crossover trades?", Description="If you do not have trailing drawdowns", Order=107, GroupName="Trading Method (choose at least one!)")]
		public bool allowCrossOver
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Allow High Risk Profit?", Description="If you do not have trailing drawdowns", Order=108, GroupName="Trading Method (choose at least one!)")]
		public bool allowHighRiskProfit
		{ get; set; }
		#endregion
		
		#region Troubleshooting
		[NinjaScriptProperty]
		[Display(Name="Show debug prints?", Description="If you do not have trailing drawdowns", Order=201, GroupName="Troubleshooting")]
		public bool debug
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="trade directions?", Description="Shows the direction of the trade (Long or Short)", Order=202, GroupName="Troubleshooting")]
		public bool directiondetails
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="trade conditions?", Description="Shows the trade conditions ( for Entry or Exit)", Order=203, GroupName="Troubleshooting")]
		public bool conditionDetails
		{ get; set; }
				
		[NinjaScriptProperty]
		[Display(Name="stoploss & profit Target?", Description="Shows the SL & PT details", Order=204, GroupName="Troubleshooting")]
		public bool slptSetup
		{ get; set; }
				
		#endregion
		
		#region ATM Dropdown Option
		[NinjaScriptProperty]
		[Display(Name="Use ATM Strategy ?", Description="If you like to use ATM an strategy for controlling the SL & PT", Order=301, GroupName="Atm Strategy")]
		public bool UseATMStrategy
		{ get; set; }
		
		[TypeConverter(typeof(FriendlyAtmConverter))] // Converts the found ATM template file names to string values
		[PropertyEditor("NinjaTrader.Gui.Tools.StringStandardValuesEditorKey")] // Create the combo box on the property grid
		[Display(Name = "Atm Strategy 1", Order = 302, GroupName = "Atm Strategy")]
		public string AtmStrategy
		{ get; set; }
		
		[TypeConverter(typeof(FriendlyAtmConverter))] // Converts the found ATM template file names to string values
		[PropertyEditor("NinjaTrader.Gui.Tools.StringStandardValuesEditorKey")] // Create the combo box on the property grid
		[Display(Name = "Atm Strategy 2", Order = 303, GroupName = "Atm Strategy")]
		public string AtmStrategy2
		{ get; set; }
		#endregion
		
		#endregion
	}
	/// Borrowed by Numan
	#region ATM Strategy Typeconversion class routine
	public class FriendlyAtmConverter : TypeConverter
	{  
	    // Set the values to appear in the combo box
	    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
	    {
	        List<string> values = new List<string>();
	        string[] files = System.IO.Directory.GetFiles(System.IO.Path.Combine(NinjaTrader.Core.Globals.UserDataDir, "templates", "AtmStrategy"), "*.xml");  
	 
	        foreach(string atm in files)
	        {
	            values.Add(System.IO.Path.GetFileNameWithoutExtension(atm));
	            NinjaTrader.Code.Output.Process(System.IO.Path.GetFileNameWithoutExtension(atm), PrintTo.OutputTab1);
	        }
	        return new StandardValuesCollection(values);
	    }
	 
	    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
	    {
	        return value.ToString();
	    }
	 
	    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
	    {
	        return value;
	    }
	 
	    // required interface members needed to compile
	    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	    { return true; }
	 
	    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	    { return true; }
	 
	    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
	    { return true; }
	 
	    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
	    { return true; }
	}
	#endregion
}