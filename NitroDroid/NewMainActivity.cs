using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System.Collections.Generic;
using System.Linq;
using Android.Preferences;
using Android.Content.PM;
using Android.Webkit;
using Android.Net;
using goheja.Services;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;

namespace goheja
{
    //by Afroz date 1/9/2016
    //[Activity(Label = "Nitro", MainLauncher = false, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleInstance)]
    public class NewMainActivity : Android.Support.V4.App.Fragment
    {
		ImageView Icon01,
		Icon02;
            //Icon03,
            //Icon04,
            //Icon05,
            //Icon06,
            //Icon07;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.MainActivity, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            InitializeComponent(view);
        }

        private void InitializeComponent(View view)
        {
            var whatItLabel = view.FindViewById<TextView>(Resource.Id.whatItLabel);
            whatItLabel.SetTypeface(Typeface.CreateFromAsset(Activity.Assets,
            "font/arialbd.ttf"), TypefaceStyle.Bold);

            Icon01 = view.FindViewById<ImageView>(Resource.Id.icon01);
            Icon02 = view.FindViewById<ImageView>(Resource.Id.icon02);
            //Icon03 = view.FindViewById<ImageView>(Resource.Id.icon03);
            //Icon04 = view.FindViewById<ImageView>(Resource.Id.icon04);
            //Icon05 = view.FindViewById<ImageView>(Resource.Id.icon05);
            //Icon06 = view.FindViewById<ImageView>(Resource.Id.icon06);
            //Icon07 = view.FindViewById<ImageView>(Resource.Id.icon07);
            Icon01.Click += delegate
            {
                Icon_click(1);
            };

            Icon02.Click += delegate
            {
                Icon_click(2);
            };
        }
        private void Icon_click(int iconNumber)
        {
            var intent = new Intent(Activity, typeof(MainActivity));
            intent.PutExtra("EventNumber", iconNumber);
            StartActivity(intent);
        }
    }

    //end by Afroz date 1/9/2016
}

//end of update by afroz
