using Foundation;
using System;
using UIKit;
using CoreGraphics;
using PortableLibrary;
using Google.Maps;
using System.Drawing;
using CoreLocation;

namespace location2
{
    public partial class LocationViewController : BaseViewController
    {
		public string eventID;
		private MapView mMapView;

		EventPoints mEventMarker = new EventPoints();

        public LocationViewController() : base()
		{
		}
		public LocationViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			NavigationItem.HidesBackButton = true;

			var leftButton = new UIButton(new CGRect(0, 0, 20, 20));
			leftButton.SetImage(UIImage.FromFile("icon_left.png"), UIControlState.Normal);
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

			if (!IsNetEnable()) return;

			InitMapView();

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(PortableLibrary.Constants.MSG_LOADING_ALL_MARKERS);

				mEventMarker = GetAllMarkers(eventID);
				var trackPoints = GetTrackPoints(eventID);

				var mapBounds = new CoordinateBounds();

				var polyline = new Polyline();
				var path = new MutablePath();

				InvokeOnMainThread(() =>
				{
					for (int i = 0; i < mEventMarker.markers.Count; i++)
					{
						var point = mEventMarker.markers[i];
						var imgPin = GetPinIconByType(point.type);
						var pointLocation = new CLLocationCoordinate2D(point.lat, point.lng);

						var marker = new Marker
						{
							Position = pointLocation,
							Map = mMapView,
							Icon = imgPin,
							ZIndex = i
						};

						mapBounds = mapBounds.Including(pointLocation);
						path.AddCoordinate(pointLocation);
					}

					if (mEventMarker.markers.Count > 0)
					{
						mMapView.MoveCamera(CameraUpdate.FitBounds(mapBounds, 50.0f));

						polyline.Path = path;
						polyline.StrokeColor = UIColor.Red;
						polyline.StrokeWidth = 2;
						polyline.Map = mMapView;
					}
				});
				HideLoadingView();
			});
		}

		public override void ViewWillLayoutSubviews()
		{
			if (mMapView != null && viewMapContent != null && viewMapContent.Window != null)
			{
				RepaintMap();
			}
		}

		void InitMapView()
		{
			var camera = CameraPosition.FromCamera(31.0461, 34.8516, zoom: PortableLibrary.Constants.MAP_ZOOM_LEVEL);
			mMapView = MapView.FromCamera(RectangleF.Empty, camera);
			mMapView.MyLocationEnabled = false;

			mMapView.TappedMarker = ClickedDropItem;
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
		}

		#region map pin click event
		bool ClickedDropItem(MapView mapView, Marker marker)
		{
			var selectedPoint = mEventMarker.markers[marker.ZIndex];

			PointInfoView cpuv = PointInfoView.Create(selectedPoint);
			cpuv.PopUp(true, delegate
			{
				Console.WriteLine("cpuv will close");
			});

			return true;
		}
		#endregion
    }
}