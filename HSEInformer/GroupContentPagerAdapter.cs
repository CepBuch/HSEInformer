using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Java.Lang;
using Android.Preferences;

namespace HSEInformer
{
    class GroupContentPagerAdapter : FragmentStatePagerAdapter
    {
        List<Android.Support.V4.App.Fragment> fragments;
        List<string> titles;
        ISharedPreferences _prefs;
        ApiManager _manager;
        int group_id;
        Context context;

        public GroupContentPagerAdapter(Android.Support.V4.App.FragmentManager fm, int group_id, Context context) : base(fm)
        {
            this.group_id = group_id;
            this.context = context;
            _prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            FillTabs();
        }

        public async void FillTabs()
        {
            var host = _prefs.GetString("host", null);
            _manager = new ApiManager(host);

            //TODO await _manager.GetGroupContent();

            fragments = new List<Android.Support.V4.App.Fragment>
            {
                //RisksContainerFragment.newInstance(category_id) ,
                //CategoriesContainerFragment.newInstance(category_id)
            };
            titles = new List<string>
            {

            };
        }


        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return fragments[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(titles[position]);
        }

        public override int Count
        {
            get
            {
                return fragments.Count;
            }
        }
    }
}