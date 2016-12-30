using System;
using UIKit;
using BigTed;
using Foundation;
using System.Drawing;
using CoreGraphics;
using Newtonsoft.Json;

namespace location2
{
	public partial class BaseViewController : UIViewController
	{
		public MainPageViewController rootVC;
		public int pageIndex = 0;
		public string titleText;
		public string imageFile;

		public UILabel lblTitle;

		trackSvc.Service1 mTrackSvc = new trackSvc.Service1();
		Reachability.Reachability mConnection = new Reachability.Reachability();

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

		public bool IsNetEnable()
		{
			return mConnection.IsHostReachable("www.google.com") ? true : false;
		}

		#region integrate with web reference
		public string RegisterUser(string fName, string lName, string deviceId, string userName, string psw, string email, int age, bool ageSpecified = true, bool acceptedTerms = true, bool acceptedTermsSpecified = true)
		{
			var result = mTrackSvc.insertNewDevice(fName, lName, deviceId, userName, psw, acceptedTerms, acceptedTermsSpecified, email, age, ageSpecified);
			return result;
		}
		public string GetUserID()
		{
			var userID = NSUserDefaults.StandardUserDefaults.StringForKey("userID");
			if (userID != null && userID != "0")
				return userID;
			
			try
			{
				string strEmail = NSUserDefaults.StandardUserDefaults.StringForKey("email");
				string strPassword = NSUserDefaults.StandardUserDefaults.StringForKey("password");

				userID = mTrackSvc.getListedDeviceId(strEmail, strPassword);

				if (userID != "0")
					NSUserDefaults.StandardUserDefaults.SetString(userID, "userID");
				
				return userID;
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return "0";
			}
		}

		public RootMember GetUserObject()
		{
			var userID = GetUserID();

			try
			{
				var jsonUser = mTrackSvc.getUsrObject(userID).ToString();
				//var jsonUser = "{ \"profile\" : { \"idData\" : [], \"userSystemData\" : [], \"injuries\" : [{ \"_id\" : \"17\", \"name\" : \"in name\", \"value\" : \"\", \"isCurrent\" : false, \"date\" : \"1 / 1 / 0001 12:00:00 AM\" }], \"history\" : [{ \"_id\" : \"0\", \"date\" : \"1 / 1 / 0001 12:00:00 AM\", \"text\" : \"\" }], \"performance\" : [{ \"_id\" : \"0\", \"name\" : \"ACL\", \"value\" : \"\" }, { \"_id\" : \"1\", \"name\" : \"ATL\", \"value\" : \"\" }, { \"_id\" : \"2\", \"name\" : \"TSB\", \"value\" : \"\" }, { \"_id\" : \"3\", \"name\" : \"TSS\", \"value\" : \"\" }], \"physical\" : [{ \"_id\" : \"0\", \"name\" : \"Weight\", \"value\" : \"55\", \"unit\" : \"55\" }, { \"_id\" : \"1\", \"name\" : \"Height\", \"value\" : \"44\", \"unit\" : \"44\" }, { \"_id\" : \"2\", \"name\" : \"BMI\", \"value\" : \"33\", \"unit\" : \"33\" }, { \"_id\" : \"3\", \"name\" : \"Fat percentage\", \"value\" : \"11\", \"unit\" : \"11\" }], \"goals\" : [{ \"_id\" : \"0\", \"year\" : \"2016\", \"text\" : \"\" }], \"bestResults\" : [], \"experience\" : [{ \"_id\" : \"0\", \"name\" : \"Swim\", \"value\" : \"\" }, { \"_id\" : \"2\", \"name\" : \"Run\", \"value\" : \"\" }, { \"_id\" : \"1\", \"name\" : \"Bike\", \"value\" : \"\" }, { \"_id\" : \"3\", \"name\" : \"Endurance\", \"value\" : \"\" }], \"selfRanking\" : [{ \"fieldId\" : \"0\", \"fieldName\" : \"Swim\", \"rank\" : \"1\" }, { \"fieldId\" : \"1\", \"fieldName\" : \"Bike\", \"rank\" : \"1\" }, { \"fieldId\" : \"2\", \"fieldName\" : \"Run\", \"rank\" : \"1\" }], \"events\" : [], \"fields\" : [] }, \"updatedBy\" : \"12 / 12 / 2017 12:00:00 AM\", \"lastUpdateDate\" : \"1 / 1 / 2017 12:00:00 AM\", \"createdDate\" : \"1 / 1 / 2016 12:00:00 AM\", \"auth\" : \"\", \"userName\" : \"test user\", \"password\" : \"test password\", \"sportComp\" : 0, \"sportCompKey\" : \"\" }";
				RootMember rootMember = JsonConvert.DeserializeObject<RootMember>(jsonUser);
				return rootMember;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				ShowMessageBox(null, ex.Message);
			}
			return null;
		}

