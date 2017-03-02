using System;
using System.Collections.Generic;

namespace PortableLibrary
{
	public class Legend
	{
		public bool useGraphSettings { get; set; }
	}
	public class ReportData
	{
		public string date { get; set; }
		public double tsb { get; set; }
		public double atl { get; set; }
		public double ctl { get; set; }
		public double dayliIf { get; set; }
		public double interval { get; set; }
		public double dayliTss { get; set; }
		public double tssth { get; set; }
		public double test { get; set; }
	}
	public class ReportAxis
	{
		public string id { get; set; }
		public int offset { get; set; }
		public string axisColor { get; set; }
		public double axisThickness { get; set; }
		public double axisAlpha { get; set; }
		public string position { get; set;}
	}
	public class ReportRegend
	{
		public string valueAxis { get; set; }
		public int bulletSize { get; set; }
		public string lineColor { get; set; }
		public string bullet { get; set; }
		public double bulletBorderThickness { get; set; }
		public int hideBulletsCount { get; set; }
		public string title { get; set; }
		public string balloonText { get; set; }
		public double lineAlpha { get; set; }
		public string valueField { get; set; }
		public double fillAlphas { get; set; }
	}
	public class CategoryAxis
	{
		public string parseDates { get; set; }
		public string axisColor { get; set; }
		public bool minorGridEnabled { get; set; }
		public List<AxisGuide> guides { get; set; }
	}
	public class AxisGuide
	{
		public bool above { get; set; }
		public string category { get; set; }
		public string toCategory { get; set; }
		public string lineColor { get; set; }
		public int lineAlpha { get; set; }
		public double fillAlpha { get; set; }
		public string fillColor { get; set; }
		public int dashLength { get; set; }
		public bool inside { get; set; }
		public int labelRotation { get; set; }
		public string label { get; set; }
	}
	public class Export
	{
		public bool enabled { get; set; }
		public string position { get; set; }
	}

	public class ReportGraphData
	{
		public string type { get; set; }
		public string theme { get; set; }
		public Legend legend { get; set; }
		public List<ReportData> dataProvider { get; set; }
		public bool synchronizeGrid { get; set; }
		public List<ReportAxis> valueAxes { get; set; }
		public List<ReportRegend> graphs { get; set; }
		public string categoryField { get; set; }
		public CategoryAxis categoryAxis { get; set; }
		public Export export { get; set; }

		public IList<object> GetSalesDataList()
		{
			var list = new List<object>();
			for (int i = 0; i < dataProvider.Count; i++)
			{
				list.Add(dataProvider[i]);
			}
			return list;
		}

		public int TodayIndex()
		{
			for (int i = 0; i < dataProvider.Count; i++)
			{
				var pDate = DateTime.Parse(dataProvider[i].date);
				if (pDate.DayOfYear == DateTime.Now.DayOfYear)
					return i;
			}
			return -1;
		}
	}

}
