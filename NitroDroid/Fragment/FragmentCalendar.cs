﻿using System;
using Android.Animation;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.GrapeCity.Xuni.FlexChart;
using Com.GrapeCity.Xuni.Core;
using PortableLibrary;

using EventArgs = System.EventArgs;
using Com.GrapeCity.Xuni.ChartCore;
using Android.Graphics;

namespace goheja
{
	public class FragmentCalendar : Android.Support.V4.App.Fragment
	{
		int ALPHA_FILL = Convert.ToInt32(0.3 * 255);
		int ALPHA_AXIS = Convert.ToInt32(0.8 * 255);

		SwipeTabActivity rootActivity;

		public View mView;

		RangeSliderControl zoomSlider;

		FlexChart mPChart;
		ChartRectangleAnnotation annoFocused = new ChartRectangleAnnotation();

		TextView lblCycleDuration, lblRunDuration, lblSwimDuration, lblCycleDistance, lblRunDistance, lblSwimDistance, lblCycleStress, lblRunStress, lblSwimStress;
		ImageView btnCycle, btnRun, btnSwim;
		LinearLayout viewCycle, viewRun, viewSwim;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			//LicenseManager.Key = License.Key;

			rootActivity = this.Activity as SwipeTabActivity;
			return inflater.Inflate(Resource.Layout.fCalendar, container, false);
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
			mView = view;

			SetUIVariablesAndActions();

			if (!rootActivity.IsNetEnable()) return;
		}

