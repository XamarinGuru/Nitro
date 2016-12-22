﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Net;
using Android.Support.V4.App;
using Android.Text;
using System.Threading.Tasks;

namespace goheja
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]

    public class SplashActivity : BaseActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(() =>
            {
                Task.Delay(500);  
            });

            startupWork.ContinueWith(t =>
            {
                initiatAth();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }

        public void initiatAth()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NotificationManager notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(1, CreateNotification());

            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            bool isOnline = (activeConnection != null) && activeConnection.IsConnected;

            if (!isOnline)
            {
				ShowMessageBox("No internet connection", "Oops!No internet connection... Pls try again later", true);
                return;
            }

            trackSvc.Service1 test = new trackSvc.Service1();
            string deviceId = "0";
            try
            {
                deviceId = test.getListedDeviceId(Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId));
            }
            catch
            {
				ShowMessageBox("Nitro service is not available", "Oops!Service not available... Pls try again later", true);
                return;
            }

            if (deviceId == "0")//not registered yet
            {
                var activity2 = new Intent(this, typeof(RegisterActivity));
                activity2.PutExtra("MyData", "Data from Activity1");
                StartActivity(activity2);
            }
            else//already registered
            {
                StartActivity(new Intent(this, typeof(SwipeTabActivity)));
            }
        }

        public Notification CreateNotification()
        {
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
			
            var clossIntent = new Intent(this, typeof(CloseApplicationActivity));
            clossIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.ClearTop);
            var dismissIntent = PendingIntent.GetActivity(this, 0, clossIntent, PendingIntentFlags.CancelCurrent);
            var action = new NotificationCompat.Action(Resource.Drawable.switch_off, "Switch off", dismissIntent);
            builder.AddAction(action);

            var n = builder.Build();
            n.Flags |= NotificationFlags.NoClear;
            return n;
        }
    }
}
