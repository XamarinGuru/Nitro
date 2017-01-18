using Foundation;
using System;
using UIKit;
using CoreGraphics;
using Softweb.Xamarin.Controls.iOS;
using PortableLibrary;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace location2
{
	public partial class EventCalendarViewController : BaseViewController
    {
		private readonly Calendar _calendar;
		private List<NitroEvent> _events;

        public EventCalendarViewController (IntPtr handle) : base (handle)
        {
			_calendar = new Calendar();
			_events = new List<NitroEvent>();
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = true;

			var leftButton = new UIButton(new CGRect(0, 0, 20, 20));
			leftButton.SetImage(UIImage.FromFile("icon_left.png"), UIControlState.Normal);
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

			SetCalendarView();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			_calendar.ReloadData();
		}

		void SetCalendarView()
		{
			this.View.LayoutIfNeeded();

			var menuView = new CalendarMenuView { Frame = new CGRect(0, 0, viewDate.Frame.Size.Width, viewDate.Frame.Size.Height) };
			var contentView = new CalendarContentView { Frame = new CGRect(0, 0, viewCalendar.Frame.Size.Width, viewCalendar.Frame.Size.Height) };

			var appearance = _calendar.CalendarAppearance;
			appearance.GetNSCalendar().FirstWeekDay = (nuint)2;
			appearance.DayDotColor = appearance.DayCircleColorSelected = UIColor.FromRGB(229 / 225f, 161 / 225f, 9 / 225f);
			appearance.DayTextColorOtherMonth = appearance.DayDotColorOtherMonth = UIColor.Gray;
			appearance.DayTextColor = appearance.MenuMonthTextColor = UIColor.White;
			appearance.DayCircleColorToday = UIColor.Red;
			appearance.DayCircleRatio = (9f / 10f);
			appearance.WeekDayFormat = CalendarWeekDayFormat.Single;

			appearance.SetMonthLabelTextCallback((NSDate date, Calendar cal) => new NSString(((DateTime)date).ToString("MMMM yyyy")));

			//Link the views to the calendar
			_calendar.MenuMonthsView = menuView;
			_calendar.ContentView = contentView;

			_calendar.DateSelected += DateSelected;
			_calendar.NextPageLoaded += DidLoadNextPage;
			_calendar.PreviousPageLoaded += DidLoadPreviousPage;

			//Add calendar views to the main view
			viewDate.Add(menuView);
			viewCalendar.Add(contentView);

			//System.Threading.ThreadPool.QueueUserWorkItem(delegate
			//{
				ShowLoadingView("Retreving Nitro Events...");

				var pastEvents = GetPastEvents();
				var todayEvents = GetTodayEvents();
				var futureEvents = GetFutureEvents();

				_events.AddRange(pastEvents);
				_events.AddRange(todayEvents);
				_events.AddRange(futureEvents);

				AddEventsToCustomCalendar();
				FilterEventsByDate(DateTime.Now);

				HideLoadingView();
			//});
		}

		void AddEventsToCustomCalendar()
		{
			EventDetails[] eventDetails = new EventDetails[_events.Count];
			for (var i = 0; i < _events.Count; i ++)
			{
				var nitroEvent = _events[i];
				var startDate = DateTime.Parse(nitroEvent.start);
				var endDate = DateTime.Parse(nitroEvent.end);
				var eventDetail = new EventDetails((NSDate)startDate, (NSDate)endDate, nitroEvent.title);
				eventDetails[i] = eventDetail;
			}

			_calendar.EventSchedule = eventDetails;
		}

		void FilterEventsByDate(DateTime filterDate)
		{
			var eventsByDate = new List<NitroEvent>();
			foreach (var nitroEvent in _events)
			{
				var eventDate = DateTime.Parse(nitroEvent.start);
				if ((int)(filterDate - eventDate).TotalDays == 0)
				{
					eventsByDate.Add(nitroEvent);
				}
			}
			var tblDataSource = new NitroEventTableViewSource(eventsByDate, this);
			this.InvokeOnMainThread(delegate
			{
				tableView.Source = tblDataSource;
				tableView.ReloadData();
			});
		}
		public void DateSelected(object sender, DateSelectedEventArgs args)
		{
			Console.WriteLine(String.Format("Selected date is {0}", ((DateTime)args.Date).ToLocalTime().ToString("dd-MMM-yyyy")));
			FilterEventsByDate((DateTime)args.Date);
		}

		public void DidLoadPreviousPage(object sender, EventArgs args)
		{
			Console.WriteLine("loaded previous page");
		}

		public void DidLoadNextPage(object sender, EventArgs args)
		{
			Console.WriteLine("loaded next page");
		}

		#region NitroEventTableViewSource

		class NitroEventTableViewSource : UITableViewSource
		{
			private List<NitroEvent> nitroEvents;
			private EventCalendarViewController eventCalendarVC;

			public NitroEventTableViewSource(List<NitroEvent> events, EventCalendarViewController eventCalendarVC)
			{
				this.nitroEvents = new List<NitroEvent>();

				if (events == null) return;

				this.nitroEvents = events;
				this.eventCalendarVC = eventCalendarVC;
			}

			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return nitroEvents.Count;
			}

			public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
			{
				return 60;
			}
			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				NitroEventCell cell = tableView.DequeueReusableCell("NitroEventCell") as NitroEventCell;
				cell.SetCell(nitroEvents[indexPath.Section]);

				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				var selectedEvent = nitroEvents[indexPath.Row];
				eventCalendarVC.NavigationController.PushViewController(null, true);
			}
		}
		#endregion
    }
}