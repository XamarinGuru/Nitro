
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using PortableLibrary;

namespace goheja
{
	public class FragmentSerious : Android.Support.V4.App.Fragment
	{
		const int TAG_COLLEPS_PHYSICAL = 101;
		const int TAG_COLLEPS_GOALS = 102;
		const int TAG_COLLEPS_BEST_RESULTS = 103;
		const int TAG_COLLEPS_SELF_RANKING = 104;

		const int TAG_EDIT_PHYSICAL = 1001;
		const int TAG_EDIT_GOALS = 1002;
		const int TAG_EDIT_BEST_RESULTS = 1003;
		const int TAG_EDIT_SELF_RANKING = 1004;
		const int TAG_EDIT_SWIM = 1005;
		const int TAG_EDIT_RUN = 1006;
		const int TAG_EDIT_BIKE = 1007;

		View mView;

		private RootMemberModel MemberModel { get; set; }

		SwipeTabActivity rootActivity;

		TextView lblFirstname, lblLastname, lblCountry, lblAddress, lblBib, lblAge, lblGender, lblBirth, lblEmail, lblPhone;
		EditText txtWeight, txtHeight, txtBMI, txtFatPercentage;
		EditText txtID, txtLastname, txtCountry;
		EditText txtSprint, txtOlympic, txtHDistance, txtDistance, txtMarathon, txtHMarathon, txt10KRun;
		EditText txtRankSwim, txtRankRun, txtRankBike;
		EditText txtSZone1HR, txtSZone2HR, txtSZone3HR, txtSZone4HR, txtSZone5HR;
		EditText txtSZone1PACE, txtSZone2PACE, txtSZone3PACE, txtSZone4PACE, txtSZone5PACE;
		EditText txtSFTPace, txtSFTPHB;
		EditText txtRZone1HR, txtRZone2HR, txtRZone3HR, txtRZone4HR, txtRZone5HR;
		EditText txtRZone1PACE, txtRZone2PACE, txtRZone3PACE, txtRZone4PACE, txtRZone5PACE;
		EditText txtRZone1Power, txtRZone2Power, txtRZone3Power, txtRZone4Power, txtRZone5Power;
		EditText txtRFTPace, txtRFTPHB, txtRFTPower;
		EditText txtBZone1HR, txtBZone2HR, txtBZone3HR, txtBZone4HR, txtBZone5HR;
		EditText txtBZone1POWER, txtBZone2POWER, txtBZone3POWER, txtBZone4POWER, txtBZone5POWER;
		EditText txtBFTPHB, txtBFTPower;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			MemberModel = new RootMemberModel();
			rootActivity = this.Activity as SwipeTabActivity;

			return inflater.Inflate(Resource.Layout.fSerious, container, false);
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);

			MemberModel.rootMember = rootActivity.GetUserObject();

			mView = view;

