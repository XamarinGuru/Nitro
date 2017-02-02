﻿
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
	[Activity(Label = "ChangePasswordActivity")]
	public class ChangePasswordActivity : BaseActivity
	{
		EditText txtPassword, txtPwConfirm;

		ImageView invalidPassword, invalidPwConfirm;

		LinearLayout errorPassword, errorPwConfirm;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.ChangePasswordActivity);

			InitUI();
		}

		void InitUI()
		{
			FindViewById<Button>(Resource.Id.ActionResetPassword).Click += ActionResetPassword;
			FindViewById<ImageView>(Resource.Id.ActionBack).Click += ActionBack;

			txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
			txtPwConfirm = FindViewById<EditText>(Resource.Id.txtPwConfirm);

			invalidPassword = FindViewById<ImageView>(Resource.Id.invalidPassword);
			errorPassword = FindViewById<LinearLayout>(Resource.Id.errorPassword);
			invalidPwConfirm = FindViewById<ImageView>(Resource.Id.invalidPwConfirm);
			errorPwConfirm = FindViewById<LinearLayout>(Resource.Id.errorPwConfirm);

			invalidPassword.Visibility = ViewStates.Invisible;
			invalidPwConfirm.Visibility = ViewStates.Invisible;
			errorPassword.Visibility = ViewStates.Invisible;
			errorPwConfirm.Visibility = ViewStates.Invisible;
		}

		private bool Validate()
		{
			invalidPassword.Visibility = ViewStates.Visible;
			invalidPwConfirm.Visibility = ViewStates.Visible;
			
			bool isValid = true;

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

					int isSuccess = ResetPassword(AppSettings.currentEmail, txtPassword.Text);

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
			var activity = new Intent(this, typeof(SwipeTabActivity));
			StartActivity(activity);
			Finish();
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				var activity = new Intent(this, typeof(SwipeTabActivity));
				StartActivity(activity);
				Finish();
			}

			return base.OnKeyDown(keyCode, e);
		}
	}
}