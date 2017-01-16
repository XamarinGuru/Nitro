using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using PortableLibrary;

namespace location2
{
	public partial class MainPageViewController : BaseViewController
	{
		private UIPageViewController mPageViewController;
		List<BaseViewController> subControllers = new List<BaseViewController>();

		public MainPageViewController() : base()
		{
		}
		public MainPageViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			AppDelegate myDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
			myDelegate.baseVC = this;

			var calendarVC = (BaseViewController)this.Storyboard.InstantiateViewController("CalendarViewController");
			var analyticsVC = (BaseViewController)this.Storyboard.InstantiateViewController("AnalyticsViewController");
			var profileVC = (BaseViewController)this.Storyboard.InstantiateViewController("ProfileViewController");
			var seriousVC = (BaseViewController)this.Storyboard.InstantiateViewController("SeriousViewController");
			calendarVC.pageIndex = 0;
			analyticsVC.pageIndex = 1;
			profileVC.pageIndex = 2;
			seriousVC.pageIndex = 3;
			profileVC.rootVC = this;
			seriousVC.rootVC = this;

			subControllers.Add(calendarVC);
			subControllers.Add(analyticsVC);
			subControllers.Add(profileVC);
			subControllers.Add(seriousVC);

			mPageViewController = this.Storyboard.InstantiateViewController("PageViewController") as UIPageViewController;
			mPageViewController.DataSource = new PageDataSource(this, subControllers);

			var startVC = this.ViewControllerAtIndex(0) as BaseViewController;
			var viewControllers = new UIViewController[] { startVC };

			mPageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, false, null);
			mPageViewController.View.Frame = pageContent.Bounds;
			AddChildViewController(this.mPageViewController);
			pageContent.AddSubview(this.mPageViewController.View);
			mPageViewController.DidMoveToParentViewController(this);

			foreach (UIScrollView view in mPageViewController.View.Subviews)
			{
				if (view is UIScrollView)
					view.ScrollEnabled = false;
			}

			mPageViewController.DidFinishAnimating += delegate (object sender, UIPageViewFinishedAnimationEventArgs e)
			{
				var con = (UIPageViewController)sender;
				var con1 = (BaseViewController) con.ViewControllers[con.ViewControllers.Length-1];
				var index = con1.pageIndex;
				TabBarAnimation(index);
				var dir = con1.ModalTransitionStyle;
			};

			btnCalendar.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnCalendar.TouchUpInside += (sender, e) => { SetCurrentPage(0); };

			btnHome.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnHome.TouchUpInside += (sender, e) => { SetCurrentPage(1); };

			btnProfile.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnProfile.TouchUpInside += (sender, e) => { SetCurrentPage(2); };
		}



		public void SetCurrentPage(int pIndex)
		{
			var startVC = this.ViewControllerAtIndex(pIndex) as BaseViewController;
			var viewControllers = new UIViewController[] { startVC };

			var con1 = (BaseViewController)mPageViewController.ViewControllers[0];
			var cIndex = con1.pageIndex;

			if (cIndex == pIndex) return;

			var direction = cIndex > pIndex ? UIPageViewControllerNavigationDirection.Reverse : UIPageViewControllerNavigationDirection.Forward;

			mPageViewController.SetViewControllers(viewControllers, direction, true, null);
			TabBarAnimation(pIndex);
		}

		public BaseViewController ViewControllerAtIndex(int index)
		{
			return subControllers[index];
		}

		public void TabBarAnimation(int pageNumber)
		{
			if (pageNumber == 3) return;

			btnCalendar.Selected = false;
			btnHome.Selected = false;
			btnProfile.Selected = false;

			switch (pageNumber)
			{
				case 0:
					btnCalendar.Selected = true;
					break;
				case 1:
					btnHome.Selected = true;
					break;
				case 2:
					btnProfile.Selected = true;
					break;
			}
		}
		private void WatchButtonClicked(object sender, EventArgs e)
		{
			var url = String.Format(Constants.WATCH_URL, AppSettings.UserID);
			UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
		}
	}


	public class PageDataSource : UIPageViewControllerDataSource
	{
		private MainPageViewController _parentViewController;
		List<BaseViewController> subPages;

		public PageDataSource(UIViewController parentViewController, List<BaseViewController> pages)
		{
			_parentViewController = parentViewController as MainPageViewController;
			subPages = pages;
		}

		public override UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var vc = referenceViewController as BaseViewController;
			var index = vc.pageIndex;
			if (index == 0)
			{
				return null;
			}
			else {
				index--;
				return _parentViewController.ViewControllerAtIndex(index);
			}
		}

		public override UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var vc = referenceViewController as BaseViewController;
			var index = vc.pageIndex;

			index++;
			if (index == subPages.Count)
			{
				return null;
			}
			else {
				return _parentViewController.ViewControllerAtIndex(index);
			}
		}
	}
}