		public string UpdateUserDataJson(RootMember updatedUserObject, string updatedById = null, string specGroup = "1")
		{
			var userID = GetUserID();
			var jsonUser = JsonConvert.SerializeObject(updatedUserObject);
			Console.WriteLine(jsonUser);
			updatedById = userID;
			var result = mTrackSvc.updateUserDataJson(userID, jsonUser, updatedById, specGroup);
			return result;
		}

		public bool ValidateUserNickName(string nickName)
		{
			var validate = mTrackSvc.validateNickName(nickName);
			if (validate != "1")
			{
				return true;
			}
			else {
				return false;
			}
		}

		#endregion


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

		public void MarkAsInvalide(UITextField textField)
		{
			InvokeOnMainThread(() =>
			{
				textField.Layer.BorderColor = UIColor.Red.CGColor;
				textField.Layer.BorderWidth = 3;
				textField.Layer.CornerRadius = 5;
			});
		}


		protected void SetupPicker(UITextField field, string type)
		{
			// Setup the toolbar
			UIToolbar toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Black;
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			// Create a 'done' button for the toolbar and add it to the toolbar
			UIBarButtonItem doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
			{
				field.ResignFirstResponder();
			});

			toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);

			UIPickerViewModel picker_model = new UIPickerViewModel();
			if (type == "time")
				picker_model = new TimePickerViewModel(field);
			else if (type == "ranking")
				picker_model = new RankingPickerViewModel(field);
			else if (type == "HR")
				picker_model = new HRPickerViewModel(field);
			else if (type == "pace")
				picker_model = new PacePickerViewModel(field);
			
			UIPickerView picker = new UIPickerView();
			picker.BackgroundColor = UIColor.White;
			picker.Model = picker_model;
			picker.ShowSelectionIndicator = true;

			field.InputView = picker;
			field.InputAccessoryView = toolbar;

			field.ShouldChangeCharacters = new UITextFieldChange(delegate (UITextField textField, NSRange range, string replacementString)
			{
				return false;
			});
		}

		#region CLASS RANKING_PICKER
		public class RankingPickerViewModel : UIPickerViewModel
		{
			UITextField textField;

			public RankingPickerViewModel(UITextField textField)
			{
				this.textField = textField;
			}

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 1;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				return 5;
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				return ((int)row + 1).ToString() ;
			}

			public override void Selected(UIPickerView pickerView, nint row, nint component)
			{
				textField.Text = (pickerView.SelectedRowInComponent(0) + 1).ToString();
				textField.SendActionForControlEvents(UIControlEvent.ValueChanged);
			}
		}
		#endregion

		#region CLASS RANKING_PICKER
		public class HRPickerViewModel : UIPickerViewModel
		{
			UITextField textField;
			string[] list = new string[5];

			public HRPickerViewModel(UITextField textField)
			{
				this.textField = textField;
				list[0] = "80-101";
				list[1] = "102-118";
				list[2] = "119-132";
				list[3] = "133-150";
				list[4] = "151-170";
			}

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 1;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				return list.Length;
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				return list[row];
			}

			public override void Selected(UIPickerView pickerView, nint row, nint component)
			{
				textField.Text = list[row];
				textField.SendActionForControlEvents(UIControlEvent.ValueChanged);
			}
		}
		#endregion

		#region CLASS PACE_PICKER HH:MM:SS
		public class PacePickerViewModel : UIPickerViewModel
		{
			UITextField textField;

			public PacePickerViewModel(UITextField textField)
			{
				this.textField = textField;
			}

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 2;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				switch (component)
				{
					case 0:
						return 100;
					case 1:
						return 60;
					default:
						break;
				}
				return 0;
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				string title = string.Format("{0:00}", (int)row);
				return title;
			}

			public override void Selected(UIPickerView pickerView, nint row, nint component)
			{
				var mm = string.Format("{0:00}", pickerView.SelectedRowInComponent(0));
				var ss = string.Format("{0:00}", pickerView.SelectedRowInComponent(1));

				textField.Text = mm + ":" + ss;
				textField.SendActionForControlEvents(UIControlEvent.ValueChanged);
			}
		}
		#endregion

		#region CLASS TIME_PICKER HH:MM:SS
		public class TimePickerViewModel : UIPickerViewModel
		{
			UITextField textField;

			public TimePickerViewModel(UITextField textField)
			{
				this.textField = textField;
			}

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 3;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				switch (component)
				{
					case 0:
						return 100;
					case 1:
						return 60;
					case 2:
						return 60;
					default:
						break;
				}
				return 0;
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				string title = string.Format("{0:00}", (int)row);
				return title;
			}

			public override void Selected(UIPickerView pickerView, nint row, nint component)
			{
				var hh = string.Format("{0:00}", pickerView.SelectedRowInComponent(0));
				var mm = string.Format("{0:00}", pickerView.SelectedRowInComponent(1));
				var ss = string.Format("{0:00}", pickerView.SelectedRowInComponent(2));

				textField.Text = hh + ":" + mm + ":" + ss;
				textField.SendActionForControlEvents(UIControlEvent.ValueChanged);
			}
		}
		#endregion
	}
}

