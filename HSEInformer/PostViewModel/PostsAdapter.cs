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
    class PostsAdapter : RecyclerView.Adapter
    {
        public PostsList _postsList;

        public PostsAdapter(PostsList postsList)
        {
            _postsList = postsList;

        }

        public override int ItemCount
        {
            get { return _postsList.NumPosts; }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder rh;
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PostView, parent, false);
            rh = new PostViewHolder(itemView);
            return rh;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Modifying base view with exact data
            if (holder is PostViewHolder)
            {
                PostViewHolder rh = holder as PostViewHolder;
                rh.HeaderTextView.Text = _postsList[position].Theme;
                rh.TimeTextView.Text = _postsList[position].Time.ToString();

                if (_postsList[position].User != null)
                {
                    var surname = $"{_postsList[position].User.Surname} ";
                    var name_init = !string.IsNullOrEmpty(_postsList[position].User.Name) ? $"{_postsList[position].User.Name[0]}." : null;
                    var patr_init = !string.IsNullOrEmpty(_postsList[position].User.Patronymic) ? $"{_postsList[position].User.Patronymic[0]}." : null;
                    var initials = $"{surname}{name_init}{patr_init} ({_postsList[position].User.Email})";

                    rh.FromTextView.Text = initials;
                    rh.ContentTextView.Text = _postsList[position].Content;
                }
            }
        }
    }
}