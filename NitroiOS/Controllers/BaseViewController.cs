using System;
using UIKit;
using BigTed;
using Foundation;
using System.Drawing;

namespace location2
{
	public partial class BaseViewController : UIViewController
	{
		public int pageIndex = 0;
		public string titleText;
		public string imageFile;

		public UILabel lblTitle;

		public BaseViewController() : base()
		{
		}
		public BaseViewController(IntPtr handle) : base(handle)
		{
		}

		protected void ShowLoadingView(string title)
		{
			InvokeOnMainThread(() => { BTProgressHUD.Show(title, -1, ProgressHUD.MaskType.Black); });
		}

		protected void HideLoadingView()
		{
			InvokeOnMainThread(() => { BTProgressHUD.Dismiss(); });
		}


		// Show the alert view
		protected void ShowMessageBox(string title, string message, string cancelButton, string[] otherButtons, Action successHandler)
		{
			var alertView = new UIAlertView(title, message, null, cancelButton, otherButtons);
			alertView.Clicked += (sender, e) =>
			{
				if (e.ButtonIndex == 0)
				{
					return;
				}
				if (successHandler != null)
				{
					successHandler();
				}
			};
			alertView.Show();
		}

		//overloaded method
		protected void ShowMessageBox(string title, string message)
		{
			InvokeOnMainThread(() => { ShowMessageBox(title, message, "Ok", null, null); });
		}

		protected bool TextFieldShouldReturn(UITextField textField)
		{
			textField.ResignFirstResponder();
			return true;
		}

		public UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
		{
			try
			{
				var sourceSize = sourceImage.Size;
				var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
				if (maxResizeFactor > 1) return sourceImage;
				var width = maxResizeFactor * sourceSize.Width;
				var height = maxResizeFactor * sourceSize.Height;
				UIGraphics.BeginImageContext(new SizeF((float)width, (float)height));
				sourceImage.Draw(new RectangleF(0, 0, (float)width, (float)height));
				var resultImage = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
				return resultImage;
			}
			catch
			{
				return null;
			}
		}

		public UIImage getPictureFromLocal()
		{
			var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string jpgFilename = System.IO.Path.Combine(documentsDirectory, "meImg.jpg");

			UIImage currentImage = UIImage.FromFile(jpgFilename);
			return currentImage;
		}

		public void saveImageToLocal(UIImage img)
		{
			trackSvc.Service1 sv = new trackSvc.Service1();
			try
			{
				var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				string jpgFilename = System.IO.Path.Combine(documentsDirectory, "meImg.jpg");
				NSData imgData = img.AsJPEG();

				NSError err = null;
				if (!imgData.Save(jpgFilename, false, out err))
				{
					ShowMessageBox(null, "NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
				}
			}
			catch (Exception err)
			{
				bool out1 = false;
				bool out2 = false;
				sv.getErrprFromMobile(err.ToString(), NSUserDefaults.StandardUserDefaults.StringForKey("id").ToString(), out out1, out out2);
				ShowMessageBox(null, "Save error");
			}
		}
	}
}

