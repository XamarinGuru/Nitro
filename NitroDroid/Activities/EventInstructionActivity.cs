
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

			if (selectedEvent.attended == "1")
				FindViewById(Resource.Id.totalContent).Visibility = ViewStates.Visible;
			else
				FindViewById(Resource.Id.totalContent).Visibility = ViewStates.Gone;

			InitUISettings();
			InitBindingEventData(selectedEvent);
		}

		protected override void OnResume()
		{
			base.OnResume();

			var selectedEvent = AppSettings.selectedEvent;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Retreving Event Details...");

				var eventTotal = GetEventTotals(selectedEvent._id);
				var eventComment = GetComments(selectedEvent._id);

				RunOnUiThread(() =>
				{
					InitBindingEventTotal(eventTotal);
					InitBindingEventComments(eventComment);
				});

				HideLoadingView();
			});
		}

		void InitUISettings()
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
		}

		void InitBindingEventData(NitroEvent selectedEvent)
		{
			var startDateFormats = DateTime.Parse(selectedEvent.start).GetDateTimeFormats();
			FindViewById<TextView>(Resource.Id.lblTitle).Text = selectedEvent.title;
			FindViewById<TextView>(Resource.Id.lblStartDate).Text = startDateFormats[11];
			FindViewById<TextView>(Resource.Id.lblData).Text = selectedEvent.eventData;

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
