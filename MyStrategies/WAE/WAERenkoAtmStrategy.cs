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
namespace NinjaTrader.NinjaScript.Strategies.MyStrategies.WAE
{
	#region // Properties display preference
	[Gui.CategoryOrder("Trading Params", 1)]
	[Gui.CategoryOrder("Troubleshooting", 2)]
	[Gui.CategoryOrder("Atm Strategy", 3)] 
	#endregion
	
	public class WAERenkoAtmStrategy : Strategy
	{
		private NinjaTrader.NinjaScript.Indicators.NMN.WAE_Mod WAE;
		
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
				Description	= @"Waddah Attar Explosion Trades with Renko Bars using ATM Strategy";
				Name		= @"WAERenkoAtmStrategy";
				// This strategy has been designed to take advantage of performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration = false;
				
				#region Waddah Attar Explosion Variables default values
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
				#endregion
				
				#region // Trading variables defaults
				Profit					= 100;
				Stop					= 50;
				StopLookback			= 10;			/// 'lookback' Bar for finding lowest or highest Stoploss setup
				StopOffset				= 3;
				Contracts				= 2;
				lookbackPeriod			= 2;
				allowHighRiskProfit		= false;		/// change it to 'true' if you do not have trailing drawdown.
				
				DefaultQuantity 		= Contracts; 	/// Default quantity is reset to user quantity.
				IncludeCommission 		= true;  		/// Makes use of default 'commission' used for the account.
				
