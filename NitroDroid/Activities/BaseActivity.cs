using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using AndroidHUD;
using Newtonsoft.Json;

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

		public void ShowLoadingView(string title)
		{
			AndHUD.Shared.Show(this, title, -1, MaskType.Black);
		}

		public void HideLoadingView()
		{
			AndHUD.Shared.Dismiss(this);
		}

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

		#region integrate with web reference
		public string RegisterUser(string fName, string lName, string deviceId, string userName, string psw, string email, int age, bool ageSpecified = true, bool acceptedTerms = true, bool acceptedTermsSpecified = true)
		{
			var result = mTrackSvc.insertNewDevice(fName, lName, deviceId, userName, psw, acceptedTerms, acceptedTermsSpecified, email, age, ageSpecified);
			return result;
		}
		public string GetUserID()
		{
			if (AppSettings.UserID != null && AppSettings.UserID != "0")
				return AppSettings.UserID;

			try
			{
				var userID = mTrackSvc.getListedDeviceId(AppSettings.Email, AppSettings.Password);

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
				var strUser = mTrackSvc.getUsrObject(userID).ToString();
				var jsonUser = FormatJsonType(strUser);
				//var jsonUser = "{ \"profile\" : { \"idData\" : [{ \"_id\" : \"0\", \"name\" : \"Name\", \"value\" : \"dfirst\" }, { \"_id\" : \"1\", \"name\" : \"Last name\", \"value\" : \"dlast\" }, { \"_id\" : \"2\", \"name\" : \"Country\", \"value\" : \"\" }, { \"_id\" : \"3\", \"name\" : \"Adddress\", \"value\" : \"\" }, { \"_id\" : \"4\", \"name\" : \"Bib\", \"value\" : \"\" }, { \"_id\" : \"5\", \"name\" : \"Age\", \"value\" : \"33\" }, { \"_id\" : \"6\", \"name\" : \"Gender\", \"value\" : \"\" }, { \"_id\" : \"7\", \"name\" : \"Date of birth\", \"value\" : \"\" }, { \"_id\" : \"8\", \"name\" : \"E-mail\", \"value\" : \"d@mail.com\" }, { \"_id\" : \"9\", \"name\" : \"Phone\", \"value\" : \"972-5\" }], \"userSystemData\" : [{ \"_id\" : \"0\", \"name\" : \"Device id\", \"value\" : \"f4ce0233b2676f1e\" }, { \"_id\" : \"1\", \"name\" : \"User type\", \"value\" : \"1\" }, { \"_id\" : \"2\", \"name\" : \"Coache id\", \"value\" : \"1\" }, { \"_id\" : \"3\", \"name\" : \"Terms\", \"value\" : \"1\" }, { \"_id\" : \"4\", \"name\" : \"Image\", \"value\" : \"./images/no-photo.png\" }, { \"_id\" : \"5\", \"name\" : \"entityStatus\", \"value\" : \"1\" }, { \"_id\" : \"6       \", \"name\" : \"Group id\", \"value\" : \"1\" }], \"injuries\" : [{ \"_id\" : \"0\", \"name\" : \"\", \"value\" : \"\", \"isCurrent\" : false, \"date\" : \"1/1/0001 12:00:00 AM\" }], \"history\" : [{ \"_id\" : \"0\", \"date\" : \"1/1/0001 12:00:00 AM\", \"text\" : \"\" }], \"performance\" : [{ \"_id\" : \"0\", \"name\" : \"ACL\", \"value\" : \"\" }, { \"_id\" : \"1\", \"name\" : \"ATL\", \"value\" : \"\" }, { \"_id\" : \"2\", \"name\" : \"TSB\", \"value\" : \"\" }, { \"_id\" : \"3\", \"name\" : \"TSS\", \"value\" : \"\" }], \"physical\" : [{ \"_id\" : \"0\", \"name\" : \"Weight\", \"value\" : \"\", \"unit\" : \"\" }, { \"_id\" : \"1\", \"name\" : \"Height\", \"value\" : \"\", \"unit\" : \"\" }, { \"_id\" : \"2\", \"name\" : \"BMI\", \"value\" : \"\", \"unit\" : \"\" }, { \"_id\" : \"3\", \"name\" : \"Fat percentage\", \"value\" : \"\", \"unit\" : \"\" }], \"goals\" : [{ \"_id\" : \"0\", \"year\" : \"2017\", \"text\" : \"\" }], \"bestResults\" : [{ \"_id\" : \"1\", \"name\" : \"Sprint\", \"value\" : \"00:00:00\" }, { \"_id\" : \"2\", \"name\" : \"Olympic\", \"value\" : \"00:00:00\" }, { \"_id\" : \"3\", \"name\" : \"Half distance\", \"value\" : \"00:00:00\" }, { \"_id\" : \"4\", \"name\" : \"Full distance\", \"value\" : \"00:00:00\" }, { \"_id\" : \"5\", \"name\" : \"10k run\", \"value\" : \"00:00:00\" }, { \"_id\" : \"6\", \"name\" : \"Half marathon\", \"value\" : \"00:00:00\" }, { \"_id\" : \"7\", \"name\" : \"Marathon\", \"value\" : \"00:00:00\" }], \"experience\" : [{ \"_id\" : \"0\", \"name\" : \"Swim\", \"value\" : \"\" }, { \"_id\" : \"2\", \"name\" : \"Run\", \"value\" : \"\" }, { \"_id\" : \"1\", \"name\" : \"Bike\", \"value\" : \"\" }, { \"_id\" : \"3\", \"name\" : \"Endurance\", \"value\" : \"\" }], \"selfRanking\" : [{ \"fieldId\" : \"0\", \"fieldName\" : \"Swim\", \"rank\" : \"1\" }, { \"fieldId\" : \"1\", \"fieldName\" : \"Bike\", \"rank\" : \"1\" }, { \"fieldId\" : \"2\", \"fieldName\" : \"Run\", \"rank\" : \"1\" }], \"events\" : [], \"fields\" : [{ \"fieldId\" : \"3\", \"fieldName\" : \"SWIM\", \"ZoneLevel\" : [{ \"_id\" : \"1\", \"name\" : \"Zone 1\", \"hr\" : \"112-130\", \"pace\" : \"150\", \"power\" : \"\" }, { \"_id\" : \"2\", \"name\" : \"Zone 2\", \"hr\" : \"132-140\", \"pace\" : \"150\", \"power\" : \"\" }, { \"_id\" : \"3\", \"name\" : \"Zone 3\", \"hr\" : \"142-149\", \"pace\" : \"150\", \"power\" : \"\" }, { \"_id\" : \"4\", \"name\" : \"Zone 4\", \"hr\" : \"151-168\", \"pace\" : \"150\", \"power\" : \"\" }, { \"_id\" : \"5\", \"name\" : \"Zone 5\", \"hr\" : \"170-173\", \"pace\" : \"150\", \"power\" : \"\" }], \"FTPACE\" : \"120\", \"FTPWATT\" : \"\", \"FTPHB\" : \"70\" }, { \"fieldId\" : \"1\", \"fieldName\" : \"BIKE\", \"ZoneLevel\" : [{ \"_id\" : \"1\", \"name\" : \"Zone 1\", \"hr\" : \"112-130\", \"pace\" : \"\", \"power\" : \"50\" }, { \"_id\" : \"2\", \"name\" : \"Zone 2\", \"hr\" : \"132-140\", \"pace\" : \"\", \"power\" : \"70\" }, { \"_id\" : \"3\", \"name\" : \"Zone 3\", \"hr\" : \"142-149\", \"pace\" : \"\", \"power\" : \"90\" }, { \"_id\" : \"4\", \"name\" : \"Zone 4\", \"hr\" : \"151-168\", \"pace\" : \"\", \"power\" : \"100\" }, { \"_id\" : \"4\", \"name\" : \"Zone 4\", \"hr\" : \"151-168\", \"pace\" : \"\", \"power\" : \"100\" }], \"FTPACE\" : \"\", \"FTPWATT\" : \"150\", \"FTPHB\" : \"100\" }, { \"fieldId\" : \"2\", \"fieldName\" : \"RUN\", \"ZoneLevel\" : [{ \"_id\" : \"1\", \"name\" : \"Zone 1\", \"hr\" : \"112-130\", \"pace\" : \"300\", \"power\" : \"50\" }, { \"_id\" : \"2\", \"name\" : \"Zone 2\", \"hr\" : \"132-140\", \"pace\" : \"300\", \"power\" : \"70\" }, { \"_id\" : \"3\", \"name\" : \"Zone 3\", \"hr\" : \"142-149\", \"pace\" : \"300\", \"power\" : \"90\" }, { \"_id\" : \"4\", \"name\" : \"Zone 4\", \"hr\" : \"151-168\", \"pace\" : \"300\", \"power\" : \"100\" }, { \"_id\" : \"5\", \"name\" : \"Zone 5\", \"hr\" : \"170-173\", \"pace\" : \"300\", \"power\" : \"110\" }], \"FTPACE\" : \"360\", \"FTPWATT\" : \"150\", \"FTPHB\" : \"100\" }] }, \"updatedBy\" : \"mobileListing\", \"lastUpdateDate\" : \"\", \"createdDate\" : \"\", \"auth\" : \"d@mail.com\", \"userName\" : \"dpick\", \"password\" : \"dpass\", \"sportComp\" : 0, \"sportCompKey\" : \"\" }";
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

		private string FormatJsonType(string jsonUser)
		{
			var returnString = jsonUser.Replace("ISODate(\"", "\"");
			returnString = returnString.Replace("\")", "\"");

			return returnString;
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
