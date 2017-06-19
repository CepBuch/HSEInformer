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
using HSEInformer.Fragments;
using System;

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
        ApiManager _manager;

        string token;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _editor = _prefs.Edit();
            var host = "http://hseinformerserver.azurewebsites.net";
            _editor.PutString("host", host);
            _editor.Apply();
            _manager = new ApiManager(host);
            SetContentView(Resource.Layout.Main);

            var authorized = CheckAuthorization();

            if (authorized)
            {
                CustomizeToolbarAndNavView();
                ShowFeed();
            }
        }


        //private async void ConnectToHub(string host, string token)
        //{
        //    //Путь к серверу
        //    var hubConnection = new HubConnection(host, new Dictionary<string, string> { { "Authorization", "Bearer " + token } });

        //    //Токен
        //    hubConnection.Headers.Add("Authorization", "Bearer" + token);

        //    //Устанавливаем прокси-соединенние с хабом, который надо прослушивать (имя класса)
        //    var chatHubProxy = hubConnection.CreateHubProxy("PostHub");


        //    ////Событие, которое будет возникать при получении сообщения 
        //    chatHubProxy.On<string, string>("UpdateChatMessage", (name, message) =>
        //    {
        //        this.RunOnUiThread(() =>
        //        {
        //            Toast.MakeText(this, $"{name} : {message}", ToastLength.Long).Show();
        //        });

        //    });



        //    //Когда юзер отправит сообщение
        //    OnMessageSent += async (message) =>
        //     {
        //         await chatHubProxy.Invoke("SendMessage", new object[] { message });
        //     };

        //    // Соединяемся
        //    try
        //    {
        //        await hubConnection.Start();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Toast.MakeText(this, "Не получается подсоединится к хабу: " + ex.Message, ToastLength.Long).Show();
        //    }
        //}

        private void ShowFeed()
        {
            var trans = SupportFragmentManager.BeginTransaction();
            var categoriesContainerFragment = FeedFragment.newInstance(-1);
            trans.Replace(Resource.Id.mainFragmentContainer, categoriesContainerFragment);
            trans.Commit();
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
                        var intent = new Intent(this, typeof(AddActivity));
                        StartActivity(intent);
                        return true;
                    }
                case (Resource.Id.menu_message):
                    {
                        ShowGroupsList();
                        return true;
                    }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        async void ShowGroupsList()
        {
            var token = _prefs.GetString("token", null);

            if (token != null && CheckConnection())
            {
                var dialog = new Android.App.AlertDialog.Builder(this);

                try
                {

                    var permissions = await _manager.GetUserPostPermissions(token);

                    if (permissions != null && permissions.Count > 0)
                    {
                        dialog.SetTitle("Выберите группу");
                        ArrayAdapter<Model.Group> arrayAdapter = new ArrayAdapter<Model.Group>(this, Android.Resource.Layout.SelectDialogSingleChoice);
                        permissions.ForEach(p => arrayAdapter.Add(p));
                        dialog.SetAdapter(arrayAdapter, (o, e) =>
                        {
                            var selectedGroup = arrayAdapter.GetItem(e.Which);
                            OpenMesageInputWindow(token, selectedGroup);
                        });
                        dialog.SetNegativeButton("Отмена", delegate { });
                        dialog.Show();
                    }
                    else
                    {
                        string message = "К сожаелнию, на данный момент нет ни одной группы, куда вы могли бы сделать объявление";
                        dialog.SetMessage(message);
                        dialog.SetPositiveButton("Ок", delegate { });
                        dialog.Show();
                    }

                }
                catch (UnauthorizedAccessException)
                {
                    string message = "Ваши параметры авторизации устарели." +
                        "\nВы будете возвращены на страницу авторизации, чтобы пройти процедуру авторизации заново";
                    dialog.SetMessage(message);
                    dialog.SetCancelable(false);
                    dialog.SetPositiveButton("Ок", delegate
                    {
                        LogOut();

                    });
                    dialog.Show();
                }
                catch (System.Exception ex)
                {

                    string message = ex.Message;
                    dialog.SetMessage(message);
                    dialog.SetPositiveButton("Ок", delegate { });
                    dialog.Show();
                }
            }
        }
        void OpenMesageInputWindow(string token, Model.Group group)
        {
            var dialog = new Android.App.AlertDialog.Builder(this);
            dialog.SetTitle($"Отправка сообщения в группу {group?.Name}:");
            View viewInflated = LayoutInflater.From(this).Inflate(Resource.Layout.MessageLayout, null);
            var headerText = viewInflated.FindViewById<EditText>(Resource.Id.header);
            var contentText = viewInflated.FindViewById<EditText>(Resource.Id.input);
            dialog.SetView(viewInflated);
            dialog.SetPositiveButton("Отправить", delegate { SendPost(token, group, headerText.Text, contentText.Text); });
            dialog.SetNegativeButton("Отмена", delegate { });
            dialog.Show();
        }

        async void SendPost(string token, Model.Group group, string theme, string content)
        {
            if (CheckConnection())
            {
                var dialog = new Android.App.AlertDialog.Builder(this);

                try
                {
                    await _manager.SendMessage(token, theme, content, group.Id);
                    string message = "Сообщение было отправленно";
                    dialog.SetMessage(message);
                    dialog.SetPositiveButton("Ок", delegate { });
                    dialog.Show();


                }
                catch (UnauthorizedAccessException)
                {
                    string message = "Ваши параметры авторизации устарели." +
                        "\nВы будете возвращены на страницу авторизации, чтобы пройти процедуру авторизации заново";
                    dialog.SetMessage(message);
                    dialog.SetCancelable(false);
                    dialog.SetPositiveButton("Ок", delegate
                    {
                        LogOut();

                    });
                    dialog.Show();
                }
                catch (System.Exception ex)
                {

                    string message = ex.Message;
                    dialog.SetMessage(message);
                    dialog.SetPositiveButton("Ок", delegate { });
                    dialog.Show();
                }
            }
            ShowFeed();
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
                        var trans = SupportFragmentManager.BeginTransaction();
                        var categoriesContainerFragment = ProfileFragment.newInstance();
                        trans.Replace(Resource.Id.mainFragmentContainer, categoriesContainerFragment);
                        trans.Commit();
                        break;
                        break;
                    }
                case Resource.Id.nav_feed:
                    {
                        ShowFeed();
                        break;
                    }
                case Resource.Id.nav_groups:
                    {
                        var trans = SupportFragmentManager.BeginTransaction();
                        var categoriesContainerFragment = GroupsFragment.newInstance(false);
                        trans.Replace(Resource.Id.mainFragmentContainer, categoriesContainerFragment);
                        trans.Commit();
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
            else return true;
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

