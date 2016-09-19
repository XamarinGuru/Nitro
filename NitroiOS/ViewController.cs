using System;
using Foundation;
using UIKit;
using CoreLocation;
using System.Threading.Tasks;
using System.Net;
using UIKit;
using System.Drawing;


namespace location2
{
	
	public partial class ViewController : PageContentViewController
	{
		trackSvc.Service1 meServ;
		bool  isWatchSynced = false;//
		string deviceId;
		string[] athData;
		Reachability.Reachability connection;
		public bool startStop;
		public bool paused;
		bool toolStatus=false;
		string selected="";
		bool isTimerStarted=false;

		public override void ViewDidLoad ()
		{
			try
			{

				base.ViewDidLoad();
				//displaying profile image in dropdown
				if(getImg()!=null)//updated afroz 8/8/2016
				{
					//meBtn.SetBackgroundImage(getImg(),UIControlState.Normal);
				}
				stopBtn.Enabled = false;
				startStop = false;
				paused = false;
				this.Title = "Nitro live";

				btnBack.TouchUpInside += DoneButtonClicked;

				altimg.Layer.ZPosition = 1;
				bpmImg.Layer.ZPosition = 1;
				distImg.Layer.ZPosition = 1;
				wattImg.Layer.ZPosition = 1;
				speedImg.Layer.ZPosition = 1;

				lblSpeed.Layer.ZPosition = 1;
				bpmLbl.Layer.ZPosition = 1;
				wattLbl.Layer.ZPosition = 1;
				lblDist.Layer.ZPosition = 1;
				lblAlt.Layer.ZPosition = 1;

				speedTypeLbl.Layer.ZPosition = 1;
				bpmValueLbl.Layer.ZPosition = 1;
				wattLbl.Layer.ZPosition = 1;
				distTypLbl.Layer.ZPosition = 1;
				altTypeLbl.Layer.ZPosition = 1;

				startStopBtn.Enabled = false;
				startStopBtn.Hidden = true;


				selectedBtn.Enabled = false;
				selectedBtn.Hidden = true;

			}
			catch (Exception err)
			{
				string temp = err.ToString();
			}
			//watch 
			if(isWatchSynced)//updated by afroz 8/8/2016
			{
				//watchBtn.Hidden = true;
				//dropDownView.Frame = new RectangleF (270f, 81f, 45f, 96f);
				//CalenBtn.Frame = new RectangleF (0f, 45f, 45f, 52f);
				//viewLine.Hidden=true;
				//isWatchSynced = false;
			}
			else
			{
				//watchBtn.Hidden = false;
				//dropDownView.Frame = new RectangleF (270f, 81f, 45f, 150f);
				//CalenBtn.Frame = new RectangleF (0f, 100f, 45f, 52f);
				//viewLine.Hidden=false;
			}
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			btnBack.Hidden = false;
			//displaying profile image in dropdown
			if(getImg()!=null)//updated afroz 8/8/2016
			{
				//meBtn.SetBackgroundImage(getImg(),UIControlState.Normal);
			}
		}
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
		#region Computed Properties
		public static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public static LocationManager Manager { get; set;}
		#endregion

		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
			// As soon as the app is done launching, begin generating location updates in the location manager
			Manager = new LocationManager();
			Manager.StartLocationUpdates();
		}

		#endregion

		#region Public Methods
		CLLocation _lastLocation;
		double _currentDistance=0;
		Double _lastAltitude;
		DateTime _dt;
		double _speed;
		string _bearing;
		float currdistance = 0;
		int flag=0;

