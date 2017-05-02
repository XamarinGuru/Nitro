using Foundation;
using System;
using UIKit;
using PortableLibrary;

namespace location2
{
    public partial class UserCell : UITableViewCell
    {
		public static readonly NSString Key = new NSString("UserCell");
		public static readonly UINib Nib;

		static UserCell()
		{
			Nib = UINib.FromName("UserCell", NSBundle.MainBundle);
		}

		protected UserCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public void SetCell(User user)
		{
			lblName.Text = user.name;

			//switch (goHejaEvent.type)
			//{
			//	case "0":
			//		imgType.Image = UIImage.FromFile("icon_triathlon.png");
			//		break;
			//	case "1":
			//		imgType.Image = UIImage.FromFile("icon_bike.png");
			//		break;
			//	case "2":
			//		imgType.Image = UIImage.FromFile("icon_run.png");
			//		break;
			//	case "3":
			//		imgType.Image = UIImage.FromFile("icon_swim.png");
			//		break;
			//	case "4":
			//		imgType.Image = UIImage.FromFile("icon_triathlon.png");
			//		break;
			//	case "5":
			//		imgType.Image = UIImage.FromFile("icon_other.png");
			//		break;
			//}
		}
    }
}