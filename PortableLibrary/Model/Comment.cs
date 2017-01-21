using System;
using System.Collections.Generic;

namespace PortableLibrary
{
	public class Content
	{
		public Content()
		{
		}
		public string commentId { get; set; }
		public string author { get; set; }
		public string authorId { get; set;}
		public string commentText { get; set;}
		public string date { get; set;}
	}

	public class Comment
	{
		public Comment()
		{
		}
		public string ownerId { get; set;}
		public string type { get; set;}
		public List<Content> comments { get; set; }
	}
}
