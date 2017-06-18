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

namespace HSEInformer.GroupMemberViewModel
{
    class GroupMemberViewHolder : RecyclerView.ViewHolder
    {
        public TextView NameTextView;
        public TextView UsernameTextView;

        public GroupMemberViewHolder(View itemView) : base(itemView)
        {
            NameTextView = itemView.FindViewById<TextView>(Resource.Id.member_name);
            UsernameTextView = itemView.FindViewById<TextView>(Resource.Id.member_username);
        }
    }
}