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

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Retreving Event Details...");

				MemberModel.rootMember = GetUserObject();

				HideLoadingView();
			});

			InitUISettings();
		}

		void InitUISettings()
		{
			//lblTime = FindViewById<TextView>(Resource.Id.lblTime);
			//lblDistance = FindViewById<TextView>(Resource.Id.lblDistance);
			//lblTSS = FindViewById<TextView>(Resource.Id.lblTSS);
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
					UpdateMemberNotes(txtComment.Text, AppSettings.UserID, selectedEvent._id, MemberModel.username, checkAttended.On ? "1" : "0", lblTime.Text, lblDistance.Text, lblTSS.Text, selectedEvent.type);
					this.DismissModalViewController(true);
				});
			});
		}
    }
}