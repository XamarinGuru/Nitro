using Foundation;
using System;
using UIKit;
using PortableLibrary;

namespace location2
{
    public partial class CalendarHomeViewController : BaseViewController
    {
        public CalendarHomeViewController (IntPtr handle) : base (handle)
        {
        }
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			calendarWebView.BackgroundColor = UIColor.Black;
			var url = string.Format(Constants.URL_GAUGE, AppSettings.UserID);
			calendarWebView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

			viewCycle.Alpha = 0;
			viewRunning.Alpha = 0;
			viewSwimming.Alpha = 0;
			heightCycle.Constant = 0;
			heightRunning.Constant = 0;
			heightSwimming.Constant = 0;

			InitGaugeData();
		}

		void InitGaugeData()
		{
			var gaugeData = GetGauge();

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
    }
}