
using System;

using Android.App;
using Android.OS;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "ActionAdjustTrainning")]
	public class AdjustTrainningActivity : BaseActivity
	{
		private RootMemberModel MemberModel = new RootMemberModel();

		TextView lblTime, lblDistance, lblTSS;
		EditText txtComment;
		SeekBar seekTime, seekDistance, seekTSS;
		CheckBox attended;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.AdjustTrainningActivity);

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Retreving Event Details...");

				MemberModel.rootMember = GetUserObject();

				HideLoadingView();
			});

			InitUISettings();
			InitBindingEventTotal();
		}

		void InitUISettings()
		{
			attended = FindViewById<CheckBox>(Resource.Id.checkAttended);

			lblTime = FindViewById<TextView>(Resource.Id.lblTime);
			lblDistance = FindViewById<TextView>(Resource.Id.lblDistance);
			lblTSS = FindViewById<TextView>(Resource.Id.lblTSS);
			txtComment = FindViewById<EditText>(Resource.Id.txtComment);

			seekTime = FindViewById<SeekBar>(Resource.Id.ActionTimeChanged);
			seekDistance = FindViewById<SeekBar>(Resource.Id.ActionDistanceChanged);
			seekTSS = FindViewById<SeekBar>(Resource.Id.ActionTSSChanged);

			seekTime.ProgressChanged += (sender, e) => { lblTime.Text = ((SeekBar)sender).Progress.ToString(); };
			seekDistance.ProgressChanged += (sender, e) => { lblDistance.Text = ((SeekBar)sender).Progress.ToString(); };
			seekTSS.ProgressChanged += (sender, e) => { lblTSS.Text = ((SeekBar)sender).Progress.ToString(); };
			FindViewById(Resource.Id.ActionAdjustTrainning).Click += ActionAdjustTrainning;
		}

		void InitBindingEventTotal()
		{
			var eventTotal = AppSettings.currentEventTotal;

			if (eventTotal == null || eventTotal.totals == null) return;

			var distance = GetFormatedDurationAsMin(eventTotal.totals[2].value);

			lblTime.Text = distance.ToString();
			lblDistance.Text = eventTotal.totals[1].value;
			lblTSS.Text = eventTotal.totals[7].value;

			seekTime.Progress = distance;
			seekDistance.Progress = int.Parse(eventTotal.totals[1].value);
			seekTSS.Progress = int.Parse(eventTotal.totals[7].value);

			attended.Checked = AppSettings.selectedEvent.attended == "1" ? true : false;
		}

		void ActionAdjustTrainning(object sender, EventArgs e)
		{
			//if (txtComment.Text == "")
			//{
			//	ShowMessageBox(null, "Type your comment...");
			//	return;
			//}

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Adjusting Trainning...");

				UpdateMemberNotes(txtComment.Text, AppSettings.UserID, AppSettings.selectedEvent._id, MemberModel.username, attended.Checked ? "1" : "0", lblTime.Text, lblDistance.Text, lblTSS.Text, AppSettings.selectedEvent.type);

				HideLoadingView();

				Finish();
			});
		}
	}
}
