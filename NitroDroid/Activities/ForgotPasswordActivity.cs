
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
		EditText txtEmail;
		ImageView invalidEmail;
		LinearLayout errorEmail;

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

			invalidEmail = FindViewById<ImageView>(Resource.Id.invalidEmail);
			errorEmail = FindViewById<LinearLayout>(Resource.Id.errorEmail);

			invalidEmail.Visibility = ViewStates.Invisible;
			errorEmail.Visibility = ViewStates.Invisible;
		}

		private bool Validate()
		{
			invalidEmail.Visibility = ViewStates.Visible;
			
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

			return isValid;
		}

		void ActionResetPassword(object sender, EventArgs e)
		{
			if (Validate())
			{
				System.Threading.ThreadPool.QueueUserWorkItem(delegate
				{
					ShowLoadingView("Requesting...");

					string isSuccess = GetCode(txtEmail.Text);

					HideLoadingView();

					if (isSuccess == "1")
					{
						AlertDialog.Builder alert = new AlertDialog.Builder(this);
						alert.SetTitle("");
						alert.SetMessage("A mail was sent to you with a temporary code");
						alert.SetPositiveButton("OK", (senderAlert, args) =>
						{
							RunOnUiThread(() => { base.OnBackPressed(); });
						});
						RunOnUiThread(() =>
						{
							alert.Show();
						});
					}
					else if (isSuccess == "0")
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
