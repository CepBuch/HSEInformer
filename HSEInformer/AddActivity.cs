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
using Android.Support.V4.View;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Net;

namespace HSEInformer
{
    [Activity(Theme = "@style/AppTheme")]
    public class AddActivity : AppCompatActivity
    {

        SupportToolbar toolbar;
        ViewPager viewPager;
        ISharedPreferences prefs;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            SetContentView(Resource.Layout.AddActivity);
            CustomizeToolbarAndViewPager();
        }

        public void CustomizeToolbarAndViewPager()
        {
            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar_add);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            toolbarTitle.Text = "Добавление и поиск";

            viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
            viewPager.Adapter = new AddActivityPagerAdapter(SupportFragmentManager, this);
            TabLayout tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            tabLayout.SetupWithViewPager(viewPager);
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
    }
}