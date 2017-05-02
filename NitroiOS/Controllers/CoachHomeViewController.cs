using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using PortableLibrary;
using System.Linq;

namespace location2
{
	public partial class CoachHomeViewController : BaseViewController
	{
		List<User> _users = new List<User>();

		public CoachHomeViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			InitUISettings();

			if (!IsNetEnable()) return;

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.MSG_LOADING_DATA);

				for (int i = 0; i < 10; i++)
				{
					var tmp = new User();
					tmp.name = "Sagiv Daniel" + i;

					_users.Add(tmp);
				}

				var tblDataSource = new UserTableViewSource(_users, this);

				InvokeOnMainThread(() =>
				{
					tableView.Source = tblDataSource;
					tableView.ReloadData();
					HideLoadingView();
				});
			});
		}

		void InitUISettings()
		{
			txtSearch.EditingChanged += ActionSearch;

			//lblTSB.TextColor = GROUP_COLOR;
			//lblCTL.TextColor = GROUP_COLOR;
			//lblATL.TextColor = GROUP_COLOR;
			//lblLoad.TextColor = GROUP_COLOR;
			//lblNoEvents.TextColor = GROUP_COLOR;O
		}

		void ActionSearch(object sender, EventArgs e)
		{
			(tableView.Source as UserTableViewSource).PerformSearch((sender as UITextField).Text);
			tableView.ReloadData();  
		}

//		bool bbb(UITextField textField, NSRange range, string replacementString)
//		{
//			(tableView.Source as UserTableViewSource).PerformSearch(textField.Text);
//tableView.ReloadData();
//			return true;
//		}

//		void aaa(object sender, EventArgs e)
//		{
//			(tableView.Source as UserTableViewSource).PerformSearch((sender as UITextField).Text);
//tableView.ReloadData();  
//		}

		partial void ActionGoToGroup(UIButton sender)
		{
			//throw new NotImplementedException();
		}

		#region UserTableViewSource
		class UserTableViewSource : UITableViewSource
		{
			List<User> _users;
			List<User> _searchUsers;
			CoachHomeViewController _coachHomeVC;

			public UserTableViewSource(List<User> users, CoachHomeViewController coachHomeVC)
			{
				_users = new List<User>();

				if (users == null) return;

				_users = users;
				_searchUsers = users;
				_coachHomeVC = coachHomeVC;
			}
			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return _searchUsers.Count;
			}

			public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
			{
				return 50;
			}
			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				UserCell cell = tableView.DequeueReusableCell("UserCell") as UserCell;
				cell.SetCell(_searchUsers[indexPath.Row]);

				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				if (!_coachHomeVC.IsNetEnable()) return;

				var selectedEvent = _searchUsers[indexPath.Row];
				UIStoryboard sb = UIStoryboard.FromName("Main", null);
				//EventInstructionController eventInstructionVC = sb.InstantiateViewController("EventInstructionController") as EventInstructionController;
				//eventInstructionVC.selectedEvent = selectedEvent;
				//_coachHomeVC.NavigationController.PushViewController(eventInstructionVC, true);
			}

			public void PerformSearch(string strSearch)
			{
				strSearch = strSearch.ToLower();
				_searchUsers = _users.Where(x => x.name.ToLower().Contains(strSearch)).ToList();
			}
		}
		#endregion
	}
}