		public override void OnResume()
		{
			base.OnResume();

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				rootActivity.ShowLoadingView("Loading data...");

				var performanceData = rootActivity.GetPerformance();
				var gaugeData = rootActivity.GetGauge();

				rootActivity.HideLoadingView();

				rootActivity.RunOnUiThread(() =>
				{
					InitPerformanceGraph(performanceData);
					InitGaugeData(gaugeData);
				});
			});
		}

		private void SetUIVariablesAndActions()
		{
			#region UI Variables
			zoomSlider = mView.FindViewById<RangeSliderControl>(Resource.Id.zoomSlider);
			zoomSlider.SetBarHeight(8);
			zoomSlider.AlwaysActive = false;
			zoomSlider.DefaultColor = Color.Gray;
			zoomSlider.ShowTextAboveThumbs = false;
			zoomSlider.ActiveColor = Color.Rgb(230, 160, 11);

			lblCycleDuration = mView.FindViewById<TextView>(Resource.Id.lblCycleDuration);
			lblRunDuration = mView.FindViewById<TextView>(Resource.Id.lblRunDuration);
			lblSwimDuration = mView.FindViewById<TextView>(Resource.Id.lblSwimDuration);
			lblCycleDistance = mView.FindViewById<TextView>(Resource.Id.lblCycleDistance);
			lblRunDistance = mView.FindViewById<TextView>(Resource.Id.lblRunDistance);
			lblSwimDistance = mView.FindViewById<TextView>(Resource.Id.lblSwimDistance);
			lblCycleStress = mView.FindViewById<TextView>(Resource.Id.lblCycleStress);
			lblRunStress = mView.FindViewById<TextView>(Resource.Id.lblRunStress);
			lblSwimStress = mView.FindViewById<TextView>(Resource.Id.lblSwimStress);

			btnCycle = mView.FindViewById<ImageView>(Resource.Id.btnCycle);
			btnRun = mView.FindViewById<ImageView>(Resource.Id.btnRun);
			btnSwim = mView.FindViewById<ImageView>(Resource.Id.btnSwim);

			viewCycle = mView.FindViewById<LinearLayout>(Resource.Id.viewCycle);
			viewRun = mView.FindViewById<LinearLayout>(Resource.Id.viewRun);
			viewSwim = mView.FindViewById<LinearLayout>(Resource.Id.viewSwim);

			CollepseAnimation(viewCycle);
			CollepseAnimation(viewRun);
			CollepseAnimation(viewSwim);
			#endregion

			#region Actions
			zoomSlider.LowerValueChanged += HanelerGraphZoomChanged;
			zoomSlider.UpperValueChanged += HanelerGraphZoomChanged;

			mView.FindViewById<RelativeLayout>(Resource.Id.collapsCycle).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapsRun).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapsSwim).Click += ActionCollepse;
			mView.FindViewById<Button>(Resource.Id.ActionViewCalendar).Click += ActionViewCalendar;

			//toggle series visibility
			//mView.FindViewById<LinearLayout>(Resource.Id.ActionToggleTSB).Click += ActionToggleSeries;
			//mView.FindViewById<LinearLayout>(Resource.Id.ActionToggleATL).Click += ActionToggleSeries;
			//mView.FindViewById<LinearLayout>(Resource.Id.ActionToggleCTL).Click += ActionToggleSeries;
			//mView.FindViewById<LinearLayout>(Resource.Id.ActionToggleDailyLoad).Click += ActionToggleSeries;
			//mView.FindViewById<LinearLayout>(Resource.Id.ActionToggleDailyIf).Click += ActionToggleSeries;
			#endregion
		}


		void InitPerformanceGraph(ReportGraphData pData)
		{
			mView.FindViewById<ScrollView>(Resource.Id.scrollView).ScrollTo(0, 0);
			if (pData == null) return;

			mPChart = mView.FindViewById<FlexChart>(Resource.Id.pChart);

			//var mainAxisX = mPChart.AxisX;
			//var mainAxisY = mPChart.AxisY;

			//mPChart.Axes.Clear();
			//mPChart.Series.Clear();
			//mPChart.Annotations.Clear();

			//mPChart.AxisX = mainAxisX;
			//mPChart.AxisY = mainAxisY;


			#region configure
			mPChart.SetPalette(Palettes.Modern);
			mPChart.SetBackgroundColor(Color.Transparent);
			mPChart.ChartType = ChartType.Splinearea;
			mPChart.BindingX = pData.categoryField;
			mPChart.Animated = false;
			#endregion

			#region regend
			mPChart.Legend.Position = ChartPositionType.None;
			#endregion

			#region axis
			mPChart.AxisX.LabelsVisible = false;
			mPChart.AxisX.MajorTickWidth = 0;
			mPChart.AxisX.LineWidth = 0.5f;

			mPChart.AxisY.LabelsVisible = false;
			mPChart.AxisY.LineColor = new Color(Color.Orange.R, Color.Orange.G, Color.Orange.B, ALPHA_AXIS);
			mPChart.AxisY.LineWidth = 2;

			for (int i = 0; i < pData.valueAxes.Count; i++)
			{
				var axis = pData.valueAxes[i];
				ChartAxis cAxis = new ChartAxis(mPChart, i % 2 == 0 ? ChartPositionType.Left : ChartPositionType.Right);
				cAxis.Name = axis.id;
				cAxis.LineColor = Color.ParseColor(axis.axisColor);
				cAxis.MajorTickWidth = 0;
				cAxis.MajorGridVisible = false;
				cAxis.LabelsVisible = false;
				cAxis.AxisLineVisible = false;

				mPChart.Axes.Add(cAxis);
			}
			#endregion

			#region series
			foreach (var series in pData.graphs)
			{
				ChartSeries cSeries = new ChartSeries(mPChart, series.title, series.valueField);
				cSeries.AxisY = series.valueAxis;

				Color sColor = Color.ParseColor(series.lineColor);
				var sRGBA = new Color(sColor.R, sColor.G, sColor.B, ALPHA_FILL);
				cSeries.SetColor(new Java.Lang.Integer(sRGBA.ToArgb()));

				ImageView imgSymbol = new ImageView(this.Context);
				switch (series.valueField)
				{
					case "tsb":
						imgSymbol = mView.FindViewById<ImageView>(Resource.Id.symTSB);
						break;
					case "atl":
						imgSymbol = mView.FindViewById<ImageView>(Resource.Id.symATL);
						break;
					case "ctl":
						imgSymbol = mView.FindViewById<ImageView>(Resource.Id.symCTL);
						break;
					case "dayliTss":
						imgSymbol = mView.FindViewById<ImageView>(Resource.Id.symDailyTSS);
						break;
					case "dayliIf":
						imgSymbol = mView.FindViewById<ImageView>(Resource.Id.symDailyIF);
						break;
				}
				imgSymbol.SetBackgroundColor(sColor);

				if (series.lineAlpha.Equals(0))
				{
					cSeries.ChartType = ChartType.Scatter;
					cSeries.SymbolSize = new Java.Lang.Float(1.5f);
					cSeries.SymbolColor = new Java.Lang.Integer(sColor);
					cSeries.SetColor(new Java.Lang.Integer(sColor));
				}
				else
				{
					cSeries.BorderWidth = 0.5f;
					cSeries.SetColor(new Java.Lang.Integer(sRGBA.ToArgb()));
				}

				mPChart.Series.Add(cSeries);

				zoomSlider.SetRangeValues(0, pData.dataProvider.Count);
				zoomSlider.SetSelectedMaxValue(pData.dataProvider.Count);
				zoomSlider.SetSelectedMinValue(0);
			}
			#endregion

			#region annotation
			if (pData.TodayIndex() != -1)
			{
				ChartRectangleAnnotation today = new ChartRectangleAnnotation();
				today.Attachment = ChartAnnotationAttachment.DataIndex;
				today.PointIndex = pData.TodayIndex();
				today.Width = 3;
				today.Height = 10000;

				today.Color = Color.Black;
				today.BorderWidth = 0;
				today.FontSize = 10;
				today.TextColor = Color.White.ToArgb();
				today.TooltipText = "Future planned performance";
				mPChart.Annotations.Add(today);
			}

			annoFocused.Attachment = ChartAnnotationAttachment.DataIndex;
			annoFocused.PointIndex = pData.TodayIndex();
			annoFocused.Width = 1;
			annoFocused.Height = 10000;
			annoFocused.Color = Color.White;
			annoFocused.BorderWidth = 0;
			annoFocused.Visible = false;
			annoFocused.FontSize = 12;
			annoFocused.TextColor = Color.White.ToArgb();
			mPChart.Annotations.Add(annoFocused);
			#endregion

			mPChart.ItemsSource = pData.GetSalesDataList();

			#region custom tooltip
			mPChart.Tooltip.Content = new MyTooltip(mPChart, this, pData, annoFocused);
			#endregion
			mPChart.ZoomMode = ZoomMode.X;
			mPChart.AxisX.Scale = 1;
		}

		void InitGaugeData(Gauge gaugeData)
		{
			if (gaugeData == null) return;
			lblCycleDuration.Text = rootActivity.FormatNumber(gaugeData.Bike[0].value) + "%";
			lblRunDuration.Text = rootActivity.FormatNumber(gaugeData.Run[0].value) + "%";
			lblSwimDuration.Text = rootActivity.FormatNumber(gaugeData.Swim[0].value) + "%";

			lblCycleDistance.Text = rootActivity.FormatNumber(gaugeData.Bike[1].value) + "%";
			lblRunDistance.Text = rootActivity.FormatNumber(gaugeData.Bike[1].value) + "%";
			lblSwimDistance.Text = rootActivity.FormatNumber(gaugeData.Bike[1].value) + "%";

			lblCycleStress.Text = rootActivity.FormatNumber(gaugeData.Bike[2].value) + "%";
			lblRunStress.Text = rootActivity.FormatNumber(gaugeData.Bike[2].value) + "%";
			lblSwimStress.Text = rootActivity.FormatNumber(gaugeData.Bike[2].value) + "%";
		}

		void ActionViewCalendar(object sender, EventArgs e)
		{
			var intent = new Intent(Activity, typeof(EventCalendarActivity));
			StartActivityForResult(intent, 1);
		}

		#region Action Collepse

		void ActionToggleSeries(object sender, EventArgs e)
		{
			var sIndex = int.Parse(((LinearLayout)sender).Tag.ToString());
			var series = mPChart.Series.Get(sIndex) as ChartSeries;
			var sVisibility = series.SeriesVisibility == ChartSeriesVisibilityType.Hidden ? ChartSeriesVisibilityType.Visible : ChartSeriesVisibilityType.Hidden;

			series.SetVisibility(sVisibility);
		}

		void ActionCollepse(object sender, EventArgs e)
		{
			switch (int.Parse(((RelativeLayout)sender).Tag.ToString()))
			{
				case 0:
					btnCycle.SetImageResource(viewCycle.Visibility.Equals(ViewStates.Gone) ? Resource.Drawable.icon_down : Resource.Drawable.icon_right);
					CollepseAnimation(viewCycle);
					break;
				case 1:
					btnRun.SetImageResource(viewRun.Visibility.Equals(ViewStates.Gone) ? Resource.Drawable.icon_down : Resource.Drawable.icon_right);
					CollepseAnimation(viewRun);
					break;
				case 2:
					btnSwim.SetImageResource(viewSwim.Visibility.Equals(ViewStates.Gone) ? Resource.Drawable.icon_down : Resource.Drawable.icon_right);
					CollepseAnimation(viewSwim);
					break;
				default:
					break;
			}
		}

		void CollepseAnimation(LinearLayout content)
		{
			if (content.Visibility.Equals(ViewStates.Gone))
			{
				content.Visibility = ViewStates.Visible;

				int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
				int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
				content.Measure(widthSpec, heightSpec);

				ValueAnimator mAnimator = slideAnimator(0, content.MeasuredHeight, content);
				mAnimator.Start();
			}
			else
			{
				int finalHeight = content.Height;

				ValueAnimator mAnimator = slideAnimator(finalHeight, 0, content);
				mAnimator.Start();
				mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
				{
					content.Visibility = ViewStates.Gone;
				};
			}
		}

		private ValueAnimator slideAnimator(int start, int end, LinearLayout content)
		{
			ValueAnimator animator = ValueAnimator.OfInt(start, end);
			animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
			{
				var value = (int)animator.AnimatedValue;
				ViewGroup.LayoutParams layoutParams = content.LayoutParameters;
				layoutParams.Height = value;
				content.LayoutParameters = layoutParams;
			};
			return animator;
		}
		#endregion

		#region Handler
		void HanelerGraphZoomChanged(object sender, EventArgs e)
		{
			var rSlider = sender as RangeSliderControl;

			var gZoomLevel = (rSlider.GetSelectedMaxValue() - rSlider.GetSelectedMinValue()) / rSlider.GetAbsoluteMaxValue();
			mPChart.AxisX.Scale = gZoomLevel;
			var posX = new Java.Lang.Double(rSlider.GetSelectedMinValue());
			mPChart.AxisX.ScrollTo(posX, Position.Max);
		}
		#endregion
	}

