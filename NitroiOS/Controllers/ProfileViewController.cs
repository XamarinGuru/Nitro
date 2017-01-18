using Foundation;
using System;
using UIKit;
using EventKit;
using GalaSoft.MvvmLight.Helpers;
using PortableLibrary;
using System.Threading.Tasks;

namespace location2
{
	partial class ProfileViewController : BaseViewController
	{
		internal static UIImage temMeImg;

		UIImagePickerController imagePicker = new UIImagePickerController();

		//private RootMemberModel MemberModel { get; set; }
		public ProfileViewController(IntPtr handle) : base(handle)
		{
			MemberModel = new RootMemberModel();
		}

		public override void ViewDidLoad()
		{
			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

			imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
			imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
			imagePicker.Canceled += Handle_Canceled;
		}

		async public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//ShowLoadingView("Getting User Data...");

			//await Task.Run(() =>
			//{
			//	MemberModel.rootMember = GetUserObject();
			//	HideLoadingView();
			//});

			//SetInputBinding();
		}

		private void SetInputBinding()
		{
			#region physical
			this.SetBinding(() => MemberModel.username, () => lblUserName.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.email, () => lblEmail.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.phone, () => lblPhone.Text, BindingMode.TwoWay);
			#endregion

			if (getPictureFromLocal() != null)
			{
				imgPicture.Image = getPictureFromLocal();
				temMeImg = getPictureFromLocal();
			}
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();
			imgPicture.Layer.CornerRadius = imgPicture.Frame.Width / 2;
			imgPicture.Layer.MasksToBounds = true;
		}


		#region event handler
		partial void ActionChangePhoto(UIButton sender)
		{
			this.PresentViewController(imagePicker, true, null);
		}

		partial void ActionEditProfile(UIButton sender)
		{
			rootVC.SetCurrentPage(3);
		}

		partial void ActionSyncDevice(UIButton sender)
		{
			var userID = GetUserID();
			UIApplication.SharedApplication.OpenUrl(new NSUrl(string.Format(Constants.URL_WATCH, userID)));
		}

		partial void ActionSignOut(UIButton sender)
		{
			SignOutUser();

			LoginViewController loginVC = Storyboard.InstantiateViewController("LoginViewController") as LoginViewController;
			this.PresentViewController(loginVC, false, null);
		}

		#endregion

		#region photo library

		protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
		{
			NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
			if (referenceURL != null)
				Console.WriteLine("Url:" + referenceURL.ToString());

			UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
			if (originalImage != null)
			{
				imgPicture.Image = originalImage;
				temMeImg = originalImage;
				//save to local
				saveImageToLocal(originalImage);
			}
			// dismiss the picker
			imagePicker.DismissViewControllerAsync(true);
		}
		void Handle_Canceled(object sender, EventArgs e)
		{
			imagePicker.DismissViewControllerAsync(true);
		}
		#endregion

		#region remove all events from device nitro calendar
		partial void removeNitroEvents(UIButton sender)
		{
			var calendars = App.Current.EventStore.GetCalendars(EKEntityType.Event);
			foreach (var calendar in calendars)
			{
				if (calendar.Title == Constants.CALENDAR_TITLE)
				{
					NSError pE;
					App.Current.EventStore.RemoveCalendar(calendar, true, out pE);
				}
			}
		}
		#endregion
	}
}
