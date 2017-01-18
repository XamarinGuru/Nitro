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
		}
	}
}
