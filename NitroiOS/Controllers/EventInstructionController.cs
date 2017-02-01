using Foundation;
using System;
using UIKit;
using PortableLibrary;
using CoreGraphics;

namespace location2
{
    public partial class EventInstructionController : BaseViewController
    {
		public NitroEvent selectedEvent;
		private EventTotal eventTotal;
		string eventID;
        public EventInstructionController() : base()
		{
		}
		public EventInstructionController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = true;

			var leftButton = new UIButton(new CGRect(0, 0, 20, 20));
			leftButton.SetImage(UIImage.FromFile("icon_left.png"), UIControlState.Normal);
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

			eventID = selectedEvent._id;

			InitUISettings();
			InitBindingEventData();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Retreving Event Details...");

				selectedEvent = GetEventDetail(selectedEvent._id);
				selectedEvent._id = eventID;
				eventTotal = GetEventTotals(selectedEvent._id);
				var eventComment = GetComments(selectedEvent._id);

				InvokeOnMainThread(() =>
				{
					InitBindingEventData();
					InitBindingEventTotal();
					InitBindingEventComments(eventComment);
				});

				HideLoadingView();
			});
		}

		void InitUISettings()
		{
			if ((selectedEvent.StartDateTime() - DateTime.Now).TotalMinutes > 1)
			{
				heightInstructions.Constant = 0;
				heightAdjust.Constant = 0;
			}
			else
			{
				heightInstructions.Constant = 200;
				heightAdjust.Constant = 100;
			}
		}

		void InitBindingEventData()
		{
			var startDateFormats = String.Format("{0:f}", selectedEvent.StartDateTime());
			lblTitle.Text = selectedEvent.title;
			lblStartDate.Text = startDateFormats;
			lblData.Text = selectedEvent.eventData;

			var strDistance = selectedEvent.distance;
			float floatDistance = strDistance == "" ? 0 : float.Parse(strDistance);
			var b = Math.Truncate(floatDistance * 100);
			var c = b / 100;
			var formattedDistance = c.ToString("F2");

			lblPDistance.Text = FormatNumber(formattedDistance) + " KM";

			var durMin = selectedEvent.durMin == "" ? 0 : int.Parse(selectedEvent.durMin);
			var durHrs = selectedEvent.durHrs == "" ? 0 : int.Parse(selectedEvent.durHrs);
			var pHrs = durMin / 60;
			durHrs = durHrs + pHrs;
			durMin = durMin % 60;
			var strDuration = durHrs.ToString() + ":" + durMin.ToString("D2");

			lblPDuration.Text = FormatNumber(strDuration);
			lblPLoad.Text = FormatNumber(selectedEvent.tss);
			lblPHB.Text = FormatNumber(selectedEvent.hb);

			switch (selectedEvent.type)
			{
				case "0":
					imgType.Image = UIImage.FromFile("icon_triathlon.png");
					break;
				case "1":
					imgType.Image = UIImage.FromFile("icon_bike.png");
					break;
				case "2":
					imgType.Image = UIImage.FromFile("icon_run.png");
					break;
				case "3":
					imgType.Image = UIImage.FromFile("icon_swim.png");
					break;
				case "4":
					imgType.Image = UIImage.FromFile("icon_triathlon.png");
					break;
				case "5":
					imgType.Image = UIImage.FromFile("icon_other.png");
					break;
			}
		}

		void InitBindingEventTotal()
		{
			if (eventTotal == null || eventTotal.totals == null) return;

			lblTotalName0.Text = eventTotal.totals[0].name;
			lblTotalName1.Text = eventTotal.totals[1].name;
			lblTotalName2.Text = eventTotal.totals[2].name;
			lblTotalName3.Text = eventTotal.totals[3].name;
			lblTotalName4.Text = eventTotal.totals[4].name;
			lblTotalName5.Text = eventTotal.totals[5].name;
			lblTotalName6.Text = eventTotal.totals[6].name;
			lblTotalName7.Text = eventTotal.totals[7].name;
			lblTotalName8.Text = eventTotal.totals[8].name;

			lblTotalValue0.Text = FormatNumber(eventTotal.totals[0].value);
			lblTotalValue1.Text = FormatNumber(eventTotal.totals[1].value);
			lblTotalValue2.Text = FormatNumber(eventTotal.totals[2].value);
			lblTotalValue3.Text = FormatNumber(eventTotal.totals[3].value);
			lblTotalValue4.Text = FormatNumber(eventTotal.totals[4].value);
			lblTotalValue5.Text = FormatNumber(eventTotal.totals[5].value);
			lblTotalValue6.Text = FormatNumber(eventTotal.totals[6].value);
			lblTotalValue7.Text = FormatNumber(eventTotal.totals[7].value);
			lblTotalValue8.Text = FormatNumber(eventTotal.totals[8].value);
		}

		void InitBindingEventComments(Comment comments)
		{
			foreach (var subView in contentComment.Subviews)
				subView.RemoveFromSuperview();

			lblCommentTitle.Text = "COMMENT" + " (" + comments.comments.Count + ")";

			nfloat posY = 0;
			foreach (var comment in comments.comments)
			{
				CommentView cv = CommentView.Create();
				var height = cv.SetView(comment);
				cv.Frame = new CGRect(0, posY, contentComment.Frame.Size.Width, height);
				contentComment.AddSubview(cv);

				posY += height;
			}

			heightCommentContent.Constant = posY;
		}

		#region Actions
		partial void ActionAdjustTrainning(UIButton sender)
		{
			AdjustTrainningController atVC = Storyboard.InstantiateViewController("AdjustTrainningController") as AdjustTrainningController;
			atVC.selectedEvent = selectedEvent;
			atVC.eventTotal = eventTotal;

			NavigationController.PushViewController(atVC, true);

			//AppDelegate myDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
			//myDelegate.baseVC.PresentModalViewController(atVC, true);
		}

		partial void ActionAddComment(UIButton sender)
		{
			AddCommentViewController acVC = Storyboard.InstantiateViewController("AddCommentViewController") as AddCommentViewController;
			acVC.selectedEvent = selectedEvent;

			NavigationController.PushViewController(acVC, true);

			//AppDelegate myDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
			//myDelegate.baseVC.PresentModalViewController(acVC, true);
		}
		#endregion
    }
}