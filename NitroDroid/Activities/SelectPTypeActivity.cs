﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace goheja
{
	[Activity(Label = "SelectPTypeActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class SelectPTypeActivity : BaseActivity
	{
		Color COLOR_ACTIVE = Color.Rgb(229, 161, 9);
		Color COLOR_DISABLE = Color.Rgb(67, 67, 67);

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.SelectPTypeActivity);

			InitUISettings();
		}

		void InitUISettings()
		{
			FindViewById<LinearLayout>(Resource.Id.stateCycling).SetBackgroundColor(COLOR_DISABLE);
			FindViewById<LinearLayout>(Resource.Id.stateRunning).SetBackgroundColor(COLOR_DISABLE);
			FindViewById<LinearLayout>(Resource.Id.stateSwimming).SetBackgroundColor(COLOR_DISABLE);
			FindViewById<LinearLayout>(Resource.Id.stateTriathlon).SetBackgroundColor(COLOR_DISABLE);
			FindViewById<LinearLayout>(Resource.Id.stateOther).SetBackgroundColor(COLOR_DISABLE);

			switch (AppSettings.selectedEvent.type)
			{
				case "1":
					FindViewById<LinearLayout>(Resource.Id.stateCycling).SetBackgroundColor(COLOR_ACTIVE);
					break;
				case "2":
					FindViewById<LinearLayout>(Resource.Id.stateRunning).SetBackgroundColor(COLOR_ACTIVE);
					break;
				case "3":
					FindViewById<LinearLayout>(Resource.Id.stateSwimming).SetBackgroundColor(COLOR_ACTIVE);
					break;
				case "4":
					FindViewById<LinearLayout>(Resource.Id.stateTriathlon).SetBackgroundColor(COLOR_ACTIVE);
					break;
				case "5":
					FindViewById<LinearLayout>(Resource.Id.stateOther).SetBackgroundColor(COLOR_ACTIVE);
					break;
			}

			FindViewById(Resource.Id.ActionCycling).Click += ActionSelectedType;
			FindViewById(Resource.Id.ActionRunning).Click += ActionSelectedType;
			FindViewById(Resource.Id.ActionSwimming).Click += ActionSelectedType;
			FindViewById(Resource.Id.ActionTriathlon).Click += ActionSelectedType;
			FindViewById(Resource.Id.ActionOther).Click += ActionSelectedType;
		}

		void ActionSelectedType(object sender, EventArgs e)
		{
			AppSettings.selectedEvent.type = (sender as RelativeLayout).Tag.ToString();
			var activity = new Intent(this, typeof(AdjustTrainningActivity));
			StartActivity(activity);
			Finish();
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				var activity = new Intent(this, typeof(AdjustTrainningActivity));
				StartActivity(activity);
				Finish();
			}

			return base.OnKeyDown(keyCode, e);
		}
	}
}
