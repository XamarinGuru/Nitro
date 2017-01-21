
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
		}

		void InitUISettings()
		{
			lblTime = FindViewById<TextView>(Resource.Id.lblTime);
			lblDistance = FindViewById<TextView>(Resource.Id.lblDistance);
			lblTSS = FindViewById<TextView>(Resource.Id.lblTSS);
			txtComment = FindViewById<EditText>(Resource.Id.txtComment);
			attended = FindViewById<CheckBox>(Resource.Id.checkAttended);

			FindViewById<SeekBar>(Resource.Id.ActionTimeChanged).ProgressChanged += (sender, e) => { lblTime.Text = ((SeekBar)sender).Progress.ToString(); };
			FindViewById<SeekBar>(Resource.Id.ActionDistanceChanged).ProgressChanged += (sender, e) => { lblDistance.Text = ((SeekBar)sender).Progress.ToString(); };
			FindViewById<SeekBar>(Resource.Id.ActionTSSChanged).ProgressChanged += (sender, e) => { lblTSS.Text = ((SeekBar)sender).Progress.ToString(); };
			FindViewById(Resource.Id.ActionAdjustTrainning).Click += ActionAdjustTrainning;
		}

		void ActionAdjustTrainning(object sender, EventArgs e)
		{
			if (txtComment.Text == "")
			{
				ShowMessageBox(null, "Type your comment...");
				return;
			}

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
