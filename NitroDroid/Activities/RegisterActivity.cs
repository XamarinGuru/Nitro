using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace goheja
{
    [Activity(Label = "listing")]
	public class RegisterActivity : BaseActivity
    {
		EditText txtFirstname, txtLastname, txtUsername, txtEmail, txtPassword, txtAge;

		ImageView invalidFirstname, invalidLastname, invalidUsername, invalidEmail, invalidPassword, invalidAge, invalidTerms;

		LinearLayout errorFirstname, errorLastname, errorUsername, errorEmail, errorPassword;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RegisterActivity);

			InitUI();
        }

		void InitUI()
		{
			FindViewById<Button>(Resource.Id.ActionSignUp).Click += ActionSignUp;
			FindViewById<Button>(Resource.Id.ActionTerms).Click += ActionTerms;

			txtFirstname = FindViewById<EditText>(Resource.Id.txtFirstname);
			txtLastname = FindViewById<EditText>(Resource.Id.txtLastname);
			txtUsername = FindViewById<EditText>(Resource.Id.txtUsername);
			txtEmail = FindViewById<EditText>(Resource.Id.txtEmail);
			txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
			txtAge = FindViewById<EditText>(Resource.Id.txtAge);

			invalidFirstname = FindViewById<ImageView>(Resource.Id.invalidFirstname);
			invalidLastname = FindViewById<ImageView>(Resource.Id.invalidLastname);
			invalidUsername = FindViewById<ImageView>(Resource.Id.invalidUsername);
			invalidEmail = FindViewById<ImageView>(Resource.Id.invalidEmail);
			invalidPassword = FindViewById<ImageView>(Resource.Id.invalidPassword);
			invalidAge = FindViewById<ImageView>(Resource.Id.invalidAge);
			invalidTerms = FindViewById<ImageView>(Resource.Id.invalidTerms);
			errorFirstname = FindViewById<LinearLayout>(Resource.Id.errorFirstname);
			errorLastname = FindViewById<LinearLayout>(Resource.Id.errorLastname);
			errorUsername = FindViewById<LinearLayout>(Resource.Id.errorUsername);
			errorEmail = FindViewById<LinearLayout>(Resource.Id.errorEmail);
			errorPassword = FindViewById<LinearLayout>(Resource.Id.errorPassword);

			invalidFirstname.Visibility = Android.Views.ViewStates.Invisible;
			invalidLastname.Visibility = Android.Views.ViewStates.Invisible;
			invalidUsername.Visibility = Android.Views.ViewStates.Invisible;
			invalidEmail.Visibility = Android.Views.ViewStates.Invisible;
			invalidPassword.Visibility = Android.Views.ViewStates.Invisible;
			invalidAge.Visibility = Android.Views.ViewStates.Invisible;
			invalidTerms.Visibility = Android.Views.ViewStates.Invisible;
			errorFirstname.Visibility = Android.Views.ViewStates.Invisible;
			errorLastname.Visibility = Android.Views.ViewStates.Invisible;
			errorUsername.Visibility = Android.Views.ViewStates.Invisible;
			errorEmail.Visibility = Android.Views.ViewStates.Invisible;
			errorPassword.Visibility = Android.Views.ViewStates.Invisible;
		}

		private bool Validate()
		{
			var checkTerms = FindViewById<CheckBox>(Resource.Id.checkTerms);

			invalidFirstname.Visibility = Android.Views.ViewStates.Visible;
			invalidLastname.Visibility = Android.Views.ViewStates.Visible;
			invalidUsername.Visibility = Android.Views.ViewStates.Visible;
			invalidEmail.Visibility = Android.Views.ViewStates.Visible;
			invalidPassword.Visibility = Android.Views.ViewStates.Visible;
			invalidAge.Visibility = Android.Views.ViewStates.Visible;
			invalidTerms.Visibility = Android.Views.ViewStates.Visible;

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

			if (txtFirstname.Text.Length <= 0)
			{
				MarkAsInvalide(invalidFirstname, errorFirstname, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidFirstname, errorFirstname, false);
			}

			if (txtLastname.Text.Length <= 0)
			{
				MarkAsInvalide(invalidLastname, errorLastname, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidLastname, errorLastname, false);
			}

			if (txtUsername.Text.Length <= 0 || txtUsername.Text.Length >= 8)
			{
				MarkAsInvalide(invalidUsername, errorUsername, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidUsername, errorUsername, false);
			}

			int testage = 0;
			int.TryParse(txtAge.Text, out testage);
			if (txtAge.Text.Length < 1 || txtAge.Text.Length > 2 || testage < 8 || testage > 90)
			{
				MarkAsInvalide(invalidAge, null, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidAge, null, false);
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

			if (!checkTerms.Checked)
			{
				MarkAsInvalide(invalidTerms, null, true);
				isValid = false;
			}
			else
			{
				MarkAsInvalide(invalidTerms, null, false);
			}

			return isValid;
		}

        private void ActionSignUp(object sender, EventArgs eventArgs)
        {
            try
            {
				if (Validate())
				{

					string deviceUDID = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

					var result = "";

					ShowLoadingView("Sign Up...");

					result = RegisterUser(txtFirstname.Text, txtLastname.Text, deviceUDID, txtUsername.Text, txtPassword.Text, txtEmail.Text, int.Parse(txtAge.Text));

					if (result == "user added")
						GoToMainPage(deviceUDID);
					else
					{
						HideLoadingView();

						ShowMessageBox(null, result);
					}
				}
                
            }
            catch (Exception err)
            {
				HideLoadingView();
				ShowMessageBox(null, err.Message.ToString());
				return;
            }
        }
		private void ActionTerms(object sender, EventArgs eventArgs)
		{
			var uri = Android.Net.Uri.Parse("http://go-heja.com/nitro/terms.php");
			var intent = new Intent(Intent.ActionView, uri);
			StartActivity(intent);
		}

		private void GoToMainPage(string deviceUDID)
		{
			AppSettings.Email = txtEmail.Text;
			AppSettings.Password = txtPassword.Text;
			AppSettings.Username = txtUsername.Text;
			AppSettings.DeviceUDID = deviceUDID;

			string userID = GetUserID();

			HideLoadingView();

			if (userID == "0")//if the user not registered yet, go to register screen
			{
				ShowMessageBox(null, "You are not registered to Nitro services...");
			}
			else//if the user already registered, go to main screen
			{
				var activity2 = new Intent(this, typeof(SwipeTabActivity));
				StartActivity(activity2);
				Finish();
			}
		}

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Finish();
        }


    }
}