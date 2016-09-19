
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
using System.Threading.Tasks;
using Android.Net;
using Android.Support.V4.App;
using Android.Text;

namespace goheja
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        //by Afroz date 1/9/2016
        ConnectivityManager connectivityManager;
        private NotificationManager notificationManager;
        NetworkInfo activeConnection;
        bool isOnline;

        //end by Afroz date 1/9/2016
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            //Log.Debug(TAG, "SplashActivity.OnCreate");
        }

        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(() =>
            {
                //Log.Debug(TAG, "Performing some startup work that takes a bit of time.");
                Task.Delay(500);  // Simulate a bit of startup work.
                                  //Log.Debug(TAG, "Working in the background - important stuff.");
            });

            startupWork.ContinueWith(t =>
            {
                //Log.Debug(TAG, "Work is finished - start Activity1.");

                //by Afroz date 1/9/2016
                //StartActivity(new Intent(Application.Context, typeof(NewMainActivity)));
                //StartActivity(new Intent(Application.Context, typeof(SwipeTabActivity)));
                initiatAth();
                //end by Afroz date 1/9/2016
            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }

        //by Afroz date 1/9/2016
        public void initiatAth()
        {
            connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(1, CreateNotification());
            activeConnection = connectivityManager.ActiveNetworkInfo;
            isOnline = (activeConnection != null) && activeConnection.IsConnected;

            if (!isOnline)
            {
                //Toast.MakeText(this, "No internet connection!", ToastLength.Long).Show();
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("No internet connection");
                builder.SetMessage("Oops!No internet connection... Pls try again later");
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate { Finish(); });
                builder.Show();
                return;
            }
            trackSvc.Service1 test = new trackSvc.Service1();
            string deviceId = "0";
            try
            {
                deviceId = test.getListedDeviceId(Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId));
            }
            catch (Exception err)
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Nitro service is not available");
                builder.SetMessage("Oops!Service not available... Pls try again later");
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate { Finish(); });
                builder.Show();
                return;
            }


            if (deviceId == "0")
            {
                var activity2 = new Intent(this, typeof(listingActivity));
                activity2.PutExtra("MyData", "Data from Activity1");
                StartActivity(activity2);
            }
            else
            {
                StartActivity(new Intent(this, typeof(SwipeTabActivity)));
            }

        }

        public Notification CreateNotification()
        {

            //by Afroz date 2/9/2016
            //var contentIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(NewMainActivity)), PendingIntentFlags.UpdateCurrent);

          //  var builder = new Notification.Builder(this)
          //     .SetContentTitle("Nitro on the go")
          //     .SetSmallIcon(Resource.Drawable.icon)
          //     .SetPriority(1)
          //    .SetContentIntent(contentIntent)
          //     .SetCategory("tst")
          //     .SetStyle(new Notification.BigTextStyle()
          //.BigText(Html.FromHtml("Nitro is now tarcking practice<br> Running. Tap to Open")))
          //     .SetContentText(Html.FromHtml("Nitro is now tarcking practice \n Running. Tap to Open"));

            var contentIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(SwipeTabActivity)), PendingIntentFlags.UpdateCurrent);
            var builder = new NotificationCompat.Builder(this)
               .SetContentTitle("Nitro on the go")
               .SetSmallIcon(Resource.Drawable.icon)
               .SetPriority(1)
              .SetContentIntent(contentIntent)
               .SetCategory("tst")
               .SetStyle(new NotificationCompat.BigTextStyle()
          .BigText(Html.FromHtml("Nitro is now tarcking practice<br> Running. Tap to Open")))
               .SetContentText(Html.FromHtml("Nitro is now tarcking practice \n Running. Tap to Open"));


            //end by Afroz date 2/9/2016

            //by Afroz date 16/08/2016


            var clossIntent = new Intent(this, typeof(CloseApplication));
            clossIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.ClearTop);
            var dismissIntent = PendingIntent.GetActivity(this, 0, clossIntent, PendingIntentFlags.CancelCurrent);
            //end of update by afroz
            //by Afroz date 2/9/2016
            //var action = new Notification.Action(Resource.Drawable.switch_off, "Switch off", dismissIntent);
            var action = new NotificationCompat.Action(Resource.Drawable.switch_off, "Switch off", dismissIntent);
            //end by Afroz date 2/9/2016
            builder.AddAction(action);

            var n = builder.Build();
            n.Flags |= NotificationFlags.NoClear;
            return n;
        }
        //end by Afroz date 1/9/2016
    }
}

