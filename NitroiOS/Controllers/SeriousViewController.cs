﻿using Foundation;
using System;
using UIKit;
using EventKit;
using GalaSoft.MvvmLight.Helpers;
using System.Collections.Generic;

namespace location2
{
	partial class SeriousViewController : BaseViewController
	{
		const int TAG_COLLEPS_PHYSICAL = 130;
		const int TAG_COLLEPS_GOALS = 100;
		const int TAG_COLLEPS_BEST_RESULTS = 220;
		const int TAG_COLLEPS_SELF_RANKING = 1000;

		const int TAG_EDIT_PHYSICAL = 1001;
		const int TAG_EDIT_GOALS = 1002;
		const int TAG_EDIT_BEST_RESULTS = 1003;
		const int TAG_EDIT_SELF_RANKING = 1004;
		const int TAG_EDIT_SWIM = 1005;
		const int TAG_EDIT_RUN = 1006;
		const int TAG_EDIT_BIKE = 1007;

		public SeriousViewController(IntPtr handle) : base(handle)
        {
			MemberModel = new RootMemberModel();
		}
		private RootMemberModel MemberModel { get; set; }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MemberModel.rootMember = GetUserObject();

			var tap = new UITapGestureRecognizer(() => { View.EndEditing(true); });
			View.AddGestureRecognizer(tap);

			SetInputEditingChanged(this.View);
			SetInputValidation();
			SetInputBinding();
		}

