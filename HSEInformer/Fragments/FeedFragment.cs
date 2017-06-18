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
using HSEInformer.Model;
using Android.Support.V7.Widget;
using HSEInformer.PostViewModel;

namespace HSEInformer.Fragments
{
    public class FeedFragment : Android.Support.V4.App.Fragment
    {
        private static string GROUP_ID = "group_id";
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        ISharedPreferences prefs;
        PostsList postsList;
        PostsAdapter postsAdapter;
        ApiManager _manager;
        ProgressBar progressBar;
        public static FeedFragment newInstance(int group_id)
        {
            FeedFragment fragment = new FeedFragment();
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
            postsList = new PostsList(new List<Model.Post>());
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.FeedFragment, container, false);
            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);
            postsAdapter = new PostsAdapter(postsList);
            recyclerView.SetAdapter(postsAdapter);
            layoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            var group_id = Arguments.GetInt(GROUP_ID);
            if (group_id > 0)
            {
                ShowPosts(group_id);
            }
        }

        public async void ShowPosts(int group_id)
        {
            var token = prefs.GetString("token", null);

            if (token != null && (Activity as GroupContentActivity).CheckConnection())
            {
                try
                {
                    progressBar.Visibility = ViewStates.Visible;
                    recyclerView.Visibility = ViewStates.Gone;
                    var posts = await _manager.GetPosts(token, group_id);
                    if (posts != null)
                    {
                        postsList.Posts = posts;
                        postsAdapter.NotifyDataSetChanged();
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
                    recyclerView.Visibility = ViewStates.Visible;
                }
            }
        }
    }
}