
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace goheja
{
	public class TimeFormatDialog : DialogFragment
	{
		private NumberPicker[] numPickers;

		private int numDials;
		private int currentValue;

		private string type;

		private static string ARG_numDials = "numDials";
		private static string ARG_initValue = "initValue";

		private EditText textView;

		public static TimeFormatDialog newInstance(EditText textView, int numDials, string type)
		{
			TimeFormatDialog numdialog = new TimeFormatDialog();
			numdialog.textView = textView;
			numdialog.type = type;
			Bundle args = new Bundle();
			args.PutInt(ARG_numDials, numDials);
			numdialog.Arguments = args;
			return numdialog;
		}

		public TimeFormatDialog()
		{
			// Required empty public constructor
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if (Arguments != null)
			{
				numDials = Arguments.GetInt(ARG_numDials);
				currentValue = Arguments.GetInt(ARG_initValue);
				numPickers = new NumberPicker[numDials];

				if (savedInstanceState != null)
				{
					currentValue = savedInstanceState.GetInt("CurrentValue");
				}
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LinearLayout linLayoutH = new LinearLayout(this.Activity);

			LinearLayout.LayoutParams params1 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			linLayoutH.LayoutParameters = params1;

			if (type == "hr")
			{
				var minValue = 60;
				var maxValue = 250;
				for (int i = 0; i < numDials; i++)
				{
					numPickers[numDials - i - 1] = new NumberPicker(this.Activity);
					numPickers[numDials - i - 1].MaxValue = 19;
					numPickers[numDials - i - 1].MinValue = 0;
					numPickers[numDials - i - 1].Value = 0;//getDigit(currentValue, numDials - i - 1);

					string[] valSet = new string[20];
					for (int val = minValue; val <= maxValue; val += 10)
					{
						valSet[(val - 60) / 10] = val.ToString();
					}
					numPickers[numDials - i - 1].SetDisplayedValues(valSet);
					linLayoutH.AddView(numPickers[numDials - i - 1]);
				}
			}
			else {
				var maxValue = numDials == 1 ? 5 : 59;
				var minValue = numDials == 1 ? 1 : 0;
				for (int i = 0; i < numDials; i++)
				{
					numPickers[numDials - i - 1] = new NumberPicker(this.Activity);
					numPickers[numDials - i - 1].MaxValue = maxValue;
					numPickers[numDials - i - 1].MinValue = minValue;
					numPickers[numDials - i - 1].Value = 0;//getDigit(currentValue, numDials - i - 1);
					linLayoutH.AddView(numPickers[numDials - i - 1]);
				}
			}

			LinearLayout linLayoutV = new LinearLayout(this.Activity);
			linLayoutV.Orientation = Orientation.Vertical;
			linLayoutV.AddView(linLayoutH);

			Button okButton = new Button(this.Activity);
			okButton.Click += (sender, e) => {
				string strText = "";
				if (type == "hr")
				{
					strText = (60 + numPickers[1].Value * 10) + "-" + (60 + numPickers[0].Value * 10);
				}else if (numDials == 1)
				{
					strText = numPickers[0].Value.ToString();
				}
				else {
					strText = String.Format("{0:00}", numPickers[1].Value) + ":" + String.Format("{0:00}", numPickers[0].Value);
					if (numDials > 2)
						strText = String.Format("{0:00}", numPickers[2].Value) + ":" + strText;
				}
				textView.Text = strText;
				Dismiss();
			};

			params1.Gravity = GravityFlags.CenterHorizontal;
			okButton.LayoutParameters = params1;
			okButton.Text = "Done";

			linLayoutV.AddView(okButton);
			return linLayoutV;

		}

		public void onSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			outState.PutInt("CurrentValue", getValue());
		}

		private int getValue()
		{
			int value = 0;
			int mult = 1;
			for (int i = 0; i < numDials; i++)
			{
				value += numPickers[i].Value * mult;
				mult *= 10;
			}
			return value;
		}
	}
}
