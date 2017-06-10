﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Content;
using Android.Preferences;
using Android.Net;
using Android.Support.V4.Widget;
using Android.Views;

namespace HSEInformer
{
    [Activity(MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {

        ISharedPreferences _prefs;
        ISharedPreferencesEditor _editor;
        SupportToolbar _toolbar;
        DrawerLayout _drawerLayout;
        NavigationView _navigationView;
        ActionBarDrawerToggle _drawerToggle;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();
            _editor.PutString("host", "http://hseinformerserver.azurewebsites.net");
            _editor.Apply();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            var authorized = CheckAuthorization();

            if (authorized)
            {
                CustomizeToolbarAndNavView();

                FillData();
            }
        }

        public void FillData()
        {

        }

        public void CustomizeToolbarAndNavView()
        {
            //Accepting toolbar and drawerlayout
            _toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(_toolbar);

            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _drawerToggle = new ActionBarDrawerToggle(this, _drawerLayout,
                Resource.String.openDrawer, Resource.String.closeDrawer);
            _drawerLayout.AddDrawerListener(_drawerToggle);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            _drawerToggle.SyncState();
            _navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            _navigationView.NavigationItemSelected += _navigationView_NavigationItemSelected;
        }



        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            _navigationView.InflateMenu(Resource.Menu.nav_menu);
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);

            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            _drawerToggle.OnOptionsItemSelected(item);
            switch (item.ItemId)
            {
                case (Resource.Id.menu_plus):
                    {
                        //TODO: Open add activity
                        return true;
                    }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void _navigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.nav_exit:
                    {
                        //var dialog = new Android.App.AlertDialog.Builder(this);
                        //dialog.SetMessage(GetString(Resource.String.logout_message));

                        //dialog.SetPositiveButton("Yes", delegate { LogOut(); });
                        //dialog.SetNegativeButton("Cancel", delegate { });
                        //dialog.Show();
                        //break;
                        Toast.MakeText(this, "Выход", ToastLength.Long).Show();
                        break;
                    }
                case Resource.Id.nav_profile:
                    {
                        Toast.MakeText(this, "Профиль", ToastLength.Long).Show();
                        break;
                    }
                case Resource.Id.nav_content:
                    {
                        Toast.MakeText(this, "Новости", ToastLength.Long).Show();

                        break;
                    }
                case Resource.Id.nav_deadlines:
                    {
                        Toast.MakeText(this, "Дедлайны", ToastLength.Long).Show();
                        break;
                    }
            }
            e.MenuItem.SetChecked(true);
            _drawerLayout.CloseDrawers();
        }

        public bool CheckAuthorization()
        {
            var authorized = _prefs.GetBoolean("authorized", false);
            var token = _prefs.GetString("token", null);

            return true;
            CheckConnection();

            if (!authorized || string.IsNullOrWhiteSpace(token))
            {
                var intet = new Intent(this, typeof(LoginActivity));
                StartActivity(intet);
                this.Finish();
                return false;
            }
            else
            {
                //TODO: check token
                return true;
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
                string title = "Нет интернет-соединения";
                dialog.SetTitle(title);
                dialog.SetMessage(message);

                dialog.SetCancelable(false);
                dialog.SetMessage(message);
                dialog.SetPositiveButton("Ok", delegate { this.Finish(); });
                dialog.Show();
                return false;
            }
        }
    }
}
