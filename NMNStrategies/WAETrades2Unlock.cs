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
namespace NinjaTrader.NinjaScript.Strategies.NMNStrategies
{	
	[Gui.CategoryOrder("Trading Params", 1)] 
	[Gui.CategoryOrder("WAE Parameters", 2)] 
	[Gui.CategoryOrder("Atm Strategy", 3)] 
		
	public class WAETrades2Unlock : Strategy
	{
		#region private variables
		#region --- Strategy specific private variables ---
		private NinjaTrader.NinjaScript.Indicators.NMN.WAE_Mod WAE;
		private const string myLong = "LongEntry", myShort = "ShortEntry", myLExit = "LongExit", mySExit = "ShortExit";
		private bool 	setSLTP, 
						WAELong1, WAELong2, WAELong3, 
						WAEShort1, WAEShort2, WAEShort3, 
						WAEShortExit, WAELongExit,
						currentLongEntry, currentShortEntry;
		#endregion
		
		/// <summary>
        /// This strategy will adapt the current account position and then submit a PT limit order and SL Stop order.
		/// Upon either the PT or SL or even a manual order which closes the account position, working SL and PT orders
		/// will be canceled.
		///
		
//		private bool DoOnceLong;
//		private bool DoOnceShort;

//		private Order ptLongOrder;

//		private Order slLongOrder;
//		private Order ptShortOrder;

//		private Order slShortOrder;
		
	    ///
		/// You can manually cancel the orders on chart, and it won't disable strategy.
		/// 
		/// Written By Alan Palmer.
		/// </summary>
        
		#region --- ATM Strategy dropdown variables ---
		private string  atmStrategyId			= string.Empty;
		private string  orderId					= string.Empty;
		private bool	isAtmStrategyCreated	= false;
		#endregion
		#endregion

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Trading with Waddah Attar Explosion";
				Name										= "WAETrades2Unlock";
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
				TimeInForce									= TimeInForce.Day;
				TraceOrders									= true;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.ByStrategyPosition;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				
				
//			  	IsAdoptAccountPositionAware = true;		/// By Alan Palmer
                
				ManagedOrder 			= true;
				// To use Unmanaged order methods
//        		IsUnmanaged = !ManagedOrder;
				
				#region Strategy Variables
				Profit					= 300;
				Stop					= 50;
				Contracts				= 1;
				lookbackPeriod			= 1;
				allowHighRiskProfit		= false;		/// change it to 'true' if you do not have trailing drawdown.
				
				setSLTP 				= false;
				#endregion
				
				#region WAE Indicator variables
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
				#endregion
				
				AtmStrategy				= @"ATM 1";
			}
			else if (State == State.Configure)
			{
//				DoOnceLong 		=	false;
//				DoOnceShort 	=	false;
		
//				ptLongOrder 	=	null;
		
//				slLongOrder 	=	null;
//				ptShortOrder 	=	null;
		
//				slShortOrder 	=	null;
			}
			else if (State == State.DataLoaded)
			{
				ClearOutputWindow();
				WAE				= WAE_Mod(Close, Convert.ToInt32(WAESensitivity), Convert.ToInt32(WAEFastLength), true, Convert.ToInt32(WAEFastSmoothLength), Convert.ToInt32(WAESlowLength), true, Convert.ToInt32(WAESlowSmoothLength), Convert.ToInt32(WAEChannelLength), WAEMult, WAEDeadZone);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < BarsRequiredToTrade)
				return;
			
			// Make sure this strategy does not execute against historical data 
			if(State == State.Historical)		/// ATM Strategy requirement
				return;

			 // Set 1
			Print("Inside checking SET 1.");
			
			#region --- Trade Entry Logic Board ---
			
