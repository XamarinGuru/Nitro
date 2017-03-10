using Foundation;
using System;
using UIKit;
using CoreGraphics;
using Softweb.Xamarin.Controls.iOS;
using PortableLibrary;
using System.Collections.Generic;

namespace location2
{
	public partial class EventCalendarViewController : BaseViewController
    {
		UIColor COLOR_PAST = new UIColor(229 / 255f, 161 / 255f, 9 / 255f, 1.0f);
		UIColor COLOR_FUTURE = new UIColor(63 / 255f, 187 / 255f, 190 / 255f, 1.0f);

		Calendar _calendar;
		List<NitroEvent> _events;

        public EventCalendarViewController (IntPtr handle) : base (handle)
        {
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

			var rightButton = new UIButton(new CGRect(0, 0, 70, 20));
			rightButton.SetTitle("Reload", UIControlState.Normal);
			rightButton.TouchUpInside += (sender, e) => ResetCalendarView();

			var rightButton1 = new UIButton(new CGRect(100, 0, 70, 20));
			rightButton1.SetTitle("Today", UIControlState.Normal);
			rightButton1.TouchUpInside += (sender, e) => _calendar.CurrentDate = (NSDate)DateTime.Now;

			UIBarButtonItem[] rightButtons = { new UIBarButtonItem(rightButton), new UIBarButtonItem(rightButton1) };

			NavigationItem.RightBarButtonItems = rightButtons;

			if (!IsNetEnable()) return;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			ResetCalendarView();
		}

		void ResetCalendarView()
		{
			this.View.LayoutIfNeeded();

			_calendar = new Calendar();
			_calendar.CurrentDate = (NSDate)DateTime.Now;

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

			foreach (var view in viewDate.Subviews)
				view.RemoveFromSuperview();

			foreach (var view in viewCalendar.Subviews)
				view.RemoveFromSuperview();
			
			viewDate.Add(menuView);
			viewCalendar.Add(contentView);

			ReloadEvents();
		}

		void ReloadEvents()
		{
			if (!IsNetEnable()) return;

			_events = new List<NitroEvent>();
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_EVENTS);

				var pastEvents = GetPastEvents();
				var todayEvents = GetTodayEvents();
				var futureEvents = GetFutureEvents();

				_events.AddRange(pastEvents);
				_events.AddRange(todayEvents);
				_events.AddRange(futureEvents);

				InvokeOnMainThread(() =>
				{
					AddEventsToCustomCalendar();
					FilterEventsByDate(DateTime.Now);
					_calendar.ReloadData();

					HideLoadingView();
				});
			});
		}

		void AddEventsToCustomCalendar()
		{
			EventDetails[] eventDetails = new EventDetails[_events.Count];
			for (var i = 0; i < _events.Count; i ++)
			{
				var nitroEvent = _events[i];

				var startDate = nitroEvent.StartDateTime();
				var endDate = nitroEvent.EndDateTime();

				var eventDetail = new EventDetails((NSDate)(ConvertUTCToLocalTimeZone(startDate)), (NSDate)(ConvertUTCToLocalTimeZone(endDate)), nitroEvent.title);
				eventDetails[i] = eventDetail;
			}

			_calendar.EventSchedule = eventDetails;

		}

		void FilterEventsByDate(DateTime filterDate)
		{
			var eventsByDate = new List<NitroEvent>();
			foreach (var nitroEvent in _events)
			{
				var eventDate = nitroEvent.StartDateTime();
				if (filterDate.DayOfYear == eventDate.DayOfYear)
				{
					eventsByDate.Add(nitroEvent);
				}
			}
			if (DateTime.Compare(filterDate, DateTime.Now) > 0)
				SetPerformanceDataColor(true);
			else
				SetPerformanceDataColor(false);

			if (eventsByDate.Count == 0)
				lblNoEvents.Hidden = false;
			else
				lblNoEvents.Hidden = true;
			
			var tblDataSource = new NitroEventTableViewSource(eventsByDate, this);
			this.InvokeOnMainThread(delegate
			{
				tableView.Source = tblDataSource;
				tableView.ReloadData();

				SetTableViewHeightBasedOnChildren(tableView, eventsByDate, heightEventDetail);
				InitPerformanceData(filterDate);
			});
		}
		void InitPerformanceData(DateTime date)
		{
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_EVENTS);

				var performanceData = GetPerformanceForDate(date);

				HideLoadingView();


				InvokeOnMainThread(() =>
				{
					if (performanceData == null)
					{
						lblTSB.Text = "-";
						lblCTL.Text = "-";
						lblATL.Text = "-";
						lblLoad.Text = "-";
					}
					else
					{
						lblTSB.Text = performanceData.TSB == "NaN" ? "0" : performanceData.TSB;
						lblCTL.Text = performanceData.CTL == "NaN" ? "0" : performanceData.CTL;
						lblATL.Text = performanceData.ATL == "NaN" ? "0" : performanceData.ATL;
						lblLoad.Text = performanceData.LOAD == "NaN" ? "0" : performanceData.LOAD;
					}
				});
			});
		}

		void SetPerformanceDataColor(bool isFuture)
		{
			UIColor color = isFuture ? COLOR_FUTURE : COLOR_PAST;
			lblTSB.TextColor = color;
			lblCTL.TextColor = color;
			lblATL.TextColor = color;
			lblLoad.TextColor = color;
		}
		public void DateSelected(object sender, DateSelectedEventArgs args)
		{
			Console.WriteLine(String.Format("Selected date is {0}", ((DateTime)args.Date).ToLocalTime().ToString("dd-MMM-yyyy")));
			FilterEventsByDate(((DateTime)args.Date).ToLocalTime());
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
			List<NitroEvent> nitroEvents;
			EventCalendarViewController eventCalendarVC;

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
				cell.SetCell(nitroEvents[indexPath.Row]);

				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				if (!eventCalendarVC.IsNetEnable()) return;

				var selectedEvent = nitroEvents[indexPath.Row];
				UIStoryboard sb = UIStoryboard.FromName("Main", null);
				EventInstructionController eventInstructionVC = sb.InstantiateViewController("EventInstructionController") as EventInstructionController;
				eventInstructionVC.selectedEvent = selectedEvent;
				eventCalendarVC.NavigationController.PushViewController(eventInstructionVC, true);
			}
		}
		#endregion
    }
}