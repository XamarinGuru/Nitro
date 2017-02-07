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

			if (!IsNetEnable())
			{
				return;
			}

			ShowLoadingView("Getting User Data...");

			await Task.Run(() =>
			{
				MemberModel.rootMember = GetUserObject();

				InvokeOnMainThread(() => { SetInputBinding(); });

				HideLoadingView();
			});
		}

		private void SetInputBinding()
		{
			#region physical
			this.SetBinding(() => MemberModel.username, () => lblUserName.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.email, () => lblEmail.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.phone, () => lblPhone.Text, BindingMode.TwoWay);
			#endregion

			if (GetPictureFromLocal() != null)
			{
				imgPicture.Image = GetPictureFromLocal();
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
			if (!IsNetEnable())
			{
				ShowMessageBox(null, "No internet connection!");
				return;
			}

			AppDelegate myDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
			myDelegate.baseVC.PresentViewController(imagePicker, true, null);
		}

		partial void ActionChangePassword(UIButton sender)
		{
			if (!IsNetEnable())
			{
				ShowMessageBox(null, "No internet connection!");
				return;
			}

			ChangePasswordViewController changePWVC = Storyboard.InstantiateViewController("ChangePasswordViewController") as ChangePasswordViewController;
			changePWVC.email = MemberModel.email;
			NavigationController.PushViewController(changePWVC, true);
		}

		partial void ActionEditProfile(UIButton sender)
		{
			if (!IsNetEnable())
			{
				ShowMessageBox(null, "No internet connection!");
				return;
			}

			SeriousViewController eventVC = Storyboard.InstantiateViewController("SeriousViewController") as SeriousViewController;
			NavigationController.PushViewController(eventVC, true);
		}

		partial void ActionSyncDevice(UIButton sender)
		{
			if (!IsNetEnable())
			{
				ShowMessageBox(null, "No internet connection!");
				return;
			}

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
			UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
			if (originalImage != null)
			{
				imgPicture.Image = originalImage;
				SaveUserImage(imgPicture.Image);
			}
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
