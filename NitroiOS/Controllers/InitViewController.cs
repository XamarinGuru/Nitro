using System;

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
			// Perform any additional setup after loading the view, typically from a nib.
			//SetFirstViewController();
			SetFirstViewController1();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		private void SetFirstViewController1()
		{
			string userID = GetUserID();
			BeginInvokeOnMainThread(delegate
			{
				if (userID == "0")//if the user not registered yet, go to register screen
				{
					RegisterViewController registerVC = Storyboard.InstantiateViewController("vcListing") as RegisterViewController;
					this.PresentViewController(registerVC, false, null);
				}
				else//if the user already registered, go to main screen
				{
					MainPageViewController mainVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
					this.PresentViewController(mainVC, false, null);
				}
			});
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

