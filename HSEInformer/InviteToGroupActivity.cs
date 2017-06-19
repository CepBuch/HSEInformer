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
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Net;
using Android.Preferences;
using HSEInformer.InviteViewModek;
using HSEInformer.GroupMemberViewModel;
using Android.Support.V7.Widget;

namespace HSEInformer
{
    [Activity(Theme = "@style/AppTheme")]
    public class InviteToGroupActivity : AppCompatActivity
    {
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        ISharedPreferences prefs;
        ApiManager _manager;
        ProgressBar progressBar;
        LinearLayout contentLayout;
        SupportToolbar toolbar;
        InviteAdapter inviteAdapter;
        GroupMemberList invitesList;
        private int id;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.InviteActivity);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            var host = prefs.GetString("host", null);
            _manager = new ApiManager(host);
            invitesList = new GroupMemberList(new List<Model.User>());
            CustomizeToolbarAndViewPager();
            var group_id = Intent.GetIntExtra("group_id", 0);
            if(group_id > 0)
            {
                ShowUsers(group_id);
                id = group_id;
            }
            
            base.OnCreate(savedInstanceState);
        }

        public void CustomizeToolbarAndViewPager()
        {
            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar_invite);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            toolbarTitle.Text = "Добавление и поиск";

            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            contentLayout = FindViewById<LinearLayout>(Resource.Id.mainContentLayout);
            recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            inviteAdapter = new InviteAdapter(invitesList);
            inviteAdapter.ItemClick += InviteAdapter_ItemClick;
            recyclerView.SetAdapter(inviteAdapter);
            layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);

        }

        private async void InviteAdapter_ItemClick(Model.User obj)
        {
            var token = prefs.GetString("token", null);

            if (token != null && CheckConnection())
            {
                try
                {

                    await _manager.SendInvite(token, id, obj.Email);
                    var dialog = new Android.App.AlertDialog.Builder(this);
                    string message = $"Пользователь {obj.Email} был приглашен в группу";
                    dialog.SetMessage(message);
                    dialog.SetPositiveButton("Ок", delegate { });
                    dialog.Show();
                }
                catch (UnauthorizedAccessException)
                {
                    var dialog = new Android.App.AlertDialog.Builder(this);
                    string message = "Ваши параметры авторизации устарели." +
                        "\nВы будете возвращены на страницу авторизации, чтобы пройти процедуру авторизации заново";
                    dialog.SetMessage(message);
                    dialog.SetCancelable(false);
                    dialog.SetPositiveButton("Ок", delegate
                    {
                        Finish();
                    });
                    dialog.Show();
                }
                catch (Exception ex)
                {
                    var dialog = new Android.App.AlertDialog.Builder(this);
                    string message = ex.Message;
                    dialog.SetMessage(message);
                    dialog.SetPositiveButton("Ок", delegate { });
                    dialog.Show();
                }
 
            }
        }

        public bool CheckConnection()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo info = connectivityManager.ActiveNetworkInfo;

            if (info != null && info.IsConnected)
            {
                return true;
            }
            else
            {
                var dialog = new Android.App.AlertDialog.Builder(this);
                string message = "Для работы приложения необходимо интернет-соединение";
                string title = "Нет интернет соединения";
                dialog.SetTitle(title);
                dialog.SetMessage(message);

                dialog.SetCancelable(false);
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Повторить", delegate { this.Recreate(); });
                dialog.SetNegativeButton("Выйти", delegate { this.Finish(); });
                dialog.Show();
                return false;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    {
                        Finish();
                        return true;
                    }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }


        public async void ShowUsers(int group_id)
        {
            var token = prefs.GetString("token", null);

            if (token != null && CheckConnection())
            {
                try
                {
                    progressBar.Visibility = ViewStates.Visible;
                    contentLayout.Visibility = ViewStates.Gone;
                    var invites = await _manager.GetUsersToInvite(token, group_id);

                    if (invites != null)
                    {
                        invitesList.Members = invites;
                        inviteAdapter.NotifyDataSetChanged();
                    }

                }
                catch (UnauthorizedAccessException)
                {
                    var dialog = new Android.App.AlertDialog.Builder(this);
                    string message = "Ваши параметры авторизации устарели." +
                        "\nВы будете возвращены на страницу авторизации, чтобы пройти процедуру авторизации заново";
                    dialog.SetMessage(message);
                    dialog.SetCancelable(false);
                    dialog.SetPositiveButton("Ок", delegate
                    {
                        Finish();
                    });
                    dialog.Show();
                }
                catch (Exception ex)
                {

                    var dialog = new Android.App.AlertDialog.Builder(this);
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