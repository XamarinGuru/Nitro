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
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIKit.UIImageView altimg { get; set; }


        [Outlet]
        UIKit.UILabel altTypeLbl { get; set; }


        [Outlet]
        UIKit.UIImageView bpmImg { get; set; }


        [Outlet]
        UIKit.UILabel bpmLbl { get; set; }


        [Outlet]
        UIKit.UILabel bpmValueLbl { get; set; }


        [Outlet]
        UIKit.UIButton CalenBtn { get; set; }


        [Outlet]
        UIKit.UIImageView distImg { get; set; }


        [Outlet]
        UIKit.UILabel distTypLbl { get; set; }


        [Outlet]
        UIKit.UIView dropDownView { get; set; }


        [Outlet]
        UIKit.UIView firstScreenView { get; set; }


        [Outlet]
        UIKit.UIImageView imgArrow { get; set; }


        [Outlet]
        UIKit.UILabel lblAlt { get; set; }


        [Outlet]
        UIKit.UILabel lblDist { get; set; }


        [Outlet]
        UIKit.UILabel lblSpeed { get; set; }


        [Outlet]
        UIKit.UIView mainframe { get; set; }


        [Outlet]
        UIKit.UIButton meBtn { get; set; }


        [Outlet]
        UIKit.UIButton selectBikeBtn { get; set; }


        [Outlet]
        UIKit.UIButton selectedBtn { get; set; }


        [Outlet]
        UIKit.UIButton selectRunBtn { get; set; }


        [Outlet]
        UIKit.UIImageView speedImg { get; set; }


        [Outlet]
        UIKit.UILabel speedTypeLbl { get; set; }


        [Outlet]
        UIKit.UIButton startStopBtn { get; set; }


        [Outlet]
        UIKit.UIButton stopBtn { get; set; }


        [Outlet]
        UIKit.UILabel timerLbl { get; set; }


        [Outlet]
        UIKit.UINavigationItem titleView { get; set; }


        [Outlet]
        UIKit.UIButton toolBtn { get; set; }


        [Outlet]
        UIKit.UIView viewLine { get; set; }


        [Outlet]
        UIKit.UIButton watchBtn { get; set; }


        [Outlet]
        UIKit.UIImageView wattImg { get; set; }


        [Outlet]
        UIKit.UILabel wattLbl { get; set; }


        [Outlet]
        UIKit.UIWebView wvOngoing { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBack { get; set; }


        [Action ("CalenBtn_TouchUpInside:")]
        partial void CalenBtn_TouchUpInside (UIKit.UIButton sender);


        [Action ("MeBtn_TouchUpInside:")]
        partial void MeBtn_TouchUpInside (UIKit.UIButton sender);


        [Action ("SelectBikeBtn_TouchUpInside:")]
        partial void SelectBikeBtn_TouchUpInside (UIKit.UIButton sender);


        [Action ("SelectRunBtn_TouchUpInside:")]
        partial void SelectRunBtn_TouchUpInside (UIKit.UIButton sender);


        [Action ("StartStopBtn_TouchUpInside:")]
        partial void StartStopBtn_TouchUpInside (UIKit.UIButton sender);


        [Action ("StopBtn_TouchUpInside:")]
        partial void StopBtn_TouchUpInside (UIKit.UIButton sender);


        [Action ("ToolBtn_TouchUpInside:")]
        partial void ToolBtn_TouchUpInside (UIKit.UIButton sender);


        [Action ("WatchBtn_TouchUpInside:")]
        partial void WatchBtn_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (altimg != null) {
                altimg.Dispose ();
                altimg = null;
            }

            if (altTypeLbl != null) {
                altTypeLbl.Dispose ();
                altTypeLbl = null;
            }

            if (bpmImg != null) {
                bpmImg.Dispose ();
                bpmImg = null;
            }

            if (bpmLbl != null) {
                bpmLbl.Dispose ();
                bpmLbl = null;
            }

            if (bpmValueLbl != null) {
                bpmValueLbl.Dispose ();
                bpmValueLbl = null;
            }

            if (btnBack != null) {
                btnBack.Dispose ();
                btnBack = null;
            }

            if (distImg != null) {
                distImg.Dispose ();
                distImg = null;
            }

            if (distTypLbl != null) {
                distTypLbl.Dispose ();
                distTypLbl = null;
            }

            if (firstScreenView != null) {
                firstScreenView.Dispose ();
                firstScreenView = null;
            }

            if (lblAlt != null) {
                lblAlt.Dispose ();
                lblAlt = null;
            }

            if (lblDist != null) {
                lblDist.Dispose ();
                lblDist = null;
            }

            if (lblSpeed != null) {
                lblSpeed.Dispose ();
                lblSpeed = null;
            }

            if (mainframe != null) {
                mainframe.Dispose ();
                mainframe = null;
            }

            if (selectBikeBtn != null) {
                selectBikeBtn.Dispose ();
                selectBikeBtn = null;
            }

            if (selectedBtn != null) {
                selectedBtn.Dispose ();
                selectedBtn = null;
            }

            if (selectRunBtn != null) {
                selectRunBtn.Dispose ();
                selectRunBtn = null;
            }

            if (speedImg != null) {
                speedImg.Dispose ();
                speedImg = null;
            }

            if (speedTypeLbl != null) {
                speedTypeLbl.Dispose ();
                speedTypeLbl = null;
            }

            if (startStopBtn != null) {
                startStopBtn.Dispose ();
                startStopBtn = null;
            }

            if (stopBtn != null) {
                stopBtn.Dispose ();
                stopBtn = null;
            }

            if (timerLbl != null) {
                timerLbl.Dispose ();
                timerLbl = null;
            }

            if (titleView != null) {
                titleView.Dispose ();
                titleView = null;
            }

            if (wattImg != null) {
                wattImg.Dispose ();
                wattImg = null;
            }

            if (wattLbl != null) {
                wattLbl.Dispose ();
                wattLbl = null;
            }

            if (wvOngoing != null) {
                wvOngoing.Dispose ();
                wvOngoing = null;
            }
        }
    }
}