		public void HandleLocationChanged (object sender, LocationUpdatedEventArgs e)
		{
			if (flag == 2) 
			{
				//wvOngoing.Hidden=false;
				var url = "http://go-heja.com/nitro/mobongoingApl.php?txt="+NSUserDefaults.StandardUserDefaults.StringForKey("userName"); // NOTE: https secure request
				wvOngoing.LoadRequest(new NSUrlRequest(new NSUrl(url)));
				wvOngoing.ScalesPageToFit = true;

			}
			if (startStop) {
				if (this.Title.Contains ("GPS")) {
					this.Title = "On the go";
				}
				// Handle foreground updates
				CLLocation location = e.Location;

				if (!paused) {
					try {//we might have coordinat thta is not initiated
						//_currentDistance = _currentDistance  + calculate.distance(location.Coordinate.Latitude,location.Coordinate.Longitude, _lastLocation.Coordinate.Latitude,_lastLocation.Coordinate.Longitude,'k');
						if (location != null & _lastLocation != null) {
							_currentDistance = _currentDistance + location.DistanceFrom (_lastLocation) / 1000;
						}
						_lastAltitude = NSUserDefaults.StandardUserDefaults.DoubleForKey ("lastAltitude") + calculate.difAlt (_lastAltitude, location.Altitude);
					} catch {
					}
				}
				_bearing = calculate.getDirection (location.Course);
				_dt = DateTime.Now;


				if (selected == "bike") 
				{
					_speed = location.Speed * 3.6;
				}
				if (selected == "run") 
				{
					if (location.Speed > 0) {
						_speed = 16.6666 / location.Speed;
					}
					else 
					{
						_speed = 0;
					}	
				}
				float course = float.Parse (location.Course.ToString ());

				currdistance = float.Parse (_currentDistance.ToString ());
				float currAlt = float.Parse (_lastAltitude.ToString ());
				float currspeed = float.Parse (_speed.ToString ());
				try {
					if (!paused)
					{
						meServ.updateMomgoData (NSUserDefaults.StandardUserDefaults.StringForKey ("firstName").ToString () + " " + NSUserDefaults.StandardUserDefaults.StringForKey ("lastName").ToString (), location.Coordinate.Latitude.ToString () + "," + location.Coordinate.Longitude.ToString (), _dt, true, NSUserDefaults.StandardUserDefaults.StringForKey ("deviceId").ToString (), currspeed, true,(NSUserDefaults.StandardUserDefaults.StringForKey ("id").ToString ()), NSUserDefaults.StandardUserDefaults.StringForKey ("country").ToString (), currdistance, true, currAlt, true, course, true, 0, true,selected);

						if (currspeed < 0)
							currspeed = 0;
						lblSpeed.Text = currspeed.ToString ("0.00");
						lblAlt.Text = currAlt.ToString ("0.00");
						//lblDir.Text = " "+_bearing ;
						lblDist.Text = _currentDistance.ToString ("0.00");
					}

					//end
					//save this used location as last location
					_lastLocation = location;
					NSUserDefaults.StandardUserDefaults.SetDouble (_currentDistance, "lastDistance"); 
					NSUserDefaults.StandardUserDefaults.SetDouble (currAlt, "lastAltitude"); 

				} catch (Exception err) {

				}

			}
			if (flag <=2) {

				flag++;
			}

		}


		/*partial void BtnStress_TouchUpInside (UIButton sender)
		{
			//bool accepted=MessageBox.ShowAsync("ACTIVATE SOS?","",MessageBoxButton.YesNo);
		
				new UIAlertView(null, "Not implemented yet", null, "OK", null).Show();


			//meServ.updateMomgoData(NSUserDefaults.StandardUserDefaults.StringForKey("firstName").ToString()+" "+NSUserDefaults.StandardUserDefaults.StringForKey("lastName").ToString(),_lastLocation.Coordinate.Longitude.ToString()+","+_lastLocation.Coordinate.Latitude.ToString(),_dt,true,NSUserDefaults.StandardUserDefaults.StringForKey("deviceId").ToString(),0.01f,true,int.Parse(NSUserDefaults.StandardUserDefaults.IntForKey("id").ToString()),true,NSUserDefaults.StandardUserDefaults.StringForKey("country").ToString().Trim(),currdistance,true,float.Parse(_lastAltitude.ToString()),true,0f,true,2,true);
				Manager.LocationUpdated -= HandleLocationChanged;
			//new UIAlertView(null, "SOS call", null, null, null).Show();
		}*/


