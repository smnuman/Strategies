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
	public class LoWaddahAttarExplosion : Strategy
	{
		private int Last_trade;

		private NinjaTrader.NinjaScript.Indicators.Lo.WaddahAttarExplosion WaddahAttarExplosion1;
		
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
				Name										= "LoWaddahAttarExplosion";
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
				MACD_Fast					= 20;
				MACD_Slow					= 40;
				MACD_Smooth					= 7;
				Sensitivity					= 150;
				StDev_Bars					= 20;
				DeadZone					= 20;
				Repeat_Trades					= 1;
				Start_Time						= DateTime.Parse("09:00", System.Globalization.CultureInfo.InvariantCulture);
				End_Time						= DateTime.Parse("21:00", System.Globalization.CultureInfo.InvariantCulture);
				Last_trade					= 0;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				green = new Series<double>(this);
				red = new Series<double>(this);
				brown = new Series<double>(this);
				longs = new Series<int>(this);
				shorts = new Series<int>(this);			
				WaddahAttarExplosion1				= WaddahAttarExplosion(Close, Convert.ToInt32(Sensitivity), Convert.ToInt32(MACD_Fast), true, Convert.ToInt32(MACD_Smooth), Convert.ToInt32(MACD_Slow), true, Convert.ToInt32(MACD_Smooth), Convert.ToInt32(StDev_Bars), 2, DeadZone);
				SetProfitTarget("", CalculationMode.Ticks, Target);
				SetStopLoss("", CalculationMode.Ticks, Stop, false);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			green[0] = WaddahAttarExplosion1.TrendUp[0];
			red[0] = WaddahAttarExplosion1.TrendDown[0];
			brown[0] = WaddahAttarExplosion1.ExplosionLine[0];
			longs[0] = 0;
			shorts[0] = 0;
			if (CurrentBars[0] < 1)
				return;

			 // Set 2
			if ((green[0] > green[1])
				 && (green[0] > brown[0])
				 && (green[0] > DeadZone)
				 && (Times[0][0].TimeOfDay >= Start_Time.TimeOfDay)
				 && (Times[0][0].TimeOfDay < End_Time.TimeOfDay))
			{
				longs[0] = 1;
			}
			
			 // Set 3
			if ((red[0] > red[1])
				 && (red[0] > brown[0])
				 && (red[0] > DeadZone)
				 && (Times[0][0].TimeOfDay >= Start_Time.TimeOfDay)
				 && (Times[0][0].TimeOfDay < End_Time.TimeOfDay))
			{
				shorts[0] = 1;
			}
			
			 // Set 4
			if ((longs[0] > longs[1])
				 // Repeat Filter
				 && ((Repeat_Trades == 1)
				 || (Last_trade != 1)))
			{
				BackBrush = Brushes.CornflowerBlue;
				Last_trade = 1;
				EnterLong(Convert.ToInt32(DefaultQuantity), "");
			}
			
			 // Set 5
			if ((shorts[0] > shorts[1])
				 // Repeat Filter
				 && ((Repeat_Trades == 1)
				 || (Last_trade != -1)))
			{
				BackBrush = Brushes.LightSalmon;
				Last_trade = -1;
				EnterShort(Convert.ToInt32(DefaultQuantity), "");
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
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="Start_Time", Order=10, GroupName="Parameters")]
		public DateTime Start_Time
		{ get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="End_Time", Order=11, GroupName="Parameters")]
		public DateTime End_Time
		{ get; set; }
		#endregion

	}
}