#region custom tooltip
	public class MyTooltip : BaseChartTooltipView
	{
		FragmentCalendar mContext;
		ChartRectangleAnnotation mAnnoFocused;

		ReportGraphData mData;

		public MyTooltip(FlexChart chart, FragmentCalendar context, ReportGraphData data, ChartRectangleAnnotation annoFocused) : base(chart)
		{
			mContext = context;
			mAnnoFocused = annoFocused;
			mData = data;
		}
		public override void Render(SuperChartDataPoint point)
		{
			var data = mData.dataProvider[point.PointIndex];
			mAnnoFocused.PointIndex = point.PointIndex;
			mAnnoFocused.Text = String.Format("Date: {0}", data.date);
			mAnnoFocused.Visible = true;

			mContext.mView.FindViewById<TextView>(Resource.Id.txtTSB).Text = String.Format("TSB: {0}", data.tsb);
			mContext.mView.FindViewById<TextView>(Resource.Id.txtATL).Text = String.Format("ATL: {0}", data.atl);
			mContext.mView.FindViewById<TextView>(Resource.Id.txtCTL).Text = String.Format("CTL: {0}", data.ctl);
			mContext.mView.FindViewById<TextView>(Resource.Id.txtDailyTSS).Text = String.Format("Daily Load: {0}", data.dayliTss);
			mContext.mView.FindViewById<TextView>(Resource.Id.txtDailyIF).Text = String.Format("Daily IF: {0}", data.dayliIf);
		}
	}
#endregion
}
