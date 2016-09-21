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

namespace goheja
{

    //by Afroz date 22/07/2016
    [Activity(Label = "Nitro", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    //end of update by afroz
    //[Activity(Label = "Nitro", MainLauncher = false, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait,LaunchMode=LaunchMode.SingleInstance)]
    public class MainActivity : Activity, ILocationListener
    {
        //ConnectivityManager connectivityManager = (ConnectivityManager) GetSystemService(ConnectivityService);
        Location _currentLocation;
        Location _lastLocation;
        LocationManager _locationManager;
        TextView _locationText;
        TextView _speedText;
        TextView _altitudeText;
        TextView _GradientText;
        TextView _distance;
        TextView timer;
        Button startEvent;
        trackSvc.Service1 svc;
        string _locationProvider;
        string athId;
        string athFirstName;
        string athLastName;
        string athCountry;
        private NotificationManager notificationManager;
        float lastAlt;
        float dist;
        float gainAlt;
        string android_id;
        int fFlag = 1;
        Android.Locations.Location location;
        ConnectivityManager connectivityManager;
        Button startBtn;
        NetworkInfo activeConnection;
        bool isStarted;
        Button btnStopPractice;
		Button btnBack;
        bool isPaused;
        Timer _timer;
        int duration;
        int lapDuration;
        //by Afroz date 23/07/2016
        //Button btnSelectBike;
        //Button btnSelectRun;
        bool IsWatchSynced = false;
        //end of update by afroz
        string type = "bike";
        //Button selectLbl;
        Button dummyBtn;
        TextView speedLbl;
        ImageView profile;
        //Button deviceBtn;
        ImageView deviceBtn;
        //Button timerBtn;
        TextView timerBtn;
        Button btnLL;
        Button btnLapDist;
        float distForLap;
        DateTime tempTime;//for vibrate
        ISharedPreferences prefs;
        handleRecord updateRecord;
        NetworkInfo internetavilable;
        bool isOnline;
        WebView wv;
        ISharedPreferences contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
        string internetState;
        private TextView TitleBarText;
        ImageView drawerHandle;
        ImageView watchDevider;


        public void OnLocationChanged(Location location)
        {

        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }


        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnResume()
        {
            base.OnResume();
            setBitmapImg();
			btnBack.Visibility = ViewStates.Visible;
            //  _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
        }
        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Finish();
        }

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            IntializeComponants();
            
            TitleBarText = FindViewById<TextView>(Resource.Id.TitleBarText);
            startLocationService();

            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            prefs.Edit().PutFloat("gainAlt", 0f).Commit();
            prefs.Edit().PutFloat("lastAlt", 0f).Commit();
            prefs.Edit().PutFloat("dist", 0f).Commit();

            initiatAth();
            trackSvc.Service1 test = new trackSvc.Service1();
            string _deviceId = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            _speedText = FindViewById<TextView>(Resource.Id.tvSpeed);
            _altitudeText = FindViewById<TextView>(Resource.Id.tvAltitude);

            _distance = FindViewById<TextView>(Resource.Id.tvDistance);

            startEvent = FindViewById<Button>(Resource.Id.StartPractice);
            wv = FindViewById<WebView>(Resource.Id.wvCurrentEvent);

            FindViewById<ImageView>(Resource.Id.meCalendar).Click += meCalendar_OnClick;
            FindViewById<Button>(Resource.Id.StartPractice).Click += StartPractice_OnClick;
            FindViewById<Button>(Resource.Id.stopPractice).Click += stopPractice_OnClick;
			FindViewById<Button>(Resource.Id.btnBack).Click += back_OnClick;
            
            FindViewById<ImageView>(Resource.Id.deviceBtn).Click += selectDevice_OnClick;
            deviceBtn = FindViewById<ImageView>(Resource.Id.deviceBtn);
            FindViewById<ImageView>(Resource.Id.profileIv).Click += personalData_OnClick;
            dummyBtn = FindViewById<Button>(Resource.Id.dummyType);
            startBtn = FindViewById<Button>(Resource.Id.StartPractice);
            btnStopPractice = FindViewById<Button>(Resource.Id.stopPractice);
			btnBack = FindViewById<Button>(Resource.Id.btnBack);
            
            speedLbl = FindViewById<TextView>(Resource.Id.speedTv);
            btnLL = FindViewById<Button>(Resource.Id.btnLL);
            btnLapDist = FindViewById<Button>(Resource.Id.btnLD);

            profile = FindViewById<ImageView>(Resource.Id.profileIv);
            timerBtn = FindViewById<TextView>(Resource.Id.btnTotalTime);
            btnStopPractice.Enabled = false;
            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
            svc = new trackSvc.Service1();
            android_id = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

            var contextEdit = contextPref.Edit();

            wv.Settings.JavaScriptEnabled = true;
            wv.SetWebViewClient(new WebViewClient());
            string nickName = contextPref.GetString("storedUserName", "");
            isStarted = false;
            isPaused = false;
            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.Elapsed += OnTimedEvent;
            duration = 0;
            lapDuration = 0;
            startBtn.Enabled = false;
            startBtn.SetBackgroundResource(Resource.Drawable.transparent);
            btnStopPractice.SetBackgroundResource(Resource.Drawable.transparent);
            dummyBtn.SetBackgroundResource(Resource.Drawable.transparent);
            _timer.Enabled = true;
            dummyBtn.Enabled = false;
            btnLL.Enabled = false;
            btnLapDist.Enabled = false;
            btnLL.Text = "";
            btnLapDist.Text = "";
            timerBtn.Text = "";
            distForLap = 0f;
            tempTime = DateTime.Now;
            updateRecord = new handleRecord();
            internetavilable = connectivityManager.ActiveNetworkInfo;

            isOnline = (internetavilable != null) && internetavilable.IsConnected;
            wv.Settings.JavaScriptEnabled = true;
            wv.SetWebViewClient(new WebViewClient());
            contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
            nickName = contextPref.GetString("storedUserName", "");
            contextEdit.PutString("isOnline", "online");

            try
            {
                string[] athData = test.getAthDataByDeviceId(_deviceId);
                contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
                contextEdit = contextPref.Edit();

                contextEdit.PutString("storedFirstName", athData[0].ToString());
                contextEdit.PutString("storedAthId", athData[2].ToString());
                contextEdit.PutString("storedLastName", athData[1].ToString());
                contextEdit.PutString("storedCountry", athData[3].ToString());
                contextEdit.PutString("storedUserName", athData[4].ToString());
                contextEdit.PutString("storedPsw", athData[5].ToString());
                ///////
                contextEdit.PutFloat("lastAlt", 0f);
                contextEdit.PutFloat("gainAlt", 0f);
                contextEdit.PutFloat("distance", 0f);
                contextEdit.PutString("prevLoc", "");
                ///////
                contextEdit.Commit();
                athId = (contextPref.GetString("storedAthId", "0").ToString());
                athFirstName = contextPref.GetString("storedFirstName", "");
                athLastName = contextPref.GetString("storedLastName", "");
                athCountry = contextPref.GetString("storedCountry", "");
                
                setBitmapImg();

                wv.SetBackgroundColor(Color.Transparent);
            }
            catch (Exception err)
            {
                string x = err.ToString();
            }

            var activityNumber = Intent.GetIntExtra("EventNumber", 1);
            switch (activityNumber)
            {
                case 1:
                    selectBike_OnClick();
                    break;
                case 2:
                    selectRun_OnClick();
                    break;
            }
        }

        //by Afroz date 23/07/2016
        private void IntializeComponants()
        {
            var drawerContent = FindViewById<LinearLayout>(Resource.Id.drawerContent);
            drawerHandle = FindViewById<ImageView>(Resource.Id.drawerHandle);
            drawerContent.Visibility = ViewStates.Gone;
            drawerHandle.Click += delegate (object sender, EventArgs e)
            {
                if (drawerContent.Visibility == ViewStates.Visible)
                {
                    drawerHandle.SetImageResource(Resource.Drawable.menu_icon);
                    drawerContent.Visibility = ViewStates.Gone;
                }
                else
                {
                    drawerHandle.SetImageResource(Resource.Drawable.menu_icon_01);
                    drawerContent.Visibility = ViewStates.Visible;
                }
            };

            watchDevider = FindViewById<ImageView>(Resource.Id.watchDevider);
            deviceBtn = FindViewById<ImageView>(Resource.Id.deviceBtn);
            if (IsWatchSynced)
            {
                deviceBtn.Visibility = ViewStates.Gone;
                watchDevider.Visibility = ViewStates.Gone;
            }
        }

        //end of update by afroz

        private void btnExit_OnClick(object sender, EventArgs e)
        {
            startEvent.Enabled = true;
            DateTime dt = DateTime.Now;
            var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
            var contextEdit = contextPref.Edit();
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

            if (isStarted)
            {
                isStarted = false;
                isPaused = false;
                try
                { //there might be times where user will exit with params bot initiated
                    svc.updateMomgoData(athFirstName + " " + athLastName, String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude), dt, true, android_id, 0f, true, athId, athCountry, prefs.GetFloat("dist", 0f), true, gainAlt, true, _currentLocation.Bearing, true, 2, true, type);
                }
                catch (Exception err)
                {
                    string t = err.ToString();
                }
            }
            lastAlt = 0;
            dist = 0;
            gainAlt = 0;

            contextEdit.PutFloat("lastAlt", 0f);
            contextEdit.PutFloat("gainAlt", 0f);
            contextEdit.PutFloat("distance", 0f);
            contextEdit.PutString("prevLoc", "");
            contextEdit.PutFloat("lastAlt", 0f);
            contextEdit.PutFloat("gainAlt", 0f);
            contextEdit.PutFloat("distance", 0f);
            contextEdit.PutString("prevLoc", "");
            contextEdit.PutFloat("dist", 0f);
            prefs.Edit().PutFloat("dist", 0f).Commit();
            OnDestroy();
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            Finish();
        }
        private void meCalendar_OnClick(object sender, EventArgs e)
        {
            var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
            string nickName = contextPref.GetString("storedUserName", "");
            var uri = Android.Net.Uri.Parse("http://go-heja.com/nitro/mobda.php?userNickName=" + nickName + "&userId=" + contextPref.GetString("storedAthId", ""));
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
            drawerHandle.CallOnClick();
        }

