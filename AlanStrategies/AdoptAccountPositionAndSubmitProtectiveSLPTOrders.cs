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
namespace NinjaTrader.NinjaScript.Strategies.AlanStrategies
{
	public class AdoptAccountPositionAndSubmitProtectiveSLPTOrders : Strategy
	{
		
		private bool DoOnceLong 	=	false;
		private bool DoOnceShort 	=	false;

		private Order ptLongOrder 	=	null;

		private Order slLongOrder 	=	null;
		private Order ptShortOrder 	=	null;

		private Order slShortOrder 	=	null;
		
	    /// <summary>
        /// This strategy will adapt the current account position and then submit a PT limit order and SL Stop order.
		/// Upon either the PT or SL or even a manual order which closes the account position, working SL and PT orders
		/// will be canceled.
		///
		/// You can manually cancel the orders on chart, and it won't disable strategy.
		/// 
		/// Written By Alan Palmer.
		/// </summary>
        

		protected override void OnStateChange()
		{

	
            
            if (State == State.SetDefaults)
            {
                Description                                 = @"Enter the description for your new custom Strategy here.";
                Name                                        = "AdoptAccountPositionAndSubmitProtectiveSLPTOrders";
                Calculate                                   = Calculate.OnBarClose;
                EntriesPerDirection                         = 9;
                EntryHandling                               = EntryHandling.UniqueEntries;
                IsExitOnSessionCloseStrategy                = false;
                ExitOnSessionCloseSeconds                   = 30;
                IsFillLimitOnTouch                          = false;
                MaximumBarsLookBack                         = MaximumBarsLookBack.TwoHundredFiftySix;
                OrderFillResolution                         = OrderFillResolution.Standard;
                Slippage                                    = 0;
                StartBehavior                               = StartBehavior.AdoptAccountPosition;
                TimeInForce                                 = TimeInForce.Gtc;
                TraceOrders                                 = false;
                RealtimeErrorHandling                       = RealtimeErrorHandling.IgnoreAllErrors;
                StopTargetHandling                          = StopTargetHandling.PerEntryExecution;
                BarsRequiredToTrade                         = 0;
                // Disable this property for performance gains in Strategy Analyzer optimizations
                // See the Help Guide for additional information
                IsInstantiatedOnEachOptimizationIteration   = true;
				
				
			  	IsAdoptAccountPositionAware = true;
                
			
            }
            else if (State == State.Configure)
           {

        	}
		}



        
        protected override void OnBarUpdate()
        {		
			if(State ==State.Historical) return;

			Print(State.ToString()+PositionsAccount[0].Quantity.ToString());
			Print(PositionsAccount[0].MarketPosition.ToString());
			
			///If account position is long upon starting strategy, submit a PT and SL order for the open position.
			if(PositionsAccount[0].MarketPosition == MarketPosition.Long && DoOnceLong ==false)
			{
				Print("Position is long");
				ExitLongLimit(0, true,  PositionsAccount[0].Quantity, Close[0]*1.01,"LongLimitPT", "");
				ExitLongStopMarket(0, true, PositionsAccount[0].Quantity, Close[0]*.99, "StopForLong", "");
				DoOnceLong =true;
			}
			
			///If account position is short upon starting strategy, submit a PT and SL order for the open position.
			if(PositionsAccount[0].MarketPosition ==  MarketPosition.Short && DoOnceShort ==false)
			{
				Print("Position is short");
	
				ExitShortLimit(0, true,  PositionsAccount[0].Quantity, Close[0]*.99,"ShortLimitPT", "");  //Submit PT Limit order for open position
				ExitShortStopMarket(0, true, PositionsAccount[0].Quantity, Close[0]*1.01, "StopForShort", ""); //Submit SL order for open position
				DoOnceShort =true;
			}
			
			 ///Should 1 SL or PT or manual order close the position, then need to cancel orders.
			if(PositionsAccount[0].MarketPosition ==  MarketPosition.Flat) 
			{
					Print("Cancel all orders");
	
					if(ptLongOrder != null); //Checking that order object is not null before canceling orders.
					{
						CancelOrder(ptLongOrder);  //Cancel ptOrder since we are now flat.
						ptLongOrder=null;  //Setting order objects back to null.
					}
					
					if(slLongOrder != null);
					{
						CancelOrder(slLongOrder);
						slLongOrder=null;
					}	
					if(ptShortOrder != null);
					{
						CancelOrder(ptShortOrder);
						ptShortOrder=null;
					}
					if(slShortOrder != null);
					{
						CancelOrder(slShortOrder);
						slShortOrder=null;
					}		
			}
	    }	
		
		protected override void OnOrderUpdate(Order order, double limitPrice, double stopPrice, int quantity, int filled,  double averageFillPrice, OrderState orderState, DateTime time, ErrorCode error, string nativeError)
		{
			//Assiging order objects to SL and PT for the purpose of canceling orders if the position becomes flat.
			if (order.Name == "LongLimitPT" && orderState != OrderState.Working)
     			 ptLongOrder = order;
			 
			if (order.Name == "StopForLong" && orderState != OrderState.Accepted )
				  slLongOrder = order;
			  
			  
			if (order.Name == "ShortLimitPT" && orderState != OrderState.Working)
     			 ptShortOrder = order;
			 
			if (order.Name == "StopForShort" && orderState != OrderState.Accepted)
				  slShortOrder = order;
			
		}
	}
}
