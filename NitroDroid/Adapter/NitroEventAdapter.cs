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
			((TextView)convertView.FindViewById(Resource.Id.txtTitle)).Text = _events[position].title;

			return convertView;
		}

		void ActionEventDetail(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}
	}
}
