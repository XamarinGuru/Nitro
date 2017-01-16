using Foundation;
using System;
using UIKit;
using System.Threading.Tasks;

namespace location2
{
    public partial class LoginViewController : BaseViewController
    {
        public LoginViewController() : base()
		{
		}
		public LoginViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
		}

		async partial void ActionLogin(UIButton sender)
		{
			ShowLoadingView("Log In...");

			bool isSuccess = false;
			await Task.Run(() =>
			{
				InvokeOnMainThread(() => { isSuccess = LoginUser(txtEmail.Text, txtPassword.Text); });
				HideLoadingView();
			});

			if (isSuccess)
			{
				MainPageViewController mainVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
				this.PresentViewController(mainVC, false, null);
			}
			else
			{
				ShowMessageBox(null, "Login Failed.");
			}

		}

		partial void ActionBack(UIButton sender)
		{
			InitViewController mainVC = Storyboard.InstantiateViewController("InitViewController") as InitViewController;
			this.PresentViewController(mainVC, false, null);
		}
    }
}