using System;
using Foundation;
using UIKit;
using CoreLocation;
using System.Threading.Tasks;
using PortableLibrary;
using Google.Maps;
using System.Drawing;
using CoreGraphics;
using System.Collections.Generic;

namespace location2
{
	public partial class AnalyticsViewController : BaseViewController
	{
		enum RIDE_TYPE
		{
			bike = 0,
			run = 1,
			mountain = 5
		};

		//public static LocationManager Manager = new LocationManager();

		MapView mMapView;

		Marker markerMyLocation = null;

		EventPoints mEventMarker = new EventPoints();

		trackSvc.Service1 meServ = new trackSvc.Service1();

		RIDE_TYPE selected;

		public bool startStop;
		public bool paused;

		bool isTimerStarted = false;

		public AnalyticsViewController(IntPtr handle) : base(handle)
		{
			//Manager = new LocationManager();
			//Manager.StartLocationUpdates();
			MemberModel = new RootMemberModel();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			startStop = false;
			paused = false;

			altimg.Layer.ZPosition = 1;
			bpmImg.Layer.ZPosition = 1;
			distImg.Layer.ZPosition = 1;
			wattImg.Layer.ZPosition = 1;
			speedImg.Layer.ZPosition = 1;

			lblSpeed.Layer.ZPosition = 1;
			bpmLbl.Layer.ZPosition = 1;
			lblWatt.Layer.ZPosition = 1;
			lblDist.Layer.ZPosition = 1;
			lblAlt.Layer.ZPosition = 1;

			speedTypeLbl.Layer.ZPosition = 1;
			lblBpm.Layer.ZPosition = 1;
			lblWatt.Layer.ZPosition = 1;
			distTypLbl.Layer.ZPosition = 1;
			altTypeLbl.Layer.ZPosition = 1;

			if (!IsNetEnable()) return;

			MemberModel.rootMember = GetUserObject();

			InitMapView();
		}

		//public static bool UserInterfaceIdiomIsPhone
		//{
		//	get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		//}

		//public override void ViewDidAppear(bool animated)
		//{
		//	base.ViewDidAppear(animated);

		//	InitMapView();
		//}

		//public override void ViewWillLayoutSubviews()
		//{
		//	if (mMapView != null && viewMapContent != null && viewMapContent.Window != null)
		//	{
		//		RepaintMap();
		//	}
		//}
		void InitMapView()
		{
			var camera = CameraPosition.FromCamera(31.0461, 34.8516, zoom: PortableLibrary.Constants.MAP_ZOOM_LEVEL);
			mMapView = MapView.FromCamera(RectangleF.Empty, camera);
			mMapView.MyLocationEnabled = false;

			mMapView.TappedMarker = ClickedDropItem;

			var myLocation = LocationHelper.GetLocationResult();

			markerMyLocation = new Marker
			{
				Position = new CLLocationCoordinate2D(myLocation.Latitude, myLocation.Longitude),
				Map = mMapView,
				Icon = UIImage.FromFile("pin_me.png")
			};
			SetMapPosition(new CLLocation(myLocation.Latitude, myLocation.Longitude));

			//viewMapContent.LayoutIfNeeded();
			//var width = viewMapContent.Frame.Width;
			//var height = viewMapContent.Frame.Height;
			//mMapView.Frame = new CGRect(0, 0, width, height);
			
			//viewMapContent.AddSubview(mMapView);
		}

		public void RepaintMap()
		{
			foreach (var subview in viewMapContent.Subviews)
			{
				subview.RemoveFromSuperview();
			}

			viewMapContent.LayoutIfNeeded();
			var width = viewMapContent.Frame.Width;
			var height = viewMapContent.Frame.Height;
			mMapView.Frame = new CGRect(0, 0, width, height);

			viewMapContent.AddSubview(mMapView);

			//SetNearestEventMarkers();
		}

