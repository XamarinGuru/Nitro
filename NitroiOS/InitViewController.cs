using Foundation;
using System;
using UIKit;

using System.Threading;
using Security;

using BigTed;



using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using FireSharp;
using FireSharp.Extensions;





namespace location2
{
	public partial class InitViewController : UIViewController
	{



		//private static FirebaseClient _client;

		//public class Todo
		//{
		//	public string name { get; set; }
		//	public int priority { get; set; }
		//}




		public InitViewController(IntPtr handle) : base(handle)
		{
		}
		public InitViewController() : base("InitViewController", null)
		{
		}

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();



			//IFirebaseConfig config = new FirebaseConfig
			//{
			//	AuthSecret = "PtLxxW6zYGZSE3UXmmiFxVCqzNdZOLxLNHdHNixF",
			//	BasePath = "https://nitro-8cbda.firebaseio.com/"
			//};
			//_client = new FirebaseClient(config);
			//var todo = new Todo
			//{
			//	name = "Execute SET",
			//	priority = 2
			//};
			//SetResponse response = await _client.SetAsync("todos/set", todo);
			//Todo result = response.ResultAs<Todo>();

			//var todo1 = new Todo
			//{
			//	name = "Execute PUSH",
			//	priority = 2
			//};
			//PushResponse response1 = await _client.PushAsync("todos/push", todo1);
			//var strName = response1.Result.name;




			// Perform any additional setup after loading the view, typically from a nib.
			BTProgressHUD.Show("Verifying Your Device...");

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
							//var builder = new AlertDialog.Builder(this);
							//builder.SetTitle("Nitro service is not available");
							//builder.SetMessage("Oops!Service not available... Pls try again later");
							//builder.SetCancelable(false);
							//builder.SetPositiveButton("OK", delegate { Finish(); });
							//builder.Show();
							//return;
						}

						if (deviceId == "0")
						{
							vcListing controller1 = Storyboard.InstantiateViewController("vcListing") as vcListing;
							controller1.deviceID = id;
							this.PresentViewController(controller1, false, null);
						}
						else
						{
							NSUserDefaults.StandardUserDefaults.SetString(id, "deviceId");

							MainPageViewController controller = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
							this.PresentViewController(controller, false, null);
						}

						//var id = match.ValueData.ToString();
						//NSUserDefaults.StandardUserDefaults.SetString(id, "deviceId");

						//MainPageViewController controller = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
						//this.PresentViewController(controller, false, null);
					}
					else
					{
						

						vcListing controller = Storyboard.InstantiateViewController("vcListing") as vcListing;
						controller.deviceID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
						this.PresentViewController(controller, false, null);
					}
					BTProgressHUD.Dismiss();
				});
			});
		}
	}
}

