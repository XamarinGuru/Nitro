﻿using System;
using System.Threading.Tasks;
using UIKit;

namespace location2
{
	public partial class InitViewController : BaseViewController
	{
		public InitViewController() : base()
		{
		}
		public InitViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			GotoMainIfAlreadyLoggedin();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		private void GotoMainIfAlreadyLoggedin()
		{
			string userID = "0";
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Sign In...");

				userID = GetUserID();

				InvokeOnMainThread(() =>
				{
					HideLoadingView();

					if (userID != "0")
					{
						MainPageViewController mainVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
						this.PresentViewController(mainVC, false, null);
					}
				});
			});
		}

		partial void ActionSignIn(UIButton sender)
		{
			LoginViewController mainVC = Storyboard.InstantiateViewController("LoginViewController") as LoginViewController;
			this.PresentViewController(mainVC, true, null);
		}

		partial void ActionSignUp(UIButton sender)
		{
			SignUpViewController mainVC = Storyboard.InstantiateViewController("SignUpViewController") as SignUpViewController;
			this.PresentViewController(mainVC, true, null);
		}
		//private void SetFirstViewController()
		//{
		//	ThreadPool.QueueUserWorkItem(delegate
		//	{
		//		BeginInvokeOnMainThread(delegate
		//		{
		//			var rec = new SecRecord(SecKind.GenericPassword)
		//			{
		//				Generic = NSData.FromString("foo")
		//			};

		//			SecStatusCode res;
		//			var match = SecKeyChain.QueryAsRecord(rec, out res);
		//			if (res == SecStatusCode.Success)
		//			{
		//				var id = match.ValueData.ToString();

		//				trackSvc.Service1 serv = new trackSvc.Service1();
		//				string deviceId = "0";

		//				try
		//				{
		//					string strEmail = NSUserDefaults.StandardUserDefaults.StringForKey("email");
		//					string strPassword = NSUserDefaults.StandardUserDefaults.StringForKey("password");

		//					deviceId = serv.getListedDeviceId(strEmail, strPassword);
		//				}
		//				catch (Exception err)
		//				{
		//					ShowMessageBox(null, err.Message);
		//					return;
		//				}

		//				if (deviceId == "0")//if the user not registered yet, go to register screen
		//				{
		//					RegisterViewController registerVC = Storyboard.InstantiateViewController("vcListing") as RegisterViewController;
		//					registerVC.deviceID = id;
		//					this.PresentViewController(registerVC, false, null);
		//				}
		//				else//if the user already registered, go to main screen
		//				{
		//					NSUserDefaults.StandardUserDefaults.SetString(id, "deviceId");

		//					MainPageViewController mainVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
		//					this.PresentViewController(mainVC, false, null);
		//				}
		//			}
		//			else
		//			{
		//				RegisterViewController controller = Storyboard.InstantiateViewController("vcListing") as RegisterViewController;
		//				controller.deviceID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
		//				this.PresentViewController(controller, false, null);
		//			}
		//		});
		//	});
		//}
	}
}

