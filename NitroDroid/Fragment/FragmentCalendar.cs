using System;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Webkit;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
    public class FragmentCalendar : Android.Support.V4.App.Fragment
    {
		SwipeTabActivity rootActivity;

		View mView;

		TextView lblCycleDuration, lblRunDuration, lblSwimDuration, lblCycleDistance, lblRunDistance, lblSwimDistance, lblCycleStress, lblRunStress, lblSwimStress;
		ImageView btnCycle, btnRun, btnSwim;
		LinearLayout viewCycle, viewRun, viewSwim;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			rootActivity = this.Activity as SwipeTabActivity;
			return inflater.Inflate(Resource.Layout.fCalendar, container, false);
        }

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
			//child
			mView = view;

			SetUIVariablesAndActions();

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				rootActivity.ShowLoadingView("Loading data...");

				var gaugeData = rootActivity.GetGauge();

				rootActivity.HideLoadingView();

				rootActivity.RunOnUiThread(() =>
				{
					InitGaugeData(gaugeData);
				});
			});
			//InitGaugeData();
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

			var webView = mView.FindViewById<WebView>(Resource.Id.webView);

			webView.Settings.JavaScriptEnabled = true;
			webView.Settings.AllowContentAccess = true;
			webView.Settings.EnableSmoothTransition();
			webView.Settings.LoadsImagesAutomatically = true;
			webView.Settings.SetGeolocationEnabled(true);
			webView.SetWebViewClient(new WebViewClient());
			webView.SetBackgroundColor(Android.Graphics.Color.Transparent);

			webView.ClearCache(true);
			webView.ClearHistory();

			var url = string.Format(Constants.URL_GAUGE, AppSettings.UserID);
			webView.LoadUrl(url);

			#endregion

			#region Actions
			mView.FindViewById<RelativeLayout>(Resource.Id.collapsCycle).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapsRun).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapsSwim).Click += ActionCollepse;
			mView.FindViewById<Button>(Resource.Id.ActionViewCalendar).Click += ActionViewCalendar;

			#endregion
		}


		void InitGaugeData(Gauge gaugeData)
		{
			//var gaugeData = rootActivity.GetGauge();

			lblCycleDuration.Text = gaugeData.Bike[0].value + "%";
			lblRunDuration.Text = gaugeData.Run[0].value + "%";
			lblSwimDuration.Text = gaugeData.Swim[0].value + "%";

			lblCycleDistance.Text = gaugeData.Bike[1].value + "%";
			lblRunDistance.Text = gaugeData.Bike[1].value + "%";
			lblSwimDistance.Text = gaugeData.Bike[1].value + "%";

			lblCycleStress.Text = gaugeData.Bike[2].value + "%";
			lblRunStress.Text = gaugeData.Bike[2].value + "%";
			lblSwimStress.Text = gaugeData.Bike[2].value + "%";
		}

		void ActionViewCalendar(object sender, EventArgs e)
		{
			var intent = new Intent(Activity, typeof(EventCalendarActivity));
			StartActivity(intent);
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

				ValueAnimator mAnimator = slideAnimator(0, 200, content);
				mAnimator.Start();
			}
			else {
				ValueAnimator mAnimator = slideAnimator(200, 0, content);
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

		//public Animation OnCreateAnimation(int transit, bool enter, int nextAnim)
		//{
		//	return AnimationUtils.LoadAnimation(Activity,
		//		enter ? Android.Resource.Animation.FadeIn : Android.Resource.Animation.FadeOut);
		//}
    }
}
