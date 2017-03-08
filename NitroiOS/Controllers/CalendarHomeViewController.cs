using System;
using UIKit;
using PortableLibrary;
using Xuni.iOS.Core;
using Xuni.iOS.ChartCore;
using Xuni.iOS.FlexChart;

namespace location2
{
    public partial class CalendarHomeViewController : BaseViewController
    {
		nfloat ALPHA_FILL = 0.6f;
		nfloat ALPHA_AXIS = 0.3f;

		ReportGraphData pData;

        public CalendarHomeViewController (IntPtr handle) : base (handle)
        {
        }
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			XuniLicenseManager.Key = License.Key;

			viewCycle.Alpha = 0;
			viewRunning.Alpha = 0;
			viewSwimming.Alpha = 0;
			heightCycle.Constant = 0;
			heightRunning.Constant = 0;
			heightSwimming.Constant = 0;

			if (!IsNetEnable()) return;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			InitPerformanceGraph();
			InitGaugeData();
		}

		void InitPerformanceGraph()
		{
			pData = GetPerformance();

			if (pData == null) return;

			mPChart.Palette = XuniPalettes.Modern;
			mPChart.BackgroundColor = UIColor.Clear;
			mPChart.ChartType = ChartType.SplineArea;
			mPChart.BindingX = pData.categoryField;
			mPChart.IsAnimated = false;
			mPChart.ItemsSource = pData.GetSalesDataList();
			mPChart.SymbolSize = 3;

			mPChart.Legend.Position = Position.None;

			mPChart.AxisX.LabelsVisible = false;
			mPChart.AxisX.MajorTickWidth = 0;
			mPChart.AxisX.LineWidth = 0.5f;

			mPChart.AxisY.LabelsVisible = false;
			mPChart.AxisY.LineColor = UIColor.Orange.ColorWithAlpha(ALPHA_AXIS);
			mPChart.AxisY.MajorTickWidth = 0;
			mPChart.AxisY.MajorGridVisible = false;
			mPChart.AxisY.LabelsVisible = false;
			mPChart.AxisY.LineWidth = 2;

			for (int i = 0; i < pData.valueAxes.Count; i++)
			{
				var axis = pData.valueAxes[i];
				Axis cAxis = new Axis(i % 2 == 0 ? Position.Left : Position.Right, mPChart);
				cAxis.AxisName = axis.id;
				cAxis.LineColor = FromHexString(axis.axisColor);
				cAxis.MajorTickWidth = 0;
				cAxis.MajorGridVisible = false;
				cAxis.LabelsVisible = false;
				cAxis.AxisLineVisible = false;

				mPChart.AxesArray.Add(cAxis);
			}

			foreach (var series in pData.graphs)
			{
				Series cSeries = new Series(mPChart, series.valueField, series.title);
				cSeries.AxisYname = series.valueAxis;

				UIColor sColor = FromHexString(series.lineColor);

				switch (series.valueField)
				{
					case "tsb":
						symTSB.BackgroundColor = sColor;
						break;
					case "atl":
						symATL.BackgroundColor = sColor;
						break;
					case "ctl":
						symCTL.BackgroundColor = sColor;
						break;
					case "dayliTss":
						symDailyLoad.BackgroundColor = sColor;
						break;
					case "dayliIf":
						symDailyIF.BackgroundColor = sColor;
						break;
				}

				if (series.lineAlpha.Equals(0))
				{
					cSeries.ChartType = ChartType.Scatter;
					cSeries.SymbolColor = sColor;
					cSeries.Color = sColor;
				}
				else
				{
					cSeries.BorderWidth = 1;
					cSeries.BorderColor = sColor;
					cSeries.Color = sColor;
					cSeries.Opacity = ALPHA_FILL;
				}

				mPChart.Series.Add(cSeries);
			}
			mPChart.ItemsSource = pData.GetSalesDataList();
			if (pData.TodayIndex() != -1)
			{
				var start = new XuniPoint(pData.TodayPosition() * mPChart.AxisX.ActualDataMax, mPChart.AxisY.ActualDataMax);
				var end = new XuniPoint(pData.TodayPosition() * mPChart.AxisX.ActualDataMax, mPChart.AxisY.ActualDataMin);
				XuniChartLineAnnotation today = new XuniChartLineAnnotation(mPChart)
				{
					IsVisible = true,
					Attachment = XuniChartAnnotationAttachment.DataCoordinate,
					Start = start,
					End = end,
					LineWidth = 3,
					TooltipText = "Future planned performance",
					Color = UIColor.Black,
				};

				mPChart.Annotations.Add(today);
			}

			var annoFocused = new XuniChartRectangleAnnotation(mPChart)
			{
				IsVisible = false,
				Position = XuniChartAnnotationPosition.Center,
				Attachment = XuniChartAnnotationAttachment.DataCoordinate,
				Width = 2,
				Height = mPChart.AxisY.ActualDataMax,
				BorderWidth = 0,
				Text = DateTime.Now.ToString(),
				TextColor = UIColor.White,
				Color = UIColor.White,
			};

			mPChart.Annotations.Add(annoFocused);

			#region custom tooltip
			mPChart.Tooltip.Content = new MyTooltip(txtTSB, txtATL, txtCTL, txtDailyTSS, txtDailyIF, pData, annoFocused, mPChart);
			#endregion

			mPChart.ZoomMode = ZoomMode.Disabled;
			mPChart.AxisX.Scale = 1;

			zoomSlider.LowerValueChanged += HanelerGraphZoomChanged;
			zoomSlider.UpperValueChanged += HanelerGraphZoomChanged;
			zoomSlider.LowerValue = 0;
			zoomSlider.UpperValue = pData.dataProvider.Count;
			zoomSlider.MinimumValue = 0;
			zoomSlider.MaximumValue = pData.dataProvider.Count;
			zoomSlider.MinimumRange = 1;
		}

