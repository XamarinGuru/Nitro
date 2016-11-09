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
	partial class RegisterViewController : BaseViewController
	{
		public string deviceID = "";//from previous controller

		bool termsAccepted = false;

		private float scroll_amount = 0.0f;
		private bool moveViewUp = false;

		Reachability.Reachability connection;
		trackSvc.Service1 meServ = new trackSvc.Service1();

		public RegisterViewController() : base()
		{
		}
		public RegisterViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, KeyBoardUpNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			connection = new Reachability.Reachability();
			if (!connection.IsHostReachable("www.google.com"))
			{
				ShowMessageBox(null, "No internet connection!");
				this.Title = "No internet connection..";
				return;
			}
		}

		private void GoToMainPage()
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				BeginInvokeOnMainThread(delegate
				{
					//register device id to keychain
					var s = new SecRecord(SecKind.GenericPassword)
					{
						Label = "Item Label",
						Description = "Item description",
						Account = "Account",
						Service = "Service",
						Comment = "Your comment here",
						ValueData = deviceID,
						Generic = NSData.FromString("foo")
					};


					var err = SecKeyChain.Add(s);

					if (err != SecStatusCode.Success && err != SecStatusCode.DuplicateItem)
						ShowMessageBox(null, "Can't save device id to keychain");

					NSUserDefaults.StandardUserDefaults.SetString(deviceID, "deviceId");

					MainPageViewController mainVC = this.Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
					this.PresentViewController(mainVC, false, null);
				});
			});
		}
		private bool validate()
		{
			if (termsAccepted == false)
			{
				ShowMessageBox(null, "You didnt accept terms!");
				return false;
			}

			if (!(txtEmail.Text.Contains("@")) || !(txtEmail.Text.Contains(".")))
			{
				ShowMessageBox(null, "E-mail not valid!");
				return false;
			}

			bool isValidate = true;

			if (txtFirstName.Text.Length <= 0) {
				isValidate = false;
				markAsInvalide(txtFirstName);
			}
				
			if ( txtNickName.Text.Length <= 0 || txtNickName.Text.Length >= 8) {
				isValidate = false;
				markAsInvalide(txtNickName);
			}

			int testage = 0;
			int.TryParse(txtAge.Text, out testage);
			if (txtAge.Text.Length < 1 || txtAge.Text.Length > 2 || testage < 8 || testage > 90)
			{
				isValidate = false;
				markAsInvalide(txtAge);
			}

			if (txtPassword.Text.Length <= 0 ) {
				isValidate = false;
				markAsInvalide(txtPassword);
			}

			return isValidate;
		}

		private void markAsInvalide(UITextField textField)
		{
			InvokeOnMainThread(() =>
			{
				textField.Layer.BorderColor = UIColor.Red.CGColor;
				textField.Layer.BorderWidth = 3;
				textField.Layer.CornerRadius = 5;
			});
		}

		private bool validateUserNickName()
		{
			string validate = meServ.validateNickName (txtNickName.Text);
			if (validate != "1") {
				return true;
			} else {
				return false;
			}
		}

		private void KeyBoardUpNotification(NSNotification notification)
		{
			if (!txtEmail.IsEditing && !txtPassword.IsEditing)
				return;
			
			CGRect r = UIKeyboard.BoundsFromNotification(notification);

			scroll_amount = (float)r.Height / 2;

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

		#region event handler
		partial void ListBtn_TouchUpInside(UIButton sender)
		{
			if (validate())
			{
				try
				{
					int tempage = 30;
					int.TryParse(txtAge.Text, out tempage);
					meServ.insertNewDevice(txtFirstName.Text, txtLastName.Text, deviceID, txtLastName.Text, txtPassword.Text, true, true, txtEmail.Text, 40, true);

					GoToMainPage();
				}
				catch (Exception err)
				{
					ShowMessageBox(null, err.ToString());
				}
			}
		}

		partial void AcceprtBtn_TouchUpInside(UIButton sender)
		{
			if (!validateUserNickName())
			{
				ShowMessageBox(null, "Nick name is taken, try another.");
				return;
			}
			termsAccepted = true;
			acceprtBtn.SetTitle("Terms accepted!  ", UIControlState.Normal);
		}

		partial void TermsBtn_TouchUpInside(UIButton sender)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/nitro/terms.php/"));
		}
		#endregion
	}
}
