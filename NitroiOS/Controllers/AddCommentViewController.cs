using Foundation;
using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;

namespace location2
{
    public partial class AddCommentViewController : BaseViewController
    {
		public NitroEvent selectedEvent;

        public AddCommentViewController() : base()
		{
		}
		public AddCommentViewController(IntPtr handle) : base(handle)
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

			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Loading data...");

				MemberModel.rootMember = GetUserObject();

				HideLoadingView();
			});
		}

		partial void ActionAddComment(UIButton sender)
		{
			if (txtComment.Text == "")
			{
				ShowMessageBox(null, "Type your comment...");
				return;
			}

			var author = MemberModel.firstname + " " + MemberModel.lastname;
			var authorID = AppSettings.UserID;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Saving your comment...");

				InvokeOnMainThread(() =>
				{
					HideLoadingView();
					var response = SetComment(author, authorID, txtComment.Text, selectedEvent._id);
					NavigationController.PopViewController(true);
				});
			});
		}

		partial void ActionClose(UIButton sender)
		{
			DismissModalViewController(true);
		}
    }
}