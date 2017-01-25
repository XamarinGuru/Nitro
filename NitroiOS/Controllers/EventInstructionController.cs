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
			if ((DateTime.Parse(selectedEvent.start) - DateTime.Now).TotalMinutes > 1)
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
			var startDateFormats = DateTime.Parse(selectedEvent.start).GetDateTimeFormats();
			lblTitle.Text = selectedEvent.title;
			lblStartDate.Text = startDateFormats[11];
			lblData.Text = selectedEvent.eventData;

			var strDistance = selectedEvent.distance;
			float floatDistance = strDistance == "" ? 0 : float.Parse(strDistance);
			var b = Math.Truncate(floatDistance * 100);
			var c = b / 100;
			var formattedDistance = c.ToString("F2");

			lblPDistance.Text = formattedDistance + " KM";

			var durMin = selectedEvent.durMin == "" ? 0 : int.Parse(selectedEvent.durMin);
			var durHrs = selectedEvent.durHrs == "" ? 0 : int.Parse(selectedEvent.durHrs);
			var pHrs = durMin / 60;
			durHrs = durHrs + pHrs;
			durMin = durMin % 60;
			var strDuration = durHrs.ToString() + ":" + durMin.ToString("D2");

			lblPDuration.Text = strDuration;
			lblPLoad.Text = selectedEvent.tss;
			lblPHB.Text = selectedEvent.hb;

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
			}
		}

		void InitBindingEventTotal()
		{
			if (eventTotal == null || eventTotal.totals == null) return;

			lblAvgSpeed.Text = eventTotal.totals[0].value;
			lblTotalDistance.Text = eventTotal.totals[1].value;
			lblElapsedTime.Text = eventTotal.totals[2].value;
			lblTotalAcent.Text = eventTotal.totals[3].value;
			lblAvgHR.Text = eventTotal.totals[4].value;
			lblTotalCalories.Text = eventTotal.totals[5].value;
			lblAvgPower.Text = eventTotal.totals[6].value;
			lblLoad.Text = eventTotal.totals[7].value;
			lblLeveledPower.Text = eventTotal.totals[8].value;
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

			AppDelegate myDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
			myDelegate.baseVC.PresentModalViewController(atVC, true);
		}

		partial void ActionAddComment(UIButton sender)
		{
			AddCommentViewController acVC = Storyboard.InstantiateViewController("AddCommentViewController") as AddCommentViewController;
			acVC.selectedEvent = selectedEvent;

			AppDelegate myDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
			myDelegate.baseVC.PresentModalViewController(acVC, true);
		}
		#endregion
    }
}