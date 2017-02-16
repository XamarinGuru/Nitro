using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Android;

namespace goheja
{
    [Activity(Label = "Nitro" , Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleInstance)]
	public class SwipeTabActivity : BaseActivity
    {
		string[] PermissionsCalendar =
		{
			Manifest.Permission.ReadCalendar,
			Manifest.Permission.WriteCalendar
		};
		const int RequestCalendarId = 0;

		static Intent serviceIntent = null;

		RelativeLayout tabCalendar, tabAnalytics, tabProfile;

        ViewPager _pager;

		Android.Graphics.Color cTabEnable = new Android.Graphics.Color(146, 146, 146);
		Android.Graphics.Color cTabDisable = new Android.Graphics.Color(69, 69, 69);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SwipeTabActivity);

            InitUISettings();
           
			CheckCalendarPermission();
            //StartLocationService();
        }

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				if (_pager.CurrentItem != 0)
				{
					SetPage(0);
					return true;
				}
				else {
					return false;
				}
			}

			return base.OnKeyDown(keyCode, e);
		}

        private void InitUISettings()
        {
            tabCalendar = FindViewById<RelativeLayout>(Resource.Id.tabCalendar);
            tabAnalytics = FindViewById<RelativeLayout>(Resource.Id.tabAnalytics);
            tabProfile = FindViewById<RelativeLayout>(Resource.Id.tabProfile);

            GenericFragmentPagerAdaptor _adaptor = new GenericFragmentPagerAdaptor(SupportFragmentManager, this);
			_pager = FindViewById<ViewPager>(Resource.Id.pager);
            _pager.Adapter = _adaptor;
            _pager.PageSelected += PagerOnPageSelected;

			FindViewById<RelativeLayout>(Resource.Id.tabCalendar).Click += (sender, args) => { SetPage(0); };
            FindViewById<RelativeLayout>(Resource.Id.tabAnalytics).Click += (sender, args) => { SetPage(1); };
            FindViewById<RelativeLayout>(Resource.Id.tabProfile).Click += (sender, args) => { SetPage(2); };
        }

		public void SetPage(int position)
        {
            _pager.SetCurrentItem(position, true);
        }

        private void PagerOnPageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            SetSelect(e.Position);
        }

        private void SetSelect(int position)
        {
            switch (position)
            {
                case 0:
					tabCalendar.SetBackgroundColor(cTabEnable);
					tabAnalytics.SetBackgroundColor(cTabDisable);
					tabProfile.SetBackgroundColor(cTabDisable);
                    break;
                case 1:
                    tabCalendar.SetBackgroundColor(cTabDisable);
					tabAnalytics.SetBackgroundColor(cTabEnable);
					tabProfile.SetBackgroundColor(cTabDisable);
                    break;
                case 2:
                    tabCalendar.SetBackgroundColor(cTabDisable);
					tabAnalytics.SetBackgroundColor(cTabDisable);
					tabProfile.SetBackgroundColor(cTabEnable);
                    break;
				case 3:
					tabCalendar.SetBackgroundColor(cTabDisable);
					tabAnalytics.SetBackgroundColor(cTabDisable);
					tabProfile.SetBackgroundColor(cTabEnable);
					break;
            }
        }

		#region grant calendar access permission
		private void CheckCalendarPermission()
		{
			if ((int)Build.VERSION.SdkInt < 23)
			{
				StartBackgroundService();
			}
			else {
				RequestCalendarPermission();

			}
		}
		void RequestCalendarPermission()
		{
			const string rdPermission = Manifest.Permission.ReadCalendar;
			const string wrPermission = Manifest.Permission.WriteCalendar;
			if (CheckSelfPermission(rdPermission) == (int)Permission.Granted && CheckSelfPermission(wrPermission) == (int)Permission.Granted)
			{
				StartBackgroundService();
				return;
			}

			if (ShouldShowRequestPermissionRationale(rdPermission) || ShouldShowRequestPermissionRationale(wrPermission))
			{
				AlertDialog.Builder alert = new AlertDialog.Builder(this);
				alert.SetTitle("");
				alert.SetMessage("Calendar access is required to show your events on your device calendar.");
				alert.SetPositiveButton("Cancel", (senderAlert, args) =>
				{
				});
				alert.SetNegativeButton("OK", (senderAlert, args) =>
				{
					ActivityCompat.RequestPermissions(this, PermissionsCalendar, RequestCalendarId);
				});
				RunOnUiThread(() =>
				{
					alert.Show();
				});

				return;
			}

			ActivityCompat.RequestPermissions(this, PermissionsCalendar, RequestCalendarId);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			switch (requestCode)
			{
				case RequestCalendarId:
					{
						if (grantResults[0] == Permission.Granted)
						{
							StartBackgroundService();
						}
						else
						{
						}
					}
					break;
			}
		}

		void StartBackgroundService()
		{
			if (serviceIntent == null)
			{
				AppSettings.baseVC = this;
				serviceIntent = new Intent(this, typeof(BackgroundService));
				this.StartService(serviceIntent);
			}
		}
		#endregion


        //private void StartLocationService()
        //{
        //    if (App.Current.locationServiceConnection?.Binder == null)
        //    {
        //        App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
        //        {
        //            SubscribeLocationServie();
        //        };
        //    }
        //    else
        //    {
        //        SubscribeLocationServie();
        //    }
        //}

        //private void SubscribeLocationServie()
        //{
        //    App.Current.LocationService.LocationChanged += HandleLocationChanged;
        //    App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
        //    App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
        //    App.Current.LocationService.StatusChanged += HandleStatusChanged;
        //}

        //public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        //{
        //}

        //public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        //{
        //}

        //public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        //{
        //    //SetMiddleTabTitle("GPS enabled");
        //}

        //public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        //{
        //    //SetMiddleTabTitle("GPS low signal");
        //}


    }
}
