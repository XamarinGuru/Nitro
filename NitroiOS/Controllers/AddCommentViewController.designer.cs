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
    [Register ("AddCommentViewController")]
    partial class AddCommentViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtComment { get; set; }

        [Action ("ActionAddComment:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionAddComment (UIKit.UIButton sender);

        [Action ("ActionClose:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionClose (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (txtComment != null) {
                txtComment.Dispose ();
                txtComment = null;
            }
        }
    }
}