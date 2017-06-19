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
using Android.Preferences;
using HSEInformer.Fragments.AddAndSearch;
using Java.Lang;
using HSEInformer.Fragments;

namespace HSEInformer
{
    class AddActivityPagerAdapter : FragmentStatePagerAdapter
    {
        List<Android.Support.V4.App.Fragment> fragments;
        List<string> titles;
        ISharedPreferences _prefs;
        ApiManager _manager;
        Context context;

        public AddActivityPagerAdapter(Android.Support.V4.App.FragmentManager fm, Context context) : base(fm)
        {
            this.context = context;
            _prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            FillTabs();
        }

        public void FillTabs()
        {
            var host = _prefs.GetString("host", null);
            _manager = new ApiManager(host);
            var token = _prefs.GetString("token", null);

            fragments = new List<Android.Support.V4.App.Fragment>
            {
                InvitesFragment.newInstance(),
                GroupsFragment.newInstance(true),
                CreateGroupFragment.newInstance()
            };
            titles = new List<string>
            {
               "Приглашения",
               "Найти",
               "Добавить"
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
                return fragments != null ? fragments.Count : 0;
            }
        }
    }

}