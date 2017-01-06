using Foundation;
using System;
using UIKit;
using EventKit;
using PortableLibrary;

namespace location2
{
	partial class CalendarViewController : BaseViewController
	{
		public CalendarViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var url = string.Format(Constants.CALENDAR_URL, AppSettings.Username, AppSettings.UserID);
			calendarWebView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

			//request for accessing device calendar
			App.Current.EventStore.RequestAccess(EKEntityType.Event, (bool granted, NSError e) =>
			{
				InvokeOnMainThread(() =>
				{
					if (!granted)
						ShowMessageBox("Access Denied", "User Denied Access to Calendars/Reminders");
				});
			});
		}
	}
}
