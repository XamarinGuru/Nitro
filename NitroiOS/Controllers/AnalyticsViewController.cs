using System;
using Foundation;
using UIKit;
using CoreLocation;
using System.Threading.Tasks;
using PortableLibrary;

namespace location2
{
	public partial class AnalyticsViewController : BaseViewController
	{
		enum RIDE_TYPE
		{
			bike = 0,
			run = 1,
		};

		public static LocationManager Manager { get; set; }


		trackSvc.Service1 meServ = new trackSvc.Service1();
		Reachability.Reachability connection = new Reachability.Reachability();

		RIDE_TYPE selected;

		public bool startStop;
		public bool paused;

		bool isTimerStarted = false;

		private RootMemberModel MemberModel { get; set; }
		public AnalyticsViewController(IntPtr handle) : base(handle)
		{
			Manager = new LocationManager();
			Manager.StartLocationUpdates();
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

			MemberModel.rootMember = GetUserObject();

			wvOngoing.ScalesPageToFit = true;
		}

		public static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		#region Public Methods
		CLLocation _lastLocation;
		double _currentDistance = 0;
		Double _lastAltitude;
		DateTime _dt;
		double _speed;
		float currdistance = 0;
		int flag = 0;

		public void HandleLocationChanged(object sender, LocationUpdatedEventArgs e)
		{
			if (flag == 2)
			{
				var url = String.Format(Constants.ANALYTICS_MAP_URL, AppSettings.Username);
				wvOngoing.LoadRequest(new NSUrlRequest(new NSUrl(url)));
			}

			if (flag <= 2) flag++;

			if (startStop)
			{
				//if (this.lblTitle.Text.Contains("GPS"))
				//	this.lblTitle.Text = "On the go";

				// Handle foreground updates
				CLLocation location = e.Location;

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

						meServ.updateMomgoData(name, loc, _dt, true, AppSettings.DeviceUDID, currspeed, true, AppSettings.UserID, country, currdistance, true, currAlt, true, course, true, 0, true, selected.ToString());

						if (currspeed < 0)
							currspeed = 0;
						lblSpeed.Text = currspeed.ToString("0.00");
						lblAlt.Text = currAlt.ToString("0.00");
						lblDist.Text = _currentDistance.ToString("0.00");
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
			}
			else
			{
				if (!startStop)
				{
					if (!connection.IsHostReachable("www.google.com"))
					{
						ShowMessageBox(null, "No internet connection!");
						//this.lblTitle.Text = "No intenet connecion...";
						return;
					}
					else
					{
						//this.lblTitle.Text = "Searching for GPS...";
						Manager.LocationUpdated += HandleLocationChanged;
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

					//connection = new Reachability.Reachability();
				}
				else
				{
					//this.lblTitle.Text = "Paused...";
					startStopBtn.SetBackgroundImage(UIImage.FromFile("resume_active.png"), UIControlState.Normal);
					stopBtn.Hidden = false;
					paused = true;
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

				meServ.updateMomgoData(name, location, _dt, true, AppSettings.DeviceUDID, speed, true, AppSettings.UserID, MemberModel.country, currdistance, true, alt, true, bearing, true, 2, true, selected.ToString());
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

			Manager.LocationUpdated -= HandleLocationChanged;

			NSUserDefaults.StandardUserDefaults.SetInt(0, "timer");
			NSUserDefaults.StandardUserDefaults.SetDouble(0, "lastDistance");
			NSUserDefaults.StandardUserDefaults.SetDouble(0, "lastAltitude");
		}

		private void SwitchSecondViewByType(RIDE_TYPE type)
		{
			selected = type;

			viewSelectType.Hidden = true;

			Manager.LocationUpdated += HandleLocationChanged;

			if (type == RIDE_TYPE.run)
			{
				speedTypeLbl.Text = "min/km";
				imgTypeIcon.Image = UIImage.FromBundle("runRound_new.png");
			}
			else {
				speedTypeLbl.Text = "km/h";
				imgTypeIcon.Image = UIImage.FromBundle("bikeRound_new.png");
			}
		}

		#region event handlers
		partial void SelectRunBtn_TouchUpInside(UIButton sender)
		{
			SwitchSecondViewByType(RIDE_TYPE.run);
		}

		partial void SelectBikeBtn_TouchUpInside(UIButton sender)
		{
			SwitchSecondViewByType(RIDE_TYPE.bike);
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

