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
using Android.Net;

namespace HSEInformer.Fragments
{
    public class LoginFragment : Android.Support.V4.App.Fragment
    {

        Button loginButton;
        Button registerButton;
        EditText loginEditText;
        EditText passwordEditText;
        TextView orTextView;
        Spinner domainSpinner;
        ProgressBar loadBar;
        ApiManager manager;

        ISharedPreferences prefs;
        ISharedPreferencesEditor _editor;
        public static LoginFragment newInstance(string username)
        {
            LoginFragment fragment = new LoginFragment();
            Bundle args = new Bundle();
            args.PutString("username", username);

            fragment.Arguments = args;
            return fragment;
        }



        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);
            _editor = prefs.Edit();
            var host = prefs.GetString("host", null);
            manager = new ApiManager(host);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.LoginFragment, container, false);

            var username = Arguments.GetString("username");

            loginButton = view.FindViewById<Button>(Resource.Id.loginButton);
            loginButton.Click += LoginButton_Click;

            registerButton = view.FindViewById<Button>(Resource.Id.registerButton);
            registerButton.Click += RegisterButton_Click;


            loginEditText = view.FindViewById<EditText>(Resource.Id.login_text);
            loginEditText.Text = username ?? string.Empty;
            loginEditText.TextChanged += LoginTextView_TextChanged;

            passwordEditText = view.FindViewById<EditText>(Resource.Id.password_text);
            passwordEditText.TextChanged += LoginTextView_TextChanged;

            orTextView = view.FindViewById<TextView>(Resource.Id.orTextView);
            loadBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);


            domainSpinner = view.FindViewById<Spinner>(Resource.Id.domain_spinner);
            string[] domains = Resources.GetStringArray(Resource.Array.domains_array);
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleSpinnerItem, domains);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            domainSpinner.Adapter = adapter;
            domainSpinner.SetSelection(0);
            return view;
        }

        private void LoginTextView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            loginButton.Enabled = !string.IsNullOrWhiteSpace(loginEditText.Text) && !string.IsNullOrWhiteSpace(passwordEditText.Text);
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            var trans = FragmentManager.BeginTransaction();
            var confirmationFragment = ConfirmationFragment.newInstance();
            trans.Replace(Resource.Id.LoginFragmentContainer, confirmationFragment);
            trans.AddToBackStack("LoginFragment");
            trans.Commit();
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            if ((Activity as LoginActivity).CheckConnection())
            {
                
                EnableControls(false);
                try
                {
                    var domain = domainSpinner.SelectedItem.ToString();
                    var username = $"{loginEditText.Text}@{domain}";
                    var password = passwordEditText.Text;
                    var token = await manager.Login(username, MD5Converter.Convert(password));
                    _editor.PutString("token", token);
                    _editor.PutBoolean("authorized", true);
                    _editor.Apply();
                    ShowMainActiity();
                }
                catch (Exception ex)
                {
                    var dialog = new Android.App.AlertDialog.Builder(Context);
                    string message = ex.Message;
                    dialog.SetMessage(message);
                    dialog.SetMessage(message);
                    dialog.SetPositiveButton("Îê", delegate { });
                    dialog.Show();
                }
                loginEditText.Text = string.Empty;
                passwordEditText.Text = string.Empty;
                EnableControls(true);

            }
        }

        private void EnableControls(bool enable)
        {
            loginEditText.Enabled = enable;
            passwordEditText.Enabled = enable;
            domainSpinner.Enabled = enable;
            orTextView.Visibility = enable ? ViewStates.Visible : ViewStates.Gone;
            loginButton.Visibility = enable ? ViewStates.Visible : ViewStates.Gone;
            registerButton.Visibility = enable ? ViewStates.Visible : ViewStates.Gone;
            loadBar.Visibility = !enable ? ViewStates.Visible : ViewStates.Gone;
        }

        private void ShowMainActiity()
        {
            var intent = new Intent(Context, typeof(MainActivity));
            StartActivity(intent);
            Activity.Finish();
        }


    }
}