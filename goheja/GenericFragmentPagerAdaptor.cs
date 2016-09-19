using Android.Support.V4.App;
using System;

namespace goheja
{
    //by Afroz date 1/9/2016
    public class GenericFragmentPagerAdaptor : FragmentPagerAdapter
    {
        private FragmentActivity _activity;

        public event EventHandler TabChanged;

        public GenericFragmentPagerAdaptor(FragmentManager fm, FragmentActivity activity)
            : base(fm)
        {
            _activity = activity;
        }

        public override int Count
        {
            get { return 3; }
        }

        public override Fragment GetItem(int position)
        {
            if (position == 0)
                return new FragmentCalendar();
            if (position == 1)
                return new NewMainActivity();
            if (position == 2)
                return new FragmentPersonalData();

            TabChanged?.Invoke(position, null);

            return null;
        }

        //public override ICharSequence GetPageTitleFormatted(int position)
        //{
        //    Drawable myDrawable = _activity.GetDrawable(Resource.Drawable.calender_icon_inactive_new);
        //    if (position == 0)
        //    {
        //        myDrawable = _activity.GetDrawable(Resource.Drawable.calender_icon_inactive_new);
        //    }
        //    else if (position == 1)
        //    {
        //        myDrawable = _activity.GetDrawable(Resource.Drawable.home_icon);
        //    }
        //    else if (position == 2)
        //    {
        //        myDrawable = _activity.GetDrawable(Resource.Drawable.user_inactive_new);
        //    }

        //    SpannableStringBuilder sb = new SpannableStringBuilder(" ");
        //    myDrawable.SetBounds(0, 0, myDrawable.IntrinsicWidth, myDrawable.IntrinsicHeight);
        //    ImageSpan span = new ImageSpan(myDrawable, SpanAlign.Baseline);
        //    sb.SetSpan(span, 0, 1,SpanTypes.ExclusiveExclusive);
        //    return sb;
        //}
    }

    //public class ViewPageListenerForActionBar : ViewPager.SimpleOnPageChangeListener
    //{
    //    private ActionBar _bar;
    //    public ViewPageListenerForActionBar(ActionBar bar)
    //    {
    //        _bar = bar;
    //    }
    //    public override void OnPageSelected(int position)
    //    {
    //        _bar.SetSelectedNavigationItem(position);
    //    }
    //}
    //public static class ViewPagerExtensions
    //{
    //    public static ActionBar.Tab GetViewPageTab(this ViewPager viewPager, ActionBar actionBar, string name)
    //    {
    //        var tab = actionBar.NewTab();
    //        tab.SetText(name);
    //        tab.TabSelected += (o, e) =>
    //        {
    //            viewPager.SetCurrentItem(actionBar.SelectedNavigationIndex, false);
    //        };
    //        return tab;
    //    }
    //}
}

//end by Afroz date 1/9/2016