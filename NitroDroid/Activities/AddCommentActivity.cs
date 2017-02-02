
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
	[Activity(Label = "AddCommentActivity")]
	public class AddCommentActivity : BaseActivity
	{
		private RootMemberModel MemberModel = new RootMemberModel();

		EditText txtComment;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.AddCommentActivity);

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Loading data...");

				MemberModel.rootMember = GetUserObject();

				HideLoadingView();
			});

			InitUISettings();
		}

		void InitUISettings()
		{
			txtComment = FindViewById<EditText>(Resource.Id.txtComment);
			FindViewById(Resource.Id.ActionAddComment).Click += ActionAddComment;
		}

		void ActionAddComment(object sender, EventArgs e)
		{
			if (txtComment.Text == "")
			{
				ShowMessageBox(null, "Type your comment...");
				return;
			}

			var author = MemberModel.firstname + " " + MemberModel.lastname;
			var authorID = AppSettings.UserID;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView("Saving your comment...");

				var response = SetComment(author, authorID, txtComment.Text, AppSettings.selectedEvent._id);

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