#region Wizard settings, neither change nor remove
/*@
<?xml version="1.0"?>
<ScriptProperties xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Calculate>OnBarClose</Calculate>
  <ConditionalActions>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set green</Name>
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
                <Date>2024-01-12T20:57:16.7908059</Date>
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
            <VariableDateTime>2024-01-12T20:57:16.7908059</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">WaddahAttarExplosion(Sensitivity, MACD_Fast, true, MACD_Smooth, MACD_Slow, true, MACD_Smooth, StDev_Bars, 2, DeadZone).TrendUp[0]</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WaddahAttarExplosion</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WaddahAttarExplosion</Command>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
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
                            <Date>2024-01-12T20:55:21.7116166</Date>
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
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">MACD_Fast</LiveValue>
                          <BindingValue xsi:type="xsd:string">MACD_Fast</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>MACD_Fast</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>MACD_Fast</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:55:32.7810142</Date>
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
                          <LiveValue xsi:type="xsd:string">MACD_Smooth</LiveValue>
                          <BindingValue xsi:type="xsd:string">MACD_Smooth</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>MACD_Smooth</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>MACD_Smooth</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:56:13.3930295</Date>
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
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">MACD_Slow</LiveValue>
                          <BindingValue xsi:type="xsd:string">MACD_Slow</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>MACD_Slow</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>MACD_Slow</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:56:20.9995261</Date>
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
                          <LiveValue xsi:type="xsd:string">MACD_Smooth</LiveValue>
                          <BindingValue xsi:type="xsd:string">MACD_Smooth</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>MACD_Smooth</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>MACD_Smooth</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:56:26.1972656</Date>
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
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">StDev_Bars</LiveValue>
                          <BindingValue xsi:type="xsd:string">StDev_Bars</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>StDev_Bars</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>StDev_Bars</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:56:36.714716</Date>
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
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <IsLiteral>true</IsLiteral>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">DeadZone</LiveValue>
                          <BindingValue xsi:type="xsd:string">DeadZone</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>DeadZone</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>DeadZone</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:56:45.5556021</Date>
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
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WaddahAttarExplosion</IndicatorName>
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
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
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
                <Date>2024-01-12T20:54:50.176614</Date>
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
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">WaddahAttarExplosion(Close, Convert.ToInt32(Sensitivity), Convert.ToInt32(MACD_Fast), true, Convert.ToInt32(MACD_Smooth), Convert.ToInt32(MACD_Slow), true, Convert.ToInt32(MACD_Smooth), Convert.ToInt32(StDev_Bars), 2, DeadZone).TrendUp[0]</LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetCustomSeriesValue</ActionType>
          <UserVariableType>Double</UserVariableType>
          <VariableName>green</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set red</Name>
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
                <Date>2024-01-12T20:57:34.2620682</Date>
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
            <VariableDateTime>2024-01-12T20:57:34.2620682</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">WaddahAttarExplosion(Sensitivity, MACD_Fast, true, MACD_Smooth, MACD_Slow, true, MACD_Smooth, StDev_Bars, 2, DeadZone).TrendDown[0]</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WaddahAttarExplosion</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WaddahAttarExplosion</Command>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
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
                            <Date>2024-01-12T20:58:31.290308</Date>
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
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">MACD_Fast</LiveValue>
                          <BindingValue xsi:type="xsd:string">MACD_Fast</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>MACD_Fast</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>MACD_Fast</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:58:27.5862947</Date>
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
                          <LiveValue xsi:type="xsd:string">MACD_Smooth</LiveValue>
                          <BindingValue xsi:type="xsd:string">MACD_Smooth</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>MACD_Smooth</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>MACD_Smooth</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:58:17.2784308</Date>
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
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">MACD_Slow</LiveValue>
                          <BindingValue xsi:type="xsd:string">MACD_Slow</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>MACD_Slow</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>MACD_Slow</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:58:11.6964502</Date>
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
                          <LiveValue xsi:type="xsd:string">MACD_Smooth</LiveValue>
                          <BindingValue xsi:type="xsd:string">MACD_Smooth</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>MACD_Smooth</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>MACD_Smooth</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:58:05.3839192</Date>
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
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">StDev_Bars</LiveValue>
                          <BindingValue xsi:type="xsd:string">StDev_Bars</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>StDev_Bars</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>StDev_Bars</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:57:55.4473069</Date>
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
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <IsLiteral>true</IsLiteral>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">DeadZone</LiveValue>
                          <BindingValue xsi:type="xsd:string">DeadZone</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>DeadZone</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>DeadZone</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:57:44.1208873</Date>
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
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WaddahAttarExplosion</IndicatorName>
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
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
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
                <Date>2024-01-12T20:57:36.818731</Date>
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
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">WaddahAttarExplosion(Close, Convert.ToInt32(Sensitivity), Convert.ToInt32(MACD_Fast), true, Convert.ToInt32(MACD_Smooth), Convert.ToInt32(MACD_Slow), true, Convert.ToInt32(MACD_Smooth), Convert.ToInt32(StDev_Bars), 2, DeadZone).TrendDown[0]</LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetCustomSeriesValue</ActionType>
          <UserVariableType>Double</UserVariableType>
          <VariableName>red</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set brown</Name>
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
                <Date>2024-04-08T16:58:02.9162222</Date>
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
            <VariableDateTime>2024-04-08T16:58:02.9162222</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">WaddahAttarExplosion(Sensitivity, MACD_Fast, true, MACD_Smooth, MACD_Slow, true, MACD_Smooth, StDev_Bars, 2, DeadZone).ExplosionLine[0]</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>WaddahAttarExplosion</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>WaddahAttarExplosion</Command>
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
                          <LiveValue xsi:type="xsd:string">Sensitivity</LiveValue>
                          <BindingValue xsi:type="xsd:string">Sensitivity</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
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
                            <Date>2024-04-08T16:58:18.6637364</Date>
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
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>FastLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">MACD_Fast</LiveValue>
                          <BindingValue xsi:type="xsd:string">MACD_Fast</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>MACD_Fast</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>MACD_Fast</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:59:36.1742383</Date>
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
                          <LiveValue xsi:type="xsd:string">MACD_Smooth</LiveValue>
                          <BindingValue xsi:type="xsd:string">MACD_Smooth</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>MACD_Smooth</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>MACD_Smooth</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:59:30.5779966</Date>
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
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SlowLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">MACD_Slow</LiveValue>
                          <BindingValue xsi:type="xsd:string">MACD_Slow</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>MACD_Slow</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>MACD_Slow</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:59:14.6841607</Date>
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
                          <LiveValue xsi:type="xsd:string">MACD_Smooth</LiveValue>
                          <BindingValue xsi:type="xsd:string">MACD_Smooth</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>MACD_Smooth</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>MACD_Smooth</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:59:10.552638</Date>
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
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ChannelLength</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">StDev_Bars</LiveValue>
                          <BindingValue xsi:type="xsd:string">StDev_Bars</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>StDev_Bars</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>StDev_Bars</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:59:06.2460734</Date>
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
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Mult</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <IsLiteral>true</IsLiteral>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>DeadZone</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <LiveValue xsi:type="xsd:string">DeadZone</LiveValue>
                          <BindingValue xsi:type="xsd:string">DeadZone</BindingValue>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>DeadZone</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>DeadZone</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-01-12T20:58:57.7200624</Date>
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
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>WaddahAttarExplosion</IndicatorName>
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
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF8B4513&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ExplosionLine</Name>
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
                <Date>2024-01-12T20:58:48.2255576</Date>
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
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">WaddahAttarExplosion(Close, Convert.ToInt32(Sensitivity), Convert.ToInt32(MACD_Fast), true, Convert.ToInt32(MACD_Smooth), Convert.ToInt32(MACD_Slow), true, Convert.ToInt32(MACD_Smooth), Convert.ToInt32(StDev_Bars), 2, DeadZone).ExplosionLine[0]</LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetCustomSeriesValue</ActionType>
          <UserVariableType>Double</UserVariableType>
          <VariableName>brown</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set longs</Name>
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
                <Date>2023-11-03T15:06:35.0602467</Date>
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
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <CustomSeries>
                <Name>longs</Name>
                <Type>Int</Type>
                <SeriesID>abfae2f1-b771-4a5d-a77e-31e18589c4b0</SeriesID>
              </CustomSeries>
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>CustomSeries</SeriesType>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2023-11-03T15:06:35.0602467</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetCustomSeriesValue</ActionType>
          <UserVariableType>Int</UserVariableType>
          <VariableName>longs</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set shorts</Name>
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
                <Date>2023-11-03T15:06:42.0824207</Date>
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
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <CustomSeries>
                <Name>shorts</Name>
                <Type>Int</Type>
                <SeriesID>bda4e439-9670-40dd-9a5f-596116f833d4</SeriesID>
              </CustomSeries>
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>CustomSeries</SeriesType>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2023-11-03T15:06:42.0824207</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetCustomSeriesValue</ActionType>
          <UserVariableType>Int</UserVariableType>
          <VariableName>shorts</VariableName>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Set longs</Name>
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
              <Date>2023-11-03T15:06:35.0602467</Date>
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
          <Series>
            <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
            <CustomProperties />
            <CustomSeries>
              <Name>longs</Name>
              <Type>Int</Type>
              <SeriesID>abfae2f1-b771-4a5d-a77e-31e18589c4b0</SeriesID>
            </CustomSeries>
            <IsExplicitlyNamed>false</IsExplicitlyNamed>
            <IsPriceTypeLocked>false</IsPriceTypeLocked>
            <PlotOnChart>false</PlotOnChart>
            <PriceType>Close</PriceType>
            <SeriesType>CustomSeries</SeriesType>
          </Series>
          <ServiceName />
          <ScreenshotPath />
          <SoundLocation />
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2023-11-03T15:06:35.0602467</VariableDateTime>
          <VariableBool>false</VariableBool>
        </ActionProperties>
        <ActionType>SetCustomSeriesValue</ActionType>
        <UserVariableType>Int</UserVariableType>
        <VariableName>longs</VariableName>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions />
      <SetName>Set 1</SetName>
      <SetNumber>1</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set longs</Name>
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
                <Date>2023-11-03T15:06:53.0681906</Date>
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
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <CustomSeries>
                <Name>longs</Name>
                <Type>Int</Type>
                <SeriesID>abfae2f1-b771-4a5d-a77e-31e18589c4b0</SeriesID>
              </CustomSeries>
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>CustomSeries</SeriesType>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableInt>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">1</BindingValue>
              <IsLiteral>true</IsLiteral>
              <LiveValue xsi:type="xsd:string">1</LiveValue>
            </VariableInt>
            <VariableDateTime>2023-11-03T15:06:53.0681906</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetCustomSeriesValue</ActionType>
          <UserVariableType>Int</UserVariableType>
          <VariableName>longs</VariableName>
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
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>green</Name>
                    <Type>Double</Type>
                    <SeriesID>a4c52d8b-db1c-49a9-8981-2481d15baa3d</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:02:26.4637918</Date>
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
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>green</Name>
                    <Type>Double</Type>
                    <SeriesID>a4c52d8b-db1c-49a9-8981-2481d15baa3d</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:02:26.4873156</Date>
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
          <DisplayName>green[0] &gt; green[1]</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>green</Name>
                    <Type>Double</Type>
                    <SeriesID>a4c52d8b-db1c-49a9-8981-2481d15baa3d</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:02:26.4637918</Date>
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
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>brown</Name>
                    <Type>Double</Type>
                    <SeriesID>27ef1e20-d2ec-4e52-afd1-f886c7afce96</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:02:26.4873156</Date>
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
          <DisplayName>green[0] &gt; brown[0]</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>green</Name>
                    <Type>Double</Type>
                    <SeriesID>a4c52d8b-db1c-49a9-8981-2481d15baa3d</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:02:26.4637918</Date>
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
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>DeadZone</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DeadZone</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-11-03T15:03:21.2817488</Date>
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
          <DisplayName>green[0] &gt; DeadZone</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Time series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Times[{0}][{1}].TimeOfDay</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-06-23T15:26:52.1140931</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>true</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
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
                <Name>Start_Time</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Start_Time.TimeOfDay</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-06-23T15:26:52.1855188</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Times[Default input][0].TimeOfDay &gt;= Start_Time.TimeOfDay</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Time series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Times[{0}][{1}].TimeOfDay</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-06-23T15:26:52.1140931</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>true</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
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
                <Name>End_Time</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>End_Time.TimeOfDay</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-06-23T15:27:10.3201509</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Times[Default input][0].TimeOfDay &lt; End_Time.TimeOfDay</DisplayName>
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
          <Name>Set shorts</Name>
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
                <Date>2023-11-03T15:07:04.49095</Date>
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
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <CustomSeries>
                <Name>shorts</Name>
                <Type>Int</Type>
                <SeriesID>bda4e439-9670-40dd-9a5f-596116f833d4</SeriesID>
              </CustomSeries>
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>CustomSeries</SeriesType>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableInt>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">1</BindingValue>
              <IsLiteral>true</IsLiteral>
              <LiveValue xsi:type="xsd:string">1</LiveValue>
            </VariableInt>
            <VariableDateTime>2023-11-03T15:07:04.49095</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetCustomSeriesValue</ActionType>
          <UserVariableType>Int</UserVariableType>
          <VariableName>shorts</VariableName>
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
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>red</Name>
                    <Type>Double</Type>
                    <SeriesID>653a1056-dddd-4e7f-8130-92fb875f5ee9</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:02:26.4637918</Date>
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
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>red</Name>
                    <Type>Double</Type>
                    <SeriesID>653a1056-dddd-4e7f-8130-92fb875f5ee9</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:02:26.4873156</Date>
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
          <DisplayName>red[0] &gt; red[1]</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>red</Name>
                    <Type>Double</Type>
                    <SeriesID>653a1056-dddd-4e7f-8130-92fb875f5ee9</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:02:26.4637918</Date>
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
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>brown</Name>
                    <Type>Double</Type>
                    <SeriesID>27ef1e20-d2ec-4e52-afd1-f886c7afce96</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:02:26.4873156</Date>
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
          <DisplayName>red[0] &gt; brown[0]</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>red</Name>
                    <Type>Double</Type>
                    <SeriesID>653a1056-dddd-4e7f-8130-92fb875f5ee9</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:02:26.4637918</Date>
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
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>DeadZone</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DeadZone</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-11-03T15:03:21.2817488</Date>
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
          <DisplayName>red[0] &gt; DeadZone</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Time series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Times[{0}][{1}].TimeOfDay</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-06-23T15:26:52.1140931</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>true</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
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
                <Name>Start_Time</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Start_Time.TimeOfDay</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-06-23T15:26:52.1855188</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Times[Default input][0].TimeOfDay &gt;= Start_Time.TimeOfDay</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Time series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Times[{0}][{1}].TimeOfDay</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-06-23T15:26:52.1140931</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>true</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
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
                <Name>End_Time</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>End_Time.TimeOfDay</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-06-23T15:27:10.3201509</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Times[Default input][0].TimeOfDay &lt; End_Time.TimeOfDay</DisplayName>
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
          <Name>Set background color</Name>
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
                <Date>2023-11-03T15:08:13.8170888</Date>
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
            <VariableDateTime>2023-11-03T15:08:13.8170888</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>brush</UserVariableType>
          <VariableName>BackBrush</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set Last_trade</Name>
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
                <Date>2023-11-03T15:11:06.5421655</Date>
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
            <VariableInt>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">1</BindingValue>
              <IsLiteral>true</IsLiteral>
              <LiveValue xsi:type="xsd:string">1</LiveValue>
            </VariableInt>
            <VariableDateTime>2023-11-03T15:11:06.5421655</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>int</UserVariableType>
          <VariableName>Last_trade</VariableName>
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
                <Date>2023-11-03T15:16:37.8898772</Date>
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
                  <StringValue>Set Enter long position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2023-11-03T15:16:37.8898772</VariableDateTime>
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
              <Date>2023-11-03T15:16:37.8898772</Date>
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
                <StringValue>Set Enter long position</StringValue>
              </NinjaScriptString>
            </Strings>
          </Tag>
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2023-11-03T15:16:37.8898772</VariableDateTime>
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
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>longs</Name>
                    <Type>Int</Type>
                    <SeriesID>abfae2f1-b771-4a5d-a77e-31e18589c4b0</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:07:34.082188</Date>
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
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>longs</Name>
                    <Type>Int</Type>
                    <SeriesID>abfae2f1-b771-4a5d-a77e-31e18589c4b0</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:07:34.1142359</Date>
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
          <DisplayName>longs[0] &gt; longs[1]</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Repeat_Trades</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Repeat_Trades</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-11-03T15:14:12.1612788</Date>
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
                <Date>2023-11-03T15:14:12.1962187</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">1</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">1</LiveValue>
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
                <Name>Last_trade</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Last_trade</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-11-03T15:14:34.5372712</Date>
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
              <Operator>NotEqual</Operator>
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
                <Date>2023-11-03T15:14:34.5623948</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">1</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">1</LiveValue>
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
          <DisplayName>Repeat Filter</DisplayName>
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
          <Name>Set background color</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <Brush xsi:type="SolidColorBrush">
              <Opacity>1</Opacity>
              <Transform xsi:type="MatrixTransform">
                <Matrix>
                  <M11>1</M11>
                  <M12>0</M12>
                  <M21>0</M21>
                  <M22>1</M22>
                  <OffsetX>0</OffsetX>
                  <OffsetY>0</OffsetY>
                </Matrix>
              </Transform>
              <RelativeTransform xsi:type="MatrixTransform">
                <Matrix>
                  <M11>1</M11>
                  <M12>0</M12>
                  <M21>0</M21>
                  <M22>1</M22>
                  <OffsetX>0</OffsetX>
                  <OffsetY>0</OffsetY>
                </Matrix>
              </RelativeTransform>
              <Color>
                <A>255</A>
                <R>255</R>
                <G>160</G>
                <B>122</B>
                <ScA>1</ScA>
                <ScR>1</ScR>
                <ScG>0.351532638</ScG>
                <ScB>0.194617838</ScB>
              </Color>
            </Brush>
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
                <Date>2023-11-03T15:08:26.0879945</Date>
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
            <VariableDateTime>2023-11-03T15:08:26.0879945</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>brush</UserVariableType>
          <VariableName>BackBrush</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set Last_trade</Name>
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
                <Date>2023-11-03T15:11:20.6912512</Date>
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
            <VariableInt>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">-1</BindingValue>
              <IsLiteral>true</IsLiteral>
              <LiveValue xsi:type="xsd:string">-1</LiveValue>
            </VariableInt>
            <VariableDateTime>2023-11-03T15:11:20.6912512</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>int</UserVariableType>
          <VariableName>Last_trade</VariableName>
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
                <Date>2023-11-03T15:16:43.6255227</Date>
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
                  <StringValue>Set Enter short position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2023-11-03T15:16:43.6255227</VariableDateTime>
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
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>shorts</Name>
                    <Type>Int</Type>
                    <SeriesID>bda4e439-9670-40dd-9a5f-596116f833d4</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:07:34.082188</Date>
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
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Custom series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>CustomSeries</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>1</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <CustomSeries>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <CustomSeries>
                    <Name>shorts</Name>
                    <Type>Int</Type>
                    <SeriesID>bda4e439-9670-40dd-9a5f-596116f833d4</SeriesID>
                  </CustomSeries>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>CustomSeries</SeriesType>
                </CustomSeries>
                <Date>2023-11-03T15:07:34.1142359</Date>
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
          <DisplayName>shorts[0] &gt; shorts[1]</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Repeat_Trades</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Repeat_Trades</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-11-03T15:14:12.1612788</Date>
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
                <Date>2023-11-03T15:14:12.1962187</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">1</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">1</LiveValue>
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
                <Name>Last_trade</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Last_trade</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2023-11-03T15:14:34.5372712</Date>
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
              <Operator>NotEqual</Operator>
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
                <Date>2023-11-03T15:14:34.5623948</Date>
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
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Repeat Filter</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 5</SetName>
      <SetNumber>5</SetNumber>
    </ConditionalAction>
  </ConditionalActions>
  <CustomSeries>
    <CustomSeriesProperties>
      <Name>green</Name>
      <Type>Double</Type>
      <SeriesID>a4c52d8b-db1c-49a9-8981-2481d15baa3d</SeriesID>
    </CustomSeriesProperties>
    <CustomSeriesProperties>
      <Name>red</Name>
      <Type>Double</Type>
      <SeriesID>653a1056-dddd-4e7f-8130-92fb875f5ee9</SeriesID>
    </CustomSeriesProperties>
    <CustomSeriesProperties>
      <Name>brown</Name>
      <Type>Double</Type>
      <SeriesID>27ef1e20-d2ec-4e52-afd1-f886c7afce96</SeriesID>
    </CustomSeriesProperties>
    <CustomSeriesProperties>
      <Name>longs</Name>
      <Type>Int</Type>
      <SeriesID>abfae2f1-b771-4a5d-a77e-31e18589c4b0</SeriesID>
    </CustomSeriesProperties>
    <CustomSeriesProperties>
      <Name>shorts</Name>
      <Type>Int</Type>
      <SeriesID>bda4e439-9670-40dd-9a5f-596116f833d4</SeriesID>
    </CustomSeriesProperties>
  </CustomSeries>
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
      <Default>10</Default>
      <Description />
      <Name>Target</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>10</Default>
      <Description />
      <Name>Stop</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>20</Default>
      <Description />
      <Name>MACD_Fast</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>40</Default>
      <Description />
      <Name>MACD_Slow</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>7</Default>
      <Description />
      <Name>MACD_Smooth</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>150</Default>
      <Description />
      <Name>Sensitivity</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>20</Default>
      <Description />
      <Name>StDev_Bars</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>20</Default>
      <Description />
      <Name>DeadZone</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>1</Default>
      <Description>1=YES  ,  2=NO</Description>
      <Name>Repeat_Trades</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>09:00</Default>
      <Description />
      <Name>Start_Time</Name>
      <Minimum />
      <Type>time</Type>
    </InputParameter>
    <InputParameter>
      <Default>21:00</Default>
      <Description />
      <Name>End_Time</Name>
      <Minimum />
      <Type>time</Type>
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
            <Date>2023-11-03T15:16:51.4719556</Date>
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
            <Date>2023-11-03T15:16:57.1304252</Date>
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
        <VariableDateTime>2023-11-03T15:16:51.4719556</VariableDateTime>
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
      <Name>Stop loss</Name>
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
            <Date>2023-11-03T15:17:03.0720786</Date>
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
              <StringValue>Set Stop loss</StringValue>
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
            <Date>2023-11-03T15:17:08.1248132</Date>
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
        <VariableDateTime>2023-11-03T15:17:03.0720786</VariableDateTime>
        <VariableBool>false</VariableBool>
      </ActionProperties>
      <ActionType>Misc</ActionType>
      <Command>
        <Command>SetStopLoss</Command>
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
  <Variables>
    <InputParameter>
      <Default>0</Default>
      <Name>Last_trade</Name>
      <Type>int</Type>
    </InputParameter>
  </Variables>
  <Name>LoWaddahAttarExplosion</Name>
</ScriptProperties>
@*/
#endregion
