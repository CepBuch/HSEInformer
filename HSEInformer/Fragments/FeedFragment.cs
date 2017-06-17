using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Widget;
using System.Threading.Tasks;
using Android.Preferences;

namespace HSEInformer.Fragments
{
    public class FeedFragment : Android.Support.V4.App.Fragment
    {
        SwipeRefreshLayout swipeRefreshLayout;
        TextView infoTextView;
        ApiManager _manager;
        ISharedPreferences _prefs;
        public static FeedFragment newInstance()
        {
            FeedFragment fragment = new FeedFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            _prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);
            var host = _prefs.GetString("host", null);
            _manager = new ApiManager(host);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.FeedFragment, container, false);
            swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            infoTextView = view.FindViewById<TextView>(Resource.Id.infoTxt);
            int i = 0;
            swipeRefreshLayout.Refresh += async (e, s) =>
            {
                await Task.Delay(3000);
                infoTextView.Text = $"Refreshed {++i}";
                swipeRefreshLayout.Refreshing = false;
            };

            return view;
        }


    }
}