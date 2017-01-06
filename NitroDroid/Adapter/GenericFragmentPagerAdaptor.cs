using Android.Support.V4.App;
using System;

namespace goheja
{
    public class GenericFragmentPagerAdaptor : FragmentPagerAdapter
    {
        public event EventHandler TabChanged;

        public GenericFragmentPagerAdaptor(FragmentManager fm, FragmentActivity activity) : base(fm)
        {
        }

        public override int Count
        {
            get { return 4; }
        }

        public override Fragment GetItem(int position)
        {
			if (position == 0)
                return new FragmentCalendar();
            if (position == 1)
				return new FragmentEvents();
            if (position == 2)
				return new FragmentProfile();
			if (position == 3)
				return new FragmentSerious();

            TabChanged?.Invoke(position, null);

            return null;
        }
    }
}

