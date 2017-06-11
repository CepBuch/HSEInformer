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
using HSEInformer.Fragments;
using Android.Net;

namespace HSEInformer
{
    [Activity(Theme = "@style/AppTheme")]
    public class LoginActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LoginActivity);
            ShowLoginFragment();
        }

        private void ShowLoginFragment()
        {
            var trans = SupportFragmentManager.BeginTransaction();
            var emailFragment = LoginFragment.newInstance(string.Empty);
            trans.Replace(Resource.Id.LoginFragmentContainer, emailFragment);
            trans.Commit();
        }
        public override void OnBackPressed() { }

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
                dialog.SetPositiveButton("Ок", delegate { });
                dialog.Show();
                return false;
            }
        }
    }
}