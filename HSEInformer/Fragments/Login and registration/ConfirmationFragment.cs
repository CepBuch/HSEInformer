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

namespace HSEInformer.Fragments
{
    public class ConfirmationFragment : Android.Support.V4.App.Fragment
    {
        Button backButton;
        Spinner domainSpinner;
        Button sendButton;
        EditText emailEditText;
        ApiManager manager;
        ISharedPreferences _prefs;
        public static ConfirmationFragment newInstance()
        {
            ConfirmationFragment fragment = new ConfirmationFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);
            var host = _prefs.GetString("host", null);
            manager = new ApiManager(host);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var view = inflater.Inflate(Resource.Layout.ConfirmationFragment, container, false);
            backButton = view.FindViewById<Button>(Resource.Id.backButton);
            backButton.Click += (e, s) => FragmentManager.PopBackStack();

            sendButton = view.FindViewById<Button>(Resource.Id.sendButton);
            sendButton.Click += SendButton_Click;

            emailEditText = view.FindViewById<EditText>(Resource.Id.email_text);
            emailEditText.TextChanged += (e, s) => sendButton.Enabled = !string.IsNullOrWhiteSpace(emailEditText.Text);

            domainSpinner = view.FindViewById<Spinner>(Resource.Id.domain_spinner);
            string[] domains = Resources.GetStringArray(Resource.Array.domains_array);
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleSpinnerItem, domains);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            domainSpinner.Adapter = adapter;
            domainSpinner.SetSelection(0);

            return view;

        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            var dialog = new Android.App.AlertDialog.Builder(Context);
            if ((Activity as LoginActivity).CheckConnection())
            {
                EnableControls(false);
                try
                {
                    var domain = domainSpinner.SelectedItem.ToString();
                    var email = $"{emailEditText.Text}@{domain}";
                    var memberExists = await manager.SendConfirmationCode(email);

                    if(memberExists)
                    {
                        string message = $"Код подтверждения был выслан на почтовый адрес {email}";
                        dialog.SetMessage(message);
                        dialog.SetPositiveButton("Ок", delegate { ShowCodeFragment(email); });
                        dialog.Show();
                    }
                    else
                    {
                        string message = $"В базе данных не было найдено студента/сотрудника ВШЭ с почтовым адресом {email}";
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
                emailEditText.Text = string.Empty;
                EnableControls(true);


            }
        }

        private void EnableControls(bool enable)
        {
            emailEditText.Enabled = enable;
            domainSpinner.Enabled = enable;
            sendButton.Visibility = enable ? ViewStates.Visible : ViewStates.Gone;
            backButton.Visibility = enable ? ViewStates.Visible : ViewStates.Gone;
        }
        private void ShowCodeFragment(string email)
        {
            var trans = FragmentManager.BeginTransaction();
            var codeFragment = CodeFragment.newInstance(email);
            trans.Replace(Resource.Id.LoginFragmentContainer, codeFragment);
            trans.AddToBackStack("ConfirmationFragment");
            trans.Commit();

        }
    }
}