				UseATMStrategy			= true;
				AtmStrategy				= @"ATM-Micro 1 20-50";
				debug					= true;
				directiondetails		= true;
				conditionDetails		= true;
				slptSetup				= true;
				#endregion
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{
				//clear the output window as soon as the bars data is loaded
    			ClearOutputWindow();        
				WAE	= WAE_Mod(Close, Sensitivity, FastLength, FastSmooth, FastSmoothLength, SlowLength, SlowSmooth, SlowSmoothLength, ChannelLength, Mult, DeadZone);
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
			
			/// <summary>
			/// Imports : Numan
			#region Time/Session setup
			// Check if RTH
			bool market_open = ToTime(Time[0]) >= 093000 && ToTime(Time[0]) <= 153000;
			// Check if ETH
			bool pre_market_open = (ToTime(Time[0]) >= 000000 && ToTime(Time[0]) <= 093000) || (ToTime(Time[0]) > 180000 && ToTime(Time[0]) <= 235959 ) ;
			#endregion
			/// </summary>
			
			#region --- Trade Setups ---
			
			#region // Entry conditions : Waddah Attar Explosion setup
			string tradeMsg = "-";
			#region /// Entry Condition Pieces
			/// ======================================================
			/// Set 1 : 			Strong TrendUp just Crossed above Explosion :: Dark Green WAE Bars above zero line - Long entry
			/// 					OR TrendDown Crossed the ExplosionDn up :: Orange WAE Bars below zero line approaching zero! - Long entry
			bool WAExUP1 		= (	CrossAbove(WAE.TrendUp, WAE.ExplosionLine, lookbackPeriod) );
			bool WAExUP2		= false; /// ( CrossAbove(WAE.TrendDown, WAE.ExplosionLineDn, lookbackPeriod) );
			bool WAExUP			=  WAExUP1 || WAExUP2 ;
			
			tradeMsg			+= ( WAExUP1 ? "\nEntry when: WAExUP1" : (WAExUP2 ? "\nEntry when: WAExUP2": "") );
			
			/// Set 2 : 			Strong TrendDown Just crossed the Explosion below :: Red WAE Bars below zeroLine - Short entry
			/// 					OR TrendUp Crossed the Explosion down :: Green WAE Bars above zero line approaching zero! - Short entry
			bool WAExDn1 		= (	CrossBelow(WAE.TrendDown, WAE.ExplosionLineDn, lookbackPeriod) );
			bool WAExDn2		= false; /// ( CrossBelow(WAE.TrendUp, WAE.ExplosionLine, lookbackPeriod) );
			bool WAExDn			= WAExDn1 || WAExDn2 ;
			
			tradeMsg			+= ( WAExDn1 ? "\nEntry when: WAExDn1" : (WAExDn2 ? "\nEntry when: WAExDn2": "") );
			
			/// ======================================================

			#endregion
			
			#region // Prints - debug : Numan  
			if (debug && conditionDetails)
			{
				Print(string.Format("\n{0} | (CrossAbove(WAE.TrendUp, WAE.ExplosionLine, lookbackPeriod)): {1}", Time[0], WAExUP));
				Print(string.Format("_____________________ | (WAE.TrendUp[0] < WAE.TrendUp[1]): {0}, << WAE.TrendUp[0]={1} < WAE.TrendUp[1]={2} >>", (WAE.TrendUp[0] < WAE.TrendUp[1]), WAE.TrendUp[0], WAE.TrendUp[1]));
				Print(string.Format("_____________________ | (WAE.ExplosionLine[0] < WAE.ExplosionLine[1]): {0}, << WAE.ExplosionLine[0]={1} < WAE.ExplosionLine[1]={2} >>", (WAE.ExplosionLine[0] < WAE.ExplosionLine[1]),WAE.ExplosionLine[0], WAE.ExplosionLine[1]));
//				Print(string.Format("_____________________ | **** WAETrendUp: {0}", WAETrendUp));
				Print(string.Format("\n{0} | (CrossBelow(WAE.TrendDown, WAE.ExplosionLineDn, lookbackPeriod)): {1}", Time[0], WAExDn));
				Print(string.Format("_____________________ | (WAE.TrendDown[0] > WAE.TrendDown[1]): {0}, <<WAE.TrendDown[0]={1} > WAE.TrendDown[1]={2}>>", (WAE.TrendDown[0] > WAE.TrendDown[1]), WAE.TrendDown[0], WAE.TrendDown[1]));
				Print(string.Format("_____________________ | (WAE.ExplosionLineDn[0] > WAE.ExplosionLineDn[1]): {0}, <<WAE.ExplosionLineDn[0]={1} > WAE.ExplosionLineDn[1]={2}>>", (WAE.ExplosionLineDn[0] > WAE.ExplosionLineDn[1]),WAE.ExplosionLineDn[0], WAE.ExplosionLineDn[1]));
//				Print(string.Format("_____________________ | **** WAETrendDown: {0}", WAETrendDown));
				
			}
			#endregion
			
			bool WAE_up = (	WAExUP ); /// || WAEReversalUp || WAETrendUp || WAEupRun ) ;
			bool WAE_dn = (	WAExDn ); /// || WAEReversalDn || WAETrendDown || WAEdownRun ) ;
			
			#region // Prints - debug : Numan  
			if (debug && directiondetails)
			{
				Print(string.Format("\n{0} | Final Conditions: {1}", Time[0], CurrentBar));
				Print(string.Format("_____________________ LONG  | WAE_up: {0}", WAE_up ));
				Print(string.Format("_____________________             | WAExup: {0}", WAExUP ));
//				Print(string.Format("_____________________             | WAEReversalUp: {0}", WAEReversalUp ));
//				Print(string.Format("_____________________             | WAETrendUp: {0}", WAETrendUp ));
				Print(string.Format("\n_____________________ SHORT | WAE_dn: {0}", WAE_dn ));
				Print(string.Format("_____________________              | WAExDn: {0}", WAExDn ));
//				Print(string.Format("_____________________              | WAEReversalDn: {0}", WAEReversalDn ));
//				Print(string.Format("_____________________              | WAETrendDown: {0}", WAETrendDown ));
				
			}
			#endregion
			
			#endregion
			
			#endregion
			
			if (UseATMStrategy)
			{
				#region --- Trades with ATM attachment ---
				#region // Long Trade 
				/// Mods by Numan
				/// ---- need to modify & optimize more
				if (WAE_up)
				{
					Print(string.Format("\n*** {0} \n***", tradeMsg));
					tradeMsg = "-" ; /// reset 'tradeMsg' !
					// Submits an entry limit order at the current low price to initiate an ATM Strategy if both order id and strategy id are in a reset state
					
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
									Print(Time[0] + ": _____________ LONG ___________________\n" + Time[0] + ": The entry order average fill price is: " + status[0]);
									Print(Time[0] + ": The entry order filled amount is: " + status[1]);
									Print(Time[0] + ": The entry order order state is: " + status[2]);
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
							thisOrderName = "STOP" + CurrentBar;
							// You can change the stop price
							if (GetAtmStrategyMarketPosition(atmStrategyId) != MarketPosition.Flat)
								AtmStrategyChangeStopTarget(0, Low[LowestBar(Low,StopLookback)] - StopOffset * TickSize, thisOrderName, atmStrategyId);
							
							if (debug && slptSetup)
							{
								// Print some information about the strategy to the output window, please note you access the ATM strategy specific position object here
								// the ATM would run self contained and would not have an impact on your NinjaScript strategy position and PnL
								Print(Time[0] + ": ____________________ LONG _____________________\n" + Time[0] + ": The current ATM Strategy market position is: " + GetAtmStrategyMarketPosition(atmStrategyId));
								Print(Time[0] + ": The current ATM Strategy position quantity is: " + GetAtmStrategyPositionQuantity(atmStrategyId));
								Print(Time[0] + ": The current ATM Strategy average price is: " + GetAtmStrategyPositionAveragePrice(atmStrategyId));
								Print(Time[0] + ": The current ATM Strategy Unrealized PnL is: " + GetAtmStrategyUnrealizedProfitLoss(atmStrategyId));						
							}
		
						}						
					}
					else {
						Print(Time[0] + ": E R R O R :: ATM Strategy creation *N=O=T* Successful!! ****** Long entry ");
					}
					
				}
	