        private void StartPractice_OnClick(object sender, EventArgs e)
        {

            btnLapDist.Enabled = true;

            btnLL.Enabled = true;

            var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
            string nickName = contextPref.GetString("storedUserName", "");
            //////////////////////////////////////////////////
            var wv = FindViewById<WebView>(Resource.Id.wvCurrentEvent);
            wv.Settings.JavaScriptEnabled = true;
            wv.SetWebViewClient(new WebViewClient());
            wv.LoadUrl("http://go-heja.com/nitro/mobongoing.php?txt=" + nickName);
            //////////////////////////////////////////////////
            if (isPaused)
            {
                startBtn.SetBackgroundResource(Resource.Drawable.resume_inactive);
                isPaused = false;
                btnStopPractice.Enabled = false;
                btnStopPractice.SetBackgroundResource(Resource.Drawable.transparent);

				btnBack.Visibility = ViewStates.Gone;
            }
            else
            {
                if (!isStarted)
                {
					btnBack.Visibility = ViewStates.Gone;
					btnStopPractice.SetBackgroundResource(Resource.Drawable.transparent);
                    isPaused = false;
                    startBtn.SetBackgroundResource(Resource.Drawable.resume_inactive);
                    btnStopPractice.Enabled = false;
                    btnStopPractice.SetBackgroundResource(Resource.Drawable.transparent);

                    DateTime dt = DateTime.Now;
                    try
                    { //there might be times where user will exit with params bot initiated
                        svc.updateMomgoData(athFirstName + " " + athLastName, String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude), dt, true, android_id, 0f, true, athId, athCountry, dist, true, gainAlt, true, _currentLocation.Bearing, true, 1, true, type);
                    }
                    catch
                    {
                    }
                    wv = FindViewById<WebView>(Resource.Id.wvCurrentEvent);
                    wv.Settings.JavaScriptEnabled = true;
                    wv.SetWebViewClient(new WebViewClient());
                    contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
                    nickName = contextPref.GetString("storedUserName", "");
                    wv.LoadUrl("http://go-heja.com/nitro/mobongoing.php?txt=" + nickName);
                    isStarted = true;
                }
                else
                {
                    startBtn.SetBackgroundResource(Resource.Drawable.resume_active);
                    btnStopPractice.SetBackgroundResource(Resource.Drawable.stop_active);
                    btnStopPractice.Enabled = true;
                    isPaused = true;
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
            var lapTimeSpan = TimeSpan.FromSeconds(lapDuration);

            RunOnUiThread(() =>
            {
                timerBtn.Text = timespan.ToString(@"hh\:mm\:ss");
            });
        }

        private void selectDevice_OnClick(object sender, EventArgs e)
        {
            var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
            string nickName = contextPref.GetString("storedUserName", "");
            var uri = Android.Net.Uri.Parse("http://go-heja.com/gh/mob/sync.php?userId=" + contextPref.GetString("storedAthId", "0").ToString() + "&mog=nitro&url=uurrll");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
            drawerHandle.CallOnClick();
        }

        //by Afroz date 23/07/2016
        private void selectRun_OnClick(/*object sender, EventArgs e*/)
        {
            type = "run";

            dummyBtn.SetBackgroundResource(Resource.Drawable.runRound_new);
            startBtn.SetBackgroundResource(Resource.Drawable.go_button);
            dummyBtn.SetBackgroundResource(Resource.Drawable.transparent);
            dummyBtn.Enabled = true;
            startBtn.Enabled = true;
            speedLbl.Text = "min/km";
            dummyBtn.SetBackgroundResource(Resource.Drawable.runRound_new);
        }
        //end of update by afroz

        private void setImage_OnClick(object sender, EventArgs e)
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(
            Intent.CreateChooser(imageIntent, "Select photo"), 0);
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

        //by Afroz date 23/07/2016
        private void selectBike_OnClick(/*object sender, EventArgs e*/)
        {
            type = "bike";

            startBtn.SetBackgroundResource(Resource.Drawable.go_button);

            dummyBtn.Enabled = true;
            //dummyBtn.SetBackgroundResource(Resource.Drawable.bikeRound);
            dummyBtn.SetBackgroundResource(Resource.Drawable.bikeRound_new);
            startBtn.Enabled = true;
            speedLbl.Text = "km/h";
            
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
			btnStopPractice.Enabled = false;
			btnStopPractice.SetBackgroundResource(Resource.Drawable.transparent);
			startBtn.SetBackgroundResource(Resource.Drawable.go_button);
			TitleBarText.Text = "Go-Heja Live is ready...";

			startEvent.Enabled = true;
			DateTime dt = DateTime.Now;
			var contextPref = Application.Context.GetSharedPreferences("goheja", FileCreationMode.Private);
			var contextEdit = contextPref.Edit();
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

			trackSvc.Service1 test = new trackSvc.Service1();
			try //there might be times where user will exit with params bot initiated
			{
				svc.updateMomgoData(athFirstName + " " + athLastName, String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude), dt, true, android_id, 0f, true, athId, athCountry, prefs.GetFloat("dist", 0f), true, gainAlt, true, _currentLocation.Bearing, true, 2, true, type);
			}
			catch (Exception err)
			{
				string t = err.ToString();
			}
			lastAlt = 0;
			dist = 0;
			gainAlt = 0;

			contextEdit.PutFloat("lastAlt", 0f);
			contextEdit.PutFloat("gainAlt", 0f);
			contextEdit.PutFloat("distance", 0f);
			contextEdit.PutString("prevLoc", "");
			contextEdit.PutFloat("lastAlt", 0f);
			contextEdit.PutFloat("gainAlt", 0f);
			contextEdit.PutFloat("distance", 0f);
			contextEdit.PutString("prevLoc", "");
			contextEdit.PutFloat("dist", 0f);
			prefs.Edit().PutFloat("dist", 0f).Commit();
			var wv = FindViewById<WebView>(Resource.Id.wvCurrentEvent);
			wv.Settings.JavaScriptEnabled = true;
			wv.SetWebViewClient(new WebViewClient());
			string nickName = contextPref.GetString("storedUserName", "");
			wv.LoadUrl("");

			dummyBtn.Enabled = false;
			dummyBtn.SetBackgroundResource(Resource.Drawable.transparent);
			btnLapDist.Text = "";
			btnLapDist.Enabled = false;
			btnLapDist.SetBackgroundResource(Resource.Drawable.transparent);
			btnLL.Text = "";
			btnLL.Enabled = false;
			btnLL.SetBackgroundResource(Resource.Drawable.transparent);
			timerBtn.Text = "";
			timerBtn.Enabled = false;
			//timerBtn.SetBackgroundResource (Resource.Drawable.transparent);
			btnLapDist.Enabled = false;
			btnLapDist.SetBackgroundResource(Resource.Drawable.transparent);
			btnLL.Enabled = false;
			btnLL.SetBackgroundResource(Resource.Drawable.transparent);

			startBtn.Enabled = false;
			startBtn.SetBackgroundResource(Resource.Drawable.transparent);

			Finish();
		}
        private void startLocationService()
        {
            TitleBarText.Text = "Searching for GPS...";

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
        //end of update by afroz
        private void personalData_OnClick(object sender, EventArgs eventArgs)
        {


            var activity2go = new Intent(this, typeof(personalData));
            activity2go.PutExtra("MyData", "Data from Activity1");
            StartActivity(activity2go);
            drawerHandle.CallOnClick();
        }

        public void initiatAth()
        {
            connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            activeConnection = connectivityManager.ActiveNetworkInfo;
            isOnline = (activeConnection != null) && activeConnection.IsConnected;

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

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            string nickName = contextPref.GetString("storedUserName", "");
            string status = "online";

            if (!isStarted)
            {
                TitleBarText.Text = "Nitro ready...";
                _speedText.Text = "0.0";
                _altitudeText.Text = "0.0";
                _distance.Text = "0.0";

            }
            else
            {
                location = e.Location;
                if (location.Latitude.ToString() != "")
                {
                    TitleBarText.Text = "On the go";
                }
                try
                {
                    RunOnUiThread(() =>
                    {
                        ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
                        _currentLocation = location;
                        if (!isPaused)
                        {
                            if (_lastLocation != null & _currentLocation != null)
                            {
                                dist = prefs.GetFloat("dist", 0f) + _currentLocation.DistanceTo(_lastLocation) / 1000;
                            }
                            lastAlt = prefs.GetFloat("lastAlt", 0f);
                            float dAlt = difAlt(lastAlt, float.Parse(_currentLocation.Altitude.ToString()));
                            if (dAlt < 4)
                                gainAlt = gainAlt + dAlt;

							prefs.Edit().PutFloat("gainAlt", gainAlt).Commit();
                        }
                        if (_currentLocation == null)
                        {
                            _locationText.Text = "Unable to determine your location.";
                        }
                        else
                        {
                            if (true)
                            {//(_currentLocation.Speed>0.3)

                                if (type == "bike")
                                {
                                    _speedText.Text = (location.Speed * 3.6f).ToString("0.00");
                                }
                                if (type == "run" & location.Speed > 0.1)
                                {
                                    _speedText.Text = (16.6666 / (location.Speed)).ToString("0.00");
                                }
                                if (location.Speed < 0.1)
                                {
                                    _speedText.Text = "0.00";
                                }
                                _altitudeText.Text = gainAlt.ToString("0.00");
                                _distance.Text = (dist).ToString("0.00");
                                distForLap = dist;
                                if (type == "run")
                                {
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
                                if (type == "bike")
                                {
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

                                athFirstName = contextPref.GetString("storedFirstName", "");
                                athLastName = contextPref.GetString("storedLastName", "");

                                DateTime dt = DateTime.Now;

                                android_id = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

                                float speed = float.Parse(_currentLocation.Speed.ToString());
                                speed = speed * 3.6f;

                                if (!isPaused)
                                {
                                    internetavilable = connectivityManager.ActiveNetworkInfo;
                                    record merecord = new record(athFirstName + " " + athLastName, _currentLocation.Latitude, _currentLocation.Longitude, dt, android_id, athId, athCountry, dist, speed, gainAlt, _currentLocation.Bearing, 0, type);

                                    status = updateRecord.updaterecord(merecord, (internetavilable != null) && internetavilable.IsConnected);//the record and is there internet connection
                                }

                                _lastLocation = _currentLocation;
                                prefs.Edit().PutFloat("lastAlt", float.Parse(_currentLocation.Altitude.ToString())).Commit();
                                prefs.Edit().PutFloat("dist", dist).Commit();
                                if (fFlag == 1 || status == "backFromOffline")
                                {
                                    wv.LoadUrl("http://go-heja.com/nitro/mobongoing.php?txt=" + nickName);
                                    status = "online";
                                }
                                fFlag = 0;
                            }
                        }
                    });
                }
                catch (Exception err)
                {
                    string temp = err.ToString();
                }
            }
        }

        public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
            TitleBarText.Text = "GPS disabled";
        }

        public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
            TitleBarText.Text = "GPS enabled";
        }

        public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
            TitleBarText.Text = "GPS low signal";
        }
        public void vibrate(long time)
        {
            Vibrator vibrator = (Vibrator)this.GetSystemService(Context.VibratorService);
            vibrator.Vibrate(time);
        }

        #endregion

        public static float difAlt(float prev, float curr)
        {
            try
            {
                if ((curr - prev) > 0)
                {
                    return curr - prev;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
        void setBitmapImg()
        {
            try
            {
                var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
                var filePath = System.IO.Path.Combine(sdCardPath, "data/goheja.gohejanitro/files/me.png");
                var s2 = new FileStream(filePath, FileMode.Open);
                Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
                profile.SetImageBitmap(bitmap2);
                s2.Close();
            }
            catch (Exception err)
            {
            }
            finally
            {
            }
        }

        void ExportBitmapAsPNG2()
        {
            try
            {
                Bitmap bitmap = BitmapFactory.DecodeResource(null, Resource.Drawable.ninja);
                var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
                var filePath = System.IO.Path.Combine(sdCardPath, "data/goheja.gohejanitro/files/me.png");

                var stream = new FileStream(filePath, FileMode.Create);

                bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);// Bitmap.CompressFormat.Png, 100, stream);
                stream.Close();

                GC.Collect();
            }
            catch (Exception err)
            {
                Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
            }
        }

		private void back_OnClick(object sender, EventArgs e)
		{
			BackProcess();
		}
    }
}

