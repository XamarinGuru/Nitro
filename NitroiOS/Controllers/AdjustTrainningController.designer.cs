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
    [Register ("AdjustTrainningController")]
    partial class AdjustTrainningController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch checkAttended { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDistance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTSS { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtComment { get; set; }

        [Action ("ActionAdjustTrainning:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionAdjustTrainning (UIKit.UIButton sender);

        [Action ("ActionDataChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionDataChanged (UIKit.UISlider sender);

        void ReleaseDesignerOutlets ()
        {
            if (checkAttended != null) {
                checkAttended.Dispose ();
                checkAttended = null;
            }

            if (lblDistance != null) {
                lblDistance.Dispose ();
                lblDistance = null;
            }

            if (lblTime != null) {
                lblTime.Dispose ();
                lblTime = null;
            }

            if (lblTSS != null) {
                lblTSS.Dispose ();
                lblTSS = null;
            }

            if (txtComment != null) {
                txtComment.Dispose ();
                txtComment = null;
            }
        }
    }
}