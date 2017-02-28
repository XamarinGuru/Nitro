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
			var strTitle = nitroEvent.title;
			var strTime = String.Format("{0:t}", nitroEvent.StartDateTime());

			lblTitle.Text = strTitle;
			lblEventTime.Text = strTime;

			if (nitroEvent.attended == "0" && nitroEvent.StartDateTime().DayOfYear <= DateTime.Now.DayOfYear)
			{
				var attrTitle = new NSAttributedString(lblTitle.Text, strikethroughStyle: NSUnderlineStyle.Single);
				var attrTime = new NSAttributedString(lblEventTime.Text, strikethroughStyle: NSUnderlineStyle.Single);
				lblTitle.AttributedText = attrTitle;
				lblEventTime.AttributedText = attrTime;
				lblTitle.TextColor = UIColor.FromRGB(112, 112, 112);
				lblEventTime.TextColor = UIColor.FromRGB(112, 112, 112);
			}
			else
			{
				var attrTitle = new NSAttributedString(lblTitle.Text, strikethroughStyle: NSUnderlineStyle.None);
				var attrTime = new NSAttributedString(lblEventTime.Text, strikethroughStyle: NSUnderlineStyle.None);
				lblTitle.AttributedText = attrTitle;
				lblEventTime.AttributedText = attrTime;
				lblTitle.TextColor = UIColor.White;
				lblEventTime.TextColor = UIColor.White;
			}

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