		partial void StartStopBtn_TouchUpInside (UIButton sender)
		{
			
			if (paused)
			{
				btnBack.Hidden = true;
				startStopBtn.SetBackgroundImage(UIImage.FromFile ("resume_inactive.png"), UIControlState.Normal);
				paused=false;
				stopBtn.SetBackgroundImage(UIImage.FromFile (""), UIControlState.Normal);
				stopBtn.Enabled=false;
				this.Title="On the go...";
			}
			else
			{
				if (!startStop)
				{

					if (!isTimerStarted)
					{
						StartTimer();
						btnBack.Hidden = true;
					}
					isTimerStarted=true;
					startStopBtn.SetBackgroundImage(UIImage.FromFile ("resume_active.png"), UIControlState.Normal);
					stopBtn.SetBackgroundImage(UIImage.FromFile (""), UIControlState.Normal);
					stopBtn.Enabled=true;
					connection = new Reachability.Reachability ();
					if (!connection.IsHostReachable("www.google.com")) 
					{
						new UIAlertView(null, "No internet connection!", null, "OK", null).Show();
						this.Title="No intenet connecion..."	;
						return;
					}
					else
					{
						if (deviceId == "0") 
						{
							//new UIAlertView(null, "You are not listed in Go-Heja Live!", null, "OK", null).Show();
							//vcListing controller = this.Storyboard.InstantiateViewController ("vcListing") as vcListing;
							//this.NavigationController.PushViewController (controller, true);
						} 
						else 
						{
							this.Title="Searching for GPS...";
							Manager.LocationUpdated += HandleLocationChanged;

							//startBtn.Enabled=false;
							//cant use google maps, untill ill get the hang of app maps ill use the browser in btnMap
							//
							//

						}
					}
					startStop=true;
					paused=false;

				}
				else
				{
					//btnBack.Hidden = false;
					startStopBtn.SetBackgroundImage(UIImage.FromFile ("resume_active.png"), UIControlState.Normal);
					stopBtn.SetBackgroundImage(UIImage.FromFile ("stop_active.png"), UIControlState.Normal);
					this.Title="Paused...";
					stopBtn.Enabled=true;
					stopBtn.Hidden=false;
					paused=true;


				}
			}


		}

		//partial void MapBtn_TouchUpInside (UIButton sender)
		//{
		//UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/  ski/mob/mobileDay.php?userNickName="+NSUserDefaults.StandardUserDefaults.StringForKey("userName")+"&userId=+"+NSUserDefaults.StandardUserDefaults.IntForKey("id")));
		//UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/surfski/mobongoingApl.php?txt="+NSUserDefaults.StandardUserDefaults.StringForKey("userName")));
		//}





		partial void CalenBtn_TouchUpInside (UIButton sender)
		{
			//NSUserDefaults.StandardUserDefaults.SetString ("calen", "source");
			//UIcalendar calendarPage = this.Storyboard.InstantiateViewController ("UIcalendar") as UIcalendar;
			//this.NavigationController.PushViewController (calendarPage, true);
			UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/nitro/mobda.php?userNickName="+NSUserDefaults.StandardUserDefaults.StringForKey("userName")+"&userId=+"+NSUserDefaults.StandardUserDefaults.IntForKey("id")));
			//UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/gh/hia.php"));
		}

		partial void StopBtn_TouchUpInside (UIButton sender)
		{
			BackProcess();
		}

