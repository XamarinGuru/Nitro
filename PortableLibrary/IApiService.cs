using System;
using System.Collections.Generic;

namespace PortableLibrary
{
	public interface IApiService
	{
		#region user management
		string RegisterUser(string fName, string lName, string deviceId, string userName, string psw, string email, int age, bool ageSpecified = true, bool acceptedTerms = true, bool acceptedTermsSpecified = true);
		bool LoginUser(string email, string password);
		void SignOutUser();
		string GetCode(string email);
		int ResetPassword(string email, string password);
		string GetUserID();
		RootMember GetUserObject();
		string UpdateUserDataJson(RootMember updatedUserObject, string updatedById = null);
		#endregion

		#region event management
		Gauge GetGauge();
		List<NitroEvent> GetPastEvents();
		#endregion
	}
}
