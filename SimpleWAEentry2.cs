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
	public class SimpleWAEentry2 : Strategy
	{
		private NinjaTrader.NinjaScript.Indicators.Numan.WAE_Mod WAE_Mod1;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "SimpleWAEentry2";
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
				Sensitivity					= 300;
				Qty					= 1;
				BarCount					= 6;
				Target					= 40;
				Stop					= 20;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				WAE_Mod1				= WAE_Mod(Close, Convert.ToInt32(Sensitivity), 10, true, 9, 30, true, 9, 30, 2, 200);
				SetProfitTarget("", CalculationMode.Ticks, Target);
				SetTrailStop("", CalculationMode.Ticks, Stop, false);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 2)
				return;

			 // Set 1
			if ((Position.MarketPosition == MarketPosition.Flat)
				 // Long : Waddah Attar zero up
				 && ((WAE_Mod1.TrendUp[2] == 0)
				 && (WAE_Mod1.TrendUp[1] > 0)))
			{
				EnterLongLimit(Convert.ToInt32(Qty), GetCurrentAsk(0), @"Long");
			}
			
			 // Set 2
			if ((Position.MarketPosition == MarketPosition.Flat)
				 // Short : Waddah Attar zero down
				 && ((WAE_Mod1.TrendDown[2] == 0)
				 && (WAE_Mod1.TrendDown[1] > 0)))
			{
				EnterShortLimit(Convert.ToInt32(Qty), GetCurrentBid(0), @"Short");
			}
			
			 // Set 3
			if ((Position.MarketPosition == MarketPosition.Long)
				 && (BarsSinceEntryExecution(0, "", 0) == BarCount)
				 && (WAE_Mod1.TrendUp[2] < WAE_Mod1.TrendUp[1])
				 && (WAE_Mod1.TrendUp[1] < WAE_Mod1.TrendUp[0]))
			{
				ExitLongStopMarket(Convert.ToInt32(Qty), GetCurrentBid(0), @"BarExit", @"Long");
			}
			
			 // Set 4
			if ((Position.MarketPosition == MarketPosition.Short)
				 && (BarsSinceEntryExecution(0, "", 0) == BarCount)
				 && (WAE_Mod1.TrendDown[2] > WAE_Mod1.TrendDown[1])
				 && (WAE_Mod1.TrendDown[1] > WAE_Mod1.TrendDown[0]))
			{
				ExitShortStopLimit(Convert.ToInt32(Qty), GetCurrentBid(0), GetCurrentAsk(0), @"BarExit", @"Short");
			}
			
			 // Set 5
			if ((Position.MarketPosition == MarketPosition.Long)
				 && (WAE_Mod1.TrendUp[0] == 0)
				 && (WAE_Mod1.TrendDown[0] < 0))
			{
				EnterShort(Convert.ToInt32(Qty), @"Short");
			}
			
			 // Set 6
			if ((Position.MarketPosition == MarketPosition.Short)
				 && (WAE_Mod1.TrendUp[0] >= 0)
				 && (WAE_Mod1.TrendDown[0] == 0))
			{
				EnterLong(Convert.ToInt32(Qty), @"Long");
			}
			
			 // Set 7
			if ((Position.MarketPosition == MarketPosition.Flat)
				 && (WAE_Mod1.TrendUp[0] > WAE_Mod1.ExplosionLine[0])
				 && (WAE_Mod1.ExplosionLine[1] > WAE_Mod1.ExplosionLine[2]))
			{
				EnterLong(Convert.ToInt32(Qty), @"Long");
			}
			
			 // Set 8
			if ((Position.MarketPosition == MarketPosition.Flat)
				 && (WAE_Mod1.TrendDown[0] < WAE_Mod1.ExplosionLineDn[0])
				 && (WAE_Mod1.ExplosionLineDn[1] < WAE_Mod1.ExplosionLineDn[2]))
			{
				EnterShort(Convert.ToInt32(Qty), @"Short");
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
		[Display(Name="Qty", Description="Number of Contracts to trade", Order=2, GroupName="Parameters")]
		public int Qty
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="BarCount", Description="BarCount by waddah Attar trade entry", Order=3, GroupName="Parameters")]
		public int BarCount
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Target", Order=4, GroupName="Parameters")]
		public int Target
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Stop", Order=5, GroupName="Parameters")]
		public int Stop
		{ get; set; }
		#endregion

	}
}

