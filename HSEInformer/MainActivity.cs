using Android.App;
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
using Microsoft.AspNet.SignalR.Client;
using Java.Lang;
using System.Collections.Generic;

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
        Button sendButton;
        TextView lastMessageView;
        EditText messageText;
        string token;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();
            var host = "http://hseinformerserver.azurewebsites.net";
            _editor.PutString("host", host);
            _editor.Apply();

            SetContentView(Resource.Layout.Main);
            //------------------------------------------
            sendButton = FindViewById<Button>(Resource.Id.sendMessage);
            lastMessageView = FindViewById<TextView>(Resource.Id.lastMessage);
            messageText = FindViewById<EditText>(Resource.Id.textMessage);
            //--------------------------------------
            // Set our view from the "main" layout resource

            var authorized = CheckAuthorization();

            if (authorized)
            {
                CustomizeToolbarAndNavView();

                ConnectToHub(host);

                //Выбираем вкладку "Уведомления"

                //FillData();
            }
        }

        private async void ConnectToHub(string host)
        {
            //Путь к серверу
            var hubConnection = new HubConnection(host, new Dictionary<string, string> { { "Authorization", "Bearer " + token } });
            //Устанавливаем прокси-соединенние с хабом, который надо прослушивать (имя класса)
            var chatHubProxy = hubConnection.CreateHubProxy("ChatHub");

            ////Событие, которое будет возникать при получении сообщения 
            chatHubProxy.On<string, string>("UpdateChatMessage", (name, message) =>
            {
                this.RunOnUiThread(() =>
                {
                    lastMessageView.Text = message;
                });

            });



            //////По нажатии кнопки юзер отправляет сообщение
            sendButton.Click += async (o, e) =>
            {
                await chatHubProxy.Invoke("SendMessage", new object[] { "A: ", messageText.Text });
            };

            // Соединяемся
            try
            {
                await hubConnection.Start();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Не получается подсоединится к хабу: " + ex.Message, ToastLength.Long).Show();
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
            _navigationView.Menu.GetItem(1).SetChecked(true);
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
                        var dialog = new Android.App.AlertDialog.Builder(this);
                        string message = "Вы действительно хотите выйти?";
                        dialog.SetMessage(message);

                        dialog.SetPositiveButton("Да", delegate { LogOut(); });
                        dialog.SetNegativeButton("Нет", delegate { });
                        dialog.Show();
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
            token = _prefs.GetString("token", null);

            var connected = CheckConnection();

            if (connected && (!authorized || string.IsNullOrWhiteSpace(token)))
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

        public void LogOut()
        {
            //Удаляем все данные о пользователе
            _editor.PutBoolean("authorized", false);
            _editor.PutString("token", null);
            _editor.Apply();

            //Перезапускаем приложение
            this.Recreate();
        }
    }
}

