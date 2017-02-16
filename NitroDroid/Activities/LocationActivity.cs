
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "LocationActivity")]
	public class LocationActivity : BaseActivity, IOnMapReadyCallback, ILocationListener, ActivityCompat.IOnRequestPermissionsResultCallback, GoogleMap.IOnMarkerClickListener
	{
		const int Location_Request_Code = 0;

		EventPoints mEventMarker = new EventPoints();
		private IList<string> pointIDs;

		SupportMapFragment mMapViewFragment;
		GoogleMap mMapView = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.LocationActivity);

			mMapViewFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
			mMapViewFragment.GetMapAsync(this);
		}


		#region google map

		public void OnMapReady(GoogleMap googleMap)
		{
			mMapView = googleMap;

			if (mMapView != null)
			{
				mMapView.SetOnMarkerClickListener(this);

				string[] PermissionsLocation =
				{
					Manifest.Permission.AccessCoarseLocation,
					Manifest.Permission.AccessFineLocation
				};
				//Explain to the user why we need to read the contacts
				ActivityCompat.RequestPermissions(this, PermissionsLocation, Location_Request_Code);

				GetMarkersAndPoints();
			}
		}

		#endregion
		void GetMarkersAndPoints()
		{
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_ALL_MARKERS);

				mEventMarker = GetAllMarkers(AppSettings.selectedEvent._id);
				var trackPoints = GetTrackPoints(AppSettings.selectedEvent._id);

				var mapBounds = new LatLngBounds.Builder();

				LatLng[] path = new LatLng[mEventMarker.markers.Count];

				if (mMapView == null) return;

				pointIDs = new List<string>();

				RunOnUiThread(() =>
				{
					for (int i = 0; i < mEventMarker.markers.Count; i++)
					{
						var point = mEventMarker.markers[i];
						var pointLocation = new LatLng(point.lat, point.lng);

						MarkerOptions markerOpt = new MarkerOptions();
						markerOpt.SetPosition(pointLocation);

						var metrics = Resources.DisplayMetrics;
						var wScreen = metrics.WidthPixels;

						Bitmap bmp = GetPinIconByType(point.type);
						Bitmap newBitmap = ScaleDownImg(bmp, wScreen / 7, true);
						markerOpt.SetIcon(BitmapDescriptorFactory.FromBitmap(newBitmap));

						RunOnUiThread(() =>
						{
							var marker = mMapView.AddMarker(markerOpt);
							pointIDs.Add(marker.Id);

						});
						mapBounds.Include(pointLocation);
						path[i] = pointLocation;
					}

					if (mEventMarker.markers.Count > 0)
					{
						mMapView.MoveCamera(CameraUpdateFactory.NewLatLngBounds(mapBounds.Build(), 50));

						//mMapView.AddPolyline(new PolylineOptions().Add(path).InvokeColor(Color.Red).InvokeWidth(5f));
					}
				});
				HideLoadingView();
			});
		}

		public bool OnMarkerClick(Marker marker)
		{
			PortableLibrary.Point selectedPoint = null;
			for (var i = 0; i < pointIDs.Count; i++)
			{
				if (marker.Id == pointIDs[i])
					selectedPoint = mEventMarker.markers[i];
			}
			if (selectedPoint == null) return false;

			PointInfoDialog myDiag = PointInfoDialog.newInstance(selectedPoint);//(Constants.STR_VERIFY_PASSWORD_TITLE, VerifyPassword);
			myDiag.Show(FragmentManager, "Diag");

			return true;
		}

		#region current location
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			switch (requestCode)
			{
				case Location_Request_Code:
				{
					if (grantResults.Length > 0 && grantResults[0] == (int)Permission.Granted)
					{
						GetMarkersAndPoints();
					}
					else {
						//SetMyLocationOnMap(false);
					}
					return;
				}
			}
		}

		public void OnLocationChanged(Location location){}
		public void OnProviderEnabled(string provider){}
		public void OnStatusChanged(string provider, Availability status, Bundle extras){}

		public void OnProviderDisabled(string provider)
		{
			using (var alert = new AlertDialog.Builder(this))
			{
				alert.SetTitle("Please enable GPS");
				alert.SetMessage("Enable GPS in order to get your current location.");

				alert.SetPositiveButton("Enable", (senderAlert, args) =>
				{
					Intent intent = new Intent(global::Android.Provider.Settings.ActionLocationSourceSettings);
					StartActivity(intent);
				});

				alert.SetNegativeButton("Continue", (senderAlert, args) =>
				{
					alert.Dispose();
				});

				Dialog dialog = alert.Create();
				dialog.Show();
			}
		}

		#endregion

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back)
			{
				var activity = new Intent(this, typeof(EventInstructionActivity));
				StartActivity(activity);
				Finish();
			}

			return base.OnKeyDown(keyCode, e);
		}
	}
}
