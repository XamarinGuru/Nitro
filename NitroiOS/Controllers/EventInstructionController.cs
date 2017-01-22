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

			InitBindingEventData();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Retreving Event Details...");

				var eventTotal = GetEventTotals(selectedEvent._id);
				var eventComment = GetComments(selectedEvent._id);

				InvokeOnMainThread(() =>
				{
					InitBindingEventTotal(eventTotal);
					InitBindingEventComments(eventComment);
				});

				HideLoadingView();
			});
		}

		void InitBindingEventData()
		{
			var startDateFormats = DateTime.Parse(selectedEvent.start).GetDateTimeFormats();
			lblTitle.Text = selectedEvent.title;
			lblStartDate.Text = startDateFormats[11];
			lblData.Text = selectedEvent.eventData;

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

		void InitBindingEventTotal(EventTotal eventTotal)
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