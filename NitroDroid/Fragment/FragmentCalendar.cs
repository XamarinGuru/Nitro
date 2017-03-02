using System;
using Android.Animation;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;
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
		SwipeTabActivity rootActivity;

		View mView;

		FlexChart mPChart;

		TextView lblCycleDuration, lblRunDuration, lblSwimDuration, lblCycleDistance, lblRunDistance, lblSwimDistance, lblCycleStress, lblRunStress, lblSwimStress;
		ImageView btnCycle, btnRun, btnSwim;
		LinearLayout viewCycle, viewRun, viewSwim;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LicenseManager.Key = License.Key;

			rootActivity = this.Activity as SwipeTabActivity;
			return inflater.Inflate(Resource.Layout.fCalendar, container, false);
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
			mView = view;

			SetUIVariablesAndActions();

			if (!rootActivity.IsNetEnable()) return;

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
			mView.FindViewById<RelativeLayout>(Resource.Id.collapsCycle).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapsRun).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapsSwim).Click += ActionCollepse;
			mView.FindViewById<Button>(Resource.Id.ActionViewCalendar).Click += ActionViewCalendar;

			#endregion
		}

		void InitPerformanceGraph(ReportGraphData pData)
		{
			if (pData == null) return;

			mPChart = mView.FindViewById<FlexChart>(Resource.Id.pChart);

			#region configure
			//mPChart.Header = "PERFORMANCE";
			mPChart.SetPalette(Palettes.Modern);
			mPChart.ChartType = ChartType.Splinearea;
			mPChart.SetBackgroundColor(Color.Black);
			mPChart.BindingX = pData.categoryField;// bind X axis to display category names
			mPChart.Animated = false;
			#endregion

			#region regend
			//mPChart.ToggleLegend = true;
			mPChart.Legend.LegendFontSize = 15;
			mPChart.Legend.BackgroundColor = Color.Transparent.ToArgb();
			mPChart.Legend.Position = ChartPositionType.Bottom;
			#endregion

			#region axis
			mPChart.AxisX.LabelsVisible = false;
			mPChart.AxisX.LineWidth = 3;

			mPChart.AxisY.LabelsVisible = false;
			mPChart.AxisY.LineColor = Color.Orange.ToArgb();
			mPChart.AxisY.LineWidth = 3;

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
				cSeries.SetColor(new Java.Lang.Integer(Color.ParseColor(series.lineColor).ToArgb()));

				if (series.lineAlpha.Equals(0))
				{
					cSeries.ChartType = ChartType.Scatter;
					cSeries.SymbolSize = new Java.Lang.Float(2);
				}
				else
				{
					cSeries.BorderWidth = 1;
				}

				mPChart.Series.Add(cSeries);
			}
			#endregion

			#region annotation
			if (pData.TodayIndex() != -1)
			{
				ChartRectangleAnnotation today = new ChartRectangleAnnotation();
				today.Attachment = ChartAnnotationAttachment.DataIndex;
				today.PointIndex = pData.TodayIndex();
				today.Width = 1;
				today.Height = 10000;
				today.Color = Color.White;
				today.BorderWidth = 0;
				today.FontSize = 10;
				today.Text = "Today";
				today.TextColor = Color.Gray.ToArgb();
				today.TooltipText = "Planned performance after today";
				mPChart.Annotations.Add(today);
			}
			#endregion

			mPChart.ItemsSource = pData.GetSalesDataList();

			#region custom tooltip
			mPChart.Tooltip.Content = new MyTooltip(mPChart, mPChart.Context, pData);
			#endregion
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
	}

#region custom tooltip
	public class MyTooltip : BaseChartTooltipView
	{
		FlexChart mChart;
		TextView txtDate, txtTSB, txtATL, txtCTL, txtDLoad, txtDIf;

		ReportGraphData mData;

		public MyTooltip(FlexChart chart, Context context, ReportGraphData data) : base(chart)
		{
			mChart = chart;
			mData = data;
			txtDate = new TextView(context);
			txtTSB = new TextView(context);
			txtATL = new TextView(context);
			txtCTL = new TextView(context);
			txtDLoad = new TextView(context);
			txtDIf = new TextView(context);

			txtTSB.TextSize = 10;
			txtATL.TextSize = 10;
			txtCTL.TextSize = 10;
			txtDLoad.TextSize = 10;
			txtDIf.TextSize = 10;

			foreach (var series in mData.graphs)
			{
				switch (series.valueField)
				{
					case "tsb":
						txtTSB.SetTextColor(Color.ParseColor(series.lineColor));
						break;
					case "atl":
						txtATL.SetTextColor(Color.ParseColor(series.lineColor));
						break;
					case "ctl":
						txtCTL.SetTextColor(Color.ParseColor(series.lineColor));
						break;
					case "dayliTss":
						txtDLoad.SetTextColor(Color.ParseColor(series.lineColor));
						break;
					case "dayliIf":
						txtDIf.SetTextColor(Color.ParseColor(series.lineColor));
						break;
				}
			}

			LinearLayout layout = new LinearLayout(context);
			layout.Orientation = Orientation.Vertical;
			layout.SetBackgroundColor(Color.Gray);
			layout.SetPadding(5, 5, 5, 5);
			layout.AddView(txtDate);
			layout.AddView(txtTSB);
			layout.AddView(txtATL);
			layout.AddView(txtCTL);
			layout.AddView(txtDLoad);
			layout.AddView(txtDIf);

			AddView(layout);
		}
		public override void Render(SuperChartDataPoint point)
		{
			var data = mData.dataProvider[point.PointIndex];
			txtDate.SetText(String.Format("Date: {0}", data.date), TextView.BufferType.Normal);
			txtTSB.SetText(String.Format("TSB: {0}", data.tsb), TextView.BufferType.Normal);
			txtATL.SetText(String.Format("ATL: {0}", data.atl), TextView.BufferType.Normal);
			txtCTL.SetText(String.Format("CTL: {0}", data.ctl), TextView.BufferType.Normal);
			txtDLoad.SetText(String.Format("Day LOAD: {0}", data.dayliTss), TextView.BufferType.Normal);
			txtDIf.SetText(String.Format("Day IF: {0}", data.dayliIf), TextView.BufferType.Normal);
			RequestLayout();
		}
	}
#endregion
}
