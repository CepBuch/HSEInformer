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

namespace HSEInformer.InviteResponseViewModel
{
    class InviteResponseViewHolder : RecyclerView.ViewHolder
    {
        public TextView GroupTextView;
        public Button AcceptButton;
        public Button DeclineButton;

        public InviteResponseViewHolder(View itemView, Action<int, bool> listener) : base(itemView)
        {
            GroupTextView = itemView.FindViewById<TextView>(Resource.Id.groupTextView);
            AcceptButton = itemView.FindViewById<Button>(Resource.Id.acceptButton);
            DeclineButton = itemView.FindViewById<Button>(Resource.Id.declineButton);
            AcceptButton.Click += (e, s) => listener?.Invoke(AdapterPosition, true);
            DeclineButton.Click += (e, s) => listener?.Invoke(AdapterPosition, false);
        }
    }
}