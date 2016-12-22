using Foundation;
using System;
using UIKit;
using EventKit;

namespace location2
{
	partial class SeriousViewController : BaseViewController
	{
		UIImagePickerController imagePicker = new UIImagePickerController();

		public SeriousViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

