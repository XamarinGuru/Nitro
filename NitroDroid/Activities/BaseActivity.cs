using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidHUD;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortableLibrary;

namespace goheja
{
	[Activity(Label = "BaseActivity")]
	public class BaseActivity : FragmentActivity
	{
		AlertDialog.Builder alert;

		trackSvc.Service1 mTrackSvc = new trackSvc.Service1();
		//Reachability.Reachability mConnection = new Reachability.Reachability();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			Window.RequestFeature(WindowFeatures.NoTitle);
			base.OnCreate(savedInstanceState);
		}

		protected override void OnResume()
		{
			base.OnResume();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (this.CurrentFocus == null) return true;

			InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
			imm.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, 0);
			return base.OnTouchEvent(e);
		}

		public void ShowLoadingView(string title)
		{
			RunOnUiThread(() =>
			{
				AndHUD.Shared.Show(this, title, -1, MaskType.Black);
			});
		}

		public void HideLoadingView()
		{
			RunOnUiThread(() =>
			{
				AndHUD.Shared.Dismiss(this);
			});
		}

		public void ShowMessageBox(string title, string message, string cancelButton, string[] otherButtons, Action successHandler)
		{
			alert = new AlertDialog.Builder(this);
			alert.SetTitle(title);
			alert.SetMessage(message);
			alert.SetPositiveButton("Cancel", (senderAlert, args) =>
			{
			});
			alert.SetNegativeButton("OK", (senderAlert, args) =>
			{
				successHandler();
			});
			RunOnUiThread(() =>
			{
				alert.Show();
			});
		}

		//public void ShowMessageBox(string title, string message)
		//{
		//	ShowMessageBox(title, message, "Ok", null, null);
		//}

		public void ShowMessageBox(string title, string message, bool isFinish = false)
		{
			alert = new AlertDialog.Builder(this);
			alert.SetTitle(title);
			alert.SetMessage(message);
			alert.SetCancelable(false);
			alert.SetPositiveButton("OK", delegate { if (isFinish) Finish(); });
			RunOnUiThread(() =>
			{
				alert.Show();
			});
		}

		protected void OnBack()
		{
			base.OnBackPressed();
			OverridePendingTransition(Resource.Animation.fromRight, Resource.Animation.toLeft);
		}

		#region integrate with web reference

		#region USER_MANAGEMENT
		public string RegisterUser(string fName, string lName, string deviceId, string userName, string psw, string email, int age, bool ageSpecified = true, bool acceptedTerms = true, bool acceptedTermsSpecified = true)
		{
			var result = mTrackSvc.insertNewDevice(fName, lName, deviceId, userName, psw, acceptedTerms, acceptedTermsSpecified, email, age, ageSpecified, Constants.SPEC_GROUP_TYPE[0]);
			return result;
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

		public RootMember GetUserObject()
		{
			var userID = GetUserID();

			try
			{
				var strUser = mTrackSvc.getUsrObject(userID, Constants.SPEC_GROUP_TYPE[0]).ToString();
				var jsonUser = FormatJsonType(strUser);
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
		#endregion

		#region USER_GAUGE
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
		#endregion

		#region EVENT_MANAGEMENT
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

		public void SaveUserImage(byte[] fileBytes)
		{
			try
			{
				var response = mTrackSvc.saveUserImage(AppSettings.UserID, fileBytes);
			}
			catch (Exception err)
			{
				ShowMessageBox(null, "Save error");
			}
		}
		#endregion

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
			var returnString = jsonData.Replace("ObjectId(\"", "\"");
			returnString = returnString.Replace(" ISODate(\"", "\"");
			returnString = returnString.Replace("\")", "\"");

			return returnString;
		}

		public string FormatEventDescription(string rawString)
		{
			if (rawString == "") return rawString;

			int startIndex = rawString.IndexOf("<textarea");
			int endIndex = rawString.IndexOf(">");

			if (startIndex < 0 || endIndex < 0) return rawString;

			int count = endIndex - startIndex;

			var theString = new StringBuilder(rawString);
			theString.Remove(startIndex, count);

			var returnString = theString.ToString();
			returnString = returnString.Replace("</textarea><br/>", "");
			return returnString;
		}

		public int GetFormatedDurationAsMin(string strTime)
		{
			if (strTime == "") return 0;

			var arrTimes = strTime.Split(new char[] { ':' });

			var hrs = int.Parse(arrTimes[0]);
			var min = int.Parse(arrTimes[1]);

			return hrs * 60 + min;
		}

		public void MarkAsInvalide(ImageView validEmail, LinearLayout errorEmail, bool isInvalid)
		{
			if (validEmail != null)
				validEmail.SetImageResource(isInvalid ? Resource.Drawable.icon_cross : Resource.Drawable.icon_check);

			if (errorEmail != null)
				errorEmail.Visibility = isInvalid ? ViewStates.Visible : ViewStates.Invisible;
		}

		#endregion

		public float difAlt(float prev, float curr)
		{
			try
			{
				if ((curr - prev) > 0)
				{
					return curr - prev;
				}
				else
				{
					return 0;
				}
			}
			catch
			{
				return 0;
			}
		}
	}
}
