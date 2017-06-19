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
using Android.Support.V4.Widget;

namespace HSEInformer.Fragments
{
    public class GroupsFragment : Android.Support.V4.App.Fragment
    {
        SwipeRefreshLayout swiperefresh;
        ApiManager _manager;
        ISharedPreferences _prefs;
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        ProgressBar progressBar;
        GroupList groupsList;
        GroupAdapter groupsAdapter;
        bool allgroups = false;


        public static GroupsFragment newInstance(bool allGroups)
        {
            GroupsFragment fragment = new GroupsFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            args.PutBoolean("allgroups", allGroups);
            return fragment;
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            _prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);
            var host = _prefs.GetString("host", null);
            _manager = new ApiManager(host);
            groupsList = new GroupList(new List<Model.Group>());

            base.OnCreate(savedInstanceState);
        }




        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.GroupsFragment, container, false);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);
            groupsAdapter = new GroupAdapter(Context, groupsList);
            recyclerView.SetAdapter(groupsAdapter);
            layoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);
            swiperefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            swiperefresh.Refresh += (e, s) => RefreshData();
            return view;
        }

        public void RefreshData()
        {
            if (!allgroups)
            {
                ShowUserGroups();
            }
            else
            {
                ShowAllGroups();
            }
            swiperefresh.Refreshing = false;
        }



        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            allgroups = Arguments.GetBoolean("allgroups");
            if (!allgroups)
            {
                ShowUserGroups();
            }
            else
            {
                ShowAllGroups();
            }
        }


        public async void ShowUserGroups()
        {
            var token = _prefs.GetString("token", null);

            if (token != null && ((Activity is MainActivity && (Activity as MainActivity).CheckConnection())
                || (Activity is AddActivity && (Activity as AddActivity).CheckConnection())))
            {
                try
                {
                    progressBar.Visibility = ViewStates.Visible;
                    recyclerView.Visibility = ViewStates.Gone;
                    var groups = await _manager.GetGroups(token, false);
                    if (groups != null)
                    {
                        groupsList.Groups = groups;
                        groupsAdapter.NotifyDataSetChanged();
                        groupsAdapter.ItemClick += groupsAdapter_ItemClick;
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
                        if (Activity is MainActivity)
                        {
                            (Activity as MainActivity).LogOut();
                        }
                        else if (Activity is AddActivity)
                        {
                            (Activity as AddActivity).Finish();
                        }

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
                    recyclerView.Visibility = ViewStates.Visible;
                }
            }
        }

        public async void ShowAllGroups()
        {
            var token = _prefs.GetString("token", null);

            if (token != null && ((Activity is MainActivity && (Activity as MainActivity).CheckConnection())
                || (Activity is AddActivity && (Activity as AddActivity).CheckConnection())))
            {
                try
                {
                    progressBar.Visibility = ViewStates.Visible;
                    recyclerView.Visibility = ViewStates.Gone;
                    var groups = await _manager.GetGroups(token,true);
                    if (groups != null)
                    {
                        groupsList.Groups = groups;
                        groupsAdapter.NotifyDataSetChanged();
                        groupsAdapter.ItemClick += groupsAdapter_ItemClick;
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
                        if (Activity is MainActivity)
                        {
                            (Activity as MainActivity).LogOut();
                        }
                        else if (Activity is AddActivity)
                        {
                            (Activity as AddActivity).Finish();
                        }

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
                    recyclerView.Visibility = ViewStates.Visible;
                }
            }
        }
        private void groupsAdapter_ItemClick(Model.Group group)
        {
            if (!allgroups)
            {
                var intent = new Intent(Context, typeof(GroupContentActivity));
                intent.PutExtra("group_id", group.Id);
                intent.PutExtra("group_name", group.Name);
                StartActivity(intent);
            }
            else
            {
                SendPostPermission(group.Id);
            }
        }

        private async void SendPostPermission(int id)
        {
            var token = _prefs.GetString("token", null);

            if (token != null && ((Activity is MainActivity && (Activity as MainActivity).CheckConnection())
                || (Activity is AddActivity && (Activity as AddActivity).CheckConnection())))
            {
                try
                {
                    progressBar.Visibility = ViewStates.Visible;
                    recyclerView.Visibility = ViewStates.Gone;
                    await _manager.SendRequest(token, id);

                    var dialog = new Android.App.AlertDialog.Builder(Context);
                    string message = "Запрос был послан";
                    dialog.SetMessage(message);
                    dialog.SetPositiveButton("Ок", delegate { });
                    dialog.Show();


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
                        if (Activity is MainActivity)
                        {
                            (Activity as MainActivity).LogOut();
                        }
                        else if (Activity is AddActivity)
                        {
                            (Activity as AddActivity).Finish();
                        }

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
                    recyclerView.Visibility = ViewStates.Visible;
                }
            }
        }
    }
}