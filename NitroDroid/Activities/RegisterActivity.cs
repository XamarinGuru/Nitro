using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace goheja
{
    [Activity(Label = "listing")]
	public class RegisterActivity : BaseActivity
    {
		bool termsAccepted=false;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.RegisterActivity);

            FindViewById<Button>(Resource.Id.btnGo).Click += btnGo_OnClick;
			FindViewById<Button>(Resource.Id.termsBtn).Click += termsBtn_OnClick;
			FindViewById<Button>(Resource.Id.btnTermsAccepted).Click += btnTermsAccepted_OnClick;
        }

        private void btnGo_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
				var firstName = FindViewById<EditText>(Resource.Id.etFirstName).Text;
                var lastName = FindViewById<EditText>(Resource.Id.etlastName).Text;
                var age = FindViewById<EditText>(Resource.Id.age).Text;    //added by Afroz on 27/07/2016
                var userName = FindViewById<EditText>(Resource.Id.etUserName).Text;
                var psw = FindViewById<EditText>(Resource.Id.etPsw).Text;
				var email = FindViewById<EditText>(Resource.Id.etMail).Text;

                string device = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
				string validateMessage= validate(firstName,lastName, age, userName, psw, email);

				if (validateMessage != "")
				{
					Toast.MakeText(this, validateMessage, ToastLength.Long).Show();
					return;
				}
				if (termsAccepted != true)
				{
					Toast.MakeText(this, "Terms Were not accepted", ToastLength.Long).Show();
					return;
				}

				var result = RegisterUser(firstName, lastName, device, userName, psw, email, int.Parse(age));

				if (result == "user added")
					GoToMainPage(email, psw);
				else
					ShowMessageBox(null, result);
                
            }
            catch
            {
                ShowMessageBox("Nitro service is not available", "Oops!Service not available... Pls try again later", true);
				return;
            }
        }

		private void GoToMainPage(string email, string password)
		{
			AppSettings.Email = email;
			AppSettings.Password = password;

			string userID = GetUserID();

			if (userID == "0")//if the user not registered yet, go to register screen
			{
				ShowMessageBox(null, "You are not registered to Nitro services...");
			}
			else//if the user already registered, go to main screen
			{
				var activity2 = new Intent(this, typeof(SwipeTabActivity));
				StartActivity(activity2);
				OnDestroy();
			}
		}
		private void termsBtn_OnClick(object sender, EventArgs eventArgs)
		{
			var uri = Android.Net.Uri.Parse ("http://go-heja.com/nitro/terms.php");
			var intent = new Intent (Intent.ActionView, uri);
			StartActivity (intent);
			termsAccepted = true;
		}
		private void btnTermsAccepted_OnClick(object sender, EventArgs eventArgs)
		{
			if (termsAccepted == false) {
				Toast.MakeText(this, "Oh,oh , you didnt read the terms!", ToastLength.Long).Show();
			} else {
				Button b = sender as Button;
				b.Text = "terms accepted, thanks";
			}
		}
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Finish();
        }
		private string validate (string fName,string lName, string ag,string uName,string pw,string em)
		{
			if (fName == "") 							return "Please enter first name";
			if (ag == "") 								return "Please enter age";
			if (!ValidateUserNickName(uName)) 			return "Nick name is taken, please try another";
			if (em == "") 								return "Please enter email";
			if (!em.Contains("@")||!em.Contains(".")) 	return "Email is not valid";
			if (pw == "") 								return "Please enter password";
			if (uName=="") 								return "Please enter user name";
			if (uName.Contains (" ")) 					return "Please do not use space in nick name";

            return "";
		}
    }
}