				#endregion
				
				#region // Short Trade
				/// Addition by Numan
				/// ---- need to modify & optimize here
				/// Hint: merge with previous 'if' for 'Long'
				if (WAE_dn)
				{
					Print(string.Format("\n\n*** {0} ***\n", tradeMsg));
					tradeMsg = "-";	/// reset 'tradeMsg' !
					// Submits an Long limit order at the current low price to initiate an ATM Strategy if both order id and strategy id are in a reset state
					
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
						// Check for a pending entry order
						if (orderId.Length > 0)
						{
							string[] status = GetAtmStrategyEntryOrderStatus(orderId);
			
							// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
							if (status.GetLength(0) > 0)
							{
								if(debug)
								{
									// Print out some information about the order to the output window
									Print(Time[0] + ": _____________ SHORT ___________________\n" + Time[0] + ": The entry order average fill price is: " + status[0]);
									Print(Time[0] + ": The entry order filled amount is: " + status[1]);
									Print(Time[0] + ": The entry order order state is: " + status[2]);								
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
							thisOrderName = "STOP" + CurrentBar;
							// You can change the stop price
							if (GetAtmStrategyMarketPosition(atmStrategyId) != MarketPosition.Flat)
								AtmStrategyChangeStopTarget(High[HighestBar(High, StopLookback)] + StopOffset * TickSize, 0, thisOrderName, atmStrategyId);
	
							if (debug && slptSetup)
							{
								// Print some information about the strategy to the output window, please note you access the ATM strategy specific position object here
								// the ATM would run self contained and would not have an impact on your NinjaScript strategy position and PnL
								Print(Time[0] + ": ____________________ SHORT _____________________\n" + Time[0] + ": The current ATM Strategy market position is: " + GetAtmStrategyMarketPosition(atmStrategyId));
								Print(Time[0] + ": The current ATM Strategy position quantity is: " + GetAtmStrategyPositionQuantity(atmStrategyId));
								Print(Time[0] + ": The current ATM Strategy average price is: " + GetAtmStrategyPositionAveragePrice(atmStrategyId));
								Print(Time[0] + ": The current ATM Strategy Unrealized PnL is: " + GetAtmStrategyUnrealizedProfitLoss(atmStrategyId));							
							}
	
						}						
					}
					else {
						Print(Time[0] + ": E R R O R :: ATM Strategy creation *N=O=T* Successful!! ****** Short entry");
					}
						
				}
	
				#endregion				
				#endregion
			} 
			else 
			{
				#region --- Trades without ATM attachment ---
				// enter codes here for trade entry without using ATM strategy
				#region // Long Trade
				if (WAE_up)
				{
					// Long trade routines without ATM
				}
				#endregion
				
				#region // Short Trade
				if (WAE_dn)
				{
					// Short trade routines without ATM
				}
				#endregion
				#endregion
			}			
		}
		
