
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "PointInfoDialog")]
	public class PointInfoDialog : DialogFragment
	{
		Action<string> callback;
		Point point;

		public static PointInfoDialog newInstance(Point point)
		{
			PointInfoDialog inputDialog = new PointInfoDialog();

			inputDialog.point = point;

			return inputDialog;
		}

		public PointInfoDialog()
		{
			// Required empty public constructor
		}
		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			var dialog = base.OnCreateDialog(savedInstanceState);
			dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
			return dialog;
		}
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var infoView = LayoutInflater.From(this.Activity).Inflate(Resource.Layout.item_PointInfoView, null);

			infoView.FindViewById<TextView>(Resource.Id.lblName).Text = "Name: " + point.name;
			infoView.FindViewById<TextView>(Resource.Id.lblDescription).Text = point.description;
			infoView.FindViewById<TextView>(Resource.Id.lblInterval).Text = "Interval: " + point.interval;

			infoView.FindViewById<ImageView>(Resource.Id.ActionClose).Click += (sender, e) => Dismiss();
			infoView.FindViewById<Button>(Resource.Id.ActionNavigate).Click += (sender, e) => Dismiss();

			return infoView;


			//LinearLayout.LayoutParams params1 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

			//LinearLayout linLayoutV = new LinearLayout(this.Activity);
			//linLayoutV.Orientation = Orientation.Vertical;

			//EditText input = new EditText(this.Activity);
			//linLayoutV.AddView(input);

			//Button okButton = new Button(this.Activity);
			//okButton.Click += (sender, e) =>
			//{
			//	callback(input.Text);
			//	Dismiss();
			//};

			//params1.Gravity = GravityFlags.CenterHorizontal;
			//okButton.LayoutParameters = params1;
			//okButton.Text = "Done";

			//linLayoutV.AddView(okButton);
			//return linLayoutV;

		}
	}
}