			SetUIVariablesAndActions();
			SetInputValidation();
			SetInputBinding();
		}

		private void SetUIVariablesAndActions()
		{
			#region UI Variables
			lblFirstname = mView.FindViewById<TextView>(Resource.Id.lblFirstname);
			lblLastname = mView.FindViewById<TextView>(Resource.Id.lblLastname);
			lblCountry = mView.FindViewById<TextView>(Resource.Id.lblCountry);
			lblAddress = mView.FindViewById<TextView>(Resource.Id.lblAddress);
			lblBib = mView.FindViewById<TextView>(Resource.Id.lblBib);
			lblAge = mView.FindViewById<TextView>(Resource.Id.lblAge);
			lblGender = mView.FindViewById<TextView>(Resource.Id.lblGender);
			lblBirth = mView.FindViewById<TextView>(Resource.Id.lblBirth);
			lblEmail = mView.FindViewById<TextView>(Resource.Id.lblEmail);
			lblPhone = mView.FindViewById<TextView>(Resource.Id.lblPhone);

			txtWeight = mView.FindViewById<EditText>(Resource.Id.txtWeight);
			txtHeight = mView.FindViewById<EditText>(Resource.Id.txtHeight);
			txtBMI = mView.FindViewById<EditText>(Resource.Id.txtBMI);
			txtFatPercentage = mView.FindViewById<EditText>(Resource.Id.txtFatPercentage);

			txtID = mView.FindViewById<EditText>(Resource.Id.txtID);
			txtLastname = mView.FindViewById<EditText>(Resource.Id.txtLastname);
			txtCountry = mView.FindViewById<EditText>(Resource.Id.txtCountry);

			txtSprint = mView.FindViewById<EditText>(Resource.Id.txtSprint);
			txtOlympic = mView.FindViewById<EditText>(Resource.Id.txtOlympic);
			txtHDistance = mView.FindViewById<EditText>(Resource.Id.txtHDistance);
			txtDistance = mView.FindViewById<EditText>(Resource.Id.txtDistance);
			txtMarathon = mView.FindViewById<EditText>(Resource.Id.txtMarathon);
			txtHMarathon = mView.FindViewById<EditText>(Resource.Id.txtHMarathon);
			txt10KRun = mView.FindViewById<EditText>(Resource.Id.txt10KRun);

			txtRankSwim = mView.FindViewById<EditText>(Resource.Id.txtRankSwim);
			txtRankRun = mView.FindViewById<EditText>(Resource.Id.txtRankRun);
			txtRankBike = mView.FindViewById<EditText>(Resource.Id.txtRankBike);

			txtSZone1HR = mView.FindViewById<EditText>(Resource.Id.txtSZone1HR);
			txtSZone2HR = mView.FindViewById<EditText>(Resource.Id.txtSZone2HR);
			txtSZone3HR = mView.FindViewById<EditText>(Resource.Id.txtSZone3HR);
			txtSZone4HR = mView.FindViewById<EditText>(Resource.Id.txtSZone4HR);
			txtSZone5HR = mView.FindViewById<EditText>(Resource.Id.txtSZone5HR);

			txtSZone1PACE = mView.FindViewById<EditText>(Resource.Id.txtSZone1PACE);
			txtSZone2PACE = mView.FindViewById<EditText>(Resource.Id.txtSZone2PACE);
			txtSZone3PACE = mView.FindViewById<EditText>(Resource.Id.txtSZone3PACE);
			txtSZone4PACE = mView.FindViewById<EditText>(Resource.Id.txtSZone4PACE);
			txtSZone5PACE = mView.FindViewById<EditText>(Resource.Id.txtSZone5PACE);

			txtSFTPace = mView.FindViewById<EditText>(Resource.Id.txtSFTPace);
			txtSFTPHB = mView.FindViewById<EditText>(Resource.Id.txtSFTPHB);

			txtRZone1HR = mView.FindViewById<EditText>(Resource.Id.txtRZone1HR);
			txtRZone2HR = mView.FindViewById<EditText>(Resource.Id.txtRZone2HR);
			txtRZone3HR = mView.FindViewById<EditText>(Resource.Id.txtRZone3HR);
			txtRZone4HR = mView.FindViewById<EditText>(Resource.Id.txtRZone4HR);
			txtRZone5HR = mView.FindViewById<EditText>(Resource.Id.txtRZone5HR);

			txtRZone1PACE = mView.FindViewById<EditText>(Resource.Id.txtRZone1PACE);
			txtRZone2PACE = mView.FindViewById<EditText>(Resource.Id.txtRZone2PACE);
			txtRZone3PACE = mView.FindViewById<EditText>(Resource.Id.txtRZone3PACE);
			txtRZone4PACE = mView.FindViewById<EditText>(Resource.Id.txtRZone4PACE);
			txtRZone5PACE = mView.FindViewById<EditText>(Resource.Id.txtRZone5PACE);

			txtRZone1Power = mView.FindViewById<EditText>(Resource.Id.txtRZone1Power);
			txtRZone2Power = mView.FindViewById<EditText>(Resource.Id.txtRZone2Power);
			txtRZone3Power = mView.FindViewById<EditText>(Resource.Id.txtRZone3Power);
			txtRZone4Power = mView.FindViewById<EditText>(Resource.Id.txtRZone4Power);
			txtRZone5Power = mView.FindViewById<EditText>(Resource.Id.txtRZone5Power);

			txtRFTPace = mView.FindViewById<EditText>(Resource.Id.txtRFTPace);
			txtRFTPHB = mView.FindViewById<EditText>(Resource.Id.txtRFTPHB);
			txtRFTPower = mView.FindViewById<EditText>(Resource.Id.txtRFTPower);

			txtBZone1HR = mView.FindViewById<EditText>(Resource.Id.txtBZone1HR);
			txtBZone2HR = mView.FindViewById<EditText>(Resource.Id.txtBZone2HR);
			txtBZone3HR = mView.FindViewById<EditText>(Resource.Id.txtBZone3HR);
			txtBZone4HR = mView.FindViewById<EditText>(Resource.Id.txtBZone4HR);
			txtBZone5HR = mView.FindViewById<EditText>(Resource.Id.txtBZone5HR);

			txtBZone1POWER = mView.FindViewById<EditText>(Resource.Id.txtBZone1POWER);
			txtBZone2POWER = mView.FindViewById<EditText>(Resource.Id.txtBZone2POWER);
			txtBZone3POWER = mView.FindViewById<EditText>(Resource.Id.txtBZone3POWER);
			txtBZone4POWER = mView.FindViewById<EditText>(Resource.Id.txtBZone4POWER);
			txtBZone5POWER = mView.FindViewById<EditText>(Resource.Id.txtBZone5POWER);

			txtBFTPHB = mView.FindViewById<EditText>(Resource.Id.txtBFTPHB);
			txtBFTPower = mView.FindViewById<EditText>(Resource.Id.txtBFTPower);
			#endregion

			#region Actions
			mView.FindViewById<RelativeLayout>(Resource.Id.collapsePhysical).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapseGoals).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapseBestResults).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapseSelfRanking).Click += ActionCollepse;

			mView.FindViewById<TextView>(Resource.Id.edtPhysical).Click += ActionEdit;
			mView.FindViewById<TextView>(Resource.Id.edtGoals).Click += ActionEdit;
			mView.FindViewById<TextView>(Resource.Id.edtBestResults).Click += ActionEdit;
			mView.FindViewById<TextView>(Resource.Id.edtSeflRanking).Click += ActionEdit;
			mView.FindViewById<TextView>(Resource.Id.edtSwim).Click += ActionEdit;
			mView.FindViewById<TextView>(Resource.Id.edtRun).Click += ActionEdit;
			mView.FindViewById<TextView>(Resource.Id.edtBike).Click += ActionEdit;

			mView.FindViewById<Button>(Resource.Id.btnUpdate).Click += ActionUpdate;
			#endregion
		}

		private void SetInputValidation()
		{
			SetupPicker(txtSprint, "time");
			SetupPicker(txtOlympic, "time");
			SetupPicker(txtHDistance, "time");
			SetupPicker(txtDistance, "time");
			SetupPicker(txtMarathon, "time");
			SetupPicker(txtHMarathon, "time");
			SetupPicker(txt10KRun, "time");

			SetupPicker(txtRankSwim, "ranking");
			SetupPicker(txtRankRun, "ranking");
			SetupPicker(txtRankBike, "ranking");

			SetupPicker(txtSZone1PACE, "pace");
			SetupPicker(txtSZone2PACE, "pace");
			SetupPicker(txtSZone3PACE, "pace");
			SetupPicker(txtSZone4PACE, "pace");
			SetupPicker(txtSZone5PACE, "pace");
			SetupPicker(txtRZone1PACE, "pace");
			SetupPicker(txtRZone2PACE, "pace");
			SetupPicker(txtRZone3PACE, "pace");
			SetupPicker(txtRZone4PACE, "pace");
			SetupPicker(txtRZone5PACE, "pace");
		}

		private void SetInputBinding()
		{
			#region physical
			this.SetBinding(() => MemberModel.firstname, () => lblFirstname.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.lastname, () => lblLastname.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.country, () => lblCountry.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.address, () => lblAddress.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.bib, () => lblBib.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.age, () => lblAge.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.gender, () => lblGender.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.birth, () => lblBirth.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.email, () => lblEmail.Text, BindingMode.OneWay);
			this.SetBinding(() => MemberModel.phone, () => lblPhone.Text, BindingMode.OneWay);
			#endregion

			#region physical
			this.SetBinding(() => MemberModel.weight, () => txtWeight.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.height, () => txtHeight.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.bmi, () => txtBMI.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.fper, () => txtFatPercentage.Text, BindingMode.TwoWay);
			#endregion

			#region goals
			this.SetBinding(() => MemberModel.id, () => txtID.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.lastname, () => txtLastname.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.country, () => txtCountry.Text, BindingMode.TwoWay);
			#endregion

			#region best results
			this.SetBinding(() => MemberModel.sprint, () => txtSprint.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.olympic, () => txtOlympic.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.hdistance, () => txtHDistance.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.fdistance, () => txtDistance.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.fmarathon, () => txtMarathon.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.hmarathon, () => txtHMarathon.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.krun, () => txt10KRun.Text, BindingMode.TwoWay);
			#endregion

			#region self ranking
			this.SetBinding(() => MemberModel.srSwim, () => txtRankSwim.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.srRun, () => txtRankRun.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.srBike, () => txtRankBike.Text, BindingMode.TwoWay);
			#endregion

			#region swim experience
			this.SetBinding(() => MemberModel.sZone1HR, () => txtSZone1HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sZone2HR, () => txtSZone2HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sZone3HR, () => txtSZone3HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sZone4HR, () => txtSZone4HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sZone5HR, () => txtSZone5HR.Text, BindingMode.TwoWay);

			this.SetBinding(() => MemberModel.sZone1PACE, () => txtSZone1PACE.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sZone2PACE, () => txtSZone2PACE.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sZone3PACE, () => txtSZone3PACE.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sZone4PACE, () => txtSZone4PACE.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sZone5PACE, () => txtSZone5PACE.Text, BindingMode.TwoWay);

			this.SetBinding(() => MemberModel.sFTPace, () => txtSFTPace.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sFTPHB, () => txtSFTPHB.Text, BindingMode.TwoWay);
			#endregion

			#region run experience
			this.SetBinding(() => MemberModel.rZone1HR, () => txtRZone1HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone2HR, () => txtRZone2HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone3HR, () => txtRZone3HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone4HR, () => txtRZone4HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone5HR, () => txtRZone5HR.Text, BindingMode.TwoWay);

			this.SetBinding(() => MemberModel.rZone1PACE, () => txtRZone1PACE.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone2PACE, () => txtRZone2PACE.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone3PACE, () => txtRZone3PACE.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone4PACE, () => txtRZone4PACE.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone5PACE, () => txtRZone5PACE.Text, BindingMode.TwoWay);

			this.SetBinding(() => MemberModel.rZone1POWER, () => txtRZone1Power.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone2POWER, () => txtRZone2Power.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone3POWER, () => txtRZone3Power.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone4POWER, () => txtRZone4Power.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone5POWER, () => txtRZone5Power.Text, BindingMode.TwoWay);

			this.SetBinding(() => MemberModel.rFTPace, () => txtRFTPace.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rFTPHB, () => txtRFTPHB.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rFTPower, () => txtRFTPower.Text, BindingMode.TwoWay);
			#endregion

			#region bike experience
			this.SetBinding(() => MemberModel.bZone1HR, () => txtBZone1HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.bZone2HR, () => txtBZone2HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.bZone3HR, () => txtBZone3HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.bZone4HR, () => txtBZone4HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.bZone5HR, () => txtBZone5HR.Text, BindingMode.TwoWay);

			this.SetBinding(() => MemberModel.bZone1POWER, () => txtBZone1POWER.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.bZone2POWER, () => txtBZone2POWER.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.bZone3POWER, () => txtBZone3POWER.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.bZone4POWER, () => txtBZone4POWER.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.bZone5POWER, () => txtBZone5POWER.Text, BindingMode.TwoWay);

			this.SetBinding(() => MemberModel.bFTPHB, () => txtBFTPHB.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.bFTPower, () => txtBFTPower.Text, BindingMode.TwoWay);
			#endregion
		}

		#region update actions
		void ActionUpdate(object sender, EventArgs e)
		{
			SwipeTabActivity rootVC = (SwipeTabActivity)this.Activity;
			var result = rootActivity.UpdateUserDataJson(MemberModel.rootMember);
			rootVC.ShowMessageBox(null, "updated successfully.");
			rootVC.SetPage(2);
		}
		#endregion

		#region Action Collepse
		void ActionCollepse(object sender, EventArgs e)
		{
			switch (int.Parse(((RelativeLayout)sender).Tag.ToString()))
			{
				case TAG_COLLEPS_PHYSICAL:
					CollepseAnimation(mView.FindViewById<LinearLayout>(Resource.Id.viewPhysical));
					break;
				case TAG_COLLEPS_GOALS:
					CollepseAnimation(mView.FindViewById<LinearLayout>(Resource.Id.viewGoals));
					break;
				case TAG_COLLEPS_BEST_RESULTS:
					CollepseAnimation(mView.FindViewById<LinearLayout>(Resource.Id.viewBestResults));
					break;
				case TAG_COLLEPS_SELF_RANKING:
					CollepseAnimation(mView.FindViewById<LinearLayout>(Resource.Id.viewSelfRankings));
					break;
				default:
					break;
			}
		}

		void CollepseAnimation(LinearLayout content)
		{
			if (content.Visibility.Equals(ViewStates.Gone))
			{
				content.Visibility = ViewStates.Visible;

				int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
				int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
				content.Measure(widthSpec, heightSpec);

				ValueAnimator mAnimator = slideAnimator(0, content.MeasuredHeight, content);
				mAnimator.Start();
			}
			else {
				int finalHeight = content.Height;

				ValueAnimator mAnimator = slideAnimator(finalHeight, 0, content);
				mAnimator.Start();
				mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
				{
					content.Visibility = ViewStates.Gone;
				};
			}
		}

		private ValueAnimator slideAnimator(int start, int end, LinearLayout content)
		{
			ValueAnimator animator = ValueAnimator.OfInt(start, end);
			animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
			{
				var value = (int)animator.AnimatedValue;
				ViewGroup.LayoutParams layoutParams = content.LayoutParameters;
				layoutParams.Height = value;
				content.LayoutParameters = layoutParams;
			};
			return animator;
		}
		#endregion

		#region Action Edit

		void ActionEdit(object sender, EventArgs e)
		{
			switch (int.Parse(((TextView)sender).Tag.ToString()))
			{
				case TAG_EDIT_PHYSICAL:
					txtWeight.Enabled = !txtWeight.Enabled;
					txtHeight.Enabled = !txtHeight.Enabled;
					txtBMI.Enabled = !txtBMI.Enabled;
					txtFatPercentage.Enabled = !txtFatPercentage.Enabled;
					txtWeight.SetBackgroundColor(txtWeight.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);
					txtHeight.SetBackgroundColor(txtWeight.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);
					txtBMI.SetBackgroundColor(txtWeight.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);
					txtFatPercentage.SetBackgroundColor(txtWeight.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);
					break;
				case TAG_EDIT_GOALS:
					txtID.Enabled = !txtID.Enabled;
					txtLastname.Enabled = !txtLastname.Enabled;
					txtCountry.Enabled = !txtCountry.Enabled;
					txtID.SetBackgroundColor(txtID.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtLastname.SetBackgroundColor(txtID.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtCountry.SetBackgroundColor(txtID.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					break;
				case TAG_EDIT_BEST_RESULTS:
					txtSprint.Enabled = !txtSprint.Enabled;
					txtOlympic.Enabled = !txtOlympic.Enabled;
					txtHDistance.Enabled = !txtHDistance.Enabled;
					txtDistance.Enabled = !txtDistance.Enabled;
					txtMarathon.Enabled = !txtMarathon.Enabled;
					txtHMarathon.Enabled = !txtHMarathon.Enabled;
					txt10KRun.Enabled = !txt10KRun.Enabled;
					txtSprint.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtOlympic.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtHDistance.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtDistance.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtMarathon.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtHMarathon.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txt10KRun.SetBackgroundColor(txtSprint.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					break;
				case TAG_EDIT_SELF_RANKING:
					txtRankSwim.Enabled = !txtRankSwim.Enabled;
					txtRankRun.Enabled = !txtRankRun.Enabled;
					txtRankBike.Enabled = !txtRankBike.Enabled;
					txtRankSwim.SetBackgroundColor(txtRankSwim.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRankRun.SetBackgroundColor(txtRankSwim.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRankBike.SetBackgroundColor(txtRankSwim.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					break;
				case TAG_EDIT_SWIM:
					txtSZone1HR.Enabled = !txtSZone1HR.Enabled;
					txtSZone2HR.Enabled = !txtSZone2HR.Enabled;
					txtSZone3HR.Enabled = !txtSZone3HR.Enabled;
					txtSZone4HR.Enabled = !txtSZone4HR.Enabled;
					txtSZone5HR.Enabled = !txtSZone5HR.Enabled;
					txtSZone1PACE.Enabled = !txtSZone1PACE.Enabled;
					txtSZone2PACE.Enabled = !txtSZone2PACE.Enabled;
					txtSZone3PACE.Enabled = !txtSZone3PACE.Enabled;
					txtSZone4PACE.Enabled = !txtSZone4PACE.Enabled;
					txtSZone5PACE.Enabled = !txtSZone5PACE.Enabled;
					txtSFTPace.Enabled = !txtSFTPace.Enabled;
					txtSFTPHB.Enabled = !txtSFTPHB.Enabled;
					txtSZone1HR.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtSZone2HR.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtSZone3HR.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtSZone4HR.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtSZone5HR.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtSZone1PACE.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtSZone2PACE.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtSZone3PACE.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtSZone4PACE.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtSZone5PACE.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtSFTPace.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtSFTPHB.SetBackgroundColor(txtSZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					break;
				case TAG_EDIT_RUN:
					txtRZone1HR.Enabled = !txtRZone1HR.Enabled;
					txtRZone2HR.Enabled = !txtRZone2HR.Enabled;
					txtRZone3HR.Enabled = !txtRZone3HR.Enabled;
					txtRZone4HR.Enabled = !txtRZone4HR.Enabled;
					txtRZone5HR.Enabled = !txtRZone5HR.Enabled;
					txtRZone1PACE.Enabled = !txtRZone1PACE.Enabled;
					txtRZone2PACE.Enabled = !txtRZone2PACE.Enabled;
					txtRZone3PACE.Enabled = !txtRZone3PACE.Enabled;
					txtRZone4PACE.Enabled = !txtRZone4PACE.Enabled;
					txtRZone5PACE.Enabled = !txtRZone5PACE.Enabled;
					txtRZone1Power.Enabled = !txtRZone1Power.Enabled;
					txtRZone2Power.Enabled = !txtRZone2Power.Enabled;
					txtRZone3Power.Enabled = !txtRZone3Power.Enabled;
					txtRZone4Power.Enabled = !txtRZone4Power.Enabled;
					txtRZone5Power.Enabled = !txtRZone5Power.Enabled;
					txtRFTPace.Enabled = !txtRFTPace.Enabled;
					txtRFTPHB.Enabled = !txtRFTPHB.Enabled;
					txtRFTPower.Enabled = !txtRFTPower.Enabled;
					txtRZone1HR.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone2HR.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone3HR.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone4HR.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone5HR.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone1PACE.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone2PACE.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone3PACE.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone4PACE.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone5PACE.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone1Power.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone2Power.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone3Power.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone4Power.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRZone5Power.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRFTPace.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRFTPHB.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtRFTPower.SetBackgroundColor(txtRZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					break;
				case TAG_EDIT_BIKE:
					txtBZone1HR.Enabled = !txtBZone1HR.Enabled;
					txtBZone2HR.Enabled = !txtBZone2HR.Enabled;
					txtBZone3HR.Enabled = !txtBZone3HR.Enabled;
					txtBZone4HR.Enabled = !txtBZone4HR.Enabled;
					txtBZone5HR.Enabled = !txtBZone5HR.Enabled;
					txtBZone1POWER.Enabled = !txtBZone1POWER.Enabled;
					txtBZone2POWER.Enabled = !txtBZone2POWER.Enabled;
					txtBZone3POWER.Enabled = !txtBZone3POWER.Enabled;
					txtBZone4POWER.Enabled = !txtBZone4POWER.Enabled;
					txtBZone5POWER.Enabled = !txtBZone5POWER.Enabled;
					txtBFTPower.Enabled = !txtBFTPower.Enabled;
					txtBFTPHB.Enabled = !txtBFTPHB.Enabled;
					txtBZone1HR.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtBZone2HR.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtBZone3HR.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtBZone4HR.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtBZone5HR.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtBZone1POWER.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtBZone2POWER.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtBZone3POWER.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtBZone4POWER.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtBZone5POWER.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtBFTPower.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					txtBFTPHB.SetBackgroundColor(txtBZone1HR.Enabled ? Android.Graphics.Color.White : Android.Graphics.Color.Gray);;
					break;
				default:
					break;
			}
		}

		#endregion
		private void SetupPicker(EditText textView, string format)
		{
			int dcount = 0;
			if (format == "time")
				dcount = 3;
			else if (format == "pace")
				dcount = 2;
			else
				dcount = 1;
			
			textView.Touch += (object sender, View.TouchEventArgs e) =>
			{
				if (e.Event.Action == MotionEventActions.Down)
				{
					TimeFormatDialog myDiag = TimeFormatDialog.newInstance((EditText)sender, dcount);
					myDiag.Show(this.Activity.FragmentManager, "Diag");
				}
			};
		}
	}
}
