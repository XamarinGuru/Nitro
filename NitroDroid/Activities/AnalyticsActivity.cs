using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Preferences;
using Android.Content.PM;
using Android.Webkit;
using Android.Net;
using goheja.Services;
using System.Timers;
using System.IO;
using Android.Graphics;
using PortableLibrary;

namespace goheja
{
    [Activity(Label = "Nitro", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
	public class AnalyticsActivity : BaseActivity, ILocationListener
    {
		enum RIDE_TYPE
		{
			bike = 0,
			run = 1,
		};
		RIDE_TYPE mType;

		trackSvc.Service1 svc = new trackSvc.Service1();

		ISharedPreferences contextPref;
		ISharedPreferences filePref;

		ISharedPreferencesEditor contextPrefEdit;
		ISharedPreferencesEditor filePrefEdit;

		NetworkInfo _isINET;

		Location _currentLocation;
        Location _lastLocation;

        TextView _speedText;
        TextView _altitudeText;
        TextView _distance;
		TextView _timerText;
		TextView _title;

        string athId;
        string athFirstName;
        string athLastName;
		string athNickName;
        string athCountry;

        float lastAlt;
        float dist;
        float gainAlt;
        int fFlag = 1;
        
        Button startBtn;
        Button btnStopPractice;
		Button btnBack;
		Button btnLapDist;

		bool isStarted;
        bool isPaused;
        Timer _timer;
        int duration;
        int lapDuration;
        
        DateTime tempTime;
        
        WebView wv;

		private RootMemberModel MemberModel { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.AnalyticsActivity);
            
			MemberModel = new RootMemberModel();
			MemberModel.rootMember = GetUserObject();

			svc = new trackSvc.Service1();

			_isINET = ((ConnectivityManager)GetSystemService(ConnectivityService)).ActiveNetworkInfo;

			contextPref = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
			filePref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);

			contextPrefEdit = contextPref.Edit();
			filePrefEdit = filePref.Edit();

            contextPrefEdit.PutFloat("gainAlt", 0f).Commit();
            contextPrefEdit.PutFloat("lastAlt", 0f).Commit();
            contextPrefEdit.PutFloat("dist", 0f).Commit();


			//filePrefEdit.PutString("storedFirstName", athData[0].ToString());
			//filePrefEdit.PutString("storedLastName", athData[1].ToString());
			//filePrefEdit.PutString("storedAthId", athData[2].ToString());
			//filePrefEdit.PutString("storedCountry", athData[3].ToString());
			//filePrefEdit.PutString("storedUserName", athData[4].ToString());
			//filePrefEdit.PutString("storedPsw", athData[5].ToString());
			filePrefEdit.PutFloat("lastAlt", 0f);
			filePrefEdit.PutFloat("gainAlt", 0f);
			filePrefEdit.PutFloat("distance", 0f);
			filePrefEdit.PutString("prevLoc", "");
			filePrefEdit.Commit();

			athId = GetUserID();
			athFirstName = MemberModel.firstname;
			athLastName = MemberModel.lastname;
			athNickName = MemberModel.username;
			athCountry = MemberModel.country;

			_title = FindViewById<TextView>(Resource.Id.TitleBarText);
            _speedText = FindViewById<TextView>(Resource.Id.tvSpeed);
            _altitudeText = FindViewById<TextView>(Resource.Id.tvAltitude);
            _distance = FindViewById<TextView>(Resource.Id.tvDistance);

            wv = FindViewById<WebView>(Resource.Id.wvCurrentEvent);
			wv.Settings.JavaScriptEnabled = true;
			wv.SetWebViewClient(new WebViewClient());
			wv.SetBackgroundColor(Color.Transparent);

            FindViewById<Button>(Resource.Id.StartPractice).Click += StartPractice_OnClick;
            FindViewById<Button>(Resource.Id.stopPractice).Click += stopPractice_OnClick;
			FindViewById<Button>(Resource.Id.btnBack).Click += back_OnClick;
            
            startBtn = FindViewById<Button>(Resource.Id.StartPractice);
            btnStopPractice = FindViewById<Button>(Resource.Id.stopPractice);
			btnBack = FindViewById<Button>(Resource.Id.btnBack);
			btnLapDist = FindViewById<Button>(Resource.Id.btnLD);
            
			startBtn.Visibility = ViewStates.Gone;
			btnStopPractice.Visibility = ViewStates.Gone;
			btnLapDist.Enabled = false;
			btnLapDist.Text = "";

            _timerText = FindViewById<TextView>(Resource.Id.btnTotalTime);
            
            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
           
            isStarted = false;
            isPaused = false;
            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.Elapsed += OnTimedEvent;
			_timer.Enabled = true;

			_timerText.Text = "";
			tempTime = DateTime.Now;

            duration = 0;
            lapDuration = 0;
            
            var activityNumber = Intent.GetIntExtra("EventNumber", 1);
            switch (activityNumber)
            {
                case 1:
					SetViewByType(RIDE_TYPE.bike);
                    break;
                case 2:
					SetViewByType(RIDE_TYPE.run);
                    break;
            }

			initiatAth();
			startLocationService();
        }

		private void SetViewByType(RIDE_TYPE type)
		{
			mType = type;

			startBtn.Visibility = ViewStates.Visible;

			TextView speedLbl = FindViewById<TextView>(Resource.Id.speedTv);
			Button dummyBtn = FindViewById<Button>(Resource.Id.dummyType);
			if (type == RIDE_TYPE.run)
			{
				speedLbl.Text = "min/km";
				dummyBtn.SetBackgroundResource(Resource.Drawable.bikeRound_new);
			}
			else {
				speedLbl.Text = "km/h";
				dummyBtn.SetBackgroundResource(Resource.Drawable.runRound_new);
			}
		}

        private void StartPractice_OnClick(object sender, EventArgs e)
        {
            btnLapDist.Enabled = true;
			btnBack.Visibility = ViewStates.Gone;

			var url = String.Format(Constants.URL_ANALYTICS_MAP, athNickName);
            wv.LoadUrl(url);

			if (isPaused)
            {
                isPaused = false;
				btnStopPractice.Visibility = ViewStates.Visible;
				startBtn.SetBackgroundResource(Resource.Drawable.resume_inactive);
            }
            else
            {
                if (!isStarted)
                {
					isStarted = true;
                    isPaused = false;
					btnStopPractice.Visibility = ViewStates.Visible;
                    startBtn.SetBackgroundResource(Resource.Drawable.resume_inactive);

                    DateTime dt = DateTime.Now;
                    try
                    {
						var name = athFirstName + " " + athLastName;
						var loc = String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude);
						svc.updateMomgoData(name, loc, dt, true, AppSettings.DeviceUDID, 0f, true, athId, athCountry, dist, true, gainAlt, true, _currentLocation.Bearing, true, 1, true, mType.ToString());
                    }
					catch (Exception err)
                    {
						//Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
                    }

					wv.LoadUrl(url);
                }
                else
                {
					isPaused = true;
                    startBtn.SetBackgroundResource(Resource.Drawable.resume_active);
					btnStopPractice.Visibility = ViewStates.Gone;
                }
            }
        }
        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!isPaused & isStarted)
            {
                duration++;
                lapDuration++;
            }
            var timespan = TimeSpan.FromSeconds(duration);

            RunOnUiThread(() =>
            {
                _timerText.Text = timespan.ToString(@"hh\:mm\:ss");
            });
        }
        
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                var imageView =
                    FindViewById<ImageView>(Resource.Id.imageView1);
                imageView.SetImageURI(data.Data);
            }
        }
        
		private void back_OnClick(object sender, EventArgs e)
		{
			BackProcess();
		}
        private void stopPractice_OnClick(object sender, EventArgs e)
        {
			BackProcess();
			btnBack.Visibility = ViewStates.Visible;
        }
		private void BackProcess()
		{
            isStarted = false;
			isPaused = false;
			duration = 0;

			btnStopPractice.Visibility = ViewStates.Gone;

			startBtn.SetBackgroundResource(Resource.Drawable.go_button);
			_title.Text = "Go-Heja Live is ready...";

			try //there might be times where user will exit with params bot initiated
			{
				var name = athFirstName + " " + athLastName;
				var loc = String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude);
				DateTime dt = DateTime.Now;
				dist = contextPref.GetFloat("dist", 0f);
				svc.updateMomgoData(name, loc, dt, true, AppSettings.DeviceUDID, 0f, true, athId, athCountry, dist, true, gainAlt, true, _currentLocation.Bearing, true, 2, true, mType.ToString());
			}
			catch (Exception err)
			{
				//Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
			}
			lastAlt = 0;
			dist = 0;
			gainAlt = 0;

			filePrefEdit.PutFloat("lastAlt", 0f).Commit();
			filePrefEdit.PutFloat("gainAlt", 0f).Commit();
			filePrefEdit.PutFloat("distance", 0f).Commit();
			filePrefEdit.PutString("prevLoc", "").Commit();
			filePrefEdit.PutFloat("lastAlt", 0f).Commit();
			filePrefEdit.PutFloat("gainAlt", 0f).Commit();
			filePrefEdit.PutFloat("distance", 0f).Commit();
			filePrefEdit.PutString("prevLoc", "").Commit();
			filePrefEdit.PutFloat("dist", 0f).Commit();

			contextPrefEdit.PutFloat("dist", 0f).Commit();

			btnLapDist.Text = "";
			btnLapDist.Visibility = ViewStates.Gone;

			_timerText.Text = "";
			_timerText.Enabled = false;

			startBtn.Visibility = ViewStates.Gone;

			Finish();
		}
        private void startLocationService()
        {
            _title.Text = "Searching for GPS...";

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

        //by Afroz date 2/9/2016
        private void SubscribeLocationServie()
        {
            App.Current.LocationService.LocationChanged += HandleLocationChanged;
            App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
            App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
            App.Current.LocationService.StatusChanged += HandleStatusChanged;
        }

        public void initiatAth()
        {
            bool isOnline = (_isINET != null) && _isINET.IsConnected;

            if (!isOnline)
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("No internet connection");
                builder.SetMessage("Oops!No internet connection... Pls try again later");
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate { Finish(); });
                builder.Show();
                return;
            }
        }

        //end of update afroz
        #region Android Location Service methods

        ///<summary>
        /// Updates UI with location data
        /// </summary>
        TimeSpan ts = new TimeSpan(0, 0, 20);

		public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
		{
			_title.Text = "GPS disabled";
		}

		public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
		{
			_title.Text = "GPS enabled";
		}

		public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
		{
			_title.Text = "GPS low signal";
		}
		public void vibrate(long time)
		{
			Vibrator vibrator = (Vibrator)this.GetSystemService(Context.VibratorService);
			vibrator.Vibrate(time);
		}

		public void OnLocationChanged(Location location) { }
		public void OnProviderDisabled(string provider) { }
		public void OnProviderEnabled(string provider) { }
		public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            string status = "online";

            if (!isStarted)
            {
                _title.Text = "Nitro ready...";
                _speedText.Text = "0.0";
                _altitudeText.Text = "0.0";
                _distance.Text = "0.0";
				return;
            }

            _currentLocation = e.Location;
            
            try
            {
                RunOnUiThread(() =>
                {
                    if (_currentLocation == null)
                    {
                        _title.Text = "Unable to determine your location.";
                    }
                    else
                    {
						_title.Text = "On the go";

                    	if (!isPaused)
						{
							if (_lastLocation != null)
							{
								dist = contextPref.GetFloat("dist", 0f) + _currentLocation.DistanceTo(_lastLocation) / 1000;
							}
							lastAlt = contextPref.GetFloat("lastAlt", 0f);
							float dAlt = difAlt(lastAlt, float.Parse(_currentLocation.Altitude.ToString()));
							if (dAlt < 4) gainAlt = gainAlt + dAlt;

							contextPrefEdit.PutFloat("gainAlt", gainAlt).Commit();
						}

						if (mType == RIDE_TYPE.bike)
                        {
                            _speedText.Text = (_currentLocation.Speed * 3.6f).ToString("0.00");
							btnLapDist.Text = "Lap distance : " + (dist % 5).ToString("0.00");
							if (dist % 5 < 0.01)
							{
								if (DateTime.Now - tempTime > ts)
								{
									tempTime = DateTime.Now;
									vibrate(1000);
								}
							}
                        }
						if (mType == RIDE_TYPE.run)
                        {
                            _speedText.Text = (16.6666 / (_currentLocation.Speed)).ToString("0.00");
							btnLapDist.Text = "Lap distance : " + (dist % 1).ToString("0.00");
							if (dist % 1 < 0.01)
							{
								if (DateTime.Now - tempTime > ts)
								{
									tempTime = DateTime.Now;
									vibrate(1000);
								}
							}
                        }
                        if (_currentLocation.Speed < 0.1)
                        {
                            _speedText.Text = "0.00";
                        }

                        _altitudeText.Text = gainAlt.ToString("0.00");
                        _distance.Text = (dist).ToString("0.00");

                        if (!isPaused)
                        {
							var name = athFirstName + " " + athLastName;
							DateTime dt = DateTime.Now;
							float speed = float.Parse(_currentLocation.Speed.ToString()) * 3.6f;
							record merecord = new record(name, _currentLocation.Latitude, _currentLocation.Longitude, dt, AppSettings.DeviceUDID, athId, athCountry, dist, speed, gainAlt, _currentLocation.Bearing, 0, mType.ToString());
							handleRecord updateRecord = new handleRecord();
                            status = updateRecord.updaterecord(merecord, (_isINET != null) && _isINET.IsConnected);//the record and is there internet connection
                        }

                        _lastLocation = _currentLocation;
                        contextPrefEdit.PutFloat("lastAlt", float.Parse(_currentLocation.Altitude.ToString())).Commit();
                        contextPrefEdit.PutFloat("dist", dist).Commit();
                        if (fFlag == 1 || status == "backFromOffline")
                        {
							var url = String.Format(Constants.URL_ANALYTICS_MAP, athNickName);
							wv.LoadUrl(url);
                            status = "online";
                        }
                        fFlag = 0;
                    }
                });
            }
            catch (Exception err)
            {
				//Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
            }
        }
        #endregion
    }
}

