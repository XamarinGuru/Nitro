﻿using System.Collections.Generic;
using Android.App;
using Android.Content;
using Newtonsoft.Json;
using PortableLibrary;

namespace goheja
{
	public static class AppSettings
	{
		private static ISharedPreferences _appSettings = Application.Context.GetSharedPreferences("App_settings", FileCreationMode.Private);

		public static BaseActivity baseVC;

		public static List<GoHejaEvent> currentEventsList;
		public static EventTotal currentEventTotal;
		public static GoHejaEvent selectedEvent;

		public static List<AthleteInSubGroup> selectedAthletesInSubGroup;

		public static bool isFakeUser;

		private const string userKey = "userKey";
		public static LoginUser CurrentUser
		{
			get
			{
				try
				{
					var strCurrentUser = _appSettings.GetString(userKey, "");
					return JsonConvert.DeserializeObject<LoginUser>(strCurrentUser);
				}
				catch
				{
					return null;
				}
			}
			set
			{
				var strUser = JsonConvert.SerializeObject(value);
				ISharedPreferencesEditor editor = _appSettings.Edit();
				editor.PutString(userKey, strUser);
				editor.Apply();
			}
		}

		private const string deviceUDIDKey = "deviceUDID";
		public static string DeviceUDID
		{
			get
			{
				return _appSettings.GetString(deviceUDIDKey, null);
			}
			set
			{
				ISharedPreferencesEditor editor = _appSettings.Edit();
				editor.PutString(deviceUDIDKey, value);
				editor.Apply();
			}
		}
	}
}
