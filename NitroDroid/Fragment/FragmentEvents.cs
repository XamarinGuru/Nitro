using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

namespace goheja
{
    public class FragmentEvents : Android.Support.V4.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			return inflater.Inflate(Resource.Layout.fEvents, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            InitializeComponent(view);
        }

        private void InitializeComponent(View view)
        {
            var whatItLabel = view.FindViewById<TextView>(Resource.Id.whatItLabel);
            whatItLabel.SetTypeface(Typeface.CreateFromAsset(Activity.Assets, "font/arialbd.ttf"), TypefaceStyle.Bold);

            view.FindViewById<ImageView>(Resource.Id.icon01).Click += delegate{ Icon_click(1); };
            view.FindViewById<ImageView>(Resource.Id.icon02).Click += delegate{ Icon_click(2); };

			///coming soon
            //Icon03 = view.FindViewById<ImageView>(Resource.Id.icon03);
            //Icon04 = view.FindViewById<ImageView>(Resource.Id.icon04);
            //Icon05 = view.FindViewById<ImageView>(Resource.Id.icon05);
            //Icon06 = view.FindViewById<ImageView>(Resource.Id.icon06);
            //Icon07 = view.FindViewById<ImageView>(Resource.Id.icon07);
        }
        private void Icon_click(int iconNumber)
        {
            var intent = new Intent(Activity, typeof(AnalyticsActivity));
            intent.PutExtra("EventNumber", iconNumber);
            StartActivity(intent);
        }
    }
}
