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
using HSEInformer.GroupMemberViewModel;

namespace HSEInformer.RequestViewModel
{
    class RequestAdapter : RecyclerView.Adapter
    {
        public GroupMemberList _membersList;

        public event Action<Model.User, bool> ItemClick;

        public RequestAdapter(GroupMemberList membersList)
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.RequestView, parent, false);
            rh = new RequestViewHolder(itemView, OnClick);
            return rh;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Modifying base view with exact data
            if (holder is RequestViewHolder)
            {
                RequestViewHolder rh = holder as RequestViewHolder;
                
            }
        }

        void OnClick(int postion, bool accepted)
        {
            ItemClick?.Invoke(_membersList[postion], accepted);
        }
    }
}