		#region Properties
		
		#region Trading Parameters
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Profit", Description="Profit Target in ticks", Order=1, GroupName="Trading Params")]
		public int Profit
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Stop", Description="StopLoss in ticks", Order=2, GroupName="Trading Params")]
		public int Stop
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StopLookback", Description="Number of bars to 'lookback' for finding lowest or highest Stoploss setup", Order=3, GroupName="Trading Params")]
		public int StopLookback
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StopOffset", Description="Number of ticks to offset the Stoploss setup", Order=4, GroupName="Trading Params")]
		public int StopOffset
		{ get; set; } 

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Contracts", Description="Number of contracts to trade", Order=5, GroupName="Trading Params")]
		public int Contracts
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Lookback Period", Description="Number of bars to lookback for crossing", Order=6, GroupName="Trading Params")]
		public int lookbackPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Allow High Risk Profit?", Description="If you do not have trailing drawdowns", Order=7, GroupName="Trading Params")]
		public bool allowHighRiskProfit
		{ get; set; }
		#endregion
		
		#region Troubleshooting
		[NinjaScriptProperty]
		[Display(Name="Show debug prints?", Description="If you do not have trailing drawdowns", Order=1, GroupName="Troubleshooting")]
		public bool debug
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Show debug trade directions?", Description="Shows the direction of the trade (Long or Short)", Order=2, GroupName="Troubleshooting")]
		public bool directiondetails
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Show debug trade conditions?", Description="Shows the trade conditions ( for Entry or Exit)", Order=3, GroupName="Troubleshooting")]
		public bool conditionDetails
		{ get; set; }
				
		[NinjaScriptProperty]
		[Display(Name="Show debug trade stoploss & profit Target?", Description="Shows the SL & PT details", Order=3, GroupName="Troubleshooting")]
		public bool slptSetup
		{ get; set; }
				
		#endregion
		
		#region ATM Dropdown Option
		[NinjaScriptProperty]
		[Display(Name="Use ATM Strategy ?", Description="If you like to use ATM an strategy for controlling the SL & PT", Order=1, GroupName="Atm Strategy")]
		public bool UseATMStrategy
		{ get; set; }
		
		[TypeConverter(typeof(ninZaRenkoAtmConverter))] // Converts the found ATM template file names to string values
		[PropertyEditor("NinjaTrader.Gui.Tools.StringStandardValuesEditorKey")] // Create the combo box on the property grid
		[Display(Name = "Atm Strategy", Order = 2, GroupName = "Atm Strategy")]
		public string AtmStrategy
		{ get; set; }
		
		[TypeConverter(typeof(ninZaRenkoAtmConverter))] // Converts the found ATM template file names to string values
		[PropertyEditor("NinjaTrader.Gui.Tools.StringStandardValuesEditorKey")] // Create the combo box on the property grid
		[Display(Name = "Atm Strategy", Order = 3, GroupName = "Atm Strategy")]
		public string AtmStrategy2
		{ get; set; }
		#endregion
		
		#endregion
	}
	/// Borrowed from Ninja Forum -- by Numan
	#region ATM Strategy Typeconversion class routine
	public class ninZaRenkoAtmConverter : TypeConverter
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