		partial void ToolBtn_TouchUpInside (UIButton sender)
		{
			if(toolStatus)
			{
			//dropDownView.Hidden=true;//updated afroz 6/8/2016
				//imgArrow.Hidden=true;//updated afroz 6/8/2016
//			meBtn.Enabled = true;
//			meBtn.Hidden = false;
//			watchBtn.Enabled = true;
//			watchBtn.Hidden = false;
//
//			CalenBtn.Enabled = true;
//			CalenBtn.Hidden = false;
			//toolBtn.SetBackgroundImage(UIImage.FromFile ("min.png"), UIControlState.Normal);
				toolBtn.SetBackgroundImage(UIImage.FromFile ("menu_icon.png"), UIControlState.Normal);//updated afroz 6/8/2016
				toolStatus=false;
			}
			else
			{
				//dropDownView.Hidden=false;//updated afroz 6/8/2016
				//imgArrow.Hidden=false;//updated afroz 6/8/2016
//				meBtn.Enabled = false;
//				meBtn.Hidden = true;
//				watchBtn.Enabled = false;
//				watchBtn.Hidden = true;
//				CalenBtn.Enabled = false;
//				CalenBtn.Hidden = true;	
				//toolBtn.SetBackgroundImage(UIImage.FromFile ("plu.png"), UIControlState.Normal);
				toolBtn.SetBackgroundImage(UIImage.FromFile ("menu_icon_01.png"), UIControlState.Normal);//updated afroz 6/8/2016
				toolStatus=true;
		     }
		}

		partial void WatchBtn_TouchUpInside (UIButton sender)
		{
			//NSUserDefaults.StandardUserDefaults.SetString ("watch", "source");
			//UIcalendar calendarPage = this.Storyboard.InstantiateViewController ("UIcalendar") as UIcalendar;
			//this.NavigationController.PushViewController (calendarPage, true);
			//"http://go-heja.com/gh/mob/sync.php?userId=" + contextPref.GetString ("storedAthId", "0").ToString () + "&mog=gh&url=uurrll"
			
			UIApplication.SharedApplication.OpenUrl(new NSUrl("http://go-heja.com/gh/mob/sync.php?userId=" + NSUserDefaults.StandardUserDefaults.StringForKey("id") + "&mog=nitro&url=uurrll"));
		}

		partial void MeBtn_TouchUpInside (UIButton sender)
		{
			userData controller = this.Storyboard.InstantiateViewController ("userData") as userData;
			this.NavigationController.PushViewController (controller, true);
			
		}

		partial void SelectRunBtn_TouchUpInside (UIButton sender)
		{
			speedTypeLbl.Text="min/km";
			startStopBtn.Enabled = true;
			startStopBtn.Hidden = false;
			selected="run";
			selectBikeBtn.Hidden=true;
			selectBikeBtn.Enabled=false;
			selectRunBtn.Hidden=true;
			selectRunBtn.Enabled=false;
			selectedBtn.SetBackgroundImage(UIImage.FromFile ("runRound_new.png"), UIControlState.Normal);
			selectedBtn.Hidden=false;
			selectedBtn.Enabled=false;
			firstScreenView.Hidden=true;//updated afroz 6/8/2016
			wvOngoing.Hidden=false;//
			Manager.LocationUpdated += HandleLocationChanged;
		}

		partial void SelectBikeBtn_TouchUpInside (UIButton sender)
		{
			wvOngoing.Hidden=true;//
			speedTypeLbl.Text="km/h";
			startStopBtn.Enabled = true;
			startStopBtn.Hidden = false;
			selected="bike";
			selectBikeBtn.Hidden=true;
			selectBikeBtn.Enabled=false;
			selectRunBtn.Hidden=true;
			selectRunBtn.Enabled=false;
			selectedBtn.SetBackgroundImage(UIImage.FromFile ("bikeRound_new.png"), UIControlState.Normal);
			selectedBtn.Hidden=false;
			selectedBtn.Enabled=false;
			firstScreenView.Hidden=true;//updated afroz 6/8/2016
		}
		#endregion
		private int _duration = 0;

