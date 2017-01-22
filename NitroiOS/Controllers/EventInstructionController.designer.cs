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
    [Register ("EventInstructionController")]
    partial class EventInstructionController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView contentComment { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint heightCommentContent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgType { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAvgHR { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAvgPower { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblAvgSpeed { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblCommentTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblData { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblElapsedTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblLeveledPower { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblLoad { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblStartDate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTotalAcent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTotalCalories { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTotalDistance { get; set; }

        [Action ("ActionAddComment:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionAddComment (UIKit.UIButton sender);

        [Action ("ActionAdjustTrainning:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionAdjustTrainning (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (contentComment != null) {
                contentComment.Dispose ();
                contentComment = null;
            }

            if (heightCommentContent != null) {
                heightCommentContent.Dispose ();
                heightCommentContent = null;
            }

            if (imgType != null) {
                imgType.Dispose ();
                imgType = null;
            }

            if (lblAvgHR != null) {
                lblAvgHR.Dispose ();
                lblAvgHR = null;
            }

            if (lblAvgPower != null) {
                lblAvgPower.Dispose ();
                lblAvgPower = null;
            }

            if (lblAvgSpeed != null) {
                lblAvgSpeed.Dispose ();
                lblAvgSpeed = null;
            }

            if (lblCommentTitle != null) {
                lblCommentTitle.Dispose ();
                lblCommentTitle = null;
            }

            if (lblData != null) {
                lblData.Dispose ();
                lblData = null;
            }

            if (lblElapsedTime != null) {
                lblElapsedTime.Dispose ();
                lblElapsedTime = null;
            }

            if (lblLeveledPower != null) {
                lblLeveledPower.Dispose ();
                lblLeveledPower = null;
            }

            if (lblLoad != null) {
                lblLoad.Dispose ();
                lblLoad = null;
            }

            if (lblStartDate != null) {
                lblStartDate.Dispose ();
                lblStartDate = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }

            if (lblTotalAcent != null) {
                lblTotalAcent.Dispose ();
                lblTotalAcent = null;
            }

            if (lblTotalCalories != null) {
                lblTotalCalories.Dispose ();
                lblTotalCalories = null;
            }

            if (lblTotalDistance != null) {
                lblTotalDistance.Dispose ();
                lblTotalDistance = null;
            }
        }
    }
}