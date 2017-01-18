﻿using System;

namespace PortableLibrary
{
	public class NitroEvent
	{
		public NitroEvent()
		{
		}

		public string _id { get; set;}
		public string title { get; set; }
		public string eventSysId { get; set; }
		public string userId { get; set; }
		public string ParentGroupName { get; set; }
		public string ParentGroupId { get; set; }
		public string GroupId { get; set; }
		public string GroupName { get; set; }
		public string start { get; set; }
		public string end { get; set; }
		public string eventColor { get; set; }
		public string eventDate { get; set; }
		public string distance { get; set; }
		public string type { get; set; }
		public string notes { get; set; }
		public string attended { get; set; }
		public string programName { get; set; }
		public string programStart { get; set; }
		public string programEnd { get; set; }
		public string durHrs { get; set; }
	}
}
