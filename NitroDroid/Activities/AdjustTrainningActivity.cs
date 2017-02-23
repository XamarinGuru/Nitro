
using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "ActionAdjustTrainning", ScreenOrientation = ScreenOrientation.Portrait)]
	public class AdjustTrainningActivity : BaseActivity
	{
		private RootMemberModel MemberModel = new RootMemberModel();

		TextView lblTime, lblDistance, lblTSS, lblType;
		EditText txtComment;
		SeekBar seekTime, seekDistance, seekTSS;
		CheckBox attended;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.AdjustTrainningActivity);

			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_EVENT_DETAIL);

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
			lblType = FindViewById<TextView>(Resource.Id.lblType);
			txtComment = FindViewById<EditText>(Resource.Id.txtComment);

			seekTime = FindViewById<SeekBar>(Resource.Id.ActionTimeChanged);
			seekDistance = FindViewById<SeekBar>(Resource.Id.ActionDistanceChanged);
			seekTSS = FindViewById<SeekBar>(Resource.Id.ActionTSSChanged);

			seekTime.ProgressChanged += (sender, e) => { lblTime.Text = ((SeekBar)sender).Progress.ToString(); };
			seekDistance.ProgressChanged += (sender, e) => { lblDistance.Text = ((SeekBar)sender).Progress.ToString(); };
			seekTSS.ProgressChanged += (sender, e) => { lblTSS.Text = ((SeekBar)sender).Progress.ToString(); };
			FindViewById(Resource.Id.ActionAdjustTrainning).Click += ActionAdjustTrainning;

			SetupAdjustPicker(lblTime, seekTime, 360);
			SetupAdjustPicker(lblDistance, seekDistance, 250);
			SetupAdjustPicker(lblTSS, seekTSS, 400);
			SetupAdjustPicker(lblType, null, 4, true);


		}
		void SetupAdjustPicker(TextView textView, SeekBar seekBar, int maxValue, bool isType = false)
		{
			textView.Touch += (object sender, View.TouchEventArgs e) =>
			{
				if (e.Event.Action == MotionEventActions.Down)
				{
					AdjustDialog myDiag = AdjustDialog.newInstance((TextView)sender, seekBar, maxValue, isType);
					myDiag.Show(FragmentManager, "Diag");
				}
			};
		}

		void InitBindingEventTotal()
		{
			var eventTotal = AppSettings.currentEventTotal;

			attended.Checked = AppSettings.selectedEvent.attended == "1" ? true : false;

			if (eventTotal == null || eventTotal.totals == null) return;

			var strEt = GetFormatedDurationAsMin(eventTotal.GetValue(Constants.TOTALS_ES_TIME));
			var strTd = eventTotal.GetValue(Constants.TOTALS_DISTANCE);
			var strTss = eventTotal.GetValue(Constants.TOTALS_LOAD);

			lblTime.Text = strEt.ToString();
			lblDistance.Text = float.Parse(strTd).ToString("F0");
			lblTSS.Text = float.Parse(strTss).ToString("F0");

			seekTime.Progress = strEt;
			seekDistance.Progress = (int)float.Parse(strTd);
			seekTSS.Progress = (int)float.Parse(strTss);

			lblType.Text = GetTypeStrFromID(AppSettings.selectedEvent.type);
		}

		void ActionAdjustTrainning(object sender, EventArgs e)
		{
			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_ADJUST_TRAINING);

				var type = (Array.IndexOf(Constants.PRACTICE_TYPES, lblType.Text) + 1).ToString();

				UpdateMemberNotes(txtComment.Text, AppSettings.UserID, AppSettings.selectedEvent._id, MemberModel.username, attended.Checked ? "1" : "0", lblTime.Text, lblDistance.Text, lblTSS.Text, type);

				HideLoadingView();

				var activity = new Intent(this, typeof(EventInstructionActivity));
				StartActivity(activity);
				Finish();
			});
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
