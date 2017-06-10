using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Content;
using Android.Preferences;
using Android.Net;
using Android.Support.V4.Widget;

namespace HSEInformer
{
    [Activity(MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        private SupportToolbar toolbar;
        private DrawerLayout drawerLayout;
        //private NavigationView navigationView;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            editor = prefs.Edit();
            editor.PutString("host", "http://hseinformerserver.azurewebsites.net");
            editor.Apply();

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
            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            SupportActionBar.SetHomeButtonEnabled(true);
            //SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
        }


        public bool CheckAuthorization()
        {
            var authorized = prefs.GetBoolean("authorized", false);
            var token = prefs.GetString("token", null);

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

