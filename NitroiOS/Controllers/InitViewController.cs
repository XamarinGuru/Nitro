using Foundation;
using System;
using UIKit;

using System.Threading;
using Security;

using BigTed;

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
			SetFirstViewController();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		private void SetFirstViewController()
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

						trackSvc.Service1 serv = new trackSvc.Service1();
						string deviceId = "0";

						try
						{
							deviceId = serv.getListedDeviceId(id);
						}
						catch (Exception err)
						{
							ShowMessageBox(null, err.Message);
							return;
						}

						if (deviceId == "0")//if the user not registered yet, go to register screen
						{
							RegisterViewController registerVC = Storyboard.InstantiateViewController("vcListing") as RegisterViewController;
							registerVC.deviceID = id;
							this.PresentViewController(registerVC, false, null);
						}
						else//if the user already registered, go to main screen
						{
							NSUserDefaults.StandardUserDefaults.SetString(id, "deviceId");

							MainPageViewController mainVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
							this.PresentViewController(mainVC, false, null);
						}
					}
					else
					{
						RegisterViewController controller = Storyboard.InstantiateViewController("vcListing") as RegisterViewController;
						controller.deviceID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
						this.PresentViewController(controller, false, null);
					}
				});
			});
		}
	}
}