			#region --- Long Entry ---
			WAELong1	= (CrossAbove(WAE.TrendUp, WAE.ExplosionLine, 1));	/// 1. TrendUp exits Explosion - best entry
			WAELong2	= ( /// 2. Above ZeroLine: TrendUp : 	Trend increasing, explosion increasing - getting away from zeroLine,
							/// 						   		TrendUp must be outside Explosion
								(WAE.ExplosionLine[0] > WAE.ExplosionLine[1])
				 			&& (WAE.TrendUp[0] > WAE.TrendUp[1])
							&& (WAE.TrendUp[0] > WAE.ExplosionLine[0] ) ); 
			WAELong3	= ( /// 3. Below ZeroLine: TrendDown : Trend decreasing, explosion decreasing - getting closer to zeroLine, 
							///								 	TrendDown has to be outside Explosion
								(WAE.ExplosionLineDn[0] > WAE.ExplosionLineDn[1])
				 			&& 	(WAE.TrendDown[0] > WAE.TrendDown[1])
							&& 	(WAE.TrendDown[0] < WAE.ExplosionLineDn[0]) );
			
			currentLongEntry = WAELong1 || WAELong2 || WAELong3 ;
			#endregion
			
			#region --- Short entry ---
			WAEShort1	= (CrossBelow(WAE.TrendDown, WAE.ExplosionLineDn, 1));	/// 1. TrendDown exits Explosion - best entry 
			WAEShort2	= ( /// 2. Below ZeroLine: TrendDown : Trend increasing, explosion increasing - getting away from zeroLine,
							/// 						   		TrenDown must be outside Explosion
								(WAE.ExplosionLineDn[0] < WAE.ExplosionLineDn[1])
				 			&& 	(WAE.TrendDown[0] < WAE.TrendDown[1])
							&& 	(WAE.TrendDown[0] < WAE.ExplosionLineDn[0]) ); 
			WAEShort3	= ( /// 3. Above ZeroLine: TrendUp : Trend decreasing, explosion decreasing - getting closer to zeroLine, 
							///								  TrendUp has to be outside Explosion
								(WAE.ExplosionLine[0] > WAE.ExplosionLine[1])
				 			&& 	(WAE.TrendUp[0] < WAE.TrendUp[1])
							&& 	(WAE.TrendUp[0] > WAE.ExplosionLine[0]) );
			
			currentShortEntry	= WAEShort1 || WAEShort2 || WAEShort3;
			#endregion
			
			#endregion
			
			#region --- Trade Exit Logics ---
			WAELongExit		= (WAE.ExplosionLine[0] <= WAE.ExplosionLine[1])
					 			&& (WAE.TrendUp[0] < WAE.TrendUp[1]);
			
			WAEShortExit 	= (WAE.ExplosionLineDn[0] >= WAE.ExplosionLineDn[1])
					 			&& (WAE.TrendDown[0] > WAE.TrendDown[1]);
			#endregion
			
