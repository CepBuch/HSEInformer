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
using HSEInformer.Model;

namespace HSEInformer.Fragments
{
    public class CodeFragment : Android.Support.V4.App.Fragment
    {
        Button backButton;
        Button confirmButton;
        EditText emailEditText;
        EditText codeEditText;
        ApiManager manager;
        ISharedPreferences _prefs;
        public static CodeFragment newInstance(string email)
        {
            CodeFragment fragment = new CodeFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            args.PutString("email", email);
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
            var username = Arguments.GetString("email");

            var view = inflater.Inflate(Resource.Layout.CodeFragment, container, false);

            backButton = view.FindViewById<Button>(Resource.Id.backButton);
            backButton.Click += (e, s) => FragmentManager.PopBackStack();

            confirmButton = view.FindViewById<Button>(Resource.Id.confirmButton);
            confirmButton.Click += ConfirmButton_Click;

            emailEditText = view.FindViewById<EditText>(Resource.Id.email_text);
            emailEditText.Text = username;

            codeEditText = view.FindViewById<EditText>(Resource.Id.code_text);
            codeEditText.TextChanged += (e, s) => confirmButton.Enabled = !string.IsNullOrWhiteSpace(codeEditText.Text);

            return view;
        }

        private async void ConfirmButton_Click(object sender, EventArgs e)
        {
            var dialog = new Android.App.AlertDialog.Builder(Context);
            if ((Activity as LoginActivity).CheckConnection())
            {
                EnableControls(false);
                try
                {
                    var email = emailEditText.Text;
                    var code = codeEditText.Text;
                    var member = await manager.ConfirmEmail(email, code);

                    if (member != null)
                    {
                        ShowRegistrationFragment(member, code);
                    }
                    else
                    {
                        string message = $"Код подтверждения был введен неверно.";
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
                codeEditText.Text = string.Empty;
                EnableControls(true);


            }
        }

        private void ShowRegistrationFragment(User user, string code)
        {
            var trans = FragmentManager.BeginTransaction();
            var registrationFragment = RegistrationFragment.newInstance(user, code);
            trans.Replace(Resource.Id.LoginFragmentContainer, registrationFragment);
            trans.AddToBackStack("CodeFragment");
            trans.Commit();
        }
        private void EnableControls(bool enable)
        {
            codeEditText.Enabled = enable;
            confirmButton.Visibility = enable ? ViewStates.Visible : ViewStates.Gone;
            backButton.Visibility = enable ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}