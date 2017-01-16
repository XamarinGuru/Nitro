//using Foundation;
//using System;
//using UIKit;
//using CoreGraphics;
//using Security;
//using System.Threading;
//using PortableLibrary;

//namespace location2
//{
//	partial class RegisterViewController : BaseViewController
//	{
//		bool termsAccepted = false;

//		private float scroll_amount = 0.0f;
//		private bool moveViewUp = false;

//		public RegisterViewController() : base()
//		{
//		}
//		public RegisterViewController(IntPtr handle) : base(handle)
//		{
//		}

//		public override void ViewDidLoad()
//		{
//			base.ViewDidLoad();

//			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
//			View.AddGestureRecognizer(g);

//			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, KeyBoardUpNotification);
//			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
//		}

//		public override void ViewDidAppear(bool animated)
//		{
//			base.ViewWillAppear(animated);

//			if (!IsNetEnable())
//			{
//				ShowMessageBox(null, "No internet connection!");
//				this.Title = "No internet connection..";
//				return;
//			}
//		}

//		private bool Validate()
//		{
//			if (termsAccepted == false)
//			{
//				ShowMessageBox(null, "You didnt accept terms!");
//				return false;
//			}

//			if (!(txtEmail.Text.Contains("@")) || !(txtEmail.Text.Contains(".")))
//			{
//				ShowMessageBox(null, "E-mail not valid!");
//				return false;
//			}

//			bool isValidate = true;

//			if (txtFirstName.Text.Length <= 0) {
//				isValidate = false;
//				MarkAsInvalide(txtFirstName);
//			}
				
//			if ( txtNickName.Text.Length <= 0 || txtNickName.Text.Length >= 8) {
//				isValidate = false;
//				MarkAsInvalide(txtNickName);
//			}

//			int testage = 0;
//			int.TryParse(txtAge.Text, out testage);
//			if (txtAge.Text.Length < 1 || txtAge.Text.Length > 2 || testage < 8 || testage > 90)
//			{
//				isValidate = false;
//				MarkAsInvalide(txtAge);
//			}

//			if (txtPassword.Text.Length <= 0 ) {
//				isValidate = false;
//				MarkAsInvalide(txtPassword);
//			}

//			return isValidate;
//		}

//		#region keyboard process
//		private void KeyBoardUpNotification(NSNotification notification)
//		{
//			if (!txtEmail.IsEditing && !txtPassword.IsEditing)
//				return;
			
//			CGRect r = UIKeyboard.BoundsFromNotification(notification);

//			scroll_amount = (float)r.Height / 2;

//			if (scroll_amount > 0)
//			{
//				moveViewUp = true;
//				ScrollTheView(moveViewUp);
//			}
//			else {
//				moveViewUp = false;
//			}
//		}
//		private void KeyBoardDownNotification(NSNotification notification)
//		{
//			if (moveViewUp) { ScrollTheView(false); }
//		}
//		private void ScrollTheView(bool move)
//		{
//			// scroll the view up or down
//			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
//			UIView.SetAnimationDuration(0.3);

//			CGRect frame = this.View.Frame;

//			if (move)
//			{
//				frame.Y = -(scroll_amount);
//			}
//			else {
//				frame.Y = 0;
//			}

//			this.View.Frame = frame;
//			UIView.CommitAnimations();
//		}
//		#endregion

//		#region event handler
//		partial void ListBtn_TouchUpInside(UIButton sender)
//		{
//			if (Validate())
//			{
//				try
//				{
//					string deviceUDID  = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

//					var result = RegisterUser(txtFirstName.Text, txtLastName.Text, deviceUDID, txtNickName.Text, txtPassword.Text, txtEmail.Text, int.Parse(txtAge.Text));

//					if (result == "user added")
//						GoToMainPage(deviceUDID);
//					else
//						ShowMessageBox(null, result);
						
//				}
//				catch (Exception err)
//				{
//					ShowMessageBox(null, err.Message.ToString());
//				}
//			}
//		}

//		partial void AcceprtBtn_TouchUpInside(UIButton sender)
//		{
//			if (!ValidateUserNickName(txtNickName.Text))
//			{
//				ShowMessageBox(null, "Nick name is taken, try another.");
//				return;
//			}
//			termsAccepted = true;
//			acceprtBtn.SetTitle("Terms accepted!  ", UIControlState.Normal);
//		}

//		partial void TermsBtn_TouchUpInside(UIButton sender)
//		{
//			UIApplication.SharedApplication.OpenUrl(new NSUrl(Constants.TERMS_URL));
//		}
//		#endregion

//		private void GoToMainPage(string deviceUDID)
//		{
//			AppSettings.Email = txtEmail.Text;
//			AppSettings.Password = txtPassword.Text;
//			AppSettings.Username = txtNickName.Text;
//			AppSettings.DeviceUDID = deviceUDID;

//			string userID = GetUserID();

//			if (userID == "0")//if the user not registered yet, go to register screen
//			{
//				ShowMessageBox(null, "You are not registered to Nitro services...");
//			}
//			else//if the user already registered, go to main screen
//			{
//				MainPageViewController mainVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
//				this.PresentViewController(mainVC, false, null);
//			}
//			//ThreadPool.QueueUserWorkItem(delegate
//			//{
//			//	BeginInvokeOnMainThread(delegate
//			//	{
//			//		//register device id to keychain
//			//		var s = new SecRecord(SecKind.GenericPassword)
//			//		{
//			//			Label = "Item Label",
//			//			Description = "Item description",
//			//			Account = "Account",
//			//			Service = "Service",
//			//			Comment = "Your comment here",
//			//			ValueData = deviceID,
//			//			Generic = NSData.FromString("foo")
//			//		};


//			//		var err = SecKeyChain.Add(s);

//			//		if (err != SecStatusCode.Success && err != SecStatusCode.DuplicateItem)
//			//			ShowMessageBox(null, "Can't save device id to keychain");

//			//		//NSUserDefaults.StandardUserDefaults.SetString(deviceID, "deviceId");
//			//		NSUserDefaults.StandardUserDefaults.SetString(txtEmail.Text, "email");
//			//		NSUserDefaults.StandardUserDefaults.SetString(txtPassword.Text, "password");

//			//		MainPageViewController mainVC = this.Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
//			//		this.PresentViewController(mainVC, false, null);
//			//	});
//			//});
//		}
//	}
//}
