
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Views;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "LocationActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class LocationActivity : BaseActivity, IOnMapReadyCallback, GoogleMap.IOnMarkerClickListener
	{
		const int Location_Request_Code = 0;

		EventPoints mEventMarker = new EventPoints();
		IList<string> pointIDs;

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

				if (mMapView == null) return;

				pointIDs = new List<string>();

				RunOnUiThread(() =>
				{
					for (int i = 0; i < mEventMarker.markers.Count; i++)
					{
						var point = mEventMarker.markers[i];
						var pointLocation = new LatLng(point.lat, point.lng);

						//MarkerOptions markerOpt = new MarkerOptions();
						//markerOpt.SetPosition(pointLocation);

						//var metrics = Resources.DisplayMetrics;
						//var wScreen = metrics.WidthPixels;

						//Bitmap bmp = GetPinIconByType(point.type);
						//Bitmap newBitmap = ScaleDownImg(bmp, wScreen / 7, true);
						//markerOpt.SetIcon(BitmapDescriptorFactory.FromBitmap(newBitmap));

						//RunOnUiThread(() =>
						//{
						//	var marker = mMapView.AddMarker(markerOpt);
						//	pointIDs.Add(marker.Id);
						//});
						mapBounds.Include(pointLocation);

						AddMapPin(pointLocation, point.type);
					}

					if (trackPoints != null && trackPoints.Count > 0)
					{
						foreach (var tPoints in trackPoints)
						{
							List<LatLng> paths = new List<LatLng>();
							foreach (var point in tPoints)
							{
								var pointLocation = new LatLng(point.Latitude, point.Longitude);
								paths.Add(pointLocation);
								mapBounds.Include(pointLocation);

								if (point.lapImage == "Start")
								{
									AddMapPin(pointLocation, "pSTART");
								}
								else if (point.lapImage == "Totals")
								{
									AddMapPin(pointLocation, "pFINISH");
								}
							}

							LatLng[] arrPath = new LatLng[paths.Count];
							for (var i = 0; i < paths.Count; i++)
								arrPath[i] = paths[i];

							mMapView.AddPolyline(new PolylineOptions().Add(arrPath).InvokeColor(GetRandomColor()).InvokeWidth(5f));
						}
					}

					mMapView.MoveCamera(CameraUpdateFactory.NewLatLngBounds(mapBounds.Build(), 50));
				});
				HideLoadingView();
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
