using System;

using UIKit;

namespace location2
{
	public partial class PageContentViewController : UIViewController
	{
		public int pageIndex = 0;
		public string titleText;
		public string imageFile;

		public PageContentViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//imageView.Image = UIImage.FromBundle(imageFile);
			//label.Text = titleText;
		}
	}
}


