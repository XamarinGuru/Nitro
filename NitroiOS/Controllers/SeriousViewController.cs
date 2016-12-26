using Foundation;
using System;
using UIKit;
using EventKit;

namespace location2
{
	partial class SeriousViewController : BaseViewController
	{
		const int TAG_PHYSICAL = 130;
		const int TAG_GOALS = 100;
		const int TAG_BEST_RESULTS = 220;
		const int TAG_SELF_RANKING = 1000;


		public SeriousViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			SetInputValidation();
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

			SetupPicker(txtSZone1HR, "HR");
			SetupPicker(txtSZone2HR, "HR");
			SetupPicker(txtSZone3HR, "HR");
			SetupPicker(txtSZone4HR, "HR");
			SetupPicker(txtSZone5HR, "HR");
			SetupPicker(txtRZone1HR, "HR");
			SetupPicker(txtRZone2HR, "HR");
			SetupPicker(txtRZone3HR, "HR");
			SetupPicker(txtRZone4HR, "HR");
			SetupPicker(txtRZone5HR, "HR");
			SetupPicker(txtBZone1HR, "HR");
			SetupPicker(txtBZone2HR, "HR");
			SetupPicker(txtBZone3HR, "HR");
			SetupPicker(txtBZone4HR, "HR");
			SetupPicker(txtBZone5HR, "HR");

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
				case TAG_PHYSICAL:
					heightPhysical.Constant = constant;
					viewPhysical.Alpha = alpha;
					break;
				case TAG_GOALS:
					heightGoals.Constant = constant;
					viewGoals.Alpha = alpha;
					break;
				case TAG_BEST_RESULTS:
					heightBestResults.Constant = constant;
					viewBestResults.Alpha = alpha;
					break;
				case TAG_SELF_RANKING:
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
	}
}

