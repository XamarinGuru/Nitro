using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
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
