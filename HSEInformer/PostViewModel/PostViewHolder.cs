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

namespace HSEInformer.PostViewModel
{
    class PostViewHolder : RecyclerView.ViewHolder
    {
        public TextView HeaderTextView;
        public TextView FromTextView;
        public TextView TimeTextView;
        public TextView ContentTextView;
        public PostViewHolder(View itemView) : base(itemView)
        {
            HeaderTextView = itemView.FindViewById<TextView>(Resource.Id.post_header);
            TimeTextView = itemView.FindViewById<TextView>(Resource.Id.post_time);
            FromTextView = itemView.FindViewById<TextView>(Resource.Id.post_from);
            ContentTextView = itemView.FindViewById<TextView>(Resource.Id.post_content);
        }
    }
}