		public async void StartTimer1() {
			if (startStop) 
			{
				_duration = 0;
			} else 
			{
				_duration = (int)NSUserDefaults.StandardUserDefaults.IntForKey ("timer");
			}

			// tick every second while game is in progress
			while (!paused) {
				
				await Task.Delay (1000);
				_duration++;
				NSUserDefaults.StandardUserDefaults.SetInt (_duration,"timer");
				string s = TimeSpan.FromSeconds(_duration).ToString(@"hh\:mm\:ss");
			
				timerLbl.Text =s;



			}
		}
		async Task StartTimer() {
			_duration = 0;
			while (true) {

				await Task.Delay (1000);
				if(!paused) _duration++;
				NSUserDefaults.StandardUserDefaults.SetInt (_duration,"timer");
				string s = TimeSpan.FromSeconds(_duration).ToString(@"hh\:mm\:ss");

				timerLbl.Text =s;
			}
		}
	
		private UIImage getImg()//updated afroz 8/8/2016
		{
			var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string jpgFilename = System.IO.Path.Combine (documentsDirectory, "meImg.jpg");

			UIImage currentImage = UIImage.FromFile (jpgFilename);
			return currentImage;

		}

		private void DoneButtonClicked(object sender, EventArgs e)
		{
			BackProcess();
		}

		private void BackProcess()
		{
			_duration = 0;
			NSUserDefaults.StandardUserDefaults.SetInt(0, "timer");
			timerLbl.Text = "";
			startStopBtn.SetBackgroundImage(UIImage.FromFile("go_button.png"), UIControlState.Normal);
			startStopBtn.Hidden = true;
			startStopBtn.Enabled = false;
			selectBikeBtn.Hidden = false;
			selectBikeBtn.Enabled = true;
			selectRunBtn.Hidden = false;
			selectRunBtn.Enabled = true;
			selectedBtn.Hidden = true;
			firstScreenView.Hidden = false;
			btnBack.Hidden = false;
			stopBtn.SetBackgroundImage(UIImage.FromFile(""), UIControlState.Normal);
			stopBtn.Enabled = false;
			startStop = false;
			paused = false;

			//Manager.LocationUpdated += HandleLocationChanged;
			Manager.LocationUpdated -= HandleLocationChanged;
			this.Title = "Nitro ready...";

			try
			{
				meServ.updateMomgoData(NSUserDefaults.StandardUserDefaults.StringForKey("firstName").ToString() + " " + NSUserDefaults.StandardUserDefaults.StringForKey("lastName").ToString(), _lastLocation.Coordinate.Latitude.ToString() + "," + _lastLocation.Coordinate.Longitude.ToString(), _dt, true, NSUserDefaults.StandardUserDefaults.StringForKey("deviceId").ToString(), float.Parse(_lastLocation.Speed.ToString()), true, (NSUserDefaults.StandardUserDefaults.StringForKey("id").ToString()), NSUserDefaults.StandardUserDefaults.StringForKey("country").ToString(), currdistance, true, float.Parse(NSUserDefaults.StandardUserDefaults.DoubleForKey("lastAltitude").ToString()), true, float.Parse(_lastLocation.Course.ToString()), true, 2, true, selected);
			}
			catch
			{
			}
			lblAlt.Text = "0.0";
			//lblDir.Text=" Direction";
			lblDist.Text = "0.00";
			lblSpeed.Text = "0.00";
			this.Title = "Nitro ready..";
			_lastLocation = null;
			_currentDistance = 0;
			_lastAltitude = 0;
			_speed = 0;
			_bearing = "";
			currdistance = 0;
			NSUserDefaults.StandardUserDefaults.SetDouble(0, "lastDistance");
			NSUserDefaults.StandardUserDefaults.SetDouble(0, "lastAltitude");
			flag = 0;
			timerLbl.Text = "";
			startStop = true;
			paused = true;
		}
	}
}