			#region --- ATM Strategy implementation ---
//			// Submits an entry limit order at the current low price to initiate an ATM Strategy if both order id and strategy id are in a reset state
//			// **** YOU MUST HAVE AN ATM STRATEGY TEMPLATE NAMED 'AtmStrategyTemplate' CREATED IN NINJATRADER (SUPERDOM FOR EXAMPLE) FOR THIS TO WORK ****
//			if (orderId.Length == 0 && atmStrategyId.Length == 0 && Close[0] > Open[0])
//			{
//				isAtmStrategyCreated = false;  // reset atm strategy created check to false
//				atmStrategyId = GetAtmStrategyUniqueId();
//				orderId = GetAtmStrategyUniqueId();
//				AtmStrategyCreate(OrderAction.Buy, OrderType.Limit, Low[0], 0, TimeInForce.Day, orderId, AtmStrategy, atmStrategyId, (atmCallbackErrorCode, atmCallBackId) => {
//					//check that the atm strategy create did not result in error, and that the requested atm strategy matches the id in callback
//					if (atmCallbackErrorCode == ErrorCode.NoError && atmCallBackId == atmStrategyId)
//						isAtmStrategyCreated = true;
//				});
//			}

//			// Check that atm strategy was created before checking other properties
//			if (!isAtmStrategyCreated)
//				return;

//			// Check for a pending entry order
//			if (orderId.Length > 0)
//			{
//				string[] status = GetAtmStrategyEntryOrderStatus(orderId);

//				// If the status call can't find the order specified, the return array length will be zero otherwise it will hold elements
//				if (status.GetLength(0) > 0)
//				{
//					// Print out some information about the order to the output window
//					Print("The entry order average fill price is: " + status[0]);
//					Print("The entry order filled amount is: " + status[1]);
//					Print("The entry order order state is: " + status[2]);

//					// If the order state is terminal, reset the order id value
//					if (status[2] == "Filled" || status[2] == "Cancelled" || status[2] == "Rejected")
//						orderId = string.Empty;
//				}
//			} // If the strategy has terminated reset the strategy id
//			else if (atmStrategyId.Length > 0 && GetAtmStrategyMarketPosition(atmStrategyId) == Cbi.MarketPosition.Flat)
//				atmStrategyId = string.Empty;

//			if (atmStrategyId.Length > 0)
//			{
//				// You can change the stop price
//				if (GetAtmStrategyMarketPosition(atmStrategyId) != MarketPosition.Flat)
//					AtmStrategyChangeStopTarget(0, Low[0] - 3 * TickSize, "STOP1", atmStrategyId);

//				// Print some information about the strategy to the output window, please note you access the ATM strategy specific position object here
//				// the ATM would run self contained and would not have an impact on your NinjaScript strategy position and PnL
//				Print("The current ATM Strategy market position is: " + GetAtmStrategyMarketPosition(atmStrategyId));
//				Print("The current ATM Strategy position quantity is: " + GetAtmStrategyPositionQuantity(atmStrategyId));
//				Print("The current ATM Strategy average price is: " + GetAtmStrategyPositionAveragePrice(atmStrategyId));
//				Print("The current ATM Strategy Unrealized PnL is: " + GetAtmStrategyUnrealizedProfitLoss(atmStrategyId));
//			}
			#endregion
			
			// Set 1 & 2 : long OR short entry
			if ((Position.MarketPosition == MarketPosition.Flat) && (State == State.Realtime))	/// If not in any trade, enter a trade, if any one of the 3 conditions is true.
			{
				// Set 1
				if	(currentLongEntry)
				{
					PlaceMyOrder(myLong);
					setSLTP = true;
				}
				
				// Set 2
				else if (currentShortEntry)
				{
					PlaceMyOrder(myShort);
					setSLTP = true;
				}
			}
			
			 // Set 3 : Long exit
			else if (	(Position.MarketPosition == MarketPosition.Long) && WAELongExit		)
				{
					PlaceMyOrder(myLExit);
				}
			
			 // Set 4 : Short exit
			else if (	(Position.MarketPosition == MarketPosition.Short) && WAEShortExit	)
				{
					PlaceMyOrder(mySExit);
				}
			
				
			#region // Set SL TP
			if ((Position.MarketPosition == MarketPosition.Long)
				 && (setSLTP == true))
			{
				ExitLongLimit(Convert.ToInt32(DefaultQuantity), (Position.AveragePrice + (10 * TickSize)) , @"WAELongTarget", @"WAELongEntry");
				ExitLongStopLimit(Convert.ToInt32(DefaultQuantity), 0, (Position.AveragePrice + (-10 * TickSize)) , @"WAELongStop", @"WAELongEntry");
				
				setSLTP = false;
			}
							
			 // Set 3
			if ((Position.MarketPosition == MarketPosition.Short)
				 && (setSLTP == true))
			{
				ExitShortStopLimit(Convert.ToInt32(DefaultQuantity), 0, (Position.AveragePrice + (10 * TickSize)) , @"WAEShortStop", @"WAEShortEntry");
				ExitShortLimit(Convert.ToInt32(DefaultQuantity), (Position.AveragePrice + (-10 * TickSize)) , @"WAEShortTarget", @"WAEShortEntry");
				
				setSLTP = false;
			}
			#endregion
			