		void SetNearestEventMarkers()
		{
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(PortableLibrary.Constants.MSG_LOADING_ALL_MARKERS);

				mEventMarker = GetNearestEventMarkers(AppSettings.UserID);
				//mEventMarker = GetAllMarkers("58aafae816528b16d898a1f3");

				HideLoadingView();

				InvokeOnMainThread(() =>
				{
					var boundPoints = new List<CLLocationCoordinate2D>();

					if (mEventMarker != null && mEventMarker.markers.Count > 0)
					{
						for (int i = 0; i < mEventMarker.markers.Count; i++)
						{
							var point = mEventMarker.markers[i];
							var imgPin = GetPinIconByType(point.type);
							var pointLocation = new CLLocationCoordinate2D(point.lat, point.lng);
							boundPoints.Add(pointLocation);

							AddMapPin(pointLocation, imgPin, i);
						}
					}

					if (boundPoints.Count > 0)
					{
						var mapBounds = new CoordinateBounds();
						foreach (var bound in boundPoints)
							mapBounds = mapBounds.Including(bound);
						mMapView.MoveCamera(CameraUpdate.FitBounds(mapBounds, 50.0f));
					}
				});
			});
		}



		void AddMapPin(CLLocationCoordinate2D position, UIImage icon, int zIndex)
		{
			var marker = new Marker
			{
				Position = position,
				Map = mMapView,
				Icon = icon,
				ZIndex = zIndex
			};
		}
		#region map pin click event
		bool ClickedDropItem(MapView mapView, Marker marker)
		{
			if (marker.ZIndex == -1) return true;

			var selectedPoint = mEventMarker.markers[marker.ZIndex];

			PointInfoView cpuv = PointInfoView.Create(selectedPoint);
			cpuv.PopUp(true, delegate
			{
				Console.WriteLine("cpuv will close");
			});

			return true;
		}
		#endregion

		void SetMapPosition(CLLocation location)
		{
			var camera = CameraPosition.FromCamera(location.Coordinate.Latitude, location.Coordinate.Longitude, zoom: PortableLibrary.Constants.MAP_ZOOM_LEVEL);
			mMapView.Animate(camera);

			if (markerMyLocation != null)
				markerMyLocation.Position = new CLLocationCoordinate2D(location.Coordinate.Latitude, location.Coordinate.Longitude);
		}

		#region Public Methods
		CLLocation _lastLocation;
		double _currentDistance = 0;
		Double _lastAltitude;
		DateTime _dt;
		double _speed;
		float currdistance = 0;
		int flag = 0;

		void LocationUpdated(object sender, EventArgs e)
		{
			CLLocationsUpdatedEventArgs locArgs = e as CLLocationsUpdatedEventArgs;
			var location = locArgs.Locations[locArgs.Locations.Length - 1];

		//}

		//public void HandleLocationChanged(object sender, LocationUpdatedEventArgs e)
		//{
			if (flag <= 2) flag++;

			if (startStop)
			{
				//if (this.lblTitle.Text.Contains("GPS"))
				//	this.lblTitle.Text = "On the go";

				// Handle foreground updates
				//CLLocation location = e.Location;

				if (!paused)
				{
					try
					{
						if (location != null & _lastLocation != null)
						{
							_currentDistance = _currentDistance + location.DistanceFrom(_lastLocation) / 1000;
						}
						_lastAltitude = NSUserDefaults.StandardUserDefaults.DoubleForKey("lastAltitude") + Calculate.difAlt(_lastAltitude, location.Altitude);
					}
					catch
					{
					}
				}

				_dt = DateTime.Now;

				if (selected == RIDE_TYPE.bike)
				{
					_speed = location.Speed * 3.6;
				}
				if (selected == RIDE_TYPE.run)
				{
					if (location.Speed > 0)
						_speed = 16.6666 / location.Speed;
					else
						_speed = 0;
				}
				float course = float.Parse(location.Course.ToString());

				currdistance = float.Parse(_currentDistance.ToString());
				float currAlt = float.Parse(_lastAltitude.ToString());
				float currspeed = float.Parse(_speed.ToString());
				try
				{
					if (!paused)
					{
						var name = MemberModel.firstname + " " + MemberModel.lastname;
						var loc = location.Coordinate.Latitude.ToString() + "," + location.Coordinate.Longitude.ToString();
						var country = MemberModel.country;

						meServ.updateMomgoData(name, loc, _dt, true, AppSettings.DeviceUDID, currspeed, true, AppSettings.UserID, country, currdistance, true, currAlt, true, course, true, 0, true, selected.ToString(), PortableLibrary.Constants.SPEC_GROUP_TYPE[0]);

						if (currspeed < 0)
							currspeed = 0;
						lblSpeed.Text = currspeed.ToString("0.00");
						lblAlt.Text = currAlt.ToString("0.00");
						lblDist.Text = _currentDistance.ToString("0.00");

						SetMapPosition(location);
					}

					//end
					//save this used location as last location
					_lastLocation = location;
					NSUserDefaults.StandardUserDefaults.SetDouble(_currentDistance, "lastDistance");
					NSUserDefaults.StandardUserDefaults.SetDouble(currAlt, "lastAltitude");
				}
				catch
				{
				}
			}
		}



		partial void StartStopBtn_TouchUpInside(UIButton sender)
		{
			if (paused)
			{
				//this.lblTitle.Text = "On the go...";
				startStopBtn.SetBackgroundImage(UIImage.FromFile("resume_inactive.png"), UIControlState.Normal);
				stopBtn.Hidden = true;
				paused = false;
				backBtn.Hidden = true;
				LocationHelper.StartLocationManager();
				LocationHelper.LocationUpdated += LocationUpdated;
			}
			else
			{
				if (!startStop)
				{
					if (!IsNetEnable())
					{
						return;
					}
					else
					{
						//this.lblTitle.Text = "Searching for GPS...";
						//Manager.LocationUpdated += HandleLocationChanged;
						LocationHelper.StartLocationManager();
						LocationHelper.LocationUpdated += LocationUpdated;
					}
					startStopBtn.SetBackgroundImage(UIImage.FromFile("resume_active.png"), UIControlState.Normal);
					stopBtn.Hidden = false;
					startStop = true;
					paused = false;

					if (!isTimerStarted)
					{
						StartTimer();
						backBtn.Hidden = true;
					}
					isTimerStarted = true;
				}
				else
				{
					//this.lblTitle.Text = "Paused...";
					startStopBtn.SetBackgroundImage(UIImage.FromFile("resume_active.png"), UIControlState.Normal);
					stopBtn.Hidden = false;
					paused = true;

					LocationHelper.StopLocationManager();
				}
			}
		}




		#endregion
		private int _duration = 0;

		async Task StartTimer()
		{
			_duration = 0;
			while (true)
			{
				await Task.Delay(1000);
				if (!paused) _duration++;
				NSUserDefaults.StandardUserDefaults.SetInt(_duration, "timer");
				string s = TimeSpan.FromSeconds(_duration).ToString(@"hh\:mm\:ss");

				lblTimer.Text = s;
			}
		}

		private void BackProcess()
		{
			//this.lblTitle.Text = "Nitro ready...";
			viewSelectType.Hidden = false;

			try
			{
				var name = MemberModel.firstname + " " + MemberModel.lastname;
				var location = _lastLocation.Coordinate.Latitude.ToString() + "," + _lastLocation.Coordinate.Longitude.ToString();
				var speed = float.Parse(_lastLocation.Speed.ToString());
				var alt = float.Parse(NSUserDefaults.StandardUserDefaults.DoubleForKey("lastAltitude").ToString());
				var bearing = float.Parse(_lastLocation.Course.ToString());

				meServ.updateMomgoData(name, location, _dt, true, AppSettings.DeviceUDID, speed, true, AppSettings.UserID, MemberModel.country, currdistance, true, alt, true, bearing, true, 2, true, selected.ToString(), PortableLibrary.Constants.SPEC_GROUP_TYPE[0]);
			}
			catch
			{
			}

			lblTimer.Text = "";
			lblSpeed.Text = "0.00";
			lblDist.Text = "0.00";
			lblAlt.Text = "0.0";
			//this.lblTitle.Text = "Nitro ready..";
			_duration = 0;
			_lastLocation = null;
			_currentDistance = 0;
			_lastAltitude = 0;
			_speed = 0;
			currdistance = 0;

			flag = 0;

			backBtn.Hidden = false;
			stopBtn.Hidden = true;

			startStop = true;
			paused = true;

			startStopBtn.SetBackgroundImage(UIImage.FromFile("go_button.png"), UIControlState.Normal);

			//Manager.LocationUpdated -= HandleLocationChanged;
			//Manager.StopLocationUpdates();

			//LocationHelper.LocationUpdated -= LocationUpdated;
			LocationHelper.StopLocationManager();

			NSUserDefaults.StandardUserDefaults.SetInt(0, "timer");
			NSUserDefaults.StandardUserDefaults.SetDouble(0, "lastDistance");
			NSUserDefaults.StandardUserDefaults.SetDouble(0, "lastAltitude");
		}

		//private void SwitchSecondViewByType(RIDE_TYPE type)
		//{
		//	selected = type;

		//	viewSelectType.Hidden = true;

		//	Manager.LocationUpdated += HandleLocationChanged;

		//	if (type == RIDE_TYPE.run)
		//	{
		//		speedTypeLbl.Text = "min/km";
		//		imgTypeIcon.Image = UIImage.FromBundle("runRound_new.png");
		//	}
		//	else {
		//		speedTypeLbl.Text = "km/h";
		//		imgTypeIcon.Image = UIImage.FromBundle("bikeRound_new.png");
		//	}
		//}

		#region event handlers
		partial void ActionSelectSportType(UIButton sender)
		{
			viewSelectType.Hidden = true;

			//Manager.LocationUpdated += HandleLocationChanged;

			switch (sender.Tag)
			{
				case (int)RIDE_TYPE.bike:
					speedTypeLbl.Text = "km/h";
					imgTypeIcon.Image = UIImage.FromBundle("bikeRound_new.png");
					selected = RIDE_TYPE.bike;
					//SwitchSecondViewByType(RIDE_TYPE.bike);
					break;
				case (int)RIDE_TYPE.run:
					speedTypeLbl.Text = "min/km";
					imgTypeIcon.Image = UIImage.FromBundle("runRound_new.png");
					selected = RIDE_TYPE.run;
					//SwitchSecondViewByType(RIDE_TYPE.run);
					break;
				case (int)RIDE_TYPE.mountain:
					speedTypeLbl.Text = "km/h";
					imgTypeIcon.Image = UIImage.FromBundle("icon_06.png");
					selected = RIDE_TYPE.mountain;
					//SwitchSecondViewByType(RIDE_TYPE.mountain);
					break;
			}

			if (mMapView != null && viewMapContent != null && viewMapContent.Window != null)
			{
				RepaintMap();
			}


		}

		partial void StopBtn_TouchUpInside(UIButton sender)
		{
			BackProcess();
		}
		partial void BackBtn_TouchupInside(UIButton sender)
		{
			BackProcess();
		}
		#endregion
	}
}

