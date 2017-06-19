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

namespace HSEInformer.InviteViewModek
{
    class InviteViewHolder : RecyclerView.ViewHolder
    {
        public Button InviteButton;
        public TextView MemberTextView;
        public InviteViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            MemberTextView = itemView.FindViewById<TextView>(Resource.Id.memberTextView);
            InviteButton = itemView.FindViewById<Button>(Resource.Id.inviteButton);
            InviteButton.Click += (e, s) => listener?.Invoke(AdapterPosition);
        }
    }
}