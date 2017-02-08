
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace goheja
{
	[Activity(Label = "LoginActivity")]
	public class LoginActivity : BaseActivity
	{
		EditText txtEmail, txtPassword;

		ImageView invalidEmail, invalidPassword;

		LinearLayout errorEmail, errorPassword;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.LoginActivity);

			InitUI();
		}

		void InitUI()
		{
			FindViewById<Button>(Resource.Id.ActionLogin).Click += ActionLogin;
			FindViewById<ImageView>(Resource.Id.ActionBack).Click += ActionBack;
			FindViewById<Button>(Resource.Id.ActionForgotPassword).Click += ActionForgotPassword;

			txtEmail = FindViewById<EditText>(Resource.Id.txtEmail);
			txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);

			invalidEmail = FindViewById<ImageView>(Resource.Id.invalidEmail);
			invalidPassword = FindViewById<ImageView>(Resource.Id.invalidPassword);
			errorEmail = FindViewById<LinearLayout>(Resource.Id.errorEmail);
			errorPassword = FindViewById<LinearLayout>(Resource.Id.errorPassword);

			invalidEmail.Visibility = Android.Views.ViewStates.Invisible;
			invalidPassword.Visibility = Android.Views.ViewStates.Invisible;
			errorEmail.Visibility = Android.Views.ViewStates.Invisible;
			errorPassword.Visibility = Android.Views.ViewStates.Invisible;
		}

		private bool Validate()
		{
			invalidEmail.Visibility = Android.Views.ViewStates.Visible;
			invalidPassword.Visibility = Android.Views.ViewStates.Visible;

			bool isValid = true;

			if (!(txtEmail.Text.Contains("@")) || !(txtEmail.Text.Contains(".")))
			{
				MarkAsInvalide(invalidEmail, errorEmail, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidEmail, errorEmail, false);
			}

			if (txtPassword.Text.Length <= 0)
			{
				MarkAsInvalide(invalidPassword, errorPassword, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidPassword, errorPassword, false);
			}

			return isValid;
		}

		void ActionLogin(object sender, EventArgs e)
		{
			if (!IsNetEnable()) return;

			if (Validate())
			{
				System.Threading.ThreadPool.QueueUserWorkItem(delegate
				{
					ShowLoadingView("Log In...");

					bool isSuccess = LoginUser(txtEmail.Text, txtPassword.Text);

					HideLoadingView();

					if (isSuccess)
					{
						var activity = new Intent(this, typeof(SwipeTabActivity));
						StartActivity(activity);
						Finish();
					}
					else
					{
						ShowMessageBox(null, "Login Failed.");
					}
				});

			}
		}

		void ActionForgotPassword(object sender, EventArgs e)
		{
			var activity = new Intent(this, typeof(ForgotPasswordActivity));
			StartActivity(activity);
		}

		void ActionBack(object sender, EventArgs e)
		{
			var activity = new Intent(this, typeof(InitActivity));
			StartActivity(activity);
			Finish();
		}
	}
}
