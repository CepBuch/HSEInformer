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

namespace HSEInformer.Fragments
{
    public class RegistrationFragment : Android.Support.V4.App.Fragment
    {
        public static RegistrationFragment newInstance(User user, string code)
        {
            RegistrationFragment fragment = new RegistrationFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            args.PutString("name", user.Name);
            args.PutString("surname", user.Surname);
            args.PutString("patronymic", user.Patronymic);
            args.PutString("email", user.Email);
            return fragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var name = Arguments.GetString("name");
            var surname = Arguments.GetString("surname");
            var patronymic = Arguments.GetString("patronymic");
            var email = Arguments.GetString("email");

            Toast.MakeText(Context, $"{surname} {name} {patronymic} {email}", ToastLength.Long).Show();
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}