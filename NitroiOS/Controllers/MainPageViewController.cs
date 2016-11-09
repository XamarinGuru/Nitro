using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using System.CodeDom.Compiler;
using System.Drawing;
using System.IO;


using CoreGraphics;


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

			var calendarVC = (BaseViewController)this.Storyboard.InstantiateViewController("CalendarViewController");
			var analyticsVC = (BaseViewController)this.Storyboard.InstantiateViewController("AnalyticsViewController");
			var profileVC = (BaseViewController)this.Storyboard.InstantiateViewController("ProfileViewController");
			calendarVC.pageIndex = 0;
			analyticsVC.pageIndex = 1;
			profileVC.pageIndex = 2;

			analyticsVC.lblTitle = lblTitle;

			subControllers.Add(calendarVC);
			subControllers.Add(analyticsVC);
			subControllers.Add(profileVC);

			btnWatch.TouchUpInside += WatchButtonClicked;
			btnWatch.Hidden = true;

			mPageViewController = this.Storyboard.InstantiateViewController("PageViewController") as UIPageViewController;
			mPageViewController.DataSource = new PageDataSource(this, subControllers);

			var startVC = this.ViewControllerAtIndex(0) as BaseViewController;
			var viewControllers = new UIViewController[] { startVC };

			mPageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, false, null);
			mPageViewController.View.Frame = pageContent.Bounds;
			AddChildViewController(this.mPageViewController);
			pageContent.AddSubview(this.mPageViewController.View);
			mPageViewController.DidMoveToParentViewController(this);

			var swipe = mPageViewController.GestureRecognizers;

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



		private void SetCurrentPage(int pIndex)
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
			btnCalendar.Selected = false;
			btnHome.Selected = false;
			btnProfile.Selected = false;

			btnWatch.Hidden = true;

			nfloat xPos = 0;
			switch (pageNumber)
			{
				case 0:
					btnCalendar.Selected = true;
					lblTitle.Text = "Calendar";
					xPos = btnCalendar.Frame.X;
					break;
				case 1:
					btnHome.Selected = true;
					lblTitle.Text = "Nitro ready...";
					xPos = btnHome.Frame.X;
					break;
				case 2:
					btnProfile.Selected = true;
					lblTitle.Text = "Personal Data";
					xPos = btnProfile.Frame.X;
					btnWatch.Hidden = false;
					break;
			}
			UIView.Animate(2, () => { conIndicatorX.Constant = xPos; });
		}
		private void WatchButtonClicked(object sender, EventArgs e)
		{
			string id = NSUserDefaults.StandardUserDefaults.StringForKey("id");
			var url = "http://go-heja.com/gh/mob/sync.php?userId=" + id + "&mog=nitro&url=uurrll";
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


