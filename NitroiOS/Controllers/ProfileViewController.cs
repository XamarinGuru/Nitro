using Foundation;
using System;
using UIKit;
using EventKit;

namespace location2
{
	partial class ProfileViewController : BaseViewController
	{
		internal static UIImage temMeImg;

		UIImagePickerController imagePicker = new UIImagePickerController();

		public ProfileViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

			imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
			imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
			imagePicker.Canceled += Handle_Canceled;

			if (!setPersonalData())
			{
				ShowMessageBox(null, "Error getting user data!");
			}

			calendarContent.Hidden = true;
			calendarWebView.Hidden = true;
			btnDone.Hidden = true;
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();
			imgPicture.Layer.CornerRadius = imgPicture.Frame.Width / 2;
			imgPicture.Layer.MasksToBounds = true;
		}

		private bool setPersonalData()
		{
			try
			{
				trackSvc.Service1 sv = new trackSvc.Service1();
				string _dId = NSUserDefaults.StandardUserDefaults.StringForKey("deviceId");
				string[] athData = sv.getAthDataByDeviceId(_dId);
				txtFirstName.Text = athData[0].ToString();
				txtLastName.Text = athData[1].ToString();
				lblUserName.Text = athData[4].ToString();
				passTB.Text = athData[5].ToString();
				txtEmail.Text = athData[6].ToString();
				txtPhone.Text = athData[7].ToString();

				if (getPictureFromLocal() != null)
				{
					imgPicture.Image = getPictureFromLocal();
					temMeImg = getPictureFromLocal();
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		#region event handler
		partial void BtnGo_TouchUpInside(UIButton sender)
		{
			trackSvc.Service1 sv = new trackSvc.Service1();
			try
			{
				Byte[] myByteArray;

				if (temMeImg == null)
				{
					ShowMessageBox(null, "Please choose profile image!");
					return;
				}
				using (NSData imageData = MaxResizeImage(temMeImg, 90f, 90f).AsPNG())
				{
					myByteArray = new Byte[imageData.Length];
					System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, myByteArray, 0, Convert.ToInt32(imageData.Length));
				}

				var athId = NSUserDefaults.StandardUserDefaults.StringForKey("id").ToString();
				sv.updateAthPersonalData(athId, txtFirstName.Text, txtLastName.Text, txtPhone.Text, txtEmail.Text, myByteArray);
				ShowMessageBox(null, "Data Saved! We hope you got serious...");
			}
			catch (Exception err)
			{
				bool out1 = false;
				bool out2 = false;

				sv.getErrprFromMobile(err.ToString(), NSUserDefaults.StandardUserDefaults.StringForKey("id").ToString(), out out1, out out2);
				ShowMessageBox(null, "Error saving data!");
			}
		}
		partial void SeriuosBtn_TouchUpInside(UIButton sender)
		{
			//calendarWebView.Hidden = false;
			//calendarContent.Hidden = false;
			//btnDone.Hidden = false;

			//string id = NSUserDefaults.StandardUserDefaults.StringForKey("id");
			//string userName = NSUserDefaults.StandardUserDefaults.StringForKey("userName");
			//var url = "http://go-heja.com/nitro/profile.php?txt=" + userName + "&userId=" + id;
			//calendarWebView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

			//SeriousViewController registerVC = Storyboard.InstantiateViewController("vcSerious") as SeriousViewController;
			//MainPageViewController rootVC = this.ParentViewController as MainPageViewController;
			rootVC.SetCurrentPage(3);
		}
		partial void DoneBtn_TouchUpInside(UIButton sender)
		{
			calendarWebView.Hidden = true;
			calendarContent.Hidden = true;
			btnDone.Hidden = true;

			if (!setPersonalData())
			{
				ShowMessageBox(null, "Error getting user data!");
			}
		}
		#endregion

		#region photo library
		partial void MeImgBtn_TouchUpInside(UIButton sender)
		{
			this.PresentViewController(imagePicker, true, null);
		}
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
				if (calendar.Title == "Nitro Events")
				{
					NSError pE;
					App.Current.EventStore.RemoveCalendar(calendar, true, out pE);
				}
			}
		}
		#endregion
	}
}
