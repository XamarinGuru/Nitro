using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	public class NitroEventAdapter : BaseAdapter
	{
		List<NitroEvent> _events;
		Activity mSuperActivity;

		public NitroEventAdapter(List<NitroEvent> events, Activity superActivity)
		{
			_events = events;
			mSuperActivity = superActivity;
		}

		public override int Count
		{
			get
			{
				return _events.Count;
			}
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return null;
		}

		override public long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			if (convertView == null)
			{
				convertView = LayoutInflater.From(mSuperActivity).Inflate(Resource.Layout.item_NitroEvent, null);
			}
			convertView.FindViewById(Resource.Id.ActionEventDetail).Click += ActionEventDetail;
			convertView.FindViewById(Resource.Id.ActionEventDetail).Tag = position;
			((TextView)convertView.FindViewById(Resource.Id.txtTitle)).Text = _events[position].title;

			var imgType = convertView.FindViewById<ImageView>(Resource.Id.imgType);
			switch (_events[position].type)
			{
				case "0":
					imgType.SetImageResource(Resource.Drawable.icon_triathlon);
					break;
				case "1":
					imgType.SetImageResource(Resource.Drawable.icon_bike);
					break;
				case "2":
					imgType.SetImageResource(Resource.Drawable.icon_run);
					break;
				case "3":
					imgType.SetImageResource(Resource.Drawable.icon_swim);
					break;
			}

			return convertView;
		}

		void ActionEventDetail(object sender, EventArgs e)
		{
			var index = ((LinearLayout)sender).Tag;
			var selectedEvent = _events[(int)index];

			AppSettings.selectedEvent = selectedEvent;

			mSuperActivity.StartActivity(new Intent(mSuperActivity, typeof(EventInstructionActivity)));
			mSuperActivity.OverridePendingTransition(Resource.Animation.fromLeft, Resource.Animation.toRight);
		}
	}
}
