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

namespace HSEInformer.GroupViewModel
{
    public class GroupList
    {
        public List<Group> Groups { get; private set; }

        public GroupList(List<Group> groups)
        {
            Groups = groups;
        }

        public int NumGroups
        {
            get { return Groups.Count; }
        }
        public Group this[int i]
        {
            get { return Groups[i]; }
        }
    }
}