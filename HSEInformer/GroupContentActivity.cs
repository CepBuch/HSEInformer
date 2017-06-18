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
using Android.Preferences;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.View;
using Android.Net;
using Android.Support.Design.Widget;

namespace HSEInformer
{
    [Activity(Theme = "@style/AppTheme")]
    public class GroupContentActivity : AppCompatActivity
    {
        SupportToolbar toolbar;
        ViewPager viewPager;
        ISharedPreferences prefs;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            SetContentView(Resource.Layout.GroupContentActivity);

            var group_id = Intent.GetIntExtra("group_id", -1);

            if(group_id > 0)
            {
                CustomizeToolbarAndViewPager(group_id);
            }
        }

        public void CustomizeToolbarAndViewPager(int group_id)
        {
            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar_groups);
            TextView toolbarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            var group_name = Intent.GetStringExtra("group_name");

            if (!string.IsNullOrWhiteSpace(group_name))
                toolbarTitle.Text = group_name;
                
            viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
            viewPager.Adapter = new GroupContentPagerAdapter(SupportFragmentManager, group_id, this);
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