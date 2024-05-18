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
	public class SimpleWAEentryV1A : Strategy
	{
		private bool PossibleLong;
		private bool PossibleShort;

		private NinjaTrader.NinjaScript.Indicators.Numan.WAE_Mod WAE_Mod1;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Version 01A: Limit order Exit with PT added. Long Entry on Waddah Attar Explosion of trend bar exploding over the explosion line. Opposite is true for Short Entry.  Best works with WAE-Sensitivity of 300. For large quantities it is better to split them into two sets, thus this strategy provides two position-quantity options. The second one can be set to zero if not needed. It does require the modified Waddah Attar Explosion Indicator(WAE_Mod) at the back-end.";
				Name										= "SimpleWAEentryV1A";
				Calculate									= Calculate.OnPriceChange;
				EntriesPerDirection							= 3;
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
				WAE_Sensitivity					= 300;
				PosQty_1					= 1;
				PosQty_2					= 1;
				OrderDelays					= 5;
				LongTradeOn					= true;
				ShortTradeOn					= true;
				LONG_ProfitTarget_1					= 30;
				LONG_ProfitTarget_2					= 200;
				SHORT_ProfitTarget_1					= -30;
				SHORT_ProfitTarget_2					= -200;
				ProfitTriggerON					= true;
				BarsToTrade					= 20;
				PossibleLong					= false;
				PossibleShort					= false;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				WAE_Mod1				= WAE_Mod(Close, Convert.ToInt32(WAE_Sensitivity), 10, true, 9, 30, true, 9, 30, 2, 200);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
				return;

			 // Set 1
			if ((Position.MarketPosition == MarketPosition.Short)
				 // WAE TrendDown>=0
				 && (WAE_Mod1.TrendDown[0] >= 0))
			{
				ExitShort(Convert.ToInt32(PosQty_1), @"ExitShort1", @"Short1");
				ExitShort(Convert.ToInt32(PosQty_2), @"ExitShort2", @"Short2");
			}
			
			 // Set 2
			if ((Position.MarketPosition == MarketPosition.Long)
				 // WAE TrendUp<=0
				 && (WAE_Mod1.TrendUp[0] <= 0))
			{
				ExitLong(Convert.ToInt32(PosQty_2), @"ExitLong2", @"Long2");
				ExitLong(Convert.ToInt32(PosQty_1), @"ExitLong1", @"Long1");
			}
			
			 // Set 3
			if ((LongTradeOn == true)
				 // Market position Flat or Short
				 && ((Position.MarketPosition == MarketPosition.Flat)
				 || (Position.MarketPosition == MarketPosition.Short))
				 && (WAE_Mod1.TrendUp[0] >= WAE_Mod1.TrendUp[1])
				 // Bars Since Last Entry OK
				 && ((BarsSinceEntryExecution(0, "", 0) == -1)
				 || (BarsSinceEntryExecution(0, "", 0) >= OrderDelays))
				 // Bars Since last Exit OK
				 && ((BarsSinceExitExecution(0, "", 0) == -1)
				 || (BarsSinceExitExecution(0, "", 0) >= OrderDelays))
				 && (CurrentBars[0] > BarsToTrade))
			{
				PossibleLong = true;
			}
			
			 // Set 4
			if ((PossibleLong == true)
				 // WAExplosion CrossUp
				 && ((WAE_Mod1.TrendUp[0] > WAE_Mod1.ExplosionLine[0])
				 && (WAE_Mod1.TrendUp[1] < WAE_Mod1.ExplosionLine[1])))
			{
				EnterLong(Convert.ToInt32(PosQty_1), @"Long1");
				EnterLong(Convert.ToInt32(PosQty_2), @"Long2");
			}
			
			 // Set 5
			if ((PossibleLong == true)
				 // WAExplosion CrissCrossUp
				 && ((WAE_Mod1.TrendUp[0] > WAE_Mod1.ExplosionLine[0])
				 && (WAE_Mod1.TrendDown[1] < WAE_Mod1.ExplosionLineDn[1])))
			{
				EnterLong(Convert.ToInt32(PosQty_1), @"Long1");
				EnterLong(Convert.ToInt32(PosQty_2), @"Long2");
			}
			
			 // Set 6
			if ((ShortTradeOn == true)
				 // Market position Flat or Long
				 && ((Position.MarketPosition == MarketPosition.Flat)
				 || (Position.MarketPosition == MarketPosition.Long))
				 && (WAE_Mod1.TrendDown[0] <= WAE_Mod1.TrendDown[1])
				 // Bars Since Last Entry OK
				 && ((BarsSinceEntryExecution(0, "", 0) == -1)
				 || (BarsSinceEntryExecution(0, "", 0) >= OrderDelays))
				 // Bars Since last Exit OK
				 && ((BarsSinceExitExecution(0, "", 0) == -1)
				 || (BarsSinceExitExecution(0, "", 0) >= OrderDelays))
				 && (CurrentBars[0] > BarsToTrade))
			{
				PossibleShort = true;
			}
			
			 // Set 7
			if ((PossibleShort == true)
				 // WAExplosion CrossDn
				 && ((WAE_Mod1.TrendDown[0] < WAE_Mod1.ExplosionLineDn[0])
				 && (WAE_Mod1.TrendDown[1] > WAE_Mod1.ExplosionLineDn[1])))
			{
				EnterShort(Convert.ToInt32(PosQty_1), @"Short1");
				EnterShort(Convert.ToInt32(PosQty_2), @"Short2");
			}
			
			 // Set 8
			if ((PossibleShort == true)
				 // WAExplosion CrissCrossDn
				 && ((WAE_Mod1.TrendDown[0] < WAE_Mod1.ExplosionLineDn[0])
				 && (WAE_Mod1.TrendUp[1] > WAE_Mod1.ExplosionLine[1])))
			{
				EnterShort(Convert.ToInt32(PosQty_1), @"Short1");
				EnterShort(Convert.ToInt32(PosQty_2), @"Short2");
			}
			
			 // Set 9
			if ((Position.MarketPosition == MarketPosition.Short)
				 && (ProfitTriggerON == true)
				 && (Close[0] <= (Position.AveragePrice + (SHORT_ProfitTarget_1 * TickSize)) ))
			{
				ExitShortLimit(Convert.ToInt32(PosQty_1), (Position.AveragePrice + (SHORT_ProfitTarget_1 * TickSize)) , @"Target 1 " + Convert.ToString(CurrentBars[0]), @"Short1");
			}
			
			 // Set 10
			if ((Position.MarketPosition == MarketPosition.Short)
				 && (ProfitTriggerON == true)
				 && (Close[0] <= (Position.AveragePrice + (SHORT_ProfitTarget_2 * TickSize)) ))
			{
				ExitShortLimit(Convert.ToInt32(PosQty_2), (Position.AveragePrice + (SHORT_ProfitTarget_2 * TickSize)) , @"Target 2 " + Convert.ToString(CurrentBars[0]), @"Short2");
			}
			
			 // Set 11
			if ((Position.MarketPosition == MarketPosition.Long)
				 && (ProfitTriggerON == true)
				 && (Close[0] >= (Position.AveragePrice + (LONG_ProfitTarget_1 * TickSize)) ))
			{
				ExitLongLimit(Convert.ToInt32(PosQty_1), (Position.AveragePrice + (LONG_ProfitTarget_1 * TickSize)) , @"Target 1 " + Convert.ToString(CurrentBars[0]), @"Long1");
			}
			
			 // Set 12
			if ((Position.MarketPosition == MarketPosition.Long)
				 && (ProfitTriggerON == true)
				 && (Close[0] >= (Position.AveragePrice + (LONG_ProfitTarget_2 * TickSize)) ))
			{
				ExitLongLimit(Convert.ToInt32(PosQty_2), (Position.AveragePrice + (LONG_ProfitTarget_2 * TickSize)) , @"Target 2 " + Convert.ToString(CurrentBars[0]), @"Long2");
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="WAE_Sensitivity", Description="Waddah Attar Explosion sensitivity", Order=1, GroupName="Parameters")]
		public int WAE_Sensitivity
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="PosQty_1", Order=2, GroupName="Parameters")]
		public int PosQty_1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="PosQty_2", Description="Leave zero if only one position is desired.", Order=3, GroupName="Parameters")]
		public int PosQty_2
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="OrderDelays", Description="Number of bars to wait before new order entry.", Order=4, GroupName="Parameters")]
		public int OrderDelays
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="LongTradeOn", Description="Allow Long trades?", Order=5, GroupName="Parameters")]
		public bool LongTradeOn
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="ShortTradeOn", Description="Allow Short trades? ", Order=6, GroupName="Parameters")]
		public bool ShortTradeOn
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="LONG_ProfitTarget_1", Description="First (quantity) profit target (ticks)", Order=7, GroupName="Parameters")]
		public int LONG_ProfitTarget_1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="LONG_ProfitTarget_2", Description="Secong(quantity) profit target (ticks)", Order=8, GroupName="Parameters")]
		public int LONG_ProfitTarget_2
		{ get; set; }

		[NinjaScriptProperty]
		[Range(-99999, int.MaxValue)]
		[Display(Name="SHORT_ProfitTarget_1", Description="Short 1 profit target (ticks-negative value)", Order=9, GroupName="Parameters")]
		public int SHORT_ProfitTarget_1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(-99999, int.MaxValue)]
		[Display(Name="SHORT_ProfitTarget_2", Description="Short2  profit target (ticks-negative value)", Order=10, GroupName="Parameters")]
		public int SHORT_ProfitTarget_2
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="ProfitTriggerON", Description="If profit trigget is off then profit is taken at reversal.", Order=11, GroupName="Parameters")]
		public bool ProfitTriggerON
		{ get; set; }

		[NinjaScriptProperty]
		[Range(3, int.MaxValue)]
		[Display(Name="BarsToTrade", Description="Bars required to trade", Order=12, GroupName="Parameters")]
		public int BarsToTrade
		{ get; set; }
		#endregion

	}
}