		private void SetInputEditingChanged(UIView view)
		{
			UIView[] subviews = view.Subviews;

			if (subviews.Length == 0) return;

			foreach (UIView field in subviews)
			{
				if (field is UITextField)
				{
					((UITextField)field).EditingChanged += (sender, e) => { };
					((UITextField)field).ValueChanged += (sender, e) => { };
				}
			}
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
			this.SetBinding(() => MemberModel.name, () => lblFirstname.Text, BindingMode.OneWay);
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
			this.SetBinding(() => MemberModel.bmi, () => txtBMI.Text,	BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.fper, () => txtFatPercentage.Text,	BindingMode.TwoWay);
			#endregion

			#region goals
			this.SetBinding(() => MemberModel.id, () => txtID.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.lastname, () => txtLastname.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.country, () => txtCountry.Text, BindingMode.TwoWay);
			#endregion

			#region best results
			this.SetBinding(() => MemberModel.sprint,() => txtSprint.Text,BindingMode.OneWay);
			this.SetBinding(() => txtSprint.Text,() => MemberModel.sprint,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");

			this.SetBinding(() => MemberModel.olympic,() => txtOlympic.Text,BindingMode.OneWay);
			this.SetBinding(() => txtOlympic.Text,() => MemberModel.olympic,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");

			this.SetBinding(() => MemberModel.hdistance,() => txtHDistance.Text,BindingMode.OneWay);
			this.SetBinding(() => txtHDistance.Text,() => MemberModel.hdistance,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");

			this.SetBinding(() => MemberModel.fdistance,() => txtDistance.Text,BindingMode.OneWay);
			this.SetBinding(() => txtDistance.Text,() => MemberModel.fdistance,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");

			this.SetBinding(() => MemberModel.fmarathon,() => txtMarathon.Text,BindingMode.OneWay);
			this.SetBinding(() => txtMarathon.Text,() => MemberModel.fmarathon,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");

			this.SetBinding(() => MemberModel.hmarathon,() => txtHMarathon.Text,BindingMode.OneWay);
			this.SetBinding(() => txtHMarathon.Text,() => MemberModel.hmarathon,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");

			this.SetBinding(() => MemberModel.krun,() => txt10KRun.Text,BindingMode.OneWay);
			this.SetBinding(() => txt10KRun.Text,() => MemberModel.krun,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");
			#endregion

			#region self ranking
			this.SetBinding(() => MemberModel.srSwim,() => txtRankSwim.Text,BindingMode.OneWay);
			this.SetBinding(() => txtRankSwim.Text,() => MemberModel.srSwim,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");

			this.SetBinding(() => MemberModel.srRun,() => txtRankRun.Text,BindingMode.OneWay);
			this.SetBinding(() => txtRankRun.Text,() => MemberModel.srRun,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");

			this.SetBinding(() => MemberModel.srBike,() => txtRankBike.Text,BindingMode.OneWay);
			this.SetBinding(() => txtRankBike.Text,() => MemberModel.srBike,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");
			#endregion

			#region swim experience
			this.SetBinding(() => MemberModel.sZone1HR, () => txtSZone1HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sZone2HR, () => txtSZone2HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sZone3HR, () => txtSZone3HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sZone4HR, () => txtSZone4HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sZone5HR, () => txtSZone5HR.Text, BindingMode.TwoWay);

			this.SetBinding(() => MemberModel.sZone1PACE,() => txtSZone1PACE.Text,BindingMode.OneWay);
			this.SetBinding(() => txtSZone1PACE.Text,() => MemberModel.sZone1PACE,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");
			this.SetBinding(() => MemberModel.sZone2PACE,() => txtSZone2PACE.Text,BindingMode.OneWay);
			this.SetBinding(() => txtSZone2PACE.Text,() => MemberModel.sZone2PACE,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");
			this.SetBinding(() => MemberModel.sZone3PACE,() => txtSZone3PACE.Text,BindingMode.OneWay);
			this.SetBinding(() => txtSZone3PACE.Text,() => MemberModel.sZone3PACE,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");
			this.SetBinding(() => MemberModel.sZone4PACE,() => txtSZone4PACE.Text,BindingMode.OneWay);
			this.SetBinding(() => txtSZone4PACE.Text,() => MemberModel.sZone4PACE,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");
			this.SetBinding(() => MemberModel.sZone5PACE,() => txtSZone5PACE.Text,BindingMode.OneWay);
			this.SetBinding(() => txtSZone5PACE.Text,() => MemberModel.sZone5PACE,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");

			this.SetBinding(() => MemberModel.sFTPace, () => txtSFTPace.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.sFTPHB, () => txtSFTPHB.Text, BindingMode.TwoWay);
			#endregion

			#region run experience
			this.SetBinding(() => MemberModel.rZone1HR, () => txtRZone1HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone2HR, () => txtRZone2HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone3HR, () => txtRZone3HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone4HR, () => txtRZone4HR.Text, BindingMode.TwoWay);
			this.SetBinding(() => MemberModel.rZone5HR, () => txtRZone5HR.Text, BindingMode.TwoWay);

			this.SetBinding(() => MemberModel.rZone1PACE,() => txtRZone1PACE.Text,BindingMode.OneWay);
			this.SetBinding(() => txtRZone1PACE.Text,() => MemberModel.rZone1PACE,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");
			this.SetBinding(() => MemberModel.rZone2PACE,() => txtRZone2PACE.Text,BindingMode.OneWay);
			this.SetBinding(() => txtRZone2PACE.Text,() => MemberModel.rZone2PACE,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");
			this.SetBinding(() => MemberModel.rZone3PACE,() => txtRZone3PACE.Text,BindingMode.OneWay);
			this.SetBinding(() => txtRZone3PACE.Text,() => MemberModel.rZone3PACE,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");
			this.SetBinding(() => MemberModel.rZone4PACE,() => txtRZone4PACE.Text,BindingMode.OneWay);
			this.SetBinding(() => txtRZone4PACE.Text,() => MemberModel.rZone4PACE,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");
			this.SetBinding(() => MemberModel.rZone5PACE,() => txtRZone5PACE.Text,BindingMode.OneWay);
			this.SetBinding(() => txtRZone5PACE.Text,() => MemberModel.rZone5PACE,BindingMode.OneWay).ObserveSourceEvent("ValueChanged");
			
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

		#region colleps actions
		partial void ActionCollect(UIButton sender)
		{
			this.View.LayoutIfNeeded();

			UIView.BeginAnimations("ds");
			UIView.SetAnimationDuration(0.5f);

			var constant = sender.Selected ? sender.Tag : 0;
			var alpha = sender.Selected ? 1 : 0;
			switch (sender.Tag)
			{
				case TAG_COLLEPS_PHYSICAL:
					heightPhysical.Constant = constant;
					viewPhysical.Alpha = alpha;
					break;
				case TAG_COLLEPS_GOALS:
					heightGoals.Constant = constant;
					viewGoals.Alpha = alpha;
					break;
				case TAG_COLLEPS_BEST_RESULTS:
					heightBestResults.Constant = constant;
					viewBestResults.Alpha = alpha;
					break;
				case TAG_COLLEPS_SELF_RANKING:
					heightSelfRanking.Constant = constant;
					viewSelfRanking.Alpha = alpha;
					break;
				default:
					break;
			}

			View.LayoutIfNeeded();
			UIView.CommitAnimations();

			sender.Selected = !sender.Selected;
		}
		#endregion

		#region edit actions
		partial void ActionEdit(UIButton sender)
		{
			var backgroundColor = sender.Selected ? UIColor.White : UIColor.Gray;
			switch (sender.Tag)
			{
				case TAG_EDIT_PHYSICAL:
					txtWeight.Enabled = sender.Selected;
					txtHeight.Enabled = sender.Selected;
					txtBMI.Enabled = sender.Selected;
					txtFatPercentage.Enabled = sender.Selected;
					txtWeight.BackgroundColor = backgroundColor;
					txtHeight.BackgroundColor = backgroundColor;
					txtBMI.BackgroundColor = backgroundColor;
					txtFatPercentage.BackgroundColor = backgroundColor;
					break;
				case TAG_EDIT_GOALS:
					txtID.Enabled = sender.Selected;
					txtLastname.Enabled = sender.Selected;
					txtCountry.Enabled = sender.Selected;
					txtID.BackgroundColor = backgroundColor;
					txtLastname.BackgroundColor = backgroundColor;
					txtCountry.BackgroundColor = backgroundColor;
					break;
				case TAG_EDIT_BEST_RESULTS:
					txtSprint.Enabled = sender.Selected;
					txtOlympic.Enabled = sender.Selected;
					txtHDistance.Enabled = sender.Selected;
					txtDistance.Enabled = sender.Selected;
					txtMarathon.Enabled = sender.Selected;
					txtHMarathon.Enabled = sender.Selected;
					txt10KRun.Enabled = sender.Selected;
					txtSprint.BackgroundColor = backgroundColor;
					txtOlympic.BackgroundColor = backgroundColor;
					txtHDistance.BackgroundColor = backgroundColor;
					txtDistance.BackgroundColor = backgroundColor;
					txtMarathon.BackgroundColor = backgroundColor;
					txtHMarathon.BackgroundColor = backgroundColor;
					txt10KRun.BackgroundColor = backgroundColor;
					break;
				case TAG_EDIT_SELF_RANKING:
					txtRankSwim.Enabled = sender.Selected;
					txtRankRun.Enabled = sender.Selected;
					txtRankBike.Enabled = sender.Selected;
					txtRankSwim.BackgroundColor = backgroundColor;
					txtRankRun.BackgroundColor = backgroundColor;
					txtRankBike.BackgroundColor = backgroundColor;
					break;
				case TAG_EDIT_SWIM:
					txtSZone1HR.Enabled = sender.Selected;
					txtSZone2HR.Enabled = sender.Selected;
					txtSZone3HR.Enabled = sender.Selected;
					txtSZone4HR.Enabled = sender.Selected;
					txtSZone5HR.Enabled = sender.Selected;
					txtSZone1PACE.Enabled = sender.Selected;
					txtSZone2PACE.Enabled = sender.Selected;
					txtSZone3PACE.Enabled = sender.Selected;
					txtSZone4PACE.Enabled = sender.Selected;
					txtSZone5PACE.Enabled = sender.Selected;
					txtSFTPace.Enabled = sender.Selected;
					txtSFTPHB.Enabled = sender.Selected;
					txtSZone1HR.BackgroundColor = backgroundColor;
					txtSZone2HR.BackgroundColor = backgroundColor;
					txtSZone3HR.BackgroundColor = backgroundColor;
					txtSZone4HR.BackgroundColor = backgroundColor;
					txtSZone5HR.BackgroundColor = backgroundColor;
					txtSZone1PACE.BackgroundColor = backgroundColor;
					txtSZone2PACE.BackgroundColor = backgroundColor;
					txtSZone3PACE.BackgroundColor = backgroundColor;
					txtSZone4PACE.BackgroundColor = backgroundColor;
					txtSZone5PACE.BackgroundColor = backgroundColor;
					txtSFTPace.BackgroundColor = backgroundColor;
					txtSFTPHB.BackgroundColor = backgroundColor;
					break;
				case TAG_EDIT_RUN:
					txtRZone1HR.Enabled = sender.Selected;
					txtRZone2HR.Enabled = sender.Selected;
					txtRZone3HR.Enabled = sender.Selected;
					txtRZone4HR.Enabled = sender.Selected;
					txtRZone5HR.Enabled = sender.Selected;
					txtRZone1PACE.Enabled = sender.Selected;
					txtRZone2PACE.Enabled = sender.Selected;
					txtRZone3PACE.Enabled = sender.Selected;
					txtRZone4PACE.Enabled = sender.Selected;
					txtRZone5PACE.Enabled = sender.Selected;
					txtRZone1Power.Enabled = sender.Selected;
					txtRZone2Power.Enabled = sender.Selected;
					txtRZone3Power.Enabled = sender.Selected;
					txtRZone4Power.Enabled = sender.Selected;
					txtRZone5Power.Enabled = sender.Selected;
					txtRFTPace.Enabled = sender.Selected;
					txtRFTPHB.Enabled = sender.Selected;
					txtRFTPower.Enabled = sender.Selected;
					txtRZone1HR.BackgroundColor = backgroundColor;
					txtRZone2HR.BackgroundColor = backgroundColor;
					txtRZone3HR.BackgroundColor = backgroundColor;
					txtRZone4HR.BackgroundColor = backgroundColor;
					txtRZone5HR.BackgroundColor = backgroundColor;
					txtRZone1PACE.BackgroundColor = backgroundColor;
					txtRZone2PACE.BackgroundColor = backgroundColor;
					txtRZone3PACE.BackgroundColor = backgroundColor;
					txtRZone4PACE.BackgroundColor = backgroundColor;
					txtRZone5PACE.BackgroundColor = backgroundColor;
					txtRZone1Power.BackgroundColor = backgroundColor;
					txtRZone2Power.BackgroundColor = backgroundColor;
					txtRZone3Power.BackgroundColor = backgroundColor;
					txtRZone4Power.BackgroundColor = backgroundColor;
					txtRZone5Power.BackgroundColor = backgroundColor;
					txtRFTPace.BackgroundColor = backgroundColor;
					txtRFTPHB.BackgroundColor = backgroundColor;
					txtRFTPower.BackgroundColor = backgroundColor;
					break;
				case TAG_EDIT_BIKE:
					txtBZone1HR.Enabled = sender.Selected;
					txtBZone2HR.Enabled = sender.Selected;
					txtBZone3HR.Enabled = sender.Selected;
					txtBZone4HR.Enabled = sender.Selected;
					txtBZone5HR.Enabled = sender.Selected;
					txtBZone1POWER.Enabled = sender.Selected;
					txtBZone2POWER.Enabled = sender.Selected;
					txtBZone3POWER.Enabled = sender.Selected;
					txtBZone4POWER.Enabled = sender.Selected;
					txtBZone5POWER.Enabled = sender.Selected;
					txtBFTPower.Enabled = sender.Selected;
					txtBFTPHB.Enabled = sender.Selected;
					txtBZone1HR.BackgroundColor = backgroundColor;
					txtBZone2HR.BackgroundColor = backgroundColor;
					txtBZone3HR.BackgroundColor = backgroundColor;
					txtBZone4HR.BackgroundColor = backgroundColor;
					txtBZone5HR.BackgroundColor = backgroundColor;
					txtBZone1POWER.BackgroundColor = backgroundColor;
					txtBZone2POWER.BackgroundColor = backgroundColor;
					txtBZone3POWER.BackgroundColor = backgroundColor;
					txtBZone4POWER.BackgroundColor = backgroundColor;
					txtBZone5POWER.BackgroundColor = backgroundColor;
					txtBFTPower.BackgroundColor = backgroundColor;
					txtBFTPHB.BackgroundColor = backgroundColor;
					break;
				default:
					break;
			}

			sender.Selected = !sender.Selected;
		}
		#endregion

		#region update actions
		partial void ActionUpdate(UIButton sender)
		{
			var result = UpdateUserDataJson(MemberModel.rootMember);
		}
		#endregion
	}
}