#region Wizard settings, neither change nor remove
/*@
<?xml version="1.0"?>
<ScriptProperties xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Calculate>OnEachTick</Calculate>
  <ConditionalActions>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter long position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">GetCurrentAsk(Default input)</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Ask</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>GetCurrentAsk({0})</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:46:33.142727</Date>
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
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">GetCurrentAsk(0)</LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">Qty</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Qty</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Qty</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:46:39.8721723</Date>
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
              <LiveValue xsi:type="xsd:string">Qty</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Long</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Enter long position by a limit order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-15T08:46:22.2256782</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>EnterLimit</ActionType>
          <Command>
            <Command>EnterLongLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
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
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:43:34.2524781</Date>
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
                <Date>2024-05-15T08:43:34.2774775</Date>
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
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Flat</DisplayName>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T08:44:36.7746477</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <BarsAgo>2</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:44:26.9795559</Date>
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
                <Date>2024-05-15T08:44:26.9935569</Date>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T08:45:17.7729026</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T08:45:02.3897615</Date>
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
                <Date>2024-05-15T08:45:02.3957595</Date>
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
          <DisplayName>Long : Waddah Attar zero up</DisplayName>
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
          <Name>Enter short position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">GetCurrentBid(Default input)</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bid</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>GetCurrentBid({0})</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:48:08.5169459</Date>
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
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">GetCurrentBid(0)</LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">Qty</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Qty</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Qty</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:48:22.6206013</Date>
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
              <LiveValue xsi:type="xsd:string">Qty</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Short</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Enter short position by a limit order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-15T08:47:59.4993071</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>EnterLimit</ActionType>
          <Command>
            <Command>EnterShortLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Enter short position by a limit order</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LimitPrice>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <BindingValue xsi:type="xsd:string">GetCurrentBid(Default input)</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>Bid</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>GetCurrentBid({0})</Command>
                <Parameters>
                  <string>Series1</string>
                  <string>OffsetBuilder</string>
                </Parameters>
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-05-15T08:48:08.5169459</Date>
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
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">GetCurrentBid(0)</LiveValue>
          </LimitPrice>
          <LogLevel>Information</LogLevel>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">Qty</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>Qty</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>Qty</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-05-15T08:48:22.6206013</Date>
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
            <LiveValue xsi:type="xsd:string">Qty</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SignalName>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Short</StringValue>
              </NinjaScriptString>
            </Strings>
          </SignalName>
          <SoundLocation />
          <Tag>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Set Enter short position by a limit order</StringValue>
              </NinjaScriptString>
            </Strings>
          </Tag>
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-05-15T08:47:59.4993071</VariableDateTime>
          <VariableBool>false</VariableBool>
        </ActionProperties>
        <ActionType>EnterLimit</ActionType>
        <Command>
          <Command>EnterShortLimit</Command>
          <Parameters>
            <string>quantity</string>
            <string>limitPrice</string>
            <string>signalName</string>
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
                <Date>2024-05-15T08:43:34.2524781</Date>
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
                <Date>2024-05-15T08:43:34.2774775</Date>
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
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Flat</DisplayName>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T08:44:36.7746477</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <BarsAgo>2</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:44:26.9795559</Date>
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
                <Date>2024-05-15T08:44:26.9935569</Date>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T08:45:17.7729026</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T08:45:02.3897615</Date>
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
                <Date>2024-05-15T08:45:02.3957595</Date>
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
          <DisplayName>Short : Waddah Attar zero down</DisplayName>
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
          <Name>Exit long position by a stop order</Name>
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
                  <StringValue>Long</StringValue>
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
              <BindingValue xsi:type="xsd:string">Qty</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Qty</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Qty</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:51:37.4486035</Date>
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
              <LiveValue xsi:type="xsd:string">Qty</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>BarExit</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <StopPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">GetCurrentBid(Default input)</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bid</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>GetCurrentBid({0})</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:51:56.7887773</Date>
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
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">GetCurrentBid(0)</LiveValue>
            </StopPrice>
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit long position by a stop order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-15T08:51:15.2877326</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitStop</ActionType>
          <Command>
            <Command>ExitLongStopMarket</Command>
            <Parameters>
              <string>quantity</string>
              <string>stopPrice</string>
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
                <Date>2024-05-15T08:50:14.1666239</Date>
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
                <Date>2024-05-15T08:50:14.1806247</Date>
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
                <Date>2024-05-15T08:50:29.8292007</Date>
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
                <Name>BarCount</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarCount</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:50:29.8422006</Date>
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
          <DisplayName>BarsSinceEntryExecution(0, "", 0) = BarCount</DisplayName>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T09:03:41.7650944</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <BarsAgo>2</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T09:03:21.5231161</Date>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T09:04:29.7717402</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T09:03:21.5461139</Date>
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
          <DisplayName>WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendUp[2] &lt; WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendUp[1]</DisplayName>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T09:03:41.7650944</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T09:03:21.5231161</Date>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T09:04:29.7717402</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T09:03:21.5461139</Date>
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
          <DisplayName>WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendUp[1] &lt; WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendUp[0]</DisplayName>
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
          <Name>Exit short position by a stop limit order</Name>
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
                  <StringValue>Short</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">GetCurrentBid(Default input)</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Bid</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>GetCurrentBid({0})</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:53:32.4327541</Date>
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
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">GetCurrentBid(0)</LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">Qty</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Qty</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Qty</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:53:58.3119548</Date>
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
              <LiveValue xsi:type="xsd:string">Qty</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>BarExit</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <StopPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">GetCurrentAsk(Default input)</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Ask</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>GetCurrentAsk({0})</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:53:49.3214359</Date>
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
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">GetCurrentAsk(0)</LiveValue>
            </StopPrice>
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit short position by a stop limit order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-15T08:53:02.0528325</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitStopLimit</ActionType>
          <Command>
            <Command>ExitShortStopLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>stopPrice</string>
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
                <Date>2024-05-15T08:50:14.1666239</Date>
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
                <Date>2024-05-15T08:50:14.1806247</Date>
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
                <Date>2024-05-15T08:50:29.8292007</Date>
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
                <Name>BarCount</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>BarCount</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T08:50:29.8422006</Date>
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
          <DisplayName>BarsSinceEntryExecution(0, "", 0) = BarCount</DisplayName>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T09:03:41.7650944</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <BarsAgo>2</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T09:03:21.5231161</Date>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T09:04:29.7717402</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T09:03:21.5461139</Date>
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
          <DisplayName>WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendDown[2] &gt; WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendDown[1]</DisplayName>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T09:03:41.7650944</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T09:03:21.5231161</Date>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T09:04:29.7717402</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T09:03:21.5461139</Date>
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
          <DisplayName>WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendDown[1] &gt; WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendDown[0]</DisplayName>
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
              <BindingValue xsi:type="xsd:string">Qty</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Qty</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Qty</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T10:17:17.0316243</Date>
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
              <LiveValue xsi:type="xsd:string">Qty</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Short</StringValue>
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
            <VariableDateTime>2024-05-15T10:17:07.6391368</VariableDateTime>
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
      <ActiveAction>
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
            <BindingValue xsi:type="xsd:string">Qty</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>Qty</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>Qty</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-05-15T10:17:17.0316243</Date>
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
            <LiveValue xsi:type="xsd:string">Qty</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SignalName>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Short</StringValue>
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
          <VariableDateTime>2024-05-15T10:17:07.6391368</VariableDateTime>
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
                <Date>2024-05-15T08:50:14.1666239</Date>
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
                <Date>2024-05-15T08:50:14.1806247</Date>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T10:22:00.9805297</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T10:15:52.8945139</Date>
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
                <Date>2024-05-15T10:15:52.9095129</Date>
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
          <DisplayName>WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendUp[0] = 0</DisplayName>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T10:22:14.3339772</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T10:15:52.8945139</Date>
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
                <Date>2024-05-15T10:15:52.9095129</Date>
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
          <DisplayName>WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendDown[0] &lt; 0</DisplayName>
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
              <BindingValue xsi:type="xsd:string">Qty</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Qty</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Qty</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T10:18:45.7034604</Date>
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
              <LiveValue xsi:type="xsd:string">Qty</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Long</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Enter long position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-15T10:18:40.7975786</VariableDateTime>
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
      <ActiveAction>
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
            <BindingValue xsi:type="xsd:string">Qty</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>Qty</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>Qty</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-05-15T10:18:45.7034604</Date>
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
            <LiveValue xsi:type="xsd:string">Qty</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SignalName>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Long</StringValue>
              </NinjaScriptString>
            </Strings>
          </SignalName>
          <SoundLocation />
          <Tag>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Set Enter long position</StringValue>
              </NinjaScriptString>
            </Strings>
          </Tag>
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-05-15T10:18:40.7975786</VariableDateTime>
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
                <Date>2024-05-15T08:50:14.1666239</Date>
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
                <Date>2024-05-15T08:50:14.1806247</Date>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T10:21:35.4662387</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T10:15:52.8945139</Date>
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
                <Date>2024-05-15T10:15:52.9095129</Date>
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
          <DisplayName>WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendUp[0] &gt;= 0</DisplayName>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T10:21:48.3501173</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T10:15:52.8945139</Date>
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
                <Date>2024-05-15T10:15:52.9095129</Date>
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
          <DisplayName>WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendDown[0] = 0</DisplayName>
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
              <BindingValue xsi:type="xsd:string">Qty</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Qty</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Qty</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T10:24:38.178975</Date>
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
              <LiveValue xsi:type="xsd:string">Qty</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Long</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Enter long position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-05-15T10:24:32.5358742</VariableDateTime>
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
      <ActiveAction>
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
            <BindingValue xsi:type="xsd:string">Qty</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>Qty</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>Qty</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-05-15T10:24:38.178975</Date>
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
            <LiveValue xsi:type="xsd:string">Qty</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SignalName>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Long</StringValue>
              </NinjaScriptString>
            </Strings>
          </SignalName>
          <SoundLocation />
          <Tag>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Set Enter long position</StringValue>
              </NinjaScriptString>
            </Strings>
          </Tag>
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-05-15T10:24:32.5358742</VariableDateTime>
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
                <Date>2024-05-15T10:19:57.6468809</Date>
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
                <Date>2024-05-15T10:19:57.6648842</Date>
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
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Flat</DisplayName>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T10:21:08.7126968</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T10:20:19.2176896</Date>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T10:21:02.6210578</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T10:20:19.2226896</Date>
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
          <DisplayName>WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendUp[0] &gt; WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).ExplosionLine[0]</DisplayName>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T10:21:08.7126968</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T10:20:19.2176896</Date>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T10:21:02.6210578</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <BarsAgo>2</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T10:20:19.2226896</Date>
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
          <DisplayName>WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).ExplosionLine[1] &gt; WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).ExplosionLine[2]</DisplayName>
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
              <BindingValue xsi:type="xsd:string">Qty</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Qty</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Qty</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T10:26:04.8821942</Date>
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
              <LiveValue xsi:type="xsd:string">Qty</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Short</StringValue>
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
            <VariableDateTime>2024-05-15T10:26:00.6722057</VariableDateTime>
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
      <ActiveAction>
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
            <BindingValue xsi:type="xsd:string">Qty</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>Qty</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>Qty</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-05-15T10:26:04.8821942</Date>
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
            <LiveValue xsi:type="xsd:string">Qty</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SignalName>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Short</StringValue>
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
          <VariableDateTime>2024-05-15T10:26:00.6722057</VariableDateTime>
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
                <Date>2024-05-15T10:19:57.6468809</Date>
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
                <Date>2024-05-15T10:19:57.6648842</Date>
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
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Flat</DisplayName>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T10:21:08.7126968</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T10:20:19.2176896</Date>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T10:21:02.6210578</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T10:20:19.2226896</Date>
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
          <DisplayName>WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).TrendDown[0] &lt; WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).ExplosionLineDn[0]</DisplayName>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T10:21:08.7126968</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <Date>2024-05-15T10:20:19.2176896</Date>
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
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>Sensitivity</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>Sensitivity</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-05-15T10:21:02.6210578</Date>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
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
                <BarsAgo>2</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-05-15T10:20:19.2226896</Date>
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
          <DisplayName>WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).ExplosionLineDn[1] &lt; WAE_Mod(Sensitivity, 10, true, 9, 30, true, 9, 30, 2, 200).ExplosionLineDn[2]</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 8</SetName>
      <SetNumber>8</SetNumber>
    </ConditionalAction>
  </ConditionalActions>
  <CustomSeries />
  <DataSeries />
  <Description>Enter the description for your new custom Strategy here.</Description>
  <DisplayInDataBox>true</DisplayInDataBox>
  <DrawHorizontalGridLines>true</DrawHorizontalGridLines>
  <DrawOnPricePanel>true</DrawOnPricePanel>
  <DrawVerticalGridLines>true</DrawVerticalGridLines>
  <EntriesPerDirection>1</EntriesPerDirection>
  <EntryHandling>AllEntries</EntryHandling>
  <ExitOnSessionClose>true</ExitOnSessionClose>
  <ExitOnSessionCloseSeconds>30</ExitOnSessionCloseSeconds>
  <FillLimitOrdersOnTouch>false</FillLimitOrdersOnTouch>
  <InputParameters>
    <InputParameter>
      <Default>300</Default>
      <Description>Waddah Attar Explosion sensitivity</Description>
      <Name>Sensitivity</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>1</Default>
      <Description>Number of Contracts to trade</Description>
      <Name>Qty</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>6</Default>
      <Description>BarCount by waddah Attar trade entry</Description>
      <Name>BarCount</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>40</Default>
      <Description />
      <Name>Target</Name>
      <Minimum />
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>20</Default>
      <Description />
      <Name>Stop</Name>
      <Minimum />
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
  <StopsAndTargets>
    <WizardAction>
      <Children />
      <IsExpanded>false</IsExpanded>
      <IsSelected>true</IsSelected>
      <Name>Profit target</Name>
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
        <Mode>Ticks</Mode>
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
            <Date>2024-05-15T10:42:58.7474164</Date>
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
        <Tag>
          <SeparatorCharacter> </SeparatorCharacter>
          <Strings>
            <NinjaScriptString>
              <Index>0</Index>
              <StringValue>Set Profit target</StringValue>
            </NinjaScriptString>
          </Strings>
        </Tag>
        <TextPosition>BottomLeft</TextPosition>
        <Value>
          <DefaultValue>0</DefaultValue>
          <IsInt>false</IsInt>
          <BindingValue xsi:type="xsd:string">Target</BindingValue>
          <DynamicValue>
            <Children />
            <IsExpanded>false</IsExpanded>
            <IsSelected>true</IsSelected>
            <Name>Target</Name>
            <OffsetType>Arithmetic</OffsetType>
            <AssignedCommand>
              <Command>Target</Command>
              <Parameters />
            </AssignedCommand>
            <BarsAgo>0</BarsAgo>
            <CurrencyType>Currency</CurrencyType>
            <Date>2024-05-15T10:43:11.6718716</Date>
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
          <LiveValue xsi:type="xsd:string">Target</LiveValue>
        </Value>
        <VariableDateTime>2024-05-15T10:42:58.7474164</VariableDateTime>
        <VariableBool>false</VariableBool>
      </ActionProperties>
      <ActionType>Misc</ActionType>
      <Command>
        <Command>SetProfitTarget</Command>
        <Parameters>
          <string>fromEntrySignal</string>
          <string>mode</string>
          <string>value</string>
        </Parameters>
      </Command>
    </WizardAction>
    <WizardAction>
      <Children />
      <IsExpanded>false</IsExpanded>
      <IsSelected>true</IsSelected>
      <Name>Trailing stop</Name>
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
        <Mode>Ticks</Mode>
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
            <Date>2024-05-15T10:43:28.2059176</Date>
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
        <Tag>
          <SeparatorCharacter> </SeparatorCharacter>
          <Strings>
            <NinjaScriptString>
              <Index>0</Index>
              <StringValue>Set Trailing stop</StringValue>
            </NinjaScriptString>
          </Strings>
        </Tag>
        <TextPosition>BottomLeft</TextPosition>
        <Value>
          <DefaultValue>0</DefaultValue>
          <IsInt>false</IsInt>
          <BindingValue xsi:type="xsd:string">Stop</BindingValue>
          <DynamicValue>
            <Children />
            <IsExpanded>false</IsExpanded>
            <IsSelected>true</IsSelected>
            <Name>Stop</Name>
            <OffsetType>Arithmetic</OffsetType>
            <AssignedCommand>
              <Command>Stop</Command>
              <Parameters />
            </AssignedCommand>
            <BarsAgo>0</BarsAgo>
            <CurrencyType>Currency</CurrencyType>
            <Date>2024-05-15T10:43:40.0511621</Date>
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
          <LiveValue xsi:type="xsd:string">Stop</LiveValue>
        </Value>
        <VariableDateTime>2024-05-15T10:43:28.2059176</VariableDateTime>
        <VariableBool>false</VariableBool>
      </ActionProperties>
      <ActionType>Misc</ActionType>
      <Command>
        <Command>SetTrailStop</Command>
        <Parameters>
          <string>fromEntrySignal</string>
          <string>mode</string>
          <string>value</string>
          <string>isSimulatedStop</string>
        </Parameters>
      </Command>
    </WizardAction>
  </StopsAndTargets>
  <StopTargetHandling>PerEntryExecution</StopTargetHandling>
  <TimeInForce>Gtc</TimeInForce>
  <TraceOrders>false</TraceOrders>
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
  <Variables />
  <Name>SimpleWAEentry2</Name>
</ScriptProperties>
@*/
#endregion
