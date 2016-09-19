// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace location2
{
    [Register ("PageViewController")]
    partial class MainPageViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnCalendar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnHome { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnProfile { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnWatch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint conIndicatorX { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView pageContent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView pageIndicator { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnCalendar != null) {
                btnCalendar.Dispose ();
                btnCalendar = null;
            }

            if (btnHome != null) {
                btnHome.Dispose ();
                btnHome = null;
            }

            if (btnProfile != null) {
                btnProfile.Dispose ();
                btnProfile = null;
            }

            if (btnWatch != null) {
                btnWatch.Dispose ();
                btnWatch = null;
            }

            if (conIndicatorX != null) {
                conIndicatorX.Dispose ();
                conIndicatorX = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }

            if (pageContent != null) {
                pageContent.Dispose ();
                pageContent = null;
            }

            if (pageIndicator != null) {
                pageIndicator.Dispose ();
                pageIndicator = null;
            }
        }
    }
}