			#region /// From Alan Palmer's AdoptAccountPositionSubmitProtectiveSLPTOrders
//			if(State == State.Historical) return;

//			PrintMarketPosition(State.ToString() + " - " + PositionsAccount[0].Quantity.ToString());
//			PrintMarketPosition(PositionsAccount[0].MarketPosition.ToString());
			
//			///If account position is long upon starting strategy, submit a PT and SL order for the open position.
//			if(PositionsAccount[0].MarketPosition == MarketPosition.Long && DoOnceLong ==false)
//			{
//				PrintMarketPosition("Position is long");
//				ExitLongLimit(0, true,  PositionsAccount[0].Quantity, Close[0]*1.01,"LongLimitPT", "");
//				ExitLongStopMarket(0, true, PositionsAccount[0].Quantity, Close[0]*.99, "StopForLong", "");
//				DoOnceLong =true;
//			}
			
//			///If account position is short upon starting strategy, submit a PT and SL order for the open position.
//			if(PositionsAccount[0].MarketPosition ==  MarketPosition.Short && DoOnceShort ==false)
//			{
//				PrintMarketPosition("Position is short");
	
//				ExitShortLimit(0, true,  PositionsAccount[0].Quantity, Close[0]*.99,"ShortLimitPT", "");  //Submit PT Limit order for open position
//				ExitShortStopMarket(0, true, PositionsAccount[0].Quantity, Close[0]*1.01, "StopForShort", ""); //Submit SL order for open position
//				DoOnceShort =true;
//			}
			
//			 ///Should 1 SL or PT or manual order close the position, then need to cancel orders.
//			if(PositionsAccount[0].MarketPosition ==  MarketPosition.Flat) 
//			{
//					PrintMarketPosition("Cancel all orders");
	
//					if(ptLongOrder != null); //Checking that order object is not null before canceling orders.
//					{
//						CancelOrder(ptLongOrder);  //Cancel ptOrder since we are now flat.
//						ptLongOrder=null;  //Setting order objects back to null.
//					}
					
//					if(slLongOrder != null);
//					{
//						CancelOrder(slLongOrder);
//						slLongOrder=null;
//					}	
//					if(ptShortOrder != null);
//					{
//						CancelOrder(ptShortOrder);
//						ptShortOrder=null;
//					}
//					if(slShortOrder != null);
//					{
//						CancelOrder(slShortOrder);
//						slShortOrder=null;
//					}		
//			}
			#endregion
		}
		
		#region /// From Alan Palmer's AdoptAccountPositionSubmitProtectiveSLPTOrders
//		protected override void OnOrderUpdate(Order order, double limitPrice, double stopPrice, int quantity, int filled,  double averageFillPrice, OrderState orderState, DateTime time, ErrorCode error, string nativeError)
//		{
//			//Assiging order objects to SL and PT for the purpose of canceling orders if the position becomes flat.
//			if (order.Name == "LongLimitPT" && orderState != OrderState.Working)
//     			 ptLongOrder = order;
			 
//			if (order.Name == "StopForLong" && orderState != OrderState.Accepted )
//				  slLongOrder = order;
			  
			  
//			if (order.Name == "ShortLimitPT" && orderState != OrderState.Working)
//     			 ptShortOrder = order;
			 
//			if (order.Name == "StopForShort" && orderState != OrderState.Accepted)
//				  slShortOrder = order;
			
