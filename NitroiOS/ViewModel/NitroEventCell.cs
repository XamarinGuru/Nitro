using System;
using Foundation;
using PortableLibrary;
using UIKit;

namespace location2
{
	public partial class NitroEventCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("NitroEventCell");
		public static readonly UINib Nib;

		static NitroEventCell()
		{
			Nib = UINib.FromName("NitroEventCell", NSBundle.MainBundle);
		}

		protected NitroEventCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public void SetCell(NitroEvent nitroEvent)
		{
			lblTitle.Text = nitroEvent.title;

			var eventDate = nitroEvent.StartDateTime();
			lblEventTime.Text = String.Format("{0:t}", eventDate);

			switch (nitroEvent.type)
			{
				case "0":
					imgType.Image = UIImage.FromFile("icon_triathlon.png");
					break;
				case "1":
					imgType.Image = UIImage.FromFile("icon_bike.png");
					break;
				case "2":
					imgType.Image = UIImage.FromFile("icon_run.png");
					break;
				case "3":
					imgType.Image = UIImage.FromFile("icon_swim.png");
					break;
				case "4":
					imgType.Image = UIImage.FromFile("icon_triathlon.png");
					break;
				case "5":
					imgType.Image = UIImage.FromFile("icon_other.png");
					break;
			}
		}
	}
}
