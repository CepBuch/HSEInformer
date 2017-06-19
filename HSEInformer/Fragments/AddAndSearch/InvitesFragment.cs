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
using Android.Support.V7.Widget;

namespace HSEInformer.Fragments.AddAndSearch
{
    public class InvitesFragment : Android.Support.V4.App.Fragment
    {
        RecyclerView recyclerView;
        RecyclerView.LayoutManager layoutManager;
        ISharedPreferences prefs;
        ApiManager _manager;


        public static InvitesFragment newInstance()
        {
            InvitesFragment fragment = new InvitesFragment();
            Bundle args = new Bundle();
            fragment.Arguments = args;
            return fragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.InvitesFragment, container, false);
        }
    }
}