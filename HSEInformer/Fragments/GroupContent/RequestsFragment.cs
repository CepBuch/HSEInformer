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
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using HSEInformer.GroupMemberViewModel;
using Android.Preferences;
using HSEInformer.RequestViewModel;

namespace HSEInformer.Fragments.GroupContent
{
    public class RequestsFragment : Android.Support.V4.App.Fragment
    {
        private static string GROUP_ID = "group_id";
        SwipeRefreshLayout swiperefresh;
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        ISharedPreferences prefs;
        ApiManager _manager;
        ProgressBar progressBar;
        GroupMemberList requestsList;
        RequestAdapter requestsAdapter;
        Button inviteButton;
        LinearLayout contentLayout;


        public static RequestsFragment newInstance(int group_id)
        {
            RequestsFragment fragment = new RequestsFragment();
            Bundle args = new Bundle();
            args.PutInt(GROUP_ID, group_id);
            fragment.Arguments = args;
            return fragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);
            var host = prefs.GetString("host", null);
            _manager = new ApiManager(host);
            requestsList = new GroupMemberList(new List<Model.User>());
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.PostPermissionsFragment, container, false);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
            inviteButton = view.FindViewById<Button>(Resource.Id.inviteButton);
            contentLayout = view.FindViewById<LinearLayout>(Resource.Id.mainContentLayout);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);
            requestsAdapter = new RequestAdapter(requestsList);
            requestsAdapter.ItemClick += (user, accpted) => Toast.MakeText(Context, $"{accpted} for {user}", ToastLength.Short).Show();
            recyclerView.SetAdapter(requestsAdapter);
            layoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);
            swiperefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            var group_id = Arguments.GetInt(GROUP_ID);
            swiperefresh.Refresh += (e, s) => ShowRequests(group_id);
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            var group_id = Arguments.GetInt(GROUP_ID);
            if (group_id > 0)
            {
                ShowRequests(group_id);
            }
        }

        public async void ShowRequests(int group_id)
        {
            var token = prefs.GetString("token", null);

            if (token != null && (Activity as GroupContentActivity).CheckConnection())
            {
                try
                {
                    progressBar.Visibility = ViewStates.Visible;
                    contentLayout.Visibility = ViewStates.Gone;
                    var requests = await _manager.GetPostPermissionRequests(token, group_id);

                    if (requests != null)
                    {
                        requestsList.Members = requests;
                        requestsAdapter.NotifyDataSetChanged();
                    }

                }
                catch (UnauthorizedAccessException)
                {
                    var dialog = new Android.App.AlertDialog.Builder(Context);
                    string message = "Ваши параметры авторизации устарели." +
                        "\nВы будете возвращены на страницу авторизации, чтобы пройти процедуру авторизации заново";
                    dialog.SetMessage(message);
                    dialog.SetCancelable(false);
                    dialog.SetPositiveButton("Ок", delegate
                    {
                        (Activity as GroupContentActivity).Finish();

                    });
                    dialog.Show();
                }
                catch (Exception ex)
                {

                    var dialog = new Android.App.AlertDialog.Builder(Context);
                    string message = ex.Message;
                    dialog.SetMessage(message);
                    dialog.SetPositiveButton("Ок", delegate { });
                    dialog.Show();
                }
                finally
                {
                    progressBar.Visibility = ViewStates.Gone;
                    contentLayout.Visibility = ViewStates.Visible;
                    swiperefresh.Refreshing = false;
                }
            }
        }
    }
}