using Android.App;
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

			string userID = GetUserID();

            if (userID == "0")//not registered yet
            {
                //var activity2 = new Intent(this, typeof(RegisterActivity));
				var activity2 = new Intent(this, typeof(InitActivity));
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
               	.SetSmallIcon(Resource.Drawable.icon_notification)
               	.SetPriority(1)
              	.SetContentIntent(contentIntent)
               	.SetCategory("tst")
              	.SetStyle(new NotificationCompat.BigTextStyle()
          		.BigText(Html.FromHtml("Tap to Open")))
               	.SetContentText(Html.FromHtml("Tap to Open"));
			
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

