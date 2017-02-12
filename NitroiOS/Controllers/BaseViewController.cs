﻿using System;
using UIKit;
using BigTed;
using Foundation;
using System.Drawing;
using CoreGraphics;
using Newtonsoft.Json;
using PortableLibrary;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace location2
{
	public partial class BaseViewController : UIViewController
	{
		UIColor COLOR_ORANGE = new UIColor(red: 0.90f, green: 0.63f, blue: 0.04f, alpha: 1.0f);

		protected float scroll_amount = 0.0f;
		protected bool moveViewUp = false;

		public MainPageViewController rootVC;
		public int pageIndex = 0;
		public string titleText;
		public string imageFile;

		trackSvc.Service1 mTrackSvc = new trackSvc.Service1();
		Reachability.Reachability mConnection = new Reachability.Reachability();

		protected RootMemberModel MemberModel = new RootMemberModel();

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

		protected void ShowMessageBox1(string title, string message, string cancelButton, string[] otherButtons, Action successHandler)
		{
			InvokeOnMainThread(() =>
			{
				var alertView = new UIAlertView(title, message, null, cancelButton, otherButtons);
				alertView.Clicked += (sender, e) =>
				{
					successHandler();
				};
				alertView.Show();

			});
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
			bool isOnline = mConnection.IsHostReachable(Constants.URL_GOOGLE) ? true : false;
			if (!isOnline)
			{
				ShowMessageBox(null, Constants.MSG_NO_INTERNET);
				return false;
			}
			return true;
		}

		#region integrate with web reference
		public string RegisterUser(string fName, string lName, string deviceId, string userName, string psw, string email, int age, bool ageSpecified = true, bool acceptedTerms = true, bool acceptedTermsSpecified = true)
		{
			try
			{
				var result = mTrackSvc.insertNewDevice(fName, lName, deviceId, userName, psw, acceptedTerms, acceptedTermsSpecified, email, age, ageSpecified, Constants.SPEC_GROUP_TYPE[0]);
				return result;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
		public bool LoginUser(string email, string password)
		{
			try
			{
				var userID = mTrackSvc.getListedDeviceId(email, password, Constants.SPEC_GROUP_TYPE[0]);

				if (userID == "0")
					return false;
				
				AppSettings.UserID = userID;
				return true;
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return false;
			}
		}
		public void SignOutUser()
		{
			AppSettings.UserID = string.Empty;
			AppSettings.Email = string.Empty;
			AppSettings.Password = string.Empty;
			AppSettings.DeviceID = string.Empty;
			AppSettings.DeviceUDID = string.Empty;
		}

		public string GetCode(string email)
		{
			try
			{
				var response = mTrackSvc.getCode(email, Constants.SPEC_GROUP_TYPE[0]);

				return response;
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return null;
			}
		}

		public int ResetPassword(string email, string password)
		{
			try
			{
				int result = 0;
				bool resultSpecified = false;
				mTrackSvc.restPasword(email, password, Constants.SPEC_GROUP_TYPE[0], out result, out resultSpecified);

				return result;
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return 0;
			}
		}


		public string GetUserID()
		{
			var userID = AppSettings.UserID;
			if (userID != null && userID != "0" && userID != string.Empty)
				return userID;

			if (AppSettings.Email == string.Empty || AppSettings.Password == string.Empty || AppSettings.Email == null || AppSettings.Password == null)
				return "0";
			
			try
			{
				userID = mTrackSvc.getListedDeviceId(AppSettings.Email, AppSettings.Password, Constants.SPEC_GROUP_TYPE[0]);

				if (userID != "0")
					AppSettings.UserID = userID;
				
				return userID;
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return "0";
			}
		}

		public Gauge GetGauge()
		{
			var userID = GetUserID();

			try
			{
				var strGauge = mTrackSvc.getGaugeMob(DateTime.Now, true, userID, null, Constants.SPEC_GROUP_TYPE[0], null, "5");
				Gauge gaugeObject = JsonConvert.DeserializeObject<Gauge>(strGauge);
				return gaugeObject;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				ShowMessageBox(null, ex.Message);
			}
			return null;
		}

		public RootMember GetUserObject()
		{
			var userID = GetUserID();

			try
			{
				var objUser = mTrackSvc.getUsrObject(userID, Constants.SPEC_GROUP_TYPE[0]);
				var jsonUser = FormatJsonType(objUser.ToString());
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

		public string UpdateUserDataJson(RootMember updatedUserObject, string updatedById = null)
		{
			var userID = GetUserID();
			var jsonUser = JsonConvert.SerializeObject(updatedUserObject);
			Console.WriteLine(jsonUser);
			updatedById = userID;
			var result = mTrackSvc.updateUserDataJson(userID, jsonUser, updatedById, Constants.SPEC_GROUP_TYPE[0]);
			return result;
		}

		public List<NitroEvent> GetPastEvents()
		{
			try
			{
				var strPastEvents = mTrackSvc.getUserCalendarPast(AppSettings.UserID, Constants.SPEC_GROUP_TYPE[0]);
				var eventsData = JArray.Parse(FormatJsonType(strPastEvents));
				return CastNitroEvents(eventsData);
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return null;
			}
		}

		public List<NitroEvent> GetTodayEvents()
		{
			try
			{
				var strTodayEvents = mTrackSvc.getUserCalendarToday(AppSettings.UserID, Constants.SPEC_GROUP_TYPE[0]);
				var eventsData = JArray.Parse(FormatJsonType(strTodayEvents));
				return CastNitroEvents(eventsData);
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return null;
			}
		}

		public List<NitroEvent> GetFutureEvents()
		{
			try
			{
				var strFutureEvents = mTrackSvc.getUserCalendarFuture(AppSettings.UserID, Constants.SPEC_GROUP_TYPE[0]);
				var eventsData = JArray.Parse(FormatJsonType(strFutureEvents));
				return CastNitroEvents(eventsData);
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return null;
			}
		}

		public NitroEvent GetEventDetail(string eventID)
		{
			try
			{
				var strEventDetail = mTrackSvc.getEventMob(eventID, Constants.SPEC_GROUP_TYPE[0]);
				var eventsData = JArray.Parse(FormatJsonType(strEventDetail.ToString()));
				return CastNitroEvents(eventsData)[0];
			}
			catch (Exception err)
			{
				ShowMessageBox(null, err.Message);
				return null;
			}
		}

		public List<NitroEvent> CastNitroEvents(JArray events)
		{
			
			var returnEvents = new List<NitroEvent>();

			if (events == null) return returnEvents;

			foreach (var eventJson in events)
			{
				NitroEvent nitroEvent = JsonConvert.DeserializeObject<NitroEvent>(eventJson.ToString());
				returnEvents.Add(nitroEvent);
			}
			return returnEvents;
		}

		public EventTotal GetEventTotals(string eventID)
		{
			var eventTotal = new EventTotal();
			try
			{
				var totalObject = mTrackSvc.getEventTotalsMob(eventID, Constants.SPEC_GROUP_TYPE[0]);
				eventTotal = JsonConvert.DeserializeObject<EventTotal>(totalObject.ToString());
			}
			catch (Exception ex)
			{
				ShowMessageBox(null, ex.Message);
				return null;
			}
			return eventTotal;
		}

		public Comment GetComments(string eventID, string type = "1")
		{
			var comment = new Comment();
			try
			{
				var commentObject = mTrackSvc.getComments(eventID, "1", Constants.SPEC_GROUP_TYPE[0]);
				comment = JsonConvert.DeserializeObject<Comment>(commentObject.ToString());
			}
			catch (Exception ex)
			{
				ShowMessageBox(null, ex.Message);
				return null;
			}
			return comment;
		}

		public object SetComment(string author, string authorId, string commentText, string eventId)
		{
			try
			{
				var response = mTrackSvc.setComments(author, authorId, commentText, eventId, Constants.SPEC_GROUP_TYPE[0]);
				return response;
			}
			catch (Exception ex)
			{
				ShowMessageBox(null, ex.Message);
				return null;
			}
		}

		public void UpdateMemberNotes(string notes, string userID, string eventId, string username, string attended, string duration, string distance, string trainScore, string type)
		{
			try
			{
				var response = mTrackSvc.updateMeberNotes(notes, userID, eventId, username, attended, duration, distance, trainScore, type, Constants.SPEC_GROUP_TYPE[0]);
				//return response;
			}
			catch (Exception ex)
			{
				ShowMessageBox(null, ex.Message);
				return;
			}
		}

		public void UpdateMomgoData(string name,
					string loc,
					System.DateTime time,
					bool timeSpecified,
					string deviceID,
					float speed,
					bool speedSpecified,
					string athId,
					string country,
					float dist,
					bool distSpecified,
					float alt,
					bool altSpecified,
					float bearing,
					bool bearingSpecified,
					int recordType,
					bool recordTypeSpecified,
					string eventType,
					string specGroup)
		{
			mTrackSvc.updateMomgoData(name, loc, time, true, AppSettings.DeviceUDID, speed, true, athId, country, dist, true, alt, true, bearing, true, 1, true, eventType, specGroup);
		}

		public int GetFormatedDurationAsMin(string strTime)
		{
			if (strTime == "") return 0;

			var arrTimes = strTime.Split(new char[] { ':' });

			var hrs = int.Parse(arrTimes[0]);
			var min = int.Parse(arrTimes[1]);

			return hrs * 60 + min;
		}

		public float TotalSecFromString(string strTime)
		{
			if (strTime == "") return 0;

			try
			{
				var arrTimes = strTime.Split(new char[] { ':' });

				var hrs = int.Parse(arrTimes[0]);
				var min = int.Parse(arrTimes[1]);
				var sec = int.Parse(arrTimes[2]);

				return hrs * 3600 + min * 60 + sec;
			}
			catch
			{
				return 0;
			}
		}

		public float ConvertToFloat(string value)
		{
			if (value == null || value == "")
				return 0;

			try
			{
				return float.Parse(value);
			}
			catch
			{
				return 0;
			}
		}


		public void CompareEventResult(float planned, float total, UILabel lblPlanned, UILabel lblTotal)
		{
			if (planned == total || planned == 0 || total == 0)
			{
				lblPlanned.TextColor = COLOR_ORANGE;
				lblTotal.TextColor = COLOR_ORANGE;
				return;
			}

			if (planned > total)
			{
				var delta = (planned - total) / total;
				if (delta < 0.15)
				{
					lblPlanned.TextColor = COLOR_ORANGE;
					lblTotal.TextColor = COLOR_ORANGE;
				}
				else {
					lblPlanned.TextColor = UIColor.Blue;
					lblTotal.TextColor = UIColor.Blue;
				}
			}
			else if (planned < total)
			{
				var delta = (total - planned) / planned;
				if (delta < 0.15)
				{
					lblPlanned.TextColor = COLOR_ORANGE;
					lblTotal.TextColor = COLOR_ORANGE;
				}
				else {
					lblPlanned.TextColor = UIColor.Red;
					lblTotal.TextColor = UIColor.Red;
				}
			}
		}

		public bool ValidateUserNickName(string nickName)
		{
			var validate = mTrackSvc.validateNickName(nickName, Constants.SPEC_GROUP_TYPE[0]);
			if (validate != "1")
			{
				return true;
			}
			else {
				return false;
			}
		}

		private string FormatJsonType(string jsonData)
		{
			var returnString = jsonData.Replace(Constants.INVALID_JSONS1[0], "\"");
			returnString = returnString.Replace(Constants.INVALID_JSONS1[1], "\"");
			returnString = returnString.Replace(Constants.INVALID_JSONS1[2], "\"");

			return returnString;
		}

		public string FormatEventDescription(string eventJson)
		{
			var returnString = eventJson.Replace(Constants.INVALID_JSONS2[0], "");
			returnString = returnString.Replace(Constants.INVALID_JSONS2[1], "");
			returnString = returnString.Replace(Constants.INVALID_JSONS2[2], "");

			return returnString;
		}

		public string FormatNumber(string number)
		{
			try
			{
				var fNumber = float.Parse(number);
				return fNumber.ToString("F2");
			}
			catch
			{
				return number;
			}
		}
		#endregion

		public NSDate ConvertDateTimeToNSDate(DateTime date)
		{
			DateTime newDate = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0));

			//return NSDate.FromTimeIntervalSinceReferenceDate(
			//	(date - newDate).TotalSeconds + 3600);
			return NSDate.FromTimeIntervalSinceReferenceDate(
				(date - newDate).TotalSeconds);
		}

		public static DateTime ConvertNSDateToDateTime(NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2001, 1, 1, 0, 0, 0));
			reference = reference.AddSeconds(date.SecondsSinceReferenceDate);
			if (reference.IsDaylightSavingTime())
			{
				reference = reference.AddHours(1);
			}
			return reference;
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

		public UIImage GetPictureFromLocal()
		{
			var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string jpgFilename = System.IO.Path.Combine(documentsDirectory, "meImg.jpg");

			UIImage currentImage = UIImage.FromFile(jpgFilename);
			return currentImage;
		}

		public void SaveUserImage(UIImage img)
		{
			try
			{
				var scaledImage = MaxResizeImage(img, 100, 100);
				NSData imgData = scaledImage.AsPNG();
				//save to local
				var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				string jpgFilename = System.IO.Path.Combine(documentsDirectory, "meImg.jpg");

				NSError err = null;
				if (!imgData.Save(jpgFilename, false, out err))
				{
					ShowMessageBox(null, "NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
				}

				//save to server

				var fileBytes = new Byte[imgData.Length];
				System.Runtime.InteropServices.Marshal.Copy(imgData.Bytes, fileBytes, 0, Convert.ToInt32(imgData.Length));

				var response = mTrackSvc.saveUserImage(AppSettings.UserID, fileBytes, Constants.SPEC_GROUP_TYPE[0]);
			}
			catch (Exception err)
			{
				
				ShowMessageBox(null, "Save error");
			}
		}

		public void MarkAsInvalide(UIButton validEmail, UIView errorEmail, bool isInvalid)
		{
			InvokeOnMainThread(() =>
			{
				if (validEmail != null)
					validEmail.Selected = isInvalid;

				if (errorEmail != null)
					errorEmail.Hidden = !isInvalid;
			});
		}

		protected void SetupDatePicker(UITextField field)
		{
			UIDatePicker picker = new UIDatePicker();
			picker.Mode = UIDatePickerMode.Date;
			picker.MinimumDate = NSDate.Now;
			picker.MaximumDate = NSDate.Now.AddSeconds(60 * 60 * 24 * 365 * 3);

			var format = "{0: MM-dd-yyyy}";
			picker.ValueChanged += (object s, EventArgs e) =>
			{
				//if (changeOnEdit)
				{
					updateSetupDateTimePicker(field, picker.Date, format, s, e);
				}
			};

			// Setup the toolbar
			UIToolbar toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Black;
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			// Create a 'done' button for the toolbar and add it to the toolbar
			UIBarButtonItem doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
			{
				updateSetupDateTimePicker(field, picker.Date, format, s, e, true);
			});

			toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);

			field.InputView = picker;
			field.InputAccessoryView = toolbar;

			field.ShouldChangeCharacters = new UITextFieldChange(delegate (UITextField textField, NSRange range, string replacementString)
			{
				return false;
			});
		}

		private void updateSetupDateTimePicker(UITextField field, NSDate date, String format, object sender, EventArgs e, bool done = false)
		{
			var newDate = NSDateToDateTime(date);
			var str = String.Format(format, newDate);

			field.Text = str;
			field.SendActionForControlEvents(UIControlEvent.ValueChanged);
			if (done)
			{
				field.ResignFirstResponder();
			}
		}
		protected static DateTime NSDateToDateTime(NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2001, 1, 1, 0, 0, 0));
			reference = reference.AddSeconds(date.SecondsSinceReferenceDate);
			if (reference.IsDaylightSavingTime())
			{
				reference = reference.AddHours(1);
			}
			return reference;
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
			else if (type == "hr")
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

			public HRPickerViewModel(UITextField textField)
			{
				this.textField = textField;
			}

			public override nint GetComponentCount(UIPickerView pickerView)
			{
				return 2;
			}

			public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
			{
				return 20;
			}

			public override string GetTitle(UIPickerView pickerView, nint row, nint component)
			{
				return (60 + row * 10).ToString();
			}

			public override void Selected(UIPickerView pickerView, nint row, nint component)
			{
				var mm = 60 + pickerView.SelectedRowInComponent(0) * 10;
				var ss = 60 + pickerView.SelectedRowInComponent(1) * 10;

				textField.Text = mm + "-" + ss;
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

