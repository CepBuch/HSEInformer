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

namespace HSEInformer.GroupMemberViewModel
{
    class GroupMemberList
    {
        public List<User> Members { get; set; }

        public GroupMemberList(List<User> members)
        {
            Members = members;
        }

        public int NumMembers
        {
            get { return Members.Count; }
        }
        public User this[int i]
        {
            get { return Members[i]; }
        }
    }
}