		void InitGaugeData()
		{
			var gaugeData = GetGauge();

			lblCycleDuration.Text = FormatNumber(gaugeData.Bike[0].value) + "%";
			lblRunDuration.Text = FormatNumber(gaugeData.Run[0].value) + "%";
			lblSwimDuration.Text = FormatNumber(gaugeData.Swim[0].value) + "%";

			lblCycleDistance.Text = FormatNumber(gaugeData.Bike[1].value) + "%";
			lblRunDistance.Text = FormatNumber(gaugeData.Bike[1].value) + "%";
			lblSwimDistance.Text = FormatNumber(gaugeData.Bike[1].value) + "%";

			lblCycleStress.Text = FormatNumber(gaugeData.Bike[2].value) + "%";
			lblRunStress.Text = FormatNumber(gaugeData.Bike[2].value) + "%";
			lblSwimStress.Text = FormatNumber(gaugeData.Bike[2].value) + "%";
		}

		partial void ActionViewCalendar(UIButton sender)
		{
			EventCalendarViewController eventVC = Storyboard.InstantiateViewController("EventCalendarViewController") as EventCalendarViewController;
			NavigationController.PushViewController(eventVC, true);
		}

		partial void ActionCollect(UIButton sender)
		{
			this.View.LayoutIfNeeded();

			UIView.BeginAnimations("ds");
			UIView.SetAnimationDuration(0.5f);

			var constant = sender.Selected ? 0 : 130;
			var alpha = sender.Selected ? 0 : 1;
			switch (sender.Tag)
			{
				case 0:
					viewCycle.Alpha = alpha;
					heightCycle.Constant = constant;
					btnCycleColleps.Selected = !sender.Selected;
					break;
				case 1:
					viewRunning.Alpha = alpha;
					heightRunning.Constant = constant;
					btnRunningColleps.Selected = !sender.Selected;
					break;
				case 2:
					viewSwimming.Alpha = alpha;
					heightSwimming.Constant = constant;
					btnSwimmingColleps.Selected = !sender.Selected;
					break;
				default:
					break;
			}

			View.LayoutIfNeeded();
			UIView.CommitAnimations();

			sender.Selected = !sender.Selected;
		}
		#region Handler
		void HanelerGraphZoomChanged(object sender, EventArgs e)
		{
			var rSlider = sender as RangeSliderControl;

			var gZoomLevel = (rSlider.UpperValue - rSlider.LowerValue) / rSlider.MaximumValue;//.GetAbsoluteMaxValue();
			mPChart.AxisX.Scale = gZoomLevel;
			var posX = rSlider.LowerValue * pData.dataProvider.Count;
			mPChart.AxisX.ScrollTo(posX, XuniAxisScrollPosition.Max);
		}
		#endregion
    }

	#region custom classes
	public class MyTooltip : XuniBaseChartTooltipView
	{
		UILabel txtTSB, txtATL, txtCTL, txtDailyTSS, txtDailyIF;
		XuniChartRectangleAnnotation mAnnoFocused;
		ReportGraphData mData;
		FlexChart mChart;


		public MyTooltip(UILabel txtTSB, UILabel txtATL, UILabel txtCTL, UILabel txtDailyTSS, UILabel txtDailyIF, ReportGraphData data, XuniChartRectangleAnnotation annoFocused, FlexChart chart)
		{
			this.txtTSB = txtTSB;
			this.txtATL = txtATL;
			this.txtCTL = txtCTL;
			this.txtDailyTSS = txtDailyTSS;
			this.txtDailyIF = txtDailyIF;
			mAnnoFocused = annoFocused;
			mData = data;
			mChart = chart;
		}

		public override XuniDataPoint ChartData
		{
			get
			{
				return base.ChartData;
			}
			set
			{
				base.ChartData = value;
				XuniChartDataPoint point = (XuniChartDataPoint)value;

				var start = new XuniPoint(point.PointIndex, mChart.AxisY.ActualDataMax);
				var data = mData.dataProvider[point.PointIndex];

				mAnnoFocused.Point = start;
				mAnnoFocused.Text = data.date;
				mAnnoFocused.Position = XuniChartAnnotationPosition.Bottom;
	            mAnnoFocused.IsVisible = true;

				txtTSB.Text = String.Format("TSB: {0}", data.tsb);
				txtATL.Text = String.Format("ATL: {0}", data.atl);
				txtCTL.Text = String.Format("CTL: {0}", data.ctl);
				txtDailyTSS.Text = String.Format("Daily Load: {0}", data.dayliTss);
				txtDailyIF.Text = String.Format("Daily IF: {0}", data.dayliIf);
			}
		}

		public override void Render()
		{
		}
	}
	#endregion
}