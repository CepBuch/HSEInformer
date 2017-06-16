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
using HSEInformer.Model;
using Android.Preferences;

namespace HSEInformer.Fragments
{
    public class RegistrationFragment : Android.Support.V4.App.Fragment
    {


        EditText passwordText;
        EditText passwordConfirmText;
        TextView emailTextView;
        TextView initialsTextView;
        TextView warningTextView;
        Button registerButton;
        ApiManager manager;
        ISharedPreferences _prefs;
        private string code;
        public static RegistrationFragment newInstance(User user, string code)
        {
            RegistrationFragment fragment = new RegistrationFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            args.PutString("name", user.Name);
            args.PutString("surname", user.Surname);
            args.PutString("patronymic", user.Patronymic);
            args.PutString("email", user.Email);
            args.PutString("code", code);
            return fragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            _prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);
            var host = _prefs.GetString("host", null);
            manager = new ApiManager(host);
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var name = Arguments.GetString("name");
            var surname = Arguments.GetString("surname");
            var patronymic = Arguments.GetString("patronymic");
            var email = Arguments.GetString("email");
            code = Arguments.GetString("code");


            var view = inflater.Inflate(Resource.Layout.RegistrationFragment, container, false);

            initialsTextView = view.FindViewById<TextView>(Resource.Id.initials_text);
            var nameFirstLetter = !string.IsNullOrEmpty(name) ? (name[0] + ".") : string.Empty;
            var patrFirstLetter = !string.IsNullOrEmpty(patronymic) ? (patronymic[0] + ".") : string.Empty;
            initialsTextView.Text = $"{surname} {nameFirstLetter}{patrFirstLetter}";

            emailTextView = view.FindViewById<TextView>(Resource.Id.email_text);
            emailTextView.Text = email;

            passwordText = view.FindViewById<EditText>(Resource.Id.password_text);
            passwordConfirmText = view.FindViewById<EditText>(Resource.Id.passwordconfirm_text);
            passwordConfirmText.TextChanged += PasswordText_TextChanged;
            passwordText.TextChanged += PasswordText_TextChanged;

            warningTextView = view.FindViewById<TextView>(Resource.Id.warning_text);

            registerButton = view.FindViewById<Button>(Resource.Id.registerButton);
            registerButton.Click += RegisterButton_Click;


            return view;
        }

        private void PasswordText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(passwordText.Text) && !string.IsNullOrWhiteSpace(passwordConfirmText.Text))
            {
                warningTextView.Visibility = passwordText.Text == passwordConfirmText.Text ? ViewStates.Invisible : ViewStates.Visible;
                registerButton.Enabled = passwordText.Text == passwordConfirmText.Text;
            }
            else
            {
                warningTextView.Visibility = ViewStates.Invisible;
                registerButton.Enabled = false;
            }
        }

        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            var dialog = new Android.App.AlertDialog.Builder(Context);
            if ((Activity as LoginActivity).CheckConnection())
            {
                EnableControls(false);
                try
                {
                    var username = emailTextView.Text;
                    var password = MD5Converter.Convert(passwordText.Text);
                    var confirmed_password = MD5Converter.Convert(passwordConfirmText.Text);
                    var registerStatus = await manager.Register(username, password, confirmed_password, code);

                    if (registerStatus)
                    {
                        string message = $"Пользователь {username} был успешно зарегистрирован";
                        dialog.SetMessage(message);
                        dialog.SetPositiveButton("Ок", delegate { Activity.Recreate(); });
                        dialog.Show();
                    }
                    else
                    {
                        string message = $"Ошибка ввода данных или такой пользователь уже существует";
                        dialog.SetMessage(message);
                        dialog.SetPositiveButton("Ок", delegate { });
                        dialog.Show();
                    }

                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    dialog.SetMessage(message);
                    dialog.SetPositiveButton("Ок", delegate { });
                    dialog.Show();
                }
                passwordText.Text = string.Empty;
                passwordConfirmText.Text = string.Empty;
                EnableControls(true);
            }
        }

        private void EnableControls(bool enable)
        {
            passwordConfirmText.Enabled = enable;
            passwordConfirmText.Enabled = enable;
            registerButton.Visibility = enable ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}