
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
	[Activity(Label = "InitActivity")]
	public class InitActivity : BaseActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.InitActivity);

			FindViewById<TextView>(Resource.Id.ActionSignIn).Click += ActionSignIn;
			FindViewById<TextView>(Resource.Id.ActionSignUp).Click += ActionSignUp;
		}

		void ActionSignIn(object sender, EventArgs e)
		{
			var activity = new Intent(this, typeof(LoginActivity));
			StartActivity(activity);
		}

		void ActionSignUp(object sender, EventArgs e)
		{
			var activity = new Intent(this, typeof(RegisterActivity));
			StartActivity(activity);
		}
	}
}
