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
using HSEInformer.GroupViewModel;
using Android.Support.V4.Widget;
using HSEInformer.InviteViewModek;
using Android.Preferences;
using HSEInformer.InviteResponseViewModel;

namespace HSEInformer.Fragments.AddAndSearch
{
    public class InvitesFragment : Android.Support.V4.App.Fragment
    {
        SwipeRefreshLayout swiperefresh;
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        ISharedPreferences prefs;
        ApiManager _manager;
        ProgressBar progressBar;
        GroupList groupsList;
        InviteResponseAdapter invitesAdapter;
        LinearLayout contentLayout;


        public static InvitesFragment newInstance()
        {
            InvitesFragment fragment = new InvitesFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            return fragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);
            var host = prefs.GetString("host", null);
            _manager = new ApiManager(host);
            groupsList = new GroupList(new List<Model.Group>());
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.InvitesFragment, container, false);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
            contentLayout = view.FindViewById<LinearLayout>(Resource.Id.mainContentLayout);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);
            invitesAdapter = new InviteResponseAdapter(groupsList);
            invitesAdapter.ItemClick += (group, accepted) => AnswerInvite(group.Id, accepted);
            recyclerView.SetAdapter(invitesAdapter);
            layoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);
            swiperefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swiperefresh);
            swiperefresh.Refresh += (e, s) => ShowInvites();
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            ShowInvites();
        }

        public async void ShowInvites()
        {
            var token = prefs.GetString("token", null);

            if (token != null && (Activity as AddActivity).CheckConnection())
            {
                try
                {
                    progressBar.Visibility = ViewStates.Visible;
                    contentLayout.Visibility = ViewStates.Gone;
                    var invites = await _manager.GetUserInvites(token);

                    if (invites != null)
                    {
                        groupsList.Groups = invites;
                        invitesAdapter.NotifyDataSetChanged();
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
                        (Activity as AddActivity).Finish();

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

        private async void AnswerInvite(int group_id, bool accepted)
        {
            var token = prefs.GetString("token", null);

            if (token != null && (Activity as AddActivity).CheckConnection())
            {
                try
                {
                    await _manager.AnswerInvite(token, group_id, accepted);

                    var dialog = new Android.App.AlertDialog.Builder(Context);
                    string message = string.Format("Вы {0} приглашение вступить в группу", accepted ? "приняли" : "отвергли");
                    dialog.SetMessage(message);
                    dialog.SetCancelable(false);
                    dialog.SetPositiveButton("Ок", delegate { Activity.Recreate(); });
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
                        (Activity as AddActivity).Finish();

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
            }
        }
    }
}