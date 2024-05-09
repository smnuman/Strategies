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
	public class BreakEvenBuilderExample : Strategy
	{
		private double StopPrice;
		private double TriggerPrice;
		private int TriggerState;


		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"";
				Name										= "BreakEvenBuilderExample";
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
				BreakEvenTrigger					= 5;
				InitialStopDistance					= -10;
				StopPrice					= 0;
				TriggerPrice					= 0;
				TriggerState					= 0;
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
			return;

			 // Set 1
			if ((TriggerState >= 2)
				 && (Position.MarketPosition == MarketPosition.Flat))
			{
				TriggerState = 0;
			}
			
			 // Set 2
			if (Position.MarketPosition == MarketPosition.Flat)
			{
				TriggerState = 1;
				EnterLong(Convert.ToInt32(DefaultQuantity), @"entry");
			}
			
			 // Set 3
			if ((TriggerState == 1)
				 && (Position.MarketPosition == MarketPosition.Long))
			{
				TriggerState = 2;
				StopPrice = (Position.AveragePrice + (InitialStopDistance * TickSize)) ;
				TriggerPrice = (Position.AveragePrice + (BreakEvenTrigger * TickSize)) ;
			}
			
			 // Set 4
			if ((TriggerState == 2)
				 && (Close[0] >= TriggerPrice))
			{
				TriggerState = 3;
				StopPrice = Position.AveragePrice;
				Draw.Diamond(this, @"BreakEvenBuilderExample Diamond_1", true, 0, (High[0] + (2 * TickSize)) , Brushes.DarkCyan);
			}
			
			 // Set 5
			if (TriggerState >= 2)
			{
				ExitLongStopMarket(Convert.ToInt32(DefaultQuantity), StopPrice, @"exit", @"entry");
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="BreakEvenTrigger", Description="Number of ticks above entry the breakeven movement trigger is set", Order=1, GroupName="Parameters")]
		public int BreakEvenTrigger
		{ get; set; }

		[NinjaScriptProperty]
		[Range(-999, int.MaxValue)]
		[Display(Name="InitialStopDistance", Description="(use a negative) Number of ticks from entry the stop will initially be placed below", Order=2, GroupName="Parameters")]
		public int InitialStopDistance
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
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TriggerState</Name>
          <Offset>
            <OffsetOperator>Add</OffsetOperator>
            <OffsetType>Arithmetic</OffsetType>
            <IsSetEnabled>false</IsSetEnabled>
            <OffsetValue>0</OffsetValue>
          </Offset>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <Anchor1BarsAgo>0</Anchor1BarsAgo>
            <Anchor1Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor1Y>
            <Anchor2BarsAgo>0</Anchor2BarsAgo>
            <Anchor2Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor2Y>
            <Anchor3BarsAgo>0</Anchor3BarsAgo>
            <Anchor3Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor3Y>
            <AreaBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </AreaBrush>
            <AreaOpacity>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </AreaOpacity>
            <BackBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </BackBrush>
            <BarsAgo>0</BarsAgo>
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </Brush>
            <Color>
              <A>255</A>
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
            <Currency>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Currency>
            <DashStyle>Solid</DashStyle>
            <Displacement>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Displacement>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <EndBarsAgo>0</EndBarsAgo>
            <EndY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EndY>
            <EntryBarsAgo>0</EntryBarsAgo>
            <EntryY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EntryY>
            <ExtensionBarsAgo>0</ExtensionBarsAgo>
            <ExtensionY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </ExtensionY>
            <File />
            <ForegroundBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </ForegroundBrush>
            <FromEntrySignal>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Message>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Message>
            <MessageValue>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </MessageValue>
            <MiddleBarsAgo>0</MiddleBarsAgo>
            <MiddleY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </MiddleY>
            <Mode>Currency</Mode>
            <Offset>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Offset>
            <OffsetType>Currency</OffsetType>
            <Price>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Price>
            <Priority>Medium</Priority>
            <Quantity>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:42:16.2664755</Date>
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
            </Quantity>
            <Ratio>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Ratio>
            <RearmSeconds>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </RearmSeconds>
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>DefaultSeries</SeriesType>
              <NSName>Close</NSName>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </SignalName>
            <SoundLocation />
            <StartBarsAgo>0</StartBarsAgo>
            <StartY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StartY>
            <StopPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StopPrice>
            <Subject>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Subject>
            <Tag>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Tag>
            <Text>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Text>
            <TextBarsAgo>0</TextBarsAgo>
            <TextY>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </TextY>
            <To>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </To>
            <TextPosition>BottomLeft</TextPosition>
            <Value>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Value>
            <VariableInt>
              <LiveValue xsi:type="xsd:string">1</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableInt>
            <VariableString>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </VariableString>
            <VariableDateTime>2017-12-10T14:42:16.2664755</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableDouble>
            <Width>
              <LiveValue xsi:type="xsd:string">2</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Width>
            <Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Y>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>int</UserVariableType>
          <VariableName>TriggerState</VariableName>
        </WizardAction>
        <WizardAction>
          <IsExpanded>true</IsExpanded>
          <IsSelected>true</IsSelected>
          <Children />
          <Name>Enter long position</Name>
          <Offset>
            <OffsetOperator>Add</OffsetOperator>
            <OffsetType>Arithmetic</OffsetType>
            <IsSetEnabled>false</IsSetEnabled>
            <OffsetValue>0</OffsetValue>
          </Offset>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <Anchor1BarsAgo>0</Anchor1BarsAgo>
            <Anchor1Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor1Y>
            <Anchor2BarsAgo>0</Anchor2BarsAgo>
            <Anchor2Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor2Y>
            <Anchor3BarsAgo>0</Anchor3BarsAgo>
            <Anchor3Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor3Y>
            <AreaBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </AreaBrush>
            <AreaOpacity>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </AreaOpacity>
            <BackBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </BackBrush>
            <BarsAgo>0</BarsAgo>
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </Brush>
            <Color>
              <A>255</A>
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
            <Currency>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Currency>
            <DashStyle>Solid</DashStyle>
            <Displacement>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Displacement>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <EndBarsAgo>0</EndBarsAgo>
            <EndY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EndY>
            <EntryBarsAgo>0</EntryBarsAgo>
            <EntryY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EntryY>
            <ExtensionBarsAgo>0</ExtensionBarsAgo>
            <ExtensionY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </ExtensionY>
            <File />
            <ForegroundBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </ForegroundBrush>
            <FromEntrySignal>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Message>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Message>
            <MessageValue>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </MessageValue>
            <MiddleBarsAgo>0</MiddleBarsAgo>
            <MiddleY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </MiddleY>
            <Mode>Currency</Mode>
            <Offset>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Offset>
            <OffsetType>Currency</OffsetType>
            <Price>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Price>
            <Priority>Medium</Priority>
            <Quantity>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Children />
                <Name>Default order quantity</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:27:34.5376396</Date>
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
            </Quantity>
            <Ratio>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Ratio>
            <RearmSeconds>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </RearmSeconds>
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>DefaultSeries</SeriesType>
              <NSName>Close</NSName>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>entry</StringValue>
                </NinjaScriptString>
              </Strings>
              <SeparatorCharacter> </SeparatorCharacter>
            </SignalName>
            <SoundLocation />
            <StartBarsAgo>0</StartBarsAgo>
            <StartY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StartY>
            <StopPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StopPrice>
            <Subject>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Subject>
            <Tag>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Enter long position</StringValue>
                </NinjaScriptString>
              </Strings>
              <SeparatorCharacter> </SeparatorCharacter>
            </Tag>
            <Text>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Text>
            <TextBarsAgo>0</TextBarsAgo>
            <TextY>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </TextY>
            <To>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </To>
            <TextPosition>BottomLeft</TextPosition>
            <Value>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Value>
            <VariableInt>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableInt>
            <VariableString>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </VariableString>
            <VariableDateTime>2017-12-10T14:27:34.5376396</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableDouble>
            <Width>
              <LiveValue xsi:type="xsd:string">2</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Width>
            <Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Y>
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
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:27:12.8117724</Date>
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
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:27:12.8339093</Date>
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
      </Conditions>
      <SetName>Set 2</SetName>
      <SetNumber>2</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TriggerState</Name>
          <Offset>
            <OffsetOperator>Add</OffsetOperator>
            <OffsetType>Arithmetic</OffsetType>
            <IsSetEnabled>false</IsSetEnabled>
            <OffsetValue>0</OffsetValue>
          </Offset>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <Anchor1BarsAgo>0</Anchor1BarsAgo>
            <Anchor1Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor1Y>
            <Anchor2BarsAgo>0</Anchor2BarsAgo>
            <Anchor2Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor2Y>
            <Anchor3BarsAgo>0</Anchor3BarsAgo>
            <Anchor3Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor3Y>
            <AreaBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </AreaBrush>
            <AreaOpacity>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </AreaOpacity>
            <BackBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </BackBrush>
            <BarsAgo>0</BarsAgo>
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </Brush>
            <Color>
              <A>255</A>
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
            <Currency>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Currency>
            <DashStyle>Solid</DashStyle>
            <Displacement>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Displacement>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <EndBarsAgo>0</EndBarsAgo>
            <EndY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EndY>
            <EntryBarsAgo>0</EntryBarsAgo>
            <EntryY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EntryY>
            <ExtensionBarsAgo>0</ExtensionBarsAgo>
            <ExtensionY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </ExtensionY>
            <File />
            <ForegroundBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </ForegroundBrush>
            <FromEntrySignal>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Message>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Message>
            <MessageValue>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </MessageValue>
            <MiddleBarsAgo>0</MiddleBarsAgo>
            <MiddleY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </MiddleY>
            <Mode>Currency</Mode>
            <Offset>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Offset>
            <OffsetType>Currency</OffsetType>
            <Price>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Price>
            <Priority>Medium</Priority>
            <Quantity>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:47:00.7178537</Date>
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
            </Quantity>
            <Ratio>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Ratio>
            <RearmSeconds>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </RearmSeconds>
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>DefaultSeries</SeriesType>
              <NSName>Close</NSName>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </SignalName>
            <SoundLocation />
            <StartBarsAgo>0</StartBarsAgo>
            <StartY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StartY>
            <StopPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StopPrice>
            <Subject>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Subject>
            <Tag>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Tag>
            <Text>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Text>
            <TextBarsAgo>0</TextBarsAgo>
            <TextY>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </TextY>
            <To>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </To>
            <TextPosition>BottomLeft</TextPosition>
            <Value>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Value>
            <VariableInt>
              <LiveValue xsi:type="xsd:string">2</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableInt>
            <VariableString>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </VariableString>
            <VariableDateTime>2017-12-10T14:47:00.7178537</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableDouble>
            <Width>
              <LiveValue xsi:type="xsd:string">2</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Width>
            <Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Y>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>int</UserVariableType>
          <VariableName>TriggerState</VariableName>
        </WizardAction>
        <WizardAction>
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Children />
          <Name>Set StopPrice</Name>
          <Offset>
            <OffsetOperator>Add</OffsetOperator>
            <OffsetType>Arithmetic</OffsetType>
            <IsSetEnabled>false</IsSetEnabled>
            <OffsetValue>0</OffsetValue>
          </Offset>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <Anchor1BarsAgo>0</Anchor1BarsAgo>
            <Anchor1Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor1Y>
            <Anchor2BarsAgo>0</Anchor2BarsAgo>
            <Anchor2Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor2Y>
            <Anchor3BarsAgo>0</Anchor3BarsAgo>
            <Anchor3Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor3Y>
            <AreaBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </AreaBrush>
            <AreaOpacity>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </AreaOpacity>
            <BackBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </BackBrush>
            <BarsAgo>0</BarsAgo>
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </Brush>
            <Color>
              <A>255</A>
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
            <Currency>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Currency>
            <DashStyle>Solid</DashStyle>
            <Displacement>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Displacement>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <EndBarsAgo>0</EndBarsAgo>
            <EndY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EndY>
            <EntryBarsAgo>0</EntryBarsAgo>
            <EntryY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EntryY>
            <ExtensionBarsAgo>0</ExtensionBarsAgo>
            <ExtensionY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </ExtensionY>
            <File />
            <ForegroundBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </ForegroundBrush>
            <FromEntrySignal>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Message>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Message>
            <MessageValue>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </MessageValue>
            <MiddleBarsAgo>0</MiddleBarsAgo>
            <MiddleY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </MiddleY>
            <Mode>Currency</Mode>
            <Offset>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Offset>
            <OffsetType>Currency</OffsetType>
            <Price>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Price>
            <Priority>Medium</Priority>
            <Quantity>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Children />
                <Name>Default order quantity</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:46:30.8063732</Date>
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
            </Quantity>
            <Ratio>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Ratio>
            <RearmSeconds>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </RearmSeconds>
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>DefaultSeries</SeriesType>
              <NSName>Close</NSName>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </SignalName>
            <SoundLocation />
            <StartBarsAgo>0</StartBarsAgo>
            <StartY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StartY>
            <StopPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StopPrice>
            <Subject>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Subject>
            <Tag>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Tag>
            <Text>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Text>
            <TextBarsAgo>0</TextBarsAgo>
            <TextY>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </TextY>
            <To>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </To>
            <TextPosition>BottomLeft</TextPosition>
            <Value>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Value>
            <VariableInt>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableInt>
            <VariableString>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </VariableString>
            <VariableDateTime>2017-12-10T14:46:30.8063732</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <LiveValue xsi:type="xsd:string">(Position.AveragePrice + (InitialStopDistance * TickSize)) </LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Children />
                <Name>Average position price</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.AveragePrice</Command>
                  <Parameters>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:46:35.7497237</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                  </ConditionOffset>
                  <Offset>
                    <LiveValue xsi:type="xsd:string">InitialStopDistance</LiveValue>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <DynamicValue>
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Children />
                      <Name>InitialStopDistance</Name>
                      <Offset>
                        <OffsetOperator>Add</OffsetOperator>
                        <OffsetType>Arithmetic</OffsetType>
                        <IsSetEnabled>false</IsSetEnabled>
                        <OffsetValue>0</OffsetValue>
                      </Offset>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>InitialStopDistance</Command>
                        <Parameters />
                      </AssignedCommand>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2017-12-10T14:46:48.9647705</Date>
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
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
            </VariableDouble>
            <Width>
              <LiveValue xsi:type="xsd:string">2</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Width>
            <Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Y>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StopPrice</VariableName>
        </WizardAction>
        <WizardAction>
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Children />
          <Name>Set TriggerPrice</Name>
          <Offset>
            <OffsetOperator>Add</OffsetOperator>
            <OffsetType>Arithmetic</OffsetType>
            <IsSetEnabled>false</IsSetEnabled>
            <OffsetValue>0</OffsetValue>
          </Offset>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <Anchor1BarsAgo>0</Anchor1BarsAgo>
            <Anchor1Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor1Y>
            <Anchor2BarsAgo>0</Anchor2BarsAgo>
            <Anchor2Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor2Y>
            <Anchor3BarsAgo>0</Anchor3BarsAgo>
            <Anchor3Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor3Y>
            <AreaBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </AreaBrush>
            <AreaOpacity>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </AreaOpacity>
            <BackBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </BackBrush>
            <BarsAgo>0</BarsAgo>
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </Brush>
            <Color>
              <A>255</A>
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
            <Currency>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Currency>
            <DashStyle>Solid</DashStyle>
            <Displacement>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Displacement>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <EndBarsAgo>0</EndBarsAgo>
            <EndY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EndY>
            <EntryBarsAgo>0</EntryBarsAgo>
            <EntryY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EntryY>
            <ExtensionBarsAgo>0</ExtensionBarsAgo>
            <ExtensionY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </ExtensionY>
            <File />
            <ForegroundBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </ForegroundBrush>
            <FromEntrySignal>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Message>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Message>
            <MessageValue>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </MessageValue>
            <MiddleBarsAgo>0</MiddleBarsAgo>
            <MiddleY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </MiddleY>
            <Mode>Currency</Mode>
            <Offset>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Offset>
            <OffsetType>Currency</OffsetType>
            <Price>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Price>
            <Priority>Medium</Priority>
            <Quantity>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Children />
                <Name>Default order quantity</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:37:07.0268762</Date>
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
            </Quantity>
            <Ratio>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Ratio>
            <RearmSeconds>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </RearmSeconds>
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>DefaultSeries</SeriesType>
              <NSName>Close</NSName>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </SignalName>
            <SoundLocation />
            <StartBarsAgo>0</StartBarsAgo>
            <StartY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StartY>
            <StopPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StopPrice>
            <Subject>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Subject>
            <Tag>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Tag>
            <Text>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Text>
            <TextBarsAgo>0</TextBarsAgo>
            <TextY>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </TextY>
            <To>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </To>
            <TextPosition>BottomLeft</TextPosition>
            <Value>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Value>
            <VariableInt>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableInt>
            <VariableString>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </VariableString>
            <VariableDateTime>2017-12-10T14:37:07.0268762</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <LiveValue xsi:type="xsd:string">(Position.AveragePrice + (BreakEvenTrigger * TickSize)) </LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Children />
                <Name>Average position price</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.AveragePrice</Command>
                  <Parameters>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:37:14.8144282</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                  </ConditionOffset>
                  <Offset>
                    <LiveValue xsi:type="xsd:string">BreakEvenTrigger</LiveValue>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <DynamicValue>
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Children />
                      <Name>BreakEvenTrigger</Name>
                      <Offset>
                        <OffsetOperator>Add</OffsetOperator>
                        <OffsetType>Arithmetic</OffsetType>
                        <IsSetEnabled>false</IsSetEnabled>
                        <OffsetValue>0</OffsetValue>
                      </Offset>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>BreakEvenTrigger</Command>
                        <Parameters />
                      </AssignedCommand>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2017-12-10T14:37:25.2272272</Date>
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
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
            </VariableDouble>
            <Width>
              <LiveValue xsi:type="xsd:string">2</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Width>
            <Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Y>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>TriggerPrice</VariableName>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TriggerState</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TriggerState</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:42:25.0270541</Date>
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
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:42:25.0270541</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <LiveValue xsi:type="xsd:string">1</LiveValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <IsLiteral>true</IsLiteral>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>TriggerState = 1</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Children />
                <Name>Current market position</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:34:24.6974747</Date>
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
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Children />
                <Name>Market position</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:34:24.6974747</Date>
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
      </Conditions>
      <SetName>Set 3</SetName>
      <SetNumber>3</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TriggerState</Name>
          <Offset>
            <OffsetOperator>Add</OffsetOperator>
            <OffsetType>Arithmetic</OffsetType>
            <IsSetEnabled>false</IsSetEnabled>
            <OffsetValue>0</OffsetValue>
          </Offset>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <Anchor1BarsAgo>0</Anchor1BarsAgo>
            <Anchor1Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor1Y>
            <Anchor2BarsAgo>0</Anchor2BarsAgo>
            <Anchor2Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor2Y>
            <Anchor3BarsAgo>0</Anchor3BarsAgo>
            <Anchor3Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor3Y>
            <AreaBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </AreaBrush>
            <AreaOpacity>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </AreaOpacity>
            <BackBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </BackBrush>
            <BarsAgo>0</BarsAgo>
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </Brush>
            <Color>
              <A>255</A>
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
            <Currency>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Currency>
            <DashStyle>Solid</DashStyle>
            <Displacement>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Displacement>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <EndBarsAgo>0</EndBarsAgo>
            <EndY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EndY>
            <EntryBarsAgo>0</EntryBarsAgo>
            <EntryY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EntryY>
            <ExtensionBarsAgo>0</ExtensionBarsAgo>
            <ExtensionY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </ExtensionY>
            <File />
            <ForegroundBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </ForegroundBrush>
            <FromEntrySignal>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Message>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Message>
            <MessageValue>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </MessageValue>
            <MiddleBarsAgo>0</MiddleBarsAgo>
            <MiddleY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </MiddleY>
            <Mode>Currency</Mode>
            <Offset>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Offset>
            <OffsetType>Currency</OffsetType>
            <Price>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Price>
            <Priority>Medium</Priority>
            <Quantity>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:49:11.7463357</Date>
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
            </Quantity>
            <Ratio>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Ratio>
            <RearmSeconds>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </RearmSeconds>
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>DefaultSeries</SeriesType>
              <NSName>Close</NSName>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </SignalName>
            <SoundLocation />
            <StartBarsAgo>0</StartBarsAgo>
            <StartY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StartY>
            <StopPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StopPrice>
            <Subject>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Subject>
            <Tag>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Tag>
            <Text>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Text>
            <TextBarsAgo>0</TextBarsAgo>
            <TextY>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </TextY>
            <To>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </To>
            <TextPosition>BottomLeft</TextPosition>
            <Value>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Value>
            <VariableInt>
              <LiveValue xsi:type="xsd:string">3</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableInt>
            <VariableString>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </VariableString>
            <VariableDateTime>2017-12-10T14:49:11.7463357</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableDouble>
            <Width>
              <LiveValue xsi:type="xsd:string">2</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Width>
            <Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Y>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>int</UserVariableType>
          <VariableName>TriggerState</VariableName>
        </WizardAction>
        <WizardAction>
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Children />
          <Name>Set StopPrice</Name>
          <Offset>
            <OffsetOperator>Add</OffsetOperator>
            <OffsetType>Arithmetic</OffsetType>
            <IsSetEnabled>false</IsSetEnabled>
            <OffsetValue>0</OffsetValue>
          </Offset>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <Anchor1BarsAgo>0</Anchor1BarsAgo>
            <Anchor1Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor1Y>
            <Anchor2BarsAgo>0</Anchor2BarsAgo>
            <Anchor2Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor2Y>
            <Anchor3BarsAgo>0</Anchor3BarsAgo>
            <Anchor3Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor3Y>
            <AreaBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </AreaBrush>
            <AreaOpacity>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </AreaOpacity>
            <BackBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </BackBrush>
            <BarsAgo>0</BarsAgo>
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </Brush>
            <Color>
              <A>255</A>
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
            <Currency>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Currency>
            <DashStyle>Solid</DashStyle>
            <Displacement>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Displacement>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <EndBarsAgo>0</EndBarsAgo>
            <EndY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EndY>
            <EntryBarsAgo>0</EntryBarsAgo>
            <EntryY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EntryY>
            <ExtensionBarsAgo>0</ExtensionBarsAgo>
            <ExtensionY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </ExtensionY>
            <File />
            <ForegroundBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </ForegroundBrush>
            <FromEntrySignal>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Message>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Message>
            <MessageValue>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </MessageValue>
            <MiddleBarsAgo>0</MiddleBarsAgo>
            <MiddleY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </MiddleY>
            <Mode>Currency</Mode>
            <Offset>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Offset>
            <OffsetType>Currency</OffsetType>
            <Price>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Price>
            <Priority>Medium</Priority>
            <Quantity>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Children />
                <Name>Default order quantity</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:48:54.44036</Date>
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
            </Quantity>
            <Ratio>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Ratio>
            <RearmSeconds>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </RearmSeconds>
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>DefaultSeries</SeriesType>
              <NSName>Close</NSName>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </SignalName>
            <SoundLocation />
            <StartBarsAgo>0</StartBarsAgo>
            <StartY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StartY>
            <StopPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StopPrice>
            <Subject>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Subject>
            <Tag>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Tag>
            <Text>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Text>
            <TextBarsAgo>0</TextBarsAgo>
            <TextY>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </TextY>
            <To>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </To>
            <TextPosition>BottomLeft</TextPosition>
            <Value>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Value>
            <VariableInt>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableInt>
            <VariableString>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </VariableString>
            <VariableDateTime>2017-12-10T14:48:54.44036</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <LiveValue xsi:type="xsd:string">Position.AveragePrice</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Children />
                <Name>Average position price</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.AveragePrice</Command>
                  <Parameters>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:49:04.2073972</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                  </ConditionOffset>
                  <Offset>
                    <LiveValue xsi:type="xsd:string">0</LiveValue>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <IsLiteral>true</IsLiteral>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
            </VariableDouble>
            <Width>
              <LiveValue xsi:type="xsd:string">2</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Width>
            <Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Y>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StopPrice</VariableName>
        </WizardAction>
        <WizardAction>
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Diamond</Name>
          <Offset>
            <OffsetOperator>Add</OffsetOperator>
            <OffsetType>Arithmetic</OffsetType>
            <IsSetEnabled>false</IsSetEnabled>
            <OffsetValue>0</OffsetValue>
          </Offset>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <Anchor1BarsAgo>0</Anchor1BarsAgo>
            <Anchor1Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor1Y>
            <Anchor2BarsAgo>0</Anchor2BarsAgo>
            <Anchor2Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor2Y>
            <Anchor3BarsAgo>0</Anchor3BarsAgo>
            <Anchor3Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor3Y>
            <AreaBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </AreaBrush>
            <AreaOpacity>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </AreaOpacity>
            <BackBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </BackBrush>
            <BarsAgo>0</BarsAgo>
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
                <R>0</R>
                <G>139</G>
                <B>139</B>
                <ScA>1</ScA>
                <ScR>0</ScR>
                <ScG>0.258182883</ScG>
                <ScB>0.258182883</ScB>
              </Color>
            </Brush>
            <Color>
              <A>255</A>
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
            <Currency>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Currency>
            <DashStyle>Solid</DashStyle>
            <Displacement>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Displacement>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <EndBarsAgo>0</EndBarsAgo>
            <EndY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EndY>
            <EntryBarsAgo>0</EntryBarsAgo>
            <EntryY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EntryY>
            <ExtensionBarsAgo>0</ExtensionBarsAgo>
            <ExtensionY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </ExtensionY>
            <File />
            <ForegroundBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </ForegroundBrush>
            <FromEntrySignal>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </FromEntrySignal>
            <IsAutoScale>true</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Message>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Message>
            <MessageValue>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </MessageValue>
            <MiddleBarsAgo>0</MiddleBarsAgo>
            <MiddleY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </MiddleY>
            <Mode>Currency</Mode>
            <Offset>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Offset>
            <OffsetType>Currency</OffsetType>
            <Price>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Price>
            <Priority>Medium</Priority>
            <Quantity>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:55:16.6578949</Date>
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
            </Quantity>
            <Ratio>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Ratio>
            <RearmSeconds>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </RearmSeconds>
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>DefaultSeries</SeriesType>
              <NSName>Close</NSName>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </SignalName>
            <SoundLocation />
            <StartBarsAgo>0</StartBarsAgo>
            <StartY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StartY>
            <StopPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StopPrice>
            <Subject>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Subject>
            <Tag>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>BreakEvenBuilderExample</StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Index>1</Index>
                  <StringValue>Diamond_1</StringValue>
                </NinjaScriptString>
              </Strings>
              <SeparatorCharacter> </SeparatorCharacter>
            </Tag>
            <Text>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Text>
            <TextBarsAgo>0</TextBarsAgo>
            <TextY>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </TextY>
            <To>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </To>
            <TextPosition>BottomLeft</TextPosition>
            <Value>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Value>
            <VariableInt>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableInt>
            <VariableString>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </VariableString>
            <VariableDateTime>2017-12-10T14:55:16.6578949</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableDouble>
            <Width>
              <LiveValue xsi:type="xsd:string">2</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Width>
            <Y>
              <LiveValue xsi:type="xsd:string">(High[0] + (2 * TickSize)) </LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>High</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
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
                <Date>2017-12-10T14:55:55.5756062</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                  </ConditionOffset>
                  <Offset>
                    <LiveValue xsi:type="xsd:string">2</LiveValue>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <IsLiteral>true</IsLiteral>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <Series1>
                  <AcceptableSeries>DataSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>true</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>High</PriceType>
                  <SeriesType>DefaultSeries</SeriesType>
                  <NSName>High</NSName>
                </Series1>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
            </Y>
          </ActionProperties>
          <ActionType>Drawing</ActionType>
          <Command>
            <Command>Diamond</Command>
            <Parameters>
              <string>owner</string>
              <string>tag</string>
              <string>isAutoScale</string>
              <string>barsAgo</string>
              <string>y</string>
              <string>brush</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Diamond</Name>
        <Offset>
          <OffsetOperator>Add</OffsetOperator>
          <OffsetType>Arithmetic</OffsetType>
          <IsSetEnabled>false</IsSetEnabled>
          <OffsetValue>0</OffsetValue>
        </Offset>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <Anchor1BarsAgo>0</Anchor1BarsAgo>
          <Anchor1Y>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Anchor1Y>
          <Anchor2BarsAgo>0</Anchor2BarsAgo>
          <Anchor2Y>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Anchor2Y>
          <Anchor3BarsAgo>0</Anchor3BarsAgo>
          <Anchor3Y>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Anchor3Y>
          <AreaBrush xsi:type="SolidColorBrush">
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
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
          </AreaBrush>
          <AreaOpacity>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <IsLiteral>true</IsLiteral>
          </AreaOpacity>
          <BackBrush xsi:type="SolidColorBrush">
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
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
          </BackBrush>
          <BarsAgo>0</BarsAgo>
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
              <R>0</R>
              <G>139</G>
              <B>139</B>
              <ScA>1</ScA>
              <ScR>0</ScR>
              <ScG>0.258182883</ScG>
              <ScB>0.258182883</ScB>
            </Color>
          </Brush>
          <Color>
            <A>255</A>
            <R>100</R>
            <G>149</G>
            <B>237</B>
            <ScA>1</ScA>
            <ScR>0.127437681</ScR>
            <ScG>0.3005438</ScG>
            <ScB>0.8468732</ScB>
          </Color>
          <Currency>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Currency>
          <DashStyle>Solid</DashStyle>
          <Displacement>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <IsLiteral>true</IsLiteral>
          </Displacement>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <EndBarsAgo>0</EndBarsAgo>
          <EndY>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </EndY>
          <EntryBarsAgo>0</EntryBarsAgo>
          <EntryY>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </EntryY>
          <ExtensionBarsAgo>0</ExtensionBarsAgo>
          <ExtensionY>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </ExtensionY>
          <File />
          <ForegroundBrush xsi:type="SolidColorBrush">
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
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
          </ForegroundBrush>
          <FromEntrySignal>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </FromEntrySignal>
          <IsAutoScale>true</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LimitPrice>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </LimitPrice>
          <LogLevel>Information</LogLevel>
          <Message>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </Message>
          <MessageValue>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </MessageValue>
          <MiddleBarsAgo>0</MiddleBarsAgo>
          <MiddleY>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </MiddleY>
          <Mode>Currency</Mode>
          <Offset>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <IsLiteral>true</IsLiteral>
          </Offset>
          <OffsetType>Currency</OffsetType>
          <Price>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Price>
          <Priority>Medium</Priority>
          <Quantity>
            <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <DynamicValue>
              <IsExpanded>false</IsExpanded>
              <IsSelected>false</IsSelected>
              <Name>Default order quantity</Name>
              <Offset>
                <OffsetOperator>Add</OffsetOperator>
                <OffsetType>Arithmetic</OffsetType>
                <IsSetEnabled>false</IsSetEnabled>
                <OffsetValue>0</OffsetValue>
              </Offset>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>DefaultQuantity</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2017-12-10T14:55:16.6578949</Date>
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
          </Quantity>
          <Ratio>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Ratio>
          <RearmSeconds>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <IsLiteral>true</IsLiteral>
          </RearmSeconds>
          <Series>
            <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
            <CustomProperties />
            <IsExplicitlyNamed>false</IsExplicitlyNamed>
            <IsPriceTypeLocked>false</IsPriceTypeLocked>
            <PlotOnChart>false</PlotOnChart>
            <PriceType>Close</PriceType>
            <SeriesType>DefaultSeries</SeriesType>
            <NSName>Close</NSName>
          </Series>
          <ServiceName />
          <ScreenshotPath />
          <SignalName>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </SignalName>
          <SoundLocation />
          <StartBarsAgo>0</StartBarsAgo>
          <StartY>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </StartY>
          <StopPrice>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </StopPrice>
          <Subject>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </Subject>
          <Tag>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>BreakEvenBuilderExample</StringValue>
              </NinjaScriptString>
              <NinjaScriptString>
                <Index>1</Index>
                <StringValue>Diamond_1</StringValue>
              </NinjaScriptString>
            </Strings>
            <SeparatorCharacter> </SeparatorCharacter>
          </Tag>
          <Text>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </Text>
          <TextBarsAgo>0</TextBarsAgo>
          <TextY>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </TextY>
          <To>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </To>
          <TextPosition>BottomLeft</TextPosition>
          <Value>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Value>
          <VariableInt>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <IsLiteral>true</IsLiteral>
          </VariableInt>
          <VariableString>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </VariableString>
          <VariableDateTime>2017-12-10T14:55:16.6578949</VariableDateTime>
          <VariableBool>false</VariableBool>
          <VariableDouble>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </VariableDouble>
          <Width>
            <LiveValue xsi:type="xsd:string">2</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <IsLiteral>true</IsLiteral>
          </Width>
          <Y>
            <LiveValue xsi:type="xsd:string">(High[0] + (2 * TickSize)) </LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <DynamicValue>
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>High</Name>
              <Offset>
                <OffsetOperator>Add</OffsetOperator>
                <OffsetType>Arithmetic</OffsetType>
                <IsSetEnabled>false</IsSetEnabled>
                <OffsetValue>0</OffsetValue>
              </Offset>
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
              <Date>2017-12-10T14:55:55.5756062</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <OffsetBuilder>
                <ConditionOffset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Ticks</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </ConditionOffset>
                <Offset>
                  <LiveValue xsi:type="xsd:string">2</LiveValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <IsLiteral>true</IsLiteral>
                </Offset>
              </OffsetBuilder>
              <Period>0</Period>
              <ReturnType>Series</ReturnType>
              <Series1>
                <AcceptableSeries>DataSeries DefaultSeries</AcceptableSeries>
                <CustomProperties />
                <IsExplicitlyNamed>false</IsExplicitlyNamed>
                <IsPriceTypeLocked>true</IsPriceTypeLocked>
                <PlotOnChart>false</PlotOnChart>
                <PriceType>High</PriceType>
                <SeriesType>DefaultSeries</SeriesType>
                <NSName>High</NSName>
              </Series1>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
          </Y>
        </ActionProperties>
        <ActionType>Drawing</ActionType>
        <Command>
          <Command>Diamond</Command>
          <Parameters>
            <string>owner</string>
            <string>tag</string>
            <string>isAutoScale</string>
            <string>barsAgo</string>
            <string>y</string>
            <string>brush</string>
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
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TriggerState</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TriggerState</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:48:20.9650529</Date>
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
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:48:20.9806326</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <LiveValue xsi:type="xsd:string">2</LiveValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <IsLiteral>true</IsLiteral>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>TriggerState = 2</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
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
                <Date>2017-12-10T14:48:33.4369853</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                  </ConditionOffset>
                  <Offset>
                    <LiveValue xsi:type="xsd:string">0</LiveValue>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <IsLiteral>true</IsLiteral>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <Series1>
                  <AcceptableSeries>DataSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>true</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>DefaultSeries</SeriesType>
                  <NSName>Close</NSName>
                </Series1>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TriggerPrice</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TriggerPrice</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:48:33.4526651</Date>
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
          <DisplayName>Default input[0] &gt;= TriggerPrice</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 4</SetName>
      <SetNumber>4</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position by a stop order</Name>
          <Offset>
            <OffsetOperator>Add</OffsetOperator>
            <OffsetType>Arithmetic</OffsetType>
            <IsSetEnabled>false</IsSetEnabled>
            <OffsetValue>0</OffsetValue>
          </Offset>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <Anchor1BarsAgo>0</Anchor1BarsAgo>
            <Anchor1Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor1Y>
            <Anchor2BarsAgo>0</Anchor2BarsAgo>
            <Anchor2Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor2Y>
            <Anchor3BarsAgo>0</Anchor3BarsAgo>
            <Anchor3Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor3Y>
            <AreaBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </AreaBrush>
            <AreaOpacity>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </AreaOpacity>
            <BackBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </BackBrush>
            <BarsAgo>0</BarsAgo>
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </Brush>
            <Color>
              <A>255</A>
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
            <Currency>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Currency>
            <DashStyle>Solid</DashStyle>
            <Displacement>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Displacement>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <EndBarsAgo>0</EndBarsAgo>
            <EndY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EndY>
            <EntryBarsAgo>0</EntryBarsAgo>
            <EntryY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EntryY>
            <ExtensionBarsAgo>0</ExtensionBarsAgo>
            <ExtensionY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </ExtensionY>
            <File />
            <ForegroundBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </ForegroundBrush>
            <FromEntrySignal>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>entry</StringValue>
                </NinjaScriptString>
              </Strings>
              <SeparatorCharacter> </SeparatorCharacter>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Message>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Message>
            <MessageValue>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </MessageValue>
            <MiddleBarsAgo>0</MiddleBarsAgo>
            <MiddleY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </MiddleY>
            <Mode>Currency</Mode>
            <Offset>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Offset>
            <OffsetType>Currency</OffsetType>
            <Price>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Price>
            <Priority>Medium</Priority>
            <Quantity>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:49:48.4669686</Date>
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
            </Quantity>
            <Ratio>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Ratio>
            <RearmSeconds>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </RearmSeconds>
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>DefaultSeries</SeriesType>
              <NSName>Close</NSName>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>exit</StringValue>
                </NinjaScriptString>
              </Strings>
              <SeparatorCharacter> </SeparatorCharacter>
            </SignalName>
            <SoundLocation />
            <StartBarsAgo>0</StartBarsAgo>
            <StartY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StartY>
            <StopPrice>
              <LiveValue xsi:type="xsd:string">StopPrice</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StopPrice</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StopPrice</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:50:11.419517</Date>
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
            </StopPrice>
            <Subject>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Subject>
            <Tag>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit long position by a stop order</StringValue>
                </NinjaScriptString>
              </Strings>
              <SeparatorCharacter> </SeparatorCharacter>
            </Tag>
            <Text>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Text>
            <TextBarsAgo>0</TextBarsAgo>
            <TextY>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </TextY>
            <To>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </To>
            <TextPosition>BottomLeft</TextPosition>
            <Value>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Value>
            <VariableInt>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableInt>
            <VariableString>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </VariableString>
            <VariableDateTime>2017-12-10T14:49:48.4669686</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableDouble>
            <Width>
              <LiveValue xsi:type="xsd:string">2</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Width>
            <Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Y>
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
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TriggerState</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TriggerState</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:49:23.2718609</Date>
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
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:49:23.2935529</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <LiveValue xsi:type="xsd:string">2</LiveValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <IsLiteral>true</IsLiteral>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>TriggerState &gt;= 2</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 5</SetName>
      <SetNumber>5</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TriggerState</Name>
          <Offset>
            <OffsetOperator>Add</OffsetOperator>
            <OffsetType>Arithmetic</OffsetType>
            <IsSetEnabled>false</IsSetEnabled>
            <OffsetValue>0</OffsetValue>
          </Offset>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <Anchor1BarsAgo>0</Anchor1BarsAgo>
            <Anchor1Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor1Y>
            <Anchor2BarsAgo>0</Anchor2BarsAgo>
            <Anchor2Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor2Y>
            <Anchor3BarsAgo>0</Anchor3BarsAgo>
            <Anchor3Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Anchor3Y>
            <AreaBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </AreaBrush>
            <AreaOpacity>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </AreaOpacity>
            <BackBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </BackBrush>
            <BarsAgo>0</BarsAgo>
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </Brush>
            <Color>
              <A>255</A>
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
            <Currency>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Currency>
            <DashStyle>Solid</DashStyle>
            <Displacement>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Displacement>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <EndBarsAgo>0</EndBarsAgo>
            <EndY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EndY>
            <EntryBarsAgo>0</EntryBarsAgo>
            <EntryY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </EntryY>
            <ExtensionBarsAgo>0</ExtensionBarsAgo>
            <ExtensionY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </ExtensionY>
            <File />
            <ForegroundBrush xsi:type="SolidColorBrush">
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
                <R>100</R>
                <G>149</G>
                <B>237</B>
                <ScA>1</ScA>
                <ScR>0.127437681</ScR>
                <ScG>0.3005438</ScG>
                <ScB>0.8468732</ScB>
              </Color>
            </ForegroundBrush>
            <FromEntrySignal>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Message>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Message>
            <MessageValue>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </MessageValue>
            <MiddleBarsAgo>0</MiddleBarsAgo>
            <MiddleY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </MiddleY>
            <Mode>Currency</Mode>
            <Offset>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Offset>
            <OffsetType>Currency</OffsetType>
            <Price>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Price>
            <Priority>Medium</Priority>
            <Quantity>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <DynamicValue>
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:52:32.6905717</Date>
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
            </Quantity>
            <Ratio>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Ratio>
            <RearmSeconds>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </RearmSeconds>
            <Series>
              <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
              <CustomProperties />
              <IsExplicitlyNamed>false</IsExplicitlyNamed>
              <IsPriceTypeLocked>false</IsPriceTypeLocked>
              <PlotOnChart>false</PlotOnChart>
              <PriceType>Close</PriceType>
              <SeriesType>DefaultSeries</SeriesType>
              <NSName>Close</NSName>
            </Series>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </SignalName>
            <SoundLocation />
            <StartBarsAgo>0</StartBarsAgo>
            <StartY>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StartY>
            <StopPrice>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </StopPrice>
            <Subject>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Subject>
            <Tag>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Tag>
            <Text>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </Text>
            <TextBarsAgo>0</TextBarsAgo>
            <TextY>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </TextY>
            <To>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </To>
            <TextPosition>BottomLeft</TextPosition>
            <Value>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Value>
            <VariableInt>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableInt>
            <VariableString>
              <Strings />
              <SeparatorCharacter> </SeparatorCharacter>
            </VariableString>
            <VariableDateTime>2017-12-10T14:52:32.6905717</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </VariableDouble>
            <Width>
              <LiveValue xsi:type="xsd:string">2</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <IsLiteral>true</IsLiteral>
            </Width>
            <Y>
              <LiveValue xsi:type="xsd:string">0</LiveValue>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <IsLiteral>true</IsLiteral>
            </Y>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>int</UserVariableType>
          <VariableName>TriggerState</VariableName>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Set TriggerState</Name>
        <Offset>
          <OffsetOperator>Add</OffsetOperator>
          <OffsetType>Arithmetic</OffsetType>
          <IsSetEnabled>false</IsSetEnabled>
          <OffsetValue>0</OffsetValue>
        </Offset>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <Anchor1BarsAgo>0</Anchor1BarsAgo>
          <Anchor1Y>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Anchor1Y>
          <Anchor2BarsAgo>0</Anchor2BarsAgo>
          <Anchor2Y>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Anchor2Y>
          <Anchor3BarsAgo>0</Anchor3BarsAgo>
          <Anchor3Y>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Anchor3Y>
          <AreaBrush xsi:type="SolidColorBrush">
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
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
          </AreaBrush>
          <AreaOpacity>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <IsLiteral>true</IsLiteral>
          </AreaOpacity>
          <BackBrush xsi:type="SolidColorBrush">
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
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
          </BackBrush>
          <BarsAgo>0</BarsAgo>
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
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
          </Brush>
          <Color>
            <A>255</A>
            <R>100</R>
            <G>149</G>
            <B>237</B>
            <ScA>1</ScA>
            <ScR>0.127437681</ScR>
            <ScG>0.3005438</ScG>
            <ScB>0.8468732</ScB>
          </Color>
          <Currency>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Currency>
          <DashStyle>Solid</DashStyle>
          <Displacement>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <IsLiteral>true</IsLiteral>
          </Displacement>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <EndBarsAgo>0</EndBarsAgo>
          <EndY>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </EndY>
          <EntryBarsAgo>0</EntryBarsAgo>
          <EntryY>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </EntryY>
          <ExtensionBarsAgo>0</ExtensionBarsAgo>
          <ExtensionY>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </ExtensionY>
          <File />
          <ForegroundBrush xsi:type="SolidColorBrush">
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
              <R>100</R>
              <G>149</G>
              <B>237</B>
              <ScA>1</ScA>
              <ScR>0.127437681</ScR>
              <ScG>0.3005438</ScG>
              <ScB>0.8468732</ScB>
            </Color>
          </ForegroundBrush>
          <FromEntrySignal>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </FromEntrySignal>
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LimitPrice>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </LimitPrice>
          <LogLevel>Information</LogLevel>
          <Message>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </Message>
          <MessageValue>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </MessageValue>
          <MiddleBarsAgo>0</MiddleBarsAgo>
          <MiddleY>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </MiddleY>
          <Mode>Currency</Mode>
          <Offset>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <IsLiteral>true</IsLiteral>
          </Offset>
          <OffsetType>Currency</OffsetType>
          <Price>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Price>
          <Priority>Medium</Priority>
          <Quantity>
            <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <DynamicValue>
              <IsExpanded>false</IsExpanded>
              <IsSelected>false</IsSelected>
              <Name>Default order quantity</Name>
              <Offset>
                <OffsetOperator>Add</OffsetOperator>
                <OffsetType>Arithmetic</OffsetType>
                <IsSetEnabled>false</IsSetEnabled>
                <OffsetValue>0</OffsetValue>
              </Offset>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>DefaultQuantity</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2017-12-10T14:52:32.6905717</Date>
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
          </Quantity>
          <Ratio>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Ratio>
          <RearmSeconds>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <IsLiteral>true</IsLiteral>
          </RearmSeconds>
          <Series>
            <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
            <CustomProperties />
            <IsExplicitlyNamed>false</IsExplicitlyNamed>
            <IsPriceTypeLocked>false</IsPriceTypeLocked>
            <PlotOnChart>false</PlotOnChart>
            <PriceType>Close</PriceType>
            <SeriesType>DefaultSeries</SeriesType>
            <NSName>Close</NSName>
          </Series>
          <ServiceName />
          <ScreenshotPath />
          <SignalName>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </SignalName>
          <SoundLocation />
          <StartBarsAgo>0</StartBarsAgo>
          <StartY>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </StartY>
          <StopPrice>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </StopPrice>
          <Subject>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </Subject>
          <Tag>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </Tag>
          <Text>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </Text>
          <TextBarsAgo>0</TextBarsAgo>
          <TextY>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </TextY>
          <To>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </To>
          <TextPosition>BottomLeft</TextPosition>
          <Value>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Value>
          <VariableInt>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <IsLiteral>true</IsLiteral>
          </VariableInt>
          <VariableString>
            <Strings />
            <SeparatorCharacter> </SeparatorCharacter>
          </VariableString>
          <VariableDateTime>2017-12-10T14:52:32.6905717</VariableDateTime>
          <VariableBool>false</VariableBool>
          <VariableDouble>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </VariableDouble>
          <Width>
            <LiveValue xsi:type="xsd:string">2</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <IsLiteral>true</IsLiteral>
          </Width>
          <Y>
            <LiveValue xsi:type="xsd:string">0</LiveValue>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <IsLiteral>true</IsLiteral>
          </Y>
        </ActionProperties>
        <ActionType>SetValue</ActionType>
        <UserVariableType>int</UserVariableType>
        <VariableName>TriggerState</VariableName>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TriggerState</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TriggerState</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:52:04.4781986</Date>
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
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:52:04.4938305</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <LiveValue xsi:type="xsd:string">2</LiveValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <IsLiteral>true</IsLiteral>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>TriggerState &gt;= 2</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:52:21.050474</Date>
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
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <Offset>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Arithmetic</OffsetType>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                </Offset>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2017-12-10T14:52:21.0534771</Date>
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
      </Conditions>
      <SetName>Set 1</SetName>
      <SetNumber>1</SetNumber>
    </ConditionalAction>
  </ConditionalActions>
  <CustomSeries />
  <DataSeries />
  <Description />
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
      <Default>5</Default>
      <Description>Number of ticks above entry the breakeven movement trigger is set</Description>
      <Name>BreakEvenTrigger</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>-10</Default>
      <Description>(use a negative) Number of ticks from entry the stop will initially be placed below</Description>
      <Name>InitialStopDistance</Name>
      <Minimum>-999</Minimum>
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
      <Name>StopPrice</Name>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>0</Default>
      <Name>TriggerPrice</Name>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>0</Default>
      <Name>TriggerState</Name>
      <Type>int</Type>
    </InputParameter>
  </Variables>
  <Name>BreakEvenBuilderExample</Name>
</ScriptProperties>
@*/
#endregion
