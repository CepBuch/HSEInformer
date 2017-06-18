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
    class GroupMemberAdapter : RecyclerView.Adapter
    {
        public GroupMemberList _membersList;

        public GroupMemberAdapter(GroupMemberList membersList)
        {
            _membersList = membersList;

        }

        public override int ItemCount
        {
            get { return _membersList.NumMembers; }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder rh;
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.GroupMemberView, parent, false);
            rh = new GroupMemberViewHolder(itemView);
            return rh;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Modifying base view with exact data
            if (holder is GroupMemberViewHolder)
            {
                GroupMemberViewHolder rh = holder as GroupMemberViewHolder;
                rh.NameTextView.Text = $"{_membersList[position].Surname} {_membersList[position].Name} {_membersList[position].Patronymic}";
                rh.UsernameTextView.Text = _membersList[position].Email;
            }
        }
    }
}