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
    [Register ("ProfileViewController")]
    partial class ProfileViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnGo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton changePictureBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgPicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblUserName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField passTB { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton removeNitroEventsBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton seriuosBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtFirstName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtLastName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtPhone { get; set; }

        [Action ("ActionChangePhoto:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionChangePhoto (UIKit.UIButton sender);

        [Action ("ActionSerious:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionSerious (UIKit.UIButton sender);

        [Action ("ActionUpdate:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionUpdate (UIKit.UIButton sender);

        [Action ("removeNitroEvents:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void removeNitroEvents (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnGo != null) {
                btnGo.Dispose ();
                btnGo = null;
            }

            if (changePictureBtn != null) {
                changePictureBtn.Dispose ();
                changePictureBtn = null;
            }

            if (imgPicture != null) {
                imgPicture.Dispose ();
                imgPicture = null;
            }

            if (lblUserName != null) {
                lblUserName.Dispose ();
                lblUserName = null;
            }

            if (passTB != null) {
                passTB.Dispose ();
                passTB = null;
            }

            if (removeNitroEventsBtn != null) {
                removeNitroEventsBtn.Dispose ();
                removeNitroEventsBtn = null;
            }

            if (seriuosBtn != null) {
                seriuosBtn.Dispose ();
                seriuosBtn = null;
            }

            if (txtEmail != null) {
                txtEmail.Dispose ();
                txtEmail = null;
            }

            if (txtFirstName != null) {
                txtFirstName.Dispose ();
                txtFirstName = null;
            }

            if (txtLastName != null) {
                txtLastName.Dispose ();
                txtLastName = null;
            }

            if (txtPhone != null) {
                txtPhone.Dispose ();
                txtPhone = null;
            }
        }
    }
}