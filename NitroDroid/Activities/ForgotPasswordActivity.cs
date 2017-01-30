
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
	[Activity(Label = "ForgotPasswordActivity")]
	public class ForgotPasswordActivity : BaseActivity
	{
		EditText txtEmail, txtPassword, txtPwConfirm;

		ImageView invalidEmail, invalidPassword, invalidPwConfirm;

		LinearLayout errorEmail, errorPassword, errorPwConfirm;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.ForgotPasswordActivity);

			InitUI();
		}

		void InitUI()
		{
			FindViewById<Button>(Resource.Id.ActionResetPassword).Click += ActionResetPassword;
			FindViewById<ImageView>(Resource.Id.ActionBack).Click += ActionBack;

			txtEmail = FindViewById<EditText>(Resource.Id.txtEmail);
			txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
			txtPwConfirm = FindViewById<EditText>(Resource.Id.txtPwConfirm);

			invalidEmail = FindViewById<ImageView>(Resource.Id.invalidEmail);
			invalidPassword = FindViewById<ImageView>(Resource.Id.invalidPassword);
			errorEmail = FindViewById<LinearLayout>(Resource.Id.errorEmail);
			errorPassword = FindViewById<LinearLayout>(Resource.Id.errorPassword);
			invalidPwConfirm = FindViewById<ImageView>(Resource.Id.invalidPwConfirm);
			errorPwConfirm = FindViewById<LinearLayout>(Resource.Id.errorPwConfirm);

			invalidEmail.Visibility = ViewStates.Invisible;
			invalidPassword.Visibility = ViewStates.Invisible;
			invalidPwConfirm.Visibility = ViewStates.Invisible;
			errorEmail.Visibility = ViewStates.Invisible;
			errorPassword.Visibility = ViewStates.Invisible;
			errorPwConfirm.Visibility = ViewStates.Invisible;
		}

		private bool Validate()
		{
			invalidEmail.Visibility = ViewStates.Visible;
			invalidPassword.Visibility = ViewStates.Visible;
			invalidPwConfirm.Visibility = ViewStates.Visible;
			
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

			if (txtPwConfirm.Text.Length <= 0 || txtPwConfirm.Text != txtPassword.Text)
			{
				MarkAsInvalide(invalidPwConfirm, errorPwConfirm, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidPwConfirm, errorPwConfirm, false);
			}

			return isValid;
		}

		void ActionResetPassword(object sender, EventArgs e)
		{
			if (Validate())
			{
				System.Threading.ThreadPool.QueueUserWorkItem(delegate
				{
					ShowLoadingView("Requesting...");

					int isSuccess = ResetPassword(txtEmail.Text, txtPassword.Text);

					HideLoadingView();

					if (isSuccess == 1)
					{
						AlertDialog.Builder alert = new AlertDialog.Builder(this);
						alert.SetTitle("");
						alert.SetMessage("Password updated successfully");
						alert.SetPositiveButton("OK", (senderAlert, args) =>
						{
							RunOnUiThread(() => { base.OnBackPressed(); });
						});
						RunOnUiThread(() =>
						{
							alert.Show();
						});
					}
					else if (isSuccess == 2)
					{
						ShowMessageBox(null, "Passwords don’t match");
					}
					else if (isSuccess == 3)
					{
						ShowMessageBox(null, "Email do not exists in the system , please try again or signup");
					}
				});

			}
		}

		void ActionBack(object sender, EventArgs e)
		{
			base.OnBackPressed();
		}
	}
}
