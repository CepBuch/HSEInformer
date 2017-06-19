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
using HSEInformer.GroupMemberViewModel;
using Android.Support.V7.Widget;

namespace HSEInformer.InviteViewModek
{
    class InviteAdapter : RecyclerView.Adapter
    {
        public GroupMemberList _membersList;

        public event Action<Model.User> ItemClick;

        public InviteAdapter(GroupMemberList membersList)
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.InviteView, parent, false);
            rh = new InviteViewHolder(itemView, OnClick);
            return rh;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Modifying base view with exact data
            if (holder is InviteViewHolder)
            {
                InviteViewHolder rh = holder as InviteViewHolder;
                rh.MemberTextView.Text = $"{_membersList[position].Surname} {_membersList[position].Name[0]}. {_membersList[position].Patronymic}. ({_membersList[position].Email})";
            }
        }

        void OnClick(int postion)
        {
            ItemClick?.Invoke(_membersList[postion]);
        }
    }
}