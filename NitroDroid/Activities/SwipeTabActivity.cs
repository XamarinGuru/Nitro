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
		private static Intent serviceIntent = null;

		//ISharedPreferences contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);

		RelativeLayout tabCalendar, tabAnalytics, tabProfile;

        private ViewPager _pager;

		Android.Graphics.Color cTabEnable = new Android.Graphics.Color(146, 146, 146);
		Android.Graphics.Color cTabDisable = new Android.Graphics.Color(69, 69, 69);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SwipeTabActivity);

			if (serviceIntent == null)
			{
				AppSettings.baseVC = this;
				serviceIntent = new Intent(this, typeof(BackgroundService));
				this.StartService(serviceIntent);
			}

            InitilizeComponant();
           
            StartLocationService();
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


        private void InitilizeComponant()
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

        private void StartLocationService()
        {
            //SetMiddleTabTitle("Searching for GPS...");
            
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
            //SetMiddleTabTitle("Nitro ready...");
        }

        public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
            //SetMiddleTabTitle("GPS disabled");
        }

        public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
            //SetMiddleTabTitle("GPS enabled");
        }

        public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
            //SetMiddleTabTitle("GPS low signal");
        }
    }
}
