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
using HSEInformer.Model;

namespace HSEInformer.PostViewModel
{
    class PostsList
    {
        public List<Post> Posts { get; set; }

        public PostsList(List<Post> posts)
        {
            Posts = posts;
        }

        public int NumPosts
        {
            get { return Posts.Count; }
        }
        public Post this[int i]
        {
            get { return Posts[i]; }
        }
    }
}