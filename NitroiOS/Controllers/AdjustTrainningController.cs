using Foundation;
using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;

namespace location2
{
    public partial class AdjustTrainningController : BaseViewController
    {
		public NitroEvent selectedEvent;
		public EventTotal eventTotal;

        public AdjustTrainningController() : base()
		{
		}
		public AdjustTrainningController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = true;

			var leftButton = new UIButton(new CGRect(0, 0, 20, 20));
			leftButton.SetImage(UIImage.FromFile("icon_left.png"), UIControlState.Normal);
			leftButton.TouchUpInside += (sender, e) => this.DismissModalViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, KeyBoardUpNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Retreving Event Details...");

				MemberModel.rootMember = GetUserObject();

				HideLoadingView();
			});

			InitUISettings();
			InitBindingEventTotal();
		}

		void InitUISettings()
		{
			//lblTime = FindViewById<TextView>(Resource.Id.lblTime);
			//lblDistance = FindViewById<TextView>(Resource.Id.lblDistance);
			//lblTSS = FindViewById<TextView>(Resource.Id.lblTSS);
		}

		void InitBindingEventTotal()
		{
			if (selectedEvent == null || eventTotal.totals == null) return;

			var distance = GetFormatedDurationAsMin(eventTotal.totals[2].value);

			lblTime.Text = distance.ToString();
			lblDistance.Text = eventTotal.totals[1].value;
			lblTSS.Text = eventTotal.totals[7].value;

			seekTime.Value = distance;
			seekDistance.Value = int.Parse(eventTotal.totals[1].value);
			seekTSS.Value = int.Parse(eventTotal.totals[7].value);

			attended.On = selectedEvent.attended == "1" ? true : false;
		}

		partial void ActionClose(UIButton sender)
		{
			DismissModalViewController(true);
		}

		partial void ActionDataChanged(UISlider sender)
		{
			switch (sender.Tag)
			{
				case 0:
					lblTime.Text = ((int)sender.Value).ToString();
					break;
				case 1:
					lblDistance.Text = ((int)sender.Value).ToString();
					break;
				case 2:
					lblTSS.Text = ((int)sender.Value).ToString();
					break;
			}
		}

		partial void ActionAdjustTrainning(UIButton sender)
		{
			//if (txtComment.Text == "")
			//{
			//	ShowMessageBox(null, "Type your comment...");
			//	return;
			//}

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Adjusting Trainning...");

				InvokeOnMainThread(() =>
				{
					HideLoadingView();
					UpdateMemberNotes(txtComment.Text, AppSettings.UserID, selectedEvent._id, MemberModel.username, attended.On ? "1" : "0", lblTime.Text, lblDistance.Text, lblTSS.Text, selectedEvent.type);
					this.DismissModalViewController(true);
				});
			});
		}

		#region keyboard process
		private void KeyBoardUpNotification(NSNotification notification)
		{
			if (!txtComment.IsFirstResponder)
				return;

			CGRect r = UIKeyboard.BoundsFromNotification(notification);

			scroll_amount = (float)r.Height / 1.5f;

			if (scroll_amount > 0)
			{
				moveViewUp = true;
				ScrollTheView(moveViewUp);
			}
			else {
				moveViewUp = false;
			}
		}
		private void KeyBoardDownNotification(NSNotification notification)
		{
			if (moveViewUp) { ScrollTheView(false); }
		}
		private void ScrollTheView(bool move)
		{
			// scroll the view up or down
			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration(0.3);

			CGRect frame = this.View.Frame;

			if (move)
			{
				frame.Y = -(scroll_amount);
			}
			else {
				frame.Y = 0;
			}

			this.View.Frame = frame;
			UIView.CommitAnimations();
		}
		#endregion
    }
}