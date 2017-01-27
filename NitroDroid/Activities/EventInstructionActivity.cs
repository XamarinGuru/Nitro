
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "EventInstructionActivity")]
	public class EventInstructionActivity : BaseActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.EventInstructionActivity);

			var selectedEvent = AppSettings.selectedEvent;

			InitUISettings(selectedEvent);
			InitBindingEventData(selectedEvent);
		}

		protected override void OnResume()
		{
			base.OnResume();

			var selectedEvent = AppSettings.selectedEvent;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Retreving Event Details...");

				var eventDetail = GetEventDetail(selectedEvent._id);
				var eventTotal = GetEventTotals(selectedEvent._id);
				var eventComment = GetComments(selectedEvent._id);

				AppSettings.selectedEvent = eventDetail;
				AppSettings.selectedEvent._id = selectedEvent._id;
				AppSettings.currentEventTotal = eventTotal;

				RunOnUiThread(() =>
				{
					InitBindingEventData(eventDetail);
					InitBindingEventTotal(eventTotal);
					InitBindingEventComments(eventComment);
				});

				HideLoadingView();
			});
		}

		void InitUISettings(NitroEvent selectedEvent)
		{
			FindViewById(Resource.Id.btnBack).Click += delegate (object sender, EventArgs e) { OnBack(); };
			FindViewById(Resource.Id.ActionAdjustTrainning).Click += delegate (object sender, EventArgs e)
			{
				var activity = new Intent(this, typeof(AdjustTrainningActivity));
				StartActivity(activity);
			};
			FindViewById(Resource.Id.ActionAddComment).Click += delegate (object sender, EventArgs e) { 
				var activity = new Intent(this, typeof(AddCommentActivity));
				StartActivity(activity);
			};

			if ((DateTime.Parse(selectedEvent.start) - DateTime.Now).TotalMinutes > 1)
			{
				FindViewById(Resource.Id.ActionAdjustTrainning).Visibility = ViewStates.Gone;
				FindViewById(Resource.Id.totalContent).Visibility = ViewStates.Gone;
			}
			else
			{
				FindViewById(Resource.Id.ActionAdjustTrainning).Visibility = ViewStates.Visible;
				FindViewById(Resource.Id.totalContent).Visibility = ViewStates.Visible;
			}
		}

		void InitBindingEventData(NitroEvent selectedEvent)
		{
			var startDateFormats = selectedEvent.StartDateTime().GetDateTimeFormats();
			FindViewById<TextView>(Resource.Id.lblTitle).Text = selectedEvent.title;
			FindViewById<TextView>(Resource.Id.lblStartDate).Text = startDateFormats[11];
			FindViewById<TextView>(Resource.Id.lblData).Text = selectedEvent.eventData;

			var strDistance = selectedEvent.distance;
			float floatDistance = strDistance == "" ? 0 : float.Parse(strDistance);
			var b = Math.Truncate(floatDistance * 100);
			var c = b / 100;
			var formattedDistance = c.ToString("F2");

			FindViewById<TextView>(Resource.Id.lblPDistance).Text = formattedDistance + " KM";

			var durMin = selectedEvent.durMin == "" ? 0 : int.Parse(selectedEvent.durMin);
			var durHrs = selectedEvent.durHrs == "" ? 0 : int.Parse(selectedEvent.durHrs);
			var pHrs = durMin / 60;
			durHrs = durHrs + pHrs;
			durMin = durMin % 60;
			var strDuration = durHrs.ToString() + ":" + durMin.ToString("D2");

			FindViewById<TextView>(Resource.Id.lblPDuration).Text = strDuration;
			FindViewById<TextView>(Resource.Id.lblPLoad).Text = selectedEvent.tss;
			FindViewById<TextView>(Resource.Id.lblPHB).Text = selectedEvent.hb;

			var imgType = FindViewById<ImageView>(Resource.Id.imgType);
			switch (selectedEvent.type)
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
				case "4":
					imgType.SetImageResource(Resource.Drawable.icon_triathlon);
					break;
				case "5":
					imgType.SetImageResource(Resource.Drawable.icon_other);
					break;
			}
		}

		void InitBindingEventTotal(EventTotal eventTotal)
		{
			if (eventTotal == null || eventTotal.totals == null) return;

			FindViewById<TextView>(Resource.Id.lblAvgSpeed).Text = eventTotal.totals[0].value;
			FindViewById<TextView>(Resource.Id.lblTotalDistance).Text = eventTotal.totals[1].value;
			FindViewById<TextView>(Resource.Id.lblElapsedTime).Text = eventTotal.totals[2].value;
			FindViewById<TextView>(Resource.Id.lblTotalAcent).Text = eventTotal.totals[3].value;
			FindViewById<TextView>(Resource.Id.lblAvgHR).Text = eventTotal.totals[4].value;
			FindViewById<TextView>(Resource.Id.lblTotalCalories).Text = eventTotal.totals[5].value;
			FindViewById<TextView>(Resource.Id.lblAvgPower).Text = eventTotal.totals[6].value;
			FindViewById<TextView>(Resource.Id.lblLoad).Text = eventTotal.totals[7].value;
			FindViewById<TextView>(Resource.Id.lblLeveledPower).Text = eventTotal.totals[8].value;
		}

		void InitBindingEventComments(Comment comments)
		{
			var contentComment = FindViewById<LinearLayout>(Resource.Id.contentComment);
			contentComment.RemoveAllViews();
			foreach (var comment in comments.comments)
			{
				var commentView = LayoutInflater.From(this).Inflate(Resource.Layout.item_Comment, null);

				var deltaSecs = float.Parse(comment.date) / 1000;
				var commentDateFormats = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(deltaSecs).ToLocalTime().GetDateTimeFormats();

				FindViewById<TextView>(Resource.Id.lblCommentTitle).Text = "COMMENT" + " (" + comments.comments.Count + ")";
				commentView.FindViewById<TextView>(Resource.Id.lblAuthorName).Text = comment.author;
				commentView.FindViewById<TextView>(Resource.Id.lblCommentDate).Text = commentDateFormats[106] + " | " + commentDateFormats[0];
				commentView.FindViewById<TextView>(Resource.Id.lblComment).Text = comment.commentText;
				contentComment.AddView(commentView);
			}
		}
	}
}