#region Wizard settings, neither change nor remove
/*@
<?xml version="1.0"?>
<ScriptProperties xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Calculate>OnPriceChange</Calculate>
  <ConditionalActions>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit short position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Short1</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T04:41:05.8045195</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ExitShort1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit short position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-07T04:40:48.7353783</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Exit</ActionType>
          <Command>
            <Command>ExitShort</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit short position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Short2</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T04:41:45.4103428</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ExitShort2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit short position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-07T04:41:32.2561447</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Exit</ActionType>
          <Command>
            <Command>ExitShort</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T08:32:47.5456351</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T08:32:47.5606355</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Short</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Short</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>All</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T09:57:11.6081431</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendDown</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:56:47.3919354</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:56:47.3989345</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>WAE TrendDown&gt;=0</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 1</SetName>
      <SetNumber>1</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Long2</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T08:39:35.8030143</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ExitLong2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-03T08:39:27.6098903</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Exit</ActionType>
          <Command>
            <Command>ExitLong</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Long1</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T08:39:04.5832976</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ExitLong1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit long position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-03T08:38:50.4491706</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Exit</ActionType>
          <Command>
            <Command>ExitLong</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T08:38:18.0954506</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T08:38:18.1114513</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Long</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>All</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T10:01:16.4656431</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendUp</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T10:00:56.9988336</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>LessEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T10:00:57.0063507</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>WAE TrendUp&lt;=0</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 2</SetName>
      <SetNumber>2</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set PossibleLong</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T13:03:27.7103916</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-03T13:03:27.7103916</VariableDateTime>
            <VariableBool>true</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>PossibleLong</VariableName>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>LongTradeOn</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>LongTradeOn</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:51:38.4682731</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:51:38.4847893</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>LongTradeOn = true</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:42:00.9549853</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:42:00.9739832</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Flat</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:42:15.2353156</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:42:15.2413155</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Short</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Market position Flat or Short</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T10:38:39.2880621</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendUp</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T10:38:28.6971706</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T10:39:17.848767</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendUp</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T10:38:28.7171707</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>WAE_Mod(WAE_Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendUp[0] &gt;= WAE_Mod(WAE_Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendUp[1]</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bars since entry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarsSinceEntryExecution({0}, {1}, {2})</Command>
                  <Parameters>
                    <string>BarsInProgress</string>
                    <string>SignalName</string>
                    <string>EntryExecutionsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <BarsInProgress>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>true</IsInt>
                  <BindingValue xsi:type="xsd:string">0</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">0</LiveValue>
                </BarsInProgress>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:20:01.9763396</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:20:01.982848</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">-1</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">-1</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bars since entry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarsSinceEntryExecution({0}, {1}, {2})</Command>
                  <Parameters>
                    <string>BarsInProgress</string>
                    <string>SignalName</string>
                    <string>EntryExecutionsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <BarsInProgress>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>true</IsInt>
                  <BindingValue xsi:type="xsd:string">0</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">0</LiveValue>
                </BarsInProgress>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:20:59.4727761</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:20:59.4897749</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">OrderDelays</BindingValue>
                  <DynamicValue>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>OrderDelays</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>OrderDelays</Command>
                      <Parameters />
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-05-03T09:39:13.938262</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>false</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Number</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </DynamicValue>
                  <IsLiteral>false</IsLiteral>
                  <LiveValue xsi:type="xsd:string">OrderDelays</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Bars Since Last Entry OK</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bars since exit</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarsSinceExitExecution({0}, {1}, {2})</Command>
                  <Parameters>
                    <string>BarsInProgress</string>
                    <string>SignalName</string>
                    <string>ExitExecutionsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <BarsInProgress>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>true</IsInt>
                  <BindingValue xsi:type="xsd:string">0</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">0</LiveValue>
                </BarsInProgress>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:22:38.6918329</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:22:38.6998275</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">-1</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">-1</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bars since exit</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarsSinceExitExecution({0}, {1}, {2})</Command>
                  <Parameters>
                    <string>BarsInProgress</string>
                    <string>SignalName</string>
                    <string>ExitExecutionsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <BarsInProgress>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>true</IsInt>
                  <BindingValue xsi:type="xsd:string">0</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">0</LiveValue>
                </BarsInProgress>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:23:05.7905197</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:23:05.7985224</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">OrderDelays</BindingValue>
                  <DynamicValue>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>OrderDelays</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>OrderDelays</Command>
                      <Parameters />
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-05-03T09:39:42.9308198</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>false</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Number</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </DynamicValue>
                  <IsLiteral>false</IsLiteral>
                  <LiveValue xsi:type="xsd:string">OrderDelays</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Bars Since last Exit OK</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current bar</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>CurrentBars[{0}]</Command>
                  <Parameters>
                    <string>Series1</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T07:30:34.6207861</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>true</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>BarsToTrade</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarsToTrade</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T07:30:34.684786</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>CurrentBars[Default input] &gt; BarsToTrade</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 3</SetName>
      <SetNumber>3</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter long position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T06:11:44.4034373</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Long1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-03T06:11:42.2341766</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterLong</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter long position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T06:12:19.0965097</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Long2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-03T06:12:16.5241251</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterLong</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PossibleLong</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PossibleLong</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T13:06:25.4923306</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T13:06:25.5093281</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>PossibleLong = true</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>All</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:31:04.0364522</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendUp</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:30:31.3747003</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:32:09.3223965</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>ExplosionLine</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:30:31.3817025</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:33:00.6740143</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendUp</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:32:35.9770656</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Less</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:33:35.0153369</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>ExplosionLine</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:32:35.9830655</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>WAExplosion CrossUp</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 4</SetName>
      <SetNumber>4</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter long position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T06:11:44.4034373</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Long1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-03T06:11:42.2341766</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterLong</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter long position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T06:12:19.0965097</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Long2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-03T06:12:16.5241251</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterLong</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PossibleLong</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PossibleLong</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T13:06:25.4923306</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T13:06:25.5093281</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>PossibleLong = true</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>All</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:31:04.0364522</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendUp</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:30:31.3747003</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:32:09.3223965</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>ExplosionLine</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:30:31.3817025</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:33:00.6740143</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendDown</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:32:35.9770656</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Less</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:33:35.0153369</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>ExplosionLineDn</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:32:35.9830655</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>WAExplosion CrissCrossUp</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 5</SetName>
      <SetNumber>5</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set PossibleShort</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T13:03:42.2679799</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-03T13:03:42.2679799</VariableDateTime>
            <VariableBool>true</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>PossibleShort</VariableName>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ShortTradeOn</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ShortTradeOn</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:51:15.0518772</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:51:15.0698775</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>ShortTradeOn = true</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:42:00.9549853</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:42:00.9739832</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Flat</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:42:15.2353156</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:42:15.2413155</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Market position Flat or Long</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T10:41:51.5736269</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendDown</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T10:41:38.5937904</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>LessEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T10:42:29.9349141</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendDown</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T10:41:38.6067902</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>WAE_Mod(WAE_Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendDown[0] &lt;= WAE_Mod(WAE_Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendDown[1]</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bars since entry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarsSinceEntryExecution({0}, {1}, {2})</Command>
                  <Parameters>
                    <string>BarsInProgress</string>
                    <string>SignalName</string>
                    <string>EntryExecutionsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <BarsInProgress>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>true</IsInt>
                  <BindingValue xsi:type="xsd:string">0</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">0</LiveValue>
                </BarsInProgress>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:20:01.9763396</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:20:01.982848</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">-1</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">-1</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bars since entry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarsSinceEntryExecution({0}, {1}, {2})</Command>
                  <Parameters>
                    <string>BarsInProgress</string>
                    <string>SignalName</string>
                    <string>EntryExecutionsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <BarsInProgress>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>true</IsInt>
                  <BindingValue xsi:type="xsd:string">0</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">0</LiveValue>
                </BarsInProgress>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:20:59.4727761</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:20:59.4897749</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">OrderDelays</BindingValue>
                  <DynamicValue>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>OrderDelays</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>OrderDelays</Command>
                      <Parameters />
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-05-03T09:39:13.938262</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>false</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Number</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </DynamicValue>
                  <IsLiteral>false</IsLiteral>
                  <LiveValue xsi:type="xsd:string">OrderDelays</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Bars Since Last Entry OK</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bars since exit</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarsSinceExitExecution({0}, {1}, {2})</Command>
                  <Parameters>
                    <string>BarsInProgress</string>
                    <string>SignalName</string>
                    <string>ExitExecutionsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <BarsInProgress>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>true</IsInt>
                  <BindingValue xsi:type="xsd:string">0</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">0</LiveValue>
                </BarsInProgress>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:22:38.6918329</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:22:38.6998275</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">-1</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">-1</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bars since exit</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarsSinceExitExecution({0}, {1}, {2})</Command>
                  <Parameters>
                    <string>BarsInProgress</string>
                    <string>SignalName</string>
                    <string>ExitExecutionsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <BarsInProgress>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>true</IsInt>
                  <BindingValue xsi:type="xsd:string">0</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">0</LiveValue>
                </BarsInProgress>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:23:05.7905197</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T09:23:05.7985224</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">OrderDelays</BindingValue>
                  <DynamicValue>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>OrderDelays</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>OrderDelays</Command>
                      <Parameters />
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-05-03T09:39:42.9308198</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>false</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Number</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </DynamicValue>
                  <IsLiteral>false</IsLiteral>
                  <LiveValue xsi:type="xsd:string">OrderDelays</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Bars Since last Exit OK</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current bar</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>CurrentBars[{0}]</Command>
                  <Parameters>
                    <string>Series1</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T07:30:34.6207861</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>true</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>BarsToTrade</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarsToTrade</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T07:30:34.684786</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>CurrentBars[Default input] &gt; BarsToTrade</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 6</SetName>
      <SetNumber>6</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter short position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T06:13:24.3202151</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Short1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Enter short position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-03T06:13:19.0502531</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterShort</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter short position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T06:13:43.7697034</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Short2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-03T06:13:41.1619569</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterShort</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PossibleShort</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PossibleShort</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T13:06:52.5100931</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T13:06:52.5160943</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>PossibleShort = true</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>All</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:35:14.6671892</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendDown</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:35:02.5371869</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Less</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:35:58.8766029</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>ExplosionLineDn</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:35:02.5431877</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:37:00.9132085</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendDown</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:36:47.2698228</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:37:32.0291335</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>ExplosionLineDn</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:36:47.2848213</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>WAExplosion CrossDn</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 7</SetName>
      <SetNumber>7</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter short position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T06:13:24.3202151</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Short1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Enter short position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-03T06:13:19.0502531</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterShort</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter short position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T06:13:43.7697034</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Short2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-03T06:13:41.1619569</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterShort</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PossibleShort</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PossibleShort</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T13:06:52.5100931</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T13:06:52.5160943</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>PossibleShort = true</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>All</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:35:14.6671892</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendDown</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:35:02.5371869</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Less</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:35:58.8766029</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>ExplosionLineDn</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:35:02.5431877</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:37:00.9132085</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>TrendUp</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:36:47.2698228</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WAE_Mod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WAE_Mod</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Sensitivity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">WAE_Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>WAE_Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>WAE_Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-03T12:37:32.0291335</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">WAE_Sensitivity</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowSmoothLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">9</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">9</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">30</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">30</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">200</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">200</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WAE_Mod</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendUp</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF0000&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>2</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>TrendDown</Name>
                        <PlotStyle>Bar</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8A2BE2&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLineDn</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>ExplosionLine</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-03T12:36:47.2848213</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>WAExplosion CrissCrossDn</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 8</SetName>
      <SetNumber>8</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit short position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Short1</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Position.AveragePrice + (SHORT_ProfitTarget_1 * TickSize)) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Average position price</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.AveragePrice</Command>
                  <Parameters>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:15:13.4582593</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">SHORT_ProfitTarget_1</BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>SHORT_ProfitTarget_1</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>SHORT_ProfitTarget_1</Command>
                        <Parameters />
                      </AssignedCommand>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-05-07T05:15:24.1363746</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <Period>0</Period>
                      <ReturnType>Number</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">SHORT_ProfitTarget_1</LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Position.AveragePrice + (SHORT_ProfitTarget_1 * TickSize)) </LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:15:04.4981409</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Target 1</StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Current bar</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>CurrentBars[{0}]</Command>
                      <Parameters>
                        <string>Series1</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-05-07T05:15:49.0553185</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Number</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>CurrentBars[0]</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-07T05:24:23.1612068</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitLimit</ActionType>
          <Command>
            <Command>ExitShortLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Exit short position by a limit order</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <FromEntrySignal>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Short1</StringValue>
              </NinjaScriptString>
            </Strings>
          </FromEntrySignal>
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LimitPrice>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <BindingValue xsi:type="xsd:string">(Position.AveragePrice + (SHORT_ProfitTarget_1 * TickSize)) </BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>Average position price</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>Position.AveragePrice</Command>
                <Parameters>
                  <string>OffsetBuilder</string>
                </Parameters>
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-05-07T05:15:13.4582593</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <OffsetBuilder>
                <ConditionOffset>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Ticks</OffsetType>
                </ConditionOffset>
                <Offset>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">SHORT_ProfitTarget_1</BindingValue>
                  <DynamicValue>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>SHORT_ProfitTarget_1</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>SHORT_ProfitTarget_1</Command>
                      <Parameters />
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-05-07T05:15:24.1363746</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>false</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Number</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </DynamicValue>
                  <IsLiteral>false</IsLiteral>
                  <LiveValue xsi:type="xsd:string">SHORT_ProfitTarget_1</LiveValue>
                </Offset>
              </OffsetBuilder>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">(Position.AveragePrice + (SHORT_ProfitTarget_1 * TickSize)) </LiveValue>
          </LimitPrice>
          <LogLevel>Information</LogLevel>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">PosQty_1</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>PosQty_1</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>PosQty_1</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-05-07T05:15:04.4981409</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">PosQty_1</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SignalName>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Target 1</StringValue>
              </NinjaScriptString>
              <NinjaScriptString>
                <Action>
                  <Children />
                  <IsExpanded>false</IsExpanded>
                  <IsSelected>true</IsSelected>
                  <Name>Current bar</Name>
                  <OffsetType>Arithmetic</OffsetType>
                  <AssignedCommand>
                    <Command>CurrentBars[{0}]</Command>
                    <Parameters>
                      <string>Series1</string>
                    </Parameters>
                  </AssignedCommand>
                  <BarsAgo>0</BarsAgo>
                  <CurrencyType>Currency</CurrencyType>
                  <Date>2024-05-07T05:15:49.0553185</Date>
                  <DayOfWeek>Sunday</DayOfWeek>
                  <EndBar>0</EndBar>
                  <ForceSeriesIndex>true</ForceSeriesIndex>
                  <LookBackPeriod>0</LookBackPeriod>
                  <MarketPosition>Long</MarketPosition>
                  <Period>0</Period>
                  <ReturnType>Number</ReturnType>
                  <StartBar>0</StartBar>
                  <State>Undefined</State>
                  <Time>0001-01-01T00:00:00</Time>
                </Action>
                <Index>1</Index>
                <StringValue>CurrentBars[0]</StringValue>
              </NinjaScriptString>
            </Strings>
          </SignalName>
          <SoundLocation />
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-05-07T05:24:23.1612068</VariableDateTime>
          <VariableBool>false</VariableBool>
        </ActionProperties>
        <ActionType>ExitLimit</ActionType>
        <Command>
          <Command>ExitShortLimit</Command>
          <Parameters>
            <string>quantity</string>
            <string>limitPrice</string>
            <string>signalName</string>
            <string>fromEntrySignal</string>
          </Parameters>
        </Command>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T04:49:47.8833632</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T04:49:47.8983638</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Short</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Short</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ProfitTriggerON</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ProfitTriggerON</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:44:55.9349928</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:44:55.9419916</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>ProfitTriggerON = true</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T04:55:11.5674171</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>LessEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Average position price</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.AveragePrice</Command>
                  <Parameters>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T04:55:11.5744156</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">SHORT_ProfitTarget_1</BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>SHORT_ProfitTarget_1</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>SHORT_ProfitTarget_1</Command>
                        <Parameters />
                      </AssignedCommand>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-05-07T04:59:12.4043232</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <Period>0</Period>
                      <ReturnType>Number</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">SHORT_ProfitTarget_1</LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Default input[0] &lt;= (Position.AveragePrice + (SHORT_ProfitTarget_1 * TickSize)) </DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 9</SetName>
      <SetNumber>9</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit short position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Short2</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Position.AveragePrice + (SHORT_ProfitTarget_2 * TickSize)) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Average position price</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.AveragePrice</Command>
                  <Parameters>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:15:13.4582593</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">SHORT_ProfitTarget_2</BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>SHORT_ProfitTarget_2</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>SHORT_ProfitTarget_2</Command>
                        <Parameters />
                      </AssignedCommand>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-05-07T05:19:57.6188547</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <Period>0</Period>
                      <ReturnType>Number</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">SHORT_ProfitTarget_2</LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Position.AveragePrice + (SHORT_ProfitTarget_2 * TickSize)) </LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:20:05.3644029</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Target 2</StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Current bar</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>CurrentBars[{0}]</Command>
                      <Parameters>
                        <string>Series1</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-05-07T05:15:49.0553185</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Number</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>CurrentBars[0]</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-07T05:24:06.5598392</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitLimit</ActionType>
          <Command>
            <Command>ExitShortLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Exit short position by a limit order</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <FromEntrySignal>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Short2</StringValue>
              </NinjaScriptString>
            </Strings>
          </FromEntrySignal>
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LimitPrice>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <BindingValue xsi:type="xsd:string">(Position.AveragePrice + (SHORT_ProfitTarget_2 * TickSize)) </BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>Average position price</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>Position.AveragePrice</Command>
                <Parameters>
                  <string>OffsetBuilder</string>
                </Parameters>
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-05-07T05:15:13.4582593</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <OffsetBuilder>
                <ConditionOffset>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Ticks</OffsetType>
                </ConditionOffset>
                <Offset>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">SHORT_ProfitTarget_2</BindingValue>
                  <DynamicValue>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>SHORT_ProfitTarget_2</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>SHORT_ProfitTarget_2</Command>
                      <Parameters />
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-05-07T05:19:57.6188547</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>false</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Number</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </DynamicValue>
                  <IsLiteral>false</IsLiteral>
                  <LiveValue xsi:type="xsd:string">SHORT_ProfitTarget_2</LiveValue>
                </Offset>
              </OffsetBuilder>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">(Position.AveragePrice + (SHORT_ProfitTarget_2 * TickSize)) </LiveValue>
          </LimitPrice>
          <LogLevel>Information</LogLevel>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">PosQty_2</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>PosQty_2</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>PosQty_2</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-05-07T05:20:05.3644029</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">PosQty_2</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SignalName>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Target 2</StringValue>
              </NinjaScriptString>
              <NinjaScriptString>
                <Action>
                  <Children />
                  <IsExpanded>false</IsExpanded>
                  <IsSelected>true</IsSelected>
                  <Name>Current bar</Name>
                  <OffsetType>Arithmetic</OffsetType>
                  <AssignedCommand>
                    <Command>CurrentBars[{0}]</Command>
                    <Parameters>
                      <string>Series1</string>
                    </Parameters>
                  </AssignedCommand>
                  <BarsAgo>0</BarsAgo>
                  <CurrencyType>Currency</CurrencyType>
                  <Date>2024-05-07T05:15:49.0553185</Date>
                  <DayOfWeek>Sunday</DayOfWeek>
                  <EndBar>0</EndBar>
                  <ForceSeriesIndex>true</ForceSeriesIndex>
                  <LookBackPeriod>0</LookBackPeriod>
                  <MarketPosition>Long</MarketPosition>
                  <Period>0</Period>
                  <ReturnType>Number</ReturnType>
                  <StartBar>0</StartBar>
                  <State>Undefined</State>
                  <Time>0001-01-01T00:00:00</Time>
                </Action>
                <Index>1</Index>
                <StringValue>CurrentBars[0]</StringValue>
              </NinjaScriptString>
            </Strings>
          </SignalName>
          <SoundLocation />
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-05-07T05:24:06.5598392</VariableDateTime>
          <VariableBool>false</VariableBool>
        </ActionProperties>
        <ActionType>ExitLimit</ActionType>
        <Command>
          <Command>ExitShortLimit</Command>
          <Parameters>
            <string>quantity</string>
            <string>limitPrice</string>
            <string>signalName</string>
            <string>fromEntrySignal</string>
          </Parameters>
        </Command>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T04:49:47.8833632</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T04:49:47.8983638</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Short</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Short</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ProfitTriggerON</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ProfitTriggerON</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:44:55.9349928</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:44:55.9419916</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>ProfitTriggerON = true</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T04:55:11.5674171</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>LessEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Average position price</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.AveragePrice</Command>
                  <Parameters>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T04:55:11.5744156</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">SHORT_ProfitTarget_2</BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>SHORT_ProfitTarget_2</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>SHORT_ProfitTarget_2</Command>
                        <Parameters />
                      </AssignedCommand>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-05-07T05:02:07.5680285</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <Period>0</Period>
                      <ReturnType>Number</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">SHORT_ProfitTarget_2</LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Default input[0] &lt;= (Position.AveragePrice + (SHORT_ProfitTarget_2 * TickSize)) </DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 10</SetName>
      <SetNumber>10</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Long1</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Position.AveragePrice + (LONG_ProfitTarget_1 * TickSize)) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Average position price</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.AveragePrice</Command>
                  <Parameters>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:21:26.0131973</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">LONG_ProfitTarget_1</BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>LONG_ProfitTarget_1</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>LONG_ProfitTarget_1</Command>
                        <Parameters />
                      </AssignedCommand>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-05-07T05:21:32.2997112</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <Period>0</Period>
                      <ReturnType>Number</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">LONG_ProfitTarget_1</LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Position.AveragePrice + (LONG_ProfitTarget_1 * TickSize)) </LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:21:16.6714207</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Target 1</StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Current bar</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>CurrentBars[{0}]</Command>
                      <Parameters>
                        <string>Series1</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-05-07T05:21:55.6243215</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Number</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>CurrentBars[0]</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-07T05:23:46.3624593</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitLimit</ActionType>
          <Command>
            <Command>ExitLongLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Exit long position by a limit order</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <FromEntrySignal>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Long1</StringValue>
              </NinjaScriptString>
            </Strings>
          </FromEntrySignal>
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LimitPrice>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <BindingValue xsi:type="xsd:string">(Position.AveragePrice + (LONG_ProfitTarget_1 * TickSize)) </BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>Average position price</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>Position.AveragePrice</Command>
                <Parameters>
                  <string>OffsetBuilder</string>
                </Parameters>
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-05-07T05:21:26.0131973</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <OffsetBuilder>
                <ConditionOffset>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Ticks</OffsetType>
                </ConditionOffset>
                <Offset>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">LONG_ProfitTarget_1</BindingValue>
                  <DynamicValue>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>LONG_ProfitTarget_1</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>LONG_ProfitTarget_1</Command>
                      <Parameters />
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-05-07T05:21:32.2997112</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>false</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Number</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </DynamicValue>
                  <IsLiteral>false</IsLiteral>
                  <LiveValue xsi:type="xsd:string">LONG_ProfitTarget_1</LiveValue>
                </Offset>
              </OffsetBuilder>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">(Position.AveragePrice + (LONG_ProfitTarget_1 * TickSize)) </LiveValue>
          </LimitPrice>
          <LogLevel>Information</LogLevel>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">PosQty_1</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>PosQty_1</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>PosQty_1</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-05-07T05:21:16.6714207</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">PosQty_1</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SignalName>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Target 1</StringValue>
              </NinjaScriptString>
              <NinjaScriptString>
                <Action>
                  <Children />
                  <IsExpanded>false</IsExpanded>
                  <IsSelected>true</IsSelected>
                  <Name>Current bar</Name>
                  <OffsetType>Arithmetic</OffsetType>
                  <AssignedCommand>
                    <Command>CurrentBars[{0}]</Command>
                    <Parameters>
                      <string>Series1</string>
                    </Parameters>
                  </AssignedCommand>
                  <BarsAgo>0</BarsAgo>
                  <CurrencyType>Currency</CurrencyType>
                  <Date>2024-05-07T05:21:55.6243215</Date>
                  <DayOfWeek>Sunday</DayOfWeek>
                  <EndBar>0</EndBar>
                  <ForceSeriesIndex>true</ForceSeriesIndex>
                  <LookBackPeriod>0</LookBackPeriod>
                  <MarketPosition>Long</MarketPosition>
                  <Period>0</Period>
                  <ReturnType>Number</ReturnType>
                  <StartBar>0</StartBar>
                  <State>Undefined</State>
                  <Time>0001-01-01T00:00:00</Time>
                </Action>
                <Index>1</Index>
                <StringValue>CurrentBars[0]</StringValue>
              </NinjaScriptString>
            </Strings>
          </SignalName>
          <SoundLocation />
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-05-07T05:23:46.3624593</VariableDateTime>
          <VariableBool>false</VariableBool>
        </ActionProperties>
        <ActionType>ExitLimit</ActionType>
        <Command>
          <Command>ExitLongLimit</Command>
          <Parameters>
            <string>quantity</string>
            <string>limitPrice</string>
            <string>signalName</string>
            <string>fromEntrySignal</string>
          </Parameters>
        </Command>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:04:22.9507432</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:04:22.9577437</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Long</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ProfitTriggerON</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ProfitTriggerON</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:44:55.9349928</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:44:55.9419916</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>ProfitTriggerON = true</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:04:35.6305453</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Average position price</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.AveragePrice</Command>
                  <Parameters>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:04:35.6435448</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">LONG_ProfitTarget_1</BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>LONG_ProfitTarget_1</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>LONG_ProfitTarget_1</Command>
                        <Parameters />
                      </AssignedCommand>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-05-07T05:04:54.2676984</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <Period>0</Period>
                      <ReturnType>Number</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">LONG_ProfitTarget_1</LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Default input[0] &gt;= (Position.AveragePrice + (LONG_ProfitTarget_1 * TickSize)) </DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 11</SetName>
      <SetNumber>11</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Long2</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Position.AveragePrice + (LONG_ProfitTarget_2 * TickSize)) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Average position price</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.AveragePrice</Command>
                  <Parameters>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:21:26.0131973</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">LONG_ProfitTarget_2</BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>LONG_ProfitTarget_2</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>LONG_ProfitTarget_2</Command>
                        <Parameters />
                      </AssignedCommand>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-05-07T05:23:00.6423541</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <Period>0</Period>
                      <ReturnType>Number</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">LONG_ProfitTarget_2</LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Position.AveragePrice + (LONG_ProfitTarget_2 * TickSize)) </LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">PosQty_2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>PosQty_2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>PosQty_2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:23:07.9606804</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">PosQty_2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Target 2</StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Current bar</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>CurrentBars[{0}]</Command>
                      <Parameters>
                        <string>Series1</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-05-07T05:21:55.6243215</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Number</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>CurrentBars[0]</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-07T05:22:50.836545</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitLimit</ActionType>
          <Command>
            <Command>ExitLongLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:04:22.9507432</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:04:22.9577437</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Long</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ProfitTriggerON</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ProfitTriggerON</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:44:55.9349928</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:44:55.9419916</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>ProfitTriggerON = true</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:04:35.6305453</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Average position price</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.AveragePrice</Command>
                  <Parameters>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-07T05:04:35.6435448</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">LONG_ProfitTarget_2</BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>LONG_ProfitTarget_2</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>LONG_ProfitTarget_2</Command>
                        <Parameters />
                      </AssignedCommand>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-05-07T05:07:15.3726875</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <Period>0</Period>
                      <ReturnType>Number</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">LONG_ProfitTarget_2</LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Default input[0] &gt;= (Position.AveragePrice + (LONG_ProfitTarget_2 * TickSize)) </DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 12</SetName>
      <SetNumber>12</SetNumber>
    </ConditionalAction>
  </ConditionalActions>
  <CustomSeries />
  <DataSeries />
  <Description>Version 01A: Limit order Exit with PT added. Long Entry on Waddah Attar Explosion of trend bar exploding over the explosion line. Opposite is true for Short Entry.  Best works with WAE-Sensitivity of 300. For large quantities it is better to split them into two sets, thus this strategy provides two position-quantity options. The second one can be set to zero if not needed. It does require the modified Waddah Attar Explosion Indicator("WAE_Mod") at the back-end.</Description>
  <DisplayInDataBox>true</DisplayInDataBox>
  <DrawHorizontalGridLines>true</DrawHorizontalGridLines>
  <DrawOnPricePanel>true</DrawOnPricePanel>
  <DrawVerticalGridLines>true</DrawVerticalGridLines>
  <EntriesPerDirection>3</EntriesPerDirection>
  <EntryHandling>AllEntries</EntryHandling>
  <ExitOnSessionClose>true</ExitOnSessionClose>
  <ExitOnSessionCloseSeconds>30</ExitOnSessionCloseSeconds>
  <FillLimitOrdersOnTouch>false</FillLimitOrdersOnTouch>
  <InputParameters>
    <InputParameter>
      <Default>300</Default>
      <Description>Waddah Attar Explosion sensitivity</Description>
      <Name>WAE_Sensitivity</Name>
      <Minimum>0</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>1</Default>
      <Description />
      <Name>PosQty_1</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>1</Default>
      <Description>Leave zero if only one position is desired.</Description>
      <Name>PosQty_2</Name>
      <Minimum>0</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>5</Default>
      <Description>Number of bars to wait before new order entry.</Description>
      <Name>OrderDelays</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>true</Default>
      <Description>Allow Long trades?</Description>
      <Name>LongTradeOn</Name>
      <Minimum />
      <Type>bool</Type>
    </InputParameter>
    <InputParameter>
      <Default>true</Default>
      <Description>Allow Short trades? </Description>
      <Name>ShortTradeOn</Name>
      <Minimum />
      <Type>bool</Type>
    </InputParameter>
    <InputParameter>
      <Default>30</Default>
      <Description>First (quantity) profit target (ticks)</Description>
      <Name>LONG_ProfitTarget_1</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>200</Default>
      <Description>Secong(quantity) profit target (ticks)</Description>
      <Name>LONG_ProfitTarget_2</Name>
      <Minimum>0</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>-30</Default>
      <Description>Short 1 profit target (ticks-negative value)</Description>
      <Name>SHORT_ProfitTarget_1</Name>
      <Minimum>-99999</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>-200</Default>
      <Description>Short2  profit target (ticks-negative value)</Description>
      <Name>SHORT_ProfitTarget_2</Name>
      <Minimum>-99999</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>true</Default>
      <Description>If profit trigget is off then profit is taken at reversal.</Description>
      <Name>ProfitTriggerON</Name>
      <Minimum />
      <Type>bool</Type>
    </InputParameter>
    <InputParameter>
      <Default>20</Default>
      <Description>Bars required to trade</Description>
      <Name>BarsToTrade</Name>
      <Minimum>3</Minimum>
      <Type>int</Type>
    </InputParameter>
  </InputParameters>
  <IsTradingHoursBreakLineVisible>true</IsTradingHoursBreakLineVisible>
  <IsInstantiatedOnEachOptimizationIteration>true</IsInstantiatedOnEachOptimizationIteration>
  <MaximumBarsLookBack>TwoHundredFiftySix</MaximumBarsLookBack>
  <MinimumBarsRequired>20</MinimumBarsRequired>
  <OrderFillResolution>Standard</OrderFillResolution>
  <OrderFillResolutionValue>1</OrderFillResolutionValue>
  <OrderFillResolutionType>Minute</OrderFillResolutionType>
  <OverlayOnPrice>false</OverlayOnPrice>
  <PaintPriceMarkers>true</PaintPriceMarkers>
  <PlotParameters />
  <RealTimeErrorHandling>StopCancelClose</RealTimeErrorHandling>
  <ScaleJustification>Right</ScaleJustification>
  <ScriptType>Strategy</ScriptType>
  <Slippage>0</Slippage>
  <StartBehavior>WaitUntilFlat</StartBehavior>
  <StopsAndTargets />
  <StopTargetHandling>PerEntryExecution</StopTargetHandling>
  <TimeInForce>Gtc</TimeInForce>
  <TraceOrders>true</TraceOrders>
  <UseOnAddTradeEvent>false</UseOnAddTradeEvent>
  <UseOnAuthorizeAccountEvent>false</UseOnAuthorizeAccountEvent>
  <UseAccountItemUpdate>false</UseAccountItemUpdate>
  <UseOnCalculatePerformanceValuesEvent>true</UseOnCalculatePerformanceValuesEvent>
  <UseOnConnectionEvent>false</UseOnConnectionEvent>
  <UseOnDataPointEvent>true</UseOnDataPointEvent>
  <UseOnFundamentalDataEvent>false</UseOnFundamentalDataEvent>
  <UseOnExecutionEvent>false</UseOnExecutionEvent>
  <UseOnMouseDown>true</UseOnMouseDown>
  <UseOnMouseMove>true</UseOnMouseMove>
  <UseOnMouseUp>true</UseOnMouseUp>
  <UseOnMarketDataEvent>false</UseOnMarketDataEvent>
  <UseOnMarketDepthEvent>false</UseOnMarketDepthEvent>
  <UseOnMergePerformanceMetricEvent>false</UseOnMergePerformanceMetricEvent>
  <UseOnNextDataPointEvent>true</UseOnNextDataPointEvent>
  <UseOnNextInstrumentEvent>true</UseOnNextInstrumentEvent>
  <UseOnOptimizeEvent>true</UseOnOptimizeEvent>
  <UseOnOrderUpdateEvent>false</UseOnOrderUpdateEvent>
  <UseOnPositionUpdateEvent>false</UseOnPositionUpdateEvent>
  <UseOnRenderEvent>true</UseOnRenderEvent>
  <UseOnRestoreValuesEvent>false</UseOnRestoreValuesEvent>
  <UseOnShareEvent>true</UseOnShareEvent>
  <UseOnWindowCreatedEvent>false</UseOnWindowCreatedEvent>
  <UseOnWindowDestroyedEvent>false</UseOnWindowDestroyedEvent>
  <Variables>
    <InputParameter>
      <Default>false</Default>
      <Name>PossibleLong</Name>
      <Type>bool</Type>
    </InputParameter>
    <InputParameter>
      <Default>false</Default>
      <Name>PossibleShort</Name>
      <Type>bool</Type>
    </InputParameter>
  </Variables>
  <Name>SimpleWAEentryV1A</Name>
</ScriptProperties>
@*/
#endregion
