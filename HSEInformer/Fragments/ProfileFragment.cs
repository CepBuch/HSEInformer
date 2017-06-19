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

namespace HSEInformer.Fragments
{
    public class ProfileFragment : Android.Support.V4.App.Fragment
    {
        TextView nameTxt;
        TextView usernameTxt;
        TextView surnameTxt;
        TextView patrTxt;
        ApiManager _manager;
        ISharedPreferences _prefs;

        public static ProfileFragment newInstance()
        {
            ProfileFragment fragment = new ProfileFragment();
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
            var view = inflater.Inflate(Resource.Layout.ProfileFragment, container, false);
            usernameTxt = view.FindViewById<TextView>(Resource.Id.usenameTextView);
            nameTxt = view.FindViewById<TextView>(Resource.Id.nameTextView);
            surnameTxt = view.FindViewById<TextView>(Resource.Id.surnameTextView);
            patrTxt = view.FindViewById<TextView>(Resource.Id.patrTextView);
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            ShowProfile();
        }


        public async void ShowProfile()
        {
            var token = _prefs.GetString("token", null);

            if (token != null && (Activity as MainActivity).CheckConnection())
            {
                try
                {
                    var user = await _manager.GetProfile(token);
                    usernameTxt.Text = $"Логин: {user.Email}";
                    surnameTxt.Text = $"Фамилия: {user.Surname}";
                    nameTxt.Text = $"Имя: {user.Name}";
                    patrTxt.Text = $"Отчество: {user.Patronymic}";
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
                        (Activity as MainActivity).LogOut();
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