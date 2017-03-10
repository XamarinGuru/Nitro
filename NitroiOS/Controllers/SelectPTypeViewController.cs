using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;

namespace location2
{
    public partial class SelectPTypeViewController : BaseViewController
    {
		UIColor COLOR_ACTIVE = new UIColor(229 / 255f, 161 / 255f, 9 / 255f, alpha: 1.0f);
		UIColor COLOR_DISABLE = new UIColor(67 / 255f, 67 / 255f, 67 / 255f, alpha: 1.0f);

		public NitroEvent selectedEvent;

        public SelectPTypeViewController() : base()
		{
		}
		public SelectPTypeViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = true;

			var leftButton = new UIButton(new CGRect(0, 0, 20, 20));
			leftButton.SetImage(UIImage.FromFile("icon_left.png"), UIControlState.Normal);
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			InitUISettings();
		}

		void InitUISettings()
		{
			stateCycling.BackgroundColor = COLOR_DISABLE;
			stateRunning.BackgroundColor = COLOR_DISABLE;
			stateSwimming.BackgroundColor = COLOR_DISABLE;
			stateTriathlon.BackgroundColor = COLOR_DISABLE;
			stateOther.BackgroundColor = COLOR_DISABLE;

			switch (selectedEvent.type)
			{
				case "1":
					stateCycling.BackgroundColor = COLOR_ACTIVE;
					break;
				case "2":
					stateRunning.BackgroundColor = COLOR_ACTIVE;
					break;
				case "3":
					stateSwimming.BackgroundColor = COLOR_ACTIVE;
					break;
				case "4":
					stateTriathlon.BackgroundColor = COLOR_ACTIVE;
					break;
				case "5":
					stateOther.BackgroundColor = COLOR_ACTIVE;
					break;
			}
		}

		partial void ActionSelectedType(UIButton sender)
		{
			selectedEvent.type = sender.Tag.ToString();
			NavigationController.PopViewController(true);
		}
	}
}