//		}
		#endregion
		
		#region Order entry & Auxilliary Routines
		/// <summary>
		///  Numan: These routine are repeated. So they are put together.
		/// </summary>
		public void PrintMarketPosition(string msg)
		{
			Print(Time[0] + ": (" + msg + ") Market position is : " + Position.MarketPosition);
		}
		
		private void PlaceMyOrder(string TradeType)
		{
			switch (TradeType) {
				
				case myLong:		 // Enter Long
					PrintMarketPosition("Enter Long");
					EnterLong(Convert.ToInt32(DefaultQuantity), @"WAELongEntry");
					Draw.Line(this, @"WAETrade2 Long", false, 5, Close[0], 0, Close[0], Brushes.CornflowerBlue, DashStyleHelper.Dash, 2);
					break;
					
				case myShort:		 // Enter Short
					PrintMarketPosition("Enter Short");
					EnterShort(Convert.ToInt32(DefaultQuantity), @"WAEShortEntry");
					Draw.Line(this, @"WAETrade2 Short", false, 5, Close[0], 0, Close[0], Brushes.Brown, DashStyleHelper.Dash, 2);
					break;
					
				case myLExit:		 // Exit Long
					PrintMarketPosition("Exit Long");
					ExitLong(@"WAELongEntry");
					Draw.Line(this, @"WAETrade2 Line_3", false, 5, Close[0], 0, Close[0], Brushes.LimeGreen, DashStyleHelper.Dash, 2);
					break;
					
				case mySExit:		 // Exit Short
					PrintMarketPosition("Exit Short");
					ExitShort(@"WAEShortEntry");
					Draw.Line(this, @"WAETrade2 Line_4", false, 5, Close[0], 0, Close[0], Brushes.PeachPuff, DashStyleHelper.Dash, 2);					
					break;
					
				default:
					PrintMarketPosition("*** WRONG CALL ***");
					break;
			}
		}
		
		#endregion

		#region Properties
		
		#region Trading Parameters
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Profit", Description="Take Profit in ticks", Order=1, GroupName="Trading Params")]
		public int Profit
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Stop", Description="StopLoss in ticks", Order=2, GroupName="Trading Params")]
		public int Stop
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Contracts", Description="Number of contracts", Order=3, GroupName="Trading Params")]
		public int Contracts
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Allow High Risk Profit?", Description="If you do not have trailing drawdowns", Order=4, GroupName="Trading Params")]
		public bool allowHighRiskProfit
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Trading Managed Order?", Description="Advanced: Do not change!", Order=5, GroupName="Trading Params")]
		public bool ManagedOrder
		{ get; set; }
		#endregion

		#region Waddah Attar Explosion Parameters
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAESensitivity", Description="Waddah Attar Explosion Sensitivity", Order=1, GroupName="WAE Parameters")]
		public int WAESensitivity
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAEFastLength", Description="WAE Fast MA Length", Order=2, GroupName="WAE Parameters")]
		public int WAEFastLength
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAEFastSmoothLength", Description="WAE Fast MA Smoothing Length", Order=3, GroupName="WAE Parameters")]
		public int WAEFastSmoothLength
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="WAEFastSmooth", Description="WAE Fast MA Smoothing", Order=4, GroupName="WAE Parameters")]
		public bool WAEFastSmooth
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAESlowLength", Description="WAE Slow MA Length", Order=5, GroupName="WAE Parameters")]
		public int WAESlowLength
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAESlowSmoothLength", Description="WAE Slow MA Smoothing Length", Order=6, GroupName="WAE Parameters")]
		public int WAESlowSmoothLength
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="WAESlowSmooth", Description="WAE Slow MA Smoothing", Order=7, GroupName="WAE Parameters")]
		public bool WAESlowSmooth
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAEChannelLength", Description="Band Channel Length", Order=8, GroupName="WAE Parameters")]
		public int WAEChannelLength
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.001, double.MaxValue)]
		[Display(Name="WAEMult", Description="WAE StDev. Multiplier", Order=9, GroupName="WAE Parameters")]
		public double WAEMult
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="WAEDeadZone", Description="WAE DeadZone Value", Order=10, GroupName="WAE Parameters")]
		public int WAEDeadZone
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Lookback Period", Description="Number of bars to lookback for crossing", Order=11, GroupName="WAE Parameters")]
		public int lookbackPeriod
		{ get; set; }
		#endregion

		#region ATM Dropdown Option
		[NinjaScriptProperty]
		[Display(Name="Use ATM Strategy ?", Description="If you like to use ATM an strategy for controlling the SL & TP", Order=1, GroupName="Atm Strategy")]
		public bool UseATMStrategy
		{ get; set; }
		
		[TypeConverter(typeof(myAtmConverter))] // Converts the found ATM template file names to string values
		[PropertyEditor("NinjaTrader.Gui.Tools.StringStandardValuesEditorKey")] // Create the combo box on the property grid
		[Display(Name = "Atm Strategy", Order = 2, GroupName = "Atm Strategy")]
		public string AtmStrategy
		{ get; set; }
		#endregion
		
		#endregion

	}
		
	#region ATM Strategy Typeconversion class routine
	public class myAtmConverter : TypeConverter /// Original Name: FriendlyAtmConverter : TypeConverter
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
