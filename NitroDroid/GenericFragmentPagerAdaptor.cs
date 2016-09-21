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
    }
}

//end by Afroz date 1/9/2016