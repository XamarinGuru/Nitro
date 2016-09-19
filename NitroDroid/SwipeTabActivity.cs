using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using System;
using System.IO;
using Android.Locations;
using Android.Net;
using goheja.Services;

//by Afroz date 31/8/2016
namespace goheja
{
    [Activity(Label = "Nitro" , MainLauncher = true,Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleInstance)]
    public class SwipeTabActivity : FragmentActivity
    {
		ISharedPreferences contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
        ConnectivityManager connectivityManager;
        NetworkInfo activeConnection;
        bool isOnline;
        private NotificationManager _notificationManager;
        private GenericFragmentPagerAdaptor _adaptor;

        //private ImageView hejaIcon;
        private ImageView _tab1Icon,
            _tab2Icon,
            _tab3Icon,
            _tabStrip1,
            _tabStrip2,
            _tabStrip3,
            _actionBarRight,
            _actionBarLeft;

        private ViewPager _pager;
        private TextView _titleBarText;
        private string _middleTabTitle = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
			string _deviceId = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SwipeTabActivity);
			var contextEdit = contextPref.Edit();
			contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			try
			{
				trackSvc.Service1 test = new trackSvc.Service1();
				string[] athData = test.getAthDataByDeviceId(_deviceId);
				contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
				contextEdit = contextPref.Edit();

				contextEdit.PutString("storedFirstName", athData[0].ToString());
				contextEdit.PutString("storedAthId", athData[2].ToString());
				contextEdit.PutString("storedLastName", athData[1].ToString());
				contextEdit.PutString("storedCountry", athData[3].ToString());
				contextEdit.PutString("storedUserName", athData[4].ToString());
				contextEdit.PutString("storedPsw", athData[5].ToString());
				contextEdit.Commit();


			}
			catch (Exception err)
			{
				string x = err.ToString();

			}




            InitilizeComponant();
           
