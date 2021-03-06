﻿﻿using Foundation;
using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;
using System.Threading;

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
			base.ViewDidLoad();

			var g = new UITapGestureRecognizer(() => View.EndEditing(true));
			View.AddGestureRecognizer(g);

			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, KeyBoardUpNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);

			InitUISettings();
		}

		void InitUISettings()
		{
			btnLogin.BackgroundColor = GROUP_COLOR;
		}

		private bool Validate()
		{
			btnValidPassword.Hidden = false;
			btnValidEmail.Hidden = false;

			bool isValid = true;

			if (!(txtEmail.Text.Contains("@")) || !(txtEmail.Text.Contains(".")))
			{
				MarkAsInvalide(btnValidEmail, viewErrorEmail, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(btnValidEmail, viewErrorEmail, false);
			}

			if (txtPassword.Text.Length <= 0)
			{
				MarkAsInvalide(btnValidPassword, viewErrorPassword, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(btnValidPassword, viewErrorPassword, false);
			}

			return isValid;
		}

		partial void ActionLogin(UIButton sender)
		{
			if (!IsNetEnable()) return;

			if (Validate())
			{
                var strEmail = txtEmail.Text;
                var strPassword = txtPassword.Text;

                ThreadPool.QueueUserWorkItem(delegate
				{
					ShowLoadingView(Constants.MSG_LOGIN);
					var loginUser = LoginUser(strEmail, strPassword);
					HideLoadingView();

                    InvokeOnMainThread(() =>
                    {
						if (loginUser == null)
						{
                            ShowMessageBox(null, Constants.MSG_LOGIN_FAIL);
						}
						else
						{
							UIViewController nextVC;
							if (loginUser.userType == Constants.USER_TYPE.ATHLETE)
							{
								nextVC = Storyboard.InstantiateViewController("MainPageViewController") as MainPageViewController;
							}
							else
							{
								var tabVC = Storyboard.InstantiateViewController("CoachHomeViewController") as CoachHomeViewController;
								nextVC = new UINavigationController(tabVC);

								AppDelegate myDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
								myDelegate.navVC = nextVC as UINavigationController;
							}
							PresentViewController(nextVC, true, null);
						}
					});
				});
			}
		}
		partial void ActionForgotPassword(UIButton sender)
		{
			ForgotPasswordViewController mainVC = Storyboard.InstantiateViewController("ForgotPasswordViewController") as ForgotPasswordViewController;
			PresentViewController(mainVC, true, null);
		}

		partial void ActionBack(UIButton sender)
		{
			InitViewController mainVC = Storyboard.InstantiateViewController("InitViewController") as InitViewController;
			PresentViewController(mainVC, false, null);
		}

		#region keyboard process
		void KeyBoardUpNotification(NSNotification notification)
		{
			if (!txtEmail.IsEditing && !txtPassword.IsEditing)
				return;

			CGRect r = UIKeyboard.BoundsFromNotification(notification);

			scroll_amount = (float)r.Height / 2;

			if (scroll_amount > 0)
			{
				moveViewUp = true;
				ScrollTheView(moveViewUp);
			}
			else {
				moveViewUp = false;
			}
		}
		void KeyBoardDownNotification(NSNotification notification)
		{
			if (moveViewUp) { ScrollTheView(false); }
		}
		void ScrollTheView(bool move)
		{
			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration(0.3);

			CGRect frame = this.View.Frame;

			if (move)
			{
				frame.Y = -(scroll_amount);
			}
			else {
				frame.Y = 0;
			}

			View.Frame = frame;
			UIView.CommitAnimations();
		}
		#endregion
    }
}