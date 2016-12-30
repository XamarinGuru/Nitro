using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using System;
using Android.Locations;
using Android.Net;
using goheja.Services;

namespace goheja
{
    [Activity(Label = "Nitro" , Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleInstance)]
	public class SwipeTabActivity : BaseActivity
    {
		ISharedPreferences contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);

        private ImageView 	_tab1Icon,
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
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SwipeTabActivity);

			var contextEdit = contextPref.Edit();
			contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			string _deviceId = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
			//try
			//{
			//	trackSvc.Service1 test = new trackSvc.Service1();
			//	string[] athData = test.getAthDataByDeviceId(_deviceId);
			//	contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			//	contextEdit = contextPref.Edit();

			//	contextEdit.PutString("storedFirstName", athData[0]);
			//	contextEdit.PutString("storedAthId", athData[2]);
			//	contextEdit.PutString("storedLastName", athData[1]);
			//	contextEdit.PutString("storedCountry", athData[3]);
			//	contextEdit.PutString("storedUserName", athData[4]);
			//	contextEdit.PutString("storedPsw", athData[5]);
			//	contextEdit.Commit();
			//}
			//catch
			//{
			//	ShowMessageBox("No internet connection", "Oops!No internet connection... Pls try again later", true);
			//	return;
			//}

            InitilizeComponant();
           
            startLocationService();
            initiatAth();
        }

        private void InitilizeComponant()
        {
            _titleBarText = FindViewById<TextView>(Resource.Id.TitleBarTextMain);
            _tab1Icon = FindViewById<ImageView>(Resource.Id.Tab1Icon);
            _tab2Icon = FindViewById<ImageView>(Resource.Id.Tab2Icon);
            _tab3Icon = FindViewById<ImageView>(Resource.Id.Tab3Icon);
            _tabStrip1 = FindViewById<ImageView>(Resource.Id.TabStrip1);
            _tabStrip2 = FindViewById<ImageView>(Resource.Id.TabStrip2);
            _tabStrip3 = FindViewById<ImageView>(Resource.Id.TabStrip3);

            _actionBarLeft = FindViewById<ImageView>(Resource.Id.ActionBarLeft);
			_actionBarRight = FindViewById<ImageView>(Resource.Id.ActionBarRight);
			_actionBarRight.Click += ActionBarRightOnClick;

            GenericFragmentPagerAdaptor _adaptor = new GenericFragmentPagerAdaptor(SupportFragmentManager, this);
			_pager = FindViewById<ViewPager>(Resource.Id.pager);
            _pager.Adapter = _adaptor;
            _pager.PageSelected += PagerOnPageSelected;

            _tab1Icon.Click += (sender, args) => { SetPage(0); };
            _tab2Icon.Click += (sender, args) => { SetPage(1); };
            _tab3Icon.Click += (sender, args) => { SetPage(2); };

            _titleBarText.Text = "Calendar";
        }

        private void ActionBarRightOnClick(object sender, EventArgs eventArgs)
        {
			var athId = contextPref.GetString("storedAthId", "0").ToString();
            var uri = Android.Net.Uri.Parse("http://go-heja.com/gh/mob/sync.php?userId=" + athId + "&mog=nitro&url=uurrll");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
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
                    _titleBarText.Text = "Calendar";
                    break;
                case 1:
                    _titleBarText.Text = _middleTabTitle;
                    break;
                case 2:
                    _titleBarText.Text = "Personal data";
                    break;
            }
        }

        private void startLocationService()
        {
            SetMiddleTabTitle("Searching for GPS...");
            
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
            App.Current.LocationService.LocationChanged += HandleLocationChanged;
            App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
            App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
            App.Current.LocationService.StatusChanged += HandleStatusChanged;
        }

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            SetMiddleTabTitle("Nitro ready...");
        }

        public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
            SetMiddleTabTitle("GPS disabled");
        }

        public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
            SetMiddleTabTitle("GPS enabled");
        }

        public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
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
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            bool isOnline = (activeConnection != null) && activeConnection.IsConnected;

    //        if (!isOnline)
    //        {
				//ShowMessageBox("No internet connection", "Oops!No internet connection... Pls try again later", true);
    //            return;
    //        }
    //        trackSvc.Service1 test = new trackSvc.Service1();
    //        string deviceId = "0";
    //        try
    //        {
    //            deviceId = test.getListedDeviceId(Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId));
    //        }
    //        catch
    //        {
				//ShowMessageBox("Nitro service is not available", "Oops!Service not available... Pls try again later", true);
    //            return;
    //        }

    //        if (deviceId == "0")
    //        {
    //            var activity2 = new Intent(this, typeof(RegisterActivity));
    //            activity2.SetFlags(ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask);
    //            activity2.PutExtra("MyData", "Data from Activity1");
    //            StartActivity(activity2);
    //            Finish();
    //        }
        }
    }
}
