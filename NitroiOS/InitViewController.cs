using Foundation;
using System;
using UIKit;

using System.Threading;
using Security;

namespace location2
{
	public partial class InitViewController : UIViewController
	{
		public InitViewController(IntPtr handle) : base(handle)
		{
		}
		public InitViewController() : base("InitViewController", null)
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
						NSUserDefaults.StandardUserDefaults.SetString(id, "deviceId");

						MainPageViewController controller = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
						this.PresentViewController(controller, false, null);
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

						vcListing controller = Storyboard.InstantiateViewController("vcListing") as vcListing;
						this.PresentViewController(controller, false, null);
					}
				});
			});
		}
	}
}

