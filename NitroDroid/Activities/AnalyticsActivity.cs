using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android.Preferences;
using Android.Content.PM;
using goheja.Services;
using System.Timers;
using PortableLibrary;
using Android;
using Android.Support.V4.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using System.Collections.Generic;

namespace goheja
{
    [Activity(Label = "Nitro", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
	public class AnalyticsActivity : BaseActivity, ILocationListener, IOnMapReadyCallback, ActivityCompat.IOnRequestPermissionsResultCallback, GoogleMap.IOnMarkerClickListener
    {
		readonly string[] PermissionsLocation =
		{
		  Manifest.Permission.AccessCoarseLocation,
		  Manifest.Permission.AccessFineLocation
		};

		const int RequestLocationId = 0;

		LocationManager _locationManager;

		SupportMapFragment mMapViewFragment;
		GoogleMap mMapView = null;

		EventPoints mEventMarker = new EventPoints();
		IList<string> pointIDs;

		enum RIDE_TYPE
		{
			bike = 1,
			run = 2,
			mountain = 6
		};
		RIDE_TYPE mType;

		trackSvc.Service1 svc = new trackSvc.Service1();

		ISharedPreferences contextPref;
		ISharedPreferences filePref;

		ISharedPreferencesEditor contextPrefEdit;
		ISharedPreferencesEditor filePrefEdit;

		Location _currentLocation;
        Location _lastLocation;

        TextView _speedText, _altitudeText, _distance, _timerText, _title;
        string athId, athFirstName, athLastName, athNickName, athCountry;
        float lastAlt, dist, gainAlt;

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

		private RootMemberModel MemberModel { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.AnalyticsActivity);

			if (!IsNetEnable()) return;
            
			MemberModel = new RootMemberModel();
			MemberModel.rootMember = GetUserObject();

			svc = new trackSvc.Service1();

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

			mMapViewFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
			mMapViewFragment.GetMapAsync(this);

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
				case 6:
					SetViewByType(RIDE_TYPE.mountain);
					break;
            }

			_locationManager = GetSystemService(Context.LocationService) as LocationManager;

			CheckLocationPermission();
        }

		#region google map

		public void OnMapReady(GoogleMap googleMap)
		{
			if (googleMap == null) return;

			mMapView = googleMap;
			mMapView.MyLocationEnabled = false;

			if (mMapView != null)
			{
				mMapView.SetOnMarkerClickListener(this);

				//SetNearestEventMarkers();
			}
		}

		void SetNearestEventMarkers()
		{
			var currentLocation = GetGPSLocation();

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_ALL_MARKERS);

				mEventMarker = GetNearestEventMarkers(AppSettings.UserID);
				//mEventMarker = GetAllMarkers("58aafae816528b16d898a1f3");

				HideLoadingView();

				var mapBounds = new LatLngBounds.Builder();

				mapBounds.Include(new LatLng(currentLocation.Latitude, currentLocation.Longitude));

				pointIDs = new List<string>();

				RunOnUiThread(() =>
				{
					if (mEventMarker != null && mEventMarker.markers.Count > 0)
					{
						for (int i = 0; i < mEventMarker.markers.Count; i++)
						{
							var point = mEventMarker.markers[i];
							var pointLocation = new LatLng(point.lat, point.lng);
							mapBounds.Include(pointLocation);

							AddMapPin(pointLocation, point.type);
						}
					}

					mMapView.MoveCamera(CameraUpdateFactory.NewLatLngBounds(mapBounds.Build(), 50));
				});
			});
		}

		void AddMapPin(LatLng position, string type)
		{
			MarkerOptions markerOpt = new MarkerOptions();
			markerOpt.SetPosition(position);

			var metrics = Resources.DisplayMetrics;
			var wScreen = metrics.WidthPixels;

			Bitmap bmp = GetPinIconByType(type);
			Bitmap newBitmap = ScaleDownImg(bmp, wScreen / 7, true);
			markerOpt.SetIcon(BitmapDescriptorFactory.FromBitmap(newBitmap));

			RunOnUiThread(() =>
			{
				var marker = mMapView.AddMarker(markerOpt);
				pointIDs.Add(marker.Id);
			});
		}

		public bool OnMarkerClick(Marker marker)
		{
			try
			{
				PortableLibrary.Point selectedPoint = null;
				for (var i = 0; i < pointIDs.Count; i++)
				{
					if (marker.Id == pointIDs[i])
						selectedPoint = mEventMarker.markers[i];
				}
				if (selectedPoint == null) return false;

				PointInfoDialog myDiag = PointInfoDialog.newInstance(selectedPoint);
				myDiag.Show(FragmentManager, "Diag");
			}
			catch
			{
				return true;
			}

			return true;
		}

		#endregion

		private void SetViewByType(RIDE_TYPE type)
		{
			mType = type;

			startBtn.Visibility = ViewStates.Visible;

			TextView speedLbl = FindViewById<TextView>(Resource.Id.speedTv);
			Button dummyBtn = FindViewById<Button>(Resource.Id.dummyType);
			if (type == RIDE_TYPE.run)
			{
				speedLbl.Text = "min/km";
				dummyBtn.SetBackgroundResource(Resource.Drawable.runRound_new);
			}
			else if (type == RIDE_TYPE.bike) {
				speedLbl.Text = "km/h";
				dummyBtn.SetBackgroundResource(Resource.Drawable.bikeRound_new);
			}
			else if (type == RIDE_TYPE.mountain) {
				speedLbl.Text = "km/h";
				dummyBtn.SetBackgroundResource(Resource.Drawable.icon_06);
			}
		}

        private void StartPractice_OnClick(object sender, EventArgs e)
        {
            btnLapDist.Enabled = true;
			btnBack.Visibility = ViewStates.Gone;

			if (isPaused)
            {
                isPaused = false;
				btnStopPractice.Visibility = ViewStates.Visible;
				startBtn.SetBackgroundResource(Resource.Drawable.resume_inactive);

				_locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);
            }
            else
            {
                if (!isStarted)
                {

					_locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);
					

					isStarted = true;
                    isPaused = false;
					btnStopPractice.Visibility = ViewStates.Visible;
                    startBtn.SetBackgroundResource(Resource.Drawable.resume_inactive);

                    DateTime dt = DateTime.Now;
                    try
                    {
						var name = athFirstName + " " + athLastName;
						var loc = String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude);
						svc.updateMomgoData(name, loc, dt, true, AppSettings.DeviceUDID, 0f, true, athId, athCountry, dist, true, gainAlt, true, _currentLocation.Bearing, true, 1, true, mType.ToString(), Constants.SPEC_GROUP_TYPE[0]);
                    }
					catch (Exception err)
                    {
						//Toast.MakeText(this, err.ToString(), ToastLength.Long).Show();
                    }
                }
                else
                {
					_locationManager.RemoveUpdates(this);


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

				svc.updateMomgoData(name, loc, dt, true, AppSettings.DeviceUDID, 0f, true, athId, athCountry, dist, true, gainAlt, true, _currentLocation.Bearing, true, 2, true, mType.ToString(), Constants.SPEC_GROUP_TYPE[0]);
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

			_locationManager.RemoveUpdates(this);

			var activity = new Intent(this, typeof(SwipeTabActivity));
			StartActivity(activity);
			Finish();
		}


		#region grant location service
		private void CheckLocationPermission()
		{
			if ((int)Build.VERSION.SdkInt < 23)
			{
				StartLocationService();
			}
			else {
				RequestLocationPermission();
			}
		}
		void RequestLocationPermission()
		{
			const string permission = Manifest.Permission.AccessFineLocation;
			if (CheckSelfPermission(permission) == (int)Permission.Granted)
			{
				StartLocationService();
				return;
			}

			if (ShouldShowRequestPermissionRationale(permission))
			{
				AlertDialog.Builder alert = new AlertDialog.Builder(this);
				alert.SetTitle("");
				alert.SetMessage("Location access is required to gaugo your sports.");
				alert.SetPositiveButton("Cancel", (senderAlert, args) =>
				{
				});
				alert.SetNegativeButton("OK", (senderAlert, args) =>
				{
					ActivityCompat.RequestPermissions(this, PermissionsLocation, RequestLocationId);
				});
				RunOnUiThread(() =>
				{
					alert.Show();
				});

				return;
			}

			ActivityCompat.RequestPermissions(this, PermissionsLocation, RequestLocationId);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			if (requestCode == RequestLocationId && grantResults[0] == Permission.Granted)
			{
				StartLocationService();
			}
		}


		#endregion
        private void StartLocationService()
        {
            _title.Text = "Searching for GPS...";

            //if (App.Current.locationServiceConnection?.Binder == null)
            //{
            //    App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
            //    {
            //        SubscribeLocationServie();
            //    };
            //}
            //else
            //{
            //    SubscribeLocationServie();
            //}


        }

        //by Afroz date 2/9/2016
   //     private void SubscribeLocationServie()
   //     {
   //         App.Current.LocationService.LocationChanged += HandleLocationChanged;
   //         App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
   //         App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
   //         App.Current.LocationService.StatusChanged += HandleStatusChanged;

			//App.Current.LocationService.StopLocationUpdates();
   //     }

        //end of update afroz
        #region Android Location Service methods

        ///<summary>
        /// Updates UI with location data
        /// </summary>
        TimeSpan ts = new TimeSpan(0, 0, 20);

		//public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
		//{
		//	_title.Text = "GPS disabled";
		//}

		//public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
		//{
		//	_title.Text = "GPS enabled";
		//}

		//public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
		//{
		//	_title.Text = "GPS low signal";
		//}
		public void vibrate(long time)
		{
			Vibrator vibrator = (Vibrator)this.GetSystemService(Context.VibratorService);
			vibrator.Vibrate(time);
		}

		//public void OnLocationChanged(Location location) { }
		public void OnProviderDisabled(string provider) {_title.Text = "GPS disabled"; }
		public void OnProviderEnabled(string provider) { _title.Text = "GPS enabled";}
		public void OnStatusChanged(string provider, Availability status, Bundle extras) {_title.Text = "GPS low signal"; }

		private Location GetGPSLocation()
		{
			_locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);
			Location currentLocation = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
			_locationManager.RemoveUpdates(this);

			return currentLocation;
		}

        //public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
		public void OnLocationChanged(Location location)
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

            _currentLocation = location;
            
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
							status = updateRecord.updaterecord(merecord, IsNetEnable());//the record and is there internet connection

							SetMapPosition(new LatLng(_currentLocation.Latitude, _currentLocation.Longitude));
                        }

                        _lastLocation = _currentLocation;
                        contextPrefEdit.PutFloat("lastAlt", float.Parse(_currentLocation.Altitude.ToString())).Commit();
                        contextPrefEdit.PutFloat("dist", dist).Commit();
                        if (fFlag == 1 || status == "backFromOffline")
                        {
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

		void SetMapPosition(LatLng location)
		{
			if (mMapView == null) return;

			CameraUpdate cu_center = CameraUpdateFactory.NewLatLngZoom(location, Constants.MAP_ZOOM_LEVEL);
			mMapView.MoveCamera(cu_center);
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				return false;
			}

			return base.OnKeyDown(keyCode, e);
		}
    }
}

