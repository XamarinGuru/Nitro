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

    //by Afroz date 22/07/2016
    //[Activity(Label = "Nitro", MainLauncher = false, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleInstance)]
    //public class NewMainActivity : Activity
    //{
    //    ConnectivityManager connectivityManager;
    //    private NotificationManager notificationManager;
    //    NetworkInfo activeConnection;
    //    bool isOnline;
    //    ImageView hejaIcon,
    //        watchIcon,
    //        calenderIcon,
    //        drawerHandle,
    //        Icon01,
    //        Icon02,
    //        Icon03,
    //        Icon04,
    //        Icon05,
    //        Icon06,
    //        Icon07,
    //        watchDevider;

    //    LinearLayout watchLayout;

    //    bool IsWatchSynced = false;

    //    protected override void OnCreate(Bundle bundle)
    //    {
    //        RequestWindowFeature(WindowFeatures.NoTitle);
    //        base.OnCreate(bundle);

    //        SetContentView(Resource.Layout.MainActivity);
    //        initiatAth();
    //        InitializeComponent();
    //    }

    //    private void InitializeComponent()
    //    {
    //        var drawerContent = FindViewById<LinearLayout>(Resource.Id.drawerContent);
    //        drawerHandle = FindViewById<ImageView>(Resource.Id.drawerHandle);
    //        drawerContent.Visibility = ViewStates.Gone;
    //        drawerHandle.Click += delegate (object sender, EventArgs e)
    //        {
    //            if (drawerContent.Visibility == ViewStates.Visible)
    //            {
    //                drawerHandle.SetImageResource(Resource.Drawable.menu_icon);
    //                drawerContent.Visibility = ViewStates.Gone;
    //            }
    //            else
    //            {
    //                drawerHandle.SetImageResource(Resource.Drawable.menu_icon_01);
    //                drawerContent.Visibility = ViewStates.Visible;
    //            }
    //        };

    //        var whatItLabel = FindViewById<TextView>(Resource.Id.whatItLabel);
    //        whatItLabel.SetTypeface(Typeface.CreateFromAsset(Assets,
    //        "font/arialbd.ttf"), TypefaceStyle.Bold);

    //        Icon01 = FindViewById<ImageView>(Resource.Id.icon01);
    //        Icon02 = FindViewById<ImageView>(Resource.Id.icon02);
    //        Icon03 = FindViewById<ImageView>(Resource.Id.icon03);
    //        Icon04 = FindViewById<ImageView>(Resource.Id.icon04);
    //        Icon05 = FindViewById<ImageView>(Resource.Id.icon05);
    //        Icon06 = FindViewById<ImageView>(Resource.Id.icon06);
    //        Icon07 = FindViewById<ImageView>(Resource.Id.icon07);
    //        Icon01.Click += delegate
    //        {
    //            Icon_click(1);
    //        };

    //        Icon02.Click += delegate
    //        {
    //            Icon_click(2);
    //        };

    //        hejaIcon = FindViewById<ImageView>(Resource.Id.hejaIcon);
    //        watchIcon = FindViewById<ImageView>(Resource.Id.watchIcon);
    //        //watchLayout = FindViewById<LinearLayout>(Resource.Id.watchLayout);
    //        watchDevider = FindViewById<ImageView>(Resource.Id.watchDevider);
    //        calenderIcon = FindViewById<ImageView>(Resource.Id.calenderIcon);
    //        calenderIcon.Click += meCalendar_OnClick;
    //        watchIcon.Click += selectDevice_OnClick;
    //        hejaIcon.Click += personalData_OnClick;
    //        notificationManager.Notify(1, CreateNotification());
    //        setBitmapImg();

    //        if (IsWatchSynced)
    //        {
    //            watchIcon.Visibility = ViewStates.Gone;
    //            watchDevider.Visibility = ViewStates.Gone;
    //        }
    //    }

    //    public void initiatAth()
    //    {
    //        connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
    //        notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
    //        activeConnection = connectivityManager.ActiveNetworkInfo;
    //        isOnline = (activeConnection != null) && activeConnection.IsConnected;

    //        if (!isOnline)
    //        {
    //            //Toast.MakeText(this, "No internet connection!", ToastLength.Long).Show();
    //            var builder = new AlertDialog.Builder(this);
    //            builder.SetTitle("No internet connection");
    //            builder.SetMessage("Oops!No internet connection... Pls try again later");
    //            builder.SetCancelable(false);
    //            builder.SetPositiveButton("OK", delegate { Finish(); });
    //            builder.Show();
    //            return;
    //        }
    //        trackSvc.Service1 test = new trackSvc.Service1();
    //        string deviceId = "0";
    //        try
    //        {
    //            deviceId = test.getListedDeviceId(Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId));
    //        }
    //        catch (Exception err)
    //        {
    //            var builder = new AlertDialog.Builder(this);
    //            builder.SetTitle("Nitro service is not available");
    //            builder.SetMessage("Oops!Service not available... Pls try again later");
    //            builder.SetCancelable(false);
    //            builder.SetPositiveButton("OK", delegate { Finish(); });
    //            builder.Show();
    //            return;
    //        }


    //        if (deviceId == "0")
    //        {
    //            var activity2 = new Intent(this, typeof(listingActivity));
    //            activity2.PutExtra("MyData", "Data from Activity1");
    //            StartActivity(activity2);
    //        }
    //        else
    //        {

    //        }

    //    }

    //    private void meCalendar_OnClick(object sender, EventArgs e)
    //    {
    //        //var activity2go = new Intent(this, typeof(calen));
    //        //activity2go.PutExtra("MyData", "Data from Activity1");
    //        //StartActivity(activity2go);


    //        var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
    //        string nickName = contextPref.GetString("storedUserName", "");
    //        var uri = Android.Net.Uri.Parse("http://go-heja.com/nitro/mobda.php?userNickName=" + nickName + "&userId=" + contextPref.GetString("storedAthId", ""));
    //        var intent = new Intent(Intent.ActionView, uri);
    //        StartActivity(intent);
    //        drawerHandle.CallOnClick();
    //    }

    //    private void selectDevice_OnClick(object sender, EventArgs e)
    //    {
    //        var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
    //        string nickName = contextPref.GetString("storedUserName", "");
    //        var uri = Android.Net.Uri.Parse("http://go-heja.com/gh/mob/sync.php?userId=" + contextPref.GetString("storedAthId", "0").ToString() + "&mog=nitro&url=uurrll");
    //        var intent = new Intent(Intent.ActionView, uri);
    //        StartActivity(intent);
    //        drawerHandle.CallOnClick();
    //    }

    //    private void personalData_OnClick(object sender, EventArgs eventArgs)
    //    {
    //        var activity2go = new Intent(this, typeof(personalData));
    //        activity2go.PutExtra("MyData", "Data from Activity1");
    //        StartActivity(activity2go);
    //        drawerHandle.CallOnClick();
    //    }

    //    private void Icon_click(int iconNumber)
    //    {
    //        var intent = new Intent(this, typeof(MainActivity));
    //        intent.PutExtra("EventNumber", iconNumber);
    //        StartActivity(intent);
    //    }

    //    protected override void OnDestroy()
    //    {

    //        // Toast.MakeText(this, "test", ToastLength.Long).Show();
    //        base.OnDestroy();
    //        DateTime destroytTime = DateTime.Now;
    //        // _locationManager.RemoveUpdates(this);
    //        notificationManager.CancelAll();


    //        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
    //        //if (isStarted) 
    //        //{
    //        //	svc.updateMomgoData (athFirstName + " " + athLastName, String.Format ("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude), destroytTime, true, android_id, 0f, true, athId, athCountry, prefs.GetFloat ("dist", 0f), true, gainAlt, true, _currentLocation.Bearing, true, 2, true, type);
    //        //}
    //        Finish();


    //    }

    //    public Notification CreateNotification()
    //    {


    //        var contentIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(NewMainActivity)), PendingIntentFlags.UpdateCurrent);
    //        var builder = new Notification.Builder(this)
    //            .SetContentTitle("Nitro on the go")
    //            .SetSmallIcon(Resource.Drawable.icon)
    //            .SetPriority(1)
    //           .SetContentIntent(contentIntent)
    //            .SetCategory("tst")
    //            .SetStyle(new Notification.BigTextStyle()
    //       .BigText(Html.FromHtml("Nitro is now tarcking practice<br> Running. Tap to Open")))
    //            .SetContentText(Html.FromHtml("Nitro is now tarcking practice \n Running. Tap to Open"))
    //            ;

    //        //by Afroz date 16/08/2016
    //        var clossIntent = new Intent(this, typeof(CloseApplication));
    //        clossIntent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop);
    //        var dismissIntent = PendingIntent.GetActivity(this, 0, clossIntent, PendingIntentFlags.CancelCurrent);
    //        //end of update by afroz
    //        var action = new Notification.Action(Resource.Drawable.switch_off, "Switch off", dismissIntent);
    //        builder.AddAction(action);

    //        Notification n = builder.Build();
    //        n.Flags |= NotificationFlags.NoClear;
    //        return n;
    //    }

    //    void setBitmapImg()
    //    {


    //        try
    //        {
    //            var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
    //            var filePath = System.IO.Path.Combine(sdCardPath, "data/goheja.gohejanitro/files/me.png");
    //            var s2 = new FileStream(filePath, FileMode.Open);
    //            Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
    //            hejaIcon.SetImageBitmap(bitmap2);
    //            s2.Close();

    //        }
    //        catch (Exception err)
    //        {

    //        }
    //        finally
    //        {
    //            //s2.Close();

    //        }



    //    }

    //    protected override void OnResume()
    //    {
    //        base.OnResume();
    //        setBitmapImg();
    //        //  _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
    //    }

    //   
    //    public override void OnBackPressed()
    //    {
    //       MoveTaskToBack(true);
    //    }
    //    
    //}

    //by Afroz date 1/9/2016
    //[Activity(Label = "Nitro", MainLauncher = false, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleInstance)]
    public class NewMainActivity : Android.Support.V4.App.Fragment
    {
        //ConnectivityManager connectivityManager;
        //private NotificationManager notificationManager;
        //NetworkInfo activeConnection;
        //bool isOnline;
        //ImageView hejaIcon,
        //    watchIcon,
        //    calenderIcon,
        //    drawerHandle,
        ImageView Icon01,
            Icon02,
            Icon03,
            Icon04,
            Icon05,
            Icon06,
            Icon07;
        //watchDevider;

        LinearLayout watchLayout;

        bool IsWatchSynced = false;

        //protected override void OnCreate(Bundle bundle)
        //{
        //    RequestWindowFeature(WindowFeatures.NoTitle);
        //    base.OnCreate(bundle);

        //    SetContentView(Resource.Layout.MainActivity);
        //    initiatAth();
        //    InitializeComponent();
        //}

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.MainActivity, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            //initiatAth();
            InitializeComponent(view);
        }

        private void InitializeComponent(View view)
        {
            //var drawerContent = view.FindViewById<LinearLayout>(Resource.Id.drawerContent);
            //drawerHandle = view.FindViewById<ImageView>(Resource.Id.drawerHandle);
            //drawerContent.Visibility = ViewStates.Gone;
            //drawerHandle.Click += delegate (object sender, EventArgs e)
            //{
            //    if (drawerContent.Visibility == ViewStates.Visible)
            //    {
            //        drawerHandle.SetImageResource(Resource.Drawable.menu_icon);
            //        drawerContent.Visibility = ViewStates.Gone;
            //    }
            //    else
            //    {
            //        drawerHandle.SetImageResource(Resource.Drawable.menu_icon_01);
            //        drawerContent.Visibility = ViewStates.Visible;
            //    }
            //};

            var whatItLabel = view.FindViewById<TextView>(Resource.Id.whatItLabel);
            whatItLabel.SetTypeface(Typeface.CreateFromAsset(Activity.Assets,
            "font/arialbd.ttf"), TypefaceStyle.Bold);

            Icon01 = view.FindViewById<ImageView>(Resource.Id.icon01);
            Icon02 = view.FindViewById<ImageView>(Resource.Id.icon02);
            Icon03 = view.FindViewById<ImageView>(Resource.Id.icon03);
            Icon04 = view.FindViewById<ImageView>(Resource.Id.icon04);
            Icon05 = view.FindViewById<ImageView>(Resource.Id.icon05);
            Icon06 = view.FindViewById<ImageView>(Resource.Id.icon06);
            Icon07 = view.FindViewById<ImageView>(Resource.Id.icon07);
            Icon01.Click += delegate
            {
                Icon_click(1);
            };

            Icon02.Click += delegate
            {
                Icon_click(2);
            };

            //hejaIcon = view.FindViewById<ImageView>(Resource.Id.hejaIcon);
            //watchIcon = view.FindViewById<ImageView>(Resource.Id.watchIcon);
            //watchLayout = FindViewById<LinearLayout>(Resource.Id.watchLayout);
            //watchDevider = view.FindViewById<ImageView>(Resource.Id.watchDevider);
            //calenderIcon = view.FindViewById<ImageView>(Resource.Id.calenderIcon);
            //calenderIcon.Click += meCalendar_OnClick;
            //watchIcon.Click += selectDevice_OnClick;
            //hejaIcon.Click += personalData_OnClick;
            //notificationManager.Notify(1, CreateNotification());
            //setBitmapImg();

            //if (IsWatchSynced)
            //{
            //    watchIcon.Visibility = ViewStates.Gone;
            //    watchDevider.Visibility = ViewStates.Gone;
            //}
        }

        //public void initiatAth()
        //{
        //    connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
        //    notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
        //    activeConnection = connectivityManager.ActiveNetworkInfo;
        //    isOnline = (activeConnection != null) && activeConnection.IsConnected;

        //    if (!isOnline)
        //    {
        //        //Toast.MakeText(this, "No internet connection!", ToastLength.Long).Show();
        //        var builder = new AlertDialog.Builder(this);
        //        builder.SetTitle("No internet connection");
        //        builder.SetMessage("Oops!No internet connection... Pls try again later");
        //        builder.SetCancelable(false);
        //        builder.SetPositiveButton("OK", delegate { Finish(); });
        //        builder.Show();
        //        return;
        //    }
        //    trackSvc.Service1 test = new trackSvc.Service1();
        //    string deviceId = "0";
        //    try
        //    {
        //        deviceId = test.getListedDeviceId(Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId));
        //    }
        //    catch (Exception err)
        //    {
        //        var builder = new AlertDialog.Builder(this);
        //        builder.SetTitle("Nitro service is not available");
        //        builder.SetMessage("Oops!Service not available... Pls try again later");
        //        builder.SetCancelable(false);
        //        builder.SetPositiveButton("OK", delegate { Finish(); });
        //        builder.Show();
        //        return;
        //    }


        //    if (deviceId == "0")
        //    {
        //        var activity2 = new Intent(this, typeof(listingActivity));
        //        activity2.PutExtra("MyData", "Data from Activity1");
        //        StartActivity(activity2);
        //    }
        //    else
        //    {

        //    }

        //}

        //private void meCalendar_OnClick(object sender, EventArgs e)
        //{
        //    //var activity2go = new Intent(this, typeof(calen));
        //    //activity2go.PutExtra("MyData", "Data from Activity1");
        //    //StartActivity(activity2go);


        //    var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
        //    string nickName = contextPref.GetString("storedUserName", "");
        //    var uri = Android.Net.Uri.Parse("http://go-heja.com/nitro/mobda.php?userNickName=" + nickName + "&userId=" + contextPref.GetString("storedAthId", ""));
        //    var intent = new Intent(Intent.ActionView, uri);
        //    StartActivity(intent);
        //    drawerHandle.CallOnClick();
        //}

        //private void selectDevice_OnClick(object sender, EventArgs e)
        //{
        //    var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
        //    string nickName = contextPref.GetString("storedUserName", "");
        //    var uri = Android.Net.Uri.Parse("http://go-heja.com/gh/mob/sync.php?userId=" + contextPref.GetString("storedAthId", "0").ToString() + "&mog=nitro&url=uurrll");
        //    var intent = new Intent(Intent.ActionView, uri);
        //    StartActivity(intent);
        //    drawerHandle.CallOnClick();
        //}

        //private void personalData_OnClick(object sender, EventArgs eventArgs)
        //{
        //    var activity2go = new Intent(this, typeof(personalData));
        //    activity2go.PutExtra("MyData", "Data from Activity1");
        //    StartActivity(activity2go);
        //    drawerHandle.CallOnClick();
        //}

        private void Icon_click(int iconNumber)
        {
            var intent = new Intent(Activity, typeof(MainActivity));
            intent.PutExtra("EventNumber", iconNumber);
            StartActivity(intent);
        }

        //protected override void OnDestroy()
        //{

        //    // Toast.MakeText(this, "test", ToastLength.Long).Show();
        //    base.OnDestroy();
        //    DateTime destroytTime = DateTime.Now;
        //    // _locationManager.RemoveUpdates(this);
        //    notificationManager.CancelAll();


        //    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        //    //if (isStarted) 
        //    //{
        //    //	svc.updateMomgoData (athFirstName + " " + athLastName, String.Format ("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude), destroytTime, true, android_id, 0f, true, athId, athCountry, prefs.GetFloat ("dist", 0f), true, gainAlt, true, _currentLocation.Bearing, true, 2, true, type);
        //    //}
        //    Finish();


        //}

        //public Notification CreateNotification()
        //{


        //    var contentIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(NewMainActivity)), PendingIntentFlags.UpdateCurrent);
        //    var builder = new Notification.Builder(this)
        //        .SetContentTitle("Nitro on the go")
        //        .SetSmallIcon(Resource.Drawable.icon)
        //        .SetPriority(1)
        //       .SetContentIntent(contentIntent)
        //        .SetCategory("tst")
        //        .SetStyle(new Notification.BigTextStyle()
        //   .BigText(Html.FromHtml("Nitro is now tarcking practice<br> Running. Tap to Open")))
        //        .SetContentText(Html.FromHtml("Nitro is now tarcking practice \n Running. Tap to Open"))
        //        ;

        //    //by Afroz date 16/08/2016
        //    var clossIntent = new Intent(this, typeof(CloseApplication));
        //    clossIntent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop);
        //    var dismissIntent = PendingIntent.GetActivity(this, 0, clossIntent, PendingIntentFlags.CancelCurrent);
        //    //end of update by afroz
        //    var action = new Notification.Action(Resource.Drawable.switch_off, "Switch off", dismissIntent);
        //    builder.AddAction(action);

        //    Notification n = builder.Build();
        //    n.Flags |= NotificationFlags.NoClear;
        //    return n;
        //}

        //void setBitmapImg()
        //{


        //    try
        //    {
        //        var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
        //        var filePath = System.IO.Path.Combine(sdCardPath, "data/goheja.gohejanitro/files/me.png");
        //        var s2 = new FileStream(filePath, FileMode.Open);
        //        Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
        //        hejaIcon.SetImageBitmap(bitmap2);
        //        s2.Close();

        //    }
        //    catch (Exception err)
        //    {

        //    }
        //    finally
        //    {
        //        //s2.Close();

        //    }



        //}

        //protected override void OnResume()
        //{
        //    base.OnResume();
        //    setBitmapImg();
        //    //  _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
        //}

        
        //public override void OnBackPressed()
        //{
        //    MoveTaskToBack(true);
        //}
        
    }

    //end by Afroz date 1/9/2016
}

//end of update by afroz