            startLocationService();
            initiatAth();
        }

        private void InitilizeComponant()
        {
            _notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            _titleBarText = FindViewById<TextView>(Resource.Id.TitleBarTextMain);
            _tab1Icon = FindViewById<ImageView>(Resource.Id.Tab1Icon);
            _tab2Icon = FindViewById<ImageView>(Resource.Id.Tab2Icon);
            _tab3Icon = FindViewById<ImageView>(Resource.Id.Tab3Icon);
            _tabStrip1 = FindViewById<ImageView>(Resource.Id.TabStrip1);
            _tabStrip2 = FindViewById<ImageView>(Resource.Id.TabStrip2);
            _tabStrip3 = FindViewById<ImageView>(Resource.Id.TabStrip3);
            _actionBarRight = FindViewById<ImageView>(Resource.Id.ActionBarRight);
            _actionBarLeft = FindViewById<ImageView>(Resource.Id.ActionBarLeft);
            _pager = FindViewById<ViewPager>(Resource.Id.pager);
            _adaptor = new GenericFragmentPagerAdaptor(SupportFragmentManager, this);
            _pager.Adapter = _adaptor;
            _pager.PageSelected += PagerOnPageSelected;
            _actionBarRight.Click += ActionBarRightOnClick;
            _tab1Icon.Click += (sender, args) =>
            {
                SetPage(0);
            };

            _tab2Icon.Click += (sender, args) =>
            {
                SetPage(1);

            };

            _tab3Icon.Click += (sender, args) =>
            {
                SetPage(2);

            };
            Tab1Click();
        }

        private void ActionBarRightOnClick(object sender, EventArgs eventArgs)
        {
            if (_pager.CurrentItem == 0)
            {
            }
            else if (_pager.CurrentItem == 2)
            {
                var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
                string nickName = contextPref.GetString("storedUserName", "");
                var uri =
                    Android.Net.Uri.Parse("http://go-heja.com/gh/mob/sync.php?userId=" +
                                          contextPref.GetString("storedAthId", "0").ToString() + "&mog=nitro&url=uurrll");
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            }
        }

        private void SetPage(int position)
        {
            _pager.SetCurrentItem(position, true);
        }

        private void PagerOnPageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            SetSelect(e.Position);
        }

        private void SetSelect(int position)
        {
            _tab1Icon.SetImageResource(position == 0 ? Resource.Drawable.calender_icon_active_new : Resource.Drawable.calender_icon_inactive_new);
            _tabStrip1.Visibility = position == 0 ? ViewStates.Visible : ViewStates.Gone;
            //_tab2Icon.SetImageResource(position == 0 ? Resource.Drawable.calender_icon_active_new : Resource.Drawable.calender_icon_inactive_new);
            _tabStrip2.Visibility = position == 1 ? ViewStates.Visible : ViewStates.Gone;
            _tab3Icon.SetImageResource(position == 2 ? Resource.Drawable.user_active_new : Resource.Drawable.user_inactive_new);
            _tabStrip3.Visibility = position == 2 ? ViewStates.Visible : ViewStates.Gone;
            _actionBarRight.Visibility = position == 1 ? ViewStates.Gone : ViewStates.Visible;
            _actionBarRight.SetImageResource(position == 0 ? Resource.Drawable.athletes_icon : Resource.Drawable.watch_new2);
            _actionBarLeft.Visibility = position == 0 ? ViewStates.Visible : ViewStates.Gone;
            _titleBarText.Gravity = position == 0 ? GravityFlags.Center : GravityFlags.Left | GravityFlags.CenterVertical;

            switch (position)
            {
                case 0:
                    Tab1Click();
                    break;
                case 1:
                    Tab2Click();
                    break;
                case 2:
                    Tab3Click();
                    break;
            }

        }

        private void Tab1Click()
        {
            _titleBarText.Text = "Calendar";
        }

        private void Tab2Click()
        {
            _titleBarText.Text = _middleTabTitle;
        }

        private void Tab3Click()
        {
            _titleBarText.Text = "Personal data";
        }
        public override void OnBackPressed()
        {
            MoveTaskToBack(true);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DateTime destroytTime = DateTime.Now;
            // _locationManager.RemoveUpdates(this);
        }

        //protected override void OnResume()
        //{
        //    base.OnResume();
        //    setBitmapImg();
        //}

        //private void setBitmapImg()
        //{
        //    try
        //    {
        //        var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
        //        var filePath = System.IO.Path.Combine(sdCardPath, "data/goheja.gohejanitro/files/me.png");
        //        var s2 = new FileStream(filePath, FileMode.Open);
        //        Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
        //        _tab2Icon.SetImageBitmap(bitmap2);
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

        private void startLocationService()
        {
            //this.Title = "Searching for GPS...";
            SetMiddleTabTitle("Searching for GPS...");
            // This event fires when the ServiceConnection lets the client (our App class) know that
            // the Service is connected. We use this event to start updating the UI with location
            // updates from the Service
            if (App.Current.locationServiceConnection?.Binder == null)
            {
                App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
                {
                    SubscribeLocationServie();
                };
            }
            else
            {
                SubscribeLocationServie();
            }

        }

        private void SubscribeLocationServie()
        {
            //Log.Debug (logTag, "ServiceConnected Event Raised");
            // notifies us of location changes from the system
            App.Current.LocationService.LocationChanged += HandleLocationChanged;
            //notifies us of user changes to the location provider (ie the user disables or enables GPS)
            App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
            App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
            // notifies us of the changing status of a provider (ie GPS no longer available)
            App.Current.LocationService.StatusChanged += HandleStatusChanged;
        }

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            SetMiddleTabTitle("Nitro ready...");
        }

        public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
            //this.Title=  "GPS disabled";
            SetMiddleTabTitle("GPS disabled");
        }

        public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
            //this.Title= "GPS enabled";
            SetMiddleTabTitle("GPS enabled");
        }

        public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
            //this.Title ="GPS low signal";
            SetMiddleTabTitle("GPS low signal");
        }

        private void SetMiddleTabTitle(string message)
        {
            _middleTabTitle = message;
            if (_pager.CurrentItem == 1)
            {
                _titleBarText.Text = _middleTabTitle;
            }
        }

        public void initiatAth()
        {
            connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
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
                activity2.SetFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
                activity2.PutExtra("MyData", "Data from Activity1");
                StartActivity(activity2);
                Finish();
            }
            else
            {

            }

        }
    }
}

//end by Afroz date 31/8/2016