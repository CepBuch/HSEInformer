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
using Android.Support.V7.Widget;
using HSEInformer.GroupMemberViewModel;
using Android.Preferences;

namespace HSEInformer.Fragments.GroupContent
{
    class GroupMembersFragment : Android.Support.V4.App.Fragment
    {
        private static string GROUP_ID = "group_id";
        LinearLayout contentLayout;
        TextView adminNameTextView;
        TextView adminUsernameTextView;
        RecyclerView recyclerViewMembers;
        RecyclerView.LayoutManager layoutManagerMembers;
        RecyclerView recyclerViewPermissions;
        RecyclerView.LayoutManager layoutManagerPermissions;

        ISharedPreferences prefs;
        GroupMemberList membersList;
        GroupMemberAdapter membersAdapter;
        GroupMemberList permissionsList;
        GroupMemberAdapter permissionsAdapter;
        ApiManager _manager;
        ProgressBar progressBar;
        public static GroupMembersFragment newInstance(int group_id)
        {
            GroupMembersFragment fragment = new GroupMembersFragment();
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
            membersList = new GroupMemberList(new List<Model.User>());
            permissionsList = new GroupMemberList(new List<Model.User>());
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.GroupMembersContainerFragment, container, false);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
            adminNameTextView = view.FindViewById<TextView>(Resource.Id.member_name);
            adminUsernameTextView = view.FindViewById<TextView>(Resource.Id.member_username);

            contentLayout = view.FindViewById<LinearLayout>(Resource.Id.mainContentLayout);
            recyclerViewMembers = view.FindViewById<RecyclerView>(Resource.Id.recycler_view_members);
            recyclerViewPermissions = view.FindViewById<RecyclerView>(Resource.Id.recycler_view_permissions);
            membersAdapter = new GroupMemberAdapter(membersList);
            permissionsAdapter = new GroupMemberAdapter(permissionsList);
            recyclerViewMembers.SetAdapter(membersAdapter);
            recyclerViewPermissions.SetAdapter(permissionsAdapter);
            layoutManagerMembers = new LinearLayoutManager(Activity);
            layoutManagerPermissions = new LinearLayoutManager(Activity);
            recyclerViewMembers.SetLayoutManager(layoutManagerMembers);
            recyclerViewPermissions.SetLayoutManager(layoutManagerPermissions);
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            var group_id = Arguments.GetInt(GROUP_ID);
            if (group_id > 0)
            {
                ShowGroupMembers(group_id);
            }
        }

        public async void ShowGroupMembers(int group_id)
        {
            var token = prefs.GetString("token", null);

            if (token != null && (Activity as GroupContentActivity).CheckConnection())
            {
                try
                {
                    progressBar.Visibility = ViewStates.Visible;
                    contentLayout.Visibility = ViewStates.Gone;
                    var administrator = await _manager.GetAdministrator(token, group_id);
                    var members = await _manager.GetGroupMembers(token, group_id);
                    var permissions = await _manager.GetPostPermissions(token, group_id);

                    adminNameTextView.Text = administrator != null ?
                        $"{administrator.Surname} {administrator.Name} {administrator.Patronymic}" : "Отсусутствует";
                    adminUsernameTextView.Text = administrator != null ? administrator.Email : "Пока без администратора";

                    if (members != null)
                    {
                        membersList.Members = members;
                        membersAdapter.NotifyDataSetChanged();
                    }

                    if(permissions != null)
                    {
                        permissionsList.Members = permissions;
                        permissionsAdapter.NotifyDataSetChanged();
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
                }
            }
        }
    }
}