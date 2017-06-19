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
using Android.Preferences;

namespace HSEInformer.Fragments.AddAndSearch
{
    public class CreateGroupFragment : Android.Support.V4.App.Fragment
    {
        ISharedPreferences _prefs;
        ApiManager _manager;
        List<string> groups = new List<string>();
        EditText groupNameTxt;
        Button createButton;
        public static CreateGroupFragment newInstance()
        {
            CreateGroupFragment fragment = new CreateGroupFragment();
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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.CreateGroupFragment, container, false);
            groupNameTxt = view.FindViewById<EditText>(Resource.Id.groupNametxt);
            createButton = view.FindViewById<Button>(Resource.Id.createButton);

            groupNameTxt.TextChanged += (e, s) => createButton.Enabled = groupNameTxt.Text.Length >= 3;
            createButton.Click += CreateButton_Click;
            return view;
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            var newGroupName = groupNameTxt.Text;
            if (groups.Exists(g => g.ToLower() == newGroupName.ToLower()))
            {
                var dialog = new Android.App.AlertDialog.Builder(Context);
                string message = "Группа с таким названием уже существует";
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Ок", delegate { groupNameTxt.Text = string.Empty; });
                dialog.Show();
            }
            else
            {
                CreateGroup(newGroupName);
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            GetGroupNames();
        }
        public async void GetGroupNames()
        {
            var token = _prefs.GetString("token", null);

            if (token != null && (Activity as AddActivity).CheckConnection())
            {
                try
                {
                    groups = await _manager.GetGroupNames(token);

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



        public async void CreateGroup(string group_name)
        {
            var token = _prefs.GetString("token", null);

            if (token != null && (Activity as AddActivity).CheckConnection())
            {
                try
                {
                    await _manager.CreateGroup(token, group_name);
                    var dialog = new Android.App.AlertDialog.Builder(Context);
                    string message = $"Группа {group_name} была успешно создана";
                    dialog.SetMessage(message);
                    dialog.SetCancelable(false);
                    dialog.SetPositiveButton("Ок", delegate { Activity.Finish(); });
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