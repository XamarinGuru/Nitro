using Foundation;
using System;
using UIKit;
using BigTed;

using CoreGraphics;
using Security;

using System.Collections.Generic;
using System.Threading.Tasks;

using System.Threading;

namespace location2
{
	partial class vcListing : PageContentViewController
	{
		private float scroll_amount = 0.0f;    // amount to scroll 
		private bool moveViewUp = false;

		string[] athData;
		Reachability.Reachability connection;
		string deviceId;

		//string id = default(string);
		bool username, firstname, password, termsAccepted, email, age;
		string validationMessage = "Note red fields!";
		trackSvc.Service1 meServ = new trackSvc.Service1();

		public vcListing(IntPtr handle) : base(handle)
		{
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			username = true;
			firstname = true;
			password = true;
			age = true;
			termsAccepted = false;

			this.listingView.AccessibilityScroll(UIAccessibilityScrollDirection.Up);

			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, KeyBoardUpNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);

			//GetUDID();
		}

		private void GetUDID()
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				BeginInvokeOnMainThread(delegate
				{
					var rec = new SecRecord(SecKind.GenericPassword)
					{
						Generic = NSData.FromString("foo")
					};

					SecStatusCode res;
					var match = SecKeyChain.QueryAsRecord(rec, out res);
					if (res == SecStatusCode.Success)
					{
						var id = match.ValueData.ToString();
						NSUserDefaults.StandardUserDefaults.SetString(id, "deviceId");
						GoToMainPage();
					}
					else
					{
						var s = new SecRecord(SecKind.GenericPassword)
						{
							Label = "Item Label",
							Description = "Item description",
							Account = "Account",
							Service = "Service",
							Comment = "Your comment here",
							ValueData = NSData.FromString(UIDevice.CurrentDevice.IdentifierForVendor.AsString()),
							Generic = NSData.FromString("foo")
						};


						var err = SecKeyChain.Add(s);

						if (err != SecStatusCode.Success && err != SecStatusCode.DuplicateItem)
							new UIAlertView(null, string.Format("Error adding record: {0}", err), null, "OK", null).Show();

						var id = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
						NSUserDefaults.StandardUserDefaults.SetString(id, "deviceId");
					}
				});
			});
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			//displaying profile image in dropdown
			connection = new Reachability.Reachability();
			if (!connection.IsHostReachable("www.google.com"))
			{
				new UIAlertView(null, "No internet connection!", null, "OK", null).Show();
				this.Title = "No internet connection..";
				return;
			}
		}

		//private void CheckGoToMainPage(string deviceUDID)
		//{
		//	UIApplication.SharedApplication.IdleTimerDisabled = true;
		//	///check if device exists in service
		//	/// if not - list the user using vcListing
		//	//check the device id
		//	meServ = new location2.trackSvc.Service1();
		//	try
		//	{
		//		deviceId = meServ.getListedDeviceId(deviceUDID);
		//	}
		//	catch
		//	{
		//		new UIAlertView(null, "You are not connected to Nitro services...", null, "OK", null).Show();
		//		deviceId = "tempDeviceId";
		//	}

		//	if (deviceId == "0")
		//	{
		//		//vcListing controller = this.Storyboard.InstantiateViewController("vcListing") as vcListing;
		//		//this.NavigationController.PushViewController(controller, true);
		//	}
		//	else {
		//		athData = meServ.getAthDataByDeviceId(NSUserDefaults.StandardUserDefaults.StringForKey("deviceId"));
		//		if (athData == null) athData = meServ.getAthDataByDeviceId(deviceUDID);
		//		NSUserDefaults.StandardUserDefaults.SetString(deviceUDID, "deviceId");
		//		NSUserDefaults.StandardUserDefaults.SetString(athData[0].ToString(), "firstName");
		//		NSUserDefaults.StandardUserDefaults.SetString(athData[1].ToString(), "lastName");
		//		NSUserDefaults.StandardUserDefaults.SetString(athData[2].ToString(), "id");
		//		NSUserDefaults.StandardUserDefaults.SetString(athData[3].ToString(), "country");
		//		NSUserDefaults.StandardUserDefaults.SetString(athData[4].ToString(), "userName");
		//		NSUserDefaults.StandardUserDefaults.SetString(athData[5].ToString(), "password");

		//		InvokeOnMainThread(() =>
		//		{
		//			MainPageViewController controller = this.Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
		//			this.PresentViewController(controller, true, null);
		//		});


		//	}
		//}

		partial void ListBtn_TouchUpInside (UIButton sender)
		{
			if (termsAccepted == false)
			{
				new UIAlertView(null, "You didnt accept terms!", null, "OK", null).Show();
				return;
			}
			validate();
			if (password == false || firstname == false || username == false)
			{
				new UIAlertView(null, "Note red fields!", null, "OK", null).Show();

			}
			else
			{
				try
				{
					var id = NSUserDefaults.StandardUserDefaults.StringForKey("deviceId");
					int tempage = 30;
					int.TryParse(this.ageTxt.Text,out tempage);
					meServ.insertNewDevice(this.firstNameTexInput.Text, this.lastNameTextInput.Text, id, this.nickeNameTextInput.Text, this.passwordTextInput.Text, true, true,emailText.Text, 40,true);

					GoToMainPage();
				}
				catch (Exception err)
				{
					new UIAlertView(null, err.ToString(), null, "OK", null).Show();
				}
			}
		}

		private void GoToMainPage()
		{
			InvokeOnMainThread(() =>
			{
				MainPageViewController controller = this.Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
				this.PresentViewController(controller, false, null);
			});
		}
		private void validate()
		{
				// perform a simple "required" validation
			if (firstNameTexInput.Text.Length <= 0) {
				// need to update on the main thread to change the border color
				InvokeOnMainThread (() => {
					//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
					this.firstNameTexInput.Layer.BorderColor = UIColor.Red.CGColor;
					this.firstNameTexInput.Layer.BorderWidth = 3;
					this.firstNameTexInput.Layer.CornerRadius = 5;
					firstname = false;
				});
			} else {
				firstname = true;
			}
				

				// perform a simple "required" validation
			if ( nickeNameTextInput.Text.Length <= 0 ) {
					// need to update on the main thread to change the border color
					InvokeOnMainThread ( () => {
						//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
						this.nickeNameTextInput.Layer.BorderColor = UIColor.Red.CGColor;
						this.nickeNameTextInput.Layer.BorderWidth = 3;
						this.nickeNameTextInput.Layer.CornerRadius = 5;
						username=false;
					} );
			}else {
				username = true;
			}
			if ( nickeNameTextInput.Text.Length >= 8 ) {
				// need to update on the main thread to change the border color
				InvokeOnMainThread ( () => {
					//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
					this.nickeNameTextInput.Layer.BorderColor = UIColor.Red.CGColor;
					this.nickeNameTextInput.Layer.BorderWidth = 3;
					this.nickeNameTextInput.Layer.CornerRadius = 5;
					username=false;
				} );
			}else {
				username = true;
			}
			if (ageTxt.Text.Length < 1)
			{
				// need to update on the main thread to change the border color
				InvokeOnMainThread(() =>
				{
					//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
					this.ageTxt.Layer.BorderColor = UIColor.Red.CGColor;
					this.ageTxt.Layer.BorderWidth = 3;
					this.ageTxt.Layer.CornerRadius = 5;
					username = false;
				});
			}
			else {
				username = true;
			}
			if (ageTxt.Text.Length >2)
			{
				// need to update on the main thread to change the border color
				InvokeOnMainThread(() =>
				{
					//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
					this.ageTxt.Layer.BorderColor = UIColor.Red.CGColor;
					this.ageTxt.Layer.BorderWidth = 3;
					this.ageTxt.Layer.CornerRadius = 5;
					age = false;
				});
			}
			else {
				age = true;
			}
			int testage = 0;
			int.TryParse(ageTxt.Text,out testage);
			if (testage < 8)
			{
				// need to update on the main thread to change the border color
				InvokeOnMainThread(() =>
				{
					//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
					this.ageTxt.Layer.BorderColor = UIColor.Red.CGColor;
					this.ageTxt.Layer.BorderWidth = 3;
					this.ageTxt.Layer.CornerRadius = 5;
					username = false;
				});
			}
			else {
				username = true;
			}
			if (testage >90)
			{
				// need to update on the main thread to change the border color
				InvokeOnMainThread(() =>
				{
					//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
					this.ageTxt.Layer.BorderColor = UIColor.Red.CGColor;
					this.ageTxt.Layer.BorderWidth = 3;
					this.ageTxt.Layer.CornerRadius = 5;
					age = false;
				});
			}
			else {
				age = true;
			}
				// perform a simple "required" validation
			if ( passwordTextInput.Text.Length <= 0 ) {
					// need to update on the main thread to change the border color
					InvokeOnMainThread ( () => {
						//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
						this.passwordTextInput.Layer.BorderColor = UIColor.Red.CGColor;
						this.passwordTextInput.Layer.BorderWidth = 3;
						this.passwordTextInput.Layer.CornerRadius = 5;
						password=false;
					} );
			}else {
				password = true;
			}

			// perform a simple "required" validation
			if ( !(emailText.Text.Contains("@"))||!(emailText.Text.Contains("."))) {
				// need to update on the main thread to change the border color
				InvokeOnMainThread ( () => {
					//this.firstNameTexInput.BackgroundColor = UIColor.Yellow;
					this.passwordTextInput.Layer.BorderColor = UIColor.Red.CGColor;
					this.passwordTextInput.Layer.BorderWidth = 3;
					this.passwordTextInput.Layer.CornerRadius = 5;
					email=false;
					validationMessage="E-mail not valid!";
				} );
			}else 
			{
				email = true;
			}
		}


		partial void AcceprtBtn_TouchUpInside (UIButton sender)
		{
			if (!validateUserNickName())
			{
				new UIAlertView(null, "Nick name is taken, try another.", null, "OK", null).Show();
				return;
			}
			termsAccepted=true;
			acceprtBtn.SetTitle("Terms accepted!  ",UIControlState.Normal);
		}

		partial void TermsBtn_TouchUpInside (UIButton sender)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/nitro/terms.php/"));
		}
		private bool validateUserNickName()
		{
			string validate = meServ.validateNickName (nickeNameTextInput.Text);
			if (validate != "1") {
				return true;
			} else {
				return false;
			}
		}

		private void KeyBoardUpNotification(NSNotification notification)
		{
			if (!emailText.IsEditing && !passwordTextInput.IsEditing)
				return;
			
			CGRect r = UIKeyboard.BoundsFromNotification(notification);

			scroll_amount = (float)r.Height / 2;// - ((float)this.View.Frame.Y - (float)this.View.Frame.Size.Height);

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
	}
}
