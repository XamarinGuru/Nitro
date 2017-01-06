using Foundation;
using System;
using UIKit;
using EventKit;
using GalaSoft.MvvmLight.Helpers;
using PortableLibrary;

namespace location2
{
	partial class ProfileViewController : BaseViewController
	{
		internal static UIImage temMeImg;

		UIImagePickerController imagePicker = new UIImagePickerController();

		private RootMemberModel MemberModel { get; set; }
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

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			MemberModel.rootMember = GetUserObject();

			SetInputBinding();
		}

		private void SetInputBinding()
		{
			#region physical
			this.SetBinding(() => MemberModel.username, () => lblUserName.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.firstname, () => txtFirstName.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.lastname, () => txtLastName.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.password, () => passTB.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.email, () => txtEmail.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.phone, () => txtPhone.Text, BindingMode.TwoWay);
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

		partial void ActionUpdate(UIButton sender)
		{
			var result = UpdateUserDataJson(MemberModel.rootMember);
			ShowMessageBox(null, "updated successfully");
			//trackSvc.Service1 sv = new trackSvc.Service1();
			//try
			//{
			//	Byte[] myByteArray;

			//	if (temMeImg == null)
			//	{
			//		ShowMessageBox(null, "Please choose profile image!");
			//		return;
			//	}
			//	using (NSData imageData = MaxResizeImage(temMeImg, 90f, 90f).AsPNG())
			//	{
			//		myByteArray = new Byte[imageData.Length];
			//		System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, myByteArray, 0, Convert.ToInt32(imageData.Length));
			//	}

			//	var athId = NSUserDefaults.StandardUserDefaults.StringForKey("id").ToString();
			//	sv.updateAthPersonalData(athId, txtFirstName.Text, txtLastName.Text, txtPhone.Text, txtEmail.Text, myByteArray);
			//	ShowMessageBox(null, "Data Saved! We hope you got serious...");
			//}
			//catch (Exception err)
			//{
			//	bool out1 = false;
			//	bool out2 = false;

			//	sv.getErrprFromMobile(err.ToString(), NSUserDefaults.StandardUserDefaults.StringForKey("id").ToString(), out out1, out out2);
			//	ShowMessageBox(null, "Error saving data!");
			//}
		}

		partial void ActionSerious(UIButton sender)
		{
			rootVC.SetCurrentPage(3);
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
