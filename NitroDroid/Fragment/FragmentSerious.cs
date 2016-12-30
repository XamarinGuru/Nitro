
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

			mView.FindViewById<RelativeLayout>(Resource.Id.collapsePhysical).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapseGoals).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapseBestResults).Click += ActionCollepse;
			mView.FindViewById<RelativeLayout>(Resource.Id.collapseSelfRanking).Click += ActionCollepse;
		}

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
	}
}
