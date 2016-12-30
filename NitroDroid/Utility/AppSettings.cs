using System;
using Android.App;
using Android.Content;

namespace goheja
{
	public static class AppSettings
	{
		private static ISharedPreferences _appSettings = Application.Context.GetSharedPreferences("App_settings", FileCreationMode.Private);

		private const string userIDKey = "userID";
		public static string UserID
		{
			get
			{
				return _appSettings.GetString(userIDKey, null);
			}
			set
			{
				ISharedPreferencesEditor editor = _appSettings.Edit();
				editor.PutString(userIDKey, value);
				editor.Apply();
			}
		}

		private const string emailKey = "email";
		public static string Email
		{
			get
			{
				return _appSettings.GetString(emailKey, null);
			}
			set
			{
				ISharedPreferencesEditor editor = _appSettings.Edit();
				editor.PutString(emailKey, value);
				editor.Apply();
			}
		}

		private const string passwordKey = "password";
		public static string Password
		{
			get
			{
				return _appSettings.GetString(passwordKey, null);
			}
			set
			{
				ISharedPreferencesEditor editor = _appSettings.Edit();
				editor.PutString(passwordKey, value);
				editor.Apply();
			}
		}

	}
}
