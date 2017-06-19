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
using Android.Support.V7.Widget;

namespace HSEInformer.RequestViewModel
{
    class RequestViewHolder : RecyclerView.ViewHolder
    {
        public TextView MemberTextView;
        public Button acceptButton;
        public Button declineButton;

        public RequestViewHolder(View itemView, Action<int,bool> listener) : base(itemView)
        {
            MemberTextView = itemView.FindViewById<TextView>(Resource.Id.memberTextView);
            acceptButton = itemView.FindViewById<Button>(Resource.Id.acceptButton);
            declineButton = itemView.FindViewById<Button>(Resource.Id.declineButton);
            acceptButton.Click += (e, s) => listener?.Invoke(AdapterPosition, true);
            declineButton.Click += (e, s) => listener?.Invoke(AdapterPosition, false);
        }
    }
}