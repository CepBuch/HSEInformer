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
using Android.Preferences;
using HSEInformer.GroupViewModel;
using Android.Support.V7.Widget;
using System.Threading.Tasks;

namespace HSEInformer.Fragments
{
    public class GroupsFragment : Android.Support.V4.App.Fragment
    {
        ApiManager _manager;
        ISharedPreferences _prefs;
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        ProgressBar progressBar;
        GroupList groupsList;
        GroupAdapter groupsAdapter;

        public static GroupsFragment newInstance()
        {
            GroupsFragment fragment = new GroupsFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            return fragment;
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            _prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);
            var host = _prefs.GetString("host", null);
            _manager = new ApiManager(host);

            base.OnCreate(savedInstanceState);
        }

        public async override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            await ShowGroups();

        }
        public async Task ShowGroups()
        {
            var token = _prefs.GetString("token", null);

            if (token != null)
            {
                progressBar.Visibility = ViewStates.Visible;
                recyclerView.Visibility = ViewStates.Gone;
                var groups = await _manager.GetGroups(token);
                groupsList = new GroupList(groups);
                groupsAdapter = new GroupAdapter(Context, groupsList);
                recyclerView.SetAdapter(groupsAdapter);

                groupsAdapter.ItemClick += _groupsAdapter_ItemClick;
                progressBar.Visibility = ViewStates.Gone;
                recyclerView.Visibility = ViewStates.Visible;
            }
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.GroupsFragment, container, false);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);
            layoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);
            return view;
          
        }

        




        private void _groupsAdapter_ItemClick(Model.Group group)
        {
            Toast.MakeText(Context, $"{group.Name} ({group.Id})", ToastLength.Short).Show();
        }
    }
}