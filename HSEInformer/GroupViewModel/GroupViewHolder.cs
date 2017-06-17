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

namespace HSEInformer.GroupViewModel
{
    class GroupViewHolder : RecyclerView.ViewHolder
    {
        public Button ContentButton;
        public GroupViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            ContentButton = itemView.FindViewById<Button>(Resource.Id.groupButton);
            ContentButton.Selected = true;
            ContentButton.Click += (o, e) => listener(base.AdapterPosition);
        }
    }
}